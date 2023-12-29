using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

public class CoordinateConverter : JsonConverter<Coordinate>
{
    public override Coordinate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var val = reader.GetString();
            Coordinate.TryParse(val, out var coordinate);
            return coordinate;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, Coordinate value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}