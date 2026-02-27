namespace OpenLayers.Blazor;

/// <summary>
/// Map component configured for OpenStreetMap tile layers.
/// Provides a simple map using OSM as the default base layer.
/// </summary>
public class OpenStreetMap : Map
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenStreetMap"/> class with OSM tile layer.
    /// </summary>
    public OpenStreetMap()
    {
        LayersList.Add(new Layer
        {
            SourceType = SourceType.OSM
        });
        Center = new Coordinate(0, 0);
    }
}