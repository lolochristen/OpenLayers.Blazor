using Microsoft.AspNetCore.Components;
using OpenLayers.Blazor.Internal;

namespace OpenLayers.Blazor;

public class Line : Shape
{
    public Line() : base(ShapeType.LineString)
    {
    }

    public Line(Coordinate point1, Coordinate point2) : this()
    {
        Points = new List<Coordinate> { point1, point2 };
    }

    [Parameter]
    public IEnumerable<Coordinate> Points
    {
        get => CoordinatesHelper.GetCoordinates(InternalFeature.Coordinates);
        set => InternalFeature.Coordinates = CoordinatesHelper.SetCoordinates(value);
    }
}