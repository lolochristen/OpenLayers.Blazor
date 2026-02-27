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

    /// <summary>
    ///     Gets or sets the event callback when a shape parameter changes.
    /// </summary>
    [Parameter]
    public EventCallback<Shape> OnChanged { get; set; }

    /// <summary>
    ///     Gets or sets the font definition for text labels.
    /// </summary>
    [Parameter]
    public string? Font
    {
        get => HasStyleOptions<StyleOptions.TextOptions>() ? GetOrCreateStyleOptions<StyleOptions.TextOptions>().Font : null;
        set => GetOrCreateStyleOptions<StyleOptions.TextOptions>().Font = value;
    }

    /// <summary>
    /// Gets or sets the text label for the shape.
    /// </summary>
    [Parameter]
    public string? Text
    {
        get => GetText();
        set => SetText(value ?? "");
    }

    /// <summary>
    /// Sets the text label for the shape.
    /// </summary>
    /// <param name="value">The text value to display.</param>
    protected virtual void SetText(string value)
    {
        GetOrCreateStyleOptions<StyleOptions.TextOptions>().Text = value;
    }

    /// <summary>
    /// Gets the current text label of the shape.
    /// </summary>
    /// <returns>The text value, or null if no text is set.</returns>
    protected virtual string? GetText()
    {
        if (HasStyleOptions<StyleOptions.TextOptions>())
            return GetOrCreateStyleOptions<StyleOptions.TextOptions>().Text;
        return null;
    }

    /// <summary>
    ///     Gets or sets the text color for labels.
    /// </summary>
    [Parameter]
    public string? TextColor
    {
        get => HasStyleOptions<StyleOptions.TextOptions>() ? GetOrCreateStyleOptions<StyleOptions.TextOptions>().Fill?.Color : null;
        set => GetOrCreateStyleOptions<StyleOptions.TextOptions>().Fill = new StyleOptions.FillOptions() { Color = value };
    }

    /// <summary>
    ///     Gets or sets whether to show a popup for this shape when clicked.
    /// </summary>
    [Parameter]
    public bool Popup
    {
        get => InternalFeature.Popup;
        set => InternalFeature.Popup = value;
    }

    /// <summary>
    ///     Gets or sets the radius for circular shapes.
    /// </summary>
    [Parameter]
    public double? Radius
    {
        get => GetRadius();
        set => SetRadius(value ?? 0);
    }

    /// <summary>
    /// Sets the radius for circular shapes.
    /// </summary>
    /// <param name="value">The radius value in pixels.</param>
    protected virtual void SetRadius(double value)
    {
        InternalFeature.Radius = value;
    }

    /// <summary>
    /// Gets the current radius of circular shapes.
    /// </summary>
    /// <returns>The radius value, or null if not applicable.</returns>
    protected virtual double? GetRadius()
    {
        return InternalFeature.Radius;
    }

    /// <summary>
    ///     Gets or sets the stroke color for the shape outline.
    /// </summary>
    [Parameter]
    public string? Stroke
    {
        get => GetStroke();
        set
        {
            if (value != null)
                SetStroke(value);
        }
    }

    /// <summary>
    /// Sets the stroke color for the shape outline.
    /// </summary>
    /// <param name="color">The color value (e.g., "red", "#FF0000", "rgba(255,0,0,1)").</param>
    protected virtual void SetStroke(string color)
    {
        GetOrCreateStyleOptions<StyleOptions.StrokeOptions>().Color = color;
    }

    /// <summary>
    /// Gets the current stroke color of the shape.
    /// </summary>
    /// <returns>The stroke color, or null if not set.</returns>
    protected virtual string? GetStroke()
    {
        if (HasStyleOptions<StyleOptions.StrokeOptions>())
            return GetOrCreateStyleOptions<StyleOptions.StrokeOptions>().Color;
        return null;
    }

    /// <summary>
    ///     Gets or sets the stroke thickness (line width).
    /// </summary>
    [Parameter]
    public double? StrokeThickness
    {
        get => GetStrokeThickness();
        set => SetStrokeThickness(value ?? 0);
    }

    /// <summary>
    /// Sets the stroke thickness (line width) for the shape outline.
    /// </summary>
    /// <param name="value">The line width in pixels.</param>
    protected virtual void SetStrokeThickness(double value)
    {
        GetOrCreateStyleOptions<StyleOptions.StrokeOptions>().Width = value;
    }

    /// <summary>
    /// Gets the current stroke thickness of the shape.
    /// </summary>
    /// <returns>The line width, or null if not set.</returns>
    protected virtual double? GetStrokeThickness()
    {
        if (HasStyleOptions<StyleOptions.StrokeOptions>())
            return GetOrCreateStyleOptions<StyleOptions.StrokeOptions>().Width;
        return null;
    }

    /// <summary>
    ///     Gets or sets the fill color for the shape interior.
    /// </summary>
    [Parameter]
    public string? Fill
    {
        get => GetFill();
        set => SetFill(value!);
    }

    /// <summary>
    /// Sets the fill color for the shape interior.
    /// </summary>
    /// <param name="color">The color value (e.g., "blue", "#0000FF", "rgba(0,0,255,0.5)").</param>
    protected virtual void SetFill(string color)
    {
        GetOrCreateStyleOptions<StyleOptions.FillOptions>().Color = color;
    }

    /// <summary>
    /// Gets the current fill color of the shape.
    /// </summary>
    /// <returns>The fill color, or null if not set.</returns>
    protected virtual string? GetFill()
    {
        if (HasStyleOptions<StyleOptions.FillOptions>())
            return GetOrCreateStyleOptions<StyleOptions.FillOptions>().Color;
        return null;
    }

    /// <summary>
    ///     Gets or sets the scale factor for the shape.
    /// </summary>
    [Parameter]
    public double? Scale
    {
        get => GetScale();
        set => SetScale(value);
    }

    /// <summary>
    /// Sets the scale factor for the shape.
    /// </summary>
    /// <param name="value">The scale factor (1.0 = normal size, 2.0 = double size, etc.).</param>
    protected virtual void SetScale(double? value)
    {
        GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>().Scale = value;
    }

    /// <summary>
    /// Gets the current scale factor of the shape.
    /// </summary>
    /// <returns>The scale factor, or null if not set.</returns>
    protected virtual double? GetScale()
    {
        if (HasStyleOptions<StyleOptions.IconStyleOptions>())
            return GetOrCreateStyleOptions<StyleOptions.IconStyleOptions>().Scale;
        return null;
    }

    /// <summary>
    ///     Gets or sets the scale factor for text labels.
    /// </summary>
    [Parameter]
    public double? TextScale
    {
        get => HasStyleOptions<StyleOptions.TextOptions>() ? GetOrCreateStyleOptions<StyleOptions.TextOptions>().Scale : null;
        set => GetOrCreateStyleOptions<StyleOptions.TextOptions>().Scale = value;
    }

    /// <summary>
    ///     Gets or sets the z-index for shape layering.
    /// </summary>
    [Parameter]
    public int? ZIndex
    {
        get => GetOrCreateStyleOptions<StyleOptions>().ZIndex;
        set => GetOrCreateStyleOptions<StyleOptions>().ZIndex = value;
    }

    /// <summary>
    ///     Gets or sets flat style options. See https://openlayers.org/en/latest/apidoc/module-ol_style_flat.html#~FlatStyleLike
    /// </summary>
    [Parameter]
    public Dictionary<string, object>? FlatStyle
    {
        get => InternalFeature.FlatStyle;
        set => InternalFeature.FlatStyle = value;
    }

    /// <summary>
    ///     Gets or sets a list of style options for multi-style shapes.
    /// </summary>
    [Parameter]
    public List<StyleOptions>? Styles
    {
        get => InternalFeature.Styles;
        set => InternalFeature.Styles = value;
    }

    /// <summary>
    /// GetOrCreateStyleOptions
    /// </summary>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
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
            return (style.Stroke as T)!;
        }
        if (typeof(T) == typeof(StyleOptions.FillOptions))
        {
            style.Fill ??= new StyleOptions.FillOptions();
            return (style.Fill as T)!;
        }
        if (typeof(T) == typeof(StyleOptions.TextOptions))
        {
            style.Text ??= new StyleOptions.TextOptions();
            return (style.Text as T)!;
        }
        if (typeof(T) == typeof(StyleOptions.CircleStyleOptions))
        {
            style.Circle ??= new StyleOptions.CircleStyleOptions();
            return (style.Circle as T)!;
        }
        if (typeof(T) == typeof(StyleOptions.TextOptions))
        {
            style.Text ??= new StyleOptions.TextOptions();
            return (style.Text as T)!;
        }
        if (typeof(T) == typeof(StyleOptions.IconStyleOptions))
        {
            style.Icon ??= new StyleOptions.IconStyleOptions();
            return (style.Icon as T)!;
        }
        if (typeof(T) == typeof(StyleOptions))
        {
            return (style as T)!;
        }
        throw new ArgumentException("Invalid style option type");
    }

    /// <summary>
    /// Checks if a style options object of the specified type exists at the given index.
    /// </summary>
    /// <typeparam name="T">The type of style options to check for.</typeparam>
    /// <param name="index">The style index, or null to check the last style.</param>
    /// <returns>True if the style options exist, false otherwise.</returns>
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

    /// <summary>
    /// Disposes the shape and removes it from its layer.
    /// </summary>
    public void Dispose()
    {
        Layer?.ShapesList.Remove(this);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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
        if (parameters.TryGetValue(nameof(Coordinates), out Coordinates? c) && c! != Coordinates)
            _coordinatesParametersChanged = true;

        return base.SetParametersAsync(parameters);
    }

    /// <inheritdoc/>
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