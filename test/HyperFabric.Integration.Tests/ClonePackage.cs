using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace HyperFabric.Integration.Tests
{
    public static class ClonePackage
    {
        private static readonly XNamespace _fabricNamespace = XNamespace.Get(FabricNamespace);
        private const string FabricNamespace = "http://schemas.microsoft.com/2011/01/fabric";
        
        public static string CloneWith(int packageId, string tempDir, PackageInfo package)
        {
            var targetDirName = CopyPackage(tempDir, packageId, package.DeleteCode, package.DeleteConfig);
            UpdateApplicationManifest(
                targetDirName, package.AppTypeName, package.AppTypeVersion, package.ServiceVersion);
            UpdateAppParameters(targetDirName, package.AppName);
            UpdateServiceManifest(targetDirName, package.ServiceVersion, package.CodeVersion, package.ConfigVersion);
            return Path.Combine(tempDir, $"App{packageId}");
        }

        private static string CopyPackage(string tempDir, int packageId, bool deleteCode, bool deleteConfig)
        {
            UnzipSourcePackage();
            var sourceDirInfo = new DirectoryInfo(Path.Combine(TestInfo.OutputDir, "App1"));
            var targetDirName = Path.Combine(tempDir, sourceDirInfo.Name.Replace("App1", $"App{packageId}"));
            var packagePath = sourceDirInfo.FullName;
            CopyDirectory(packagePath, targetDirName);

            if (deleteCode)
            {
                var path = Path.Combine(targetDirName, "StatusServicePkg", "Code");
                Directory.Delete(path, true);
            }

            if (deleteConfig)
            {
                var path = Path.Combine(targetDirName, "StatusServicePkg", "Config");
                Directory.Delete(path, true);
            }
            
            return targetDirName;
        }

        private static void UnzipSourcePackage()
        {
            var sourceDirInfo = new DirectoryInfo(Path.Combine(TestInfo.OutputDir, "App1"));

            if (sourceDirInfo.Exists) return;
            
            var sourceFileInfo = new FileInfo(Path.Combine(TestInfo.OutputDir, "../../../Packages/App1.zip"));
            
            ZipFile.ExtractToDirectory(sourceFileInfo.FullName, TestInfo.OutputDir);
        }

        private static void CopyDirectory(string sourceDirName, string targetDirName)
        {
            var sourceDirInfo = new DirectoryInfo(sourceDirName);
            
            CreateTargetDir(targetDirName);
            CopySourceFiles(sourceDirInfo, targetDirName);
            CopySourceSubDirFiles(sourceDirInfo, targetDirName);
        }
        
        private static void CreateTargetDir(string targetDirName)
        {
            if (!Directory.Exists(targetDirName))
            {
                Directory.CreateDirectory(targetDirName);
            }
        }

        private static void CopySourceFiles(DirectoryInfo sourceDirInfo, string targetDirName)
        {
            var files = sourceDirInfo.GetFiles();
            
            foreach (var file in files)
            {
                var tempPath = Path.Combine(targetDirName, file.Name);
                file.CopyTo(tempPath, false);
            }
        }
        
        private static void CopySourceSubDirFiles(DirectoryInfo sourceDirInfo, string targetDirName)
        {
            var sourceSubDirs = sourceDirInfo.GetDirectories();
            
            foreach (var sourceSubDir in sourceSubDirs)
            {
                var tempPath = Path.Combine(targetDirName, sourceSubDir.Name);
                CopyDirectory(sourceSubDir.FullName, tempPath);
            }
        }
        
        private static void UpdateApplicationManifest(
            string targetDirName, 
            string appTypeName, 
            string appTypeVersion,
            string serviceVersion)
        {
            var path = Path.Combine(targetDirName, "ApplicationManifest.xml");
            var document = XDocument.Load(path);
            UpdateAppTypeName(document, appTypeName);
            UpdateAppTypeVersion(document, appTypeVersion);
            UpdateAppServiceManifestVersion(document, serviceVersion);
            document.Save(path);
        }
        
        private static void UpdateAppParameters(string targetDirName, string appName)
        {
            var document = XDocument.Load(Path.Combine(targetDirName, "Local.1Node.xml"));
            UpdateAppName(document, appName);
            document.Save(Path.Combine(targetDirName, "Local.1Node.xml"));
        }
        
        private static void UpdateAppTypeName(XDocument document, string appTypeName)
        {
            var element = document.Element(_fabricNamespace + "ApplicationManifest");
            var attribute = element?.Attribute("ApplicationTypeName");
            
            if (attribute != null)
                attribute.Value = appTypeName;
        }
        
        private static void UpdateAppTypeVersion(XDocument document, string appTypeVersion)
        {
            var element = document.Element(_fabricNamespace + "ApplicationManifest");
            var attribute = element?.Attribute("ApplicationTypeVersion");
            
            if (attribute != null)
                attribute.Value = appTypeVersion;
        }
        
        private static void UpdateAppServiceManifestVersion(XDocument document, string serviceVersion)
        {
            var element = document.Descendants().FirstOrDefault(
                e => e.Name == _fabricNamespace + "ServiceManifestRef");
            var attribute = element?.Attribute("ServiceManifestVersion");
            
            if (attribute != null)
                attribute.Value = serviceVersion;
        }
        
        private static void UpdateAppName(XDocument document, string appName)
        {
            var element = document.Element(_fabricNamespace + "Application");
            var attribute = element?.Attribute("Name");
            
            if (attribute != null)
                attribute.Value = appName;
        }
        
        private static void UpdateServiceManifest(
            string targetDirName, 
            string serviceVersion, 
            string codeVersion, 
            string configVersion)
        {
            var path = Path.Combine(targetDirName, "StatusServicePkg", "ServiceManifest.xml");
            var document = XDocument.Load(path);
            UpdateServiceVersion(document, serviceVersion);
            UpdateServiceCodeVersion(document, codeVersion);
            UpdateServiceConfigVersion(document, configVersion);
            document.Save(path);
        }
        
        private static void UpdateServiceVersion(XDocument document, string serviceVersion)
        {
            var element = document.Element(_fabricNamespace + "ServiceManifest");
            var attribute = element?.Attribute("Version");
            
            if (attribute != null)
                attribute.Value = serviceVersion;
        }
        
        private static void UpdateServiceCodeVersion(XDocument document, string codeVersion)
        {
            var element = document.Descendants().FirstOrDefault(e => e.Name == _fabricNamespace + "CodePackage");
            var attribute = element?.Attribute("Version");
            
            if (attribute != null)
                attribute.Value = codeVersion;
        }
        
        private static void UpdateServiceConfigVersion(XDocument document, string configVersion)
        {
            var element = document.Descendants().FirstOrDefault(e => e.Name == _fabricNamespace + "ConfigPackage");
            var attribute = element?.Attribute("Version");
            
            if (attribute != null)
                attribute.Value = configVersion;
        }
    }
}
