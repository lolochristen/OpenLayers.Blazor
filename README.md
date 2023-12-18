# OpenLayers.Blazor

This is a map component based on [OpenLayers](https://openlayers.org/) to show different types and layers of maps from sources like OpenStreet, Bing, SwissTopo ary WMTS/WMS compatible source with some features like markes or shape drawing. The component has implemented the swiss specific coordination projection (VL03/VL95) and therefore can project map content from swisstopo (https://map.geo.admin.ch/). 

## Major Changes

Starting fromn version 1.5, the coordinates are represented and defined according to EPSG. This means the input for a Coordinate object will be switched: Coordinate(1197650, 2604200) > Coordinate(2604200, 1197650) or new Coordinate(51, 0) > Coordinate(0, 51)

## Demo/Examples

https://openlayers-blazor-demo.laurent-christen.ch/

## Usage

### Setup

- install nuget package ```dotnet add package OpenLayers.Blazor```
- Download the openlayers distribution js/css files https://github.com/openlayers/openlayers or from other sources and include them in the index.html file:

    ```html
    <head>
    ...
        <link href="https://cdnjs.cloudflare.com/ajax/libs/openlayers/8.1.0/ol.min.css" rel="stylesheet" />
        <link href="_content/OpenLayers.Blazor/OpenLayers.Blazor.css" rel="stylesheet" />
        <script src="https://cdnjs.cloudflare.com/ajax/libs/openlayers/8.1.0/dist/ol.min.js"></script>
    ...
    </head>
    ```

### Examples

```html
    <SwissMap Style="height:800px">
        <Popup>
            <div id="popup" class="ol-box">
                @if (context is Marker marker)
                {
                    <h3>@marker.Title</h3>
                    <p>@marker.Coordinate.X / @marker.Coordinate.Y</p>
                }
            </div>
        </Popup>
        <Features>
            <Marker Type="MarkerType.MarkerPin" Coordinate="new Coordinate(2604200, 1197650)"></Marker>
            <Marker Type="MarkerType.MarkerFlag" Coordinate="new Coordinate(2624200, 1177650)" Title="Hallo" BackgroundColor="#449933" Popup></Marker>
            <Line Points="new []{new Coordinate(1197650, 2604200), new Coordinate(2624200, 1177650)}" BorderColor="cyan"></Line>
        </Features>
    </SwissMap>

    <OpenStreetMap Style="height:480px; width:640px" Zoom="5" Center="new Coordinate(0, 51)">
        <Layers>
            <Layer SourceType="SourceType.TileWMS"
                    Url="https://sedac.ciesin.columbia.edu/geoserver/ows?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&FORMAT=image%2Fpng&TRANSPARENT=true&LAYERS=gpw-v3%3Agpw-v3-population-density_2000&LANG=en"
                    Opacity=".3"
                    CrossOrigin="anonymous"></Layer>
        </Layers>
    </OpenStreetMap>
``` 
