using HyperFabric.Core;

namespace HyperFabric.Handlers
{
    public interface IManifestHandler
    {
        bool Handle(Manifest manifest);
    }
}
