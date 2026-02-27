using Bunit;
using Xunit;

namespace OpenLayers.Blazor.Tests;

public class LayerTests
{
    [Fact]
    public void Layer_Properties_SetCorrectly()
    {
        using var ctx = new TestContext();

        var layer = new Layer();
        layer.Id = "test-layer";
        layer.Opacity = 0.7;
        layer.Visibility = true;
        layer.ZIndex = 5;

        Assert.Equal("test-layer", layer.Id);
        Assert.Equal(0.7, layer.Opacity);
        Assert.True(layer.Visibility);
        Assert.Equal(5, layer.ZIndex);
    }

    [Fact]
    public void Layer_SourceProperties_SetCorrectly()
    {
        var layer = new Layer();
        
        layer.SourceType = SourceType.WMTS;
        layer.Url = "https://example.com/wmts";
        layer.CrossOrigin = "anonymous";

        Assert.Equal(SourceType.WMTS, layer.SourceType);
        Assert.Equal("https://example.com/wmts", layer.Url);
        Assert.Equal("anonymous", layer.CrossOrigin);
    }

    [Fact]
    public void Layer_ZoomProperties_SetCorrectly()
    {
        var layer = new Layer();

        layer.MinZoom = 5;
        layer.MaxZoom = 18;
        layer.MinResolution = 10.5;
        layer.MaxResolution = 100.5;

        Assert.Equal(5, layer.MinZoom);
        Assert.Equal(18, layer.MaxZoom);
        Assert.Equal(10.5, layer.MinResolution);
        Assert.Equal(100.5, layer.MaxResolution);
    }

    [Fact]
    public void Layer_Extent_SetCorrectly()
    {
        var layer = new Layer();
        var extent = new Extent(1, 2, 3, 4);

        layer.Extent = extent;

        Assert.Equal(extent, layer.Extent);
    }

    [Fact]
    public void Layer_BingMapProperties_SetCorrectly()
    {
        var layer = new Layer();

        layer.SourceType = SourceType.BingMaps;
        layer.ImagerySet = BingMapImagerySet.Aerial;
        layer.Key = "test-api-key";

        Assert.Equal(SourceType.BingMaps, layer.SourceType);
        Assert.Equal(BingMapImagerySet.Aerial, layer.ImagerySet);
        Assert.Equal("test-api-key", layer.Key);
    }

    [Fact]
    public void Layer_WMSProperties_SetCorrectly()
    {
        var layer = new Layer();

        layer.SourceType = SourceType.TileWMS;
        layer.Layers = "test-layer";
        layer.Styles = "default";
        layer.Format = "image/png";

        Assert.Equal(SourceType.TileWMS, layer.SourceType);
        Assert.Equal("test-layer", layer.Layers);
        Assert.Equal("default", layer.Styles);
        Assert.Equal("image/png", layer.Format);
    }

    [Fact]
    public void Layer_VectorProperties_SetCorrectly()
    {
        var layer = new Layer();

        layer.SourceType = SourceType.VectorGeoJson;
        layer.LayerType = LayerType.Vector;
        layer.Declutter = true;

        Assert.Equal(SourceType.VectorGeoJson, layer.SourceType);
        Assert.Equal(LayerType.Vector, layer.LayerType);
        Assert.True(layer.Declutter);
    }

    [Fact]
    public void Layer_SelectionProperties_SetCorrectly()
    {
        var layer = new Layer();
        var selectionStyle = new StyleOptions { ZIndex = 100 };

        layer.SelectionEnabled = true;
        layer.MultiSelect = true;
        layer.SelectionStyle = selectionStyle;

        Assert.True(layer.SelectionEnabled);
        Assert.True(layer.MultiSelect);
        Assert.Equal(selectionStyle, layer.SelectionStyle);
    }

    [Fact]
    public void Layer_ShapesList_IsInitialized()
    {
        var layer = new Layer();

        Assert.NotNull(layer.ShapesList);
        Assert.Empty(layer.ShapesList);
    }

    [Fact]
    public void Layer_ClusterDistance_SetCorrectly()
    {
        var layer = new Layer();

        layer.ClusterDistance = 50;

        Assert.Equal(50, layer.ClusterDistance);
    }

    [Fact]
    public void Layer_Preload_SetCorrectly()
    {
        var layer = new Layer();

        layer.Preload = 2;

        Assert.Equal(2, layer.Preload);
    }

    [Fact]
    public void Layer_Projection_SetCorrectly()
    {
        var layer = new Layer();

        layer.Projection = "EPSG:4326";

        Assert.Equal("EPSG:4326", layer.Projection);
    }
}
