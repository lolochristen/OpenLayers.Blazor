using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

/// <summary>
/// Defines the geometric shape types for vector features.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ShapeType
{
    /// <summary>
    /// Single point geometry.
    /// </summary>
    Point,
    
    /// <summary>
    /// Line string geometry (connected line segments).
    /// </summary>
    LineString,
    
    /// <summary>
    /// Polygon geometry (closed shape).
    /// </summary>
    Polygon,
    
    /// <summary>
    /// Circle geometry.
    /// </summary>
    Circle,
    
    /// <summary>
    /// Collection of multiple points.
    /// </summary>
    MultiPoint,
    
    /// <summary>
    /// Collection of multiple polygons.
    /// </summary>
    MultiPolygon,
    
    /// <summary>
    /// Collection of multiple line strings.
    /// </summary>
    MultiLineString
}