var _MapOL = new Array();

export function MapOLInit(mapId, popupId, defaults, center, zoom, markers, shapes, layers, instance) {
    _MapOL[mapId] = new MapOL(mapId, popupId, defaults, center, zoom, markers, shapes, layers, instance);
}

export function MapOLDispose(mapId) {
    _MapOL[mapId] = undefined;
}

export function MapOLCenter(mapId, point) {
    _MapOL[mapId].setCenter(point);
}

export function MapOLZoom(mapId, zoom) {
    _MapOL[mapId].setZoom(zoom);
}

export function MapOLSetDefaults(mapId, defaults) {
    _MapOL[mapId].setDefaults(defaults);
}

export function MapOLLoadGeoJson(mapId, url) {
    _MapOL[mapId].loadGeoJson(url);
}

export function MapOLZoomToExtent(mapId, extent) {
    _MapOL[mapId].setZoomToExtent(extent);
}

export function MapOLMarkers(mapId, markers) {
    _MapOL[mapId].setMarkers(markers);
}

export function MapOLSetShapes(mapId, shapes) {
    _MapOL[mapId].setShapes(shapes);
}

export function MapOLCenterToCurrentGeoLocation(mapId) {
    _MapOL[mapId].centerToCurrentGeoLocation();
}

export function MapOLGetCurrentGeoLocation(mapId) {
    return _MapOL[mapId].getCurrentGeoLocation();
}

export function MapOLSetLayers(mapId, layers) {
    _MapOL[mapId].setLayers(layers);
}

export function MapOLRemoveLayer(mapId, layer) {
    _MapOL[mapId].removeLayer(layer);
}

export function MapOLAddLayer(mapId, layer) {
    _MapOL[mapId].addLayer(layer);
}

export function MapOLUpdateLayer(mapId, layer) {
    _MapOL[mapId].updateLayer(layer);
}

// --- MapOL ----------------------------------------------------------------------------//

