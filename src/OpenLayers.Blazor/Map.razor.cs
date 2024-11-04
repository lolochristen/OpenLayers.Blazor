using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OpenLayers.Blazor.Internal;

namespace OpenLayers.Blazor;

/// <summary>
///     Component to show OpenLayers Maps
/// </summary>
public partial class Map : IAsyncDisposable
{
    private static int _counter = 0;
    private string _mapId;
    private IJSObjectReference? _module;
    private Feature? _popupContext;
    private string _popupId;

    /// <summary>
    ///     Default Constructor
    /// </summary>
    public Map()
    {
        _counter++;
        _mapId = "map_" + _counter;
        _popupId = "map_popup_" + _counter;
    }

    [Inject] private IJSRuntime? JSRuntime { get; set; }

    /// <summary>
    ///     Gets or set then center of the map
    /// </summary>
    [Parameter]
    public Coordinate Center { get; set; } = new(0, 0);

    /// <summary>
    ///     Event when center changes
    /// </summary>
    [Parameter]
    public EventCallback<Coordinate> CenterChanged { get; set; }

    /// <summary>
    ///     Gets or set the rotation of the map in radians.
    ///     Negative value = counter-clockwise
    ///     Positive value = clockwise
    /// </summary>
    [Parameter]
    public double Rotation { get; set; } = 0;

    /// <summary>
    ///     Event when rotation changes
    /// </summary>
    [Parameter]
    public EventCallback<double> RotationChanged { get; set; }

    /// <summary>
    ///     Zoom level of the map
    /// </summary>
    [Parameter]
    public double Zoom { get; set; } = 5;

    /// <summary>
    ///     Event on zoom changes
    /// </summary>
    [Parameter]
    public EventCallback<double> ZoomChanged { get; set; }

    [Parameter] public EventCallback<Extent> VisibleExtentChanged { get; set; }

    /// <summary>
    ///     Collection of attached markers
    /// </summary>
    public ObservableCollection<Shape> MarkersList
    {
        get
        {
            if (MarkersLayer != null)
                return MarkersLayer.ShapesList;

            return GetOrCreateMarkersLayer().ShapesList;
        }
    }

    /// <summary>
    ///     Collection of attached shapes
    /// </summary>
    public ObservableCollection<Shape> ShapesList
    {
        get
        {
            if (ShapesLayer != null)
                return ShapesLayer.ShapesList;

            return GetOrCreateShapesLayer().ShapesList;
        }
    }

    /// <summary>
    ///     Event when a feature (shapes/markers) is called
    /// </summary>
    [Parameter]
    public EventCallback<Feature> OnFeatureClick { get; set; }

    /// <summary>
    ///     Event when a marker get clicked
    /// </summary>
    [Parameter]
    public EventCallback<Marker> OnMarkerClick { get; set; }

    /// <summary>
    ///     Event when a shape gets clicked
    /// </summary>
    [Parameter]
    public EventCallback<Shape> OnShapeClick { get; set; }

    /// <summary>
    ///     Event when a point in the map gets clicked. Event returns current coordinates
    /// </summary>
    [Parameter]
    public EventCallback<Coordinate> OnClick { get; set; }

    /// <summary>
    ///     Event when a point in the map gets double clicked. Event returns current coordinates
    /// </summary>
    [Parameter]
    public EventCallback<Coordinate> OnDoubleClick { get; set; }

    /// <summary>
    ///     Event when the pointer gets moved
    /// </summary>
    [Parameter]
    public EventCallback<Coordinate> OnPointerMove { get; set; }

    /// <summary>
    ///     Event when the pointer hovers over a shape. Null when leaving a shape.
    /// </summary>
    [Parameter]
    public EventCallback<Shape?> OnShapeHover { get; set; }

    /// <summary>
    ///     Event when the rendering is complete
    /// </summary>
    [Parameter]
    public EventCallback OnRenderComplete { get; set; }

    /// <summary>
    ///     Content to show as a popup when a shape or marker gets clicked and <see cref="Shape.Popup" /> is set to true
    /// </summary>
    [Parameter]
    public RenderFragment<Feature?>? Popup { get; set; }

    /// <summary>
    ///     Definition of Layers to show in the map. Only items of <see cref="Layer" /> are considered.
    /// </summary>
    /// <example>
    ///     <Layers>
    ///         <Layer SourceType="SourceType.TileWMS"
    ///             Url="https://sedac.ciesin.columbia.edu/geoserver/ows?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&FORMAT=image%2Fpng&TRANSPARENT=true&LAYERS=gpw-v3%3Agpw-v3-population-density_2000&LANG=en"
    ///             Opacity=".3"
    ///             CrossOrigin="anonymous" />
    ///     </Layers>
    /// </example>
    [Parameter]
    public RenderFragment? Layers { get; set; }

