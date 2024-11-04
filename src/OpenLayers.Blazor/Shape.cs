using Microsoft.AspNetCore.Components;
using OpenLayers.Blazor.Internal;

namespace OpenLayers.Blazor;

/// <summary>
///     A base class for a shape on a map.
/// </summary>
public class Shape : Feature, IDisposable
{
    internal bool _updateableParametersChanged;
    internal bool _coordinatesParametersChanged;

    /// <summary>
    ///     Initializes a new instance of <see cref="Shape" />.
    /// </summary>
    public Shape() : this(ShapeType.Point)
    {
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Shape" />.
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
    ///     Gets or sets the type of shape.
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
    public string? Font
    {
        get => HasStyleOptions<StyleOptions.TextOptions>() ? GetOrCreateStyleOptions<StyleOptions.TextOptions>().Font : null;
        set => GetOrCreateStyleOptions<StyleOptions.TextOptions>().Font = value;
    }

    [Parameter]
    public string? Text
    {
        get => GetText();
        set => SetText(value);
    }

    protected virtual void SetText(string value)
    {
        GetOrCreateStyleOptions<StyleOptions.TextOptions>().Text = value;
    }

    protected virtual string? GetText()
    {
        if (HasStyleOptions<StyleOptions.TextOptions>())
            return GetOrCreateStyleOptions<StyleOptions.TextOptions>().Text;
        return null;
    }

    [Parameter]
    public string? TextColor
    {
        get => HasStyleOptions<StyleOptions.TextOptions>() ? GetOrCreateStyleOptions<StyleOptions.TextOptions>().Fill?.Color : null;
        set => GetOrCreateStyleOptions<StyleOptions.TextOptions>().Fill = new StyleOptions.FillOptions() { Color = value };
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
        get => GetRadius();
        set => SetRadius((double)value);
    }

    protected virtual void SetRadius(double value)
    {
        InternalFeature.Radius = value;
    }

    protected virtual double? GetRadius()
    {
        return InternalFeature.Radius;
    }

    [Parameter]
    public string? Stroke
    {
        get => GetStroke();
        set => SetStroke(value);
    }

    protected virtual void SetStroke(string color)
    {
        GetOrCreateStyleOptions<StyleOptions.StrokeOptions>().Color = color;
    }

    protected virtual string? GetStroke()
    {
        if (HasStyleOptions<StyleOptions.StrokeOptions>())
            return GetOrCreateStyleOptions<StyleOptions.StrokeOptions>().Color;
        return null;
    }

    [Parameter]
    public double? StrokeThickness
    {
        get => GetStrokeThickness();
        set => SetStrokeThickness((double)value);
    }

    protected virtual void SetStrokeThickness(double value)
    {
        GetOrCreateStyleOptions<StyleOptions.StrokeOptions>().Width = value;
    }

    protected virtual double? GetStrokeThickness()
    {
        if (HasStyleOptions<StyleOptions.StrokeOptions>())
            return GetOrCreateStyleOptions<StyleOptions.StrokeOptions>().Width;
        return null;
    }

    [Parameter]
    public string? Fill
    {
        get => GetFill();
        set => SetFill(value);
    }

    protected virtual void SetFill(string color)
    {
        GetOrCreateStyleOptions<StyleOptions.FillOptions>().Color = color;
    }

    protected virtual string? GetFill()
    {
        if (HasStyleOptions<StyleOptions.FillOptions>())
            return GetOrCreateStyleOptions<StyleOptions.FillOptions>().Color;
        return null;
    }

    [Parameter]
    public double? Scale
    {
        get => GetScale();
        set => SetScale(value);
    }

    protected virtual void SetScale(double? value)
    {
        GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>().Scale = value;
    }

    protected virtual double? GetScale()
    {
        if (HasStyleOptions<StyleOptions.IconStyleOptions>())
            return GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>().Scale;
        return null;
    }

    [Parameter]
    public double? TextScale
    {
        get => HasStyleOptions<StyleOptions.TextOptions>() ? GetOrCreateStyleOptions<StyleOptions.TextOptions>().Scale : null;
        set => GetOrCreateStyleOptions<StyleOptions.TextOptions>().Scale = value;
    }

    [Parameter]
    public int? ZIndex
    {
        get => GetOrCreateStyleOptions<StyleOptions>().ZIndex;
        set => GetOrCreateStyleOptions<StyleOptions>().ZIndex = value;
    }

    [Parameter]
    public Dictionary<string, object>? FlatStyle
    {
        get => InternalFeature.FlatStyle;
        set => InternalFeature.FlatStyle = value;
    }

    [Parameter]
    public List<StyleOptions>? Styles
    {
        get => InternalFeature.Styles;
        set => InternalFeature.Styles = value;
    }

    protected T GetOrCreateStyleOptions<T>(int? index = null) where T : class
    {
        if (Styles == null)
            Styles = new List<StyleOptions>();

        if (index == null)
            index = Styles.Count - 1; // last

        if (index < 0)
            index = 0;

        while(Styles.Count <= index)
            Styles.Add(new StyleOptions());

        var style = Styles[(int)index];

        if (typeof(T) == typeof(StyleOptions.StrokeOptions))
        {
            style.Stroke ??= new StyleOptions.StrokeOptions();
            return style.Stroke as T;
        }
        if (typeof(T) == typeof(StyleOptions.FillOptions))
        {
            style.Fill ??= new StyleOptions.FillOptions();
            return style.Fill as T;
        }
        if (typeof(T) == typeof(StyleOptions.TextOptions))
        {
            style.Text ??= new StyleOptions.TextOptions();
            return style.Text as T;
        }
        if (typeof(T) == typeof(StyleOptions.CircleStyleOptions))
        {
            style.Circle ??= new StyleOptions.CircleStyleOptions();
            return style.Circle as T;
        }
        if (typeof(T) == typeof(StyleOptions.TextOptions))
        {
            style.Text ??= new StyleOptions.TextOptions();
            return style.Text as T;
        }
        if (typeof(T) == typeof(StyleOptions.IconStyleOptions))
        {
            style.Icon ??= new StyleOptions.IconStyleOptions();
            return style.Icon as T;
        }
        if (typeof(T) == typeof(StyleOptions))
        {
            return style as T;
        }
        throw new ArgumentException("Invalid style option type");
    }

    protected bool HasStyleOptions<T>(int? index = null) where T : class
    {
        if (Styles == null)
            return false;

        if (index == null)
            index = Styles.Count - 1;

        if (Styles.Count <= index)
            return false;

        var style = Styles[(int)index];

        if (typeof(T) == typeof(StyleOptions.StrokeOptions))
            return style.Stroke != null;
        if (typeof(T) == typeof(StyleOptions.FillOptions))
            return style.Fill != null;
        if (typeof(T) == typeof(StyleOptions.TextOptions))
            return style.Text != null;
        if (typeof(T) == typeof(StyleOptions.CircleStyleOptions))
            return style.Circle != null;
        if (typeof(T) == typeof(StyleOptions.TextOptions))
            return style.Text != null;
        if (typeof(T) == typeof(StyleOptions.IconStyleOptions))
            return style.Icon != null;

        throw new ArgumentException("Invalid style option type");
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

    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(Fill), out string? bg) && bg != Fill)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(ZIndex), out int? zindex) && zindex != ZIndex)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Stroke), out string? bc) && bc != Stroke)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(StrokeThickness), out double? bs) && bs != StrokeThickness)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Text), out string? text) && text != Text)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Radius), out double? radius) && radius != Radius)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Scale), out double? scale) && scale != Scale)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Popup), out bool popup) && popup != Popup)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(TextScale), out double? ts) && ts != TextScale)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Font), out string? font) && font != Font)
            _updateableParametersChanged = true;
        if (parameters.TryGetValue(nameof(Coordinates), out Coordinates? c) && c != Coordinates)
            _coordinatesParametersChanged = true;

        Console.WriteLine($"{_updateableParametersChanged} {_coordinatesParametersChanged} { string.Join(";", parameters.ToDictionary().Select(p => p.Key + "=" + p.Value)) }");

        return base.SetParametersAsync(parameters);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_updateableParametersChanged)
        {
            await UpdateShape();
            _updateableParametersChanged = false;
            _coordinatesParametersChanged = false;
        }
        else if (_coordinatesParametersChanged)
        {
            await UpdateCoordinates();
            _coordinatesParametersChanged = false;
        }
    }

    /// <summary>
    ///     Update the shape explicitly on the parent layer.
    /// </summary>
    /// <returns></returns>
    public async Task UpdateShape()
    {
        if (Map != null && Layer != null)
            await Map.UpdateShape(this);
    }

    /// <summary>
    ///     Update the coordinates of the shape on the parent layer.
    /// </summary>
    /// <returns></returns>
    public async Task UpdateCoordinates()
    {
        if (Map != null && Layer != null)
            await Map.SetCoordinates(this, Coordinates);
    }
}