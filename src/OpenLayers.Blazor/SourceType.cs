namespace OpenLayers.Blazor;

/// <summary>
/// Defines the type of data source for map layers.
/// </summary>
public enum SourceType
{
    /// <summary>
    /// Tile image source.
    /// </summary>
    TileImage,
    
    /// <summary>
    /// Bing Maps tile source.
    /// </summary>
    BingMaps,
    
    /// <summary>
    /// OGC Map Tile source.
    /// </summary>
    OGCMapTile,
    
    /// <summary>
    /// ArcGIS REST tile service source.
    /// </summary>
    TileArcGISRest,
    
    /// <summary>
    /// TileJSON specification source.
    /// </summary>
    TileJSON,
    
    /// <summary>
    /// Web Map Service (WMS) tile source.
    /// </summary>
    TileWMS,
    
    /// <summary>
    /// Web Map Tile Service (WMTS) source.
    /// </summary>
    WMTS,
    
    /// <summary>
    /// Zoomify image tile source.
    /// </summary>
    Zoomify,
    
    /// <summary>
    /// OpenStreetMap tile source.
    /// </summary>
    OSM,
    
    /// <summary>
    /// XYZ tile source with URL template.
    /// </summary>
    XYZ,
    
    /// <summary>
    /// CartoDB tile source.
    /// </summary>
    CartoDB,
    
    /// <summary>
    /// Stamen tile source.
    /// </summary>
    Stamen,
    
    /// <summary>
    /// Debug tile source showing tile coordinates.
    /// </summary>
    TileDebug,
    
    /// <summary>
    /// Stadia Maps tile source.
    /// </summary>
    StadiaMaps,
    
    /// <summary>
    /// Generic vector source.
    /// </summary>
    Vector,
    
    /// <summary>
    /// KML (Keyhole Markup Language) vector source.
    /// </summary>
    VectorKML,
    
    /// <summary>
    /// Esri JSON vector source.
    /// </summary>
    VectorEsriJson,
    
    /// <summary>
    /// GeoJSON vector source.
    /// </summary>
    VectorGeoJson,
    
    /// <summary>
    /// TopoJSON vector source.
    /// </summary>
    VectorTopoJson,
    
    /// <summary>
    /// Mapbox Vector Tile (MVT) source.
    /// </summary>
    VectorMVT,
    
    /// <summary>
    /// IGC flight recorder format vector source.
    /// </summary>
    VectorIGC,
    
    /// <summary>
    /// Encoded polyline vector source.
    /// </summary>
    VectorPolyline,
    
    /// <summary>
    /// Well-Known Text (WKT) vector source.
    /// </summary>
    VectorWKT,
    
    /// <summary>
    /// Well-Known Binary (WKB) vector source.
    /// </summary>
    VectorWKB,
    
    /// <summary>
    /// Geography Markup Language 2 (GML2) vector source.
    /// </summary>
    VectorGML2,
    
    /// <summary>
    /// Geography Markup Language 3 (GML3) vector source.
    /// </summary>
    VectorGML3,
    
    /// <summary>
    /// GPS Exchange Format (GPX) vector source.
    /// </summary>
    VectorGPX,
    
    /// <summary>
    /// OpenStreetMap XML vector source.
    /// </summary>
    VectorOSMXML,
    
    /// <summary>
    /// Web Feature Service (WFS) vector source.
    /// </summary>
    VectorWFS,
    
    /// <summary>
    /// ArcGIS REST image service source.
    /// </summary>
    ImageArcGISRest,
    
    /// <summary>
    /// Canvas-based image source.
    /// </summary>
    ImageCanvasSource,
    
    /// <summary>
    /// MapGuide image source.
    /// </summary>
    ImageMapGuide,
    
    /// <summary>
    /// Static image source.
    /// </summary>
    ImageStatic,
    
    /// <summary>
    /// Web Map Service (WMS) image source.
    /// </summary>
    ImageWMS
}