    /// <summary>
    ///     Definition of Features to show on the map. Only items of the type <see cref="Marker" /> or <see cref="Shape" /> (
    ///     <see cref="Line" />, <see cref="Circle" />) are considered.
    /// </summary>
    /// <example>
    ///     <Features>
    ///         <Marker Type="MarkerType.MarkerPin" Coordinate="new Coordinate(1197650, 2604200)"></Marker>
    ///         <Line Points="new []{new Coordinate(1197650, 2604200), new Coordinate(1177650, 2624200)}" Stroke="cyan"></Line>
    ///     </Features>
    /// </example>
    [Parameter]
    public RenderFragment? Features { get; set; }

    /// <summary>
    ///     Collection of all Layers
    /// </summary>
    public ObservableCollection<Layer> LayersList { get; } = new();

    /// <summary>
    ///     Options to use for the map rendering
    /// </summary>
    public Options Options { get; } = new();

    /// <summary>
    ///     Class of the map element
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    ///     Styles of the map element
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    ///     Coordinates projection to use of the layers and events. Default is set to "EPSG:4326" (https://epsg.io/4326).
    ///     Additionally to the default OpenLayers projections, the swiss projections EPSG:2056 (VT95) and EPSG:21781 (VT03)
    ///     are supported.
    /// </summary>
    [Parameter]
    public string CoordinatesProjection
    {
        get => Options.CoordinatesProjection;
        set => Options.CoordinatesProjection = value;
    }

    [Parameter]
    public string ViewProjection
    {
        get => Options.ViewProjection;
        set => Options.ViewProjection = value;
    }

    /// <summary>
    ///     Unit of the ScaleLine
    /// </summary>
    [Parameter]
    public ScaleLineUnit ScaleLineUnit
    {
        get => Options.ScaleLineUnit;
        set => Options.ScaleLineUnit = value;
    }

    /// <summary>
    ///     Auto PopUp
    /// </summary>
    [Parameter]
    public bool AutoPopup
    {
        get => Options.AutoPopup;
        set => Options.AutoPopup = value;
    }

    /// <summary>
    ///     Sets or gets the visible extent of the map
    /// </summary>
    [Parameter]
    public Extent? VisibleExtent { get; set; }

    /// <summary>
    ///     A shape providing default parameters when drawing new shapes
    /// </summary>
    [Parameter]
    public ShapeType NewShapeType { get; set; }

    /// <summary>
    ///     Get or set if new shapes shall be drawn
    /// </summary>
    [Parameter]
    public bool EnableNewShapes { get; set; }

    /// <summary>
    ///     Get or sets if the position of points shall be snapped.
    /// </summary>
    [Parameter]
    public bool EnableShapeSnap { get; set; }

    /// <summary>
    ///     Get or sets if drawing new shapes is enabled.
    /// </summary>
    [Parameter]
    public bool EnableEditShapes { get; set; }

    /// <summary>
    ///     Get or sets if drawing shapes is in freehand mode.
    /// </summary>
    [Parameter]
    public bool Freehand { get; set; }

    private DotNetObjectReference<Map>? Instance { get; set; }

    /// <summary>
    ///     Get or sets an event callback when a feature is added to the map.
    /// </summary>
    [Parameter]
    public EventCallback<Shape> OnShapeAdded { get; set; }

    /// <summary>
    ///     Get or sets an event callback when a feature is changed on the map.
    /// </summary>
    [Parameter]
    public EventCallback<Shape> OnShapeChanged { get; set; }

    /// <summary>
    ///     Get or sets an event callback when a feature is removed from the map.
    /// </summary>
    [Parameter]
    public EventCallback<Shape> OnShapeRemoved { get; set; }

    /// <summary>
    ///     Gets or sets and event callback when a layer is added to the map.
    /// </summary>
    [Parameter]
    public EventCallback<Layer> OnLayerAdded { get; set; }

    /// <summary>
    ///     Gets or sets and event callback when a layer is removed from the map.
    /// </summary>
    [Parameter]
    public EventCallback<Layer> OnLayerRemoved { get; set; }

    /// <summary>
    ///     Get or sets a callback the return a <see cref="StyleOptions" /> of style for the given shape. This callback can be
    ///     used to provide custom styles per shapes.
    /// </summary>
    [Parameter]
    public Func<Shape, StyleOptions> ShapeStyleCallback { get; set; } = DefaultShapeStyleCallback;

    /// <summary>
    ///     Set or gets if any interaction is active.
    /// </summary>
    [Parameter]
    public bool InteractionsEnabled { get; set; }

    /// <summary>
    ///     Get or sets if the zoom control is visible
    /// </summary>
    [Parameter]
    public bool ZoomControl
    {
        get => Options.ZoomControl;
        set => Options.ZoomControl = value;
    }

    /// <summary>
    ///     Gets or sets if the attribution control is visible
    /// </summary>
    [Parameter]
    public bool AttributionControl
    {
        get => Options.AttributionControl;
        set => Options.AttributionControl = value;
    }

    /// <summary>
    ///     Gets or sets if full screen control is visible
    /// </summary>
    [Parameter]
    public bool FullScreenControl
    {
        get => Options.FullScreenControl;
        set => Options.FullScreenControl = value;
    }

