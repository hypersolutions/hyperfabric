using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HyperFabric.Core;
using HyperFabric.Extensions;
using HyperFabric.Logging;

namespace HyperFabric.Tasks
{
    public class CopyPackagesLocalConcurrentTask : ConcurrentTasks<DeploymentItem>
    {
        private readonly string _tempDir;
        private readonly ILogger _logger;
        private volatile bool _hasErrors;

        public CopyPackagesLocalConcurrentTask(
            string tempDir, 
            IEnumerable<DeploymentItem> items, 
            int taskCount, 
            ILogger logger) 
            : base(items, taskCount)
        {
            if (string.IsNullOrWhiteSpace(tempDir))
                throw new ArgumentException("Invalid temp directory provided.", nameof(tempDir));
            
            _tempDir = tempDir;
            _logger = logger;
        }

        public bool HasErrors => _hasErrors;
        
        protected override Task BeforeTaskRunAsync()
        {
            if (!Directory.Exists(_tempDir))
            {
                Directory.CreateDirectory(_tempDir);
            }
            
            return Task.CompletedTask;
        }
        
        protected override Task HandleItemAsync(DeploymentItem item, int taskId)
        {
            var sourceDirInfo = new DirectoryInfo(item.PackagePath);
            var targetDirName = Path.Combine(_tempDir, sourceDirInfo.Name);
            var packagePath = item.PackagePath;
            
            CopyDirectory(packagePath, targetDirName);
            UpdatePackagePathToLocal(item, targetDirName);
            
            _logger.Log(new LogMessage(
                StageTypes.Deployment, 
                $"Copy package {packagePath.GetDirectoryName()} locally succeeded", 
                LogLevelTypes.Ok));

            return Task.CompletedTask;
        }

        protected override Task HandleErrorAsync(DeploymentItem item, Exception error)
        {
            _logger.Log(new LogMessage(
                StageTypes.Deployment, 
                $"Failed to copy {item.PackagePath.GetDirectoryName()} locally: {error.Message}", 
                LogLevelTypes.Error));
            
            _hasErrors = true;
            return Task.CompletedTask;
        }
        
        private static void CopyDirectory(string sourceDirName, string targetDirName)
        {
            var sourceDirInfo = new DirectoryInfo(sourceDirName);
            
            CreateTargetDir(targetDirName);
            CopySourceFiles(sourceDirInfo, targetDirName);
            CopySourceSubDirFiles(sourceDirInfo, targetDirName);
        }

        private static void UpdatePackagePathToLocal(DeploymentItem item, string targetDirName)
        {
            item.PackagePath = targetDirName;
        }

        private static void CreateTargetDir(string targetDirName)
        {
            if (!Directory.Exists(targetDirName))
            {
                Directory.CreateDirectory(targetDirName);
            }
        }

        private static void CopySourceFiles(DirectoryInfo sourceDirInfo, string targetDirName)
        {
            var files = sourceDirInfo.GetFiles();
            
            foreach (var file in files)
            {
                var tempPath = Path.Combine(targetDirName, file.Name);
                file.CopyTo(tempPath, false);
            }
        }
        
        private static void CopySourceSubDirFiles(DirectoryInfo sourceDirInfo, string targetDirName)
        {
            var sourceSubDirs = sourceDirInfo.GetDirectories();
            
            foreach (var sourceSubDir in sourceSubDirs)
            {
                var tempPath = Path.Combine(targetDirName, sourceSubDir.Name);
                CopyDirectory(sourceSubDir.FullName, tempPath);
            }
        }
    }
}
