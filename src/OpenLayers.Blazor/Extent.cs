namespace OpenLayers.Blazor;

/// <summary>
///     Represents an extent or bounding box of a layer or visible map
/// </summary>
public class Extent : IEquatable<Extent>
{
    public Extent()
    {
    }

    public Extent(double[] array)
    {
        if (array.Length != 4)
            throw new ArgumentException("Extent must contain 4 elements.", nameof(array));

        X1 = array[0];
        Y1 = array[1];
        X2 = array[2];
        Y2 = array[3];
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
        return HashCode.Combine(X1, Y1, X2, Y2);
    }

    public override string ToString()
    {
        return $"{X1}/{Y1}:{X2}/{Y2}";
    }

    public double[] ToArray()
    {
        return new[] { X1, Y1, X2, Y2 };
    }
}