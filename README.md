# OpenLayers.Blazor

This is a map component based on [OpenLayers](https://openlayers.org/) to different compatible maps with some features to set a marker or other shapes. The component has implemented the swiss specific coordination projection (VL03/VL95) and therefore can show content from swisstopo. 

## Usage

### Setup

- Download the openlayers distribution js/css files https://github.com/openlayers/openlayers or from other sources and include them in the index.html file:

    ```html
    <head>
    ...
        <link href="lib/openlayers/ol.css" rel="stylesheet" />
        <link href="_content/OpenLayers.Blazor/OpenLayers.Blazor.css" rel="stylesheet" />
        <script src="lib/openlayers/dist/ol.min.js"></script>
    ...
    </head>
    ```
### Examples

```html
    <SwissMap OnClick="OnMapClick"></SwissMap>
``` 

see demo solution.
