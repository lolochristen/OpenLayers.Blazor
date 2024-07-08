namespace OpenLayers.Blazor;

public class SelectionChangedArgs
{
    public IEnumerable<Shape> SelectedShapes { get; set; }
    public IEnumerable<Shape> UnselectedShapes { get; set; }
}