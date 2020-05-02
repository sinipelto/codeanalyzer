using System.Collections.Generic;

namespace CodeAnalyzer.AnalyzerCore.Interface
{
    public interface ICommitService
    {
        List<string> FetchAllCommits();
    }
}