using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace OpenLayers.Blazor;

/// <summary>
///     Component to show OpenLayers Maps
/// </summary>
public partial class Map : IAsyncDisposable
{
    private Coordinate? _internalCenter;
    private double? _internalZoom;
    private INotifyCollectionChanged? _layerCollectionRef;
    private string _mapId;
    private INotifyCollectionChanged? _markersCollectionRef;
    private IJSObjectReference? _module;
    private Feature? _popupContext;
    private string _popupId;
    private INotifyCollectionChanged? _shapeCollectionRef;

    public Map()
    {
        _mapId = Guid.NewGuid().ToString();
        _popupId = Guid.NewGuid().ToString();
    }

    [Inject] private IJSRuntime? JSRuntime { get; set; }

    /// <summary>
    /// Gets or set then center of the map
    /// </summary>
    [Parameter] public Coordinate Center { get; set; } = new(0, 0);

    /// <summary>
    /// Event when center changes
    /// </summary>
    [Parameter] public EventCallback<Coordinate> CenterChanged { get; set; }

    /// <summary>
    /// Zoom level of the map
    /// </summary>
    [Parameter] public double Zoom { get; set; } = 2;

    /// <summary>
    /// Event on zoom changes
    /// </summary>
    [Parameter] public EventCallback<double> ZoomChanged { get; set; }

    /// <summary>
    /// Collection of attached markers
    /// </summary>
    public ObservableCollection<Marker> MarkersList { get; } = new();

    /// <summary>
    /// Collection of attached shapes
    /// </summary>
    public ObservableCollection<Shape> ShapesList { get; } = new();

    /// <summary>
    /// Event when a feature (shapes/markers) is called
    /// </summary>
    [Parameter] public EventCallback<Feature> OnFeatureClick { get; set; }

    /// <summary>
    /// Event when a marker get clicked
    /// </summary>
    [Parameter] public EventCallback<Marker> OnMarkerClick { get; set; }

    /// <summary>
    /// Event when a shape gets clicked
    /// </summary>
    [Parameter] public EventCallback<Internal.Shape> OnShapeClick { get; set; }

    /// <summary>
    /// Event when a point in the map gets clicked. Event returns current coordinates
    /// </summary>
    [Parameter] public EventCallback<Coordinate> OnClick { get; set; }

    /// <summary>
    /// Event when the pointer gets moved
    /// </summary>
    [Parameter] public EventCallback<Coordinate> OnPointerMove { get; set; }

    /// <summary>
    /// Content to show as a popup when a shape or marker gets clicked and <see cref="Shape.Popup"/> is set to true
    /// </summary>
    [Parameter] public RenderFragment<Feature?>? Popup { get; set; }

    /// <summary>
    /// Definition of Layers to show in the map. Only items of <see cref="Layer"/> are considered.
    /// </summary>
    /// <example>
    ///       <Layers>
    ///          <Layer SourceType="SourceType.TileWMS"
    ///             Url="https://sedac.ciesin.columbia.edu/geoserver/ows?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&FORMAT=image%2Fpng&TRANSPARENT=true&LAYERS=gpw-v3%3Agpw-v3-population-density_2000&LANG=en"
    ///             Opacity=".3"
    ///             CrossOrigin="anonymous" />
    ///       </Layers>
    /// </example>
    [Parameter] public RenderFragment? Layers { get; set; }

    /// <summary>
    /// Definition of Features to show on the map. Only items of the type <see cref="Marker"/> or <see cref="Shape"/> (<see cref="Line"/>, <see cref="Circle"/>) are considered.
    /// </summary>
    /// <example>
    ///       <Features>
    ///          <Marker Type="MarkerType.MarkerPin" Coordinate="new Coordinate(1197650, 2604200)"></Marker>
    ///          <Line Points="new []{new Coordinate(1197650, 2604200), new Coordinate(1177650, 2624200)}" BorderColor="cyan"></Line>
    ///       </Features>
    /// </example>
    [Parameter] public RenderFragment? Features { get; set; }

    /// <summary>
    /// Collection of all Layers
    /// </summary>
    public ObservableCollection<Layer> LayersList { get; } = new();

    /// <summary>
    /// Defaults to use for the map rendering
    /// </summary>
    public Defaults Defaults { get; } = new();

    /// <summary>
    /// Class of the map element
    /// </summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// Styles of the map element
    /// </summary>
    [Parameter] public string? Style { get; set; }

    /// <summary>
    /// Coordinates projection to use of the layers and events. Default is set to "EPSG:4326" (https://epsg.io/4326).
    /// Additionally to the default OpenLayers projections, the swiss projections EPSG:2056 (VT95) and EPSG:21781 (VT03) are supported.
    /// </summary>
    [Parameter]
    public string CoordinatesProjection
    {
        get => Defaults.CoordinatesProjection;
        set => Defaults.CoordinatesProjection = value;
    }

