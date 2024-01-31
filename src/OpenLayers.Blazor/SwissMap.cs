using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

public class SwissMap : Map
{
    public SwissMap()
    {
        LayerId = "ch.swisstopo.pixelkarte-farbe";
        SetBaseLayer(LayerId);
        Center = new Coordinate { X = 2660013.54, Y = 1185171.98 }; // Swiss Center
        Options.CoordinatesProjection = "EPSG:2056";
        Options.ViewProjection = "EPSG:2056"; 
        Zoom = 2.4;
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
#if DEBUG
        Console.WriteLine($"SetBaseLayer {layerId}");
#endif
        var layer = new Layer
        {
            Extent = new double[] { 2485071.58, 1074261.72, 2837119.8, 1299941.79 },
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