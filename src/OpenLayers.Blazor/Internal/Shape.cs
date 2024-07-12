using System.Text.Json.Serialization;

namespace OpenLayers.Blazor.Internal;

public class Shape : Feature
{
    public Shape()
    {
        Type = nameof(Shape);
    }

    [JsonIgnore]
    public bool Popup
    {
        get => GetProperty<bool>("popup");
        set => Properties["popup"] = value;
    }

    public Dictionary<string, object>? FlatStyle { get; set; }

    public List<StyleOptions>? Styles { get; set; }

    public double? Radius { get; set; }
}