using System;
using System.Xml.Linq;

namespace HyperFabric.Extensions
{
    public static class XElementExtensions
    {
        public static T GetAttributeValue<T>(this XElement element, string name)
        {
            var value = element.Attribute(name)?.Value;

            if (value == null) return default(T);

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
