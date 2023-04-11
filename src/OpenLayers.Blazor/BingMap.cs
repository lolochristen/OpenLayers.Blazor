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
    public BingMap()
    {
        Center = new Coordinate(-6655.5402445057125, 6709968.258934638);
        Zoom = 13;
    }

    /// <summary>
    ///     Your Bing Maps Key from https://www.bingmapsportal.com/
    /// </summary>
    public string? Key { get; set; }

    public BingMapImagerySet ImagerySet { get; set; }

    protected override void OnInitialized()
    {
        LayersList.Add(new Layer
        {
            SourceType = SourceType.BingMaps,
            Key = Key,
            ImagerySet = ImagerySet,
            MaxZoom = 19
        });
        base.OnInitialized();
    }
}