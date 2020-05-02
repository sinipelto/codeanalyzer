using System.Xml.Linq;
using CodeAnalyzer.AnalyzerCore.Model;
using CodeAnalyzer.AnalyzerCore.Service;

namespace CodeAnalyzer.AnalyzerCore.Interface
{
    public interface IAnalysisService
    {
        public void ChangeServer(string url);

        public AnalysisResult GetAnalysisResult(string componentKeys);

        public XDocument GetXmlAnalysisResult(string path, Tool tool);
    }
}