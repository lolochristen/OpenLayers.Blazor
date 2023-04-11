/* Unmerged change from project 'OpenLayers.Blazor (net6.0)'
Before:
namespace OpenLayers.Blazor.Model;
After:
using OpenLayers;
using OpenLayers.Blazor;
using OpenLayers.Blazor;
using OpenLayers.Blazor.Model;
*/

namespace OpenLayers.Blazor;

public class Coordinate : IEquatable<Coordinate>
{
    public Coordinate()
    {
    }

    /// <summary>
    ///     New Point
    /// </summary>
    /// <param name="coordinates">Latitude, Longitude</param>
    public Coordinate(double y, double x)
    {
        Y = y;
        X = x;
    }

    public Coordinate(Coordinate coordinate)
    {
        Y = coordinate.Y;
        X = coordinate.X;
    }

    public double Latitude => Y;

    public double Y
    {
        get => Coordinates[1];
        set => Coordinates[1] = value;
    }

    public double Longitude => X;

    public double X
    {
        get => Coordinates[0];
        set => Coordinates[0] = value;
    }

    /// <summary>
    ///     Coordinate in OpenLayers Style: [Longitude, Latitude]
    /// </summary>
    public double[] Coordinates { get; set; } = new double[2] { 0, 0 };

    public bool Equals(Coordinate? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Coordinates.Equals(other.Coordinates);
    }

    public override string ToString()
    {
        return $"{X}/{Y}";
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
        return Coordinates.GetHashCode();
    }
}