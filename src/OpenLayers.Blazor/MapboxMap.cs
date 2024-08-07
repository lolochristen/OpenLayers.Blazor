using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

public class MapboxMap : Map
{
    [Parameter]
    public string StyleUrl { get; set; }

    [Parameter]
    public string? AccessToken { get; set; }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        bool applyStyle = false;

        if (parameters.TryGetValue(nameof(StyleUrl), out string? styleUrl) && styleUrl != StyleUrl)
            applyStyle = true;

        if (parameters.TryGetValue(nameof(AccessToken), out string? accessToken) && accessToken != AccessToken)
            applyStyle = true;

        if (applyStyle && styleUrl != null)
        {
            _ = ApplyMapboxStyle(styleUrl, accessToken);
        }

        return base.SetParametersAsync(parameters);
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await ApplyMapboxStyle(StyleUrl, AccessToken);
        }
    }
}