function MapOL(mapId, popupId, defaults, center, zoom, markers, shapes, layers, instance) {
    this.Instance = instance;
    this.Defaults = defaults;

    //LV03
    const projectionLV03 = new ol.proj.Projection({
        code: 'EPSG:21781',
        extent: [485869.5728, 76443.1884, 837076.5648, 299941.7864],
        units: 'm',
    });
    ol.proj.addProjection(projectionLV03);

    // LV95
    const projectionLV95 = new ol.proj.Projection({
        code: 'EPSG:2056',
        extent: [2485071.58, 1074261.72, 2837119.8, 1299941.79],
        units: 'm',
    });
    ol.proj.addProjection(projectionLV95);

    ol.proj.addCoordinateTransforms(
        'EPSG:4326',
        projectionLV03,
        function (coordinate) {
            return [
                MapOL.WGStoLV03y(coordinate[1], coordinate[0]),
                MapOL.WGStoLV03x(coordinate[1], coordinate[0]),
            ];
        },
        function (coordinate) {
            return [
                MapOL.CHtoLV03lng(coordinate[0], coordinate[1]),
                MapOL.CHtoLV03lat(coordinate[0], coordinate[1]),
            ];
        }
    );

    ol.proj.addCoordinateTransforms(
        'EPSG:4326',
        projectionLV95,
        function (coordinate) {
            return [
                MapOL.WGStoLV95y(coordinate[1], coordinate[0]),
                MapOL.WGStoLV95x(coordinate[1], coordinate[0]),
            ];
        },
        function (coordinate) {
            return [
                MapOL.LV95toWGSlng(coordinate[0], coordinate[1]),
                MapOL.LV95toWGSlat(coordinate[0], coordinate[1]),
            ];
        }
    );

    const ollayers = MapOL.prepareLayers(layers);

    let viewProjection = undefined;
    let viewExtent = (ollayers.length > 0) ? ollayers[0].getExtent() : undefined;
    let viewCenter = (center && center.coordinates) ? center.coordinates : undefined;
    if (this.Defaults.coordinatesProjection == 'EPSG:2056') {
        viewProjection = projectionLV95;
    }
    else if (this.Defaults.coordinatesProjection == 'EPSG:21781') {
        viewProjection = projectionLV03;
    }
    else if (this.Defaults.coordinatesProjection == 'EPSG:4326') {
        if (viewCenter)
            viewCenter = ol.proj.transform(viewCenter, 'EPSG:4326', 'EPSG:3857');
    }

    this.Map = new ol.Map({
        layers: ollayers,
        target: mapId,
        controls: defaults.scaleLineUnit != 'none' ? ol.control.defaults.defaults().extend([
            new ol.control.ScaleLine({
                units: defaults.scaleLineUnit.toLowerCase(),
            }),
        ]) : null,
        view: new ol.View({
            projection: viewProjection,
            center: viewCenter,
            extent: viewExtent,
            zoom: zoom
        })
    });

    this.Geometries = new ol.layer.Vector({
        source: new ol.source.Vector(),
        zIndex: 999
    });

    this.Map.addLayer(this.Geometries);

    this.Markers = new ol.layer.Vector({
        source: new ol.source.Vector(),
        zIndex: 999
    });

    this.Map.addLayer(this.Markers);

    var popupElement = document.getElementById(popupId);

    var popup = new ol.Overlay({
        element: popupElement,
        positioning: 'bottom-center',
        stopEvent: false,
        offset: [0, -50],
    });

    this.Map.addOverlay(popup);

    var that = this;

    this.Map.on('click', function (evt) { that.onMapClick(evt, popup, popupElement) });
    this.Map.on('pointermove', function (evt) { that.onMapPointerMove(evt, popupElement) });
    this.Map.getView().on("change:resolution", function (evt) { that.Instance.invokeMethodAsync('OnInternalZoomChanged', that.Map.getView().getZoom()); });
    this.Map.getView().on("change:center", function (evt) { that.onMapCenterChanged(); });
    this.setMarkers(markers);
    this.setShapes(shapes);
}

MapOL.prepareLayers = function (layers) {
    const ollayers = new Array();

    layers.forEach((l, i, arr) => {

        let source;
        if (l.extent == null) l.extent = undefined;
        if (l.className == null) l.className = undefined;
        if (l.minResolution == null) l.minResolution = undefined;
        if (l.maxResolution == null) l.maxResolution = undefined;
        if (l.maxZoom == null) l.maxZoom = undefined;
        if (l.minZoom == null) l.minZoom = undefined;
        if (l.zIndex == null) l.zIndex= undefined;
        if (l.source.url == null) l.source.url = undefined;
        if (l.source.urls == null) l.source.urls = undefined;
        if (l.source.cacheSize == null) l.source.cacheSize = undefined;
        if (l.source.crossOrigin == null) l.source.crossOrigin = undefined;
        if (l.source.transition == null) l.source.transition = undefined;
        if (l.source.layer == null) l.source.layer = undefined;
        if (l.source.key == null) l.source.key = undefined;
        if (l.source.serverType == null) l.source.serverType = undefined;
        if (l.source.matrixSet == null) l.source.matrixSet = undefined;
        if (l.source.format == null) l.source.format = undefined;
        if (l.source.projection == null) l.source.projection = undefined;
        if (l.source.reprojectionErrorThreshold == null) l.source.reprojectionErrorThreshold = undefined;

        switch (l.source.sourceType) {
            case 'TileImage':
                source = new ol.source.TileImage(l.source);
                break;
            case 'BingMaps':
                source = new ol.source.BingMaps(l.source);
                break;
            case 'OGCMapTile':
                source = new ol.source.OGCMapTile(l.source);
                break;
            case 'TileArcGISRest':
                source = new ol.source.TileArcGISRest(l.source);
                break;
            case 'TileJSON':
                source = new ol.source.TileJSON(l.source);
                break;
            case 'TileWMS':
                source = new ol.source.TileWMS(l.source);
                break;
            case 'WMTS':
                source = new ol.source.WMTS(l.source);
                break;
            case 'Zoomify':
                source = new ol.source.Zoomify(l.source);
                break;
            case 'OSM':
                source = new ol.source.OSM(l.source);
                break;
            case 'XYZ':
                source = new ol.source.XYZ(l.source);
                break;
            case 'CartoDB':
                source = new ol.source.CartoDB(l.source);
                break;
            case 'Stamen':
                source = new ol.source.Stamen(l.source);
                break;
            case 'TileDebug':
                source = new ol.source.TileDebug(l.source);
                break;
        }

        l.source = source;
        ollayers.push(new ol.layer.Tile(l));
    });

    return ollayers;
}

