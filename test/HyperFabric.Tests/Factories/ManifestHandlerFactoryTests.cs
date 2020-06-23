using System.Collections.Generic;
using HyperFabric.Factories;
using HyperFabric.Handlers;
using HyperFabric.Validation;
using Shouldly;
using Xunit;

namespace HyperFabric.Tests.Factories
{
    public class ManifestHandlerFactoryTests : TestBase<ManifestHandlerFactory>
    {
        [Fact]
        public void WithValidators_Create_ReturnsPopulateManifestHandler()
        {
            var handler = Subject.Create(new List<IValidator>());
            
            handler.ShouldBeOfType<PopulateManifestHandler>();
        }
    }
}
