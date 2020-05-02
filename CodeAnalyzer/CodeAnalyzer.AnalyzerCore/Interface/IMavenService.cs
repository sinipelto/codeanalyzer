using CodeAnalyzer.AnalyzerCore.Model;
using CodeAnalyzer.AnalyzerCore.Service;

namespace CodeAnalyzer.AnalyzerCore.Interface
{
    public interface IMavenService
    {
        public void Configure(MvnConfiguration args);

        public bool PerformScan(string path, out ProcessStandard result, Tool tool);
    }
}