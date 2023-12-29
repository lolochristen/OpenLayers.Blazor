using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor.Internal;

public class Source
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SourceType SourceType { get; set; }

    public string? Attributions { get; set; }
    public bool AttributionsCollapsible { get; set; } = true;
    public int? CacheSize { get; set; }
    public string? CrossOrigin { get; set; }
    public bool Interpolate { get; set; } = true;
    public Dictionary<string, object> Params { get; set; } = new();
    public double Gutter { get; set; } = 0;
    public bool Hidpi { get; set; } = true;
    public dynamic? Projection { get; set; }
    public double? ReprojectionErrorThreshold { get; set; }
    public string? ServerType { get; set; }
    public string? Url { get; set; }
    public string[]? Urls { get; set; }
    public bool WrapX { get; set; } = true;
    public bool Transition { get; set; }        
    public double ZDirection { get; set; } = 0;
    public string? Key { get; set; }
    public string? ImagerySet { get; set; }
    public string? Layer { get; set; }
    public string? MatrixSet { get; set; }
    public string? Format { get; set; } 
    public dynamic? FormatOptions { get; set; }
}