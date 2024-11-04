var _MapOL = new Array();

export function MapOLInit(mapId, popupId, options, center, zoom, rotation, interactions, layers, instance, configureJsMethod) {
    _MapOL[mapId] = new MapOL(mapId, popupId, options, center, zoom, rotation, interactions, layers, instance, configureJsMethod);
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

export function MapOLZoomToExtent(mapId, layerId, padding) {
    _MapOL[mapId].setZoomToExtent(layerId, padding);
}

export function MapOLSetShapes(mapId, layerId, shapes) {
    _MapOL[mapId].setShapes(layerId, shapes);
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

export function MapOLRemoveLayer(mapId, layerId) {
    _MapOL[mapId].removeLayer(layerId);
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

export function MapOLSetDrawingSettings(mapId,
    drawingLayerId,
    enableNewShapes,
    enableEditShapes,
    enableShapeSnap,
    geometryType,
    freehand) {
    _MapOL[mapId].setDrawingSettings(drawingLayerId,
        enableNewShapes,
        enableEditShapes,
        enableShapeSnap,
        geometryType,
        freehand);
}

export function MapOLUndoDrawing(mapId) {
    _MapOL[mapId].undoDrawing();
}

export function MapOLUpdateShape(mapId, layerId, shape) {
    _MapOL[mapId].updateShape(layerId, shape);
}

export function MapOLRemoveShape(mapId, layerId, shape) {
    _MapOL[mapId].removeShape(layerId, shape);
}

export function MapOLAddShape(mapId, layerId, shape) {
    _MapOL[mapId].addShape(layerId, shape);
}

export function MapOLGetCoordinates(mapId, layerId, shapeId) {
    return _MapOL[mapId].getCoordinates(layerId, shapeId);
}

export function MapOLSetCoordinates(mapId, layerId, shapeId, coordinates) {
    return _MapOL[mapId].setCoordinates(layerId, shapeId, coordinates);
}

export function MapOLSetInteractions(mapId, active) {
    _MapOL[mapId].setInteractions(active);
}

export function MapOLSetSelectionSettings(mapId, layerId, selectionEnabled, selectionStyle, multiSelect) {
    _MapOL[mapId].setSelectionSettings(layerId, selectionEnabled, selectionStyle, multiSelect);
}

export function MapOLShowPopup(mapId, coordinates) {
    _MapOL[mapId].showPopup(coordinates);
}

export function MapOLApplyMapboxStyle(mapId, styleUrl, accessToken) {
    _MapOL[mapId].applyMapboxStyle(styleUrl, accessToken);
}

function MapOL(mapId, popupId, options, center, zoom, rotation, interactions, layers, instance, configureJsMethod) {
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
        function(coordinate) {
            return [
                MapOL.WGStoLV03y(coordinate[1], coordinate[0]),
                MapOL.WGStoLV03x(coordinate[1], coordinate[0])
            ];
        },
        function(coordinate) {
            return [
                MapOL.CHtoLV03lng(coordinate[0], coordinate[1]),
                MapOL.CHtoLV03lat(coordinate[0], coordinate[1])
            ];
        }
    );

    ol.proj.addCoordinateTransforms(
        "EPSG:4326",
        projectionLV95,
        function(coordinate) {
            return [
                MapOL.WGStoLV95y(coordinate[1], coordinate[0]),
                MapOL.WGStoLV95x(coordinate[1], coordinate[0])
            ];
        },
        function(coordinate) {
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
            rotation: rotation,
            maxZoom: this.Options.maxZoom,
            minZoom: this.Options.minZoom
        }),
        interactions: ol.interaction.defaults.defaults().extend([new ol.interaction.DragRotateAndZoom()])
    });

    this.addControls();

    var that = this;

    var popupElement = document.getElementById(popupId);

    this.OverlayPopup = new ol.Overlay({
        element: popupElement,
        positioning: "bottom-center",
        stopEvent: false,
        offset: [0, -50]
    });

    this.Map.addOverlay(this.OverlayPopup);

    this.setInteractions(interactions);
    this.setSelectionSettings(true);

    if (configureJsMethod) {
        try {
            const namespaces = configureJsMethod.split(".");
            const func = namespaces.pop();
            var context = window;
            for (let i = 0; i < namespaces.length; i++) {
                context = context[namespaces[i]];
            }
            context[func].apply(context, [this.Map]);
        //    configure(configureJsMethod, window, [options]);
        } catch (err) {
            console.error(err);
        }
    }

    this.Map.on("click", function(evt) { that.onMapClick(evt, that.OverlayPopup, popupElement) });
    this.Map.on("dblclick", function(evt) { that.onMapDblClick(evt) });
    this.Map.on("pointermove", function(evt) { that.onMapPointerMove(evt) });
    this.Map.on("rendercomplete", function(evt) { that.Instance.invokeMethodAsync("OnInternalRenderComplete"); });
    this.Map.getView().on("change:resolution", function(evt) { that.onMapResolutionChanged(); });
    this.Map.getView().on("change:center", function(evt) { that.onMapCenterChanged(); });
    this.Map.getView().on("change:rotation", function(evt) { that.onMapRotationChanged(); });

    this.onMapCenterChanged();
}

