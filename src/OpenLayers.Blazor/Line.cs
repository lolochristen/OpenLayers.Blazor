using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace OpenLayers.Blazor;

/// <summary>
///     Represents a line shape.
/// </summary>
public class Line : Shape
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Line" />.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public Line() : base(ShapeType.LineString)
    {
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Line" />.
    /// </summary>
    /// <param name="point1"></param>
    /// <param name="point2"></param>
    public Line(Coordinate point1, Coordinate point2) : this()
    {
        Points = new List<Coordinate> { point1, point2 };
    }

    /// <summary>
    ///     Gets or sets a list of points of the line.
    /// </summary>
    [Parameter]
    public IList<Coordinate> Points
    {
        get => InternalFeature.Coordinates.Default;
        set => InternalFeature.Coordinates.Default = value;
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(Points), out IList<Coordinate>? points) && points != Points)
            _updateableParametersChanged = true;

        return base.SetParametersAsync(parameters);
    }
}