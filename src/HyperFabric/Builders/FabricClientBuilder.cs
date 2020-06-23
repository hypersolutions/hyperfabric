using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using HyperFabric.Core;
using HyperFabric.Logging;
using Microsoft.ServiceFabric.Client;
using Microsoft.ServiceFabric.Common.Security;

namespace HyperFabric.Builders
{
    public class FabricClientBuilder : IFabricClientBuilder
    {
        private readonly ILogger _logger;

        public FabricClientBuilder(ILogger logger)
        {
            _logger = logger;
        }
        
        public async Task<IServiceFabricClient> BuildAsync(Manifest manifest)
        {
            try
            {
                var builder = new ServiceFabricClientBuilder();
                AppendSecurity(builder, manifest);
                AppendClusterEndpoint(builder, manifest);
                return await builder.BuildAsync();
            }
            catch (Exception error)
            {
                _logger.Log(new LogMessage(StageTypes.Preparation, error.Message, LogLevelTypes.Error));
            }

            return null;
        }

        private void AppendSecurity(ServiceFabricClientBuilder builder, Manifest manifest)
        {
            if (!string.IsNullOrWhiteSpace(manifest.ClusterDetails.FindByValue))
            {
                builder.UseX509Security(_ =>
                {
                    var certificate = FindCertificate(manifest.ClusterDetails);
                    var remoteSettings = new RemoteX509SecuritySettings(
                        new List<string>(new[] {certificate.Thumbprint}));
                    var settings = new X509SecuritySettings(certificate, remoteSettings);
                    return Task.FromResult<SecuritySettings>(settings);
                });
            }
        }
        
        private X509Certificate2 FindCertificate(Cluster cluster)
        {
            var store = new X509Store(StoreName.My, cluster.Location ?? StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            
            var certificates = store.Certificates.Find(
                cluster.FindBy ?? X509FindType.FindByThumbprint, cluster.FindByValue, false);
            store.Close();
            
            if (certificates.Count == 0)
                throw new Exception($"Cannot find certificate from its {cluster.FindBy} of {cluster.FindByValue}.");

            return certificates[0];
        }

        private static void AppendClusterEndpoint(ServiceFabricClientBuilder builder, Manifest manifest)
        {
            builder.UseEndpoints(new Uri(manifest.ClusterDetails.Connection));
        }
    }
}
