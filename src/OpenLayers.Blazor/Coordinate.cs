/* Unmerged change from project 'OpenLayers.Blazor (net6.0)'
Before:
namespace OpenLayers.Blazor.Model;
After:
using OpenLayers;
using OpenLayers.Blazor;
using OpenLayers.Blazor;
using OpenLayers.Blazor.Model;
*/

using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

[JsonConverter(typeof(CoordinateConverter))]
public class Coordinate : IEquatable<Coordinate>
{
    public Coordinate()
    {
    }

    /// <summary>
    ///     New Point
    /// </summary>
    /// <param name="coordinates">Latitude, Longitude</param>
    public Coordinate(double x, double y)
    {
        X = x;
        Y = y;
    }

    public Coordinate(Coordinate coordinate)
    {
        X = coordinate.X;
        Y = coordinate.Y;
    }


    public Coordinate(double[] coordinates)
    {
        if (coordinates.Length < 2)
            throw new ArgumentException(nameof(coordinates));
        Value = coordinates;
    }

    [JsonIgnore]
    public double Latitude => X;

    public double Y
    {
        get => Value[1];
        set => Value[1] = value;
    }

    [JsonIgnore]
    public double Longitude => Y;

    public double X
    {
        get => Value[0];
        set => Value[0] = value;
    }

    /// <summary>
    ///     Coordinate in OpenLayers Style: [Longitude, Latitude]
    /// </summary>
    [JsonIgnore]
    public double[] Value { get; set; } = new double[2] { 0, 0 };

    public bool Equals(Coordinate? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return GetHashCode() == other.GetHashCode();
    }

    public override string ToString()
    {
        return $"{X}, {Y}";
    }

    public static implicit operator Coordinate(string val)
    {
        return Parse(val);
    }

    public static implicit operator Coordinate(double[] val)
    {
        if (val == null)
            return null;
        return new Coordinate(val[0], val[1]);
    }

    public static Coordinate Parse(string val)
    {
        val = val.Trim();
        var parts = val.Split(',', '/', ':');

        if (parts.Length != 2)
            throw new InvalidOperationException("Cannot parse coordinate");

        return new Coordinate(double.Parse(parts[0]), double.Parse(parts[1]));
    }

    public static bool TryParse(string val, out Coordinate? result)
    {
        try
        {
            result = Parse(val);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    ///     Distance in kilometers
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
    ///     Calcola un punto distante in km
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

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Coordinate)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}