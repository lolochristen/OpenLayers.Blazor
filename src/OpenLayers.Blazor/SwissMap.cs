using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

public class SwissMap : Map
{
    public SwissMap()
    {
        LayerId = "ch.swisstopo.pixelkarte-farbe";
        SetBaseLayer(LayerId);
        Center = new Coordinate { X = 2660013.54, Y = 1185171.98 }; // Swiss Center
        Zoom = 2.4;
        Defaults.CoordinatesProjection = "EPSG:2056"; // VT95
    }


    [Parameter] public string LayerId { get; set; }

    protected override Task OnParametersSetAsync()
    {
        if (LayersList.Count > 0 && LayersList[0].SourceParameters["LAYERS"] != LayerId)
            SetBaseLayer(LayerId);
        return base.OnParametersSetAsync();
    }

    protected void SetBaseLayer(string layerId)
    {
        Console.WriteLine($"SetBaseLayer {layerId}");
        var layer = new Layer
        {
            Extent = new double[] { 2420000, 1030000, 2900000, 1350000 },
            SourceType = SourceType.TileWMS,
            Url = "https://wms.geo.admin.ch/",
            CrossOrigin = "anonymous",
            SourceParameters = new Dictionary<string, object> { { "LAYERS", layerId }, { "FORMAT", "image/jpeg" } },
            ServerType = "mapserver",
            Attributions = "© <a href=\"https://www.swisstopo.admin.ch/en/home.html\" target=\"_blank\">swisstopo</a>"
        };

        if (LayersList.Count == 0)
            LayersList.Add(layer);
        else
            LayersList[0] = layer;
    }

    /// <summary>
    ///     e.g. ch.astra.wanderland-sperrungen_umleitungen
    ///     https://wms.geo.admin.ch/?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetCapabilities
    /// </summary>
    /// <param name="layerId"></param>
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