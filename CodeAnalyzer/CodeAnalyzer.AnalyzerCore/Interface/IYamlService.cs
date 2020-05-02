using System.Collections.Generic;

namespace CodeAnalyzer.AnalyzerCore.Interface
{
    public interface IYamlService
    {
        public List<string> ReadYamlFileRootNodeChildrenToString(string path);
    }
}