@REM compress js. 
@REM due to static web apps deployment task and issue in github actions support custom msbuild tasks, this has to be called manually until a proper solution is found
@REM prerequisites: npm install uglify-js -g

uglifyjs ./src/OpenLayers.Blazor/wwwroot/openlayers_interop.js -o ./src/OpenLayers.Blazor/wwwroot/openlayers_interop.min.js -c -m --source-map "filename='./src/OpenLayers.Blazor/wwwroot/openlayers_interop.min.js.map'"
