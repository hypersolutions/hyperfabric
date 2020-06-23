using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HyperFabric.Core.Converters
{
    public sealed class StoreLocationJsonConverter : JsonConverter<StoreLocation>
    {
        public override StoreLocation Read(
            ref Utf8JsonReader reader, 
            Type typeToConvert, 
            JsonSerializerOptions options)
        {
            return Enum.TryParse<StoreLocation>(reader.GetString(), true, out var value) 
                ? value : StoreLocation.CurrentUser;
        }

        public override void Write(
            Utf8JsonWriter writer, 
            StoreLocation value, 
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
