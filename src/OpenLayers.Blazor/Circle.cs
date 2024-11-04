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
    public Coordinate Center
    {
        get => InternalFeature.Point;
        set => InternalFeature.Point = value;
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(Center), out Coordinate coordinate) && !coordinate.Equals(Center))
            _coordinatesParametersChanged = true;

        return base.SetParametersAsync(parameters);
    }
}