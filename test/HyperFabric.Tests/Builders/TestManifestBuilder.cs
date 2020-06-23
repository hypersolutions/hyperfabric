using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using HyperFabric.Core;

namespace HyperFabric.Tests.Builders
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

        public TestManifestBuilder WithThumbprint(string thumbprint)
        {
            _manifest.ClusterDetails.FindBy = X509FindType.FindByThumbprint;
            _manifest.ClusterDetails.FindByValue = thumbprint;
            return this;
        }
        
        public TestManifestBuilder WithWorkingDirectory(string workingDir)
        {
            _manifest.Options.WorkingDirectory = workingDir;
            return this;
        }
        
        public TestManifestBuilder WithClusterHealthWaitTime(int waitTime)
        {
            _manifest.Options.CheckClusterHealthWaitTime = waitTime;
            return this;
        }
        
        public TestManifestBuilder WithGroup(params DeploymentItem[] items)
        {
            var group = new DeploymentGroup {Items = items};
            var groups = _manifest.Groups?.ToList() ?? new List<DeploymentGroup>();
            groups.Add(group);
            _manifest.Groups = groups;
            return this;
        }

        public TestManifestBuilder WithGroupNullItems()
        {
            var group = new DeploymentGroup();
            var groups = _manifest.Groups?.ToList() ?? new List<DeploymentGroup>();
            groups.Add(group);
            _manifest.Groups = groups;
            return this;
        }

        public TestManifestBuilder WithEmptyGroups()
        {
            _manifest.Groups = new DeploymentGroup[0];
            return this;
        }

        public TestManifestBuilder WithTempDir(string dir)
        {
            _manifest.Options.TempDirectoryName = dir;
            return this;
        }
        
        public Manifest Build() => _manifest;
    }
}
