using System.Text.RegularExpressions;
using HyperFabric.Core;

namespace HyperFabric.Validation
{
    public sealed class ClusterValidator : IValidator
    {
        private static readonly Regex _connectionRegex = new Regex(@"^(https?://.*):(\d*)$");
        
        public ValidationResult Validate(Manifest manifest)
        {
            var result = new ValidationResult();

            ValidateConnection(manifest.ClusterDetails.Connection, result);
            
            if (manifest.ClusterDetails.FindBy.HasValue)
            {
                ValidateFindBy(manifest.ClusterDetails.FindByValue, result);    
            }
            
            return result;
        }

        private static void ValidateConnection(string connection, ValidationResult result)
        {
            const string property = "Connection";
            const string error = "The cluster connection is in an incorrect format.";
            
            if (!_connectionRegex.IsMatch(connection ?? string.Empty))
            {
                result.AddError(property, error);
            }
        }
        
        private static void ValidateFindBy(string value, ValidationResult result)
        {
            const string property = "FindByValue";
            const string error = "The find by value has not been provided.";
            
            if (value == null)
            {
                result.AddError(property, error);
            }
        }
    }
}
