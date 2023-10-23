using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ShapeType
{
    Point,
    LineString,
    Polygon,
    Circle
}