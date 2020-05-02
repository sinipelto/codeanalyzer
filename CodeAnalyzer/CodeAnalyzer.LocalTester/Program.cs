using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using CodeAnalyzer.AnalyzerCore.Client;
using CodeAnalyzer.AnalyzerCore.Model;
using CodeAnalyzer.AnalyzerCore.Service;

namespace CodeAnalyzer.LocalTester
{
    internal class Program
    {
        private static readonly string TestPath = Path.Combine(Path.GetTempPath(), "sonar");

        static void Main(string[] args)
        {
            Console.WriteLine("TEST PROGRAM START");

            var commitTester = new CommitTester();
            commitTester.TestCommitListing();

            /*
            var repo = new RepoService(TestPath);
            var maven = new MavenService();
            var anclient = new AnalysisClient();
            var analyser = new AnalysisService(anclient);

            var target = new GitRepository
            {
                Name = "simple-java-maven-app",
                Url = "https://github.com/jenkins-docs/simple-java-maven-app",
                PomFile = new PomFile{Name = "my-app"},
            };

            var repoDir = Path.Combine(TestPath, target.Name + "-master");

            // Ensure test dir exists
            Directory.CreateDirectory(TestPath);

            // Check test dir contents
            PrintFilesInPath(TestPath);

            // Clean any old downloaded repos
            repo.CleanAllRepositories();

            // Check clean was ok
            PrintFilesInPath(TestPath);

            // Download test repo
            repo.DownloadRepository(target);
            
            // Check downloaded files
            PrintFilesInPath(TestPath);

            // Perform sonar scan to the test repo
            var success = maven.PerformSonarScan(repoDir, out var cmdResult);

            Console.WriteLine("COMMAND OUTPUT: " + cmdResult.Output);
            Console.WriteLine();
            Console.WriteLine("COMMAND ERROR: " + cmdResult.Error);

            // Ensure no errors in scan
            if (!success)
            {
                Console.WriteLine("ERROR while processing Maven build.");
                return;
            }

            // Check again directory listing
            PrintFilesInPath(TestPath);

            // TEST: get raw json result from Sonarqube server
            var raw = anclient.GetReportForProjectJson(target.PomFile.Name);

            // Get analysis results from Sonarqube server
            var result = analyser.GetAnalysisResult(target.PomFile.Name);

            Console.WriteLine("REPORT: " + raw);
            */

            Console.WriteLine("TEST PROGRAM END");
        }

        private static void PrintFilesInPath(string path)
        {
            Console.WriteLine("**********************************************************");
         
            Console.WriteLine("Files in " + path + ":");

            foreach (var entry in Directory.EnumerateFileSystemEntries(path))
            {
                Console.WriteLine("File: " + entry);
            }

            Console.WriteLine("**********************************************************");
        }
    }
}
