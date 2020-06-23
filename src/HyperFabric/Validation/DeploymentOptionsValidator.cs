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
            ValidateHealthCheckWaitTime(manifest.Options.CheckClusterHealthWaitTime, result);
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
        
        private static void ValidateHealthCheckWaitTime(int? waitTime, ValidationResult result)
        {
            const string property = "CheckClusterHealthWaitTime";
            const string error = "The cluster health wait time is out of range: 10 to 300 seconds.";
            
            if (waitTime.HasValue && (waitTime.Value < 10 || waitTime.Value > 300))
            {
                result.AddError(property, error);
            }
        }
    }
}
