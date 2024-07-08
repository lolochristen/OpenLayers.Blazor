using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

public partial class Layer : ComponentBase
{
    internal Internal.Layer _internalLayer = new() { LayerType = LayerType.Tile };

    private Func<Shape, StyleOptions?> _styleCallback;

    private bool _updateableParametersChanged = false;

    internal Internal.Layer InternalLayer => _internalLayer;

    [CascadingParameter] public Map? Map { get; set; }

    /// <summary>
    ///     Add shapes to the layer
    /// </summary>
    /// <example>
    ///     <Shapes>
    ///         <Line Points="new []{new Coordinate(1197650, 2604200), new Coordinate(1177650, 2624200)}" BorderColor="cyan"></Line>
    ///     </Shapes>
    /// </example>
    [Parameter]
    public RenderFragment? Shapes { get; set; }

    /// <summary>
    ///     Gets or sets the identifier for the layer.
    /// </summary>
    [Parameter]
    public string Id
    {
        get => _internalLayer.Id;
        set => _internalLayer.Id = value;
    }

    /// <summary>
    ///     Gets or sets the opacity of the layer. value must be between 0 and 1.
    /// </summary>
    [Parameter]
    public double Opacity
    {
        get => _internalLayer.Opacity;
        set => _internalLayer.Opacity = value;
    }

    /// <summary>
    ///     Gets or sets the visibility of the layer.
    /// </summary>
    [Parameter]
    public bool Visibility
    {
        get => _internalLayer.Visibility;
        set => _internalLayer.Visibility = value;
    }

    /// <summary>
    ///     Gets or sets the extent of layer defined
    /// </summary>
    [Parameter]
    public Extent? Extent
    {
        get => _internalLayer.Extent;
        set => _internalLayer.Extent = value;
    }

    [Parameter]
    public int? ZIndex
    {
        get => _internalLayer.ZIndex;
        set => _internalLayer.ZIndex = value;
    }

    [Parameter]
    public double? MinResolution
    {
        get => _internalLayer.MinResolution;
        set => _internalLayer.MinResolution = value;
    }

    [Parameter]
    public double? MaxResolution
    {
        get => _internalLayer.MaxResolution;
        set => _internalLayer.MaxResolution = value;
    }

    [Parameter]
    public double? MinZoom
    {
        get => _internalLayer.MinZoom;
        set => _internalLayer.MinZoom = value;
    }

    [Parameter]
    public double? MaxZoom
    {
        get => _internalLayer.MaxZoom;
        set => _internalLayer.MaxZoom = value;
    }

    [Parameter]
    public string? Class
    {
        get => _internalLayer.ClassName;
        set => _internalLayer.ClassName = value;
    }

    /// <summary>
    ///     Gets or set the url of the source for the layer.
    /// </summary>
    [Parameter]
    public string? Url
    {
        get => _internalLayer.Source.Url;
        set => _internalLayer.Source.Url = value;
    }

    /// <summary>
    ///     Gets or set urls of the source for the layer.
    /// </summary>
    [Parameter]
    public string[]? Urls
    {
        get => _internalLayer.Source.Urls;
        set => _internalLayer.Source.Urls = value;
    }

    /// <summary>
    ///     Gets or set the type of the layer.
    /// </summary>
    [Parameter]
    public LayerType LayerType
    {
        get => _internalLayer.LayerType;
        set => _internalLayer.LayerType = value;
    }

    /// <summary>
    ///     Gets or set the source type of the layer.
    /// </summary>
    [Parameter]
    public SourceType SourceType
    {
        get => _internalLayer.Source.SourceType;
        set
        {
            _internalLayer.Source.SourceType = value;
            if (value >= SourceType.VectorKML && value <= SourceType.VectorWFS && _internalLayer.LayerType == LayerType.Tile) // for compatibility
                _internalLayer.LayerType = value == SourceType.VectorMVT ? LayerType.VectorTile : LayerType.Vector;
        }
    }

    /// <summary>
    ///     Gets or sets the attributions of the map layer.
    /// </summary>
    [Parameter]
    public string? Attributions
    {
        get => _internalLayer.Source.Attributions;
        set => _internalLayer.Source.Attributions = value;
    }

    [Parameter]
    public string? CrossOrigin
    {
        get => _internalLayer.Source.CrossOrigin;
        set => _internalLayer.Source.CrossOrigin = value;
    }

    [Parameter]
    public Dictionary<string, object> SourceParameters
    {
        get => _internalLayer.Source.Params;
        set => _internalLayer.Source.Params = value;
    }

    [Parameter]
    public string? Key
    {
        get => _internalLayer.Source.Key;
        set => _internalLayer.Source.Key = value;
    }

    [Parameter]
    public BingMapImagerySet? ImagerySet
    {
        get => _internalLayer.Source.ImagerySet != null ? Enum.Parse<BingMapImagerySet>(_internalLayer.Source.ImagerySet) : null;
        set => _internalLayer.Source.ImagerySet = value.ToString();
    }

