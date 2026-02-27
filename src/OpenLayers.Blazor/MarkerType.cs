namespace OpenLayers.Blazor;

/// <summary>
/// Defines the visual style of a marker on the map.
/// </summary>
public enum MarkerType
{
    /// <summary>
    /// Standard pin marker icon.
    /// </summary>
    MarkerPin,
    
    /// <summary>
    /// Flag-style marker icon.
    /// </summary>
    MarkerFlag,
    
    /// <summary>
    /// Font Awesome icon marker.
    /// </summary>
    MarkerAwesome,
    
    /// <summary>
    /// Custom image marker.
    /// </summary>
    MarkerCustomImage
}