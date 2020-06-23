using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HyperFabric.Core;
using HyperFabric.Logging;
using HyperFabric.Tasks;
using Moq;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Tasks
{
    public class CopyPackagesLocalConcurrentTaskTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void InvalidTempDir_Ctor_ThrowsException(string tempDir)
        {
            var items = new[]
            {
                new DeploymentItem {PackagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1")}
            };
            var logger = new Mock<ILogger>();
            var exception = Should.Throw<ArgumentException>(
                () => new CopyPackagesLocalConcurrentTask(tempDir, items, 1, logger.Object));
            
            exception.Message.ShouldContain("Invalid temp directory provided.");
        }
        
        [Fact]
        public async Task SinglePackage_StartAsync_CreatesTempDir()
        {
            using var outputDir = new TempOutputDir();
            var items = new[]
            {
                new DeploymentItem {PackagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1")}
            };
            var logger = new Mock<ILogger>();
            var task = new CopyPackagesLocalConcurrentTask(outputDir.TempDir, items, 1, logger.Object);

            await task.StartAsync();

            var path = Path.Combine(Environment.CurrentDirectory, outputDir.TempDir);
            Directory.Exists(path).ShouldBeTrue();
        }
        
        [Fact]
        public async Task SinglePackage_StartAsync_CopiesApp()
        {
            using var outputDir = new TempOutputDir();
            var items = new[]
            {
                new DeploymentItem {PackagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1")}
            };
            var logger = new Mock<ILogger>();
            var task = new CopyPackagesLocalConcurrentTask(outputDir.TempDir, items, 1, logger.Object);

            await task.StartAsync();

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
        public async Task MultiplePackages_StartAsync_CopiesApps()
        {
            using var outputDir = new TempOutputDir();
            var items = new[]
            {
                new DeploymentItem {PackagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1")},
                new DeploymentItem {PackagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App2")}
            };
            var logger = new Mock<ILogger>();
            var task = new CopyPackagesLocalConcurrentTask(outputDir.TempDir, items, 2, logger.Object);

            await task.StartAsync();

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
        public async Task SinglePackage_StartAsync_LogsSuccess()
        {
            using var outputDir = new TempOutputDir();
            var packagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1");
            var items = new[] {new DeploymentItem {PackagePath = packagePath}};
            var logger = new Mock<ILogger>();
            var task = new CopyPackagesLocalConcurrentTask(outputDir.TempDir, items, 1, logger.Object);

            await task.StartAsync();

            logger.Verify(l => l.Log(It.Is<LogMessage>(
                m => m.Message == "Copy package App1 locally succeeded")), Times.Once);
        }
        
        [Fact]
        public async Task MultiplePackages_StartAsync_UpdatesPackagePaths()
        {
            using var outputDir = new TempOutputDir();
            var items = new[]
            {
                new DeploymentItem {PackagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App1")},
                new DeploymentItem {PackagePath = Path.Combine(TestInfo.OutputDir, "Support", "Packages", "App2")}
            };
            var logger = new Mock<ILogger>();
            var task = new CopyPackagesLocalConcurrentTask(outputDir.TempDir, items, 2, logger.Object);

            await task.StartAsync();

            foreach (var item in items)
            {
                item.PackagePath.ShouldStartWith(outputDir.TempDir);
            }
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
                TempDir = $"TMP2{DateTime.Now:HHmmssFFF}";
            }

            public string TempDir { get; }
            
            public void Dispose()
            {
                Directory.Delete(TempDir, true);
            }
        }
    }
}
