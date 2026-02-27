using Xunit;

namespace OpenLayers.Blazor.Tests;

public class ShapeTests
{
    [Fact]
    public void Circle_Constructor_SetsProperties()
    {
        var center = new Coordinate(10, 20);
        var radius = 100.0;
        var circle = new Circle(center, radius);

        Assert.Equal(center, circle.Center);
        Assert.Equal(radius, circle.Radius);
        Assert.Equal(ShapeType.Circle, circle.ShapeType);
    }

    [Fact]
    public void Point_Constructor_SetsProperties()
    {
        var coord = new Coordinate(5, 15);
        var point = new Point(coord);

        Assert.Equal(coord, point.Coordinate);
        Assert.Equal(ShapeType.Point, point.ShapeType);
    }

    [Fact]
    public void Line_Constructor_SetsPoints()
    {
        var p1 = new Coordinate(0, 0);
        var p2 = new Coordinate(10, 10);
        var line = new Line(p1, p2);

        Assert.Equal(2, line.Points.Count);
        Assert.Equal(p1, line.Points[0]);
        Assert.Equal(p2, line.Points[1]);
        Assert.Equal(ShapeType.LineString, line.ShapeType);
    }

    [Fact]
    public void Polygon_Constructor_SetsPoints()
    {
        var points = new List<Coordinate>
        {
            new Coordinate(0, 0),
            new Coordinate(10, 0),
            new Coordinate(10, 10),
            new Coordinate(0, 10)
        };
        var polygon = new Polygon(points);

        Assert.Equal(points, polygon.Points);
        Assert.Equal(ShapeType.Polygon, polygon.ShapeType);
    }

    [Fact]
    public void Shape_StyleProperties_SetCorrectly()
    {
        var shape = new Circle(new Coordinate(0, 0), 50);

        shape.Fill = "#FF0000";
        shape.Stroke = "#00FF00";
        shape.StrokeThickness = 2.5;

        Assert.Equal("#FF0000", shape.Fill);
        Assert.Equal("#00FF00", shape.Stroke);
        Assert.Equal(2.5, shape.StrokeThickness);
    }

    [Fact]
    public void Shape_TextProperties_SetCorrectly()
    {
        var shape = new Point(new Coordinate(0, 0));

        shape.Text = "Test Label";
        shape.TextColor = "#FFFFFF";
        shape.TextScale = 1.5;

        Assert.Equal("Test Label", shape.Text);
        Assert.Equal("#FFFFFF", shape.TextColor);
        Assert.Equal(1.5, shape.TextScale);
    }

    [Fact]
    public void Shape_ZIndex_SetCorrectly()
    {
        var shape = new Point(new Coordinate(0, 0));

        shape.ZIndex = 10;

        Assert.Equal(10, shape.ZIndex);
    }

    [Fact]
    public void Shape_Popup_SetCorrectly()
    {
        var shape = new Point(new Coordinate(0, 0));

        shape.Popup = true;

        Assert.True(shape.Popup);
    }

    [Fact]
    public void Marker_Constructor_SetsTypeAndCoordinate()
    {
        var coord = new Coordinate(5, 10);
        var marker = new Marker(MarkerType.MarkerPin, coord, "Test", PinColor.Blue);

        Assert.Equal(MarkerType.MarkerPin, marker.Type);
        Assert.Equal(coord, marker.Coordinate);
        Assert.Equal("Test", marker.Text);
        Assert.Equal(PinColor.Blue, marker.PinColor);
    }

    [Fact]
    public void Marker_AwesomeConstructor_SetsProperties()
    {
        var coord = new Coordinate(5, 10);
        var icon = '\uf1b9'; // Font Awesome icon
        var marker = new Marker(coord, icon);

        Assert.Equal(MarkerType.MarkerAwesome, marker.Type);
        Assert.Equal(coord, marker.Coordinate);
        Assert.Equal(icon.ToString(), marker.Text);
    }

    [Fact]
    public void Marker_CustomImage_SetsProperties()
    {
        var coord = new Coordinate(5, 10);
        var imageSource = "https://example.com/marker.png";
        var marker = new Marker(coord, imageSource, 32, 32, 16, 32);

        Assert.Equal(MarkerType.MarkerCustomImage, marker.Type);
        Assert.Equal(coord, marker.Coordinate);
        Assert.Equal(imageSource, marker.Source);
        Assert.NotNull(marker.Size);
        Assert.Equal(32, marker.Size[0]);
        Assert.Equal(32, marker.Size[1]);
        Assert.NotNull(marker.Anchor);
        Assert.Equal(16, marker.Anchor[0]);
        Assert.Equal(32, marker.Anchor[1]);
    }

    [Fact]
    public void Marker_Rotation_SetCorrectly()
    {
        var marker = new Marker(MarkerType.MarkerPin, new Coordinate(0, 0));

        marker.Rotation = Math.PI / 2; // 90 degrees

        Assert.Equal(Math.PI / 2, marker.Rotation);
    }

    [Fact]
    public void Marker_Scale_SetCorrectly()
    {
        var marker = new Marker(MarkerType.MarkerPin, new Coordinate(0, 0));

        marker.Scale = 1.5;

        Assert.Equal(1.5, marker.Scale);
    }
}
