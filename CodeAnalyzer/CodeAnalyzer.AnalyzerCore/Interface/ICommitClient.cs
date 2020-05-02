using System.Collections.Generic;
using System.Threading.Tasks;
using CodeAnalyzer.AnalyzerCore.Model;

namespace CodeAnalyzer.AnalyzerCore.Interface
{
    internal interface ICommitClient
    {
        public Task<List<CommitResult>> GetAllCommits();
        public Task<List<CommitResult>> GetLatestCommits(int nLatestCommits);
        public Task GetAllBranches();
    }
}