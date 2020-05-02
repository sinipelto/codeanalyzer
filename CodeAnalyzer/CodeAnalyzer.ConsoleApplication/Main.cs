using CodeAnalyzer.AnalyzerCore.Client;
using CodeAnalyzer.AnalyzerCore.Interface;
using CodeAnalyzer.AnalyzerCore.Model;
using CodeAnalyzer.AnalyzerCore.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace CodeAnalyzer.ConsoleApplication
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                new Application().Execute(null);
            }
            else
            {
                var server = args[0];

                // Ensure valid ip address given.
                try
                {
                    IPAddress.Parse(server);
                }
                catch (Exception)
                {
                    Console.WriteLine("WARNING: Invalid IP address provided! Assuming hostname.");
                }

                new Application().Execute("http://" + server + ":9000");
            }
        }
    }

    internal class Application
    {
        // Paths for static files and directories
        private static readonly string CurPath = Directory.GetCurrentDirectory();
        private static readonly string TmpPath = Path.GetTempPath();

        private static readonly string DataPath = Path.Combine(CurPath, "Data");

        private static readonly string OpPath = Path.Combine(TmpPath, "sonartemp");
        private static readonly string OutPath = Path.Combine(OpPath, "output");

        private static readonly string RepoYmlPath = Path.Combine(DataPath, "repositories.yml");
        private static readonly string SonarqubeAnalysisPath = Path.Combine(OutPath, "analysis_sonarqube.csv");
        private static readonly string CheckstyleAnalysisPath = Path.Combine(OutPath, "analysis_checkstyle.csv");
        private static readonly string PmdAnalysisPath = Path.Combine(OutPath, "analysis_pmd.csv");

        // Services
        private readonly IAnalysisService _analysisService = new AnalysisService(new AnalysisClient());
        private readonly IMavenService _mavenService = new MavenService();
        private readonly IRepoService _repoService = new RepoService(OpPath);
        private readonly IYamlService _yamlService = new YamlService();
        private readonly IExportService _exportService = new ExportService();
        private readonly IConnectionService _connectionService = new ConnectionService();

        // ReSharper kitisee constista mutta älä muuta constiksi, muuten vetää koodit harmaaksi!
        // Do we remove, clone and build all repositories or do we use existing downloads (true/false)?
        private readonly bool _freshDir = true;

        public void Execute(string urlArg)
        {
            // Use either localhost or user provided sonarqube address
            var sonarServer = (string.IsNullOrWhiteSpace(urlArg) ? null : urlArg) ?? "http://localhost:9000";

            _analysisService.ChangeServer(sonarServer);
            _mavenService.Configure(new MvnConfiguration{SonarHost = sonarServer});

            // Check that HTTP connection are ok
            if (!_connectionService.EnsureConnectionToUri("https://api.github.com"))
            {
                Console.WriteLine("Could not connect to github. Check internet connection.");
                return;
            }
            Console.WriteLine("Connection to Github api OK.");

            if (!_connectionService.EnsureConnectionToUri(sonarServer + "/api/webservices/list"))
            {
                Console.WriteLine("Could not connect to Sonarqube server. Check Sonarqube instance is running.");
                return;
            }
            Console.WriteLine("Connection to Sonarqube server OK.");

            // Ensure operation directory exists
            // MUST EXIST BEFORE CLEANUP!!!
            Directory.CreateDirectory(OpPath);
            Console.WriteLine("Operation directory ensured created.");

            // Clean any old repositories
            if (_freshDir)
            {
                _repoService.CleanAllRepositories();
                Console.WriteLine("Cleaned any previous repositories.");
            }

            if (!File.Exists(RepoYmlPath))
            {
                Console.WriteLine("Please create repositories.yml file with correct github repo url entries.");
                throw new FileNotFoundException("repositories.yml file not found in: " + RepoYmlPath);
            }

            // Enumerate git repository urls to clone
            var repoUrls = _yamlService.ReadYamlFileRootNodeChildrenToString(RepoYmlPath);

            // Enumerate repositories to fetch
            var repos = _repoService.EnumerateRepositories(repoUrls);
            Console.WriteLine("Enumerated downloadable repositories.");
            
            // Ensure necessary directories exist
            Directory.CreateDirectory(OutPath);
            Console.WriteLine("All necessary subdirectories created.");

            // Keep count of current repo
            var i = 0;
            
            // Download and extract the repositories
            foreach (var repo in repos)
            {
                i++;

                Console.WriteLine($"Processing repo {i}/{repos.Count}..");

                if (_freshDir)
                {
                    _repoService.CloneRepository(repo.Url, repo.Name);
                    Console.WriteLine("Cloned repo: " + repo.Name);
                }

                repos[i-1].RepoDir = Path.Combine(OpPath, repo.Name);
                repos[i-1].PomFile = _repoService.FetchPomDetails(repo.RepoDir);

                // ********* Commits start
                var commitService = new CommitService(repo.RepoDir, repo.Name);
                var commitCsv = commitService.CreateCommitCsvModel();
                var commitPath = Path.Combine(OutPath, "commitReport_" + repo.Name + ".csv");
                
                var commitData = new Dictionary<string, List<CommitRowModel>>
                {
                    {repo.PomFile.Name, new List<CommitRowModel>(commitCsv.CommitRows)}
                };

                _exportService.ExportAnalysisResultsToCsv(commitData, commitPath);
                // ********* Commits end

                if (!_freshDir) continue;

                Console.WriteLine("Running SonarQube..");
                var sonarResult = _mavenService.PerformScan(repo.RepoDir, out var cmdResult, Tool.SonarQube);
                Console.WriteLine("SonarQube commands ran.");
                Console.WriteLine("Running Checkstyle..");
                var checkstyleResult = _mavenService.PerformScan(repo.RepoDir, out var cmdCsResult, Tool.Checkstyle);
                Console.WriteLine("Checkstyle commands ran.");
                Console.WriteLine("Running PMD..");
                var pmdResult = _mavenService.PerformScan(repo.RepoDir, out var cmdPmdResult, Tool.PMD);
                Console.WriteLine("PMD commands ran.");

                Console.WriteLine("Detecting errors..");
                if (!checkstyleResult)
                {
                    Console.WriteLine("ERROR: Checkstyle analysis failed.");
                    Console.WriteLine("STANDARD OUTPUT: " + cmdCsResult.Output);
                    Console.WriteLine("STANDARD ERROR OUTPUT: " + cmdCsResult.Error);
                }

                if (!pmdResult)
                {
                    Console.WriteLine("ERROR: PMD analysis failed.");
                    Console.WriteLine("STANDARD OUTPUT: " + cmdPmdResult.Output);
                    Console.WriteLine("STANDARD ERROR OUTPUT: " + cmdPmdResult.Error);
                }
                if (!sonarResult)
                {
                    Console.WriteLine("ERROR: Failed to build maven project.");
                    Console.WriteLine("STANDARD OUTPUT: " + cmdResult.Output);
                    Console.WriteLine("STANDARD ERROR OUTPUT: " + cmdResult.Error);
                    Console.WriteLine("Moving to next repository..");
                    continue;
                }
                Console.WriteLine("Error detection done.");

                Console.WriteLine("All analysis commands ran for project.");
            }

            Console.WriteLine("Giving enough time for SonarQube to analyze the projects..");
            Thread.Sleep(8000);

            // Create data collection for the analysis results to write
            // Dictionary (ProjectID - List of results)
            var analysisData = new Dictionary<string, List<AnalysisResultCsv>>();
            var checkstyleAnalysisData = new Dictionary<string, List<AnalysisResultCsv>>();
            var pmdAnalysisData = new Dictionary<string, List<AnalysisResultCsv>>();

            // Collect analysis results for all repos
            i = 0;
            foreach (var repo in repos)
            {
                i++;
                Console.WriteLine($"Processing repo {i}/{repos.Count}..");

                var result = _analysisService.GetAnalysisResult(repo.PomFile.GroupId + ":" + repo.PomFile.ArtifactId);
                var checkstyleResult = _analysisService.GetXmlAnalysisResult(repo.RepoDir, Tool.Checkstyle);
                var pmdResult = _analysisService.GetXmlAnalysisResult(repo.RepoDir, Tool.PMD);
                Console.WriteLine("Repository code analysis data collected from all tools.");

                // Add the collected results to analysis collection
                analysisData.Add(repo.PomFile.Name, AnalysisResultCsv.FromJsonClass(result));
                checkstyleAnalysisData.Add(repo.PomFile.Name, AnalysisResultCsv.FromXmlFile(checkstyleResult, repo.PomFile.Name, Tool.Checkstyle));
                pmdAnalysisData.Add(repo.PomFile.Name, AnalysisResultCsv.FromXmlFile(pmdResult, repo.PomFile.Name, Tool.PMD));
            }

            // Export analysis results to CSV file
            _exportService.ExportAnalysisResultsToCsv(analysisData, SonarqubeAnalysisPath);
            _exportService.ExportAnalysisResultsToCsv(checkstyleAnalysisData, CheckstyleAnalysisPath);
            _exportService.ExportAnalysisResultsToCsv(pmdAnalysisData, PmdAnalysisPath);
            Console.WriteLine("Analysis results for project exported to CSV file.");

            Console.WriteLine("Successfully completed all operations.");
        }
    }
}
