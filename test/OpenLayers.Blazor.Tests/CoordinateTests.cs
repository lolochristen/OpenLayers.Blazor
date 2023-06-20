namespace OpenLayers.Blazor.Tests;

public class CoordinateTests
{
    [Fact]
    public void Coordinate_Constructor_PropsSet()
    {
        var c = new Coordinate(10, 20);

        Assert.Equal(10, c.Y);
        Assert.Equal(20, c.X);

        Assert.Equal(10, c.Latitude);
        Assert.Equal(20, c.Longitude);
    }
}
