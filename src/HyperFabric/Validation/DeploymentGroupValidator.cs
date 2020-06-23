using System.Collections.Generic;
using System.Linq;
using HyperFabric.Core;

namespace HyperFabric.Validation
{
    public class DeploymentGroupValidator : IValidator
    {
        public ValidationResult Validate(Manifest manifest)
        {
            var result = new ValidationResult();

            foreach (var group in manifest.Groups)
            {
                ValidateItems(group.Items, result);
            }
            
            return result;
        }
        
        private static void ValidateItems(IEnumerable<DeploymentItem> items, ValidationResult result)
        {
            const string property = "Items";
            const string error = "No deployment items for the group have been provided.";
            
            var itemList = items?.ToList() ?? new List<DeploymentItem>(0);
            
            if (!itemList.Any())
            {
                result.AddError(property, error);
            }
        }
    }
}
