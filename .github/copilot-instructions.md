# OpenLayers.Blazor Development Guide

This is a Blazor component library that wraps OpenLayers for .NET. It provides map components for displaying various map types (OpenStreetMap, Bing, SwissTopo) with support for markers, shapes, and vector layers.

## Build, Test, and Lint

**Prerequisites:**
- .NET 8.0, 9.0, and 10.0 SDKs
- Node.js 22+ (for JavaScript bundling)

**Build:**
```bash
dotnet build
```

**Test:**
```bash
# Run all tests
dotnet test

# Run specific test
dotnet test --filter "FullyQualifiedName~MapLayerTests.Map_LayerComponentAdded_InCollection"
```

**Package:**
```bash
dotnet pack -c Release
```

**Note:** The project uses BuildBundlerMinifier for .NET 10.0 target to minify JavaScript. The bundling runs automatically during build for net10.0.

## Architecture

### Multi-Targeting Structure

The library targets three .NET versions (net8.0, net9.0, net10.0) with conditional compilation:
- Different `Microsoft.AspNetCore.Components` package versions per target framework
- Preprocessor directives (`NET8_0`, `NET9_0`, `NET10_0`) for version-specific code
- BuildBundlerMinifier only runs for net10.0 builds

### Component Model

**Public Components:**
- `Map.razor` - Base map component with JavaScript interop
- `Layer.razor` - Blazor component for map layers (can be used declaratively)
- Pre-configured maps: `SwissMap`, `OpenStreetMap`, `BingMap`, `MapboxMap` (all inherit from `Map`)

**Internal Types:**
- `Internal/` namespace contains serialization DTOs for JS interop
- Public component classes wrap internal types to provide clean API
- Example: `Layer.razor.cs` wraps `Internal.Layer`

### JavaScript Interop

The library uses ES6 module-based JS interop:
- `wwwroot/openlayers_interop.js` - Main interop module (not minified in development)
- `wwwroot/openlayers_interop.min.js` - Production bundle (minified via bundleconfig.json)
- Module imports are conditional based on DEBUG vs RELEASE builds
- All interop methods prefixed with `MapOL*` (e.g., `MapOLInit`, `MapOLZoom`)

### Data Flow

1. **Blazor Components** → **Internal DTOs** → **JSON** → **JavaScript** → **OpenLayers API**
2. Changes to component parameters trigger updates via `OnParametersSetAsync`
3. `ObservableRangeCollection` tracks shape/layer changes and syncs to JavaScript
4. Events flow back: JavaScript callbacks → C# EventCallbacks

### Swiss Projection System

The library has built-in support for Swiss coordinate systems (LV03/LV95):
- `SwissMap.cs` configures Swiss projection by default
- Coordinate transformations handled in `CoordinateConverter.cs`
- Distinct from standard WGS84/Web Mercator projections

## Key Conventions

### Component Initialization Pattern

Components use a static counter for unique IDs:
```csharp
private static int _counter = 0;
private string _mapId;

public Map()
{
    _counter++;
    _mapId = "map_" + _counter;
}
```

### Parameter Change Tracking

Layers and shapes track parameter changes with a flag pattern:
```csharp
private bool _updateableParametersChanged = false;

protected override void OnParametersSet()
{
    _updateableParametersChanged = true;
}

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (_updateableParametersChanged)
    {
        // Sync to JavaScript
        _updateableParametersChanged = false;
    }
}
```

### Shape/Layer Collections

- `ObservableRangeCollection<T>` extends `ObservableCollection<T>` with bulk operations
- Collections are auto-created on first access (lazy initialization)
- Internal layers (markers, shapes, drawing) are added on demand
- Use `ShapesList` for shapes layer, `MarkersList` for markers layer

### JSON Serialization

- Enums use `JsonStringEnumKebabLowerConverter` for kebab-case serialization
- Custom converters for `Coordinate`, `Coordinates` types
- `Internal.*` classes are serialization DTOs (don't add logic there)

### Testing with bUnit

Tests use bUnit for Blazor component testing:
- Mock JS interop with `JSInterop.SetupModule()`
- Set `Mode = JSRuntimeMode.Loose` to avoid strict validation
- Different module paths for DEBUG vs RELEASE builds (see `MapLayerTests.cs`)

### Suppressed Warnings

The project suppresses these Blazor analyzer warnings:
- `BL0005` - Component parameter should not be set outside of component initialization
- `BL0007` - Component parameters should be auto properties

This is intentional for the internal/public wrapper pattern used throughout.

## Project Structure

- `src/OpenLayers.Blazor/` - Main library package
- `src/OpenLayers.Blazor.Demo*` - Demo applications (Blazor Server, WebAssembly, MAUI)
- `src/OpenLayers.Blazor.FlightTracker/` - Example MAUI application
- `test/OpenLayers.Blazor.Tests/` - xUnit + bUnit tests

## Release Process

Releases are automated via GitHub Actions:
- Triggered on GitHub release publication
- Uses MinVer for semantic versioning from Git tags
- Builds all target frameworks, runs tests, packs and publishes to NuGet
- Requires Node.js 22 for JavaScript bundling

## Common Tasks

**Adding a new shape type:**
1. Create class inheriting from `Shape` (e.g., `Circle.cs`)
2. Add corresponding `Internal.Shape` serialization type
3. Update `ShapeType` enum
4. Implement JavaScript rendering in `openlayers_interop.js`

**Adding a new layer parameter:**
1. Add property to `Layer.razor.cs` wrapping `_internalLayer` field
2. Add corresponding field to `Internal.Layer`
3. Mark parameter with `[Parameter]` attribute
4. Handle in `OnParametersSet()` if needs sync after render

**Modifying JavaScript interop:**
1. Edit `wwwroot/openlayers_interop.js`
2. For net10.0, minification happens automatically
3. For net8.0/net9.0, bundling is manual (consider testing both)
4. Update module version query string on breaking changes
