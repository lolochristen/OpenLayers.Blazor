namespace OpenLayers.Blazor;

/// <summary>
/// Defines the structure type of coordinate data.
/// </summary>
public enum CoordinatesType
{
    /// <summary>
    /// Single point coordinate [x, y].
    /// </summary>
    Point = 0,
    
    /// <summary>
    /// Array of coordinates [[x1, y1], [x2, y2], ...].
    /// </summary>
    List = 1,
    
    /// <summary>
    /// Multi-dimensional array of coordinates [[[x1, y1], ...], ...].
    /// </summary>
    MultiList = 2
}