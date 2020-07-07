using System.Threading.Tasks;
using HyperFabric.Builders;
using HyperOptions;

namespace HyperFabric.Client
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var result = new CommandLineParser<DeploymentOptions>(
                    "HyperFabric", "Commandline parallel deployment tool for service fabric.")
                .Set(o => o.Json).AsRequired(
                    "Json string or file path for the manifest.", "-j", "--json")
                .Set(o => o.Loggers).AsOptional<LoggersOptionTranslator>(
                    "Comma-separated list of loggers to use e.g. Console, File.", "-l", "--loggers")
                .Set(o => o.Help, true).AsOptional(
                    "Help options", "-?", "--help")
                .Parse(args);

            var exitCode = -1;
            
            if (result.HasOptions)
            {
                var manifestBuilder = new ManifestBuilder();
                var manifest = string.IsNullOrWhiteSpace(result.Options.JsonString)
                    ? manifestBuilder.FromFile(result.Options.JsonPath)
                    : manifestBuilder.FromString(result.Options.JsonString);
                
                var success = await DeploymentService.RunAsync(manifest, result.Options.Loggers);
                exitCode = success ? 0 : -1;
            }
            
            return exitCode;
        }
    }
}
