using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
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

    [Parameter] public Coordinate Center { get; set; } = new(0, 0);

    [Parameter] public EventCallback<Coordinate> CenterChanged { get; set; }

    [Parameter] public double Zoom { get; set; } = 2;

    [Parameter] public EventCallback<double> ZoomChanged { get; set; }

    public ObservableCollection<Marker> MarkersList { get; } = new();

    public ObservableCollection<Shape> ShapesList { get; } = new();

    [Parameter] public EventCallback<Feature> OnFeatureClick { get; set; }

    [Parameter] public EventCallback<Marker> OnMarkerClick { get; set; }

    [Parameter] public EventCallback<Internal.Shape> OnShapeClick { get; set; }

    [Parameter] public EventCallback<Coordinate> OnClick { get; set; }

    [Parameter] public EventCallback<Coordinate> OnPointerMove { get; set; }

    [Parameter] public RenderFragment<Feature?>? Popup { get; set; }

    [Parameter] public RenderFragment? Layers { get; set; }

    [Parameter] public RenderFragment? Features { get; set; }

    public ObservableCollection<Layer> LayersList { get; } = new();

    public Defaults Defaults { get; } = new();

    [Parameter] public string? Class { get; set; }

    [Parameter] public string? Style { get; set; }

    [Parameter]
    public string CoordinatesProjection
    {
        get => Defaults.CoordinatesProjection;
        set => Defaults.CoordinatesProjection = value;
    }

    private DotNetObjectReference<Map>? Instance { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (_module != null)
            await _module.DisposeAsync();
        _module = null;

        MarkersList.CollectionChanged -= MarkersOnCollectionChanged;
        ShapesList.CollectionChanged -= ShapesOnCollectionChanged;
        LayersList.CollectionChanged -= LayersOnCollectionChanged;
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

    public async Task SetZoom(double zoom)
    {
        if (_module != null) await _module.InvokeVoidAsync("MapOLZoom", _mapId, zoom);
        _internalZoom = Zoom;
    }

    public ValueTask SetZoomToExtent(Extent extent)
    {
        return _module?.InvokeVoidAsync("MapOLZoomToExtent", _mapId, extent.ToString()) ?? ValueTask.CompletedTask;
    }

    public ValueTask LoadGeoJson(object json)
    {
        return _module?.InvokeVoidAsync("MapOLLoadGeoJson", _mapId, json) ?? ValueTask.CompletedTask;
    }

    public ValueTask CenterToCurrentGeoLocation()
    {
        return _module?.InvokeVoidAsync("MapOLCenterToCurrentGeoLocation", _mapId) ?? ValueTask.CompletedTask;
    }

    public ValueTask<Coordinate?> GetCurrentGeoLocation()
    {
        return _module?.InvokeAsync<Coordinate?>("MapOLGetCurrentGeoLocation", _mapId) ?? ValueTask.FromResult<Coordinate?>(null);
    }

    public ValueTask SetMarkers(IEnumerable<Marker> markers)
    {
        return _module?.InvokeVoidAsync("MapOLMarkers", _mapId, markers.Select(p => p.InternalFeature).ToArray()) ?? ValueTask.CompletedTask;
    }

    public ValueTask SetShapes(IEnumerable<Shape> shapes)
    {
        return _module?.InvokeVoidAsync("MapOLSetShapes", _mapId, shapes.Select(p => p.InternalFeature).ToArray()) ?? ValueTask.CompletedTask;
    }

    public ValueTask SetLayers(IEnumerable<Layer> layers)
    {
        return _module?.InvokeVoidAsync("MapOLSetLayers", _mapId, layers.Select(p => p.InternalLayer).ToArray()) ?? ValueTask.CompletedTask;
    }

    private void LayersOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_module == null)
            return;

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