    /// <summary>
    /// Unit of the ScaleLine
    /// </summary>
    [Parameter]
    public ScaleLineUnit ScaleLineUnit
    {
        get => Defaults.ScaleLineUnit;
        set => Defaults.ScaleLineUnit = value;
    }

    private DotNetObjectReference<Map>? Instance { get; set; }

    /// <summary>
    /// Disposing resources.
    /// </summary>
    /// <returns>ValueTask</returns>
    public async ValueTask DisposeAsync()
    {
        MarkersList.CollectionChanged -= MarkersOnCollectionChanged;
        ShapesList.CollectionChanged -= ShapesOnCollectionChanged;
        LayersList.CollectionChanged -= LayersOnCollectionChanged;

        if (_module != null)
        {
            await _module.InvokeVoidAsync("MapOLDispose", _mapId);
            await _module.DisposeAsync();
            _module = null;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_markersCollectionRef != null)
        {
            _markersCollectionRef.CollectionChanged -= MarkersOnCollectionChanged;
            if (!ReferenceEquals(_markersCollectionRef, MarkersList)) await SetMarkers(MarkersList);
        }

        MarkersList.CollectionChanged += MarkersOnCollectionChanged;
        _markersCollectionRef = MarkersList;

        if (_shapeCollectionRef != null)
        {
            _shapeCollectionRef.CollectionChanged -= ShapesOnCollectionChanged;
            if (!ReferenceEquals(_shapeCollectionRef, ShapesList)) await SetShapes(ShapesList);
        }

        ShapesList.CollectionChanged += ShapesOnCollectionChanged;
        _shapeCollectionRef = ShapesList;

        if (_layerCollectionRef != null)
        {
            _layerCollectionRef.CollectionChanged -= LayersOnCollectionChanged;
            if (!ReferenceEquals(_layerCollectionRef, LayersList)) await SetLayers(LayersList);
        }

        LayersList.CollectionChanged += LayersOnCollectionChanged;
        _layerCollectionRef = LayersList;

        if (!Center.Equals(_internalCenter)) await SetCenter(Center);

        if (Zoom != _internalZoom) await SetZoom(Zoom);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _module ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"/_content/{Assembly.GetExecutingAssembly().GetName().Name}/openlayers_interop.js");
            Instance ??= DotNetObjectReference.Create(this);

