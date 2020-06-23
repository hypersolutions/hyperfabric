using System.Collections.Generic;

namespace HyperFabric.Core
{
    public sealed class DeploymentGroup
    {
        public IEnumerable<DeploymentItem> Items { get; set; }
    }
}
