using HyperFabric.Core;

namespace HyperFabric.Validation
{
    public interface IValidator
    {
        ValidationResult Validate(Manifest manifest);
    }
}
