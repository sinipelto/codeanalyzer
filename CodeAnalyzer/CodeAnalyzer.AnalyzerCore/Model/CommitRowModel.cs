using System;
using System.Collections.Generic;
using System.Text;

namespace CodeAnalyzer.AnalyzerCore.Model
{
    public class CommitRowModel
    {
        public string ProjectID { get; set; }
        public string CommitHash { get; set; }
        public string CommitMessage { get; set; }
        public string Author { get; set; }
        public DateTime AuthorDate { get; set; }
        public TimeSpan AuthorTimezone { get; set; }
        public string Committer { get; set; }
        public DateTime CommitterDate { get; set; }
        public TimeSpan CommitterTimezone{ get; set; }
        public List<string> Branches { get; set; }
        public bool InMainBranch { get; set; }
        public bool Merge { get; set; }
        public List<string> Parents { get; set; }
    }
}
