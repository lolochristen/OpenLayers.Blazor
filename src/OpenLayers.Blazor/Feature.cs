using Microsoft.AspNetCore.Components;
using OpenLayers.Blazor.Internal;

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

    /// <summary>
    ///     Identifier
    /// </summary>
    [Parameter]
    public string Id
    {
        get => InternalFeature.Id.ToString();
        set => InternalFeature.Id = value;
    }

    protected Internal.Feature InternalFeature { get; set; }

    public GeometryTypes? GeometryType => InternalFeature.GeometryType;

    public string? Type => InternalFeature.Type;

    public Dictionary<string, dynamic> Properties => InternalFeature.Properties;

    public Coordinate? Point => Coordinates?.FirstOrDefault();

    public IEnumerable<Coordinate>? Coordinates => CoordinatesHelper.GetCoordinates(InternalFeature.Coordinates);

    public IEnumerable<IEnumerable<Coordinate>> MultiCoordinates => CoordinatesHelper.GetMultiCoordinates(InternalFeature.Coordinates);
}