using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

public partial class Layer : ComponentBase
{
    internal Internal.Layer _internalLayer = new() { LayerType = LayerType.Tile };

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
    /// Gets or sets the identifier for the layer.
    /// </summary>
    [Parameter]
    public string Id
    {
        get => _internalLayer.Id;
        set => _internalLayer.Id = value;
    }

    /// <summary>
    /// Gets or sets the opacity of the layer. value must be between 0 and 1.
    /// </summary>
    [Parameter]
    public double Opacity
    {
        get => _internalLayer.Opacity;
        set => _internalLayer.Opacity = value;
    }

    /// <summary>
    /// Gets or sets the visibility of the layer.
    /// </summary>
    [Parameter]
    public bool Visibility
    {
        get => _internalLayer.Visibility;
        set => _internalLayer.Visibility = value;
    }

    /// <summary>
    /// Gets or sets the extent of layer defined
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

    [Parameter]
    public string? Url
    {
        get => _internalLayer.Source.Url;
        set => _internalLayer.Source.Url = value;
    }

    [Parameter]
    public string[]? Urls
    {
        get => _internalLayer.Source.Urls;
        set => _internalLayer.Source.Urls = value;
    }

    
    [Parameter]
    public LayerType LayerType
    {
        get => _internalLayer.LayerType;
        set => _internalLayer.LayerType = value;
    }

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
    /// Gets or sets the attributions of the map layer.
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
    /// Gets or sets to tile image format such as image/png.
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
    /// Gets ot sets the server type for the source of tile layers
    /// </summary>
    [Parameter]
    public string? ServerType
    {
        get => _internalLayer.Source.ServerType;
        set => _internalLayer.Source.ServerType = value;
    }

    /// <summary>
    /// Gets or sets the layers the layer parameter for the source tile layers
    /// </summary>
    [Parameter]
    public string? Layers
    {
        get => _internalLayer.Source.Params.ContainsKey("LAYERS") ? _internalLayer.Source.Params["LAYERS"].ToString() : null;
        set => _internalLayer.Source.Params["LAYERS"] = value;
    }

    /// <summary>
    /// Gets or set styles parameter for tile layers.
    /// </summary>
    [Parameter]
    public string? Styles
    {
        get => _internalLayer.Source.Params.ContainsKey("STYLES") ? _internalLayer.Source.Params["STYLES"].ToString() : null;
        set => _internalLayer.Source.Params["STYLES"] = value;
    }

    /// <summary>
    /// Gets or set flat styles for vector layers. See https://openlayers.org/en/latest/apidoc/module-ol_style_flat.html#~FlatStyleLike
    /// </summary>
    [Parameter]
    public Dictionary<string, object>? FlatStyle
    {
        get => _internalLayer.FlatStyle;
        set => _internalLayer.FlatStyle = value;
    }

    [Parameter]
    public StyleOptions? Style
    {
        get => _internalLayer.Style;
        set => _internalLayer.Style = value;
    }

    /// <summary>
    /// Gets or sets the title of tile layers.
    /// </summary>
    [Parameter]
    public string? Title
    {
        get => _internalLayer.Properties.ContainsKey("TITLE") ? _internalLayer.Properties["TITLE"].ToString() : null;
        set => _internalLayer.Properties["TITLE"] = value;
    }

    [Parameter]
    public dynamic? FormatOptions
    {
        get => _internalLayer.Source.FormatOptions;
        set => _internalLayer.Source.FormatOptions = value;
    }

    /// <summary>
    /// Gets or sets direct data for vector layers
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
    /// Gets and set additional options for the layer
    /// </summary>
    [Parameter]
    public dynamic? Options
    {
        get => _internalLayer.Options;
        set => _internalLayer.Options = value;
    }

    /// <summary>
    /// Gets or sets if the layer (vector) should synchronize features collections
    /// </summary>
    [Parameter]
    public bool SyncFeatures
    {
        get => _internalLayer.SyncFeatures;
        set => _internalLayer.SyncFeatures = value;
    }

    /// <summary>
    /// Gets and set additional options for the layer
    /// </summary>
    [Parameter]
    public dynamic? SourceOptions
    {
        get => _internalLayer.Source.Options;
        set => _internalLayer.Source.Options = value;
    }

    private Func<Shape, StyleOptions?> _styleCallback;

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

    [Parameter] public EventCallback<Shape> OnShapeAdded { get; set; }

    [Parameter] public EventCallback<Shape> OnShapeChanged { get; set; }


    public ObservableCollection<Shape> ShapesList { get; } = new();

    public void Initialize(Map map)
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
        {
            foreach (Shape newShape in e.NewItems)
            {
                newShape.Layer = this;
                newShape.Map = Map;
            }
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

    protected override Task OnParametersSetAsync()
    {
        return UpdateLayer();
    }

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
}