    /// <summary>
    ///     Gets or sets boolean if zoom slider control is visible
    /// </summary>
    [Parameter]
    public bool ZoomSliderControl
    {
        get => Options.ZoomSliderControl;
        set => Options.ZoomSliderControl = value;
    }

    /// <summary>
    ///     Gets or sets boolean if rotate to 0 control is visible
    /// </summary>
    [Parameter]
    public bool RotateControl
    {
        get => Options.RotateControl;
        set => Options.RotateControl = value;
    }

    /// <summary>
    ///     Gets or sets boolean if a overview map using first layer is visible
    /// </summary>
    [Parameter]
    public bool OverviewMap
    {
        get => Options.OverviewMap;
        set => Options.OverviewMap = value;
    }

    /// <summary>
    ///     Gets or sets boolean if zoom to extent control is visible
    /// </summary>
    [Parameter]
    public bool ZoomToExtentControl
    {
        get => Options.ZoomToExtentControl;
        set => Options.ZoomToExtentControl = value;
    }

    /// <summary>
    ///     Gets or sets the default or initial center of the map.
    /// </summary>
    [Parameter]
    public Coordinate InitialCenter { get; set; } = Coordinate.Empty;

    /// <summary>
    ///     Gets or sets the default or initial center of the map.
    /// </summary>
    [Parameter]
    public double InitialZoom { get; set; } = 0;

    /// <summary>
    ///    Gets or sets the minimal zoom level.
    /// </summary>
    [Parameter]
    public double MinZoom
    {
        get => Options.MinZoom;
        set => Options.MinZoom = value;
    }

    /// <summary>
    ///    Gets or sets the maximal zoom level.
    /// </summary>
    [Parameter]
    public double MaxZoom
    {
        get => Options.MaxZoom;
        set => Options.MaxZoom = value;
    }

    /// <summary>
    ///     Gets or sets a javascript which gets call after initialization of the map to do customizations. First argument is the map object.
    /// </summary>
    /// <example>
    ///     _map.ConfigureJsMethod = "myComponent.configureMap";
    /// 
    ///     <script>
    ///         window.myComponent = {
    ///         configureMap: (map) => {
    ///                 map.getInteractions().forEach(function (interaction) {
    ///                     if (interaction instanceof ol.interaction.DoubleClickZoom) { interaction.setActive(false); }
    ///                 });
    ///         }}
    ///     </script>
    /// </example>
    [Parameter]
    public string? ConfigureJsMethod { get; set; }

    /// <summary>
    ///     Gets or set the default layer for shapes.
    /// </summary>
    public Layer? ShapesLayer { get; set; }

    /// <summary>
    ///     Gets or set the default layer for markers.
    /// </summary>
    public Layer? MarkersLayer { get; set; }

    /// <summary>
    ///     Returns a IEnumerable of all features assigned to map.
    /// </summary>
    public IEnumerable<Feature> FeaturesList => LayersList.SelectMany(p => p.ShapesList);

    /// <summary>
    ///     Disposing resources.
    /// </summary>
    /// <returns>ValueTask</returns>
    public async ValueTask DisposeAsync()
    {
        LayersList.CollectionChanged -= LayersOnCollectionChanged;

        if (_module != null)
        {
            await _module.InvokeVoidAsync("MapOLDispose", _mapId);
            await _module.DisposeAsync();
            _module = null;
        }
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(Zoom), out double zoom) && zoom != Zoom)
            _ = SetZoom(zoom);

        if (parameters.TryGetValue(nameof(Center), out Coordinate center) && !center.Equals(Center))
            _ = SetCenter(center);

        if (parameters.TryGetValue(nameof(Rotation), out double rotation) && !rotation.Equals(Rotation))
            _ = SetRotation(rotation);

        if (parameters.TryGetValue(nameof(VisibleExtent), out Extent extent) && !extent.Equals(VisibleExtent))
            _ = SetVisibleExtent(extent);

        if (parameters.TryGetValue(nameof(InteractionsEnabled), out bool interactionsEnabled) && interactionsEnabled != InteractionsEnabled)
            _ = SetInteractions(interactionsEnabled);

        short drawingChanges = 0;
        if (parameters.TryGetValue(nameof(EnableNewShapes), out bool newShapes) && newShapes != EnableNewShapes)
            drawingChanges++;
        else
            newShapes = EnableNewShapes;

        if (parameters.TryGetValue(nameof(EnableEditShapes), out bool editShapes) && editShapes != EnableEditShapes)
            drawingChanges++;
        else
            editShapes = EnableEditShapes;

        if (parameters.TryGetValue(nameof(EnableShapeSnap), out bool shapeSnap) && shapeSnap != EnableShapeSnap)
            drawingChanges++;
        else
            shapeSnap = EnableShapeSnap;

        if (parameters.TryGetValue(nameof(NewShapeType), out ShapeType shapeType) && shapeType != NewShapeType)
            drawingChanges++;
        else
            shapeType = NewShapeType;

        if (parameters.TryGetValue(nameof(Freehand), out bool freehand) && freehand != Freehand)
            drawingChanges++;
        else
            freehand = Freehand;

        if (drawingChanges > 0)
            _ = SetDrawingSettings(newShapes, editShapes, shapeSnap, shapeType, freehand);

        return base.SetParametersAsync(parameters);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
