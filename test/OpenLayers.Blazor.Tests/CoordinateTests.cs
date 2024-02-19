using System.Formats.Asn1;
using System.Globalization;
using System.Text.Json;
using Microsoft.VisualBasic.CompilerServices;
using Xunit.Abstractions;

namespace OpenLayers.Blazor.Tests;

public class CoordinateTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CoordinateTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Coordinate_Constructor_PropsSet()
    {
        var c = new Coordinate(10, 20);

        Assert.Equal(10, c.X);
        Assert.Equal(20, c.Y);

        Assert.Equal(10, c.Longitude);
        Assert.Equal(20, c.Latitude);
    }

    [Fact]
    public void Coordinate_ParseSlash_Success()
    {
        var c = Coordinate.Parse("1.1/2.2");

        Assert.Equal(1.1, c.X);
        Assert.Equal(2.2, c.Y);
    }

    [Fact]
    public void Coordinate_ParseInvariant_Success()
    {
        var c = Coordinate.Parse("1.1:2.2", CultureInfo.InvariantCulture);

        Assert.Equal(1.1, c.X);
        Assert.Equal(2.2, c.Y);
    }

    [Fact]
    public void Coordinate_ParseAltCulture_Success()
    {
        var c = Coordinate.Parse("1,1/2,2", new CultureInfo("it-IT"));

        Assert.Equal(1.1, c.X);
        Assert.Equal(2.2, c.Y);
    }

    [Fact]
    public void Coordinate_JsonDeserialize_AsString()
    {
        string json = "\"1.1/2.2\"";
        var c = JsonSerializer.Deserialize<Coordinate>(json);
        Assert.Equal(1.1, c.X);
        Assert.Equal(2.2, c.Y);
    }

    [Fact]
    public void Coordinate_JsonDeserialize_AsXY()
    {
        string json = "{\"x\":1.1,\"y\":2.2}";
        var c = JsonSerializer.Deserialize<Coordinate>(json);
        Assert.Equal(1.1, c.X);
        Assert.Equal(2.2, c.Y);
    }

    [Fact]
    public void Coordinate_JsonDeserialize_AsArray()
    {
        string json = "[1.1,2.2]";
        var c = JsonSerializer.Deserialize<Coordinate>(json);
        Assert.Equal(1.1, c.X);
        Assert.Equal(2.2, c.Y);
    }
}