MapOL.prototype.setLayers = function (layers) {
     this.Map.setLayers(MapOL.prepareLayers(layers));
}

MapOL.prototype.removeLayer = function (layer) {
    this.Map.getAllLayers().forEach((l) => {
        try {
            const source = l.getSource();
            if (source.urls[0] == layer.source.url) {
                this.Map.removeLayer(l);
            }
        }
        catch { }
    });
}

MapOL.prototype.addLayer = function (layer) {
    var ollayers = MapOL.prepareLayers([layer])
    this.Map.addLayer(ollayers[0]);
}

MapOL.prototype.updateLayer = function (layer) {
    var ollayers = MapOL.prepareLayers([layer])
    var olayer = this.findLayer(ollayers[0]);
    if (olayer != undefined) {
        olayer.setVisible(layer.visibility);
        olayer.setOpacity(layer.opacity);
        olayer.setZIndex(layer.zindex);
        olayer.setExtent(layer.extent);
    }
}

MapOL.prototype.findLayer = function (layer) {
    let foundLayer = undefined;
    this.Map.getAllLayers().forEach((l) => {
        try {
            var source = l.getSource();
            var layerSource = layer.getSource();
            if (source.urls[0] == layerSource.urls[0] && source.getKey() == layerSource.getKey()) {
                foundLayer = l;
            }
        }
        catch { }
    });
    return foundLayer;
}

MapOL.prototype.setMarkers = function (markers) {
    var source = this.Markers.getSource();

    source.clear();

    markers.forEach((marker) => {
        var feature = new ol.Feature({
            geometry: new ol.geom.Point(ol.proj.transform(marker.geometry.coordinates, this.Defaults.coordinatesProjection, this.Map.getView().getProjection().getCode())),
            popup: marker.popup,
            title: marker.title,
            content: marker.content
        });

        feature.marker = marker;

        switch (marker.kind) {
            case "MarkerPin":
                feature.setStyle(this.pinStyle(marker));
                break;

            case "MarkerFlag":
                feature.setStyle(this.flagStyle(marker));
                break;

            case "MarkerAwesome":
                feature.setStyle(this.awesomeStyle(marker));
                break;

            case "MarkerCustomImage":
                feature.setStyle(this.customImageStyle(marker));
                break;
        }

        source.addFeature(feature);
    });
}

