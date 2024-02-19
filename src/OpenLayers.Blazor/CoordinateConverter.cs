using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic.CompilerServices;

namespace OpenLayers.Blazor;

internal class CoordinateConverter : JsonConverter<Coordinate>
{
    public override Coordinate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var val = reader.GetString();
            return Coordinate.Parse(val, CultureInfo.InvariantCulture);
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            double x = 0, y = 0;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var prop = reader.GetString();
                    if (prop != null && prop.Equals("x", options.PropertyNameCaseInsensitive ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture))
                    {
                        reader.Read();
                        reader.TryGetDouble(out x);
                    }
                    else if (prop != null && prop.Equals("y", options.PropertyNameCaseInsensitive ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture))
                    {
                        reader.Read();
                        reader.TryGetDouble(out y);
                    }
                }
            }
            return new Coordinate(x, y);
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            reader.Read();
            reader.TryGetDouble(out var x);
            reader.Read();
            reader.TryGetDouble(out var y);
            while (reader.TokenType != JsonTokenType.EndArray)
                reader.Read();
            return new Coordinate(x, y);
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            reader.TryGetDouble(out var x);
            reader.Read();
            reader.TryGetDouble(out var y);
            return new Coordinate(x, y);
        }

        throw new JsonException("Cannot deserialize Coordinate");
    }

    public override void Write(Utf8JsonWriter writer, Coordinate value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value[0]);
        writer.WriteNumberValue(value[1]);
        writer.WriteEndArray();
    }
}