using HyperFabric.Core;
using HyperFabric.Logging;
using Microsoft.ServiceFabric.Client;

namespace HyperFabric.Commands
{
    public class CommandContext
    {
        public IServiceFabricClient FabricClient { get; set; }
        
        public ILogger Logger { get; set; }
        
        public IApplicationClient AppClient => FabricClient.Applications;

        public IApplicationTypeClient AppTypeClient => FabricClient.ApplicationTypes;

        public IImageStoreClient ImgStoreClient => FabricClient.ImageStore;
        
        public Manifest Manifest { get; set; }
        
        public DeploymentItem CurrentDeploymentItem { get; set; }
    }
}
