namespace OpenLayers.Blazor.Internal;

public class MarkerAwesome : Marker
{
    public MarkerAwesome()
    {
    }

    public MarkerAwesome(Coordinate coordinate) : base(coordinate)
    {
    }

    public MarkerAwesome(Coordinate coordinate, int icon) : base(coordinate)
    {
        Label = char.ConvertFromUtf32(icon);
    }

    public MarkerAwesome(Coordinate coordinate, string label) : base(coordinate)
    {
        Label = label;
    }
}