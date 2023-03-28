namespace OpenLayers.Blazor;

public class OpenStreetMap : Map
{
    public OpenStreetMap()
    {
        Layers.Add(new TileLayer()
        {
            Source =
                new TileSource()
                {
                    SourceType = SourceType.OSM,
                }
        });
        Center = new Coordinate(0,0);
        Zoom = 10;
    }
}