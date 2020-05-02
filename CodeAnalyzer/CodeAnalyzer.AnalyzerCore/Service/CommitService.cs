using CodeAnalyzer.AnalyzerCore.Interface;
using CodeAnalyzer.AnalyzerCore.Model;
using LibGit2Sharp;
using System.Collections.Generic;
using System.Linq;

namespace CodeAnalyzer.AnalyzerCore.Service
{
    public class CommitService : ICommitService
    {
        public CommitService(string repoDir, string repoName)
        {
            _currentRepoDir = repoDir;
            _currentRepoName = repoName;
        }

        public CommitCsvModel CreateCommitCsvModel()
        {
            var commitCsv = new CommitCsvModel();

            using (var repo = new Repository(_currentRepoDir))
            {
                foreach (var commit in repo.Commits)
                {
                    var csvRow = new CommitRowModel
                    {
                        ProjectID = _currentRepoName,
                        CommitHash = commit.Sha,
                        CommitMessage = commit.Message,
                        Author = commit.Author.Name,
                        AuthorDate = commit.Author.When.DateTime,
                        AuthorTimezone = commit.Author.When.Offset,
                        Committer = commit.Committer.Name,
                        CommitterTimezone = commit.Committer.When.Offset,
                        Branches = GetBranchesAsString(repo.Branches, commit)
                    };

                    csvRow.InMainBranch = csvRow.Branches.Contains("master");  //TODO: What if main branch is not master?
                    csvRow.Merge = commit.Parents.Count() > 1;
                    csvRow.Parents = GetParentsAsString(commit.Parents.ToList());

                    commitCsv.CommitRows.Add(csvRow);
                }
            }
            return commitCsv;
        }

        public List<string> FetchAllCommits()
        {
            var commitMsgList = new List<string>();
            using (var repo = new Repository(_currentRepoDir))
            {
                var commits = repo.Commits;
                var sortedCommits = commits.OrderBy(o => o.Author.When.ToLocalTime()).ToList();

                commitMsgList.AddRange(sortedCommits.Select(c => c.Message));
            }
            return commitMsgList;
        }

        private static List<string> GetBranchesAsString(BranchCollection branches, GitObject commit)
        {
            return (from branch in branches where branch.Commits.Any(c => c.Sha == commit.Sha) select branch.FriendlyName).ToList();
        }

        private static List<string> GetParentsAsString(IEnumerable<LibGit2Sharp.Commit> commitList)
        {
            return commitList.Select(commit => commit.Sha).ToList();
        }

        private readonly string _currentRepoDir;
        private readonly string _currentRepoName;
    }
}