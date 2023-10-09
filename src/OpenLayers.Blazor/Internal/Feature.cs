using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor.Internal;

public class Feature : IEquatable<Feature>
{
    public Feature()
    {
        Type = nameof(Feature);
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    [JsonIgnore]
    public string? Type
    {
        get => GetProperty<string>("type");
        set => Properties["type"] = value;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GeometryTypes? GeometryType { get; set; }

    public List<double[]> Coordinates { get; set; } = new();

    [JsonIgnore]
    public double[]? Point
    {
        get => Coordinates.FirstOrDefault();
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (Coordinates.Count == 0)
                Coordinates.Add(value);
            else 
                Coordinates[0] = value;
        }
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

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, GeometryType, Coordinates, Properties);
    }


    public bool Equals(Feature? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id) && GeometryType == other.GeometryType && Coordinates.Equals(other.Coordinates) && Properties.Equals(other.Properties);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Feature)obj);
    }
}
