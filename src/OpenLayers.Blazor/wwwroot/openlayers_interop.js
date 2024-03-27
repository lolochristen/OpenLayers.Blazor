var _MapOL = new Array();

export function MapOLInit(mapId, popupId, options, center, zoom, rotation, interactions, markers, shapes, layers, instance) {
    _MapOL[mapId] = new MapOL(mapId, popupId, options, center, zoom, rotation, interactions, markers, shapes, layers, instance);
}

export function MapOLDispose(mapId) {
    _MapOL[mapId] = undefined;
}

export function MapOLCenter(mapId, point) {
    _MapOL[mapId].setCenter(point);
}

export function MapOLRotate(mapId, rotation) {
    _MapOL[mapId].setRotation(rotation);
}

export function MapOLZoom(mapId, zoom) {
    _MapOL[mapId].setZoom(zoom);
}

export function MapOLSetOptions(mapId, options) {
    _MapOL[mapId].setOptions(options);
}

export function MapOLLoadGeoJson(mapId, json, dataProjection, raiseEvents) {
    _MapOL[mapId].loadGeoJson(json, dataProjection, raiseEvents);
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

export function MapOLSetVisibleExtent(mapId, extent) {
    _MapOL[mapId].setVisibleExtent(extent);
}

export function MapOLSetDrawingSettings(mapId, enableNewShapes, enableEditShapes, enableShapeSnap, geometryType, freehand) {
    _MapOL[mapId].setDrawingSettings(enableNewShapes, enableEditShapes, enableShapeSnap, geometryType, freehand);
}

export function MapOLUndoDrawing(mapId) {
    _MapOL[mapId].undoDrawing();
}

export function MapOLUpdateShape(mapId, shape) {
    _MapOL[mapId].updateShape(shape);
}

export function MapOLRemoveShape(mapId, shape) {
    _MapOL[mapId].removeShape(shape);
}

export function MapOLAddShape(mapId, shape) {
    _MapOL[mapId].addShape(shape);
}

export function MapOLGetCoordinates(mapId, shapeId) {
    return _MapOL[mapId].getCoordinates(shapeId);
}

export function MapOLSetInteractions(mapId, active) {
    _MapOL[mapId].setInteractions(active);
}


// --- MapOL ----------------------------------------------------------------------------//

function MapOL(mapId, popupId, options, center, zoom, rotation, interactions, markers, shapes, layers, instance) {
    this.Instance = instance;
    this.Options = options;

    //LV03
    const projectionLV03 = new ol.proj.Projection({
        code: "EPSG:21781",
        extent: [485869.5728, 76443.1884, 837076.5648, 299941.7864],
        units: "m"
    });
    ol.proj.addProjection(projectionLV03);

    // LV95
    const projectionLV95 = new ol.proj.Projection({
        code: "EPSG:2056",
        extent: [2485071.58, 1074261.72, 2837119.8, 1299941.79],
        units: "m"
    });
    ol.proj.addProjection(projectionLV95);

    ol.proj.addCoordinateTransforms(
        "EPSG:4326",
        projectionLV03,
        function (coordinate) {
            return [
                MapOL.WGStoLV03y(coordinate[1], coordinate[0]),
                MapOL.WGStoLV03x(coordinate[1], coordinate[0])
            ];
        },
        function (coordinate) {
            return [
                MapOL.CHtoLV03lng(coordinate[0], coordinate[1]),
                MapOL.CHtoLV03lat(coordinate[0], coordinate[1])
            ];
        }
    );

    ol.proj.addCoordinateTransforms(
        "EPSG:4326",
        projectionLV95,
        function (coordinate) {
            return [
                MapOL.WGStoLV95y(coordinate[1], coordinate[0]),
                MapOL.WGStoLV95x(coordinate[1], coordinate[0])
            ];
        },
        function (coordinate) {
            return [
                MapOL.LV95toWGSlng(coordinate[0], coordinate[1]),
                MapOL.LV95toWGSlat(coordinate[0], coordinate[1])
            ];
        }
    );

    if (!this.Options.coordinatesProjection)
        this.Options.coordinatesProjection = "EPSG:4326"; // default coordinates

    const ollayers = this.prepareLayers(layers);

    let viewProjection = (ollayers.length > 0) ? ollayers[0].getSource().getProjection() : undefined;
    let viewExtent = (ollayers.length > 0) ? ollayers[0].getExtent() : undefined;
    let viewCenter = (center && center) ? center : undefined;

    if (this.Options.viewProjection) {
        viewProjection = this.Options.viewProjection;
    } else {
        if (!viewProjection) {
            if (this.Options.coordinatesProjection == "EPSG:2056") {
                viewProjection = projectionLV95;
            } else if (this.Options.coordinatesProjection == "EPSG:21781") {
                viewProjection = projectionLV03;
            } else {
                viewProjection = "EPSG:3857"; // default view
            }
        }
    }

    if (viewCenter)
        viewCenter = ol.proj.transform(viewCenter, this.Options.coordinatesProjection, viewProjection);

    this.Map = new ol.Map({
        layers: ollayers,
        target: mapId,
        controls: [],
        view: new ol.View({
            projection: viewProjection,
            center: viewCenter,
            extent: viewExtent,
            zoom: zoom,
            rotation: rotation
        }),
        interactions: ol.interaction.defaults.defaults().extend([new ol.interaction.DragRotateAndZoom()])
    });

    if (this.Options.zoomControl) this.Map.addControl(new ol.control.Zoom())
    if (this.Options.attributionControl) this.Map.addControl(new ol.control.Attribution())
    if (this.Options.fullScreenControl) this.Map.addControl(new ol.control.FullScreen())
    if (this.Options.zoomSliderControl) this.Map.addControl(new ol.control.ZoomSlider())
    if (this.Options.rotateControl) this.Map.addControl(new ol.control.Rotate())
    if (this.Options.scaleLineUnit != "none") {
        this.Map.addControl(new ol.control.ScaleLine({
            units: this.Options.scaleLineUnit.toLowerCase()}));
    }
    if (this.Options.overviewMap) {
        this.Map.addControl(new ol.control.OverviewMap({
            layers: [new ol.layer.Tile(layers[0])]
        }))
    }
    if (this.Options.zoomToExtentControl) this.Map.addControl(new ol.control.ZoomToExtent())

    var that = this;
    var geoSource = new ol.source.Vector();
    geoSource.on("removefeature", function (evt) { that.onFeatureRemoved(evt.feature); });
    geoSource.on("addfeature", function (evt) { that.onFeatureAdded(evt.feature); });
    geoSource.on("changefeature", function (evt) { that.onFeatureChanged(evt.feature); });


    this.Geometries = new ol.layer.Vector({
        source: geoSource,
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
        positioning: "bottom-center",
        stopEvent: false,
        offset: [0, -50]
    });

    this.Map.addOverlay(popup);

    this.Map.on("click", function (evt) { that.onMapClick(evt, popup, popupElement) });
    this.Map.on("pointermove", function (evt) { that.onMapPointerMove(evt, popupElement) });
    this.Map.on("rendercomplete", function (evt) { that.Instance.invokeMethodAsync("OnInternalRenderComplete"); });
    this.Map.getView().on("change:resolution", function (evt) { that.onMapResolutionChanged(); });
    this.Map.getView().on("change:center", function (evt) { that.onMapCenterChanged(); });
    this.Map.getView().on("change:rotation", function (evt) { that.onMapRotationChanged(); });

    this.setInteractions(interactions);

    this.setMarkers(markers);
    this.setShapes(shapes);

    this.onMapCenterChanged();
}

MapOL.prototype.prepareLayers = function (layers) {
    const ollayers = new Array();
    let that = this;

    layers.forEach((l, i, arr) => {

        let source;
        let sourceType = l.source.sourceType;
        l = MapOL.transformNullToUndefined(l);

        if (l.extent && this.Options.coordinatesProjection) {
            let projection = l.source.projection ?? "EPSG:3857";
            l.extent = ol.proj.transformExtent(l.extent, that.Options.coordinatesProjection, projection);
        }

        switch (sourceType) {
            case "TileImage":
                source = new ol.source.TileImage(l.source);
                break;
            case "BingMaps":
                source = new ol.source.BingMaps(l.source);
                break;
            case "OGCMapTile":
                source = new ol.source.OGCMapTile(l.source);
                break;
            case "TileArcGISRest":
                source = new ol.source.TileArcGISRest(l.source);
                break;
            case "TileJSON":
                source = new ol.source.TileJSON(l.source);
                break;
            case "TileWMS":
                source = new ol.source.TileWMS(l.source);
                break;
            case "WMTS":
                source = new ol.source.WMTS(l.source);
                break;
            case "Zoomify":
                source = new ol.source.Zoomify(l.source);
                break;
            case "OSM":
                source = new ol.source.OSM(l.source);
                break;
            case "XYZ":
                source = new ol.source.XYZ(l.source);
                break;
            case "CartoDB":
                source = new ol.source.CartoDB(l.source);
                break;
            case "Stamen":
                source = new ol.source.Stamen(l.source);
                break;
            case "StadiaMaps":
                source = new ol.source.StadiaMaps(l.source);
                break;
            case "TileDebug":
                source = new ol.source.TileDebug(l.source);
                break;
            case "VectorKML":
                l.source.format = new ol.format.KML(l.source.formatOptions);
                source = new ol.source.Vector(l.source);
                break;
            case "VectorEsriJson":
                l.source.format = new ol.format.EsriJSON(l.source.formatOptions);
                source = new ol.source.Vector(l.source);
                break;
            case "VectorGeoJson":
                l.source.format = new ol.format.GeoJSON(l.source.formatOptions);
                source = new ol.source.Vector(l.source);
                break;
            case "VectorTopoJson":
                l.source.format = new ol.format.TopoJson(l.source.formatOptions);
                source = new ol.source.Vector(l.source);
                break;
            case "VectorMVT":
                l.source.format = new ol.format.MVT(l.source.formatOptions);
                source = new ol.source.Vector(l.source);
                break;
            case "VectorIGC":
                l.source.format = new ol.format.IGC(l.source.formatOptions);
                source = new ol.source.Vector(l.source);
                break;
            case "VectorPolyline":
                l.source.format = new ol.format.Polyline(l.source.formatOptions);
                source = new ol.source.Vector(l.source);
                break;
            case "VectorWKT":
                l.source.format = new ol.format.WKT(l.source.formatOptions);
                source = new ol.source.Vector(l.source);
                break;
            case "VectorWKB":
                l.source.format = new ol.format.WKB(l.source.formatOptions);
                source = new ol.source.Vector(l.source);
                break;
            case "VectorGML2":
                l.source.format = new ol.format.GML2(l.source.formatOptions);
                source = new ol.source.Vector(l.source);
                break;
            case "VectorGML3":
                l.source.format = new ol.format.GML3(l.source.formatOptions);
                source = new ol.source.Vector(l.source);
                break;
            case "VectorGPX":
                l.source.format = new ol.format.GPX(l.source.formatOptions);
                source = new ol.source.Vector(l.source);
                break;
            case "VectorOSMXML":
                l.source.format = new ol.format.OSMXML(s);
                source = new ol.source.Vector(l.source);
                break;
            case "VectorWFS":
                l.source.format = new ol.format.WFS(l.source.formatOptions);
                source = new ol.source.Vector(l.source);
                break;
            case "Graticule":
                source = l.source.formatOptions;
                if (source == undefined) {
                    source = {
                        showLabels: true,
                        wrapX: false
                    };
                }
                break;
        }

        l.source = source;
        if (source instanceof ol.source.Vector)
            ollayers.push(new ol.layer.Vector(l));
        else if (sourceType == "Graticule")
            ollayers.push(new ol.layer.Graticule(source));
        else
            ollayers.push(new ol.layer.Tile(l));
    });

    return ollayers;
};

MapOL.prototype.setLayers = function (layers) {
    this.Map.setLayers(this.prepareLayers(layers));
};

MapOL.prototype.removeLayer = function (layer) {
    this.Map.getAllLayers().forEach((l) => {
        try {
            const source = l.getSource();
            if (source.urls[0] == layer.source.url) {
                this.Map.removeLayer(l);
            }
        } catch (e) {
        }
    });
};

MapOL.prototype.addLayer = function (layer) {
    const ollayers = this.prepareLayers([layer]);
    this.Map.addLayer(ollayers[0]);
};

MapOL.prototype.updateLayer = function (layer) {
    const ollayers = this.prepareLayers([layer]);
    const olayer = this.findLayer(ollayers[0]);
    if (olayer != undefined) {
        olayer.setVisible(layer.visibility);
        olayer.setOpacity(layer.opacity);
        olayer.setZIndex(layer.zindex);
        olayer.setExtent(layer.extent);
    }
};

MapOL.prototype.findLayer = function (layer) {
    let foundLayer = undefined;
    this.Map.getAllLayers().forEach((l) => {
        try {
            const source = l.getSource();
            const layerSource = layer.getSource();
            if (source.urls[0] == layerSource.urls[0] && source.getKey() == layerSource.getKey()) {
                foundLayer = l;
            }
        }
        catch (ex) {
        }
    });
    return foundLayer;
};

MapOL.prototype.setMarkers = function (markers) {
    var source = this.Markers.getSource();
    source.clear();
    if (markers) {
        markers.forEach((marker) => {
            var feature = this.mapShapeToFeature(marker);
            source.addFeature(feature);
        });
    }
};

MapOL.prototype.setShapes = function (shapes) {
    var source = this.Geometries.getSource();
    source.clear();
    if (shapes) {
        shapes.forEach((shape) => {
            var feature = this.mapShapeToFeature(shape);
            source.addFeature(feature);
        });
    }
};

MapOL.prototype.loadGeoJson = function (json, dataProjection, raiseEvents) {
    var that = this;

    if (this.GeoLayer) {
        const source = this.GeoLayer.getSource();
        source.clear();
    }

    if (!json) return;

    const features = (new ol.format.GeoJSON()).readFeatures(json,
        { featureProjection: this.Options.coordinatesProjection, dataProjection: dataProjection });

    const geoSource = new ol.source.Vector({
        features: features
    });

    if (this.GeoLayer) {
        this.GeoLayer.setSource(geoSource);
    } else {
        this.GeoLayer = new ol.layer.Vector({
            source: geoSource,
            style: (feature) => that.getShapeStyle(feature) // needs to be sync
        });

        this.Map.addLayer(this.GeoLayer);
    }

    if (raiseEvents) features.forEach((f, i, arr) => { that.onFeatureAdded(f) });
};

MapOL.prototype.setZoom = function (zoom) {
    this.Map.getView().setZoom(zoom);
};

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
};

