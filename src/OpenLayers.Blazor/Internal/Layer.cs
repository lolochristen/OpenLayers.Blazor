namespace OpenLayers.Blazor.Internal;

public class Layer
{
    public string? ClassName { get; set; }
    public double Opacity { get; set; } = 1;
    public bool Visibility { get; set; } = true;
    public double[]? Extent { get; set; }
    public int? ZIndex { get; set; }
    public double? MinResolution { get; set; }
    public double? MaxResolution { get; set; }
    public double? MinZoom { get; set; }
    public double? MaxZoom { get; set; }
    public double Preload { get; set; } = 0;
    public Source Source { get; set; } = new();
    public bool UseInterimTilesOnError { get; set; } = true;

    public Dictionary<string, object>? Properties { get; set; } = new();
}