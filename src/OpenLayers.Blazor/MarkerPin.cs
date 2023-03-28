namespace OpenLayers.Blazor;

public class MarkerPin : Marker
{
    public MarkerPin()
    {
    }

    public MarkerPin(Coordinate coordinate) : base(coordinate)
    {
    }

    public MarkerPin(Coordinate coordinate, string color) : base(coordinate)
    {
        Color = color;
    }
}