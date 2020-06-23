namespace HyperFabric.Validation
{
    public sealed class ValidationError
    {
        public ValidationError(string property, string error)
        {
            Property = property;
            Error = error;
        }
        
        public string Property { get; }

        public string Error { get; }
    }
}