MapOL.prototype.setCenter = function (point) {
    this.Map.getView().setCenter(ol.proj.transform(point,
        this.Options.coordinatesProjection,
        this.Map.getView().getProjection()));
};

MapOL.prototype.setRotation = function (rotation) {
    this.Map.getView().setRotation(rotation);
};

MapOL.prototype.setOptions = function (options) {
    this.Options = options;
};

MapOL.prototype.setInteractions = function (active) {
    this.Map.getInteractions().forEach((interaction, i, arr) => {
        interaction.setActive(active);
    });
};

MapOL.prototype.getReducedFeature = function (feature) {
    const type = feature.getGeometry().getType();

    const objectWithoutKey = (object, key) => {
        const { [key]: deletedKey, ...otherKeys } = object;
        return otherKeys;
    };

    const properties = objectWithoutKey(feature.getProperties(), "geometry");

    const reduced = {
        type: "Feature",
        geometry: {
            type: feature.getGeometry().getType(),
            coordinates: feature.getGeometry().getCoordinates()
        },
        properties: properties
    };

    return reduced;
};

MapOL.prototype.onMapClick = function (evt, popup, element) {
    popup.setPosition(0, 0);

    var that = this;
    var invokeMethod = true;

    this.Map.forEachFeatureAtPixel(evt.pixel,
        function (feature) {

            const shape = that.mapFeatureToShape(feature);

            if (shape) {
                that.Instance.invokeMethodAsync("OnInternalFeatureClick", shape); // ?

                if (shape.properties.type == "Marker") {
                    invokeMethod = false;
                    that.Instance.invokeMethodAsync("OnInternalMarkerClick", shape);
                } else {
                    invokeMethod = false;
                    that.Instance.invokeMethodAsync("OnInternalShapeClick", shape);
                }
            }

            var showPopup = false;

            if (shape) {
                showPopup = shape.properties.popup;
            } 
            if (showPopup == undefined) {
                showPopup = that.Options.autoPopup;
            }

            if (showPopup) {
                const coordinates = feature.getGeometry().getCoordinates();
                popup.setPosition(coordinates);
            }

        });

    if (invokeMethod) {
        invokeMethod = false;
        const coordinate = ol.proj.transform(evt.coordinate, this.Map.getView().getProjection(), this.Options.coordinatesProjection);
        this.Instance.invokeMethodAsync("OnInternalClick", coordinate);
    }
};

