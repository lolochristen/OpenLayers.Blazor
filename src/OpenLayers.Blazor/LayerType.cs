namespace OpenLayers.Blazor;

public enum LayerType
{
    Tile,
    Image,
    Vector,
    VectorTile,
    Heatmap,
    Graticule,
    VectorImage,
    WebGLTile,
    /// <summary>
    /// MapboxVectorLayer. Requires ol-mapbox-style js
    /// </summary>
    MapboxVectorStyle
}