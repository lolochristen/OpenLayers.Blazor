using Microsoft.AspNetCore.Components;

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
}