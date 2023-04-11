namespace OpenLayers.Blazor.Internal;

public class ShapeCircle : Shape
{
    private Coordinate? _coordinate;

    public ShapeCircle()
    {
        Geometry = new Geometry("Point");
    }

    /// <summary>
    ///     Draw a circle
    /// </summary>
    /// <param name="center">Center</param>
    /// <param name="radius">Radius in km</param>
    public ShapeCircle(Coordinate center, double radius)
    {
        Geometry = new Geometry("Point");
        Coordinate = center;
        Radius = radius * 1000;
    }

    public Coordinate? Coordinate
    {
        get => _coordinate;
        set
        {
            _coordinate = value;
            Geometry.Coordinates = value?.Coordinates;
        }
    }
}