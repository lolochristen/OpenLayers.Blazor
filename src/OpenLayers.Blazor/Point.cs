using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

public class Point : Shape
{
    public Point() : base(ShapeType.Point)
    {
        Radius = 5;
        BorderSize = 1;
    }

    public Point(Coordinate coordinate) : this()
    {
        Coordinate = coordinate;
    }

    [Parameter]
    public Coordinate? Coordinate
    {
        get => InternalFeature.Point;
        set => InternalFeature.Point = value?.Value;
    }
}