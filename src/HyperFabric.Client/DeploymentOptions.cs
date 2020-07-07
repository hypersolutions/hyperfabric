// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace HyperFabric.Client
{
    public sealed class DeploymentOptions 
    {
        public string Json { get; set; }
        
        public string[] Loggers { get; set; }

        public string Help { get; set; }

        internal string JsonString => GetJsonString(Json);

        internal string JsonPath => GetJsonPath(Json);
        
        private static string GetJsonString(string json)
        {
            return json.StartsWith("{") && json.EndsWith("}") ? json : null;
        }
        
        private static string GetJsonPath(string json)
        {
            return !json.StartsWith("{") ? json : null;
        }
    }
}
