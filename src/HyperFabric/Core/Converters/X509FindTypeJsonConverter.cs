using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HyperFabric.Core.Converters
{
    public sealed class X509FindTypeJsonConverter : JsonConverter<X509FindType>
    {
        public override X509FindType Read(
            ref Utf8JsonReader reader, 
            Type typeToConvert, 
            JsonSerializerOptions options)
        {
            var inputValue = reader.GetString();

            if (!inputValue.StartsWith("FindBy", StringComparison.OrdinalIgnoreCase))
                inputValue = $"FindBy{inputValue}";
            
            return Enum.TryParse<X509FindType>(inputValue, true, out var value) 
                ? value : X509FindType.FindByThumbprint;
        }

        public override void Write(
            Utf8JsonWriter writer, 
            X509FindType value,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
