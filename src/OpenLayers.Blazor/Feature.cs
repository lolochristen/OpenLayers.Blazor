namespace OpenLayers.Blazor;

public class Feature
{
    // { Type = this.GetType().Name; }

    public string Type { get; set; } = "Feature";

    public Geometry Geometry { get; set; }

    public Dictionary<string, dynamic> Properties { get; set; } = new();
}