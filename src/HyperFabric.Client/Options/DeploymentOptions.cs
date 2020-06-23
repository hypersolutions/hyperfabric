// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace HyperFabric.Client.Options
{
    public sealed class DeploymentOptions 
    {
        public string Json { get; set; }
        
        public string Loggers { get; set; }

        internal string JsonString { get; set; }

        internal string JsonPath { get; set; }
        
        internal string[] LoggerList { get; set; }
    }
}
