using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

public class Options
{
    /// <summary>
    /// Gets or sets whenever the popup shall be shown when clicking on a feature.
    /// </summary>
    public bool AutoPopup { get; set; }

    /// <summary>
    /// Default label for markers.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Default color for markers.
    /// </summary>
    public string Color { get; set; } = "#FFFFFF";

    /// <summary>
    /// Default background color for markers.
    /// </summary>
    public string BackgroundColor { get; set; } = "#0000FF";

    /// <summary>
    /// Default border color for markers.
    /// </summary>
    public string BorderColor { get; set; } = "#FFFFFF";

    /// <summary>
    /// Projection code how the coordinates are represented. Default is EPSG:4326.
    /// </summary>
    public string CoordinatesProjection { get; set; } = "EPSG:4326";

    /// <summary>
    /// Code of the view of the map shall be projected. If not given, the default view of the first layer is used.
    /// </summary>
    public string? ViewProjection { get; set; }

    /// <summary>
    /// Scale unit of the map. Default is <see cref="ScaleLineUnit.Metric"/>.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ScaleLineUnit ScaleLineUnit { get; set; } = ScaleLineUnit.Metric;

    /// <summary>
    /// Sets or gets the limit whenever the list of coordinates of a feature shall be included in events to improve response times.  
    /// </summary>
    public int SerializationCoordinatesLimit { get; set; } = 512;
}