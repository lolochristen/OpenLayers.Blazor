using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor.Internal;

public class Feature : IEquatable<Feature>
{
    public Feature()
    {
        Type = nameof(Feature);
        Id = Guid.NewGuid().ToString();
        Coordinates = new Coordinates();
    }

    public object Id { get; set; }

    [JsonIgnore]
    public string? Type
    {
        get => GetProperty<string>("type");
        set => Properties["type"] = value;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GeometryTypes? GeometryType { get; set; }

    [Parameter]
    public Coordinates Coordinates { get; set; }

    [JsonIgnore]
    public Coordinate? Point
    {
        get => Coordinates.Point;
        set
        {
            Coordinates.Point = value;
            Coordinates.Type = CoordinatesType.Point;
        }
    }

    public Dictionary<string, dynamic> Properties { get; set; } = new();

    public bool Equals(Feature? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id) && GeometryType == other.GeometryType && Coordinates.Equals(other.Coordinates) && Properties.Equals(other.Properties);
    }

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

    public override int GetHashCode()
    {
        if (Coordinates is JsonElement || Coordinates != null)
            return HashCode.Combine(Id, GeometryType ?? GeometryTypes.None, Coordinates, Properties);
        else
            return HashCode.Combine(Id, GeometryType ?? GeometryTypes.None, Properties);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Feature)obj);
    }
}
