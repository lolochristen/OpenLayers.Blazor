using Microsoft.AspNetCore.Components;
using OpenLayers.Blazor.Internal;

namespace OpenLayers.Blazor;

public class Circle : Shape<ShapeCircle>
{
    public Circle()
    {
    }

    public Circle(Coordinate center, double radius)
    {
        Geometry = new Geometry("Point");
        Coordinate = center;
        Radius = radius;
    }

    [Parameter]
    public Coordinate? Coordinate
    {
        get => InternalFeature.Coordinate;
        set => InternalFeature.Coordinate = value;
    }
}