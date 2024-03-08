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

    /// <summary>
    /// Get or sets if the zoom control is visible
    /// </summary>
    public bool ZoomControl { get; set; } = true;

    /// <summary>
    /// Gets or sets if the attribution control is visible
    /// </summary>
    public bool AttributionControl { get; set; } = true;

    /// <summary>
    /// Gets or sets if full screen control is visible 
    /// </summary>
    public bool FullScreenControl { get; set; }

    /// <summary>
    /// Gets or sets boolean if zoom slider control is visible
    /// </summary>
    public bool ZoomSliderControl { get; set; }

    /// <summary>
    /// Gets or sets boolean if rotate to 0 control is visible
    /// </summary>
    public bool RotateControl { get; set; }

    /// <summary>
    /// Gets or sets boolean if a overview map using first layer is visible
    /// </summary>
    public bool OverviewMap { get; set; }

    /// <summary>
    /// Gets or sets boolean if zoom to extent control is visible
    /// </summary>
    public bool ZoomToExtentControl { get; set; }
}