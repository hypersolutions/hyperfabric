using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace HyperFabric.Extensions
{
    public static class StringExtensions
    {
        private static readonly XNamespace _fabricNamespace = XNamespace.Get(FabricXmlNamespace);
        private const string FabricXmlNamespace = "http://schemas.microsoft.com/2011/01/fabric";
        
        public static string GetFabricAttributeValue(this string path, string elementName, string attributeName)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;
            
            var document = XDocument.Load(path);
            var applicationElement = document.FindElement(_fabricNamespace + elementName);
            return applicationElement?.GetAttributeValue<string>(attributeName);
        }
        
        public static IReadOnlyDictionary<string, string> GetFabricParameterDictionary(this string parameterFile)
        {
            var parameters = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(parameterFile) || !File.Exists(parameterFile)) return parameters;
            
            var document = XDocument.Load(parameterFile);
            
            foreach (var parameterElement in document.FindElements(_fabricNamespace + "Parameter"))
            {
                var name = parameterElement.GetAttributeValue<string>("Name");
                var value = parameterElement.GetAttributeValue<string>("Value");
                parameters.Add(name, value);
            }

            return parameters;
        }

        public static string GetDirectoryName(this string path)
        {
            return new DirectoryInfo(path).Name;
        }
    }
}
