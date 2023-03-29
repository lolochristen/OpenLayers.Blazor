using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace OpenLayers.Blazor;

/// <summary>
/// Component to show OpenLayers Maps
/// </summary>
public partial class Map : IAsyncDisposable
{
    private Coordinate _center;
    private double _zoom = 5;
    private Feature? _popupContext;
    private string _mapId;
    private string _popupId;

    public Map()
    {
        _mapId = Guid.NewGuid().ToString();
        _popupId = Guid.NewGuid().ToString();
    }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Parameter]
    public Coordinate Center
    {
        get => _center;
        set => SetCenter(value);
    }

    [Parameter] public EventCallback<Coordinate> CenterChanged { get; set; }

    [Parameter]
    public double Zoom
    {
        get => _zoom;
        set => SetZoom(value);
    }

    [Parameter] public EventCallback<double> ZoomChanged { get; set; }

    [Parameter] public ObservableCollection<Marker> Markers { get; set; } = new();

    [Parameter] public ObservableCollection<Shape> Shapes { get; set; } = new();

    [Parameter] public EventCallback<Feature> OnFeatureClick { get; set; }

    [Parameter] public EventCallback<Marker> OnMarkerClick { get; set; }

    [Parameter] public EventCallback<Shape> OnShapeClick { get; set; }

    [Parameter] public EventCallback<Coordinate> OnClick { get; set; }

    [Parameter] public EventCallback<Coordinate> OnPointerMove { get; set; }

    [Parameter] public RenderFragment<Feature?>? Popup { get; set; }

    [Parameter] public ObservableCollection<TileLayer> Layers { get; set; } = new();

    public Defaults Defaults { get; } = new();

    [Parameter] public string Class { get; set; }

    [Parameter] public string Style { get; set; }

    public bool IsInitialized => _module != null;

    private IJSObjectReference? _module;

    private DotNetObjectReference<Map>? Instance { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (_module != null)
            await _module.DisposeAsync();
        _module = null;

        Markers.CollectionChanged -= MarkersOnCollectionChanged;
        Shapes.CollectionChanged -= ShapesOnCollectionChanged;
        Layers.CollectionChanged -= LayersOnCollectionChanged;
    }

    private INotifyCollectionChanged? _markersCollectionRef;
    private INotifyCollectionChanged? _shapeCollectionRef;
    private INotifyCollectionChanged? _layerCollectionRef;

    protected override async Task OnParametersSetAsync()
    {
        if (_markersCollectionRef != null)
        {
            _markersCollectionRef.CollectionChanged -= MarkersOnCollectionChanged;
            if (!ReferenceEquals(_markersCollectionRef, Markers))
            {
                await SetMarkers();
            }
        }
        Markers.CollectionChanged += MarkersOnCollectionChanged;
        _markersCollectionRef = Markers;

        if (_shapeCollectionRef != null)
        {
            _shapeCollectionRef.CollectionChanged -= ShapesOnCollectionChanged;
            if (!ReferenceEquals(_shapeCollectionRef, Shapes))
            {
                await SetShapes();
            }
        }
        Shapes.CollectionChanged += ShapesOnCollectionChanged;
        _shapeCollectionRef = Shapes;

        if (_layerCollectionRef != null)
        {
            _layerCollectionRef.CollectionChanged -= LayersOnCollectionChanged;
            if (!ReferenceEquals(_layerCollectionRef, Layers))
            {
                await SetLayers();
            }
        }
        Layers.CollectionChanged += LayersOnCollectionChanged;
        _layerCollectionRef = Layers;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _module ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"/_content/{Assembly.GetExecutingAssembly().GetName().Name}/openlayers_interop.js");
            Instance ??= DotNetObjectReference.Create(this);

            if (_module != null)
                await _module.InvokeVoidAsync("MapOLInit", _mapId, _popupId, Defaults, Center, Zoom, Markers, Shapes, Layers, Instance);
        }
    }

    [JSInvokable]
    public Task OnInternalFeatureClick(Feature feature) => OnFeatureClick.InvokeAsync(feature);

    [JSInvokable]
    public async Task OnInternalMarkerClick(Marker marker)
    {
        _popupContext = marker;
        await OnMarkerClick.InvokeAsync(marker);
        StateHasChanged();
    }

    [JSInvokable]
    public async Task OnInternalShapeClick(Shape shape)
    {
        _popupContext = shape;
        await OnShapeClick.InvokeAsync(shape);
        StateHasChanged();
    }

    [JSInvokable]
    public Task OnInternalClick(Coordinate coordinate) => OnClick.InvokeAsync(coordinate);

    [JSInvokable]
    public Task OnInternalPointerMove(Coordinate coordinate) => OnPointerMove.InvokeAsync(coordinate);

    [JSInvokable]
    public Task OnInternalZoomChanged(double zoom)
    {
        _zoom = zoom;
        return ZoomChanged.InvokeAsync(zoom);
    }

    [JSInvokable]
    public Task OnInternalCenterChanged(Coordinate coordinate)
    {
        _center = coordinate;
        return CenterChanged.InvokeAsync(_center);
    }

    public async Task SetCenter(Coordinate center)
    {
        _center = center;
        if (_module != null) await _module.InvokeVoidAsync("MapOLCenter", _mapId, center);
    }

    public async Task SetZoom(double zoom)
    {
        if (_zoom != zoom)
        {
            _zoom = zoom;
            if (_module != null) await _module.InvokeVoidAsync("MapOLZoom", _mapId, zoom);
        }
    }

    public ValueTask SetZoomToExtent(Extent extent) => _module?.InvokeVoidAsync("MapOLZoomToExtent", _mapId, extent.ToString()) ?? ValueTask.CompletedTask;

    public ValueTask LoadGeoJson(object json) => _module?.InvokeVoidAsync("MapOLLoadGeoJson", _mapId, json) ?? ValueTask.CompletedTask;

    public ValueTask CenterToCurrentGeoLocation() => _module?.InvokeVoidAsync("MapOLCenterToCurrentGeoLocation", _mapId) ?? ValueTask.CompletedTask;

    public ValueTask<Coordinate?> GetCurrentGeoLocation()
    {
        return _module?.InvokeAsync<Coordinate?>("MapOLGetCurrentGeoLocation", _mapId) ?? ValueTask.FromResult<Coordinate?>(null);
    }

    public ValueTask SetMarkers() => _module?.InvokeVoidAsync("MapOLMarkers", _mapId, Markers) ?? ValueTask.CompletedTask;

    public ValueTask SetShapes() => _module?.InvokeVoidAsync("MapOLSetShapes", _mapId, Shapes) ?? ValueTask.CompletedTask;

    public ValueTask SetLayers() => _module?.InvokeVoidAsync("MapOLSetLayers", _mapId, Layers) ?? ValueTask.CompletedTask;

    private void LayersOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_module == null)
            return;

        Task.Run(async () =>
        {
            if (e.OldItems != null)
            {
                foreach (var oldLayer in e.OldItems.OfType<TileLayer>())
                {
                    await _module.InvokeVoidAsync("MapOLRemoveLayer", _mapId, oldLayer);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var newLayer in e.NewItems.OfType<TileLayer>())
                {
                    await _module.InvokeVoidAsync("MapOLAddLayer", _mapId, newLayer);
                }
            }
        });
    }

    private void ShapesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _ = SetShapes();
    }

    private void MarkersOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _ = SetMarkers();
    }
}
