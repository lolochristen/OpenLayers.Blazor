namespace OpenLayers.Blazor.Internal;

public class Marker : Shape
{
    private Coordinate _coordinate;

    public Marker()
    {
        Geometry = new Geometry("Point");
        Scale = .2;
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

    public float[]? Size { get; set; }

    public float[]? Anchor { get; set; }

    public override int GetHashCode()
    {
        return base.GetHashCode() & _coordinate.GetHashCode();
    }
}