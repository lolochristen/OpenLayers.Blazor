using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace OpenLayers.Blazor.Internal;

internal static class CoordinatesHelper
{
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