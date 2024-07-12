﻿using Microsoft.AspNetCore.Components;
using OpenLayers.Blazor.Internal;
using System.Drawing;

namespace OpenLayers.Blazor;

/// <summary>
///     A Marker component to attach to a <see cref="Map" />
/// </summary>
public class Marker : Shape
{
    /// <summary>
    ///     Default Constructor
    /// </summary>
    public Marker()
    {
        InternalFeature.GeometryType = GeometryTypes.Point;
        InternalFeature.Type = nameof(Marker);
    }

    internal Marker(Internal.Shape shape)
    {
        InternalFeature = shape;
        InternalFeature.Type = nameof(Marker);
    }

    /// <summary>
    ///     Constructor with key parameters.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="coordinate"></param>
    /// <param name="title"></param>
    public Marker(MarkerType type, Coordinate coordinate, string? title = null, PinColor pinColor = PinColor.Red) : this()
    {
        Type = type;
        Coordinate = coordinate;
        Text = title;
        PinColor = pinColor;
    }

    /// <summary>
    ///     Constructor for a Marker <see cref="MarkerType.MarkerAwesome" />
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="icon">see https://fontawesome.com/search?o=r&m=free</param>
    public Marker(Coordinate coordinate, char icon) : this()
    {
        Type = MarkerType.MarkerAwesome;
        Coordinate = coordinate;
        Text = icon.ToString();
    }

    /// <summary>
    ///     Extended consturctor.
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="imageSource"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="anchorX"></param>
    /// <param name="anchorY"></param>
    public Marker(Coordinate coordinate, string imageSource, double width, double height, double anchorX, double anchorY) : this()
    {
        Type = MarkerType.MarkerCustomImage;
        Coordinate = coordinate;
        Source = imageSource;
        Size = new[] { width, height };
        Anchor = new[] { anchorX, anchorY };
    }

    /// <summary>
    ///     Type of the marker: <see cref="MarkerType" />
    /// </summary>
    [Parameter]
    public MarkerType Type
    {
        get => InternalFeature.GetProperty<string>("markerstyle") != null ? Enum.Parse<MarkerType>(InternalFeature.GetProperty<string>("markerstyle")) : MarkerType.MarkerPin;
        set
        {
            switch (value)
            {
                case MarkerType.MarkerPin:
                    SetPinStyle();
                    break;
                case MarkerType.MarkerCustomImage:
                    SetCustomImageStyle();
                    break;
                case MarkerType.MarkerAwesome:
                    SetAwesomeStyle();
                    break;
                case MarkerType.MarkerFlag:
                    SetFlagStyle();
                    break;
            }
            InternalFeature.Properties["markerstyle"] = value.ToString();
        }
    }

    /// <summary>
    ///     Size of the marker in width/height
    /// </summary>
    [Parameter]
    public double[]? Size
    {
        get => HasStyleOptions<StyleOptions.IconStyleOptions>() ? GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>().Size : null;
        set => GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>().Size = value ;
    }

    /// <summary>
    ///     Anchor point for the marker image/icon
    /// </summary>
    [Parameter]
    public double[]? Anchor
    {
        get => HasStyleOptions<StyleOptions.IconStyleOptions>() ? GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>().Anchor : null;
        set => GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>().Anchor = value ;
    }

    /// <summary>
    ///     Coordinate of the marker.
    /// </summary>
    [Parameter]
    public Coordinate Coordinate
    {
        get => InternalFeature.Point;
        set => InternalFeature.Point = value;
    }

    /// <summary>
    ///     Icon Rotation in radiant
    /// </summary>
    [Parameter]
    public double? Rotation
    {
        get => HasStyleOptions<StyleOptions.IconStyleOptions>() ? GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>().Rotation: null;
        set => GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>().Rotation = value;
    }

    [Parameter]
    public string? Source
    {
        get => HasStyleOptions<StyleOptions.IconStyleOptions>() ? GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>().Source: null;
        set => GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>().Source = value;
    }

    private PinColor _pinColor = PinColor.Red;

    [Parameter]
    public PinColor PinColor
    {
        get => _pinColor;
        set
        {
            _pinColor = value;
            if (Type == MarkerType.MarkerPin)
            {
                var icon2 = GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>(1);
                icon2.Source = $"./_content/OpenLayers.Blazor/img/marker-pin-{_pinColor.ToString().ToLowerInvariant()}.png";
            }

        }
    }

    protected override void SetText(string value)
    {
        if (Type == MarkerType.MarkerAwesome)
            GetOrCreateStyleOptions<StyleOptions.TextOptions>(2).Text = value;
        else
            base.SetText(value);
    }

    protected override string? GetText()
    {
        if (Type == MarkerType.MarkerAwesome)
            return GetOrCreateStyleOptions<StyleOptions.TextOptions>(2).Text;
        else
            return base.GetText();
    }

    protected override void SetStroke(string color)
    {
        if (Type == MarkerType.MarkerAwesome)
            GetOrCreateStyleOptions<StyleOptions.TextOptions>(1).Stroke.Color = color;
        else
            base.SetStroke(color);
    }

    protected override string? GetStroke()
    {
        if (Type == MarkerType.MarkerAwesome)
            return GetOrCreateStyleOptions<StyleOptions.TextOptions>(1).Stroke.Color;
        return base.GetStroke();
    }

