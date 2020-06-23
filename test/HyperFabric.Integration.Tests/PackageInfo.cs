using System.IO;

namespace HyperFabric.Integration.Tests
{
    public sealed class PackageInfo
    {
        public PackageInfo(
            string appTypeName,
            string appTypeVersion, 
            string appName, 
            string serviceVersion = null, 
            string codeVersion = null, 
            string configVersion = null)
        {
            AppTypeName = appTypeName;
            AppTypeVersion = appTypeVersion;
            AppName = appName;
            AppId = appName.Replace("fabric:/", string.Empty);
            ServiceVersion = serviceVersion ?? appTypeVersion;
            CodeVersion = codeVersion ?? appTypeVersion;
            ConfigVersion = configVersion ?? appTypeVersion;
            DeleteCode = CodeVersion != AppTypeVersion;
            DeleteConfig = ConfigVersion != AppTypeVersion;
        }
        
        public string AppTypeName { get; }
        
        public string AppTypeVersion { get; }
        
        public string AppName { get; }
        
        public string AppId { get; }
        
        public string TempDir { get; set; }
        
        public string TestPackagePath { get; set; }
        
        public string ServiceVersion { get; set; }
        
        public string CodeVersion { get; set; }
        
        public string ConfigVersion { get; set; }
        
        public bool DeleteCode { get; }
        
        public bool DeleteConfig { get; }

        public string TestParameterPath => Path.Combine(TestPackagePath, "Local.1Node.xml");
    }
}
