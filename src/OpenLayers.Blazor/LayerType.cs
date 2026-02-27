namespace OpenLayers.Blazor;

/// <summary>
/// Defines the type of map layer to be rendered.
/// </summary>
public enum LayerType
{
    /// <summary>
    /// Tile layer for rendering tiled images.
    /// </summary>
    Tile,
    
    /// <summary>
    /// Image layer for rendering static images.
    /// </summary>
    Image,
    
    /// <summary>
    /// Vector layer for rendering vector features.
    /// </summary>
    Vector,
    
    /// <summary>
    /// Vector tile layer for rendering vector tiles.
    /// </summary>
    VectorTile,
    
    /// <summary>
    /// Heatmap layer for rendering density visualizations.
    /// </summary>
    Heatmap,
    
    /// <summary>
    /// Graticule layer for rendering coordinate grid lines.
    /// </summary>
    Graticule,
    
    /// <summary>
    /// Vector image layer for rendering vectors as images.
    /// </summary>
    VectorImage,
    
    /// <summary>
    /// WebGL tile layer for hardware-accelerated rendering.
    /// </summary>
    WebGLTile,
    
    /// <summary>
    /// Mapbox vector style layer. Requires ol-mapbox-style JavaScript library.
    /// </summary>
    MapboxVectorStyle,
    
    /// <summary>
    /// Vector cluster layer for grouping nearby features.
    /// </summary>
    VectorCluster
}