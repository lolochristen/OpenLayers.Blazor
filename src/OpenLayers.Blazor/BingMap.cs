namespace OpenLayers.Blazor;

public enum BingMapImagerySet
{
    RoadOnDemand,
    Aerial,
    AerialWithLabelsOnDemand,
    CanvasDark,
    OrdnanceSurvey
}

public class BingMap : Map
{
    /// <summary>
    ///  Your Bing Maps Key from https://www.bingmapsportal.com/ 
    /// </summary>
    public string? Key { get; set; }

    public BingMapImagerySet ImagerySet { get; set; }

    public BingMap()
    {
        Center = new Coordinate(-6655.5402445057125, 6709968.258934638);
        Zoom = 13;
    }

    protected override void OnInitialized()
    {
        Layers.Add(new TileLayer()
        {
            Source =
                new TileSource()
                {
                    SourceType = SourceType.BingMaps,
                    Key = Key,
                    ImagerySet = ImagerySet.ToString()
                },
            MaxZoom = 19
        });
        base.OnInitialized();
    }
}