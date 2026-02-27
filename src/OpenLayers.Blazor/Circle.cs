using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace OpenLayers.Blazor;

/// <summary>
///     Represents a circle shape on the map.
/// </summary>
public class Circle : Shape
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Circle" />.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public Circle() : base(ShapeType.Circle)
    {
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Circle" /> with the specified center and radius.
    /// </summary>
    /// <param name="center">The center coordinate of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    public Circle(Coordinate center, double radius) : this()
    {
        Center = center;
        Radius = radius;
    }

    /// <summary>
    ///     Gets or sets the center coordinate of the circle.
    /// </summary>
    [Parameter]
    public Coordinate Center
    {
        get => InternalFeature.Point;
        set => InternalFeature.Point = value;
    }

    /// <inheritdoc/>
    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(Center), out Coordinate coordinate) && !coordinate.Equals(Center))
            _coordinatesParametersChanged = true;

        return base.SetParametersAsync(parameters);
    }
}