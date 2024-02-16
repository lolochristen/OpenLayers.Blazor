using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

/// <summary>
/// Shape types.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ShapeType
{
    Point,
    LineString,
    Polygon,
    Circle,
    MultiPoint,
    MultiPolygon,
    MultiLineString
}