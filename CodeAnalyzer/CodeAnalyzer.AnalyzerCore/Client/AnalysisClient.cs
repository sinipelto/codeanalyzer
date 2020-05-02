using CodeAnalyzer.AnalyzerCore.Interface;
using System.Net;

namespace CodeAnalyzer.AnalyzerCore.Client
{
    public class AnalysisClient : IAnalysisClient
    {
        private string _server = "http://localhost:9000";

        private const string BaseApi = "/api";
        private const string IssueSearchApi = "/issues/search";

        private const string CompKeysQuery = "componentKeys=";

        // Example queries:
        // http://localhost:9000/api/issues/search?componentKeys=com.mycompany.app:my-app
        // http://localhost:9000/api/issues/search?componentKeys=groupId:artifactId

        public void SetServer(string url)
        {
            _server = url;
        }

        public string GetReportForProjectJson(string componentKeys)
        {
            var query = "?" + CompKeysQuery + componentKeys;
            var url = _server + BaseApi + IssueSearchApi + query;
            
            using var client = new WebClient();
            
            var res = client.DownloadString(url);

            return res;
        }

        public string GetReportForCommitsJson(string projectName)
        {
            throw new System.NotImplementedException();
        }
    }
}