using System.Text.Json;
using System.Text.Json.Serialization;
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
    private Internal.Feature _internalFeature;

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

    internal Internal.Feature InternalFeature
    {
        get => _internalFeature;
        set
        {
            _internalFeature = value;

            if (_internalFeature.Coordinates is JsonElement e)
                _internalFeature.Coordinates = CoordinatesHelper.DeserializeCoordinates(e);
        }
    }

    /// <summary>
    /// Gets the geometry type of the feature.
    /// </summary>
    public GeometryTypes? GeometryType => InternalFeature.GeometryType;

    /// <summary>
    /// Gets to type of feature.
    /// </summary>
    public string? Type => InternalFeature.Type;

    /// <summary>
    /// Gets a dictionary of all dynamic properties of a feature.
    /// </summary>
    public Dictionary<string, dynamic> Properties => InternalFeature.Properties;

    /// <summary>
    /// Gets the point coordinate if the shape is defined by a single coordinate e.g. point, circle.
    /// </summary>
    public Coordinate? Point => CoordinatesHelper.IsSingleCoordinate(InternalFeature.Coordinates) ? Coordinates?.FirstOrDefault() : null;

    /// <summary>
    /// Gets an enumerator of coordinates e.g. for a line.
    /// </summary>
    public IEnumerable<Coordinate>? Coordinates => !CoordinatesHelper.IsMultiCoordinate(InternalFeature.Coordinates) ? CoordinatesHelper.GetCoordinates(InternalFeature.Coordinates) : null;

    /// <summary>
    /// Gets an enumerator of multi coordinates e.g. for a multi line or multi polygon shape.
    /// </summary>
    public IEnumerable<IEnumerable<Coordinate>>? MultiCoordinates => CoordinatesHelper.IsMultiCoordinate(InternalFeature.Coordinates) ? CoordinatesHelper.GetMultiCoordinates(InternalFeature.Coordinates) : null;

    /// <summary>
    /// Gets or sets the raw coordinates of the shape
    /// </summary>
    [JsonIgnore]
    public dynamic RawCoordinates
    {
        get => InternalFeature.Coordinates;
        set => InternalFeature.Coordinates = value;
    }
}