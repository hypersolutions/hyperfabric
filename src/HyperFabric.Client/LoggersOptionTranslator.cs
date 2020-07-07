using System;
using System.Linq;
using HyperOptions.Translators;

namespace HyperFabric.Client
{
    public sealed class LoggersOptionTranslator : ITranslator<string[]>
    {
        public string[] Translate(string value)
        {
            return value?
                .Trim()
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .ToArray() ?? new string[0];
        }
    }
}
