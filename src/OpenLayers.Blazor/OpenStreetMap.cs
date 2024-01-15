namespace OpenLayers.Blazor;

public class OpenStreetMap : Map
{
    public OpenStreetMap()
    {
        LayersList.Add(new Layer
        {
            SourceType = SourceType.OSM
        });
        Center = new Coordinate(0, 0);
    }
}