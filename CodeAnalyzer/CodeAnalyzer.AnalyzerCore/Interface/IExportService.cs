using System.Collections.Generic;

namespace CodeAnalyzer.AnalyzerCore.Interface
{
    public interface IExportService
    {
        public void CreateResultCsv(string csvPath);

        public void ExportAnalysisResultsToCsv<T>(Dictionary<string, List<T>> analysisData, string csvPath);
    }
}