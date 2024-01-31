using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

public class Options
{
    public bool AutoPopup { get; set; }

    public string? Label { get; set; }

    public string Color { get; set; } = "#FFFFFF";

    public string BackgroundColor { get; set; } = "#0000FF";

    public string BorderColor { get; set; } = "#FFFFFF";

    public string CoordinatesProjection { get; set; } = "EPSG:4326";

    public string? ViewProjection { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ScaleLineUnit ScaleLineUnit { get; set; } = ScaleLineUnit.Metric;

    public int SerializationCoordinatesLimit { get; set; } = 512;
}