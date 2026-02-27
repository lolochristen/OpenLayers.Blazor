using System.Text.Json.Serialization;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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