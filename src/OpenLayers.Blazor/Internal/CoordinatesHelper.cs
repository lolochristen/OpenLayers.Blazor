using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace OpenLayers.Blazor.Internal;

internal static class CoordinatesHelper
{
    public static Coordinate? GetPoint(dynamic coordinates)
    {
        if (coordinates is double[] c1)
            return new Coordinate(c1);
        if (coordinates is IEnumerable<double[]> c2)
            return new Coordinate(c2.First());
        if (coordinates is IEnumerable<IEnumerable<double[]>> c3)
            return new Coordinate(c3.First().First());
        return null;
    }

    public static double[] SetPoint(Coordinate coordinate)
    {
        return coordinate.Value;
    }

    public static bool IsSingleCoordinate(dynamic coordinates)
    {
        return !IsMultiCoordinate(coordinates) && (coordinates is double[] || ((coordinates is IEnumerable<double[]> c1) && c1.Count() == 1));
    }

    public static bool IsMultiCoordinate(dynamic coordinates)
    {
        return coordinates is IEnumerable<IEnumerable<double[]>>;
    }

    public static IEnumerable<Coordinate>? GetCoordinates(dynamic coordinates)
    {
        if (coordinates is double[] c1)
            return new[] { new Coordinate(c1) };
        if (coordinates is IEnumerable<double[]> c2)
            return c2.Select(p => new Coordinate(p));
        if (coordinates is IEnumerable<IEnumerable<double[]>> c3)
            return c3.First().Select(p => new Coordinate(p));
        return null;
    }

    public static double[][] SetCoordinates(IEnumerable<Coordinate> coordinates)
    {
        return coordinates.Select(p => p.Value).ToArray();
    }

    public static IEnumerable<IEnumerable<Coordinate>>? GetMultiCoordinates(dynamic coordinates)
    {
        if (coordinates is double[] c1)
            return new[] { new[] { new Coordinate(c1) } };
        if (coordinates is IEnumerable<double[]> c2)
            return new[] { c2.Select(p => new Coordinate(p)) };
        if (coordinates is IEnumerable<IEnumerable<double[]>> c3)
            return c3.Select(p => p.Select(p2 => new Coordinate(p2)));
        return null;
    }

    public static double[][][] SetMultiCoordinates(IEnumerable<IEnumerable<Coordinate>> coordinates)
    {
        return coordinates.Select(p => p.Select(p => p.Value).ToArray()).ToArray();
    }

    public static Coordinates? DeserializeCoordinates(JsonElement element)
    {
        int level = 0;
        var levelElement = element;
        while (levelElement.ValueKind == JsonValueKind.Array)
        {
            level++;
            levelElement  = levelElement[0];
        }

        switch (level)
        {
            case 1: 
                return element.Deserialize<double[]>();
            case 2: 
                return element.Deserialize<double[][]>();
            case 3: 
                return element.Deserialize<double[][][]>();
            default:
                return null;
        }
    }
}