MapOL.prototype.setShapes = function (shapes) {
    var source = this.Geometries.getSource();

    source.clear();

    if (!shapes) return;

    shapes.forEach((shape) => {
        var feature;
        var viewProjection = this.Map.getView().getProjection().getCode();
        switch (shape.kind) {
            case "ShapeLine":
                for (var i = 0; i < shape.geometry.coordinates.length; i++) {
                    shape.geometry.coordinates[i] = ol.proj.transform(shape.geometry.coordinates[i], this.Defaults.coordinatesProjection, viewProjection);
                }

                feature = new ol.Feature({
                    geometry: new ol.geom.LineString(shape.geometry.coordinates),
                    popup: shape.popup,
                    title: shape.title,
                    content: shape.content
                });
                break;

            case "ShapeCircle":
                shape.geometry.coordinates = ol.proj.transform(shape.geometry.coordinates, this.Defaults.coordinatesProjection, viewProjection);

                var circle = new ol.geom.Circle(shape.geometry.coordinates, shape.radius / ol.proj.getPointResolution(viewProjection, 1, shape.geometry.coordinates));

                feature = new ol.Feature({
                    geometry: new ol.geom.Polygon.fromCircle(circle, 32, 90),
                    popup: shape.popup,
                    title: shape.title,
                    content: shape.content
                });
                break;
        }

        feature.shape = shape;

        switch (shape.kind) {
            case "ShapeLine":
                feature.setStyle(this.lineStyle(shape));
                break;

            case "ShapeCircle":
                feature.setStyle(this.circleStyle(shape));
                break;
        }

        source.addFeature(feature);
    });
}

MapOL.prototype.loadGeoJson = function (json) {
    if (this.GeoLayer) {
        var source = this.GeoLayer.getSource();

        source.clear();
    }

    if (!json) return;

    var geoSource = new ol.source.Vector({
        features: (new ol.format.GeoJSON()).readFeatures(json, { featureProjection: this.Defaults.coordinatesProjection })
    });

    if (this.GeoLayer) {
        this.GeoLayer.setSource(geoSource);
    }
    else {
        this.GeoLayer = new ol.layer.Vector({
            source: geoSource,
            style: (feature) => this.getGeoStyle(feature)
        });

        this.Map.addLayer(this.GeoLayer);
    }
}

MapOL.prototype.setZoom = function (zoom) {
    this.Map.getView().setZoom(zoom);
}

MapOL.prototype.setZoomToExtent = function (extent) {
    switch (extent) {
        case "Markers":
            var extent = this.Markers.getSource().getExtent();
            if (extent[0] === Infinity) return;
            this.Map.getView().fit(extent, this.Map.getSize());
            break;

        case "Geometries":
            var extent = this.Geometries.getSource().getExtent();
            if (extent[0] === Infinity) return;
            this.Map.getView().fit(extent, this.Map.getSize());
            break;
    }
}

MapOL.prototype.setCenter = function (point) {
    this.Map.getView().setCenter(ol.proj.transform(point.coordinates, this.Defaults.coordinatesProjection, this.Map.getView().getProjection().getCode()));
}

MapOL.prototype.setDefaults = function (defaults) {
    this.Defaults = defaults;
}

MapOL.prototype.getReducedFeature = function (feature) {
    var type = feature.getGeometry().getType();

    var objectWithoutKey = (object, key) => {
        const { [key]: deletedKey, ...otherKeys } = object;
        return otherKeys;
    }

    var properties = objectWithoutKey(feature.getProperties(), "geometry");

    var reduced = {
        type: "Feature",
        geometry: {
            type: feature.getGeometry().getType(),
            coordinates: feature.getGeometry().getCoordinates()
        },
        properties: properties
    };

    return reduced;
}

MapOL.prototype.onMapClick = function (evt, popup, element) {
    popup.setPosition(0, 0);

    var that = this;
    var invokeMethod = true;

    this.Map.forEachFeatureAtPixel(evt.pixel, function (feature) {
        if (feature != null) {
            var reduced = that.getReducedFeature(feature);
            that.Instance.invokeMethodAsync('OnInternalFeatureClick', reduced);
        }

        var shape = null;

        if ((feature.marker != null) && invokeMethod) {
            shape = feature.marker;
            invokeMethod = false;
            that.Instance.invokeMethodAsync('OnInternalMarkerClick', shape);
        }

        if ((feature.shape != null) && invokeMethod) {
            shape = feature.shape;
            invokeMethod = false;
            that.Instance.invokeMethodAsync('OnInternalShapeClick', shape);
        }

        var showPopup = false;

        if (shape) {
            showPopup = shape.popup;
        }
        else if (that.Defaults.autoPopup) {
            showPopup = true;
        }

        if (showPopup) {
            var coordinates = feature.getGeometry().getCoordinates();
            popup.setPosition(coordinates);
        }
    });

    if (invokeMethod) {
        invokeMethod = false;
        var coordinate = ol.proj.transform(evt.coordinate, 'EPSG:3857', this.Defaults.coordinatesProjection)
        var point = { Y: coordinate[1], X: coordinate[0] };
        this.Instance.invokeMethodAsync('OnInternalClick', point);
    }
}

