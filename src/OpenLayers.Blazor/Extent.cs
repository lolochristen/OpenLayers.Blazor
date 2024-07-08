namespace OpenLayers.Blazor;

/// <summary>
///     Represents an extent or bounding box of a layer or visible map
/// </summary>
public class Extent : IEquatable<Extent>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Extent" /> class with default values.
    /// </summary>
    public Extent()
    {
        X1 = 0;
        Y1 = 0;
        X2 = 0;
        Y2 = 0;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Extent" /> class with the specified array.
    /// </summary>
    /// <param name="array">The array containing the extent values.</param>
    /// <exception cref="ArgumentException">Thrown when the array length is not 4.</exception>
    public Extent(double[] array)
    {
        if (array.Length != 4)
            throw new ArgumentException("Extent must contain 4 elements.", nameof(array));

        X1 = array[0];
        Y1 = array[1];
        X2 = array[2];
        Y2 = array[3];
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Extent" /> class with the specified coordinates.
    /// </summary>
    /// <param name="x1">The X coordinate of the bottom left corner.</param>
    /// <param name="y1">The Y coordinate of the bottom left corner.</param>
    /// <param name="x2">The X coordinate of the top right corner.</param>
    /// <param name="y2">The Y coordinate of the top right corner.</param>
    public Extent(double x1, double y1, double x2, double y2)
    {
        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;
    }

    /// <summary>
    ///     Gets or sets the X coordinate of the bottom left corner.
    /// </summary>
    public double X1 { get; set; }

    /// <summary>
    ///     Gets or sets the Y coordinate of the bottom left corner.
    /// </summary>
    public double Y1 { get; set; }

    /// <summary>
    ///     Gets or sets the X coordinate of the top right corner.
    /// </summary>
    public double X2 { get; set; }

    /// <summary>
    ///     Gets or sets the Y coordinate of the top right corner.
    /// </summary>
    public double Y2 { get; set; }

    /// <summary>
    ///     Gets or sets the coordinate value at the specified index.
    /// </summary>
    /// <param name="index">The index of the coordinate value.</param>
    /// <returns>The coordinate value at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is out of range.</exception>
    public double this[int index]
    {
        get
        {
            return index switch
            {
                0 => X1,
                1 => Y1,
                2 => X2,
                3 => Y2,
                _ => throw new IndexOutOfRangeException()
            };
        }
        set
        {
            switch (index)
            {
                case 0:
                    X1 = value;
                    break;
                case 1:
                    Y1 = value;
                    break;
                case 2:
                    X2 = value;
                    break;
                case 3:
                    Y2 = value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }

    /// <summary>
    ///     Determines whether the specified <see cref="Extent" /> object is equal to the current <see cref="Extent" /> object.
    /// </summary>
    /// <param name="other">The <see cref="Extent" /> object to compare with the current <see cref="Extent" /> object.</param>
    /// <returns>
    ///     true if the specified <see cref="Extent" /> object is equal to the current <see cref="Extent" /> object;
    ///     otherwise, false.
    /// </returns>
    public bool Equals(Extent? other)
    {
        if (other is null)
            return false;
        return X1.Equals(other.X1) && Y1.Equals(other.Y1) && X2.Equals(other.X2) && Y2.Equals(other.Y2);
    }

    /// <summary>
    ///     Calculates the hash code for the <see cref="Extent" /> object.
    /// </summary>
    /// <returns>The calculated hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(X1, Y1, X2, Y2);
    }

    /// <summary>
    ///     Returns a string representation of the <see cref="Extent" /> object.
    /// </summary>
    /// <returns>A string representation of the <see cref="Extent" /> object.</returns>
    public override string ToString()
    {
        return $"{X1}/{Y1}:{X2}/{Y2}";
    }

    /// <summary>
    ///     Converts the <see cref="Extent" /> object to an array of doubles.
    /// </summary>
    /// <returns>An array of doubles representing the extent.</returns>
    public double[] ToArray()
    {
        return new[] { X1, Y1, X2, Y2 };
    }

    /// <summary>
    ///     Determines whether the specified object is equal to the current <see cref="Extent" /> object.
    /// </summary>
    /// <param name="obj">The object to compare with the current <see cref="Extent" /> object.</param>
    /// <returns>true if the specified object is equal to the current <see cref="Extent" /> object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is Extent other && Equals(other);
    }

    /// <summary>
    ///     Implicitly converts the <see cref="Extent" /> object to an array of doubles.
    /// </summary>
    /// <param name="extent">The <see cref="Extent" /> object to convert.</param>
    /// <returns>An array of doubles representing the extent.</returns>
    public static implicit operator double[](Extent extent)
    {
        return extent.ToArray();
    }

    /// <summary>
    ///     Implicitly converts an array of doubles to a <see cref="Extent" /> object.
    /// </summary>
    /// <param name="values">The array of doubles to convert.</param>
    /// <returns>A <see cref="Extent" /> object representing the extent.</returns>
    public static implicit operator Extent?(double[]? values)
    {
        return values != null ? new Extent(values) : null;
    }
}