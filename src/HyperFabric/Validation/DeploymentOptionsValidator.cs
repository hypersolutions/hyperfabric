using System.IO;
using HyperFabric.Core;

namespace HyperFabric.Validation
{
    public sealed class DeploymentOptionsValidator : IValidator
    {
        public ValidationResult Validate(Manifest manifest)
        {
            var result = new ValidationResult();
            ValidateWorkingDirectory(manifest.Options.WorkingDirectory, result);            
            return result;
        }

        private static void ValidateWorkingDirectory(string workingDir, ValidationResult result)
        {
            const string property = "WorkingDirectory";
            const string error = "The working directory is invalid or cannot be found.";
            
            if (workingDir == null || !Directory.Exists(workingDir))
            {
                result.AddError(property, error);
            }
        }
    }
}
