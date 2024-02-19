using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

/// <summary>
///     JsonConverter for <see cref="Coordinates" />
/// </summary>
internal class CoordinatesConverter : JsonConverter<Coordinates>
{
    private readonly CoordinateConverter _coordinateConverter = new();

    public override Coordinates Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String) return new Coordinates(Coordinate.Parse(reader.GetString(), CultureInfo.InvariantCulture)) { Type = CoordinatesType.Point };

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var coordinates = new Coordinates { Type = CoordinatesType.List };
            reader.Read();

            if (reader.TokenType == JsonTokenType.Number) // point
            {
                coordinates.Point = _coordinateConverter.Read(ref reader, typeof(Coordinate), options);
                while (reader.TokenType != JsonTokenType.EndArray)
                    reader.Read();
            }
            else
            {
                while (reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        reader.Read();
                        if (reader.TokenType == JsonTokenType.Number)
                        {
                            var coordinate = _coordinateConverter.Read(ref reader, typeof(Coordinate), options);
                            coordinates.Default.Add(coordinate);

                            while (reader.TokenType != JsonTokenType.EndArray)
                                reader.Read();
                        }
                        else if (reader.TokenType == JsonTokenType.StartArray) // multi
                        {
                            coordinates.Type = CoordinatesType.MultiList;
                            if (coordinates.Values.Count == 1 && coordinates.Values[0].Count == 0)
                                coordinates.Values.Clear();

                            var innerCoordinates = new List<Coordinate>();

                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.TokenType == JsonTokenType.StartArray)
                                {
                                    var coordinate = _coordinateConverter.Read(ref reader, typeof(Coordinate), options);
                                    innerCoordinates.Add(coordinate);
                                }

                                reader.Read();
                            }

                            coordinates.Values.Add(innerCoordinates);
                        }
                    }

                    reader.Read();
                }
            }

            return coordinates;
        }

        throw new InvalidOperationException();
    }

    public override void Write(Utf8JsonWriter writer, Coordinates value, JsonSerializerOptions options)
    {
        switch (value.Type)
        {
            case CoordinatesType.Point:
                _coordinateConverter.Write(writer, value.Default[0], options);
                break;

            case CoordinatesType.List:
                writer.WriteStartArray();
                foreach (var coordinate in value.Default)
                    _coordinateConverter.Write(writer, coordinate, options);
                writer.WriteEndArray();
                break;

            case CoordinatesType.MultiList:
                writer.WriteStartArray();
                foreach (var coordinates in value)
                {
                    writer.WriteStartArray();
                    foreach (var coordinate in coordinates)
                        _coordinateConverter.Write(writer, coordinate, options);
                    writer.WriteEndArray();
                }

                writer.WriteEndArray();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}