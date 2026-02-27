namespace OpenLayers.Blazor;

/// <summary>
///     Represents the arguments for a selection changed event.
/// </summary>
public class SelectionChangedArgs
{
    /// <summary>
    ///     Gets or sets the collection of shapes that were selected.
    /// </summary>
    public IEnumerable<Shape> SelectedShapes { get; set; } = Enumerable.Empty<Shape>();

    /// <summary>
    ///     Gets or sets the collection of shapes that were unselected.
    /// </summary>
    public IEnumerable<Shape> UnselectedShapes { get; set; } = Enumerable.Empty<Shape>();
}