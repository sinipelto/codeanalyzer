namespace CodeAnalyzer.AnalyzerCore.Interface
{
    public interface IAnalysisClient
    {
        public void SetServer(string url);

        public string GetReportForProjectJson(string componentKeys);

        public string GetReportForCommitsJson(string projectName);
    }
}