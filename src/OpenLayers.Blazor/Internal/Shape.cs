using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor.Internal;

public class Shape : Feature
{
    public Shape()
    {
        Kind = GetType().Name;
        ID = Guid.NewGuid();
    }

    public string Kind { get; set; }

    public Guid ID { get; set; }

    public bool Popup { get; set; }

    [JsonIgnore]
    public string? Label
    {
        get => GetProperty<string>("label");
        set => Properties["label"] = value;
    }

    [JsonIgnore]
    public string? Title
    {
        get => GetProperty<string>("title");
        set => Properties["title"] = value;
    }

    public string? Content { get; set; }

    public double TextScale { get; set; } = 1;

    public string Color { get; set; } = "#FFFFFF";

    public string BorderColor { get; set; } = "#FFFFFF";

    public int BorderSize { get; set; } = 1;

    public string BackgroundColor { get; set; } = "#000000";

    public double Radius { get; set; }

    public double Scale { get; set; } = 1;

    private T? GetProperty<T>(string key)
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