MapOL.prototype.onMapPointerMove = function (evt, element) {
    if (evt.dragging || Number.isNaN(evt.coordinate[0])) {
        return;
    }
    const coordinate = ol.proj.transform(evt.coordinate,
        this.Map.getView().getProjection(),
        this.Options.coordinatesProjection);
    this.Instance.invokeMethodAsync("OnInternalPointerMove", coordinate);
};

MapOL.prototype.onMapResolutionChanged = function () {
    this.Instance.invokeMethodAsync("OnInternalZoomChanged", this.Map.getView().getZoom());
    this.onVisibleExtentChanged();
};
MapOL.prototype.onMapCenterChanged = function () {
    const center = this.Map.getView().getCenter();
    if (!center) return;
    const coordinate = ol.proj.transform(center,
        this.Map.getView().getProjection(),
        this.Options.coordinatesProjection);
    this.Instance.invokeMethodAsync("OnInternalCenterChanged", coordinate);
    this.onVisibleExtentChanged();
};

MapOL.prototype.onMapRotationChanged = function () {
    const rotation = this.Map.getView().getRotation();
    if (!rotation) return;
    this.Instance.invokeMethodAsync("OnInternalRotationChanged", rotation);
};

MapOL.prototype.onVisibleExtentChanged = function () {
    if (this.disableVisibleExtentChanged) return;
    const extentArray = this.Map.getView().calculateExtent(this.Map.getSize());
    const coordinate1 = ol.proj.transform([extentArray[0], extentArray[1]],
        this.Map.getView().getProjection(),
        this.Options.coordinatesProjection);
    const coordinate2 = ol.proj.transform([extentArray[2], extentArray[3]],
        this.Map.getView().getProjection(),
        this.Options.coordinatesProjection);
    const extent = { X1: coordinate1[0], Y1: coordinate1[1], X2: coordinate2[0], Y2: coordinate2[1] };
    this.Instance.invokeMethodAsync("OnInternalVisibleExtentChanged", extent);
};

