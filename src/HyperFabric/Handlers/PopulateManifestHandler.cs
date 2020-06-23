using System.IO;
using System.Linq;
using HyperFabric.Core;
using HyperFabric.Extensions;
using HyperFabric.Logging;

namespace HyperFabric.Handlers
{
    public sealed class PopulateManifestHandler : IManifestHandler
    {
        private readonly IManifestHandler _innerHandler;
        private readonly ILogger _logger;

        public PopulateManifestHandler(IManifestHandler innerHandler, ILogger logger)
        {
            _innerHandler = innerHandler;
            _logger = logger;
        }
        
        public bool Handle(Manifest manifest)
        {
            _logger.Log(new LogMessage(
                StageTypes.Preparation, "Populating the manifest with calculated values", LogLevelTypes.Ok));
            
            foreach (var group in manifest.Groups)
            {
                foreach (var item in group.Items)
                {
                    item.ApplicationManifestFile = !string.IsNullOrWhiteSpace(item.PackagePath) 
                        ? Path.Combine(item.PackagePath, "ApplicationManifest.xml") : null;
                    
                    item.ApplicationName = item.ParameterFile.GetFabricAttributeValue("Application", "Name");
                    
                    item.ApplicationTypeName = item.ApplicationManifestFile.GetFabricAttributeValue(
                        "ApplicationManifest", "ApplicationTypeName");
                    
                    item.ApplicationTypeVersion = item.ApplicationManifestFile.GetFabricAttributeValue(
                        "ApplicationManifest", "ApplicationTypeVersion");
                    
                    item.ApplicationId = !string.IsNullOrWhiteSpace(item.ApplicationName)
                        ? item.ApplicationName.Replace("fabric:/", string.Empty)
                        : null;

                    item.ApplicationTypeBuildPath = !string.IsNullOrWhiteSpace(item.PackagePath)
                        ? item.PackagePath.Split('\\').Last() : null;
                    
                    item.Parameters = item.ParameterFile.GetFabricParameterDictionary();
                }
            }
            
            return _innerHandler.Handle(manifest);     
        }
    }
}