MapOL.prototype.prepareLayers = function(layers) {
    const ollayers = new Array();
    const that = this;

    layers.forEach((l, i, arr) => {
        try {
            let source;
            let sourceType = l.source.sourceType;

            // merge options
            if (l.options) {
                l = Object.assign(l, l.options);
                delete l.options;
            }
            if (l.source && l.source.options) {
                l.source = Object.assign(l.source, l.source.options);
                delete l.source.options;
            }

            l = MapOL.transformNullToUndefined(l);

            if (l.extent && this.Options.coordinatesProjection) {
                let projection = that.Options.viewProjection ??
                    (ollayers.length > 0 ? ollayers[0].getSource().getProjection() : "EPSG:3857");
                l.extent = ol.proj.transformExtent(l.extent,
                    l.source.projection ?? that.Options.coordinatesProjection,
                    projection);
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
                break;
            case "VectorEsriJson":
                l.source.format = new ol.format.EsriJSON(l.source.formatOptions);
                break;
            case "VectorGeoJson":
                l.source.format = new ol.format.GeoJSON(l.source.formatOptions);
                break;
            case "VectorTopoJson":
                l.source.format = new ol.format.TopoJson(l.source.formatOptions);
                break;
            case "VectorMVT":
                l.source.format = new ol.format.MVT(l.source.formatOptions);
                break;
            case "VectorIGC":
                l.source.format = new ol.format.IGC(l.source.formatOptions);
                break;
            case "VectorPolyline":
                l.source.format = new ol.format.Polyline(l.source.formatOptions);
                break;
            case "VectorWKT":
                l.source.format = new ol.format.WKT(l.source.formatOptions);
                break;
            case "VectorWKB":
                l.source.format = new ol.format.WKB(l.source.formatOptions);
                break;
            case "VectorGML2":
                l.source.format = new ol.format.GML2(l.source.formatOptions);
                break;
            case "VectorGML3":
                l.source.format = new ol.format.GML3(l.source.formatOptions);
                break;
            case "VectorGPX":
                l.source.format = new ol.format.GPX(l.source.formatOptions);
                break;
            case "VectorOSMXML":
                l.source.format = new ol.format.OSMXML(s);
                break;
            case "VectorWFS":
                l.source.format = new ol.format.WFS(l.source.formatOptions);
                break;
            case "ImageArcGISRest":
                source = new ol.source.ImageArcGISRest(l.source);
                break;
            case "ImageCanvasSource":
                source = new ol.source.ImageCanvasSource(l.source);
                break;
            case "ImageMapGuide":
                source = new ol.source.ImageMapGuide(l.source);
                break;
            case "ImageStatic":
                if (l.extent && !l.source.imageExtent)
                    l.source.imageExtent = l.extent;
                source = new ol.source.ImageStatic(l.source);
                break;
            case "ImageWMS":
                source = new ol.source.ImageWMS(l.source);
                break;
            }

            var layer;
            switch (l.layerType) {
                case "Image":
                    l.source = source;
                    layer = new ol.layer.Image(l);
                    break;

                case "Vector":
                case "VectorTile":
                    var features;
                    if (l.useStyleCallback) {
                        l.style = function (feature, resolution) {
                            that.getShapeStyleAsync(feature, l.id)
                            .then(style => feature.setStyle(style));
                        };
                    } else if (l.style) {
                        var styleOptions = l.style;
                        l.style = function (feature, resolution) {
                            return that.mapStyleOptionsToStyle(styleOptions);
                        };
                    } else if (l.flatStyle) {
                        l.style = l.flatStyle;
                    }
                    if (l.source.data) {
                        features = l.source.format.readFeatures(l.source.data,
                            {
                                featureProjection: this.Options.viewProjection ??
                                    (ollayers.length > 0
                                        ? ollayers[0].getSource().getProjection()
                                        : (that.Map ? that.Map.getView().getProjection() : "EPSG:3857")),
                                dataProjection: l.source.projection ?? this.Options.coordinatesProjection
                            });
                    }
                    l.source = l.layerType == "VectorTile"
                        ? new ol.source.VectorTile(l.source)
                        : new ol.source.Vector(l.source);

                    if (l.raiseShapeEvents) { // attach and sync
                        l.source.on("addfeature", function (evt) { that.onFeatureAdded(l.id, evt.feature); });
                        l.source.on("changefeature", function (evt) { that.onFeatureChanged(l.id, evt.feature); });
                        l.source.on("removefeature", function (evt) { that.onFeatureRemoved(l.id, evt.feature); });
                    }
                    if (features) l.source.addFeatures(features);
                    layer = l.layerType == "VectorTile" ? new ol.layer.VectorTile(l) : new ol.layer.Vector(l);
                    break;

                case "Heatmap":
                    l.source = new ol.source.Vector(l.source);
                    layer = new ol.layer.Heatmap(l);
                    break;

                case "Graticule":
                    if (l.showLabels == undefined) l.showLabels = true;
                    if (l.wrapX == undefined) l.wrapX = false;
                    delete l.source; // remove source, graticule does not like it
                    layer = new ol.layer.Graticule(l);
                    break;

                case "VectorImage":
                    l.source = new ol.source.Vector(l.source);
                    layer = new ol.layer.VectorImage(l);
                    break;

                case "WebGLTile":
                    l.source = source;
                    layer = new ol.layer.WebGLTile(l);
                    break;

                case "MapboxVectorStyle":
                    l.styleUrl = l.source.url;
                    l.source = l.source.layer;
                    layer = new olms.MapboxVectorLayer(l);
                    break;

                default: // tile
                    l.source = source;
                    layer = new ol.layer.Tile(l);
                    break;
            }

            ollayers.push(layer);
        } catch (exp) {
            console.error(`Could no add layer: ${exp}`);
        }
    });

    return ollayers;
};

