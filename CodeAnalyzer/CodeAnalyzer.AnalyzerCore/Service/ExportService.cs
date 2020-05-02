using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CodeAnalyzer.AnalyzerCore.Interface;
using CodeAnalyzer.AnalyzerCore.Model;
using CsvHelper;
using CsvHelper.Configuration;

namespace CodeAnalyzer.AnalyzerCore.Service
{
    public class ExportService : IExportService
    {
        private readonly CsvConfiguration _configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            Encoding = Encoding.UTF8,
            HasHeaderRecord = true
        };

        public void CreateResultCsv(string csvPath)
        {
            File.Create(csvPath).Close();

            using var stream = new StreamWriter(csvPath);
            using var writer = new CsvWriter(stream, _configuration);

            writer.WriteHeader<AnalysisResult>();
        }

        public void ExportAnalysisResultsToCsv<T>(Dictionary<string, List<T>> analysisData, string csvPath)
        {
            // Ensure csv file exists and free resource
            File.Create(csvPath).Close();

            // Open streamwriter and initialize csv writer
            using var stream = new StreamWriter(csvPath);
            using var writer = new CsvWriter(stream, _configuration);

            // Write header by given class
            writer.WriteHeader<T>();
            writer.NextRecord();

            // Write the csv records
            foreach (var data in analysisData)
            {
                writer.WriteRecords(data.Value);
            }
        }

        private void SeekRecords(string path, int num)
        {
            using var stream = new StreamWriter(path);
            using var writer = new CsvWriter(stream, _configuration);

            for (var i = 0; i < num; i++)
            {
                writer.NextRecord();
            }

            stream.Close();
        }
    }
}