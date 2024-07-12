using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor.Internal;

public class Feature
{
    public Feature()
    {
        Type = nameof(Feature);
        Id = Guid.NewGuid().ToString();
        Coordinates = new Coordinates();
    }

    public string Id { get; set; }

    [JsonIgnore]
    public string? Type
    {
        get => GetProperty<string>("type");
        set => Properties["type"] = value;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GeometryTypes? GeometryType { get; set; }

    [Parameter] public Coordinates Coordinates { get; set; }

    [JsonIgnore]
    public Coordinate Point
    {
        get => Coordinates.Point;
        set => Coordinates.Point = value;
    }

    public Dictionary<string, dynamic> Properties { get; set; } = new();

    public T? GetProperty<T>(string key)
    {
        if (Properties.ContainsKey(key))
        {
            if (Properties[key] is JsonElement jsonElement)
                return jsonElement.Deserialize<T>();
            return (T)Properties[key];
        }

        return default;
    }
}