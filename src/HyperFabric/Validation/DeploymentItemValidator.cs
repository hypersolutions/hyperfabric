using System.IO;
using HyperFabric.Core;

namespace HyperFabric.Validation
{
    public class DeploymentItemValidator : IValidator
    {
        public ValidationResult Validate(Manifest manifest)
        {
            var result = new ValidationResult();

            foreach (var group in manifest.Groups)
            {
                foreach (var item in group.Items)
                {
                    ValidatePackagePath(item.PackagePath, result);
                    ValidateParameterFile(item.ParameterFile, result);
                    ValidateApplicationManifest(item.ApplicationManifestFile, result);
                    ValidateApplicationName(item.ApplicationName, result);
                    ValidateApplicationType(item.ApplicationTypeName, result);
                    ValidateApplicationVersion(item.ApplicationTypeVersion, result);
                }
            }

            return result;
        }

        private static void ValidatePackagePath(string path, ValidationResult result)
        {
            const string property = "PackagePath";
            const string error = "The path to the package cannot be found or has not been provided.";

            if (!Directory.Exists(path))
            {
                result.AddError(property, error);
            }
        }
        
        private static void ValidateParameterFile(string file, ValidationResult result)
        {
            const string property = "ParameterFile";
            const string error = "The parameter file cannot be found or has not been provided.";

            if (!File.Exists(file))
            {
                result.AddError(property, error);
            }
        }
        
        private static void ValidateApplicationManifest(string file, ValidationResult result)
        {
            const string property = "ApplicationManifestFile";
            const string error = "Unable to find the ApplicationManifest.xml file inside the provided PackagePath.";

            if (!File.Exists(file))
            {
                result.AddError(property, error);
            }
        }
        
        private static void ValidateApplicationName(string applicationName, ValidationResult result)
        {
            const string property = "ApplicationName";
            const string error = "Unable to find the application name in the parameters file.";

            if (string.IsNullOrWhiteSpace(applicationName))
            {
                result.AddError(property, error);
            }
        }
        
        private static void ValidateApplicationType(string applicationType, ValidationResult result)
        {
            const string property = "ApplicationType";
            const string error = "Unable to find the application type in the application manifest file.";

            if (string.IsNullOrWhiteSpace(applicationType))
            {
                result.AddError(property, error);
            }
        }
        
        private static void ValidateApplicationVersion(string applicationVersion, ValidationResult result)
        {
            const string property = "ApplicationVersion";
            const string error = "Unable to find the application version in the application manifest file.";

            if (string.IsNullOrWhiteSpace(applicationVersion))
            {
                result.AddError(property, error);
            }
        }
    }
}
