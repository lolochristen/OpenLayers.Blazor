using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

public class Feature<T> : Feature where T : Internal.Feature
{
    public Feature(T value) : base(value)
    {
    }

    internal new T InternalFeature => (T)base.InternalFeature;
}

public class Feature : ComponentBase
{
    public Feature()
    {
        InternalFeature = new Internal.Feature();
    }

    internal Feature(Internal.Feature feature)
    {
        InternalFeature = feature;
    }

    protected Internal.Feature InternalFeature { get; set; }

    [Parameter]
    public dynamic Geometry
    {
        get => InternalFeature.Geometry;
        set => InternalFeature.Geometry = value;
    }
}