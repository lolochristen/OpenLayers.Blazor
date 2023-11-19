using Microsoft.AspNetCore.Components;
namespace OpenLayers.Blazor.Demo.Components;

public static class RenderMode
{
    public static IComponentRenderMode DefaultRenderMode = new Microsoft.AspNetCore.Components.Web.InteractiveWebAssemblyRenderMode();
}