#if DEBUG
            var script = $"openlayers_interop.js";
#else
            var script = $"openlayers_interop.min.js?v={Assembly.GetExecutingAssembly().GetName().Version}";
#endif
            _module ??= await JSRuntime!.InvokeAsync<IJSObjectReference>("import", $"./_content/{Assembly.GetExecutingAssembly().GetName().Name}/{script}");
            Instance ??= DotNetObjectReference.Create(this);

            if (ShapesLayer != null && ShapesList.Count > 0)
                GetOrCreateShapesLayer();

            if (MarkersLayer != null && MarkersList.Count > 0)
                GetOrCreateMarkersLayer();

            if (InitialCenter != Coordinate.Empty)
                Center = InitialCenter;

            if (InitialZoom != 0)
                Zoom = InitialZoom;

            if (_module != null)
                await _module.InvokeVoidAsync("MapOLInit", _mapId, _popupId, Options, Center, Zoom, Rotation, InteractionsEnabled,
                    LayersList.Select(p => p.InternalLayer).ToArray(),
                    Instance, ConfigureJsMethod);

            foreach (var layer in LayersList)
            {
                await OnLayerAdded.InvokeAsync(layer);

                if (layer.ShapesList.Count > 0)
                    await SetShapesInternal(layer, layer.ShapesList);
                if (layer.SelectionEnabled)
                    await SetSelectionSettings(layer, layer.SelectionEnabled, layer.SelectionStyle, layer.MultiSelect);
            }

            LayersList.CollectionChanged += LayersOnCollectionChanged;

            if (EnableEditShapes || EnableNewShapes)
                await SetDrawingSettings(EnableNewShapes, EnableEditShapes, EnableShapeSnap, NewShapeType, Freehand);
        }
    }

    protected override void OnInitialized()
    {
        EnableShapeSnap = true;
        InteractionsEnabled = true;
    }

    [JSInvokable]
    public async Task OnInternalShapeClick(Internal.Shape shape, string layerId)
    {
#if DEBUG
        Console.WriteLine($"OnInternalShapeClick: {JsonSerializer.Serialize(shape)}");
#endif
        var layer = LayersList.FirstOrDefault(p => p.Id == layerId);
        if (layer != null)
        {
            var existingShape = layer.ShapesList.FirstOrDefault(p => p.Id == shape.Id);

            if (existingShape != null)
            {
                _popupContext = existingShape;

                await OnFeatureClick.InvokeAsync(existingShape);

                if (existingShape is Marker marker)
                    await OnMarkerClick.InvokeAsync(marker);
                else
                    await OnShapeClick.InvokeAsync(existingShape);
                await existingShape.OnClick.InvokeAsync();
                StateHasChanged();
                return;
            }
        }

        // unknown layer or shape
        await OnFeatureClick.InvokeAsync(new Feature(shape));

        if (shape.Type == nameof(Marker))
            await OnMarkerClick.InvokeAsync(new Marker(shape));
        else
            await OnShapeClick.InvokeAsync(new Shape(shape));
    }

    [JSInvokable]
    public async Task OnInternalFeatureClick(Internal.Feature feature, string layerId)
    {
#if DEBUG
        Console.WriteLine($"OnInternalFeatureClick: {JsonSerializer.Serialize(feature)}");
#endif
        var f = new Feature(feature);
        if (AutoPopup)
            _popupContext = f;
        await OnFeatureClick.InvokeAsync(f);
    }

    [JSInvokable]
    public Task OnInternalClick(Coordinate coordinate)
    {
#if DEBUG
        Console.WriteLine($"OnInternalClick: {coordinate}");
#endif
        return OnClick.InvokeAsync(coordinate);
    }

    [JSInvokable]
    public Task OnInternalDoubleClick(Coordinate coordinate)
    {
#if DEBUG
        Console.WriteLine($"OnInternalDoubleClick: {coordinate}");
#endif
        return OnDoubleClick.InvokeAsync(coordinate);
    }

    [JSInvokable]
    public Task OnInternalPointerMove(Coordinate coordinate)
    {
        return OnPointerMove.InvokeAsync(coordinate);
    }

    [JSInvokable]
    public Task OnInternalShapeHover(string? layerId, string? shapeId)
    {
#if DEBUG
        Console.WriteLine($"OnInternalShapeHover {layerId} {shapeId}");
#endif
        var layer = LayersList.FirstOrDefault(p => p.Id == layerId);
        if (layer != null && layer.ShapesList.FirstOrDefault(p => p.Id == shapeId) is Shape shape)
            return OnShapeHover.InvokeAsync(shape);
        return OnShapeHover.InvokeAsync(null);
    }

    [JSInvokable]
    public async Task OnInternalCenterChanged(Coordinate coordinate)
    {
        if (!coordinate.Equals(Center))
        {
            Center = coordinate;
            await CenterChanged.InvokeAsync(coordinate);
        }
    }

    [JSInvokable]
    public async Task OnInternalRotationChanged(double rotation)
    {
        Rotation = rotation;
        await RotationChanged.InvokeAsync(rotation);
    }

    [JSInvokable]
    public async Task OnInternalVisibleExtentChanged(Extent visibleExtent)
    {
        if (!visibleExtent.Equals(VisibleExtent))
        {
            VisibleExtent = visibleExtent;
            await VisibleExtentChanged.InvokeAsync(visibleExtent);
        }
    }

    [JSInvokable]
    public async Task OnInternalShapeAdded(string layerId, Internal.Shape shape)
    {
#if DEBUG
        Console.WriteLine($"OnInternalShapeAdded: {JsonSerializer.Serialize(shape)}");
#endif
        var layer = LayersList.FirstOrDefault(p => p.Id == layerId);
        if (layer == null)
        {
#if DEBUG
            Console.WriteLine($"Layer not found: {layerId}");
#endif
            return;
        }

        if (layer.ShapesList.All(p => p.Id != shape.Id))
        {
            var newShape = await layer.OnInternalShapeAdded(shape);
            await OnShapeAdded.InvokeAsync(newShape);
        }
    }

    [JSInvokable]
    public async Task OnInternalShapeChanged(string layerId, Internal.Shape shape)
    {
#if DEBUG
        Console.WriteLine($"OnInternalShapeChanged: {JsonSerializer.Serialize(shape)}");
#endif

        var layer = LayersList.FirstOrDefault(p => p.Id == layerId);
        if (layer == null)
        {
#if DEBUG
            Console.WriteLine($"Layer not found: {layerId}");
#endif
            return;
        }

        var existingShape = layer.ShapesList.FirstOrDefault(p => p.Id == shape.Id);

        if (existingShape == null)
        {
            await OnInternalShapeAdded(layerId, shape);
            return;
        }

        if (!existingShape.InternalFeature.Equals(shape))
        {
            existingShape.InternalFeature = shape;
            await OnShapeChanged.InvokeAsync(existingShape);
            await layer.OnInternalShapeChanged(existingShape);
        }
    }

    [JSInvokable]
    public async Task OnInternalShapeRemoved(string layerId, Internal.Shape shape)
    {
#if DEBUG
        Console.WriteLine($"OnInternalShapeRemoved: {JsonSerializer.Serialize(shape)}");
#endif

        var layer = LayersList.FirstOrDefault(p => p.Id == layerId);
        if (layer == null)
        {
#if DEBUG
            Console.WriteLine($"Layer not found: {layerId}");
#endif
            return;
        }

        var existingShape = layer.ShapesList.FirstOrDefault(p => p.Id == shape.Id);

        if (existingShape != null)
        {
            await layer.OnInternalShapeRemoved(existingShape);
            await OnShapeRemoved.InvokeAsync(existingShape);
        }
    }

    [JSInvokable]
    public async Task OnInternalSelectionChanged(string layerId, Internal.Shape[] selected, Internal.Shape[] unselected)
    {
#if DEBUG
        Console.WriteLine($"OnInternalSelectionChanged");
#endif

        var layer = LayersList.FirstOrDefault(p => p.Id == layerId);
        if (layer == null)
        {
#if DEBUG
            Console.WriteLine($"Layer not found: {layerId}");
#endif
            return;
        }

        var selectedShapes = selected.Select(p => layer.ShapesList.FirstOrDefault(s => s.Id == p.Id))
            .Where(p => p != null).ToList();

        var unselectedShapes = unselected.Select(p => layer.ShapesList.FirstOrDefault(s => s.Id == p.Id))
            .Where(p => p != null).ToList();

        if (selectedShapes.Count > 0)
            layer.SelectedShape = selectedShapes[0];
        else
            layer.SelectedShape = null;

        await layer.SelectedShapeChanged.InvokeAsync(layer.SelectedShape);

        await layer.SelectionChanged.InvokeAsync(new SelectionChangedArgs() { SelectedShapes = selectedShapes, UnselectedShapes = unselectedShapes });
    }

    [JSInvokable]
    public async Task OnInternalRenderComplete()
    {
        await OnRenderComplete.InvokeAsync();
    }

    /// <summary>
    ///     Passes the center coordination to underlying map
    /// </summary>
    /// <param name="center">Center Coordinates</param>
    /// <returns>Task</returns>
    public async Task SetCenter(Coordinate center)
    {
        if (_module != null) await _module.InvokeVoidAsync("MapOLCenter", _mapId, center);
    }

    /// <summary>
    ///     Sets the rotation of the map.
    /// </summary>
    /// <param name="rotation">Rotation in radians</param>
    /// <returns>Task</returns>
    public async Task SetRotation(double rotation)
    {
        if (_module != null) await _module.InvokeVoidAsync("MapOLRotate", _mapId, rotation);
    }

    [JSInvokable]
    public async Task OnInternalZoomChanged(double zoom)
    {
        Zoom = zoom;
        await ZoomChanged.InvokeAsync(zoom);
    }

    /// <summary>
    ///     Sets the zoom level to underlying map component
    /// </summary>
    /// <param name="zoom">zoom level</param>
    /// <returns></returns>
    public async Task SetZoom(double zoom)
    {
        if (_module != null) await _module.InvokeVoidAsync("MapOLZoom", _mapId, zoom);
    }

    /// <summary>
    ///     Zooms to the given extent
    /// </summary>
    /// <param name="extent"></param>
    /// <param name="padding">
    ///     Padding (in pixels) to be cleared inside the view. Values in the array are top, right, bottom and
    ///     left padding. defaults to [0,0,0,0]
    /// </param>
    /// <returns></returns>
    public ValueTask SetZoomToExtent(ExtentType extent, double[]? padding = null)
    {
        switch (extent)
        {
            case ExtentType.Markers:
                if (MarkersLayer != null) return SetZoomToExtent(MarkersLayer, padding);
                break;
            case ExtentType.Geometries:
                if (ShapesLayer != null) return SetZoomToExtent(ShapesLayer, padding);
                break;
        }
        return SetZoomToExtent(LayersList.First(), padding);
    }

    /// <summary>
    ///     Zooms to the given extent of the layer
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="padding">
    ///     Padding (in pixels) to be cleared inside the view. Values in the array are top, right, bottom and
    ///     left padding. defaults to [0,0,0,0]
    /// </param>
    /// <returns></returns>
    public ValueTask SetZoomToExtent(Layer layer, double[]? padding = null)
    {
        return _module?.InvokeVoidAsync("MapOLZoomToExtent", _mapId, layer.Id, padding) ?? ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Loads GeoJson data (https://geojson.org/) to the map
    /// </summary>
    /// <param name="json">GeoJson Data</param>
    /// <param name="dataProjection">Data projection of GeoJson</param>
    /// <param name="raiseEvents">Raise events for new created features and add it to list of shapes</param>
    /// <param name="styles">Flat styles collection</param>
    [Obsolete("Use map.AddLayer or LayerList.Add(new Layer() { LayerType=LayerType.Vector, SourceType=SourceType.VectorGeoJson, Data=data } instead.")]
    public async ValueTask<Layer> LoadGeoJson(JsonElement json, string? dataProjection = null, bool raiseEvents = true, Dictionary<string, object>? styles = null)
    {
        var layer = new Layer
        {
            LayerType = LayerType.Vector,
            SourceType = SourceType.VectorGeoJson,
            RaiseShapeEvents = raiseEvents,
            Projection = dataProjection,
            Data = json,
            FlatStyle = styles
        };
        await AddLayer(layer);
        return layer;
    }

    /// <summary>
    ///     Centers the map to the current GPS geo location
    /// </summary>
    public ValueTask CenterToCurrentGeoLocation()
    {
        return _module?.InvokeVoidAsync("MapOLCenterToCurrentGeoLocation", _mapId) ?? ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Current available GPS geo location
    /// </summary>
    /// <returns>Current coordinates in the current map projection</returns>
    public ValueTask<Coordinate?> GetCurrentGeoLocation()
    {
        return _module?.InvokeAsync<Coordinate?>("MapOLGetCurrentGeoLocation", _mapId) ?? ValueTask.FromResult<Coordinate?>(null);
    }

    /// <summary>
    ///     Set all layers to underlying map component
    /// </summary>
    /// <param name="layers">collection of layers</param>
    public ValueTask SetLayers(IEnumerable<Layer> layers)
    {
        return _module?.InvokeVoidAsync("MapOLSetLayers", _mapId, layers.Select(p => p.InternalLayer).ToArray()) ?? ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Updates a single layer
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public ValueTask UpdateLayer(Layer layer)
    {
#if DEBUG
        Console.WriteLine($"UpdateLayer {JsonSerializer.Serialize(layer.InternalLayer)}");
#endif
        return _module?.InvokeVoidAsync("MapOLUpdateLayer", _mapId, layer.InternalLayer) ?? ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Set visible extent of map view
    /// </summary>
    /// <param name="extent">Extent</param>
    /// <returns></returns>
    public async Task SetVisibleExtent(Extent extent)
    {
        if (_module != null) await _module.InvokeVoidAsync("MapOLSetVisibleExtent", _mapId, extent);
    }

    /// <summary>
    ///     Set interactions settings
    /// </summary>
    /// <param name="active"></param>
    /// <returns></returns>
    public async Task SetInteractions(bool active)
    {
        if (_module != null) await _module.InvokeVoidAsync("MapOLSetInteractions", _mapId, active);
    }

    [JSInvokable]
    public StyleOptions OnGetShapeStyle(Internal.Shape shape, string layer_id)
    {
#if DEBUG
        Console.WriteLine($"OnGetShapeStyle: {JsonSerializer.Serialize(shape)}");
#endif

        var style = LayersList.FirstOrDefault(p => p.Id == layer_id)?.StyleCallback?.Invoke(new Shape(shape));
        if (style == null)
            style = ShapeStyleCallback(new Shape(shape));
        return style;
    }

    [JSInvokable]
    public Task<StyleOptions> OnGetShapeStyleAsync(Internal.Shape shape, string layer_id)
    {
        return Task.FromResult(OnGetShapeStyle(shape, layer_id));
    }

    /// <summary>
    ///     Sets explicitly drawing settings
    /// </summary>
    /// <param name="newShapes"></param>
    /// <param name="editShapes"></param>
    /// <param name="shapeSnap"></param>
    /// <param name="shapeType"></param>
    /// <param name="freehand"></param>
    /// <returns></returns>
    public async Task SetDrawingSettings(bool newShapes, bool editShapes, bool shapeSnap, ShapeType shapeType, bool freehand)
    {
        try
        {
            if (ShapesLayer == null)
                GetOrCreateShapesLayer();

            if (_module != null)
                await _module.InvokeVoidAsync("MapOLSetDrawingSettings", _mapId, ShapesLayer.Id, newShapes, editShapes, shapeSnap, shapeType, freehand);
        }
        catch (Exception exp)
        {
            Console.Error.WriteLine("Failed to set drawing settings:\n" + exp);
        }
    }

    /// <summary>
    ///     Undo last drawing interaction
    /// </summary>
    /// <returns></returns>
    public async Task Undo()
    {
        if (_module != null) await _module.InvokeVoidAsync("MapOLUndoDrawing", _mapId);
    }

    /// <summary>
    ///     Explicit call to update an existing shape
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    public ValueTask UpdateShape(Shape shape)
    {
#if DEBUG
        Console.WriteLine($"UpdateShape: {JsonSerializer.Serialize(shape.InternalFeature)}");
#endif
        if (shape.Layer == null)
            throw new InvalidOperationException("Shape must be assigned to a layer");

        return _module?.InvokeVoidAsync("MapOLUpdateShape", _mapId, shape.Layer.Id, shape.InternalFeature) ?? ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Sets coordinates of an existing shape
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    public ValueTask SetCoordinates(Shape shape, Coordinates coordinates)
    {
#if DEBUG
        Console.WriteLine($"SetCoordinates: {shape.Id} {coordinates}");
#endif
        if (shape.Layer == null)
            throw new InvalidOperationException("Shape must be assigned to a layer");

        return _module?.InvokeVoidAsync("MapOLSetCoordinates", _mapId, shape.Layer.Id, shape.Id, coordinates) ?? ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Default Style Callback
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    public static StyleOptions DefaultShapeStyleCallback(Shape shape)
    {
        return new StyleOptions
        {
            Stroke = new StyleOptions.StrokeOptions
            {
                Color = Options.DefaultStroke,
                Width = 3,
                LineDash = new double[] { 4 }
            },
            Fill = new StyleOptions.FillOptions
            {
                Color = Options.DefaultFill
            },
            Circle = shape.ShapeType == ShapeType.Point ? new StyleOptions.CircleStyleOptions()
            {
                Radius = 6,
                Fill = new StyleOptions.FillOptions
                {
                    Color = Blazor.Options.DefaultFill
                },
                Stroke = new StyleOptions.StrokeOptions
                {
                    Color = Blazor.Options.DefaultStroke,
                    Width = 2
                }
            } : null
            
        };
    }

    /// <summary>
    ///     Reads and updates the coordinates of a shape.
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task ReadCoordinates(Feature feature)
    {
        if (_module == null)
            return;

        if (feature.Layer == null)
            throw new InvalidOperationException("Feature must be assigned to a layer");

        var c = await _module.InvokeAsync<dynamic>("MapOLGetCoordinates", _mapId, feature.Layer.Id, feature.Id);

        if (c is JsonElement)
            feature.InternalFeature.Coordinates = CoordinatesHelper.DeserializeCoordinates((JsonElement)c);
        else
            feature.InternalFeature.Coordinates = c;
    }

    private void LayersOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_module == null)
            return;

        if (e.NewItems != null)
            foreach (Layer newLayer in e.NewItems)
                newLayer.Initialize(this);

        Task.Run(async () =>
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                await _module.InvokeVoidAsync("MapOLSetLayers", _mapId, null);
            }
            else
            {
                if (e.OldItems != null)
                    foreach (Layer oldLayer in e.OldItems)
                        await _module.InvokeVoidAsync("MapOLRemoveLayer", _mapId, oldLayer.Id);

                if (e.NewItems != null)
                    foreach (Layer newLayer in e.NewItems)
                        await _module.InvokeVoidAsync("MapOLAddLayer", _mapId, newLayer.InternalLayer);
            }
        });
    }

    internal ValueTask SetShapesInternal(Layer layer, IEnumerable<Shape>? shapes)
    {
        return _module?.InvokeVoidAsync("MapOLSetShapes", _mapId, layer.Id, shapes?.Select(p => p.InternalFeature).ToArray()) ?? ValueTask.CompletedTask;
    }

    internal ValueTask RemoveShapeInternal(Layer layer, Shape shape)
    {
        return _module?.InvokeVoidAsync("MapOLRemoveShape", _mapId, layer.Id, shape.InternalFeature) ?? ValueTask.CompletedTask;
    }

    internal ValueTask AddShapeInternal(Layer layer, Shape shape)
    {
        return _module?.InvokeVoidAsync("MapOLAddShape", _mapId, layer.Id, shape.InternalFeature) ?? ValueTask.CompletedTask;
    }

    internal Layer GetOrCreateShapesLayer()
    {
        if (ShapesLayer != null)
            return ShapesLayer;

        var layer = LayersList.FirstOrDefault(p => p.Id == "shapes");
        if (layer == null)
        {
            layer = new Layer
            {
                LayerType = LayerType.Vector,
                SourceType = SourceType.Vector,
                RaiseShapeEvents = true,
                ZIndex = 998,
                Id = "shapes"
            };
            LayersList.Add(layer);
            if (layer.Map == null)
                layer.Initialize(this);
            ShapesLayer = layer;
        }

        return layer;
    }

    internal Layer GetOrCreateMarkersLayer()
    {
        if (MarkersLayer != null)
            return MarkersLayer;

        var layer = LayersList.FirstOrDefault(p => p.Id == "markers");
        if (layer == null)
        {
            layer = new Layer
            {
                LayerType = LayerType.Vector,
                SourceType = SourceType.Vector,
                RaiseShapeEvents = true,
                ZIndex = 999,
                Id = "markers"
            };
            LayersList.Add(layer);
            if (layer.Map == null)
                layer.Initialize(this);
            MarkersLayer = layer;
        }

        return layer;
    }

    /// <summary>
    ///     Explicitly adds a layer to the map.
    /// </summary>
    /// <param name="layer">Layer to add</param>
    /// <returns></returns>
    public async Task AddLayer(Layer layer)
    {
        if (_module == null)
            return;

#if DEBUG
        Console.WriteLine($"AddLayer {layer.Id} {layer.LayerType} {layer.SourceType} {layer.Url}");
#endif

        try
        {
            LayersList.CollectionChanged -= LayersOnCollectionChanged;
            await _module.InvokeVoidAsync("MapOLAddLayer", _mapId, layer.InternalLayer);
            if (layer.Map == null)
                layer.Initialize(this);
            LayersList.Add(layer);
        }
        finally
        {
            LayersList.CollectionChanged += LayersOnCollectionChanged;
        }

        await OnLayerAdded.InvokeAsync(layer);
    }

    /// <summary>
    ///     Explicitly removes a layer from the map
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public async Task RemoveLayer(Layer layer)
    {
        if (_module == null)
            return;

        try
        {
            LayersList.CollectionChanged -= LayersOnCollectionChanged;
            await _module.InvokeVoidAsync("MapOLRemoveLayer", _mapId, layer.Id);
            layer.Map = null;
            LayersList.Remove(layer);
        }
        finally
        {
            LayersList.CollectionChanged += LayersOnCollectionChanged;
        }

        await OnLayerRemoved.InvokeAsync(layer);
    }

    /// <summary>
    /// Sets the selection settings for a layer
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="selectionEnabled"></param>
    /// <param name="selectionStyle"></param>
    /// <returns></returns>
    public async Task SetSelectionSettings(Layer? layer, bool selectionEnabled, StyleOptions? selectionStyle, bool multiSelect)
    {
        if (_module == null)
            return;
        
        await _module.InvokeVoidAsync("MapOLSetSelectionSettings", _mapId, layer?.Id, selectionEnabled, selectionStyle, multiSelect);
    }

    /// <summary>
    /// Shows the popup with the context the given feature at given coordinate
    /// </summary>
    /// <param name="feature">If set, set the context of the popup will be this feature</param>
    /// <param name="coordinate">Shows the popup at this location, if not set it will use the location of given the feature or the last clicked feature.</param>
    /// <returns></returns>
    public async Task ShowPopup(Feature? feature = null, Coordinate? coordinate = null)
    {
        if (_module == null)
            return;

        if (feature != null)
            _popupContext = feature;

        Coordinate popupCoordinate;
        if (coordinate.HasValue)
            popupCoordinate = coordinate.Value;
        else if (_popupContext != null)
            popupCoordinate = _popupContext.Coordinates.Point;
        else
            return;

        await _module.InvokeVoidAsync("MapOLShowPopup", _mapId, popupCoordinate);
    }

    protected ValueTask ApplyMapboxStyle(string styleUrl, string? accessToken)
    {
        if (_module != null)
            return _module.InvokeVoidAsync("MapOLApplyMapboxStyle", _mapId, styleUrl, accessToken);
        return ValueTask.CompletedTask;
    }
}