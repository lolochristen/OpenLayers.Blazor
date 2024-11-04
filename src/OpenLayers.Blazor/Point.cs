using Microsoft.AspNetCore.Components;
using System.Drawing;

namespace OpenLayers.Blazor;

public class Point : Shape
{
    public Point() : base(ShapeType.Point)
    {
        Radius = 10;
        StrokeThickness = 1;
        Fill = "#809fff88";
        Stroke = "#809fff";
    }

    public Point(Coordinate coordinate) : this()
    {
        Coordinate = coordinate;
    }

    [Parameter]
    public Coordinate Coordinate
    {
        get => InternalFeature.Point;
        set => InternalFeature.Point = value;
    }

    protected override void SetFill(string color) => GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>().Fill = new StyleOptions.FillOptions() { Color = color };

    protected override string? GetFill() => GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>().Fill?.Color;

    protected override double? GetRadius() => GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>().Radius;

    protected override void SetRadius(double value) => GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>().Radius = value;

    protected override string? GetStroke() => GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>().Stroke?.Color;

    protected override void SetStroke(string color)
    {
        var cs = GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>();
        if (cs.Stroke == null)
            cs.Stroke = new StyleOptions.StrokeOptions() { Color = color };
        else
            cs.Stroke.Color = color;
    }

    protected override double? GetStrokeThickness() =>  GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>().Stroke?.Width;

    protected override void SetStrokeThickness(double value)
    {
        var cs = GetOrCreateStyleOptions<StyleOptions.CircleStyleOptions>();
        if (cs.Stroke == null)
            cs.Stroke = new StyleOptions.StrokeOptions() { Width = value };
        else
            cs.Stroke.Width = value;
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(Coordinate), out Coordinate coordinate) && !coordinate.Equals(Coordinate))
            _coordinatesParametersChanged = true;

        return base.SetParametersAsync(parameters);
    }
}