MapOL.prototype.getLayer = function(layerId) {
    return this.Map.getAllLayers().find((l) => l.get("id") == layerId);
};

MapOL.prototype.setLayers = function(layers) {
    this.Map.setLayers(this.prepareLayers(layers));
};

MapOL.prototype.removeLayer = function(layerId) {
    this.Map.removeLayer(this.getLayer(layerId));
};

MapOL.prototype.addLayer = function(layer) {
    const ollayers = this.prepareLayers([layer]);
    this.Map.addLayer(ollayers[0]);
};

MapOL.prototype.updateLayer = function(layer) {
    const olayer = this.Map.getAllLayers().find((l) => l.get("id") == layer.id);
    if (olayer != undefined) {
        olayer.setVisible(layer.visibility);
        olayer.setOpacity(layer.opacity);
        olayer.setZIndex(layer.zindex);
        olayer.setExtent(layer.extent);
    }
};

MapOL.prototype.getShapesLayer = function() {
    return this.getLayer("shapes");
};

MapOL.prototype.getMarkersLayer = function() {
    return this.getLayer("markers");
};

MapOL.prototype.setShapes = function(layerId, shapes) {
    var source = this.getLayer(layerId).getSource();
    source.clear();
    if (shapes) {
        shapes.forEach((shape) => {
            var feature = this.mapShapeToFeature(shape, source);
            source.addFeature(feature);
        });
    }
};

MapOL.prototype.setZoom = function(zoom) {
    this.Map.getView().setZoom(zoom);
};

MapOL.prototype.setZoomToExtent = function(layerId, padding) {
    if (padding == null) padding = undefined;
    const layer = this.getLayer(layerId);
    const extent = layer.getSource().getExtent();
    if (extent[0] === Infinity) return;
    this.Map.getView().fit(extent, { size: this.Map.getSize(), padding: padding });
};

MapOL.prototype.setCenter = function(point) {
    this.Map.getView().setCenter(ol.proj.transform(point,
        this.Options.coordinatesProjection,
        this.Map.getView().getProjection()));
};

MapOL.prototype.setRotation = function(rotation) {
    this.Map.getView().setRotation(rotation);
};

MapOL.prototype.setOptions = function(options) {
    this.Options = options;
};

MapOL.prototype.setInteractions = function(active) {
    this.Map.getInteractions().forEach((interaction, i, arr) => {
        interaction.setActive(active);
    });
    if (active) {
        if (this.Map.getControls().getLength() == 0)
            this.addControls();
    } else {
        this.Map.getControls().clear();
    }
};

MapOL.prototype.addControls = function() {
    if (this.Options.zoomControl) this.Map.addControl(new ol.control.Zoom());
    if (this.Options.attributionControl) this.Map.addControl(new ol.control.Attribution());
    if (this.Options.fullScreenControl) this.Map.addControl(new ol.control.FullScreen());
    if (this.Options.zoomSliderControl) this.Map.addControl(new ol.control.ZoomSlider());
    if (this.Options.rotateControl) this.Map.addControl(new ol.control.Rotate());
    if (this.Options.scaleLineUnit != "None") {
        this.Map.addControl(new ol.control.ScaleLine({
            units: this.Options.scaleLineUnit.toLowerCase()
        }));
    }
    if (this.Options.overviewMap) {
        this.Map.addControl(new ol.control.OverviewMap({
            layers: [new ol.layer.Tile(layers[0])]
        }));
    }
    if (this.Options.zoomToExtentControl) this.Map.addControl(new ol.control.ZoomToExtent());
};