MapOL.prototype.onMapPointerMove = function (evt, element) {
    if (evt.dragging || Number.isNaN(evt.coordinate[0])) {
        return;
    }
    var coordinate = ol.proj.transform(evt.coordinate, this.Map.getView().getProjection().getCode(), this.Defaults.coordinatesProjection)
    var point = { Y: coordinate[1], X: coordinate[0] };
    this.Instance.invokeMethodAsync('OnInternalPointerMove', point);
}

MapOL.prototype.onMapCenterChanged = function () {
    var coordinate = ol.proj.transform(this.Map.getView().getCenter(), this.Map.getView().getProjection().getCode(), this.Defaults.coordinatesProjection)
    var point = { Y: coordinate[1], X: coordinate[0] };
    this.Instance.invokeMethodAsync('OnInternalCenterChanged', point);
}

MapOL.prototype.centerToCurrentGeoLocation = function () {
    var that = this;
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function(position) {
            var projection = that.Map.getView().getProjection().getCode();
            that.Map.getView().setCenter(ol.proj.transform([position.coords.longitude, position.coords.latitude], 'EPSG:4326', projection));
        });
    }
}

MapOL.prototype.getCurrentGeoLocation = function () {
    var that = this;
    return new Promise((resolve, reject) => {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                var projection = that.Map.getView().getProjection().getCode();
                var coordinate = ol.proj.transform([position.coords.longitude, position.coords.latitude], 'EPSG:4326', projection);
                var point = { Y: coordinate[1], X: coordinate[0] };
                resolve(point);
            });
        }
        else {
            reject("No geolocation received");
        };
    });
}

//--- Styles -----------------------------------------------------------------//

MapOL.prototype.pinStyle = function (marker) {
    var src = './_content/OpenLayers.Blazor/img/marker-pin-red.png';
    if (marker.color == null || marker.color != '#FFFFFF')
        src = './_content/OpenLayers.Blazor/img/marker-pin-'+marker.color+'.png';
    return [
        new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0, 60],
                size: [160, 60],
                offset: [0, 0],
                opacity: 1,
                scale: marker.scale,
                anchorXUnits: 'pixels',
                anchorYUnits: 'pixels',
                src: './_content/OpenLayers.Blazor/img/pin-back.png'
            })
        }),
        new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [100, 198],
                size: [200, 200],
                offset: [0, 0],
                opacity: 1,
                scale: marker.scale,
                anchorXUnits: 'pixels',
                anchorYUnits: 'pixels',
                src: src
                })
            })
        ];
}

