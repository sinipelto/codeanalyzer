using System;
using System.Net;
using CodeAnalyzer.AnalyzerCore.Interface;

namespace CodeAnalyzer.AnalyzerCore.Client
{
    public class ConnectionClient : IConnectionClient
    {
        private readonly WebHeaderCollection _headers = new WebHeaderCollection
        {
            { HttpRequestHeader.Accept, "*/*"},
            { HttpRequestHeader.AcceptCharset, "*" },
            { HttpRequestHeader.AcceptEncoding, "*" },
            { HttpRequestHeader.AcceptLanguage, "*" },
            { HttpRequestHeader.UserAgent, "Mozilla/5.0 (CodeAnalyzer.ConnectionClient.WebClient 1.0)" },
        };

        public bool EnsureConnectionToUri(string uri)
        {
            try
            {
                using var client = new WebClient {Headers = _headers};
                client.DownloadData(uri);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }   
    }
}