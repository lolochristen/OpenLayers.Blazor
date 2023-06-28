using System.Text.Json;
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

    [JsonIgnore]
    public string? Style
    {
        get => GetProperty<string>("style");
        set => Properties["style"] = value;
    }

    [JsonIgnore]
    public string? Content
    {
        get => GetProperty<string>("content");
        set => Properties["content"] = value;
    }

    //[JsonIgnore]
    //public double? Size
    //{
    //    get => GetProperty<double>("size");
    //    set => Properties["size"] = value;
    //}

    //[JsonIgnore]
    //public double[]? Anchor
    //{
    //    get => GetProperty<double[]>("anchor");
    //    set => Properties["anchor"] = value;
    //}

    public double? TextScale { get; set; } = 1;

    public string? Color { get; set; } = "#FFFFFF";

    public string? BorderColor { get; set; } = "#FFFFFF";

    public int? BorderSize { get; set; } = 1;

    public string? BackgroundColor { get; set; } = "#000000";

    public double? Radius { get; set; }

    public double? Scale { get; set; } = 1;

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), TextScale, Color, BorderColor, BorderSize, BackgroundColor, Radius, Scale);
    }

    public bool Equals(Shape? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && Nullable.Equals(TextScale, other.TextScale) && Color == other.Color && BorderColor == other.BorderColor && BorderSize == other.BorderSize && BackgroundColor == other.BackgroundColor && Nullable.Equals(Radius, other.Radius) && Nullable.Equals(Scale, other.Scale);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Shape)obj);
    }
}