using System.Collections.Generic;

namespace HyperFabric.Core
{
    public sealed class Manifest
    {
        public Manifest()
        {
            Options = new DeploymentOptions();
        }
        
        public Cluster ClusterDetails { get; set; }
        
        public IEnumerable<DeploymentGroup> Groups { get; set; }
        
        public DeploymentOptions Options { get; set; }
    }
}
