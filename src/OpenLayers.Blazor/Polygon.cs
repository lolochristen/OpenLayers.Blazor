using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using OpenLayers.Blazor.Internal;

namespace OpenLayers.Blazor;

/// <summary>
/// Polygon shape
/// </summary>
public class Polygon : Shape
{
    /// <summary>
    /// Initializes a new instance of <see cref="Polygon"/>.
    /// </summary>
    public Polygon() : base(ShapeType.Polygon)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Polygon"/>.
    /// </summary>
    /// <param name="coordinates"></param>
    public Polygon(IList<Coordinate> coordinates) : this()
    {
        Points = coordinates;
    }

    /// <summary>
    /// Gets or sets the first array of coordinates for a polygon
    /// </summary>
    [Parameter]
    public IList<Coordinate> Points
    {
        get => InternalFeature.Coordinates.Default;
        set
        {
            InternalFeature.Coordinates.Default = value;
            InternalFeature.Coordinates.Type = CoordinatesType.MultiList; // polygon
        }
    }
}