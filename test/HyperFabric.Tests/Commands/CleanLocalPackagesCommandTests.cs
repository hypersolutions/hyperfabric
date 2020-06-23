using System;
using System.IO;
using System.Threading.Tasks;
using HyperFabric.Commands;
using HyperFabric.Tests.Builders;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Commands
{
    public class CleanLocalPackagesCommandTests : CommandTestBase
    {
        [Fact]
        public async Task WithOutputDir_RunAsync_DeletesFolder()
        {
            var outputDir = await CreateDirectoryAsync();
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithTempDir(outputDir)
                .Build();
            
            var command = new CleanLocalPackagesCommand(
                new CommandContext{Manifest = manifest, Logger = Logger.Object});
            
            await command.RunAsync();
            
            Directory.Exists(outputDir).ShouldBeFalse();
        }
        
        [Fact]
        public async Task WithOutputDir_RunAsync_LogsSuccess()
        {
            var outputDir = await CreateDirectoryAsync();
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithTempDir(outputDir)
                .Build();
            var command = new CleanLocalPackagesCommand(
                new CommandContext{Manifest = manifest, Logger = Logger.Object});
            
            await command.RunAsync();
            
            ShouldContainsLogMessage("Local folder has been cleaned");
        }
        
        [Fact]
        public async Task WithUnknownOutputDir_RunAsync_LogsFailure()
        {
            var outputDir = $"TMP{DateTime.Now:HHmmssFFF}";
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithTempDir(outputDir)
                .Build();
            var command = new CleanLocalPackagesCommand(
                new CommandContext{Manifest = manifest, Logger = Logger.Object});
            
            await command.RunAsync();
            
            ShouldContainsLogMessage("Local folder has failed with error:");
        }

        private static async Task<string> CreateDirectoryAsync()
        {
            var outputDir = $"TMP{DateTime.Now:HHmmssFFF}";
            Directory.CreateDirectory(outputDir);
            await Task.Delay(250);
            return outputDir;
        }
    }
}
