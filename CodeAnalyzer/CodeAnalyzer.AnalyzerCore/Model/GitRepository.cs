namespace CodeAnalyzer.AnalyzerCore.Model
{
    public class GitRepository
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string RepoDir { get; set; }
        public PomFile PomFile { get; set; }
    }
}