MapOL.prototype.centerToCurrentGeoLocation = function () {
    var that = this;
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            const projection = that.Map.getView().getProjection().getCode();
            that.Map.getView().setCenter(ol.proj.transform([position.coords.longitude, position.coords.latitude],
                "EPSG:4326",
                projection));
        });
    }
};

MapOL.prototype.getCurrentGeoLocation = function () {
    var that = this;
    return new Promise((resolve, reject) => {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                const projection = that.Map.getView().getProjection().getCode();
                const coordinate = ol.proj.transform([position.coords.longitude, position.coords.latitude],
                    "EPSG:4326",
                    projection);
                resolve(coordinate);
            });
        } else {
            reject("No geolocation received");
        };
    });
};

MapOL.prototype.disableVisibleExtentChranged = false;

MapOL.prototype.setVisibleExtent = function (extent) {
    this.disableVisibleExtentChanged = true;
    this.Map.getView().fit(new Array(extent.x1, extent.y1, extent.x2, extent.y2), this.Map.getSize());
    this.disableVisibleExtentChanged = false;
};

MapOL.prototype.currentDraw = null;
MapOL.prototype.currentSnap = null;
MapOL.prototype.currentModify = null;
MapOL.prototype.setDrawingSettings = function (enableNewShapes, enableEditShapes, enableShapeSnap, geometryType, freehand) {
    var that = this;
    this.removeDrawingInteractions();

    const source = this.Geometries.getSource();

    if (enableEditShapes) {
        if (this.currentModify == null) {
            this.currentModify = new ol.interaction.Modify({ source: source });
            this.Map.addInteraction(this.currentModify);
        }
    }

    if (enableNewShapes) {
        this.currentDraw = new ol.interaction.Draw({
            source: source,
            type: geometryType,
            freehand: freehand
        });
        this.currentDraw.on("drawend",
            function(evt) {
                that.getShapeStyleAsync(evt.feature)
                    .then(style => evt.feature.setStyle(style));
            });

        this.Map.addInteraction(this.currentDraw);
    }

    if (enableShapeSnap) {
        this.currentSnap = new ol.interaction.Snap({ source: source });
        this.Map.addInteraction(this.currentSnap);
    }
};

