using System;
using System.IO;
using HyperFabric.Types;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace HyperFabric.Core
{
    public sealed class DeploymentOptions
    {
        private BoundedInt _numberOfParallelDeployments = new BoundedInt(1, 10, 5);

        public DeploymentOptions()
        {
            TempDirectoryName = $"TMP{DateTime.Now:HHmmssFFF}";
        }
        
        public int NumberOfParallelDeployments
        {
            get => _numberOfParallelDeployments;
            set => _numberOfParallelDeployments.Value = value;
        }
        
        public int? CheckClusterHealthWaitTime { get; set; } = 30;
        
        public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;
        
        internal string TempDirectoryName { get; set; }

        internal string TempFullPath => Path.Combine(WorkingDirectory, TempDirectoryName);
    }
}