    [Parameter]
    public double Gutter
    {
        get => _internalLayer.Source.Gutter;
        set => _internalLayer.Source.Gutter = value;
    }

    [Parameter]
    public bool WrapX
    {
        get => _internalLayer.Source.WrapX;
        set => _internalLayer.Source.WrapX = value;
    }

    [Parameter]
    public bool Transition
    {
        get => _internalLayer.Source.Transition;
        set => _internalLayer.Source.Transition = value;
    }

    [Parameter]
    public double ZDirection
    {
        get => _internalLayer.Source.ZDirection;
        set => _internalLayer.Source.ZDirection = value;
    }

    [Parameter]
    public string? SourceLayer
    {
        get => _internalLayer.Source.Layer;
        set => _internalLayer.Source.Layer = value;
    }

    [Parameter]
    public string? MatrixSet
    {
        get => _internalLayer.Source.MatrixSet;
        set => _internalLayer.Source.MatrixSet = value;
    }

    /// <summary>
    ///     Gets or sets to tile image format such as image/png.
    /// </summary>
    [Parameter]
    public string? Format
    {
        get => _internalLayer.Source.Format;
        set => _internalLayer.Source.Format = value;
    }

    [Parameter]
    public double Preload
    {
        get => _internalLayer.Preload;
        set => _internalLayer.Preload = value;
    }

    /// <summary>
    ///     Gets ot sets the server type for the source of tile layers
    /// </summary>
    [Parameter]
    public string? ServerType
    {
        get => _internalLayer.Source.ServerType;
        set => _internalLayer.Source.ServerType = value;
    }

    /// <summary>
    ///     Gets or sets the layers the layer parameter for the source tile layers
    /// </summary>
    [Parameter]
    public string? Layers
    {
        get => _internalLayer.Source.Params.ContainsKey("LAYERS") ? _internalLayer.Source.Params["LAYERS"].ToString() : null;
        set => _internalLayer.Source.Params["LAYERS"] = value;
    }

    /// <summary>
    ///     Gets or set styles parameter for tile layers.
    /// </summary>
    [Parameter]
    public string? Styles
    {
        get => _internalLayer.Source.Params.ContainsKey("STYLES") ? _internalLayer.Source.Params["STYLES"].ToString() : null;
        set => _internalLayer.Source.Params["STYLES"] = value;
    }

    /// <summary>
    ///     Gets or set flat styles for vector layers. See
    ///     https://openlayers.org/en/latest/apidoc/module-ol_style_flat.html#~FlatStyleLike
    /// </summary>
    [Parameter]
    public Dictionary<string, object>? FlatStyle
    {
        get => _internalLayer.FlatStyle;
        set => _internalLayer.FlatStyle = value;
    }

    /// <summary>
    ///     Gets or set a default style for the given layer
    /// </summary>
    [Parameter]
    public StyleOptions? Style
    {
        get => _internalLayer.Style;
        set => _internalLayer.Style = value;
    }

    /// <summary>
    ///     Gets or sets the title of tile layers.
    /// </summary>
    [Parameter]
    public string? Title
    {
        get => _internalLayer.Properties.ContainsKey("TITLE") ? _internalLayer.Properties["TITLE"].ToString() : null;
        set => _internalLayer.Properties["TITLE"] = value;
    }

    /// <summary>
    ///     Gets or sets a dynamic object for format options ol/format/*.
    /// </summary>
    [Parameter]
    public dynamic? FormatOptions
    {
        get => _internalLayer.Source.FormatOptions;
        set => _internalLayer.Source.FormatOptions = value;
    }

    /// <summary>
    ///     Gets or sets direct data for vector layers
    /// </summary>
    [Parameter]
    public dynamic? Data
    {
        get => _internalLayer.Source.Data;
        set => _internalLayer.Source.Data = value;
    }

    [Parameter]
    public string? Projection
    {
        get => _internalLayer.Source.Projection;
        set => _internalLayer.Source.Projection = value;
    }

    /// <summary>
    ///     Gets and set additional options for the layer
    /// </summary>
    [Parameter]
    public dynamic? Options
    {
        get => _internalLayer.Options;
        set => _internalLayer.Options = value;
    }

    /// <summary>
    ///     Gets or sets if the layer (vector) should synchronize features collections
    /// </summary>
    [Parameter]
    public bool RaiseShapeEvents
    {
        get => _internalLayer.RaiseShapeEvents;
        set => _internalLayer.RaiseShapeEvents = value;
    }

    /// <summary>
    ///     Gets and set additional options for the layer
    /// </summary>
    [Parameter]
    public dynamic? SourceOptions
    {
        get => _internalLayer.Source.Options;
        set => _internalLayer.Source.Options = value;
    }

    /// <summary>
    ///     Gets or sets a callback to set styles as <see cref="StyleOptions" /> for the given shape.
    /// </summary>
    [Parameter]
    public Func<Shape, StyleOptions?> StyleCallback
    {
        get => _styleCallback;
        set
        {
            _styleCallback = value;
            if (_styleCallback != null)
                _internalLayer.UseStyleCallback = true;
        }
    }

