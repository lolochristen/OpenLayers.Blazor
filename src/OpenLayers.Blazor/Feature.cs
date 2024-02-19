using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using OpenLayers.Blazor.Internal;

namespace OpenLayers.Blazor;

/// <summary>
///     Base class of a map feature.
/// </summary>
public class Feature : ComponentBase
{
    /// <summary>
    /// Initializes a new instance of <see cref="Feature"/>.
    /// </summary>
    public Feature()
    {
        InternalFeature = new Internal.Feature();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Feature"/>.
    /// </summary>
    /// <param name="feature"></param>
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

    internal Internal.Feature InternalFeature { get; set; }

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
    [JsonIgnore]
    public Coordinate? Point => InternalFeature.Point;

    /// <summary>
    /// Gets or sets the coordinates of a feature or shape.
    /// </summary>
    [Parameter]
    public Coordinates Coordinates
    {
        get => InternalFeature.Coordinates;
        set => InternalFeature.Coordinates = value;
    }
}