using System.Threading.Tasks;
using HyperFabric.Builders;
using HyperFabric.Core;
using HyperFabric.Factories;
using HyperFabric.Logging;
using HyperFabric.Validation;

namespace HyperFabric
{
    public static class DeploymentService
    {
        public static async Task<bool> RunAsync(Manifest manifest, string[] loggers)
        {
            using var logger = CreateLogger(loggers);
            
            if (ProcessManifest(manifest, logger))
            {
                await RunCommandsAsync(manifest, logger);
            }

            return !logger.HasErrors;
        }

        private static ILogger CreateLogger(string[] loggers)
        {
            var loggingFactory = new LoggingFactory();
            return loggingFactory.Create(loggers);
        }
        
        private static bool ProcessManifest(Manifest manifest, ILogger logger)
        {
            logger.Log(new LogMessage(StageTypes.Preparation, "Processing the manifest", LogLevelTypes.Ok));
            var validators = new IValidator[]
            {
                new ManifestValidator(),
                new ClusterValidator(),
                new DeploymentOptionsValidator(),
                new DeploymentGroupValidator(),
                new DeploymentItemValidator()
            };
            var manifestHandlerFactory = new ManifestHandlerFactory(logger);
            var handler = manifestHandlerFactory.Create(validators);
            return handler.Handle(manifest);
        }

        private static async Task RunCommandsAsync(Manifest manifest, ILogger logger)
        {
            logger.Log(new LogMessage(
                StageTypes.Preparation, "About to run the deployment on the manifest...", LogLevelTypes.Ok));
            var commandFactory = await CreateDeploymentCommandFactoryAsync(manifest, logger);

            if (commandFactory != null)
            {
                var command = commandFactory.Create(manifest);
                await command.RunAsync();
            }
        }
        
        private static async Task<IDeploymentCommandFactory> CreateDeploymentCommandFactoryAsync(
            Manifest manifest,
            ILogger logger)
        {
            var clientBuilder = new FabricClientBuilder(logger);
            var fabricClient = await clientBuilder.BuildAsync(manifest);

            if (fabricClient != null)
            {
                logger.Log(new LogMessage(
                    StageTypes.Preparation, 
                    "Created a fabric client to connect to the cluster", 
                    LogLevelTypes.Ok));
            }
            else
            {
                logger.Log(new LogMessage(
                    StageTypes.Preparation, 
                    "Failed to create a fabric client to connect to the cluster", 
                    LogLevelTypes.Error));
            }
            
            return fabricClient != null ? new DeploymentCommandFactory(fabricClient, logger) : null;
        }
    }
}
