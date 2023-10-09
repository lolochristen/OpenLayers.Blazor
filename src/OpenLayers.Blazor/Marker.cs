using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

public class Marker : Shape<Internal.Marker>
{
    public Marker() : base (new Internal.Marker())
    {
    }

    public Marker(MarkerType type, Coordinate coordinate, string? title = null) : this()
    {
        Type = type;
        Coordinate = coordinate;
        Title = title;
    }

    public Marker(Coordinate coordinate, char icon) : this()
    {
        Type = MarkerType.MarkerAwesome;
        Coordinate = coordinate;
        Label = icon.ToString();
    }

    public Marker(Coordinate coordinate, string imageSource, float width, float height, float anchorX, float anchorY) : this()
    {
        Type = MarkerType.MarkerCustomImage;
        Coordinate = coordinate;
        Content = imageSource;
        InternalFeature.Size = new[] { width, height };
        InternalFeature.Anchor = new[] { anchorX, anchorY };
    }

    [Parameter]
    public MarkerType Type
    {
        get => InternalFeature.Style != null ? Enum.Parse<MarkerType>(InternalFeature.Style) : MarkerType.MarkerPin;
        set => InternalFeature.Style = value.ToString();
    }

    [Parameter]
    public float[]? Size
    {
        get => InternalFeature.Size;
        set => InternalFeature.Size = value;
    }

    [Parameter]
    public float[]? Anchor
    {
        get => InternalFeature.Anchor;
        set => InternalFeature.Anchor = value;
    }


    [Parameter]
    public Coordinate? Coordinate
    {
        get => InternalFeature.Point;
        set => InternalFeature.Point = value?.Value;
    }
}