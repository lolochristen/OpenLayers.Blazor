namespace OpenLayers.Blazor;

/// <summary>
///     Represents an extent or bounding box of a layer or visible map
/// </summary>
public class Extent : IEquatable<Extent>
{
    public Extent()
    {
        X1 = 0;
        Y1 = 0;
        X2 = 0;
        Y2 = 0;
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

    public bool Equals(Extent? other)
    {
        if (other is null)
            return false;
        return X1.Equals(other.X1) && Y1.Equals(other.Y1) && X2.Equals(other.X2) && Y2.Equals(other.Y2);
    }

    public override bool Equals(object? obj)
    {
        return obj is Extent other && Equals(other);
    }
}