MapOL.prototype.removeDrawingInteractions = function () {
    if (this.currentDraw) {
        this.Map.removeInteraction(this.currentDraw);
        this.currentDraw = null;
    }
    if (this.currentSnap) {
        this.Map.removeInteraction(this.currentSnap);
        this.currentSnap = null;
    }
    if (this.currentModify) {
        this.Map.removeInteraction(this.currentModify);
        this.currentModify = null;
    }
};

MapOL.prototype.undoDrawing = function () {
    if (this.currentDraw) {
        this.currentDraw.removeLastPoint();
    }
};

MapOL.prototype.onFeatureAdded = function (feature) {
    const shape = this.mapFeatureToShape(feature);
    this.Instance.invokeMethodAsync("OnInternalShapeAdded", shape);
};

MapOL.prototype.mapFeatureToShape = function (feature) {

    if (feature == null) return null;

    var geometry = feature.getGeometry();
    var viewProjection = this.Map.getView().getProjection();
    var coordinates = null;

    if (geometry != null && !Array.isArray(geometry)) {
        switch (geometry.getType()) {
            case "Circle":
                coordinates = ol.proj.transform(geometry.getCenter(), viewProjection, this.Options.coordinatesProjection);
                break;
            default:
                var g = geometry.getCoordinates();
                var l = g.length;
                if (Array.isArray(g[0])) g.forEach((g2) => l = l + g2.length);
                if (l < this.Options.serializationCoordinatesLimit)
                    coordinates = MapOL.transformCoordinates(geometry.getCoordinates(),
                        viewProjection,
                        this.Options.coordinatesProjection);
                break;
        }
    }

    var style = feature.getStyle();
    var stroke = style && !Array.isArray(style) ? style.getStroke() : null;
    var fill = style && !Array.isArray(style) ? style.getFill() : null;
    var text = style && !Array.isArray(style) ? style.getText() : null;
    var id = feature.getId();

    if (id == null) {
        id = self.crypto.randomUUID();
        feature.setId(id);
    }

    var objectWithoutKey = (object, key) => {
        const { [key]: deletedKey, ...otherKeys } = object;
        return otherKeys;
    };
    var properties = objectWithoutKey(feature.getProperties(), "geometry");

    if (!properties.type) properties.type = "Shape";

    var shape = {
        id: id,
        geometryType: geometry ? geometry.getType() : "None",
        coordinates: coordinates,
        properties: properties,
        borderColor: stroke ? stroke.getColor() : null,
        borderSize: stroke ? stroke.getWidth() : null,
        backgroundColor: fill ? fill.getColor() : null,
        color: text && text.getStroke() ? text.getStroke().getColor() : null,
        textScale: text ? text.getScale() : null
    };

    if (geometry.getType() == "Circle") {
        shape.radius = geometry.getRadius();
    }

    if (style && !Array.isArray(style)) {
        var imageStyle = style.getImage();
        if (imageStyle) {
            shape.scale = imageStyle.getScale();
        }
    }

    return shape;
};

