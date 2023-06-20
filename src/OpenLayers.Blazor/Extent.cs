namespace OpenLayers.Blazor;

/// <summary>
/// Represents an extent or bounding box of a layer or visible map
/// </summary>
public class Extent : IEquatable<Extent>
{
    public Extent()
    {
    }

    public Extent(double x1, double y1, double x2, double y2)
    {
        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;
    }

    public double X1 { get; set; }
    public double Y1 { get; set; }
    public double X2 { get; set; }
    public double Y2 { get; set; }

    public bool Equals(Extent? other)
    {
        if (other == null)
            return false;

        return GetHashCode() == other.GetHashCode();
    }

    public override int GetHashCode()
    {
        return X1.GetHashCode() + Y1.GetHashCode() + X2.GetHashCode() + Y2.GetHashCode();
    }

    public override string ToString() => $"{X1}/{Y1}:{X2}/{Y2}";
}