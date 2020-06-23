using System.Security.Cryptography.X509Certificates;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HyperFabric.Core
{
    public sealed class Cluster
    {
        public string Connection { get; set; }
        
        public StoreLocation? Location { get; set; }
        
        public X509FindType? FindBy { get; set; }
        
        public string FindByValue { get; set; }
    }
}
