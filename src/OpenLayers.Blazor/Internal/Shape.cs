using System.Text.Json.Serialization;

namespace OpenLayers.Blazor.Internal;

public class Shape : Feature, IEquatable<Shape>
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

    //[JsonIgnore]
    //public string? Label
    //{
    //    get => GetProperty<string>("label");
    //    set => Properties["label"] = value;
    //}

    //[JsonIgnore]
    //public string? Title
    //{
    //    get => GetProperty<string>("title");
    //    set => Properties["title"] = value;
    //}

    public Dictionary<string, object>? FlatStyle { get; set; }

    public List<StyleOptions>? Styles { get; set; }

    // public double? TextScale { get; set; } = 1;

    //public string? Color { get; set; } = "#FFFFFF";

    //public string? Stroke { get; set; } = "#FFFFFF";

    //public int? BorderSize { get; set; } = 1;

    //public string? BackgroundColor { get; set; } = "#000000";

    public double? Radius { get; set; }

    //public double? Scale { get; set; } = 1;

    //public int? ZIndex { get; set; }

    public bool Equals(Shape? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.GetHashCode() == other.GetHashCode();
    }

    //public override int GetHashCode()
    //{
    //    return HashCode.Combine(base.GetHashCode(), HashCode.Combine(TextScale, Radius, Scale), HashCode.Combine(ZIndex));
    //}

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Shape)obj);
    }
}