MapOL.prototype.getReducedFeature = function(feature) {
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

MapOL.prototype.onMapClick = function(evt, popup, element) {
    popup.setPosition(0, 0);
    var that = this;
    const coordinate = ol.proj.transform(evt.coordinate,
        this.Map.getView().getProjection(),
        this.Options.coordinatesProjection);
    this.Instance.invokeMethodAsync("OnInternalClick", coordinate);

    this.Map.forEachFeatureAtPixel(evt.pixel,
        function(feature, layer) {
            if (!layer)
                return; // no layer = drawing
            const layerId = layer.get("id");

            if (ol.Feature.prototype.isPrototypeOf(feature)) { // full feature
                const shape = that.mapFeatureToShape(feature);

                if (shape) {
                    that.Instance.invokeMethodAsync("OnInternalShapeClick", shape, layerId);
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
            } else if (ol.render.Feature.prototype.isPrototypeOf(feature)) { // render feature
                const intFeature = that.mapFeatureToInternalFeature(feature);
                if (intFeature) {
                    that.Instance.invokeMethodAsync("OnInternalFeatureClick", intFeature, layerId);
                }
                if (that.Options.autoPopup) {
                    popup.setPosition(intFeature.coordinates);
                }
            }
        });
};

MapOL.prototype.onMapDblClick = function(evt) {
    const coordinate = ol.proj.transform(evt.coordinate,
        this.Map.getView().getProjection(),
        this.Options.coordinatesProjection);
    this.Instance.invokeMethodAsync("OnInternalDoubleClick", coordinate);
}

MapOL.prototype.showPopup = function(coordinates) {
    this.OverlayPopup.setPosition(
        ol.proj.transform(coordinates,
            this.Options.coordinatesProjection,
            this.Map.getView().getProjection()));
}
MapOL.prototype.onMapPointerMove = function(evt) {
    if (evt.dragging || Number.isNaN(evt.coordinate[0])) {
        return;
    }
    const coordinate = ol.proj.transform(evt.coordinate,
        this.Map.getView().getProjection(),
        this.Options.coordinatesProjection);
    this.Instance.invokeMethodAsync("OnInternalPointerMove", coordinate);

    var hoverFeatureId = null, hoverLayerId = null;
    this.Map.forEachFeatureAtPixel(evt.pixel,
        function (feature, layer) {
            if (!layer)
                return; // no layer = drawing
            const featureId = feature.getId();
            if (!featureId)
                return;
            const layerId = layer.get("id");
            if (!hoverFeatureId) {
                hoverFeatureId = featureId.toString();
                hoverLayerId = layerId;
            }
        });

    if (this._hoverFeatureId != hoverFeatureId || this._hoverLayerId != hoverLayerId) {
        this.Instance.invokeMethodAsync("OnInternalShapeHover", hoverLayerId, hoverFeatureId);
        this._hoverFeatureId = hoverFeatureId;
        this._hoverLayerId = hoverLayerId;
    }
};

MapOL.prototype.onMapResolutionChanged = function() {
    this.Instance.invokeMethodAsync("OnInternalZoomChanged", this.Map.getView().getZoom());
    this.onVisibleExtentChanged();
};
MapOL.prototype.onMapCenterChanged = function() {
    const center = this.Map.getView().getCenter();
    if (!center) return;
    const coordinate = ol.proj.transform(center,
        this.Map.getView().getProjection(),
        this.Options.coordinatesProjection);
    this.Instance.invokeMethodAsync("OnInternalCenterChanged", coordinate);
    this.onVisibleExtentChanged();
};

MapOL.prototype.onMapRotationChanged = function() {
    const rotation = this.Map.getView().getRotation();
    if (!rotation) return;
    this.Instance.invokeMethodAsync("OnInternalRotationChanged", rotation);
};

MapOL.prototype.onVisibleExtentChanged = function() {
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

MapOL.prototype.centerToCurrentGeoLocation = function() {
    var that = this;
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function(position) {
            const projection = that.Map.getView().getProjection().getCode();
            that.Map.getView().setCenter(ol.proj.transform([position.coords.longitude, position.coords.latitude],
                "EPSG:4326",
                projection));
        });
    }
};

MapOL.prototype.getCurrentGeoLocation = function() {
    var that = this;
    return new Promise((resolve, reject) => {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function(position) {
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

MapOL.prototype.setVisibleExtent = function(extent) {
    this.disableVisibleExtentChanged = true;
    const viewProjection = this.Map.getView().getProjection();
    const te = ol.proj.transformExtent(new Array(extent.x1, extent.y1, extent.x2, extent.y2),
        this.Options.coordinatesProjection,
        viewProjection);
    this.Map.getView().fit(te, this.Map.getSize());
    this.disableVisibleExtentChanged = false;
};

MapOL.prototype.currentDraw = null;
MapOL.prototype.currentSnap = null;
MapOL.prototype.currentModify = null;
MapOL.prototype.setDrawingSettings = function(drawingLayerId,
    enableNewShapes,
    enableEditShapes,
    enableShapeSnap,
    geometryType,
    freehand) {
    var that = this;
    this.removeDrawingInteractions();

    const source = this.getLayer(drawingLayerId).getSource();

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
                that.getShapeStyleAsync(evt.feature, "geometries")
                    .then(style => evt.feature.setStyle(style));
                that.onFeatureAdded(drawingLayerId, evt.feature);
            });

        this.Map.addInteraction(this.currentDraw);
    }

    if (enableShapeSnap) {
        this.currentSnap = new ol.interaction.Snap({ source: source });
        this.Map.addInteraction(this.currentSnap);
    }
};

MapOL.prototype.removeDrawingInteractions = function() {
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

MapOL.prototype.undoDrawing = function() {
    if (this.currentDraw) {
        this.currentDraw.removeLastPoint();
    }
};

MapOL.prototype.currentSelection = null;
MapOL.prototype.setSelectionSettings = function(layerId, enableSelection, style, multiSelect) {

    var that = this;
    var defaultSelectionStyle = new ol.style.Style({
        fill: new ol.style.Fill({
            color: "#eeeeeeaa",
        }),
        stroke: new ol.style.Stroke({
            color: "rgba(67, 141, 239, 0.7)",
            width: 5,
        }),
    });

    if (enableSelection) {
        this.currentSelection = new ol.interaction.Select({
            condition: ol.events.condition.click,
            style: function(feature) {
                if (style) {
                    return that.mapStyleOptionsToStyle(style);
                } else {
                    const color = feature.get("COLOR") || "#eeeeeeaa";
                    defaultSelectionStyle.getFill().setColor(color);
                    return defaultSelectionStyle;
                }
            },
            layers: layerId ? [this.getLayer(layerId)] : undefined,
            multi: multiSelect
        });
        this.currentSelection.on("select",
            function(e) {
                var selected = new Array();
                if (e.selected) e.selected.forEach((f) => selected.push(that.mapFeatureToShape(f)));
                var unselected = new Array();
                if (e.unselected) e.unselected.forEach((f) => unselected.push(that.mapFeatureToShape(f)));
                that.Instance.invokeMethodAsync("OnInternalSelectionChanged", layerId, selected, unselected);
            });

        this.Map.addInteraction(this.currentSelection);
    } else {
        if (this.currentSelection) {
            this.Map.removeInteraction(this.currentSelection);
            this.currentSelection = null;
        }
    }
};
MapOL.prototype.onFeatureAdded = function(layerId, feature) {
    const shape = this.mapFeatureToShape(feature);
    this.Instance.invokeMethodAsync("OnInternalShapeAdded", layerId, shape);
};

MapOL.prototype.mapFeatureToShape = function(feature) {

    if (feature == null) return null;

    var geometry = feature.getGeometry();
    var viewProjection = this.Map ? this.Map.getView().getProjection() : (this.Options.viewProjection ?? "EPSG:3857");
    var coordinates = null;

    if (geometry != null && !Array.isArray(geometry)) {
        switch (geometry.getType()) {
            case "Circle":
                coordinates = ol.proj.transform(geometry.getCenter(), viewProjection, this.Options.coordinatesProjection);
                break;
            default:
                var gc = geometry.getCoordinates();
                var l = gc.length;
                if (Array.isArray(gc[0])) gc.forEach((g2) => l = l + g2.length);
                if (l < this.Options.serializationCoordinatesLimit)
                    coordinates = MapOL.transformCoordinates(gc, viewProjection, this.Options.coordinatesProjection);
                break;
        }
    }

    var style = feature.getStyle();
    if (typeof(style) == "function") style = style(feature, this.Map.getView().getResolution());

    var styleOptions = new Array();
    if (Array.isArray(style)) {
        style.forEach((s) => styleOptions.push(this.mapStylesToStyleOptions(s)));
    } else {
        if (style) styleOptions = [this.mapStylesToStyleOptions(style)];
    }

    var id = feature.getId();

    if (id == null) {
        id = self.crypto.randomUUID();
        feature.setId(id);
    } else {
        id = id.toString();
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
        styles: styleOptions
    };

    if (geometry && geometry.getType() == "Circle" && geometry.getRadius) {
        shape.radius = geometry.getRadius();
    }

    return shape;
};

MapOL.prototype.mapFeatureToInternalFeature = function (feature) {
    if (feature == null) return null;

    var viewProjection = this.Map ? this.Map.getView().getProjection() : (this.Options.viewProjection ?? "EPSG:3857");
    var coordinates = null;

    var c = feature.getFlatCoordinates();
    var l = c.length;
    if (Array.isArray(c[0])) c.forEach((g2) => l = l + g2.length);
    if (l < this.Options.serializationCoordinatesLimit)
        coordinates = MapOL.transformCoordinates(c, viewProjection, this.Options.coordinatesProjection);
    
    var id = feature.getId() ?? feature.getProperties().feature_id;
    var properties = feature.getProperties();
    properties.type = feature.getType();

    return {
        id: id ? id.toString() : null,
        coordinates: coordinates,
        properties: properties
    }
}

MapOL.prototype.mapShapeToFeature = function(shape, source = null, transformCoordinates = true) {

    var geometry;
    const viewProjection = this.Map.getView().getProjection();
    const sourceProjection = source ? source.getProjection() : null;
    if (shape.coordinates) {
        const coordinates = transformCoordinates
            ? MapOL.transformCoordinates(shape.coordinates,
                sourceProjection ?? this.Options.coordinatesProjection,
                viewProjection)
            : shape.coordinates;

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
        markerstyle: shape.properties.markerstyle,
    });

    feature.setId(shape.id);

    if (geometry) {
        feature.setGeometry(geometry);
    }

    if (shape.styles) {
        if (Array.isArray(shape.styles)) {
            var styles = new Array();
            shape.styles.forEach((s) => styles.push(this.mapStyleOptionsToStyle(s)));
            feature.setStyle(styles);
        } else {
            feature.setStyle(this.mapStyleOptionsToStyle(shape.styles));
        }
    }
    if (shape.flatStyle)
        feature.setStyle(shape.flatStyle);

    return feature;
};

MapOL.prototype.onFeatureRemoved = function(layerId, feature) {
    const shape = this.mapFeatureToShape(feature);
    this.Instance.invokeMethodAsync("OnInternalShapeRemoved", layerId, shape);
};

MapOL.prototype.onFeatureChanged = function(layerId, feature) {
    const shape = this.mapFeatureToShape(feature);
    this.Instance.invokeMethodAsync("OnInternalShapeChanged", layerId, shape);
};

MapOL.prototype.updateShape = function(layerId, shape) {

    const layer = this.getLayer(layerId);
    const feature = layer.getSource().getFeatureById(shape.id);

    if (feature) {
        const newFeature = this.mapShapeToFeature(shape);
        const geometry = newFeature.getGeometry();
        if (geometry)
            feature.setGeometry(geometry);
        feature.setStyle(newFeature.getStyle());
    }
};

MapOL.prototype.removeShape = function(layerId, shape) {
    const layer = this.Map.getAllLayers().find((l) => l.get("id") == layerId);
    const source = layer.getSource();
    const feature = source.getFeatureById(shape.id);
    if (feature) {
        source.removeFeature(feature);
    }
};

MapOL.prototype.addShape = function(layerId, shape) {
    const layer = this.Map.getAllLayers().find((l) => l.get("id") == layerId);
    const source = layer.getSource();
    const feature = this.mapShapeToFeature(shape, source);
    source.addFeature(feature);
};

MapOL.prototype.getCoordinates = function(layerId, featureId) {
    const feature = this.getLayer(layerId).getSource().getFeatureById(featureId);
    if (feature) {
        let coord = feature.getGeometry().getCoordinates();
        if (!coord)
            coord = feature.getGeometry().getCenter();
        return coord;
    }
    return null;
};

MapOL.prototype.setCoordinates = function (layerId, featureId, coordinates) {
    const feature = this.getLayer(layerId).getSource().getFeatureById(featureId);
    if (feature == null)
        return;
    const geometry = feature.getGeometry();
    const viewProjection = this.Map.getView().getProjection();
    const sourceProjection = this.getLayer(layerId).getSource().getProjection();
    const coordinatesTransformed = MapOL.transformCoordinates(coordinates,
        sourceProjection ?? this.Options.coordinatesProjection,
        viewProjection);
    if (geometry.getType() == "Circle")
        geometry.setCenter(coordinatesTransformed);
    else
        geometry.setCoordinates(coordinatesTransformed);
};

MapOL.prototype.getShapeStyleAsync = async function(feature, layer_id) {
    var shape;
    if (ol.render.Feature.prototype.isPrototypeOf(feature))
        shape = this.mapFeatureToInternalFeature(feature);
    else
        shape = this.mapFeatureToShape(feature);
    delete shape.coordinates;
    const style = await this.Instance.invokeMethodAsync("OnGetShapeStyleAsync", shape, layer_id);
    return this.mapStyleOptionsToStyle(style, shape.geometryType);
};

MapOL.prototype.getShapeStyle = function(feature, layer_id) {
    var shape;
    if (ol.render.Feature.prototype.isPrototypeOf(feature))
        shape = this.mapFeatureToInternalFeature(feature);
    else
        shape = this.mapFeatureToShape(feature);
    delete shape.coordinates;
    const style = this.Instance.invokeMethod("OnGetShapeStyle", shape, layer_id); // will fail on blazor server
    return this.mapStyleOptionsToStyle(style, shape.geometryType);
};

MapOL.prototype.mapStyleOptionsToStyle = function(style, geometryType = null) {

    style = MapOL.transformNullToUndefined(style);

    if (style.icon && style.icon.shapeSource) {
        const canvas = document.createElement("canvas");
        const context = ol.render.toContext(canvas.getContext("2d"),
            {
                size: style.icon.size,
                pixelRatio: 1
            });
        const feature = this.mapShapeToFeature(style.icon.shapeSource, null, false);
        context.setStyle(feature.getStyle()[0]);
        context.drawGeometry(feature.getGeometry());
        style.icon.img = canvas;
    }

    if (style.icon && style.icon.scale) {
        style.icon.width = undefined;
        style.icon.height = undefined;
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

    // fix point style if circle not set
    if (geometryType == 'Point' && !style.circle) {
        style.circle = {
            fill: new ol.style.Fill(style.fill),
            stroke: new ol.style.Stroke(style.stroke),
            radius: style.stroke.width * 2
        };
    }

    const styleObject = new ol.style.Style({
        stroke: style.stroke ? new ol.style.Stroke(style.stroke) : undefined,
        fill: style.fill ? new ol.style.Fill(style.fill) : undefined,
        text: style.text ? new ol.style.Text(style.text) : undefined,
        image: style.circle ? new ol.style.Circle(style.circle) : style.icon ? new ol.style.Icon(style.icon) : undefined,
        zIndex: style.zIndex,
    });

    return styleObject;
};

MapOL.prototype.mapStylesToStyleOptions = function(style) {
    var image = style.getImage();
    var fill = style.getFill();
    var stroke = style.getStroke();
    var text = style.getText();
    var icon, circle;
    if (image && image.getSrc) {
        icon = image;
    }
    if (image && image.getRadius) {
        circle = image;
    }
    return {
        fill: fill
            ? {
                color: fill.getColor()
            }
            : undefined,
        stroke: stroke
            ? {
                color: stroke.getColor(),
                lineCap: stroke.getLineCap(),
                lineJoin: stroke.getLineJoin(),
                lineDash: stroke.getLineDash(),
                lineDashOffset: stroke.getLineDashOffset(),
                miterLimit: stroke.getMiterLimit(),
                width: stroke.getWidth(),
            }
            : undefined,
        text: text
            ? {
                font: text.getFont(),
                maxAngle: text.getMaxAngle(),
                offsetX: text.getOffsetX(),
                offsetY: text.getOffsetY(),
                overflow: text.getOverflow(),
                placement: text.getPlacement(),
                repeat: text.getRepeat(),
                scale: text.getScale(),
                rotateWithView: text.getRotateWithView(),
                rotation: text.getRotation(),
                text: text.getText(),
                textAlign: text.getTextAlign(),
                justify: text.getJustify(),
                textBaseline: text.getTextBaseline(),
                fill: text.getFill()
                    ? {
                        color: text.getFill().getColor()
                    }
                    : undefined,
                backgroundFill: text.getBackgroundFill()
                    ? {
                        color: text.getBackgroundFill().getColor()
                    }
                    : undefined,
                stroke: text.getStroke()
                    ? {
                        color: text.getStroke().getColor(),
                        lineCap: text.getStroke().getLineCap(),
                        lineJoin: text.getStroke().getLineJoin(),
                        lineDash: text.getStroke().getLineDash(),
                        lineDashOffset: text.getStroke().getLineDashOffset(),
                        miterLimit: text.getStroke().getMiterLimit(),
                        width: text.getStroke().getWidth(),
                    }
                    : undefined,
                backgroundStroke: text.getBackgroundStroke()
                    ? {
                        color: text.getBackgroundStroke().getColor(),
                        lineCap: text.getBackgroundStroke().getLineCap(),
                        lineJoin: text.getBackgroundStroke().getLineJoin(),
                        lineDash: text.getBackgroundStroke().getLineDash(),
                        lineDashOffset: text.getBackgroundStroke().getLineDashOffset(),
                        miterLimit: text.getBackgroundStroke().getMiterLimit(),
                        width: text.getBackgroundStroke().getWidth(),
                    }
                    : undefined,
                backgroundFill: text.getBackgroundFill()
                    ? {
                        color: text.getBackgroundFill().getColor()
                    }
                    : undefined,
                padding: text.getPadding(),
            }
            : undefined,
        circle: circle
            ? {
                radius: circle.getRadius(),
                fill: circle.fill
                    ? {
                        color: circle.fill.getColor()
                    }
                    : undefined,
                rotation: circle.getRotation(),
                rotateWithView: circle.getRotateWithView(),
                declutterMode: circle.getDeclutterMode(),
                displacement: circle.getDisplacement(),
                stroke: circle.stroke
                    ? {
                        color: circle.stroke.getColor(),
                        lineCap: circle.stroke.getLineCap(),
                        lineJoin: circle.stroke.getLineJoin(),
                        lineDash: circle.stroke.getLineDash(),
                        lineDashOffset: circle.stroke.getLineDashOffset(),
                        miterLimit: circle.stroke.getMiterLimit(),
                        width: circle.stroke.getWidth(),
                    }
                    : undefined,
            }
            : undefined,
        icon: icon
            ? {
                anchor: icon.getAnchor(),
                color: icon.getColor(),
                declutterMode: icon.getDeclutterMode(),
                height: icon.getHeight(),
                opacity: icon.getOpacity(),
                rotation: icon.getRotation(),
                rotateWithView: icon.getRotateWithView(),
                scale: icon.getScale(),
                size: icon.getSize(),
                width: icon.getWidth(),
                src: icon.getSrc(),
            }
            : undefined,
        zIndex: style.getZIndex()
    };
};

MapOL.prototype.applyMapboxStyle = function(styleUrl, accessToken) {
    var that = this;
    this.Map.getAllLayers().forEach((l) => { that.Map.removeLayer(l); });
    olms.apply(this.Map, styleUrl, { accessToken: accessToken }).then(function(r) {
        // TODO populate layer list
    });
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
        for (let i = 0; i < coordinates.length; i++) {

            if (Array.isArray(coordinates[i][0])) {
                newCoordinates[i] = Array(coordinates[i].length);
                for (let i2 = 0; i2 < coordinates[i].length; i2++) {
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
};

// Swiss projection transform https://www.swisstopo.admin.ch/en/knowledge-facts/surveying-geodesy/reference-systems/map-projections.html
MapOL.WGStoLV03y = function WGStoLV03y(lat, lng) {
    lat = DECtoSEX(lat);
    lng = DECtoSEX(lng);
    lat = DEGtoSEC(lat);
    lng = DEGtoSEC(lng);
    const lat_aux = (lat - 169028.66) / 10000;
    const lng_aux = (lng - 26782.5) / 10000;
    const y =
        600072.37 +
            211455.93 * lng_aux -
            10938.51 * lng_aux * lat_aux -
            0.36 * lng_aux * Math.pow(lat_aux, 2) -
            44.54 * Math.pow(lng_aux, 3);
    return y;
};

MapOL.WGStoLV03x = function WGStoLV03x(lat, lng) {
    lat = DECtoSEX(lat);
    lng = DECtoSEX(lng);
    lat = DEGtoSEC(lat);
    lng = DEGtoSEC(lng);
    const lat_aux = (lat - 169028.66) / 10000;
    const lng_aux = (lng - 26782.5) / 10000;
    const x =
        200147.07 +
            308807.95 * lat_aux +
            3745.25 * Math.pow(lng_aux, 2) +
            76.63 * Math.pow(lat_aux, 2) -
            194.56 * Math.pow(lng_aux, 2) * lat_aux +
            119.79 * Math.pow(lat_aux, 3);
    return x;
};

MapOL.CHtoLV03lat = function CHtoLV03lat(y, x) {
    const y_aux = (y - 600000) / 1000000;
    const x_aux = (x - 200000) / 1000000;
    let lat =
        16.9023892 +
            3.238272 * x_aux -
            0.270978 * Math.pow(y_aux, 2) -
            0.002528 * Math.pow(x_aux, 2) -
            0.0447 * Math.pow(y_aux, 2) * x_aux -
            0.014 * Math.pow(x_aux, 3);
    lat = (lat * 100) / 36;
    return lat;
};

MapOL.CHtoLV03lng = function CHtoLV03lng(y, x) {
    const y_aux = (y - 600000) / 1000000;
    const x_aux = (x - 200000) / 1000000;
    let lng =
        2.6779094 +
            4.728982 * y_aux +
            0.791484 * y_aux * x_aux +
            0.1306 * y_aux * Math.pow(x_aux, 2) -
            0.0436 * Math.pow(y_aux, 3);
    lng = (lng * 100) / 36;
    return lng;
};

MapOL.DECtoSEX = function DECtoSEX(angle) {
    const deg = parseInt(angle, 10);
    const min = parseInt((angle - deg) * 60, 10);
    const sec = ((angle - deg) * 60 - min) * 60;
    return deg + min / 100 + sec / 10000;
};
MapOL.DEGtoSEC = function DEGtoSEC(angle) {
    const deg = parseInt(angle, 10);
    let min = parseInt((angle - deg) * 100, 10);
    let sec = ((angle - deg) * 100 - min) * 100;
    const parts = String(angle).split(".");
    if (parts.length == 2 && parts[1].length == 2) {
        min = Number(parts[1]);
        sec = 0;
    }
    return sec + min * 60 + deg * 3600;
};

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

MapOL.LV95toWGSlat = function LV95toWGSlat(y, x) {
    const x1 = (x - 1200000) / 1000000;
    const x2 = x1 * x1;
    const x3 = x2 * x1;
    const y1 = (y - 2600000) / 1000000;
    const y2 = y1 * y1;
    const latitude = 16.9023892 +
        3.238272 * x1 -
        0.270978 * y2 -
        0.002528 * x2 -
        0.0447 * y2 * x1 -
        0.0140 * x3;
    return (latitude * 100 / 36);
};

MapOL.LV95toWGSlng = function LV95toWGSlng(y, x) {
    const x1 = (x - 1200000) / 1000000;
    const x2 = x1 * x1;
    const y1 = (y - 2600000) / 1000000;
    const y2 = y1 * y1;
    const y3 = y2 * y1;
    const longitude = 2.6779094 +
        4.728982 * y1 +
        0.791484 * y1 * x1 +
        0.1306 * y1 * x2 -
        0.0436 * y3;
    return (longitude * 100 / 36);
};
