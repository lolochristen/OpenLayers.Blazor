using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;

namespace OpenLayers.Blazor;

/// <summary>
///     Represents a point shape on the map.
/// </summary>
public class Point : Shape
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Point" />.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public Point() : base(ShapeType.Point)
    {
        Radius = 10;
        StrokeThickness = 1;
        Fill = "#809fff88";
        Stroke = "#809fff";
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Point" /> with the specified coordinate.
    /// </summary>
    /// <param name="coordinate">The coordinate of the point.</param>
    public Point(Coordinate coordinate) : this()
    {
        Coordinate = coordinate;
    }

    /// <summary>
    ///     Gets or sets the coordinate of the point.
    /// </summary>
    [Parameter]
    public Coordinate Coordinate
    {
        get => InternalFeature.Point;
        set => InternalFeature.Point = value;
    }

    /// <inheritdoc/>
    protected override void SetFill(string color) => GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>().Fill = new StyleOptions.FillOptions() { Color = color };

    /// <inheritdoc/>
    protected override string? GetFill() => GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>().Fill?.Color;

    /// <inheritdoc/>
    protected override double? GetRadius() => GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>().Radius;

    /// <inheritdoc/>
    protected override void SetRadius(double value) => GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>().Radius = value;

    /// <inheritdoc/>
    protected override string? GetStroke() => GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>().Stroke?.Color;

    /// <inheritdoc/>
    protected override void SetStroke(string color)
    {
        var cs = GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>();
        if (cs.Stroke == null)
            cs.Stroke = new StyleOptions.StrokeOptions() { Color = color };
        else
            cs.Stroke.Color = color;
    }

    /// <inheritdoc/>
    protected override double? GetStrokeThickness() =>  GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>().Stroke?.Width;

    /// <inheritdoc/>
    protected override void SetStrokeThickness(double value)
    {
        var cs = GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>();
        if (cs.Stroke == null)
            cs.Stroke = new StyleOptions.StrokeOptions() { Width = value };
        else
            cs.Stroke.Width = value;
    }

    /// <inheritdoc/>
    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(Coordinate), out Coordinate coordinate) && !coordinate.Equals(Coordinate))
            _coordinatesParametersChanged = true;

        return base.SetParametersAsync(parameters);
    }
}