using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CodeAnalyzer.AnalyzerCore.Interface;
using CodeAnalyzer.AnalyzerCore.Model;
using Newtonsoft.Json;

namespace CodeAnalyzer.AnalyzerCore.Service
{
    public class AnalysisService : IAnalysisService
    {
        private IAnalysisClient AnalysisClient { get; }

        public AnalysisService(IAnalysisClient analysisClient)
        {
            AnalysisClient = analysisClient ?? throw new ArgumentNullException(nameof(analysisClient));
        }

        public void ChangeServer(string url)
        {
            AnalysisClient.SetServer(url);
        }

        public AnalysisResult GetAnalysisResult(string componentKeys)
        {
            var raw = AnalysisClient.GetReportForProjectJson(componentKeys);

            return JsonConvert.DeserializeObject<AnalysisResult>(raw);
        }

        public XDocument GetXmlAnalysisResult(string path, Tool tool)
        {
            var xmlResult = new XDocument();
            var resultFileName = "";

            switch (tool)
            {
                case Tool.Checkstyle:
                    resultFileName = "checkstyle-result.xml"; break;
                case Tool.PMD: 
                    resultFileName = "pmd.xml"; break;
            }

            List<string> resultFiles = Directory.GetFiles(path, resultFileName, SearchOption.AllDirectories).ToList();

            if (resultFiles.Any())
            {
                xmlResult = XDocument.Load(resultFiles.First());

                if (resultFiles.Count == 1) { return xmlResult; }

                for (int i = 1; i < resultFiles.Count; i++)
                {
                    var nextDoc = XDocument.Load(resultFiles[i]);
                    xmlResult = new XDocument(xmlResult.Descendants("AllNodes").Concat(nextDoc.Descendants("AllNodes")));
                }
            }
            return xmlResult;
        }
    }
}