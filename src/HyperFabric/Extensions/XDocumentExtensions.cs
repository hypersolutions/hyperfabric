using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HyperFabric.Extensions
{
    public static class XDocumentExtensions
    {
        public static XElement FindElement(this XDocument document, XName name)
        {
            return document.Descendants().FirstOrDefault(e => e.Name == name);
        }
        
        public static IEnumerable<XElement> FindElements(this XDocument document, XName name)
        {
            return document.Descendants().Where(e => e.Name == name);
        }
    }
}
