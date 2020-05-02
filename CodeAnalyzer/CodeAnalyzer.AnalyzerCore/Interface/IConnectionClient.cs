namespace CodeAnalyzer.AnalyzerCore.Interface
{
    public interface IConnectionClient
    {
        public bool EnsureConnectionToUri(string uri);
    }
}