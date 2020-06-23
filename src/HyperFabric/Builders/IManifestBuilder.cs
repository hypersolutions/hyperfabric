using HyperFabric.Core;

namespace HyperFabric.Builders
{
    public interface IManifestBuilder
    {
        Manifest FromFile(string path);
        Manifest FromString(string json);
        Manifest FromClusterDetails(string connection, string thumbprint = null);
    }
}
