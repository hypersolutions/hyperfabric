using System.Threading.Tasks;
using HyperFabric.Builders;
using HyperFabric.Client.Options;

namespace HyperFabric.Client
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var options = CommandLineHelper<DeploymentOptions>
                .FromArgs(args, "HyperFabric", "Commandline parallel deployment tool for service fabric.")
                .For(o => o.Json).IsRequired("Json string or file path for the manifest.", "-j", "--json")
                .For(o => o.Loggers).IsOptional(
                    "Comma-separated list of loggers to use e.g. Console, File.", "-l", "--loggers")
                .WithOptionHandler(new JsonOptionHandler())
                .WithOptionHandler(new LoggersOptionHandler())
                .Parse();

            var exitCode = -1;
            
            if (options != null)
            {
                var manifestBuilder = new ManifestBuilder();
                var manifest = string.IsNullOrWhiteSpace(options.JsonString)
                    ? manifestBuilder.FromFile(options.JsonPath)
                    : manifestBuilder.FromString(options.JsonString);
                
                var success = await DeploymentService.RunAsync(manifest, options.LoggerList);
                exitCode = success ? 0 : -1;
            }
            
            return exitCode;
        }
    }
}
