namespace OpenLayers.Blazor;

/// <summary>
/// Defines the type of extent calculation for map bounds.
/// </summary>
public enum ExtentType
{
    /// <summary>
    /// Calculate extent based on marker positions.
    /// </summary>
    Markers,
    
    /// <summary>
    /// Calculate extent based on geometry shapes.
    /// </summary>
    Geometries
}