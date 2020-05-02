using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CodeAnalyzer.AnalyzerCore.Interface;
using CodeAnalyzer.AnalyzerCore.Model;
using Newtonsoft.Json;

namespace CodeAnalyzer.AnalyzerCore.Client
{
    public class CommitClient : ICommitClient
    {
        private static readonly HttpClient Client = new HttpClient();

        private static readonly string UserAgent = "Dotnet CodeAnalyzer";

        private static readonly string BaseUrl = "https://api.github.com";
        public string RepoUrl { get; set; }

        public async Task<List<CommitResult>> GetAllCommits()
        {
            var commitResults = new List<CommitResult>();
            Console.WriteLine("GETting all commits...");

            try
            {
                Client.DefaultRequestHeaders.Add("User-Agent", UserAgent); // User-Agent is required for GitHub api
                var jsonResult = await Client.GetStringAsync(BaseUrl + RepoUrl + "/commits");
                commitResults = JsonConvert.DeserializeObject<List<CommitResult>>(jsonResult);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message: {0} ", e.Message);
            }

            return commitResults;
        }

        public Task<List<CommitResult>> GetLatestCommits(int nLatestCommits)
        {
            throw new NotImplementedException();
        }

        public async Task GetAllBranches()
        {
            Console.WriteLine("GETting all branches...");

            try
            {
                Client.DefaultRequestHeaders.Add("User-Agent", UserAgent); // User-Agent is required for GitHub api
                var responseBody = await Client.GetStringAsync(BaseUrl + RepoUrl + "/branches");
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message: {0} ", e.Message);
            }
        }
    }
}