using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using CodeAnalyzer.AnalyzerCore.Interface;
using CodeAnalyzer.AnalyzerCore.Model;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace CodeAnalyzer.AnalyzerCore.Service
{
    public enum Tool
    {
        SonarQube,
        Checkstyle,
        PMD
    }

    public class MavenService : IMavenService
    {
        private const string MvnSonarCmd = "clean sonar:sonar";
        private const string MvnCsCmd = "checkstyle:checkstyle";
        private const string MvnPmdCmd = "pmd:pmd";
        private string _mvnArgs = "";

        public void Configure(MvnConfiguration args)
        {
            if (args?.SonarHost != null)
            {
                _mvnArgs = "-Dsonar.host.url=" + args.SonarHost;
            }
        }

        public bool PerformScan(string path, out ProcessStandard result, Tool tool)
        {
            var MvnCmd = GetScanCmd(tool);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                result = ExecuteMavenWindows(MvnCmd, path);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                result = ExecuteMavenLinux(MvnCmd, path);
            }
            else
            {
                throw new ApplicationException("Invalid OS Platform in use. Cannot proceed.");
            }

            // Using shell execute on Linux side => no output, must assume success
            if (result == null) return true;

            return !(result.Error.Length > 0 && result.Output.Contains("[ERROR]"));
        }
        private string GetScanCmd(Tool tool)
        {
            string cmd;
            switch (tool)
            {
                case Tool.SonarQube:
                { cmd = MvnSonarCmd; break; }
                case Tool.Checkstyle:
                { cmd = MvnCsCmd; break; }
                case Tool.PMD:
                { cmd = MvnPmdCmd; break; }
                default:
                { cmd = MvnSonarCmd; break; }
            }
            return cmd;
        }

        private static ProcessStandard ExecuteMavenWindows(string cmd, string workingDir)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C mvn " + cmd,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDir
                }
            };

            proc.Start();

            var res = proc.StandardOutput.ReadToEnd();
            var err = proc.StandardError.ReadToEnd();

            proc.WaitForExit();

            return new ProcessStandard { Output = res, Error = err};
        }        
        
        private ProcessStandard ExecuteMavenLinux(string cmd, string workingDir)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = "/usr/share/maven/bin",
                    FileName = "mvn",
                    Arguments = "-f " + Path.Combine(workingDir, "pom.xml") + " " + cmd + " " + _mvnArgs,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                }
            };

            proc.Start();
            proc.WaitForExit();

            return null;
        }

    }
}