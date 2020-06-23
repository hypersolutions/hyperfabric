using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HyperFabric.Integration.Tests
{
    public static class OutputParser
    {
        public static IList<TestLogMessage> Parse(string path)
        {
            return JsonSerializer.Deserialize<IList<TestLogMessage>>(File.ReadAllText(path));
        }
    }
}
