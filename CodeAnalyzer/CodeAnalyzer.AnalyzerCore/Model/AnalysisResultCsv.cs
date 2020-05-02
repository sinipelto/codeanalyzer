using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CodeAnalyzer.AnalyzerCore.Service;

namespace CodeAnalyzer.AnalyzerCore.Model
{
    public class AnalysisResultCsv
    {
        public static List<AnalysisResultCsv> FromJsonClass(AnalysisResult src)
        {
            return src.Issues.Select(issue => new AnalysisResultCsv
                {
                    ProjectName = src.Components.FirstOrDefault()?.Key.Split(":")[1],
                    CreationDate = issue.CreationDate,
                    CreationCommitHash = issue.Hash,
                    Type = issue.Type,
                    Squid = issue.Rule,
                    Component = issue.Component,
                    Severity = issue.Severity,
                    StartLine = issue.TextRange.StartLine,
                    EndLine = issue.TextRange.EndLine,
                    Resolution = issue.Resolution,
                    Status = issue.Status,
                    Message = issue.Message,
                    Effort = issue.Effort,
                    Debt = issue.Debt,
                    Author = issue.Author
                })
                .ToList();
        }

        public static List<AnalysisResultCsv> FromXmlFile(XDocument xmlSrc, string projectName, Tool tool)
        {
            List<AnalysisResultCsv> results = new List<AnalysisResultCsv>();

            string xmlTagName;
            switch (tool)
            {
                case Tool.Checkstyle:
                    xmlTagName = "error"; break;
                case Tool.PMD:
                    xmlTagName = "violation"; break;
                default:
                    xmlTagName = "error"; break;
            }
            
            var elemList = from tag in xmlSrc.Descendants() where tag.Name.LocalName == xmlTagName select tag;
            
            foreach (XElement elem in elemList)
            {
                AnalysisResultCsv csvRow = new AnalysisResultCsv();

                csvRow.ProjectName = projectName;
                csvRow.CreationDate = DateTime.Now;

                switch (tool)
                {
                    case Tool.Checkstyle:
                        csvRow.StartLine = Int32.Parse(elem.Attribute("line").Value);
                        csvRow.EndLine = csvRow.StartLine;
                        csvRow.Severity = elem.Attribute("severity").Value;
                        csvRow.Message = elem.Attribute("message").Value;
                        break;
                    case Tool.PMD:
                        csvRow.StartLine = Int32.Parse(elem.Attribute("beginline").Value);
                        csvRow.EndLine = Int32.Parse(elem.Attribute("endline").Value);
                        csvRow.Severity = elem.Attribute("priority").Value;
                        csvRow.Message = elem.Value;
                        csvRow.Type = elem.Attribute("ruleset").Value;
                        csvRow.Component = "Class: " + elem.Attribute("class").Value + ", Method: " + elem.Attribute("method").Value;
                        break;
                }
                results.Add(csvRow);
            }
            return results;
        }

        public string ProjectName { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreationCommitHash { get; set; }
        public string Type { get; set; }
        public string Squid { get; set; }
        public string Component { get; set; }
        public string Severity { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public string Resolution { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string Effort { get; set; }
        public string Debt { get; set; }
        public string Author { get; set; }
    }
}
