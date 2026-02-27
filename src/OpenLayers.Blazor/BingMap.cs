using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

/// <summary>
/// Defines the imagery set options for Bing Maps.
/// </summary>
public enum BingMapImagerySet
{
    /// <summary>
    /// Road map imagery.
    /// </summary>
    RoadOnDemand,
    
    /// <summary>
    /// Aerial satellite imagery.
    /// </summary>
    Aerial,
    
    /// <summary>
    /// Aerial imagery with road labels overlay.
    /// </summary>
    AerialWithLabelsOnDemand,
    
    /// <summary>
    /// Dark themed canvas map.
    /// </summary>
    CanvasDark,
    
    /// <summary>
    /// Ordnance Survey map (UK).
    /// </summary>
    OrdnanceSurvey
}

/// <summary>
/// Map component configured for Bing Maps tile layers.
/// </summary>
public class BingMap : Map
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BingMap"/> class with default settings.
    /// </summary>
    public BingMap()
    {
        ImagerySet = BingMapImagerySet.RoadOnDemand;
        Center = new Coordinate(46.783290, 9.679330);
        Zoom = 6;
    }

    /// <summary>
    /// Gets or sets the Bing Maps API key. Obtain from https://www.bingmapsportal.com/.
    /// </summary>
    [Parameter]
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the imagery set type for Bing Maps tiles.
    /// </summary>
    [Parameter] 
    public BingMapImagerySet ImagerySet { get; set; }

    /// <inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        if (LayersList.Count > 0 && LayersList[0].ImagerySet != ImagerySet)
        {
            LayersList[0].ImagerySet = ImagerySet;
            await SetLayers(LayersList);
        }
    }

    /// <inheritdoc/>
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