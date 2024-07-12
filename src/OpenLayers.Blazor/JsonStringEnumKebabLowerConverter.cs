using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

internal class JsonStringEnumKebabLowerConverter : JsonStringEnumConverter
{
    public JsonStringEnumKebabLowerConverter() : base(JsonNamingPolicy.KebabCaseLower, false)
    {
    }
}