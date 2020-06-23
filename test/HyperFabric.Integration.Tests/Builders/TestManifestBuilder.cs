using System.Collections.Generic;
using System.Linq;
using HyperFabric.Core;

namespace HyperFabric.Integration.Tests.Builders
{
    public sealed class TestManifestBuilder
    {
        private readonly Manifest _manifest;

        private TestManifestBuilder(string connection)
        {
            _manifest = new Manifest {ClusterDetails = new Cluster {Connection = connection}};
        }
        
        public static TestManifestBuilder From(string connection)
        {
            return new TestManifestBuilder(connection);
        }

        public TestManifestBuilder WithGroup(params DeploymentItem[] items)
        {
            var group = new DeploymentGroup {Items = items};
            var groups = _manifest.Groups?.ToList() ?? new List<DeploymentGroup>();
            groups.Add(group);
            _manifest.Groups = groups;
            return this;
        }
        
        public Manifest Build() => _manifest;
    }
}
