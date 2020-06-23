using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HyperFabric.Core;
using HyperFabric.Logging;
using HyperFabric.Tasks;

namespace HyperFabric.Commands
{
    public sealed class CopyPackagesLocallyCommand : ICommand
    {
        private readonly ICommand _innerCommand;
        private readonly IEnumerable<DeploymentItem> _deploymentItems;
        private readonly string _tempDir;
        private readonly ILogger _logger;

        public CopyPackagesLocallyCommand(CommandContext context, ICommand innerCommand)
        {
            _innerCommand = innerCommand;
            _logger = context.Logger;
            _deploymentItems = context.Manifest.Groups.SelectMany(g => g.Items);
            _tempDir = context.Manifest.Options.TempFullPath;
        }

        public async Task RunAsync()
        {
            _logger.Log(new LogMessage(
                StageTypes.Deployment, 
                $"Coping packages to the local working directory...", 
                LogLevelTypes.Ok));
            
            var items = _deploymentItems.Select(di => di).ToList();
            var copyTask = new CopyPackagesLocalConcurrentTask(_tempDir, items, items.Count, _logger);
            await copyTask.StartAsync();

            if (!copyTask.HasErrors)
                await _innerCommand.RunAsync();
        }
    }
}
