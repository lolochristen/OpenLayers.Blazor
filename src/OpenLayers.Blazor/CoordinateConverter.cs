using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

internal class CoordinateConverter : JsonConverter<Coordinate>
{
    public override Coordinate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var val = reader.GetString();
            Coordinate.TryParse(val, out var coordinate, CultureInfo.InvariantCulture);
            return coordinate;
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
        return null;
    }

    public override void Write(Utf8JsonWriter writer, Coordinate value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}