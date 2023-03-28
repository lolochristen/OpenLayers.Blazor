namespace OpenLayers.Blazor;

public class Marker : Shape
{
    private Coordinate _coordinate;

    public Marker()
    {
        Geometry = new Geometry("Point");
    }

    public Marker(Coordinate coordinate) : this()
    {
        Coordinate = coordinate;
    }

    public Coordinate Coordinate
    {
        get => _coordinate;
        set
        {
            _coordinate = value;
            Geometry.Coordinates = value?.Coordinates;
        }
    }
}