MapOL.prototype.mapShapeToFeature = function (shape) {

    var geometry;
    const viewProjection = this.Map.getView().getProjection();
    if (shape.coordinates) {
        var coordinates = MapOL.transformCoordinates(shape.coordinates, this.Options.coordinatesProjection, viewProjection);
        switch (shape.geometryType) {
            case "Point":
                geometry = new ol.geom.Point(coordinates);
                break;
            case "LineString":
                geometry = new ol.geom.LineString(coordinates);
                break;
            case "Polygon":
                geometry = new ol.geom.Polygon(coordinates);
                break;
            case "Circle":
                geometry = new ol.geom.Circle(coordinates,
                    shape.radius / ol.proj.getPointResolution(viewProjection, 1, coordinates));
                break;
            case "MultiPoint":
                geometry = new ol.geom.MultiPoint(coordinates);
                break;
            case "MultiLineString":
                geometry = new ol.geom.MultiLineString(coordinates);
                break;
            case "MultiPolygon":
                geometry = new ol.geom.MultiPolygon(coordinates);
                break;
        }
    }
    const feature = new ol.Feature({
        type: shape.properties.type,
        popup: shape.properties.popup,
        title: shape.properties.title,
        label: shape.properties.label,
        content: shape.properties.content,
        style: shape.properties.style,
    });

    if (geometry) {
        feature.setGeometry(geometry);
    }

    feature.setId(shape.id);

    var style;
    switch (shape.properties.style) {
        case "MarkerPin":
            style = this.pinStyle(shape);
            break;

        case "MarkerFlag":
            style = this.flagStyle(shape);
            break;

        case "MarkerAwesome":
            style = this.awesomeStyle(shape);
            break;

        case "MarkerCustomImage":
            style = this.customImageStyle(shape);
            break;

        default:
            style = this.getDefaultStyle(shape);
            break;
    }

    feature.setStyle(style);
    return feature;
};

MapOL.prototype.getDefaultStyle = function (shape) {

    if (shape.geometryType == "Point") {
        const viewProjection = this.Map.getView().getProjection();
        const coordinates = ol.proj.transform(shape.coordinates ?? this.Map.getView().getCenter(),
            this.Options.coordinatesProjection,
            viewProjection);
        let radius = 5;
        if (shape.radius != null) {
            radius = shape.radius;
        }
        if (coordinates.length > 0) {
            radius = radius / ol.proj.getPointResolution(viewProjection, 1, coordinates);
        }
        return new ol.style.Style({
            image: new ol.style.Circle({
                radius: radius,
                fill: new ol.style.Fill({
                    color: shape.backgroundColor,
                }),
                stroke: new ol.style.Stroke({
                    color: shape.borderColor,
                    width: shape.borderSize
                })
            }),
            zIndex: Infinity
        });
    } else {
        return new ol.style.Style({
            fill: shape.backgroundColor ? new ol.style.Fill({ color: shape.backgroundColor }) : null,
            stroke: new ol.style.Stroke({ color: shape.borderColor, width: shape.borderSize }),
            text: shape.label
                ? new ol.style.Text({
                    overflow: true,
                    text: shape.label,
                    placement: "line",
                    scale: shape.textScale,
                    fill: new ol.style.Fill({ color: shape.color }),
                    stroke: new ol.style.Stroke({ color: shape.color, width: shape.color }),
                    offsetX: 0,
                    offsetY: 0,
                    rotation: 0
                })
                : null,
        });
    }
};

