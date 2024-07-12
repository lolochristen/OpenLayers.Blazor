using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

internal class JsonStringEnumKebabLowerConverter : JsonStringEnumConverter
{
#if NET8_0
    public JsonStringEnumKebabLowerConverter() : base(JsonNamingPolicy.KebabCaseLower, false)
#else
    public JsonStringEnumKebabLowerConverter() : base(JsonNamingPolicy.CamelCase, false) // TODO: implementation of kebabcaselower for net7/net8. some enums for styles will not work.
#endif
    {
    }
}