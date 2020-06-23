using System;
using System.IO;
using System.Threading.Tasks;
using HyperFabric.Logging;

namespace HyperFabric.Commands
{
    public sealed class CleanLocalPackagesCommand : Command, ICommand
    {
        private readonly string _tempDir;
        
        public CleanLocalPackagesCommand(CommandContext context) : base(context.Logger)
        {
            _tempDir = context.Manifest.Options.TempFullPath;
        }

        public async Task RunAsync()
        {
            try
            {
                if (!await TryDeleteLocalPackages(_tempDir))
                {
                    LogError(StageTypes.Cleanup, "Unexpected error occurred cleaning the local folder");
                }
                else
                {
                   LogInfo(StageTypes.Cleanup, "Local folder has been cleaned");
                }
            }
            catch (Exception error)
            {
                var errorMessage = error.Message;
                LogError(StageTypes.Cleanup, $"Local folder has failed with error: {errorMessage}");
            }
        }

        private static async Task<bool> TryDeleteLocalPackages(string tempDir)
        {
            Directory.Delete(tempDir, true);
            var startTime = DateTime.Now;

            while (Directory.Exists(tempDir))
            {
                await Task.Delay(100);

                var currentTime = DateTime.Now;

                if (currentTime > startTime.AddSeconds(60))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
