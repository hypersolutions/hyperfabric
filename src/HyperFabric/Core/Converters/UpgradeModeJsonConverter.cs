using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.ServiceFabric.Common;

namespace HyperFabric.Core.Converters
{
    public sealed class UpgradeModeJsonConverter : JsonConverter<UpgradeMode>
    {
        public override UpgradeMode Read(
            ref Utf8JsonReader reader, 
            Type typeToConvert, 
            JsonSerializerOptions options)
        {
            var jsonValue = reader.GetString();

            if (string.IsNullOrWhiteSpace(jsonValue)) return UpgradeMode.UnmonitoredAuto;
            
            return Enum.TryParse<UpgradeMode>(reader.GetString(), true, out var value) 
                ? value : UpgradeMode.UnmonitoredAuto;
        }

        public override void Write(
            Utf8JsonWriter writer, 
            UpgradeMode value, 
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
