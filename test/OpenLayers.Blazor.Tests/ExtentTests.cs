using Xunit;

namespace OpenLayers.Blazor.Tests;

public class ExtentTests
{
    [Fact]
    public void Extent_Constructor_SetsValues()
    {
        var extent = new Extent(1.0, 2.0, 3.0, 4.0);

        Assert.Equal(1.0, extent.X1);
        Assert.Equal(2.0, extent.Y1);
        Assert.Equal(3.0, extent.X2);
        Assert.Equal(4.0, extent.Y2);
    }

    [Fact]
    public void Extent_ArrayConstructor_SetsValues()
    {
        var array = new[] { 1.0, 2.0, 3.0, 4.0 };
        var extent = new Extent(array);

        Assert.Equal(1.0, extent.X1);
        Assert.Equal(2.0, extent.Y1);
        Assert.Equal(3.0, extent.X2);
        Assert.Equal(4.0, extent.Y2);
    }

    [Fact]
    public void Extent_ArrayConstructor_InvalidLength_ThrowsException()
    {
        var array = new[] { 1.0, 2.0, 3.0 };

        Assert.Throws<ArgumentException>(() => new Extent(array));
    }

    [Fact]
    public void Extent_Indexer_GetsCorrectValues()
    {
        var extent = new Extent(1.0, 2.0, 3.0, 4.0);

        Assert.Equal(1.0, extent[0]);
        Assert.Equal(2.0, extent[1]);
        Assert.Equal(3.0, extent[2]);
        Assert.Equal(4.0, extent[3]);
    }

    [Fact]
    public void Extent_Indexer_SetsCorrectValues()
    {
        var extent = new Extent();

        extent[0] = 1.0;
        extent[1] = 2.0;
        extent[2] = 3.0;
        extent[3] = 4.0;

        Assert.Equal(1.0, extent.X1);
        Assert.Equal(2.0, extent.Y1);
        Assert.Equal(3.0, extent.X2);
        Assert.Equal(4.0, extent.Y2);
    }

    [Fact]
    public void Extent_Indexer_InvalidIndex_ThrowsException()
    {
        var extent = new Extent(1.0, 2.0, 3.0, 4.0);

        Assert.Throws<IndexOutOfRangeException>(() => extent[4]);
        Assert.Throws<IndexOutOfRangeException>(() => extent[-1]);
    }

    [Fact]
    public void Extent_Equals_SameValues_ReturnsTrue()
    {
        var extent1 = new Extent(1.0, 2.0, 3.0, 4.0);
        var extent2 = new Extent(1.0, 2.0, 3.0, 4.0);

        Assert.True(extent1.Equals(extent2));
        Assert.Equal(extent1.GetHashCode(), extent2.GetHashCode());
    }

    [Fact]
    public void Extent_Equals_DifferentValues_ReturnsFalse()
    {
        var extent1 = new Extent(1.0, 2.0, 3.0, 4.0);
        var extent2 = new Extent(1.0, 2.0, 3.0, 5.0);

        Assert.False(extent1.Equals(extent2));
    }

    [Fact]
    public void Extent_ToArray_ReturnsCorrectArray()
    {
        var extent = new Extent(1.0, 2.0, 3.0, 4.0);
        var array = extent.ToArray();

        Assert.Equal(4, array.Length);
        Assert.Equal(1.0, array[0]);
        Assert.Equal(2.0, array[1]);
        Assert.Equal(3.0, array[2]);
        Assert.Equal(4.0, array[3]);
    }

    [Fact]
    public void Extent_ToString_ReturnsCorrectFormat()
    {
        var extent = new Extent(1.0, 2.0, 3.0, 4.0);
        var str = extent.ToString();

        Assert.Equal("1/2:3/4", str);
    }

    [Fact]
    public void Extent_ImplicitConversion_ToArray()
    {
        var extent = new Extent(1.0, 2.0, 3.0, 4.0);
        double[] array = extent;

        Assert.Equal(4, array.Length);
        Assert.Equal(1.0, array[0]);
    }

    [Fact]
    public void Extent_ImplicitConversion_FromArray()
    {
        var array = new[] { 1.0, 2.0, 3.0, 4.0 };
        Extent extent = array;

        Assert.Equal(1.0, extent.X1);
        Assert.Equal(2.0, extent.Y1);
        Assert.Equal(3.0, extent.X2);
        Assert.Equal(4.0, extent.Y2);
    }

    [Fact]
    public void Extent_ImplicitConversion_FromNull_ReturnsNull()
    {
        double[]? array = null;
        Extent? extent = array;

        Assert.Null(extent);
    }
}
