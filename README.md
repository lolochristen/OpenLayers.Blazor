# OpenLayers.Blazor

[![Release](https://github.com/lolochristen/OpenLayers.Blazor/actions/workflows/release.yml/badge.svg)](https://github.com/lolochristen/OpenLayers.Blazor/actions/workflows/release.yml)
[![NuGet](https://img.shields.io/nuget/v/OpenLayers.Blazor.svg)](https://www.nuget.org/packages/OpenLayers.Blazor/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/OpenLayers.Blazor.svg)](https://www.nuget.org/packages/OpenLayers.Blazor/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/lolochristen/OpenLayers.Blazor/blob/main/LICENSE)
![GitHub Tag](https://img.shields.io/github/v/tag/lolochristen/OpenLayers.Blazor)

A powerful Blazor component library that wraps [OpenLayers](https://openlayers.org/) for interactive mapping in .NET applications.

## 📋 Table of Contents

- [Features](#-features)
- [Demo](#-demo)
- [Installation](#-installation)
- [Quick Start](#-quick-start)
- [Supported Platforms](#-supported-platforms)
- [What's New in 2.0](#-whats-new-in-20)
- [Usage Examples](#-usage-examples)
- [Map Types](#-map-types)
- [Layer Types](#-layer-types)
- [Contributing](#-contributing)
- [License](#-license)

## ✨ Features

- **Multiple Map Providers**: OpenStreetMap, Bing Maps, Mapbox, SwissTopo, and any WMTS/WMS compatible source
- **Swiss Coordinate System**: Built-in support for Swiss projection systems (LV03/LV95)
- **Vector Layers**: Full support for vector layers with customizable styles
- **Rich Shapes**: Points, lines, polygons, circles, and multi-geometries
- **Markers**: Various marker types including pins, flags, and custom icons
- **Interactive Features**: 
  - Shape drawing and editing
  - Click events on features and layers
  - Shape selection with events
  - Popup support
- **Data Formats**: GeoJSON, KML, WKT, WMS, WMTS
- **Heatmaps**: Built-in heatmap layer support
- **Blazor Ready**: Works with Blazor Server, WebAssembly, and .NET MAUI Blazor

## 🎯 Demo

**Live Demo**: [https://openlayers-blazor-demo.laurent-christen.ch/](https://openlayers-blazor-demo.laurent-christen.ch/)

Explore interactive examples including:
- Basic maps with different providers
- Vector layers and GeoJSON data
- Drawing and editing features
- Markers and popups
- Heatmaps
- Swiss coordinate system examples

## 📦 Installation

### NuGet Package

```bash
dotnet add package OpenLayers.Blazor
```

Or via Package Manager Console:
```powershell
Install-Package OpenLayers.Blazor
```

### Include OpenLayers Assets

Add the OpenLayers JavaScript and CSS files to your `index.html` (Blazor WebAssembly/MAUI) or `_Host.cshtml`/`App.razor` (Blazor Server):

```html
<head>
    ...
    <!-- OpenLayers CSS -->
    <link href="https://cdn.jsdelivr.net/npm/ol@v10.6.1/ol.css" rel="stylesheet" />
    <!-- OpenLayers.Blazor CSS -->
    <link href="_content/OpenLayers.Blazor/OpenLayers.Blazor.css" rel="stylesheet" />
    <!-- OpenLayers JavaScript -->
    <script src="https://cdn.jsdelivr.net/npm/ol@v10.6.1/dist/ol.js"></script>
    ...
</head>
```

**Alternative CDN sources**:
```html
<!-- From cdnjs -->
<link href="https://cdnjs.cloudflare.com/ajax/libs/openlayers/10.6.1/ol.min.css" rel="stylesheet" />
<script src="https://cdnjs.cloudflare.com/ajax/libs/openlayers/10.6.1/dist/ol.min.js"></script>
```

## 🚀 Quick Start

### Basic OpenStreetMap

```razor
<OpenStreetMap Style="height:600px" Zoom="12" Center="new Coordinate(0, 51)">
    <Features>
        <Marker Type="MarkerType.MarkerPin" Coordinate="new Coordinate(0, 51)" Text="London" Popup />
    </Features>
</OpenStreetMap>
```

### Swiss Map with Markers

```razor
<SwissMap Style="height:800px" Zoom="10" Center="new Coordinate(2600000, 1200000)">
    <Features>
        <Marker Type="MarkerType.MarkerPin" 
                Coordinate="new Coordinate(2604200, 1197650)" 
                Text="Bern" 
                BackgroundColor="#FF5733" 
                Popup />
    </Features>
</SwissMap>
```

## 🎯 Supported Platforms

- **.NET 8.0**
- **.NET 9.0**
- **.NET 10.0**

Works with:
- Blazor Server
- Blazor WebAssembly
- .NET MAUI Blazor (Hybrid)

## 🆕 What's New in 2.0

### Major Features
- ✅ **Full vector layer support** with the ability to add shapes per layer
- ✅ **Comprehensive style customization** for all vector elements
- ✅ **Shape selection** feature with selection events on layers
- ✅ **Automatic updates** when parameters change on layers and shapes (component mode)
- ✅ **On-demand layer creation** - Internal shape/drawing and marker layers are added automatically when needed
- ✅ **Improved event handling** - Fixed issue where `InteractionsEnabled = false` also blocked mouse events

### Breaking Changes
- Internal shape and drawing layers are now part of the layers collection
- Some API changes for better consistency

### Bug Fixes
- Mouse events now work correctly even when interactions are disabled
- Improved memory management and disposal
- Better coordinate system transformations

## 📚 Usage Examples

### Map with Custom Popup

```razor
<OpenStreetMap Style="height:600px" Zoom="5" Center="new Coordinate(10, 47)">
    <Popup>
        <div class="custom-popup">
            @if (context is Marker marker)
            {
                <h3>@marker.Text</h3>
                <p>📍 @marker.Coordinate.X, @marker.Coordinate.Y</p>
            }
        </div>
    </Popup>
    <Features>
        <Marker Type="MarkerType.MarkerFlag" 
                Coordinate="new Coordinate(10, 47)" 
                Text="Vienna" 
                BackgroundColor="#449933" 
                Popup />
    </Features>
</OpenStreetMap>
```

### Vector Layers with GeoJSON

```razor
<OpenStreetMap Style="height:800px" Zoom="3">
    <Layers>
        <Layer LayerType="LayerType.VectorImage" 
               SourceType="SourceType.VectorGeoJson" 
               Url="https://openlayers.org/data/vector/ecoregions.json" 
               ZIndex="10" 
               Opacity=".5" />
    </Layers>
</OpenStreetMap>
```

### Drawing Shapes

```razor
<OpenStreetMap @ref="_map" Style="height:600px" Zoom="6">
    <Layers>
        <Layer LayerType="LayerType.Vector" SourceType="SourceType.Vector">
            <Shapes>
                <Circle Center="new(9.674681, 46.778543)" 
                        Radius="50000" 
                        Stroke="red" 
                        Fill="rgba(255,0,0,0.2)" 
                        StrokeThickness="3" />

                <Polygon Points="@_polygonPoints" 
                         Stroke="blue" 
                         Fill="rgba(0,0,255,0.3)" 
                         StrokeThickness="2" />

                <LineString Points="@_linePoints" 
                            Stroke="green" 
                            StrokeThickness="4" />
            </Shapes>
        </Layer>
    </Layers>
</OpenStreetMap>

@code {
    private Coordinate[] _polygonPoints = new[]
    {
        new Coordinate(5, 45),
        new Coordinate(15, 45),
        new Coordinate(15, 50),
        new Coordinate(5, 50),
        new Coordinate(5, 45)
    };

    private Coordinate[] _linePoints = new[]
    {
        new Coordinate(0, 45),
        new Coordinate(10, 50),
        new Coordinate(20, 45)
    };
}
```

### WMS Layer

```razor
<OpenStreetMap Style="height:600px" Zoom="5" Center="new Coordinate(0, 51)">
    <Layers>
        <Layer SourceType="SourceType.TileWMS"
               Url="https://sedac.ciesin.columbia.edu/geoserver/ows"
               Params="@(new Dictionary<string, string> 
               {
                   { "LAYERS", "gpw-v3:gpw-v3-population-density_2000" },
                   { "TILED", "true" }
               })"
               Opacity=".6"
               CrossOrigin="anonymous" />
    </Layers>
</OpenStreetMap>
```

### Heatmap Layer

```razor
<OpenStreetMap Style="height:600px" Zoom="2">
    <Layers>
        <Layer LayerType="LayerType.Heatmap" 
               SourceType="SourceType.VectorKML" 
               Url="https://openlayers.org/en/latest/examples/data/kml/2012_Earthquakes_Mag5.kml" 
               Options="@(new { blur = 15, radius = 5 })" />
    </Layers>
</OpenStreetMap>
```

### Interactive Events

```razor
<OpenStreetMap @ref="_map" 
               Style="height:600px" 
               OnClick="OnMapClick"
               OnPointerMove="OnPointerMove">
    <Layers>
        <Layer @ref="_layer" 
               LayerType="LayerType.Vector" 
               SourceType="SourceType.Vector"
               OnFeatureClick="OnFeatureClick">
            <Shapes>
                <Circle @ref="_selectedShape" 
                        Center="new(10, 47)" 
                        Radius="50000" 
                        Stroke="blue" 
                        Fill="rgba(0,0,255,0.2)" />
            </Shapes>
        </Layer>
    </Layers>
</OpenStreetMap>

<div class="mt-3">
    <p>Last clicked: @_lastClickPosition</p>
    <p>Mouse position: @_mousePosition</p>
</div>

@code {
    private Map? _map;
    private Layer? _layer;
    private Shape? _selectedShape;
    private string? _lastClickPosition;
    private string? _mousePosition;

    private async Task OnMapClick(Coordinate coordinate)
    {
        _lastClickPosition = $"{coordinate.X:F2}, {coordinate.Y:F2}";
        StateHasChanged();
    }

    private void OnPointerMove(Coordinate coordinate)
    {
        _mousePosition = $"{coordinate.X:F2}, {coordinate.Y:F2}";
        StateHasChanged();
    }

    private async Task OnFeatureClick(Feature feature)
    {
        if (feature is Shape shape)
        {
            shape.Stroke = "red";
            await shape.UpdateShape();
        }
    }
}
```

## 🗺️ Map Types

The library provides pre-configured map components:

### OpenStreetMap
```razor
<OpenStreetMap Style="height:600px" Zoom="12" Center="new Coordinate(0, 51)" />
```

### Bing Maps
```razor
<BingMap Style="height:600px" 
         Zoom="10" 
         Center="new Coordinate(-74, 40)"
         Key="your-bing-maps-key"
         ImagerySet="Aerial" />
```

### Mapbox
```razor
<MapboxMap Style="height:600px"
           Zoom="12"
           Center="new Coordinate(-122, 37)"
           AccessToken="your-mapbox-token"
           StyleUrl="mapbox://styles/mapbox/streets-v11" />
```

### Swiss Topo Map (LV95/LV03)
```razor
<SwissMap Style="height:800px" 
          Zoom="10" 
          Center="new Coordinate(2600000, 1200000)"
          LayerId="ch.swisstopo.pixelkarte-farbe" />
```

Available Swiss layers:
- `ch.swisstopo.pixelkarte-farbe` (default color map)
- `ch.swisstopo.pixelkarte-grau` (grayscale)
- `ch.swisstopo.swissimage` (satellite imagery)
- `ch.swisstopo.swissalti3d-reliefschattierung` (relief)
- And many more from [geo.admin.ch](https://map.geo.admin.ch/)

## 📊 Layer Types

### Tile Layers
- **TileOSM**: OpenStreetMap tiles
- **TileWMS**: Web Map Service
- **TileXYZ**: Custom tile servers
- **TileBingMaps**: Bing Maps tiles

### Vector Layers
- **Vector**: Standard vector layer
- **VectorImage**: Optimized vector rendering
- **VectorTile**: Vector tile layer

### Data Source Types
- **VectorGeoJSON**: GeoJSON data
- **VectorKML**: KML files
- **VectorWKT**: Well-Known Text
- **VectorWFS**: Web Feature Service

### Special Layers
- **Heatmap**: Density visualization
- **Image**: Static images with geographic bounds
- **Graticule**: Coordinate grid lines

## 📖 Documentation

### Key Properties

#### Map Properties
- `Center` - Map center coordinate
- `Zoom` - Zoom level (1-20)
- `Rotation` - Map rotation in radians
- `InteractionsEnabled` - Enable/disable map interactions
- `ShowControls` - Show/hide map controls
- `MinZoom` / `MaxZoom` - Zoom constraints

#### Layer Properties
- `LayerType` - Type of layer (Vector, VectorImage, Tile, etc.)
- `SourceType` - Data source type
- `Url` - Data source URL
- `Opacity` - Layer opacity (0-1)
- `ZIndex` - Layer stacking order
- `Visible` - Layer visibility

#### Shape Properties
- `Coordinates` - Shape geometry
- `Stroke` - Outline color
- `StrokeThickness` - Line width
- `Fill` - Fill color
- `Popup` - Enable popup on click
- `ZIndex` - Shape stacking order

### Events
- `OnClick` - Map click event
- `OnPointerMove` - Mouse move event
- `OnFeatureClick` - Feature/shape click event
- `OnSelectionChanged` - Selection change event
- `OnDrawEnd` - Drawing completed event

## 🛠️ Troubleshooting

### Map not displaying
- Ensure OpenLayers CSS and JS are properly loaded
- Check browser console for errors
- Verify the map has a defined height in the `Style` attribute

### Swiss coordinates not working
- Use `SwissMap` component instead of `OpenStreetMap`
- Ensure coordinates are in LV95 format (EPSG:2056)
- Check coordinate values are in the valid range

### Layers not visible
- Check `ZIndex` values - higher values are on top
- Verify `Opacity` is not set to 0
- Ensure `Visible` property is true
- Check layer bounds match the visible extent

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built on [OpenLayers](https://openlayers.org/) - A high-performance library for interactive maps
- Swiss projection support based on [geo.admin.ch](https://map.geo.admin.ch/)

## 📞 Support

- 📫 Issues: [GitHub Issues](https://github.com/lolochristen/OpenLayers.Blazor/issues)
- 💬 Discussions: [GitHub Discussions](https://github.com/lolochristen/OpenLayers.Blazor/discussions)
- 🌟 Don't forget to star the project if you find it useful!

---

Made with ❤️ for the Blazor community
