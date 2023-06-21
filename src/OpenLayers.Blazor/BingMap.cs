using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

/// <summary>
///     Imagery Set for Bing
/// </summary>
public enum BingMapImagerySet
{
    RoadOnDemand,
    Aerial,
    AerialWithLabelsOnDemand,
    CanvasDark,
    OrdnanceSurvey
}

/// <summary>
///     Bing Map
/// </summary>
public class BingMap : Map
{
    public BingMap()
    {
        ImagerySet = BingMapImagerySet.RoadOnDemand;
        Center = new Coordinate(46.783290, 9.679330);
        Zoom = 6;
    }

    /// <summary>
    ///     Your Bing Maps Key from https://www.bingmapsportal.com/
    /// </summary>
    [Parameter]
    public string? Key { get; set; }

    [Parameter] public BingMapImagerySet ImagerySet { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (LayersList.Count > 0 && LayersList[0].ImagerySet != ImagerySet)
        {
            LayersList[0].ImagerySet = ImagerySet;
            await SetLayers(LayersList);
        }
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            LayersList.Add(new Layer
            {
                SourceType = SourceType.BingMaps,
                Key = Key,
                ImagerySet = ImagerySet,
                MaxZoom = 19
            });

        return base.OnAfterRenderAsync(firstRender);
    }
}