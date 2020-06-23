using System.Collections.Generic;
using HyperFabric.Handlers;
using HyperFabric.Logging;
using HyperFabric.Validation;

namespace HyperFabric.Factories
{
    public class ManifestHandlerFactory : IManifestHandlerFactory
    {
        private readonly ILogger _logger;

        public ManifestHandlerFactory(ILogger logger)
        {
            _logger = logger;
        }
        
        public IManifestHandler Create(IEnumerable<IValidator> validators)
        {
            return new PopulateManifestHandler(new ValidateManifestHandler(validators, _logger), _logger);
        }
    }
}
