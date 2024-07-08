using Microsoft.AspNetCore.Components;
using OpenLayers.Blazor.Internal;

namespace OpenLayers.Blazor;

public abstract class Shape<T> : Shape where T : Internal.Shape, new()
{
    internal Shape(T shape) : base(shape)
    {
    }

    internal new T InternalFeature => (T)base.InternalFeature;
}

/// <summary>
///     A base class for a shape on a map. 
/// </summary>
public class Shape : Feature, IDisposable
{
    /// <summary>
    /// Initializes a new instance of <see cref="Shape"/>.
    /// </summary>
    public Shape() : this(ShapeType.Point)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Shape"/>.
    /// </summary>
    /// <param name="shapeType"></param>
    public Shape(ShapeType shapeType)
    {
        InternalFeature = new Internal.Shape();
        ShapeType = shapeType;
    }

    internal Shape(Internal.Shape shape) : base(shape)
    {
    }

    internal new Internal.Shape InternalFeature
    {
        get => (Internal.Shape)base.InternalFeature;
        set => base.InternalFeature = value;
    }

    /// <summary>
    /// Gets or sets the attached parent map.
    /// </summary>
    [CascadingParameter] public Map? Map { get; set; }

    /// <summary>
    /// Gets or sets the attached parent layer.
    /// </summary>
    [CascadingParameter] public Layer? Layer { get; set; }

    /// <summary>
    /// Gets or sets the type of shape.
    /// </summary>
    [Parameter]
    public ShapeType ShapeType
    {
        get
        {
            switch (InternalFeature.GeometryType)
            {
                case GeometryTypes.MultiLineString:
                    return ShapeType.MultiLineString;
                case GeometryTypes.LineString:
                    return ShapeType.LineString;
                case GeometryTypes.MultiPolygon:
                    return ShapeType.MultiPolygon;
                case GeometryTypes.Polygon:
                    return ShapeType.Polygon;
                case GeometryTypes.Circle:
                    return ShapeType.Circle;
                case GeometryTypes.MultiPoint:
                    return ShapeType.MultiPoint;
                default:
                    return ShapeType.Point;
            }
        }
        set
        {
            switch (value)
            {
                case ShapeType.Point:
                    InternalFeature.GeometryType = GeometryTypes.Point;
                    break;
                case ShapeType.LineString:
                    InternalFeature.GeometryType = GeometryTypes.LineString;
                    break;
                case ShapeType.Polygon:
                    InternalFeature.GeometryType = GeometryTypes.Polygon;
                    break;
                case ShapeType.Circle:
                    InternalFeature.GeometryType = GeometryTypes.Circle;
                    break;
                case ShapeType.MultiPolygon:
                    InternalFeature.GeometryType = GeometryTypes.MultiPolygon;
                    break;
                case ShapeType.MultiLineString:
                    InternalFeature.GeometryType = GeometryTypes.MultiLineString;
                    break;
                case ShapeType.MultiPoint:
                    InternalFeature.GeometryType = GeometryTypes.MultiPoint;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }

    [Parameter] public EventCallback<Shape> OnChanged { get; set; }

    [Parameter]
    public string? Title
    {
        get => InternalFeature.Title;
        set => InternalFeature.Title = value;
    }

    [Parameter]
    public string? Label
    {
        get => InternalFeature.Label;
        set => InternalFeature.Label = value;
    }

    [Parameter]
    public bool Popup
    {
        get => InternalFeature.Popup;
        set => InternalFeature.Popup = value;
    }

    [Parameter]
    public double? Radius
    {
        get => InternalFeature.Radius;
        set => InternalFeature.Radius = value;
    }

    [Parameter]
    public string? Color
    {
        get => InternalFeature.Color;
        set => InternalFeature.Color = value;
    }

    [Parameter]
    public string? BorderColor
    {
        get => InternalFeature.BorderColor;
        set => InternalFeature.BorderColor = value;
    }


    [Parameter]
    public int? BorderSize
    {
        get => InternalFeature.BorderSize;
        set => InternalFeature.BorderSize = value;
    }


    [Parameter]
    public string? BackgroundColor
    {
        get => InternalFeature.BackgroundColor;
        set => InternalFeature.BackgroundColor = value;
    }

    [Parameter]
    public double? Scale
    {
        get => InternalFeature.Scale;
        set => InternalFeature.Scale = value;
    }

    [Parameter]
    public double? TextScale
    {
        get => InternalFeature.TextScale;
        set => InternalFeature.TextScale = value;
    }

    [Parameter]
    public string? Content
    {
        get => InternalFeature.Content;
        set => InternalFeature.Content = value;
    }

    [Parameter]
    public int? ZIndex
    {
        get => InternalFeature.ZIndex;
        set => InternalFeature.ZIndex = value;
    }

    public void Dispose()
    {
        Layer?.ShapesList.Remove(this);
    }

    protected override void OnInitialized()
    {
        if (Map != null && Layer == null) // just added to map/features
        {
            if (this is Marker)
            {
                Layer = Map.GetOrCreateMarkersLayer();
                Map.MarkersList.Add((Marker)this);
            }
            else
            {
                Layer = Map.GetOrCreateShapesLayer();
                Map.ShapesList.Add(this);
            }
        }
        else
        {
            Layer?.ShapesList.Add(this);
        }

        base.OnInitialized();
    }

    internal bool _updateableParametersChanged = false; 

    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(Color), out string? color) && color != Color)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(BackgroundColor), out string? bg) && bg != BackgroundColor)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(ZIndex), out int? zindex) && zindex != ZIndex)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(BorderColor), out string? bc) && bc != BorderColor)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(BorderSize), out int? bs) && bs != BorderSize)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Label), out string? label) && label != Label)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Content), out string? content) && bc != Content)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Radius), out double? radius) && radius != Radius)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Scale), out double? scale) && scale != Scale)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Popup), out bool popup) && popup != Popup)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(TextScale), out double? ts) && ts != TextScale)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Title), out string? title) && title != Title)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Coordinates), out Coordinates? c) && c != Coordinates)
        {
            Console.WriteLine($"Coordinates changed {c} {Coordinates}");
            _updateableParametersChanged = true;
        }

        return base.SetParametersAsync(parameters);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_updateableParametersChanged)
        {
            await UpdateShape();
            _updateableParametersChanged = false;
        }
    }

    public async Task UpdateShape()
    {
        if (Map != null && Layer != null)
            await Map.UpdateShape(this);
    }
}