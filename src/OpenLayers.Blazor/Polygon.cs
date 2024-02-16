using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using OpenLayers.Blazor.Internal;

namespace OpenLayers.Blazor;

/// <summary>
/// Polygon shape
/// </summary>
public class Polygon : Shape
{
    public Polygon() : base(ShapeType.Polygon)
    {
    }

    public Polygon(IEnumerable<Coordinate> coordinates) : this()
    {
        Points = coordinates;
    }

    /// <summary>
    /// Gets or sets the first array of coordinates for a polygon
    /// </summary>
    [Parameter]
    public IEnumerable<Coordinate> Points
    {
        get => CoordinatesHelper.GetCoordinates(InternalFeature.Coordinates);
        set => InternalFeature.Coordinates = new[] { CoordinatesHelper.SetCoordinates(value) }; // polygon requires double[][][]
    }

    /// <summary>
    /// Gets or sets an array of an array of coordinates for a polygon. see https://openlayers.org/en/latest/apidoc/module-ol_geom_Polygon-Polygon.html for specification. 
    /// </summary>
    [Parameter]
    public new IEnumerable<IEnumerable<Coordinate>> Coordinates
    {
        get => CoordinatesHelper.GetMultiCoordinates(InternalFeature.Coordinates);
        set => InternalFeature.Coordinates = CoordinatesHelper.SetMultiCoordinates(value);
    }
}