namespace OpenLayers.Blazor;

public class MarkerFlag : Marker
{
    public MarkerFlag()
    {
    }

    public MarkerFlag(Coordinate coordinate) : base(coordinate)
    {
    }

    public MarkerFlag(Coordinate coordinate, string title) : base(coordinate)
    {
        Title = title;
    }
}