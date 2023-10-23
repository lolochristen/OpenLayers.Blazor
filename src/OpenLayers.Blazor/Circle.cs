using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

public class Circle : Shape
{
    public Circle() : base(ShapeType.Circle)
    {
    }

    public Circle(Coordinate center, double radius) : this()
    {
        Center = center;
        Radius = radius;
    }

    [Parameter]
    public Coordinate? Center
    {
        get => InternalFeature.Point;
        set => InternalFeature.Point = value?.Value;
    }
}