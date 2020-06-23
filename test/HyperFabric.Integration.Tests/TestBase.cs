using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Client;
using Microsoft.ServiceFabric.Common;
using Xunit.Sdk;

namespace HyperFabric.Integration.Tests
{
    public abstract class TestBase
    {
        protected static async Task SetupAsync(params PackageInfo[] packages)
        {
            var packageId = 1;
            
            foreach (var package in packages)
            {
                package.TempDir = await GetRandomDirNameAsync();

                if (package.TempDir == null)
                    throw new TestClassException("Cannot create a unique temp directory name.");

                var path = Path.Combine(TestInfo.OutputDir, package.TempDir);
                package.TestPackagePath = ClonePackage.CloneWith(packageId++, path, package);

                await Task.Delay(25);
            }
        }

        protected static async Task TearDownAsync(params PackageInfo[] packages)
        {
            try
            {
                var builder = new ServiceFabricClientBuilder();
                builder.UseEndpoints(new Uri(TestInfo.Connection));
                var fabricClient = await builder.BuildAsync();

                foreach (var package in packages)
                {
                    await DeleteExistingApplicationAsync(fabricClient, package.AppId);
                    await RemoveApplicationTypeAsync(fabricClient, package.AppTypeName);
                }
            }
            catch (Exception)
            {
                // Ignore
            }
            finally
            {
                foreach (var package in packages)
                {
                    Directory.Delete(package.TempDir, true);
                }
            }
        }
        
        private static async Task DeleteExistingApplicationAsync(IServiceFabricClient fabricClient, string appId)
        {
            var existingApplicationInfo = await fabricClient.Applications.GetApplicationInfoAsync(appId);

            if (existingApplicationInfo != null)
            {
                await fabricClient.Applications.DeleteApplicationAsync(existingApplicationInfo.Id);
            }
        }

        private static async Task RemoveApplicationTypeAsync(IServiceFabricClient fabricClient, string appTypeName)
        {
            var applicationTypes = await fabricClient.ApplicationTypes
                .GetApplicationTypeInfoListByNameAsync(appTypeName);
            
            foreach (var applicationType in applicationTypes.Data)
            {
                var descriptionInfo = new UnprovisionApplicationTypeDescriptionInfo(applicationType.Version, false);
                await fabricClient.ApplicationTypes
                    .UnprovisionApplicationTypeAsync(applicationType.Name, descriptionInfo);
            }
        }

        private static async Task<string> GetRandomDirNameAsync()
        {
            var attempts = 10;

            while (attempts > 0)
            {
                var dirName = $"PKG{DateTime.Now:HHmmssFFF}";
                var path = Path.Combine(TestInfo.OutputDir, dirName);

                if (!Directory.Exists(path))
                    return dirName;
                
                attempts--;
                await Task.Delay(10);
            }

            return null;
        }
    }
}
