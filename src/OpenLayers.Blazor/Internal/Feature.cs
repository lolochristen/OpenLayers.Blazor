namespace OpenLayers.Blazor.Internal;

public class Feature
{
    // { Type = this.GetType().Name; }

    public string Type { get; set; } = "Feature";

    public Geometry Geometry { get; set; }

    public Dictionary<string, dynamic> Properties { get; set; } = new();

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Geometry, Properties);
    }
}