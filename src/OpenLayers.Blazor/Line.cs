using Microsoft.AspNetCore.Components;
using OpenLayers.Blazor.Internal;

namespace OpenLayers.Blazor;

public class Line : Shape<ShapeLine>
{
    public Line()
    {
    }

    public Line(params Coordinate[] point)
    {
        Points = point;
    }

    [Parameter]
    public IEnumerable<Coordinate> Points
    {
        get => InternalFeature.Points;
        set => InternalFeature.Points = value;
    }
}