using Microsoft.AspNetCore.Components;
using OpenLayers.Blazor.Internal;

namespace OpenLayers.Blazor;

/// <summary>
/// Polygon shape
/// </summary>
public class Polygon : Shape
{
    public Polygon() : base(ShapeType.Polygon)
    {
    }

    public Polygon(IEnumerable<Coordinate> coordinates) : this()
    {
        Points = coordinates;
    }

    [Parameter]
    public IEnumerable<Coordinate> Points
    {
        get => CoordinatesHelper.GetCoordinates(InternalFeature.Coordinates);
        set => InternalFeature.Coordinates = new[] { CoordinatesHelper.SetCoordinates(value) }; // polygon requires double[][][]
    }
}