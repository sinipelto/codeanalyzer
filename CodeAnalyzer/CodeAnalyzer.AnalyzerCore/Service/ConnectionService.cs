using CodeAnalyzer.AnalyzerCore.Client;
using CodeAnalyzer.AnalyzerCore.Interface;

namespace CodeAnalyzer.AnalyzerCore.Service
{
    public class ConnectionService : IConnectionService
    {
        private readonly IConnectionClient _connectionClient = new ConnectionClient();

        public bool EnsureConnectionToUri(string uri)
        {
            return _connectionClient.EnsureConnectionToUri(uri);
        }
    }
}