MapOL.prototype.flagStyle = function (marker) {
    var padTop = 4;
    var padBottom = 2;
    var padLeft = 5;
    var padRight = 5;

    var size = 10;
    var width = size;
    var height = size;

    var canvas = document.createElement('canvas');
    var ctx = canvas.getContext("2d");

    var context = ol.render.toContext(canvas.getContext('2d'), {
        size: [width, height],
        pixelRatio: 1
    });

    var symbol = [
        [0, 0],
        [width, 0],
        [width / 2, height],
        [0, 0]
    ];

    context.setStyle(
        new ol.style.Style({
            fill: new ol.style.Fill({ color: marker.backgroundColor }),
            stroke: new ol.style.Stroke({ color: marker.borderColor, width: marker.borderSize }),
        })
    );

    context.drawGeometry(new ol.geom.Polygon([symbol]));

    return new ol.style.Style({
        image: new ol.style.Icon({
            anchorXUnits: 'pixels',
            anchorYUnits: 'pixels',
            anchor: [width / 2, height],
            size: [width, height],
            offset: [0, 0],
            img: canvas,
            imgSize: [width, height],
            scale: 1,
        }),
        text: new ol.style.Text({
            text: marker.properties.title,
            offsetY: -size - padBottom + 1,
            offsetX: -size,
            textAlign: "left",
            textBaseline: "bottom",
            scale: marker.textScale,
            fill: new ol.style.Fill({ color: marker.color }),
            backgroundFill: new ol.style.Fill({ color: marker.backgroundColor }),
            backgroundStroke: new ol.style.Stroke({ color: marker.borderColor, width: marker.borderSize }),
            padding: [padTop, padRight, padBottom, padLeft]
        })
    });
}

MapOL.prototype.awesomeStyle = function (marker) {
    return [
        new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0, 14],
                size: [56, 21],
                offset: [0, 0],
                opacity: 1,
                scale: 0.5,
                color: marker.color ?? this.Defaults.color,
                anchorXUnits: 'pixels',
                anchorYUnits: 'pixels',
                src: './_content/OpenLayers.Blazor/img/pin-back.png'
            }),
        }),
        new ol.style.Style({
            text: new ol.style.Text({
                text: String.fromCodePoint(0xF041), // Map Marker
                scale: 2,
                font: '900 18px "Font Awesome 6 Free"',
                textBaseline: 'bottom',
                fill: new ol.style.Fill({ color: marker.backgroundColor ?? this.Defaults.backgroundColor }),
                stroke: new ol.style.Stroke({ color: marker.borderColor ?? this.Defaults.borderColor, width: 3 })
            })
        }),
        new ol.style.Style({
            text: new ol.style.Text({
                text: marker.properties?.label ?? this.Defaults.label,
                offsetY: -22,
                opacity: 1,
                scale: 1,
                font: '900 18px "Font Awesome 6 Free"',
                fill: new ol.style.Fill({ color: marker.color ?? this.Defaults.color })
            })
        })
    ];
}

MapOL.prototype.customImageStyle = function (marker) {
    return [
        new ol.style.Style({
            image: new ol.style.Icon({
                anchor: marker.anchor,
                size: marker.size,
                offset: [0, 0],
                opacity: 1,
                scale: marker.scale,
                anchorXUnits: 'pixels',
                anchorYUnits: 'pixels',
                src: marker.content
                })
            })
        ];
}

MapOL.prototype.lineStyle = function (line) {
    return new ol.style.Style({
        stroke: new ol.style.Stroke({ color: line.borderColor, width: line.borderSize }),
        text: new ol.style.Text({
            text: line.properties.label,
            placement: "line",
            opacity: 1,
            scale: line.textScale,
            fill: new ol.style.Fill({ color: line.color }),
            stroke: new ol.style.Stroke({ color: line.borderColor, width: line.borderSize })
        }),
    });
}

MapOL.prototype.circleStyle = function (circle) {
    return new ol.style.Style({
        fill: new ol.style.Fill({ color: circle.backgroundColor }),
        stroke: new ol.style.Stroke({ color: circle.borderColor, width: circle.borderSize }),
        text: new ol.style.Text({
            overflow: true,
            text: circle.properties.label,
            placement: "line",
            scale: circle.textScale,
            fill: new ol.style.Fill({ color: circle.color }),
            stroke: new ol.style.Stroke({ color: circle.borderColor, width: circle.borderSize }),
            offsetX: 0,
            offsetY: 0,
            rotation: 0
        }),
    });
}

