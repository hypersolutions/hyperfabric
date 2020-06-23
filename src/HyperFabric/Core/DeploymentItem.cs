using System.Collections.Generic;
using HyperFabric.Types;
using Microsoft.ServiceFabric.Common;

namespace HyperFabric.Core
{
    public sealed class DeploymentItem
    {
        private BoundedInt _maxApplicationReadyWaitTime = new BoundedInt(5, 300, 10);
        
        public string PackagePath { get; set; }
        
        public string ParameterFile { get; set; }
        
        public bool RemoveApplicationFirst { get; set; }

        public bool CompressPackage { get; set; }
        
        public UpgradeMode? UpgradeMode { get; set; }
        
        public int MaxApplicationReadyWaitTime
        {
            get => _maxApplicationReadyWaitTime;
            set => _maxApplicationReadyWaitTime.Value = value;
        }

        public string ApplicationManifestFile { get; internal set; }

        public string ApplicationName { get; internal set; }

        public string ApplicationTypeName { get; internal set; }
        
        public string ApplicationTypeVersion { get; internal set; }

        public string ApplicationId { get; internal set; }
        
        public string ApplicationTypeBuildPath { get; internal set; }

        public IReadOnlyDictionary<string, string> Parameters { get; internal set; }
    }
}
