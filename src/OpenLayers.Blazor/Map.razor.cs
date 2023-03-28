using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace OpenLayers.Blazor;

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

    [Parameter] public ObservableCollection<Marker> Markers { get; set; } = new ();

    [Parameter] public ObservableCollection<Shape> Shapes { get; set; } = new();

    [Parameter] public EventCallback<Feature> OnFeatureClick { get; set; }

    [Parameter] public EventCallback<Marker> OnMarkerClick { get; set; }

    [Parameter] public EventCallback<Shape> OnShapeClick { get; set; }

    [Parameter] public EventCallback<Coordinate> OnClick { get; set; }

    [Parameter] public EventCallback<Coordinate> OnPointerMove { get; set; }

    [Parameter] public RenderFragment<Feature?>? Popup { get; set; }

    [Parameter] public List<TileLayer> Layers { get; set; } = new List<TileLayer>();

    public Defaults Defaults { get; } = new();

    [Parameter] public string Class { get; set; }

    [Parameter] public string Style { get; set; }

    private IJSObjectReference? Module { get; set; }

    private DotNetObjectReference<Map>? Instance { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (Module != null)
            await Module.DisposeAsync();
        Module = null;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Markers.CollectionChanged += (sender, args) => SetMarkers(Markers);
        Shapes.CollectionChanged += (sender, args) => SetShapes(Shapes);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            Module ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"/_content/{Assembly.GetExecutingAssembly().GetName().Name}/openlayers_interop.js");
            Instance ??= DotNetObjectReference.Create(this);

            if (Module != null)
                await Module.InvokeVoidAsync("MapOLInit", _mapId, _popupId, Defaults, Center, Zoom, Markers, Shapes, Layers, Instance);
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

    private ValueTask SetMarkers(ObservableCollection<Marker> markers) => Module?.InvokeVoidAsync("MapOLMarkers", _mapId, markers) ?? ValueTask.CompletedTask;

    private ValueTask SetShapes(ObservableCollection<Shape> shapes) => Module?.InvokeVoidAsync("MapOLSetShapes", _mapId, shapes) ?? ValueTask.CompletedTask;

    public async Task SetCenter(Coordinate center)
    {
        _center = center;
        if (Module != null) await Module.InvokeVoidAsync("MapOLCenter", _mapId, center);
    }

    public async Task SetZoom(double zoom)
    {
        if (_zoom != zoom)
        {
            _zoom = zoom;
            if (Module != null) await Module.InvokeVoidAsync("MapOLZoom", _mapId, zoom);
        }
    }

    public ValueTask SetZoomToExtent(Extent extent) => Module?.InvokeVoidAsync("MapOLZoomToExtent", _mapId, extent.ToString()) ?? ValueTask.CompletedTask;

    public ValueTask LoadGeoJson(object json) => Module?.InvokeVoidAsync("MapOLLoadGeoJson", _mapId, json) ?? ValueTask.CompletedTask;

    public ValueTask CenterToCurrentGeoLocation() => Module?.InvokeVoidAsync("MapOLCenterToCurrentGeoLocation", _mapId) ?? ValueTask.CompletedTask;

    public ValueTask<Coordinate> GetCurrentGeoLocation()
    {
        return Module?.InvokeAsync<Coordinate>("MapOLGetCurrentGeoLocation", _mapId) ?? ValueTask.FromResult<Coordinate>(null);
    }

}
