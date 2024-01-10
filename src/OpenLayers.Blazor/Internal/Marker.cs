namespace OpenLayers.Blazor.Internal;

public class Marker : Shape
{
    public Marker()
    {
        Type = nameof(Marker);
        GeometryType = GeometryTypes.Point;
        Scale = .2;
    }

    public Marker(Coordinate point) : this()
    {
        Point = point.Value;
    }

    public float[]? Size { get; set; }

    public float[]? Anchor { get; set; }

    public double? Rotation { get; set; }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Size, Anchor);
    }
}