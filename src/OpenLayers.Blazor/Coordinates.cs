using System.Collections;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

/// <summary>
///     Represents a list of coordinates
/// </summary>
[JsonConverter(typeof(CoordinatesConverter))]
public class Coordinates : IEnumerable<IList<Coordinate>>, IEquatable<Coordinates>
{
    public Coordinates()
    {
        Type = CoordinatesType.List;
    }

    public Coordinates(IList<Coordinate> coordinates)
    {
        Values[0] = coordinates;
        Type = CoordinatesType.List;
    }

    public Coordinates(double[][] coordinates)
    {
        Values[0] = new List<Coordinate>(coordinates.Select(p => new Coordinate(p)));
        Type = CoordinatesType.List;
    }

    public Coordinates(Coordinate c)
    {
        Values[0] = new List<Coordinate> { c };
        Type = CoordinatesType.Point;
    }

    public Coordinates(IList<IList<Coordinate>> coordinates)
    {
        Values = coordinates;
        Type = CoordinatesType.MultiList;
    }

    public Coordinates(double[][][] coordinates)
    {
        Values = coordinates.Select(p => new List<Coordinate>(p.Select(p2 => new Coordinate(p2)).ToList()) as IList<Coordinate>).ToList();
        Type = CoordinatesType.MultiList;
    }

    public CoordinatesType Type { get; set; }

    public IList<IList<Coordinate>> Values { get; } = new List<IList<Coordinate>> { new List<Coordinate>() };

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

    public IList<Coordinate> this[int key]
    {
        get => Values[key];
        set => Values[key] = value;
    }

    [JsonIgnore]
    public Coordinate? Point
    {
        get => Default.Count > 0 ? Default[0] : null;
        set
        {
            if (value is not null)
            {
                if (Default.Count == 0)
                    Default.Add(value);
                else
                    Default[0] = value;
                Type = CoordinatesType.Point;
            }
        }
    }

    public IEnumerator<IList<Coordinate>> GetEnumerator()
    {
        return Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Equals(Coordinates? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type && Values.SequenceEqual(other.Values, new CoordinatesEqualityComparer());
    }

    public static implicit operator Coordinates(double[] c)
    {
        return new Coordinates(c);
    }

    public static implicit operator Coordinates(double[][] c)
    {
        return new Coordinates(c);
    }

    public static implicit operator Coordinates(double[][][] c)
    {
        return new Coordinates(c);
    }

    public static bool operator ==(Coordinates c1, Coordinates? c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(Coordinates c1, Coordinates? c2)
    {
        return !c1.Equals(c2);
    }

    public override string ToString()
    {
        return $"{nameof(Coordinates)}:{Type}[{(Type == CoordinatesType.List ? Default.Count : Values.Count)}]";
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Coordinates)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Type, Values);
    }
}