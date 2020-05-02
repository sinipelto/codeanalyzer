namespace CodeAnalyzer.AnalyzerCore.Interface
{
    public interface IConnectionService
    {
        public bool EnsureConnectionToUri(string uri);
    }
}