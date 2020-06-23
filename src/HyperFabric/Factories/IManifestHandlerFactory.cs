using System.Collections.Generic;
using HyperFabric.Handlers;
using HyperFabric.Validation;

namespace HyperFabric.Factories
{
    public interface IManifestHandlerFactory
    {
        IManifestHandler Create(IEnumerable<IValidator> validators);
    }
}
