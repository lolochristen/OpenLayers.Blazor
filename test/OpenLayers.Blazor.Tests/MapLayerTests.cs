using Bunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLayers.Blazor.Tests;

public class MapLayerTests
{

    [Fact]
    public void Map_LayerComponentAdded_InCollection()
    {
        using var ctx = new TestContext();

        #if DEBUG
        var moduleInterop = ctx.JSInterop.SetupModule("./_content/OpenLayers.Blazor/openlayers_interop.js");
        #else
        var moduleInterop = ctx.JSInterop.SetupModule($"./_content/OpenLayers.Blazor/openlayers_interop.min.js?v={typeof(Map).Assembly.GetName().Version}");
        #endif
        moduleInterop.Mode = JSRuntimeMode.Loose;

        var component = ctx.RenderComponent<Map>(
            parameters =>
                parameters
                    // Add parameters
                    .Add(c => c.Layers, builder =>
                    {
                        builder.OpenComponent(0, typeof(Layer));
                        builder.AddAttribute(0, "SourceType", SourceType.WMTS);
                        builder.AddAttribute(0, "Url", "https://wmts");
                        builder.CloseComponent();
                    })
        );

        Assert.Single(component.Instance.LayersList);
        Assert.Equal("https://wmts", component.Instance.LayersList.First().Url);
    }

}

