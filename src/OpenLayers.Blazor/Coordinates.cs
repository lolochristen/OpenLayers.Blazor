using System.Collections;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

/// <summary>
/// Represents a collection of coordinates that can be a single point, list, or multi-dimensional list.
/// </summary>
[JsonConverter(typeof(CoordinatesConverter))]
public class Coordinates : IEnumerable<IList<Coordinate>>, IEquatable<Coordinates>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinates"/> class as a list type.
    /// </summary>
    public Coordinates()
    {
        Type = CoordinatesType.List;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinates"/> class from a list of coordinates.
    /// </summary>
    /// <param name="coordinates">The list of coordinates.</param>
    public Coordinates(IList<Coordinate> coordinates)
    {
        Values[0] = coordinates;
        Type = CoordinatesType.List;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinates"/> class from a 2D array.
    /// </summary>
    /// <param name="coordinates">The 2D array of coordinate values.</param>
    public Coordinates(double[][] coordinates)
    {
        Values[0] = new List<Coordinate>(coordinates.Select(p => new Coordinate(p)));
        Type = CoordinatesType.List;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinates"/> class from a single coordinate point.
    /// </summary>
    /// <param name="c">The coordinate point.</param>
    public Coordinates(Coordinate c)
    {
        Values[0] = new List<Coordinate> { c };
        Type = CoordinatesType.Point;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinates"/> class from a multi-dimensional list.
    /// </summary>
    /// <param name="coordinates">The multi-dimensional list of coordinates.</param>
    public Coordinates(IList<IList<Coordinate>> coordinates)
    {
        Values = coordinates;
        Type = CoordinatesType.MultiList;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinates"/> class from a 3D array.
    /// </summary>
    /// <param name="coordinates">The 3D array of coordinate values.</param>
    public Coordinates(double[][][] coordinates)
    {
        Values = coordinates.Select(p => new List<Coordinate>(p.Select(p2 => new Coordinate(p2)).ToList()) as IList<Coordinate>).ToList();
        Type = CoordinatesType.MultiList;
    }

    /// <summary>
    /// Gets or sets the type of coordinate structure (Point, List, or MultiList).
    /// </summary>
    public CoordinatesType Type { get; set; }

    /// <summary>
    /// Gets the underlying collection of coordinate lists.
    /// </summary>
    public IList<IList<Coordinate>> Values { get; } = new List<IList<Coordinate>> { new List<Coordinate>() };

    /// <summary>
    /// Gets or sets the default (first) list of coordinates.
    /// </summary>
    [JsonIgnore]
    public IList<Coordinate> Default
    {
        get => Values[0];
        set
        {
            if (Values.Count == 0)
                Values.Add(value);
            else
                Values[0] = value;
        }
    }

    /// <summary>
    /// Gets or sets the coordinate list at the specified index.
    /// </summary>
    /// <param name="key">The zero-based index.</param>
    /// <returns>The coordinate list at the specified index.</returns>
    public IList<Coordinate> this[int key]
    {
        get => Values[key];
        set => Values[key] = value;
    }

    /// <summary>
    /// Gets or sets the single point coordinate when Type is Point.
    /// </summary>
    [JsonIgnore]
    public Coordinate Point
    {
        get => Default.Count > 0 ? Default[0] : Coordinate.Empty;
        set
        {
            if (Default.Count == 0)
                Default.Add(value);
            else
                Default[0] = value;
            Type = CoordinatesType.Point;
        }
    }

    /// <inheritdoc/>
    public IEnumerator<IList<Coordinate>> GetEnumerator()
    {
        return Values.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc/>
    public bool Equals(Coordinates? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type && Values.SequenceEqual(other.Values, new CoordinatesEqualityComparer());
    }

    /// <summary>
    /// Implicitly converts a 1D double array to a Coordinates point.
    /// </summary>
    /// <param name="c">The array of coordinate values.</param>
    public static implicit operator Coordinates(double[] c)
    {
        return new Coordinates(c);
    }

    /// <summary>
    /// Implicitly converts a 2D double array to a Coordinates list.
    /// </summary>
    /// <param name="c">The 2D array of coordinate values.</param>
    public static implicit operator Coordinates(double[][] c)
    {
        return new Coordinates(c);
    }

    /// <summary>
    /// Implicitly converts a 3D double array to a Coordinates multi-list.
    /// </summary>
    /// <param name="c">The 3D array of coordinate values.</param>
    public static implicit operator Coordinates(double[][][] c)
    {
        return new Coordinates(c);
    }

    /// <summary>
    /// Determines whether two coordinates are equal.
    /// </summary>
    public static bool operator ==(Coordinates c1, Coordinates? c2)
    {
        return c1.Equals(c2);
    }

    /// <summary>
    /// Determines whether two coordinates are not equal.
    /// </summary>
    public static bool operator !=(Coordinates c1, Coordinates? c2)
    {
        return !c1.Equals(c2);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{nameof(Coordinates)}:{Type}[{(Type == CoordinatesType.List ? Default.Count : Values.Count)}]";
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Coordinates)obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine((int)Type, Values);
    }
}