    /// <summary>
    ///     Gets or sets the event callback when a shape is added to the layer.
    /// </summary>
    [Parameter]
    public EventCallback<Shape> OnShapeAdded { get; set; }

    /// <summary>
    ///     Gets or sets the event callback when a shape is changed on the layer.
    /// </summary>
    [Parameter]
    public EventCallback<Shape> OnShapeChanged { get; set; }

    /// <summary>
    ///     Gets or sets the event callback when a shape is removed from the layer.
    /// </summary>
    [Parameter]
    public EventCallback<Shape> OnShapeRemoved { get; set; }

    [Parameter]
    public bool SelectionEnabled { get; set; }

    [Parameter]
    public StyleOptions? SelectionStyle { get; set; }

    [Parameter]
    public EventCallback<SelectionChangedArgs> SelectionChanged { get; set; }

    [Parameter]
    public Shape SelectedShape { get; set; }

    [Parameter]
    public EventCallback<Shape> SelectedShapeChanged { get; set; }

    [Parameter]
    public bool MultiSelect { get; set; }

    /// <summary>
    ///    Gets the list of shapes in the layer.
    /// </summary>
    public ObservableCollection<Shape> ShapesList { get; } = new();

    internal void Initialize(Map map)
    {
        if (Map != map)
            Map = map;

        if (!Map.LayersList.Contains(this))
            Map.LayersList.Add(this);

        ShapesList.CollectionChanged += ShapesOnCollectionChanged;
    }

    private void ShapesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (Shape newShape in e.NewItems)
            {
                newShape.Layer = this;
                newShape.Map = Map;
            }

        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            _ = Map.SetShapesInternal(this, null);
        }
        else
        {
            if (e.OldItems != null)
                foreach (Shape oldShape in e.OldItems)
                {
                    _ = Map.RemoveShapeInternal(this, oldShape);
                    oldShape.Layer = null;
                    oldShape.Map = null;
                }

            if (e.NewItems != null)
                foreach (Shape newShape in e.NewItems)
                {
                    newShape.Layer = this;
                    newShape.Map = Map;
                    _ = Map.AddShapeInternal(this, newShape);
                }
        }
    }

    protected override void OnInitialized()
    {
        if (Map != null)
            Initialize(Map);

        base.OnInitialized();
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(Visibility), out bool visibility) && visibility != Visibility)
            _updateableParametersChanged = true;

        if (parameters.TryGetValue(nameof(Opacity), out double opacity) && opacity != Opacity)
            _updateableParametersChanged = true;

        if (parameters.TryGetValue(nameof(ZIndex), out int zindex) && zindex != ZIndex)
            _updateableParametersChanged = true;

        if (parameters.TryGetValue(nameof(Extent), out Extent extent) && extent != Extent)
            _updateableParametersChanged = true;

        bool selectionParamsChanged = false;

        if (parameters.TryGetValue(nameof(SelectionEnabled), out bool selectionEnabled) && selectionEnabled != SelectionEnabled)
            selectionParamsChanged = true;

        if (parameters.TryGetValue(nameof(SelectionStyle), out StyleOptions? style) && style != SelectionStyle)
            selectionParamsChanged = true;

        if (parameters.TryGetValue(nameof(MultiSelect), out bool multiSelect) && multiSelect != MultiSelect)
            selectionParamsChanged = true;

        if (selectionParamsChanged && Map != null)
        {
            _ = Map.SetSelectionSettings(this, selectionEnabled, style, multiSelect);
        }

        return base.SetParametersAsync(parameters);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_updateableParametersChanged)
        {
            await UpdateLayer();
            _updateableParametersChanged = false;
        }
    }

    /// <summary>
    ///     Updates the layer on the map.
    /// </summary>
    /// <returns>Task</returns>
    public async Task UpdateLayer()
    {
        if (Map != null)
            await Map.UpdateLayer(this);
    }

    public async Task<Shape> OnInternalShapeAdded(Internal.Shape shape)
    {
        var newShape = new Shape(shape)
        {
            Layer = this,
            Map = Map
        };

        ShapesList.CollectionChanged -= ShapesOnCollectionChanged;
        ShapesList.Add(newShape);
        ShapesList.CollectionChanged += ShapesOnCollectionChanged;

        await OnShapeAdded.InvokeAsync(newShape);
        return newShape;
    }

    public async Task OnInternalShapeRemoved(Shape shape)
    {
        if (!ShapesList.Contains(shape))
            return;

        ShapesList.CollectionChanged -= ShapesOnCollectionChanged;
        ShapesList.Remove(shape);
        ShapesList.CollectionChanged += ShapesOnCollectionChanged;

        await OnShapeRemoved.InvokeAsync(shape);
    }

    public async Task OnInternalShapeChanged(Shape shape)
    {
        await OnShapeChanged.InvokeAsync(shape);
        await shape.OnChanged.InvokeAsync(shape);
    }
}