// --- GeoStyles ------------------------------------------------------------------------//
MapOL.prototype.getGeoStyle = function (feature) {
    var that = this;

    var geoStyles = {
        'Point': that.awesomeStyle(feature),
        'LineString': new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'green',
                width: 1
            })
        }),
        'MultiLineString': new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'green',
                width: 1
            })
        }),
        'MultiPolygon': new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'yellow',
                width: 1
            }),
            fill: new ol.style.Fill({
                color: 'rgba(255, 255, 0, 0.1)'
            })
        }),
        'Polygon': new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'blue',
                lineDash: [4],
                width: 3
            }),
            fill: new ol.style.Fill({
                color: 'rgba(0, 0, 255, 0.1)'
            })
        }),
        'GeometryCollection': new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'magenta',
                width: 2
            }),
            fill: new ol.style.Fill({
                color: 'magenta'
            }),
            image: new ol.style.Circle({
                radius: 10,
                fill: null,
                stroke: new ol.style.Stroke({
                    color: 'magenta'
                })
            })
        }),
        'Circle': new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'red',
                width: 2
            }),
            fill: new ol.style.Fill({
                color: 'rgba(255,0,0,0.2)'
            })
        })
    };

    return geoStyles[feature.getGeometry().getType()];
}

/*
 * Swiss projection transform functions downloaded from
 * https://www.swisstopo.admin.ch/en/knowledge-facts/surveying-geodesy/reference-systems/map-projections.html
 */

// Convert WGS lat/long (° dec) to CH y
MapOL.WGStoLV03y = function WGStoLV03y(lat, lng) {
    // Converts degrees dec to sex
    lat = DECtoSEX(lat);
    lng = DECtoSEX(lng);

    // Converts degrees to seconds (sex)
    lat = DEGtoSEC(lat);
    lng = DEGtoSEC(lng);

    // Axillary values (% Bern)
    const lat_aux = (lat - 169028.66) / 10000;
    const lng_aux = (lng - 26782.5) / 10000;

    // Process Y
    const y =
        600072.37 +
        211455.93 * lng_aux -
        10938.51 * lng_aux * lat_aux -
        0.36 * lng_aux * Math.pow(lat_aux, 2) -
        44.54 * Math.pow(lng_aux, 3);

    return y;
}

// Convert WGS lat/long (° dec) to CH x
MapOL.WGStoLV03x = function WGStoLV03x(lat, lng) {
    // Converts degrees dec to sex
    lat = DECtoSEX(lat);
    lng = DECtoSEX(lng);

    // Converts degrees to seconds (sex)
    lat = DEGtoSEC(lat);
    lng = DEGtoSEC(lng);

    // Axillary values (% Bern)
    const lat_aux = (lat - 169028.66) / 10000;
    const lng_aux = (lng - 26782.5) / 10000;

    // Process X
    const x =
        200147.07 +
        308807.95 * lat_aux +
        3745.25 * Math.pow(lng_aux, 2) +
        76.63 * Math.pow(lat_aux, 2) -
        194.56 * Math.pow(lng_aux, 2) * lat_aux +
        119.79 * Math.pow(lat_aux, 3);

    return x;
}

// Convert CH y/x to WGS lat
MapOL.CHtoLV03lat = function CHtoLV03lat(y, x) {
    // Converts military to civil and to unit = 1000km
    // Axillary values (% Bern)
    const y_aux = (y - 600000) / 1000000;
    const x_aux = (x - 200000) / 1000000;

    // Process lat
    let lat =
        16.9023892 +
        3.238272 * x_aux -
        0.270978 * Math.pow(y_aux, 2) -
        0.002528 * Math.pow(x_aux, 2) -
        0.0447 * Math.pow(y_aux, 2) * x_aux -
        0.014 * Math.pow(x_aux, 3);

    // Unit 10000" to 1 " and converts seconds to degrees (dec)
    lat = (lat * 100) / 36;

    return lat;
}

