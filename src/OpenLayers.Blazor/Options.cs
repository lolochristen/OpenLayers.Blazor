using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

/// <summary>
/// Provides configuration options for the map component.
/// </summary>
public class Options
{
    /// <summary>
    ///     Default fill color.
    /// </summary>
    public static string DefaultFill { get; set; } = "#2d7cc7cc";

    /// <summary>
    ///     Default stroke color
    /// </summary>
    public static string DefaultStroke { get; set; } = "#ffffff";

    /// <summary>
    ///     Gets or sets whenever the popup shall be shown when clicking on a feature.
    /// </summary>
    public bool AutoPopup { get; set; }
 
    /// <summary>
    ///     Projection code how the coordinates are represented. Default is EPSG:4326.
    /// </summary>
    public string CoordinatesProjection
    {
        get => _coordinatesProjection;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("CoordinatesProjection cannot be null or empty", nameof(value));
            _coordinatesProjection = value;
        }
    }
    private string _coordinatesProjection = "EPSG:4326";

    /// <summary>
    ///     Code of the view of the map shall be projected. If not given, the default view of the first layer is used.
    /// </summary>
    public string? ViewProjection { get; set; }

    /// <summary>
    ///     Scale unit of the map. Default is <see cref="ScaleLineUnit.Metric" />.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ScaleLineUnit ScaleLineUnit { get; set; } = ScaleLineUnit.Metric;

    /// <summary>
    ///     Sets or gets the limit whenever the list of coordinates of a feature shall be included in events to improve
    ///     response times.
    /// </summary>
    public int SerializationCoordinatesLimit
    {
        get => _serializationCoordinatesLimit;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "SerializationCoordinatesLimit must be non-negative");
            _serializationCoordinatesLimit = value;
        }
    }
    private int _serializationCoordinatesLimit = 512;

    /// <summary>
    ///     Get or sets if the zoom control is visible
    /// </summary>
    public bool ZoomControl { get; set; } = true;

    /// <summary>
    ///     Gets or sets if the attribution control is visible
    /// </summary>
    public bool AttributionControl { get; set; } = true;

    /// <summary>
    ///     Gets or sets if full screen control is visible
    /// </summary>
    public bool FullScreenControl { get; set; }

    /// <summary>
    ///     Gets or sets boolean if zoom slider control is visible
    /// </summary>
    public bool ZoomSliderControl { get; set; }

    /// <summary>
    ///     Gets or sets boolean if rotate to 0 control is visible
    /// </summary>
    public bool RotateControl { get; set; }

    /// <summary>
    ///     Gets or sets boolean if a overview map using first layer is visible
    /// </summary>
    public bool OverviewMap { get; set; }

    /// <summary>
    ///     Gets or sets boolean if zoom to extent control is visible
    /// </summary>
    public bool ZoomToExtentControl { get; set; }

    /// <summary>
    ///    Gets or sets the minimal zoom level.
    /// </summary>
    public double MinZoom
    {
        get => _minZoom;
        set
        {
            if (value < 0 || value > MaxZoom)
                throw new ArgumentOutOfRangeException(nameof(value), "MinZoom must be between 0 and MaxZoom");
            _minZoom = value;
        }
    }
    private double _minZoom = 0;

    /// <summary>
    ///    Gets or sets the maximal zoom level.
    /// </summary>
    public double MaxZoom
    {
        get => _maxZoom;
        set
        {
            if (value < MinZoom || value > 50)
                throw new ArgumentOutOfRangeException(nameof(value), "MaxZoom must be between MinZoom and 50");
            _maxZoom = value;
        }
    }
    private double _maxZoom = 28;

    /// <summary>
    ///    Gets or sets whenever popups shall allow events like click forward to the map. Defaults to false.
    /// </summary>
    public bool PopupAllowMapEvents { get; set; } = false;

    /// <summary>
    ///   Gets or set the Offset of the popup. Default is [0, -50] (to appear above the feature location).
    /// </summary>
    public double[] PopupOffset { get; set; } = new double[] { 0, -50 };
}