MapOL.prototype.onFeatureRemoved = function (feature) {

};

MapOL.prototype.onFeatureChanged = function (feature) {

    if (feature.getProperties().type != "Marker") {
        const shape = this.mapFeatureToShape(feature);
        this.Instance.invokeMethodAsync("OnInternalShapeChanged", shape);
    }
};

MapOL.prototype.updateShape = function (shape) {

    var feature;
    if (shape.properties.type == "Marker") {
        feature = this.Markers.getSource().getFeatureById(shape.id);
    } else {
        feature = this.Geometries.getSource().getFeatureById(shape.id);
        if (!feature && this.GeoLayer) feature = this.GeoLayer.getSource().getFeatureById(shape.id);
    }

    if (feature) {
        const newFeature = this.mapShapeToFeature(shape);
        const geometry = newFeature.getGeometry();
        if (geometry)
            feature.setGeometry(geometry);
        feature.setStyle(newFeature.getStyle());
    }
};

MapOL.prototype.removeShape = function (shape) {
    var source;
    if (shape.properties.type == "Marker")
        source = this.Markers.getSource();
    else
        source = this.Geometries.getSource();
    var feature = source.getFeatureById(shape.id);
    if (feature) {
        source.removeFeature(feature);
    }
};

MapOL.prototype.addShape = function (shape) {
    var source;
    if (shape.properties.type == "Marker")
        source = this.Markers.getSource();
    else
        source = this.Geometries.getSource();
    var feature = this.mapShapeToFeature(shape);
    source.addFeature(feature);
};

MapOL.prototype.getCoordinates = function(featureId) {
    var feature = this.Geometries.getSource().getFeatureById(featureId);
    if (feature) {
        var coord = feature.getGeometry().getCoordinates();
        if (!coord)
            coord = feature.getGeometry().getCenter();
        return coord;
    }
    return null;
}

//--- Styles -----------------------------------------------------------------//

MapOL.prototype.pinStyle = function (marker) {
    var src = "./_content/OpenLayers.Blazor/img/marker-pin-red.png";
    if (marker.color == null || marker.color != "#FFFFFF")
        src = `./_content/OpenLayers.Blazor/img/marker-pin-${marker.color}.png`;
    return [
        new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0, 60],
                size: [160, 60],
                offset: [0, 0],
                opacity: 1,
                scale: marker.scale,
                anchorXUnits: "pixels",
                anchorYUnits: "pixels",
                src: "./_content/OpenLayers.Blazor/img/pin-back.png"
            })
        }),
        new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [100, 198],
                size: [200, 200],
                offset: [0, 0],
                opacity: 1,
                scale: marker.scale,
                anchorXUnits: "pixels",
                anchorYUnits: "pixels",
                src: src
            })
        })
    ];
};

MapOL.prototype.flagStyle = function (marker) {
    const padTop = 4;
    const padBottom = 2;
    const padLeft = 5;
    const padRight = 5;

    const size = 10;
    const width = size;
    const height = size;

    const canvas = document.createElement("canvas");
    const ctx = canvas.getContext("2d");

    const context = ol.render.toContext(canvas.getContext("2d"),
        {
            size: [width, height],
            pixelRatio: 1
        });

    const symbol = [
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
            anchorXUnits: "pixels",
            anchorYUnits: "pixels",
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
};

MapOL.prototype.awesomeStyle = function (marker) {
    return [
        new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0, 14],
                size: [56, 21],
                offset: [0, 0],
                opacity: 1,
                scale: 0.5,
                color: marker.color ?? this.Options.color,
                anchorXUnits: "pixels",
                anchorYUnits: "pixels",
                src: "./_content/OpenLayers.Blazor/img/pin-back.png"
            }),
        }),
        new ol.style.Style({
            text: new ol.style.Text({
                text: String.fromCodePoint(0xF041), // Map Marker
                scale: 2,
                font: '900 18px "Font Awesome 6 Free"',
                textBaseline: "bottom",
                fill: new ol.style.Fill({ color: marker.backgroundColor ?? this.Options.backgroundColor }),
                stroke: new ol.style.Stroke({ color: marker.borderColor ?? this.Options.borderColor, width: 3 })
            })
        }),
        new ol.style.Style({
            text: new ol.style.Text({
                text: marker.properties?.label ?? this.Options.label,
                offsetY: -22,
                opacity: 1,
                scale: 1,
                font: '900 18px "Font Awesome 6 Free"',
                fill: new ol.style.Fill({ color: marker.color ?? this.Options.color })
            })
        })
    ];
};

