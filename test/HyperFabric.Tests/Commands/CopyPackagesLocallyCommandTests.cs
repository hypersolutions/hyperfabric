using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HyperFabric.Commands;
using HyperFabric.Core;
using HyperFabric.Tests.Builders;
using Moq;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Commands
{
    public class CopyPackagesLocallyCommandTests : CommandTestBase
    {
        [Fact]
        public async Task SinglePackageInManifest_RunAsync_CopiesAppToTargetFolder()
        {
            using var outputDir = new TempOutputDir();
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithTempDir(outputDir.TempDir)
                .WithGroup(new DeploymentItem
                {
                    PackagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1")
                })
                .Build();
            var command = new CopyPackagesLocallyCommand(
                new CommandContext{Manifest = manifest, Logger = Logger.Object}, InnerCommand.Object);
            
            await command.RunAsync();

            DirectoryExists(outputDir.TempDir, "App1");
            FileExists(outputDir.TempDir, "App1", "ApplicationManifest.xml");
            DirectoryExists(outputDir.TempDir, "App1", "StatusServicePkg");
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "ServiceManifest.xml");
            DirectoryExists(outputDir.TempDir, "App1", "StatusServicePkg", "Code");
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "Code", "StatusService.dll");
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "Code", "StatusService.exe");
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "Code", "StatusService.pdb");
            DirectoryExists(outputDir.TempDir, "App1", "StatusServicePkg", "Config");    
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "Config", "Settings.xml");
        }
        
        [Fact]
        public async Task MultiplePackagesInSingleManifestGroup_RunAsync_CopiesAppsToTargetFolder()
        {
            using var outputDir = new TempOutputDir();
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithTempDir(outputDir.TempDir)
                .WithGroup(
                    new DeploymentItem
                    {
                        PackagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1")
                    },
                    new DeploymentItem
                    {
                        PackagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App2")
                    })
                .Build();
            var command = new CopyPackagesLocallyCommand(
                new CommandContext{Manifest = manifest, Logger = Logger.Object}, InnerCommand.Object);
            
            await command.RunAsync();

            DirectoryExists(outputDir.TempDir, "App1");
            FileExists(outputDir.TempDir, "App1", "ApplicationManifest.xml");
            DirectoryExists(outputDir.TempDir, "App1", "StatusServicePkg");
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "ServiceManifest.xml");
            DirectoryExists(outputDir.TempDir, "App1", "StatusServicePkg", "Code");
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "Code", "StatusService.dll");
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "Code", "StatusService.exe");
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "Code", "StatusService.pdb");
            DirectoryExists(outputDir.TempDir, "App1", "StatusServicePkg", "Config");    
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "Config", "Settings.xml");
            
            DirectoryExists(outputDir.TempDir, "App2");
            FileExists(outputDir.TempDir, "App2", "ApplicationManifest.xml");
            DirectoryExists(outputDir.TempDir, "App2", "StatusServicePkg");
            FileExists(outputDir.TempDir, "App2", "StatusServicePkg", "ServiceManifest.xml");
            DirectoryExists(outputDir.TempDir, "App2", "StatusServicePkg", "Code");
            FileExists(outputDir.TempDir, "App2", "StatusServicePkg", "Code", "StatusService.dll");
            FileExists(outputDir.TempDir, "App2", "StatusServicePkg", "Code", "StatusService.exe");
            FileExists(outputDir.TempDir, "App2", "StatusServicePkg", "Code", "StatusService.pdb");
            DirectoryExists(outputDir.TempDir, "App2", "StatusServicePkg", "Config");    
            FileExists(outputDir.TempDir, "App2", "StatusServicePkg", "Config", "Settings.xml");
        }
        
        [Fact]
        public async Task MultiplePackagesInMultipleManifestGroups_RunAsync_CopiesAppsToTargetFolder()
        {
            using var outputDir = new TempOutputDir();
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithTempDir(outputDir.TempDir)
                .WithGroup(
                    new DeploymentItem
                    {
                        PackagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1")
                    })
                .WithGroup(
                    new DeploymentItem
                    {
                        PackagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App2")
                    })
                .Build();
            var command = new CopyPackagesLocallyCommand(
                new CommandContext{Manifest = manifest, Logger = Logger.Object}, InnerCommand.Object);
            
            await command.RunAsync();

            DirectoryExists(outputDir.TempDir, "App1");
            FileExists(outputDir.TempDir, "App1", "ApplicationManifest.xml");
            DirectoryExists(outputDir.TempDir, "App1", "StatusServicePkg");
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "ServiceManifest.xml");
            DirectoryExists(outputDir.TempDir, "App1", "StatusServicePkg", "Code");
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "Code", "StatusService.dll");
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "Code", "StatusService.exe");
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "Code", "StatusService.pdb");
            DirectoryExists(outputDir.TempDir, "App1", "StatusServicePkg", "Config");    
            FileExists(outputDir.TempDir, "App1", "StatusServicePkg", "Config", "Settings.xml");
            
            DirectoryExists(outputDir.TempDir, "App2");
            FileExists(outputDir.TempDir, "App2", "ApplicationManifest.xml");
            DirectoryExists(outputDir.TempDir, "App2", "StatusServicePkg");
            FileExists(outputDir.TempDir, "App2", "StatusServicePkg", "ServiceManifest.xml");
            DirectoryExists(outputDir.TempDir, "App2", "StatusServicePkg", "Code");
            FileExists(outputDir.TempDir, "App2", "StatusServicePkg", "Code", "StatusService.dll");
            FileExists(outputDir.TempDir, "App2", "StatusServicePkg", "Code", "StatusService.exe");
            FileExists(outputDir.TempDir, "App2", "StatusServicePkg", "Code", "StatusService.pdb");
            DirectoryExists(outputDir.TempDir, "App2", "StatusServicePkg", "Config");    
            FileExists(outputDir.TempDir, "App2", "StatusServicePkg", "Config", "Settings.xml");
        }
        
        [Fact]
        public async Task NoErrors_RunAsync_CallsInnerCommandRunAsync()
        {
            using var outputDir = new TempOutputDir();
            var manifest = TestManifestBuilder
                .From("http://localhost:19080")
                .WithTempDir(outputDir.TempDir)
                .WithGroup(new DeploymentItem
                {
                    PackagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1")
                })
                .Build();
            var command = new CopyPackagesLocallyCommand(
                new CommandContext{Manifest = manifest, Logger = Logger.Object}, InnerCommand.Object);
            
            await command.RunAsync();

            InnerCommand.Verify(c => c.RunAsync(), Times.Once);
        }
        
        private static void DirectoryExists(string tempDir, params string[] folders)
        {
            var folderList = new List<string> {TestInfo.OutputDir, tempDir};
            folderList.AddRange(folders);
            var path = Path.Combine(folderList.ToArray());
            Directory.Exists(path).ShouldBeTrue();
        }
        
        private static void FileExists(string tempDir, params string[] folders)
        {
            var folderList = new List<string> {TestInfo.OutputDir, tempDir};
            folderList.AddRange(folders);
            var path = Path.Combine(folderList.ToArray());
            File.Exists(path).ShouldBeTrue();
        }
        
        private sealed class TempOutputDir : IDisposable
        {
            public TempOutputDir()
            {
                TempDir = $"TMP1{DateTime.Now:HHmmssFFF}";
            }

            public string TempDir { get; }
            
            public void Dispose()
            {
                Directory.Delete(TempDir, true);
            }
        }
    }
}
