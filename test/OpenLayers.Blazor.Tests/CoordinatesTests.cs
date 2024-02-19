using System.Text.Json;
using Xunit.Abstractions;

namespace OpenLayers.Blazor.Tests;

public class CoordinatesTests(ITestOutputHelper testOutputHelper)
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

    [Fact]
    public void Coordinates_FeatureSerializeDeserialize_List()
    {
        var feature = new Internal.Feature();
        feature.Coordinates = new Coordinates(new List<Coordinate>() { new[] { 1.1, 2.2 }, new[]{ 3.3, 4.4 } });

        var doc = JsonSerializer.SerializeToDocument(feature);
        _testOutputHelper.WriteLine(doc.RootElement.ToString());

        var feature2 = doc.Deserialize<Internal.Feature>();

        Assert.Equal(feature.Coordinates.Default, feature2.Coordinates.Default);
    }

    [Fact]
    public void Coordinates_FeatureSerializeDeserialize_MultiList()
    {
        var feature = new Internal.Feature();
        feature.Coordinates = new double[][][]
        {
            new double[][]
            {
                new double[] { 1.1, 2.2 },
                new double[] { 3.3, 4.4 }
            },
            new double[][]
            {
                new double[] { 5.5, 6.6 },
                new double[] { 7.7, 8.8 }
            }
        };

        var doc = JsonSerializer.SerializeToDocument(feature);

        var feature2 = doc.Deserialize<Internal.Feature>();

        Assert.Equal(feature.Coordinates[0], feature2.Coordinates[0]);
        Assert.Equal(feature.Coordinates[1], feature2.Coordinates[1]);
    }

    [Fact]
    public void Coordinates_FeatureSerializeDeserialize_Point()
    {
        var feature = new Internal.Feature();
        feature.Coordinates.Point = new Coordinate(1.1, 2.2);

        var doc = JsonSerializer.SerializeToDocument(feature);
        _testOutputHelper.WriteLine(doc.RootElement.ToString());
        var feature2 = doc.Deserialize<Internal.Feature>();

        Assert.Equal(feature.Coordinates.Default, feature2.Coordinates.Default);
        Assert.Equal(new double[] { 1.1, 2.2 }, feature.Coordinates.Point);
    }
}