// Convert CH y/x to WGS long
MapOL.CHtoLV03lng = function CHtoLV03lng(y, x) {
    // Converts military to civil and to unit = 1000km
    // Axillary values (% Bern)
    const y_aux = (y - 600000) / 1000000;
    const x_aux = (x - 200000) / 1000000;

    // Process long
    let lng =
        2.6779094 +
        4.728982 * y_aux +
        0.791484 * y_aux * x_aux +
        0.1306 * y_aux * Math.pow(x_aux, 2) -
        0.0436 * Math.pow(y_aux, 3);

    // Unit 10000" to 1 " and converts seconds to degrees (dec)
    lng = (lng * 100) / 36;

    return lng;
}

// Convert DEC angle to SEX DMS
MapOL.DECtoSEX = function DECtoSEX(angle) {
    // Extract DMS
    const deg = parseInt(angle, 10);
    const min = parseInt((angle - deg) * 60, 10);
    const sec = ((angle - deg) * 60 - min) * 60;

    // Result in degrees sex (dd.mmss)
    return deg + min / 100 + sec / 10000;
}

// Convert Degrees angle to seconds
MapOL.DEGtoSEC = function DEGtoSEC(angle) {
    // Extract DMS
    const deg = parseInt(angle, 10);
    let min = parseInt((angle - deg) * 100, 10);
    let sec = ((angle - deg) * 100 - min) * 100;

    // Avoid rounding problems with seconds=0
    const parts = String(angle).split('.');
    if (parts.length == 2 && parts[1].length == 2) {
        min = Number(parts[1]);
        sec = 0;
    }

    // Result in degrees sex (dd.mmss)
    return sec + min * 60 + deg * 3600;
}

// LV95 / port from https://github.com/perron2/lv95/blob/master/lib/lv95.dart
MapOL.WGStoLV95y = function WGStoLV95y(lat, lng) {
    const phi = (lat * 3600 - 169028.66) / 10000;
    const phi2 = phi * phi;

    const lambda = (lng * 3600 - 26782.5) / 10000;
    const lambda2 = lambda * lambda;
    const lambda3 = lambda2 * lambda;

    const y = 2600072.37 +
        211455.93 * lambda -
        10938.51 * lambda * phi -
        0.36 * lambda * phi2 -
        44.54 * lambda3;

    return y;
}

// Convert WGS lat/long (° dec) to CH x
MapOL.WGStoLV95x = function WGStoLV95x(lat, lng) {
    const phi = (lat * 3600 - 169028.66) / 10000;
    const phi2 = phi * phi;
    const phi3 = phi2 * phi;

    const lambda = (lng * 3600 - 26782.5) / 10000;
    const lambda2 = lambda * lambda;

    const x = 1200147.07 +
        308807.95 * phi +
        3745.25 * lambda2 +
        76.63 * phi2 -
        194.56 * lambda2 * phi +
        119.79 * phi3;

    return x;
}

// Convert CH y/x to WGS lat
MapOL.LV95toWGSlat = function LV95toWGSlat(y, x) {
    const x1 = (x - 1200000) / 1000000;
    const x2 = x1 * x1;
    const x3 = x2 * x1;

    const y1 = (y - 2600000) / 1000000;
    const y2 = y1 * y1;

    // Calculate latitude
    const latitude = 16.9023892 +
        3.238272 * x1 -
        0.270978 * y2 -
        0.002528 * x2 -
        0.0447 * y2 * x1 -
        0.0140 * x3;

    // Convert to degrees
    return (latitude * 100 / 36);
}

// Convert CH y/x to WGS long
MapOL.LV95toWGSlng = function LV95toWGSlng(y, x) {
    const x1 = (x - 1200000) / 1000000;
    const x2 = x1 * x1;

    const y1 = (y - 2600000) / 1000000;
    const y2 = y1 * y1;
    const y3 = y2 * y1;

    // Calculate longitude
    const longitude = 2.6779094 +
        4.728982 * y1 +
        0.791484 * y1 * x1 +
        0.1306 * y1 * x2 -
        0.0436 * y3;

    // Convert to degrees
    return (longitude * 100 / 36);
}

