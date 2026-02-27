using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

/// <summary>
/// Map component configured for Swiss Federal Office of Topography (swisstopo) WMS layers.
/// Uses EPSG:2056 (LV95) coordinate system by default.
/// </summary>
public class SwissMap : Map
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SwissMap"/> class with swisstopo base layer.
    /// </summary>
    public SwissMap()
    {
        LayerId = "ch.swisstopo.pixelkarte-farbe";
        SetBaseLayer(LayerId);
        Center = new Coordinate { X = 2660013.54, Y = 1185171.98 }; // Swiss Center
        Options.CoordinatesProjection = "EPSG:2056";
        Options.ViewProjection = "EPSG:2056";
        Zoom = 2.4;
    }

    /// <summary>
    /// Gets or sets the swisstopo layer identifier (e.g., "ch.swisstopo.pixelkarte-farbe").
    /// </summary>
    [Parameter] 
    public string LayerId { get; set; }

    /// <inheritdoc/>
    protected override Task OnParametersSetAsync()
    {
        if (LayersList.Count > 0 && LayersList[0].SourceParameters["LAYERS"] != LayerId)
            SetBaseLayer(LayerId);
        return base.OnParametersSetAsync();
    }

    /// <summary>
    /// Configures the base layer with the specified swisstopo layer identifier.
    /// </summary>
    /// <param name="layerId">The swisstopo WMS layer identifier.</param>
    protected void SetBaseLayer(string layerId)
    {
#if DEBUG
        Console.WriteLine($"SetBaseLayer {layerId}");
#endif
        var layer = new Layer
        {
            Extent = new[] { 2485071.58, 1074261.72, 2837119.8, 1299941.79 },
            SourceType = SourceType.TileWMS,
            Url = "https://wms.geo.admin.ch/",
            CrossOrigin = "anonymous",
            Layers = layerId,
            Format = "image/jpeg",
            ServerType = "mapserver",
            Attributions = "© <a href=\"https://www.swisstopo.admin.ch/en/home.html\" target=\"_blank\">swisstopo</a>"
        };

        if (LayersList.Count == 0)
            LayersList.Add(layer);
        else
            LayersList[0] = layer;
    }

    /// <summary>
    /// Adds a Swiss geo.admin.ch WMS overlay layer to the map.
    /// </summary>
    /// <param name="layerId">The WMS layer identifier (e.g., "ch.astra.wanderland-sperrungen_umleitungen"). 
    /// See https://wms.geo.admin.ch/?SERVICE=WMS&amp;VERSION=1.3.0&amp;REQUEST=GetCapabilities for available layers.</param>
    /// <param name="opacity">The layer opacity (0.0 to 1.0). Default is 1.0.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task AddSwissGeoLayer(string layerId, double opacity = 1)
    {
        LayersList.Add(new Layer
        {
            Opacity = opacity,
            SourceType = SourceType.TileWMS,
            Url = $"https://wms.geo.admin.ch/?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&FORMAT=image%2Fpng&TRANSPARENT=true&LAYERS={layerId}&LANG=en"
        });
        return Task.CompletedTask;
    }
}