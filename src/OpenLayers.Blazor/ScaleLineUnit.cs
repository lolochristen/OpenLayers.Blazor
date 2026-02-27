namespace OpenLayers.Blazor;

/// <summary>
/// Defines the unit of measurement for the map scale line control.
/// </summary>
public enum ScaleLineUnit
{
    /// <summary>
    /// Metric units (meters, kilometers).
    /// </summary>
    Metric,
    
    /// <summary>
    /// Angular degrees.
    /// </summary>
    Degrees,
    
    /// <summary>
    /// Imperial units (feet, miles).
    /// </summary>
    Imperial,
    
    /// <summary>
    /// Nautical miles.
    /// </summary>
    Nautical,
    
    /// <summary>
    /// US survey units.
    /// </summary>
    US,
    
    /// <summary>
    /// No scale line displayed.
    /// </summary>
    None
}