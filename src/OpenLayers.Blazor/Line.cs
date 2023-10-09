using Microsoft.AspNetCore.Components;
using OpenLayers.Blazor.Internal;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

public class Line : Shape
{
    public Line() : base(ShapeType.LineString)
    {
    }

    public Line(Coordinate point1, Coordinate point2) : this()
    {
        Points = new List<Coordinate>() { point1, point2 };
    }

    [Parameter]
    public IEnumerable<Coordinate> Points
    {
        get => InternalFeature.Coordinates.Select(p => new Coordinate(p)).ToList();
        set => InternalFeature.Coordinates = value.Select(p => p.Value).ToList();
    }
}
