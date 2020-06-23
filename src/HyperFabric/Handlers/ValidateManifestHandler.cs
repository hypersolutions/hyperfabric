using System.Collections.Generic;
using System.Linq;
using HyperFabric.Core;
using HyperFabric.Logging;
using HyperFabric.Validation;

namespace HyperFabric.Handlers
{
    public sealed class ValidateManifestHandler : IManifestHandler
    {
        private readonly IEnumerable<IValidator> _validators;
        private readonly ILogger _logger;

        public ValidateManifestHandler(IEnumerable<IValidator> validators, ILogger logger)
        {
            _validators = validators;
            _logger = logger;
        }
        
        public bool Handle(Manifest manifest)
        {
            var result = new ValidationResult();
            
            foreach (var validator in _validators)
            {
                var validationResult = validator.Validate(manifest);

                if (validationResult.Success) continue;
                
                foreach (var validationError in validationResult.Errors)
                {
                    result.AddError(validationError.Property, validationError.Error);
                }
            }

            if (!result.Success)
            {
                var messages = result.Errors
                    .Select(e => new LogMessage(
                        StageTypes.Preparation, $"{e.Property}: {e.Error}", LogLevelTypes.Error))
                    .ToArray();
                _logger.Log(messages);
            }

            return result.Success;
        }
    }
}
