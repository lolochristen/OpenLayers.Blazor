using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace OpenLayers.Blazor;

public class Marker : Shape<Internal.Marker>
{
    public Marker() : base(new Internal.Marker())
    {
    }

    public Marker(MarkerType type, Coordinate coordinate, string? title = null)
    {
        Type = type;
        Coordinate = coordinate;
        Title = title;
    }

    public Marker(Coordinate coordinate, int icon)
    {
        Type = MarkerType.MarkerAwesome;
        Coordinate = coordinate;
        Label = icon.ToString();
    }

    public Marker(Coordinate coordinate, string imageSource, float width, float height, float anchorX, float anchorY)
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
        get => Enum.Parse<MarkerType>(InternalFeature.Kind);
        set => InternalFeature.Kind = value.ToString();
    }

    [Parameter]
    public Coordinate Coordinate
    {
        get => InternalFeature.Coordinate;
        set => InternalFeature.Coordinate = value;
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
}