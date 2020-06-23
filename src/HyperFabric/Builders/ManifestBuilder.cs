using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using HyperFabric.Core;
using HyperFabric.Core.Converters;

namespace HyperFabric.Builders
{
    public class ManifestBuilder : IManifestBuilder 
    {
        public Manifest FromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Unable to find the manifest file.", path);

            return FromString(File.ReadAllText(path));
        }

        public Manifest FromString(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("Invalid json string provided.", nameof(json));

            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            options.Converters.Add(new StoreLocationJsonConverter());
            options.Converters.Add(new X509FindTypeJsonConverter());
            options.Converters.Add(new UpgradeModeJsonConverter());
            return JsonSerializer.Deserialize<Manifest>(json, options);
        }

        public Manifest FromClusterDetails(string connection, string thumbprint = null)
        {
            if (string.IsNullOrWhiteSpace(connection))
                throw new ArgumentException("Invalid connection provided.", nameof(connection));

            var hasThumbprint = !string.IsNullOrWhiteSpace(thumbprint);
            
            return new Manifest
            {
                ClusterDetails = new Cluster
                {
                    Connection = connection, 
                    FindBy = hasThumbprint ? X509FindType.FindByThumbprint : (X509FindType?) null, 
                    FindByValue = thumbprint
                },
                Groups = new DeploymentGroup[0],
                Options = new DeploymentOptions()
            };
        }
    }
}
