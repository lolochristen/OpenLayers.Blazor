using System.Globalization;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

[JsonConverter(typeof(CoordinateConverter))]
public class Coordinate : IEquatable<Coordinate>
{
    private static readonly char[] _separatorsAlt = new[] { '/', ':' };
    private static readonly char[] _separators = new[] { ',', '/', ':' };

    private readonly double[] _value;

    public Coordinate()
    {
        _value = new double[2];
    }

    /// <summary>
    ///     New Point
    /// </summary>
    /// <param name="coordinates">Latitude, Longitude</param>
    public Coordinate(double x, double y)
    {
        _value = new[] { x, y };
    }

    public Coordinate(Coordinate coordinate)
    {
        _value = coordinate._value;
    }

    public Coordinate(double[] coordinates)
    {
        if (coordinates.Length < 2)
            throw new ArgumentException(nameof(coordinates));
        _value = coordinates;
    }

    public double[] Value => _value;

    [JsonIgnore]
    public double Latitude => Y;

    public double Y
    {
        get => _value[1];
        set => _value[1] = value;
    }

    [JsonIgnore]
    public double Longitude => X;

    public double X
    {
        get => _value[0];
        set => _value[0] = value;
    }

    public override string ToString()
    {
        return $"{X}/{Y}";
    }

    public static implicit operator Coordinate(string val)
    {
        return Parse(val);
    }

    public static implicit operator Coordinate(double[] val)
    {
        return new Coordinate(val);
    }

    public static bool operator ==(Coordinate c1, Coordinate c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(Coordinate c1, Coordinate c2)
    {
        return !c1.Equals(c2);
    }

    public double this[int key]
    {
        get => _value[key];
        set => _value[key] = value;
    }

    public static Coordinate Parse(string val, IFormatProvider? formatProvider = null)
    {
        val = val.Trim();

        var numberFormatInfo = formatProvider?.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo ?? CultureInfo.CurrentCulture.NumberFormat;

        var parts = val.Split(numberFormatInfo.CurrencyDecimalSeparator == "," ? _separatorsAlt : _separators);

        if (parts.Length != 2)
            throw new InvalidOperationException("Cannot parse coordinate");

        return new Coordinate(double.Parse(parts[0], formatProvider), double.Parse(parts[1], formatProvider));
    }

    public static bool TryParse(string val, out Coordinate? result, IFormatProvider? formatProvider = null)
    {
        try
        {
            result = Parse(val, formatProvider);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    ///     Calculates distance in kilometers
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public double DistanceTo(Coordinate target, int decimals = 2)
    {
        var baseRad = Math.PI * Y / 180;
        var targetRad = Math.PI * target.Y / 180;
        var theta = X - target.X;
        var thetaRad = Math.PI * theta / 180;

        var dist =
            Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
            Math.Cos(targetRad) * Math.Cos(thetaRad);
        dist = Math.Acos(dist);

        dist = dist * 180 / Math.PI;
        dist = dist * 60 * 1.1515 * 1.609344;

        return Math.Round(dist, decimals);
    }

    /// <summary>
    ///     Calculates distance.
    /// </summary>
    /// <param name="distance">distance in km</param>
    /// <returns></returns>
    public Coordinate PointByDistance(double distance)
    {
        var rad = 64.10;

        rad *= Math.PI / 180;

        var angDist = distance / 6371;
        var latitude = Y;
        var longitude = X;

        latitude *= Math.PI / 180;
        longitude *= Math.PI / 180;

        var lat2 = Math.Asin(Math.Sin(latitude) * Math.Cos(angDist) + Math.Cos(latitude) * Math.Sin(angDist) * Math.Cos(rad));
        var forAtana = Math.Sin(rad) * Math.Sin(angDist) * Math.Cos(latitude);
        var forAtanb = Math.Cos(angDist) - Math.Sin(latitude) * Math.Sin(lat2);
        var lon2 = longitude + Math.Atan2(forAtana, forAtanb);

        lat2 *= 180 / Math.PI;
        lon2 *= 180 / Math.PI;

        return new Coordinate(lat2, lon2);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public bool Equals(Coordinate other)
    {
        return _value[0].Equals(other._value[0]) && _value[1].Equals(other._value[1]);
    }
}