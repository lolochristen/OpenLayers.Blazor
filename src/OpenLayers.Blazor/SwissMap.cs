namespace OpenLayers.Blazor;

public class SwissMap : Map
{
    public SwissMap()
    {
        LayersList.Add(new Layer
        {
            Extent = new double[] { 2420000, 1030000, 2900000, 1350000 },
            SourceType = SourceType.TileWMS,
            Url = "https://wms.geo.admin.ch/",
            CrossOrigin = "anonymous",
            SourceParameters = new Dictionary<string, object> { { "LAYERS", "ch.swisstopo.pixelkarte-farbe" }, { "FORMAT", "image/jpeg" } },
            ServerType = "mapserver",
            Attributions = "© <a href=\"https://www.swisstopo.admin.ch/en/home.html\" target=\"_blank\">swisstopo</a>"
        });
        Center = new Coordinate { X = 2660013.54, Y = 1185171.98 }; // Swiss Center
        Zoom = 2.4;
        Defaults.CoordinatesProjection = "EPSG:2056"; // VT95
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