MapOL.prototype.customImageStyle = function (marker) {
    return [
        new ol.style.Style({
            image: new ol.style.Icon({
                anchor: marker.anchor,
                size: marker.size,
                offset: [0, 0],
                opacity: 1,
                scale: marker.scale,
                rotation: marker.rotation,
                anchorXUnits: "pixels",
                anchorYUnits: "pixels",
                src: marker.properties["content"]
            })
        })
    ];
};

// Shape Style
MapOL.prototype.getShapeStyleAsync = async function(feature) {
    const shape = this.mapFeatureToShape(feature);
    var style = await this.Instance.invokeMethodAsync("OnGetShapeStyleAsync", shape);
    return this.mapStyleOptionsToStyle(style, feature);
}
MapOL.prototype.getShapeStyle = function(feature) {
    const shape = this.mapFeatureToShape(feature);
    var style = this.Instance.invokeMethod("OnGetShapeStyle", shape);
    return this.mapStyleOptionsToStyle(style, feature);
}

MapOL.prototype.mapStyleOptionsToStyle = function(style, feature) {

    style = MapOL.transformNullToUndefined(style);

    if (feature.getGeometry().getType() == "Point") {
        if (style.circle == undefined) {
            style.circle = {
                radius: 5,
                fill: style.fill,
                stroke: style.stroke
            };
        }
    }

    if (style.circle) {
        if (style.circle.fill) style.circle.fill = new ol.style.Fill(style.circle.fill);
        if (style.circle.stroke) style.circle.stroke = new ol.style.Stroke(style.circle.stroke);
    }

    if (style.text) {
        if (style.text.fill) style.text.fill = new ol.style.Fill(style.text.fill);
        if (style.text.stroke) style.text.stroke = new ol.style.Stroke(style.text.stroke);
        if (style.text.backgroundFill) style.text.backgroundFill = new ol.style.Fill(style.text.backgroundFill);
        if (style.text.backgroundStroke) style.text.backgroundStroke = new ol.style.Stroke(style.text.backgroundStroke);
    }

    const styleObject = new ol.style.Style({
        stroke: style.stroke ? new ol.style.Stroke(style.stroke) : undefined,
        fill: style.fill ? new ol.style.Fill(style.fill) : undefined,
        text: style.text ? new ol.style.Text(style.text) : undefined,
        image: style.circle ? new ol.style.Circle(style.circle) : style.icon ? new ol.style.Icon(style.icon) : undefined
    });

    return styleObject;
};

MapOL.transformNullToUndefined = function transformNullToUndefined(obj) {
    for (const key in obj) {
        if (obj.hasOwnProperty(key) && obj[key] === null) {
            obj[key] = undefined;
        } else if (typeof obj[key] === "object") {
            transformNullToUndefined(obj[key]);
        }
    }
    return obj;
};

MapOL.transformCoordinates = function(coordinates, source, destination) {
    var newCoordinates;
    if (source === destination)
        return coordinates;
    if (Array.isArray(coordinates) && Array.isArray(coordinates[0])) {
        newCoordinates = Array(coordinates.length);
        for (var i = 0; i < coordinates.length; i++) {

            if (Array.isArray(coordinates[i][0])) {
                newCoordinates[i] = Array(coordinates[i].length);
                for (var i2 = 0; i2 < coordinates[i].length; i2++) {
                    newCoordinates[i][i2] = ol.proj.transform(coordinates[i][i2], source, destination);
                }
            } else {
                newCoordinates[i] = ol.proj.transform(coordinates[i], source, destination);
            }
        }
    } else {
        newCoordinates = ol.proj.transform(coordinates, source, destination);
    }
    return newCoordinates;
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
};

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
};

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
};

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
};

// Convert DEC angle to SEX DMS
MapOL.DECtoSEX = function DECtoSEX(angle) {
    // Extract DMS
    const deg = parseInt(angle, 10);
    const min = parseInt((angle - deg) * 60, 10);
    const sec = ((angle - deg) * 60 - min) * 60;

    // Result in degrees sex (dd.mmss)
    return deg + min / 100 + sec / 10000;
};

// Convert Degrees angle to seconds
MapOL.DEGtoSEC = function DEGtoSEC(angle) {
    // Extract DMS
    const deg = parseInt(angle, 10);
    let min = parseInt((angle - deg) * 100, 10);
    let sec = ((angle - deg) * 100 - min) * 100;

    // Avoid rounding problems with seconds=0
    const parts = String(angle).split(".");
    if (parts.length == 2 && parts[1].length == 2) {
        min = Number(parts[1]);
        sec = 0;
    }

    // Result in degrees sex (dd.mmss)
    return sec + min * 60 + deg * 3600;
};

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
};

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
};

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
};

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
};