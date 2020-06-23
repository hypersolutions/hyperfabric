using System;
using System.Threading.Tasks;
using HyperFabric.Extensions;
using HyperFabric.Logging;

namespace HyperFabric.Commands
{
    public sealed class CopyImageToStoreCommand : Command, ICommand
    {
        private readonly CommandContext _context;
        private readonly ICommand _innerCommand;

        public CopyImageToStoreCommand(CommandContext context, ICommand innerCommand) : base(context.Logger)
        {
            _context = context;
            _innerCommand = innerCommand;
        }
        
        public async Task RunAsync()
        {
            var item = _context.CurrentDeploymentItem;
            LogInfo(
                StageTypes.Deployment, 
                $"Copying package {item.PackagePath.GetDirectoryName()} to the image store...");
            
            try
            {
                await _context.ImgStoreClient.UploadApplicationPackageAsync(item.PackagePath, item.CompressPackage);
                LogInfo(
                    StageTypes.Deployment, 
                    $"Copied the package {item.PackagePath.GetDirectoryName()} to the image store");
            }
            catch (Exception error)
            {
                LogError(
                    StageTypes.Deployment,
                    $"Failed to copy {item.PackagePath.GetDirectoryName()} to the image store: {error.Message}");
            }

            await _innerCommand.RunAsync();
        }
    }
}
