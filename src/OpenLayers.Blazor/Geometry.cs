namespace OpenLayers.Blazor;

public class Geometry
{
    public Geometry()
    {
    }

    public Geometry(string type)
    {
        Type = type;
    }

    public string Type { get; set; }

    public dynamic Coordinates { get; set; }
}