    protected override void SetScale(double? value)
    {
        if (Type == MarkerType.MarkerPin)
        {
            GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>(0).Scale = value;
            GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>(1).Scale = value;
        }
        else
            base.SetScale(value);
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(Rotation), out double? rotation) && rotation != Rotation)
            _updateableParametersChanged = true;

        if (parameters.TryGetValue(nameof(PinColor), out PinColor pinColor) && pinColor != PinColor)
            _updateableParametersChanged = true;

        return base.SetParametersAsync(parameters);
    }

    protected void SetPinStyle()
    {
        var scale = 0.2; // is small
        if (Scale != null && Scale != 1)
            scale = Scale.Value;

        var icon1 = GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>(0);
        icon1.Anchor = [0, 60];
        icon1.Size = [160, 60];
        icon1.Offset = [0, 0];
        icon1.Opacity = 1;
        icon1.Scale = scale;
        icon1.AnchorXUnits = StyleOptions.IconAnchorUnits.Pixels;
        icon1.AnchorYUnits = StyleOptions.IconAnchorUnits.Pixels;
        icon1.Source = "./_content/OpenLayers.Blazor/img/pin-back.png";

        var icon2 = GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>(1);
        icon2.Anchor = [100, 198];
        icon2.Size = [200, 200];
        icon2.Offset = [0, 0];
        icon2.Opacity = 1;
        icon2.Scale = scale;
        icon2.AnchorXUnits = StyleOptions.IconAnchorUnits.Pixels;
        icon2.AnchorYUnits = StyleOptions.IconAnchorUnits.Pixels;
        icon2.Source = $"./_content/OpenLayers.Blazor/img/marker-pin-{_pinColor.ToString().ToLowerInvariant()}.png";
    }

    protected void SetCustomImageStyle()
    {
        var icon = GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>();
        icon.Anchor = Anchor;
        icon.AnchorXUnits = StyleOptions.IconAnchorUnits.Pixels;
        icon.AnchorYUnits = StyleOptions.IconAnchorUnits.Pixels;
        icon.Size = Size;
        icon.Scale = Scale;
        icon.Source = Source;
    }

    protected override void SetFill(string color)
    {
        if (Type == MarkerType.MarkerAwesome)
            GetOrCreateStyleOptions<StyleOptions.TextOptions>(1).Fill = new StyleOptions.FillOptions() { Color = color };
        else if (Type == MarkerType.MarkerFlag)
        {
            GetOrCreateStyleOptions<StyleOptions.TextOptions>(0).BackgroundFill = new StyleOptions.FillOptions() { Color = color };
            var shape = GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>(0).Shape;
            if (shape != null) shape.Fill = color;
        }
        else
            base.SetFill(color);
    }

    protected override string? GetFill()
    {
        if (Type == MarkerType.MarkerAwesome)
            return GetOrCreateStyleOptions<StyleOptions.TextOptions>(1).Fill?.Color;
        else if (Type == MarkerType.MarkerFlag)
        {
            var backgroundFill = GetOrCreateStyleOptions<StyleOptions.TextOptions>(0).BackgroundFill;
            if (backgroundFill != null) return backgroundFill.Color;
        }
        return base.GetFill();
    }

    protected void SetAwesomeStyle()
    {
        var icon1 = GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>(0);
        icon1.Anchor = [0, 40];
        icon1.Size = [160, 60];
        icon1.Offset = [0, 0];
        icon1.Opacity = 1;
        icon1.Scale = .2;
        icon1.AnchorXUnits = StyleOptions.IconAnchorUnits.Pixels;
        icon1.AnchorYUnits = StyleOptions.IconAnchorUnits.Pixels;
        icon1.Source = "./_content/OpenLayers.Blazor/img/pin-back.png";

        var text1 = GetOrCreateStyleOptions<StyleOptions.TextOptions>(1);
        text1.Text = "\uf041"; // marker
        text1.Scale = 2;
        text1.Font = "900 18px 'Font Awesome 6 Free'";
        text1.TextBaseline = StyleOptions.CanvasTextBaseline.Bottom;
        text1.Fill = new StyleOptions.FillOptions() { Color = Fill ?? Options.DefaultFill };
        text1.Stroke = new StyleOptions.StrokeOptions() { Color = Stroke ?? Options.DefaultStroke, Width = 3 };

        var text2 = GetOrCreateStyleOptions<StyleOptions.TextOptions>(2);
        text2.Text = Text;
        text2.OffsetY = -22;
        text2.Scale = 1;
        text2.Font = "900 18px 'Font Awesome 6 Free'";
        text2.Fill = new StyleOptions.FillOptions() { Color = TextColor ?? Options.DefaultStroke };
    }

    protected void SetFlagStyle()
    {
        var padBottom = 2;
        var size = 10;
        var width = size;
        var height = size;

        var fill = Fill ?? Options.DefaultFill;
        var stroke = Stroke ?? Options.DefaultStroke;

        var text = GetOrCreateStyleOptions<StyleOptions.TextOptions>();
        text.Fill = new StyleOptions.FillOptions() { Color = TextColor ?? stroke };
        text.OffsetY = -size - padBottom + 1;
        text.OffsetX = -size;
        text.TextBaseline = StyleOptions.CanvasTextBaseline.Bottom;
        text.BackgroundFill = new StyleOptions.FillOptions() { Color = fill };
        text.BackgroundStroke = new StyleOptions.StrokeOptions() { Color = stroke };
        text.Padding = new double [] { 4, 5, padBottom, 5 };

        var icon = GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>();
        icon.Shape = new Polygon()
        {
            Coordinates = new double[][][]
            {
                [
                    [0, 0],
                    [width, 0],
                    [width / 2, height],
                    [0, 0]
                ]
            },
            Fill = fill, Stroke = stroke
        };
        icon.Size = [width, height];
        icon.Anchor = [width / 2, height];
        icon.Offset = [0, 0];
        icon.AnchorXUnits = StyleOptions.IconAnchorUnits.Pixels;
        icon.AnchorYUnits = StyleOptions.IconAnchorUnits.Pixels;
    }
}