using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

/// <summary>
/// A Marker component to attach to a <see cref="Map"/>
/// </summary>
public class Marker : Shape<Internal.Marker>
{
    /// <summary>
    /// Default Constructor
    /// </summary>
    public Marker() : base(new Internal.Marker())
    {
    }

    /// <summary>
    /// Constructor with key parameters.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="coordinate"></param>
    /// <param name="title"></param>
    public Marker(MarkerType type, Coordinate coordinate, string? title = null) : this()
    {
        Type = type;
        Coordinate = coordinate;
        Title = title;
    }

    /// <summary>
    /// Constructor for a Marker <see cref="MarkerType.MarkerAwesome"/>
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="icon"></param>
    public Marker(Coordinate coordinate, char icon) : this()
    {
        Type = MarkerType.MarkerAwesome;
        Coordinate = coordinate;
        Label = icon.ToString();
    }

    /// <summary>
    /// Extended consturctor.
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="imageSource"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="anchorX"></param>
    /// <param name="anchorY"></param>
    public Marker(Coordinate coordinate, string imageSource, float width, float height, float anchorX, float anchorY) : this()
    {
        Type = MarkerType.MarkerCustomImage;
        Coordinate = coordinate;
        Content = imageSource;
        InternalFeature.Size = new[] { width, height };
        InternalFeature.Anchor = new[] { anchorX, anchorY };
    }

    /// <summary>
    /// Type of the marker: <see cref="MarkerType"/>
    /// </summary>
    [Parameter]
    public MarkerType Type
    {
        get => InternalFeature.Style != null ? Enum.Parse<MarkerType>(InternalFeature.Style) : MarkerType.MarkerPin;
        set => InternalFeature.Style = value.ToString();
    }

    /// <summary>
    /// Size of the marker in width/height
    /// </summary>
    [Parameter]
    public float[]? Size
    {
        get => InternalFeature.Size;
        set => InternalFeature.Size = value;
    }

    /// <summary>
    /// Anchor point for the marker image/icon
    /// </summary>
    [Parameter]
    public float[]? Anchor
    {
        get => InternalFeature.Anchor;
        set => InternalFeature.Anchor = value;
    }


    /// <summary>
    /// Coordinate of the marker.
    /// </summary>
    [Parameter]
    public Coordinate? Coordinate
    {
        get => InternalFeature.Point;
        set => InternalFeature.Point = value?.Value;
    }

    /// <summary>
    /// Icon Rotation in radiant
    /// </summary>
    [Parameter]
    public double? Rotation
    {
        get => InternalFeature.Rotation;
        set => InternalFeature.Rotation = value;
    }
}