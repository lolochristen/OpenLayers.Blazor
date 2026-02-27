using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

/// <summary>
/// Map component configured for Mapbox vector tile layers.
/// Requires the ol-mapbox-style JavaScript library.
/// </summary>
public class MapboxMap : Map
{
    /// <summary>
    /// Gets or sets the Mapbox style URL (e.g., "mapbox://styles/mapbox/streets-v11").
    /// </summary>
    [Parameter]
    public string StyleUrl { get; set; }

    /// <summary>
    /// Gets or sets the Mapbox access token for authenticated requests.
    /// </summary>
    [Parameter]
    public string? AccessToken { get; set; }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await ApplyMapboxStyle(StyleUrl, AccessToken);
        }
    }
}