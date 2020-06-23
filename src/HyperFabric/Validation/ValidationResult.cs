using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperFabric.Validation
{
    public sealed class ValidationResult
    {
        private readonly List<ValidationError> _errors = new List<ValidationError>();

        public bool Success => !_errors.Any();

        public IEnumerable<ValidationError> Errors => _errors;
        
        public void AddError(string property, string error)
        {
            if (string.IsNullOrWhiteSpace(property)) 
                throw new ArgumentException("Property not provided.", nameof(property));
            
            if (string.IsNullOrWhiteSpace(error)) 
                throw new ArgumentException("Error message not provided.", nameof(error));
            
            _errors.Add(new ValidationError(property, error));
        }
    }
}
