using System.Collections.Generic;
using CodeAnalyzer.AnalyzerCore.Model;

namespace CodeAnalyzer.AnalyzerCore.Interface
{
    public interface IRepoService
    {
        public List<GitRepository> EnumerateRepositories(List<string> repoUrls);

        public PomFile FetchPomDetails(string projectPath);

        public void DownloadRepository(GitRepository repo);

        public void CloneRepository(string repoUrl, string repoName);

        public void CleanAllRepositories();
    }
}