using HyperFabric.Commands;
using HyperFabric.Core;

namespace HyperFabric.Factories
{
    public interface IDeploymentCommandFactory
    {
        ICommand Create(Manifest manifest);
    }
}
