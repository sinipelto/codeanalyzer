using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeAnalyzer.AnalyzerCore.Interface;
using YamlDotNet.RepresentationModel;

namespace CodeAnalyzer.AnalyzerCore.Service
{
    public class YamlService : IYamlService
    {
        public List<string> ReadYamlFileRootNodeChildrenToString(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using var reader = new StreamReader(path);

            var yaml = new YamlStream();

            yaml.Load(reader);

            var mapping = (YamlSequenceNode)yaml.Documents[0].RootNode;

            var urls = mapping.Children;

            return urls.Select(url => url.ToString()).ToList();
        }
    }
}