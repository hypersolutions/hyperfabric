using System.Collections.Generic;
using System.Linq;
using HyperFabric.Core;

namespace HyperFabric.Validation
{
    public class ManifestValidator : IValidator
    {
        public ValidationResult Validate(Manifest manifest)
        {
            var result = new ValidationResult();

            ValidateClusterDetails(manifest.ClusterDetails, result);
            ValidateGroups(manifest.Groups, result);
            
            return result;
        }

        private static void ValidateClusterDetails(Cluster cluster, ValidationResult result)
        {
            const string property = "ClusterDetails";
            const string error = "No cluster details have been provided.";
            
            if (cluster == null)
            {
                result.AddError(property, error);
            }
        }
        
        private static void ValidateGroups(IEnumerable<DeploymentGroup> groups, ValidationResult result)
        {
            const string property = "Groups";
            const string error = "No deployment groups have been provided.";
            
            var groupList = groups?.ToList() ?? new List<DeploymentGroup>(0);
            
            if (!groupList.Any())
            {
                result.AddError(property, error);
            }
        }
    }
}
