using System.Threading.Tasks;
using HyperFabric.Core;
using Microsoft.ServiceFabric.Client;

namespace HyperFabric.Builders
{
    public interface IFabricClientBuilder
    {
        Task<IServiceFabricClient> BuildAsync(Manifest manifest);
    }
}
