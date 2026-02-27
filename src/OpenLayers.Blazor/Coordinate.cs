using System.Globalization;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

/// <summary>
///     Represents a coordinate of two points
/// </summary>
[JsonConverter(typeof(CoordinateConverter))]
public struct Coordinate : IEquatable<Coordinate>
{
    private static readonly char[] _separatorsAlt = { '/', ':' };
    private static readonly char[] _separators = { ',', '/', ':' };
    
    /// <summary>
    /// Gets an empty coordinate (0, 0).
    /// </summary>
    public static Coordinate Empty => new();

    /// <summary>
    ///     X
    /// </summary>
    public double X;

    /// <summary>
    ///     Y
    /// </summary>
    public double Y;

    /// <summary>
    ///     Initializes a new instance of <see cref="Coordinate" />.
    /// </summary>
    public Coordinate()
    {
        X = 0;
        Y = 0;
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Coordinate" />.
    /// </summary>
    /// <param name="coordinates">X/Longitude, Y/Latitude</param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Coordinate(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Coordinate" />.
    /// </summary>
    /// <param name="coordinate"></param>
    public Coordinate(Coordinate coordinate)
    {
        X = coordinate.X;
        Y = coordinate.Y;
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Coordinate" />.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <exception cref="ArgumentException"></exception>
    public Coordinate(double[] coordinates)
    {
        if (coordinates.Length < 2)
            throw new ArgumentException(nameof(coordinates));
        X = coordinates[0];
        Y = coordinates[1];
    }

    /// <summary>
    ///     Latitude
    /// </summary>
    [JsonIgnore]
    public double Latitude => Y;

    /// <summary>
    ///     Longitude
    /// </summary>
    [JsonIgnore]
    public double Longitude => X;

    /// <inheritdoc />
    public bool Equals(Coordinate other)
    {
        var precision = 0.0000001;
        return Math.Abs(X - other.X) < precision && Math.Abs(Y - other.Y) < precision;
    }

    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{X}/{Y}";
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Coordinate" />.
    /// </summary>
    /// <param name="val"></param>
    public static implicit operator Coordinate(string val)
    {
        return Parse(val);
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Coordinate" />.
    /// </summary>
    /// <param name="val"></param>
    public static implicit operator Coordinate(double[] val)
    {
        return new Coordinate(val);
    }

    /// <summary>
    ///     Indicates if given objects are equal
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <returns>bool</returns>
    public static bool operator ==(Coordinate? c1, Coordinate? c2)
    {
        if (c1 is null || c2 is null)
            return false;
        return c1.Equals(c2);
    }

    /// <summary>
    ///     Indicates if given objects are not equal
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <returns></returns>
    public static bool operator !=(Coordinate? c1, Coordinate? c2)
    {
        return !c1.Equals(c2);
    }


    /// <summary>
    ///     Parses a new instances of <see cref="Coordinate" />
    /// </summary>
    /// <param name="val"></param>
    /// <param name="formatProvider"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Coordinate Parse(string val, IFormatProvider? formatProvider = null)
    {
        val = val.Trim();

        var numberFormatInfo = formatProvider?.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo ?? CultureInfo.CurrentCulture.NumberFormat;

        var parts = val.Split(numberFormatInfo.CurrencyDecimalSeparator == "," ? _separatorsAlt : _separators);

        if (parts.Length != 2)
            throw new InvalidOperationException("Cannot parse coordinate");

        return new Coordinate(double.Parse(parts[0], formatProvider), double.Parse(parts[1], formatProvider));
    }

    /// <summary>
    ///     Parses a new instances of <see cref="Coordinate" />
    /// </summary>
    /// <param name="val"></param>
    /// <param name="result"></param>
    /// <param name="formatProvider"></param>
    /// <returns></returns>
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
    ///     Calculates distance to another coordinate in kilometers.
    /// </summary>
    /// <param name="target">The target coordinate.</param>
    /// <param name="decimals">Number of decimal places to round to.</param>
    /// <returns>Distance in kilometers.</returns>
    public double DistanceTo(Coordinate target, int decimals = 2)
    {
        if (decimals < 0)
            throw new ArgumentOutOfRangeException(nameof(decimals), "Decimals must be non-negative");

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

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(X.GetHashCode(), Y.GetHashCode());
    }
}