            if (_module != null)
                await _module.InvokeVoidAsync("MapOLInit", _mapId, _popupId, Defaults, Center, Zoom,
                    MarkersList.Select(p => p.InternalFeature).ToArray(),
                    ShapesList.Select(p => p.InternalFeature).ToArray(),
                    LayersList.Select(p => p.InternalLayer).ToArray(),
                    Instance);
        }
    }

    [JSInvokable]
    public async Task OnInternalFeatureClick(Internal.Feature feature)
    {
        await OnFeatureClick.InvokeAsync(new Feature(feature));
    }

    [JSInvokable]
    public async Task OnInternalMarkerClick(Internal.Marker marker)
    {
        var m = MarkersList.FirstOrDefault(p => p.InternalFeature.ID == marker.ID);

        if (m != null)
        {
            _popupContext = m;
            await OnMarkerClick.InvokeAsync(m);
            StateHasChanged();
        }
    }

    [JSInvokable]
    public async Task OnInternalShapeClick(Internal.Shape shape)
    {
        //_popupContext = shape;
        await OnShapeClick.InvokeAsync(shape);
        StateHasChanged();
    }

    [JSInvokable]
    public Task OnInternalClick(Coordinate coordinate)
    {
        return OnClick.InvokeAsync(coordinate);
    }

    [JSInvokable]
    public Task OnInternalPointerMove(Coordinate coordinate)
    {
        return OnPointerMove.InvokeAsync(coordinate);
    }

    [JSInvokable]
    public async Task OnInternalCenterChanged(Coordinate coordinate)
    {
        Center = coordinate;
        _internalCenter = coordinate;
        await CenterChanged.InvokeAsync(Center);
    }

    /// <summary>
    /// Passes the center coordination to underlying map
    /// </summary>
    /// <param name="center">Center Coordinates</param>
    /// <returns>Task</returns>
    public async Task SetCenter(Coordinate center)
    {
        if (_module != null) await _module.InvokeVoidAsync("MapOLCenter", _mapId, center);
        _internalCenter = Center;
    }

    [JSInvokable]
    public async Task OnInternalZoomChanged(double zoom)
    {
        Zoom = zoom;
        _internalZoom = zoom;
        await ZoomChanged.InvokeAsync(zoom);
    }

    /// <summary>
    /// Sets the zoom level to underlying map component
    /// </summary>
    /// <param name="zoom">zoom level</param>
    /// <returns></returns>
    public async Task SetZoom(double zoom)
    {
        if (_module != null) await _module.InvokeVoidAsync("MapOLZoom", _mapId, zoom);
        _internalZoom = Zoom;
    }

    /// <summary>
    /// Zooms to the given extent
    /// </summary>
    /// <param name="extent"></param>
    /// <returns></returns>
    public ValueTask SetZoomToExtent(Extent extent)
    {
        return _module?.InvokeVoidAsync("MapOLZoomToExtent", _mapId, extent.ToString()) ?? ValueTask.CompletedTask;
    }

    /// <summary>
    /// Loads GeoJson data (https://geojson.org/) to the map
    /// </summary>
    /// <param name="json">GeoJson Data</param>
    public ValueTask LoadGeoJson(JsonElement json)
    {
        return _module?.InvokeVoidAsync("MapOLLoadGeoJson", _mapId, json) ?? ValueTask.CompletedTask;
    }

    /// <summary>
    /// Centers the map to the current GPS geo location
    /// </summary>
    public ValueTask CenterToCurrentGeoLocation()
    {
        return _module?.InvokeVoidAsync("MapOLCenterToCurrentGeoLocation", _mapId) ?? ValueTask.CompletedTask;
    }

    /// <summary>
    /// Current available GPS geo location
    /// </summary>
    /// <returns>Current coordinates in the current map projection</returns>
    public ValueTask<Coordinate?> GetCurrentGeoLocation()
    {
        return _module?.InvokeAsync<Coordinate?>("MapOLGetCurrentGeoLocation", _mapId) ?? ValueTask.FromResult<Coordinate?>(null);
    }

    /// <summary>
    /// Set given markers to underlying map component
    /// </summary>
    /// <param name="markers">collection of markers</param>
    public ValueTask SetMarkers(IEnumerable<Marker> markers)
    {
        return _module?.InvokeVoidAsync("MapOLMarkers", _mapId, markers.Select(p => p.InternalFeature).ToArray()) ?? ValueTask.CompletedTask;
    }

    /// <summary>
    /// Set given shapes to underlying map component
    /// </summary>
    /// <param name="shapes">collection of shapes</param>
    /// <returns></returns>
    public ValueTask SetShapes(IEnumerable<Shape> shapes)
    {
        return _module?.InvokeVoidAsync("MapOLSetShapes", _mapId, shapes.Select(p => p.InternalFeature).ToArray()) ?? ValueTask.CompletedTask;
    }

    /// <summary>
    /// Set all layers to underlying map component
    /// </summary>
    /// <param name="layers">collection of layers</param>
    public ValueTask SetLayers(IEnumerable<Layer> layers)
    {
        return _module?.InvokeVoidAsync("MapOLSetLayers", _mapId, layers.Select(p => p.InternalLayer).ToArray()) ?? ValueTask.CompletedTask;
    }

    /// <summary>
    /// Updates a single layer
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public ValueTask UpdateLayer(Layer layer)
    {
        return _module?.InvokeVoidAsync("MapOLUpdateLayer", _mapId, layer.InternalLayer) ?? ValueTask.CompletedTask;
    }

    /// <summary>
    /// Helper method to add a WMS Layer
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <param name="layers"></param>
    /// <param name="opacity"></param>
    /// <param name="styles"></param>
    /// <param name="transparent"></param>
    /// <param name="format"></param>
    /// <param name="lang"></param>
    /// <param name="wmsVersion"></param>
    /// <returns></returns>
    public ValueTask<Layer> AddTileWMSLayer(string url, string layers, double opacity = 1, string styles = "", bool transparent = true, string format = "image/png", string lang = "en", string wmsVersion = "1.3.0", double[] extent = null)
    {
        var layer = new Layer()
        {
            Opacity = 1,
            SourceType = SourceType.TileWMS,
            CrossOrigin = "anonymous",
            ServerType = "mapserver",
            //Url = $"{baseUrl}?SERVICE=WMS&VERSION={wmsVersion }&REQUEST=GetMap&FORMAT={HttpUtility.UrlEncode(format)}&TRANSPARENT={transparent}&LAYERS={HttpUtility.UrlEncode(layers)}&LANG={lang}&STYLE={HttpUtility.UrlEncode(styles)}"
            Url = url,
            Extent = extent,
            Params = new Dictionary<string, object> { { "LAYERS", layers }, { "FORMAT", format } },
        };

        LayersList.Add(layer);
        return ValueTask.FromResult(layer);
    }

    private void LayersOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_module == null)
            return;

        if (e.NewItems != null)
            foreach (var newLayer in e.NewItems.OfType<Layer>())
                newLayer.ParentMap = this;

        Task.Run(async () =>
        {
            if (e.OldItems != null)
                foreach (var oldLayer in e.OldItems.OfType<Layer>())
                    await _module.InvokeVoidAsync("MapOLRemoveLayer", _mapId, oldLayer.InternalLayer);

            if (e.NewItems != null)
                foreach (var newLayer in e.NewItems.OfType<Layer>())
                    await _module.InvokeVoidAsync("MapOLAddLayer", _mapId, newLayer.InternalLayer);
        });
    }

    private void ShapesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _ = SetShapes(ShapesList);
    }

    private void MarkersOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _ = SetMarkers(MarkersList);
    }
}