using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

/// <summary>
///     Provides styling options for map features including fill, stroke, text, and icons.
/// </summary>
public class StyleOptions
{
    /// <summary>
    /// Defines horizontal text alignment options for canvas text.
    /// </summary>
    public enum CanvasTextAlign
    {
        /// <summary>
        /// Align text to the left.
        /// </summary>
        Left,
        
        /// <summary>
        /// Align text to the right.
        /// </summary>
        Right,
        
        /// <summary>
        /// Center align text.
        /// </summary>
        Center,
        
        /// <summary>
        /// Align text to the end (language-dependent).
        /// </summary>
        End,
        
        /// <summary>
        /// Align text to the start (language-dependent).
        /// </summary>
        Start
    }

    /// <summary>
    /// Defines vertical text baseline options for canvas text.
    /// </summary>
    public enum CanvasTextBaseline
    {
        /// <summary>
        /// Align text to the middle.
        /// </summary>
        Middle,
        
        /// <summary>
        /// Align text to the bottom.
        /// </summary>
        Bottom,
        
        /// <summary>
        /// Align text to the top.
        /// </summary>
        Top,
        
        /// <summary>
        /// Alphabetic baseline (default for most scripts).
        /// </summary>
        Alphabetic,
        
        /// <summary>
        /// Hanging baseline (used by some scripts like Devanagari).
        /// </summary>
        Hanging,
        
        /// <summary>
        /// Ideographic baseline (used for ideographic scripts).
        /// </summary>
        Ideographic
    }

    /// <summary>
    /// Defines how overlapping features should be handled.
    /// </summary>
    public enum DeclutterMode
    {
        /// <summary>
        /// Declutter overlapping features (hide some to reduce overlap).
        /// </summary>
        Declutter,
        
        /// <summary>
        /// Treat as obstacle for other features.
        /// </summary>
        Obstacle,
        
        /// <summary>
        /// No decluttering (show all features).
        /// </summary>
        None
    }

    /// <summary>
    /// Defines the units for icon anchor positioning.
    /// </summary>
    public enum IconAnchorUnits
    {
        /// <summary>
        /// Anchor position specified in pixels.
        /// </summary>
        Pixels,
        
        /// <summary>
        /// Anchor position specified as a fraction of icon size (0.0 to 1.0).
        /// </summary>
        Fraction
    }

    /// <summary>
    /// Defines the origin point for icon positioning and offsets.
    /// </summary>
    public enum IconOrigin
    {
        /// <summary>
        /// Origin at bottom-left corner.
        /// </summary>
        BottomLeft,
        
        /// <summary>
        /// Origin at bottom-right corner.
        /// </summary>
        BottomRight,
        
        /// <summary>
        /// Origin at top-left corner.
        /// </summary>
        TopLeft,
        
        /// <summary>
        /// Origin at top-right corner.
        /// </summary>
        TopRight
    }

    /// <summary>
    /// Defines text justification options.
    /// </summary>
    public enum TextJustify
    {
        /// <summary>
        /// Left justify text.
        /// </summary>
        Left,
        
        /// <summary>
        /// Center justify text.
        /// </summary>
        Center,
        
        /// <summary>
        /// Right justify text.
        /// </summary>
        Right
    }

    /// <summary>
    /// Defines how text should be placed relative to features.
    /// </summary>
    public enum TextPlacement
    {
        /// <summary>
        /// Place text at a point.
        /// </summary>
        Point,
        
        /// <summary>
        /// Place text along a line.
        /// </summary>
        Line
    }

    /// <summary>
    ///     Gets or sets the fill options for the style.
    /// </summary>
    public FillOptions? Fill { get; set; }

    /// <summary>
    ///     Gets or sets the stroke options for the style.
    /// </summary>
    public StrokeOptions? Stroke { get; set; }

    /// <summary>
    ///     Gets or sets the text options for the style.
    /// </summary>
    public TextOptions? Text { get; set; }

    /// <summary>
    ///     Gets or sets the circle style options for point features.
    /// </summary>
    public CircleStyleOptions? Circle { get; set; }

    /// <summary>
    ///     Gets or sets the icon style options for markers and icons.
    /// </summary>
    public IconStyleOptions? Icon { get; set; }

    /// <summary>
    ///     Gets or sets the z-index for layer ordering.
    /// </summary>
    public int? ZIndex { get; set; }

    /// <summary>
    ///     Fill options for shapes. See https://openlayers.org/en/latest/apidoc/module-ol_style_Fill-Fill.html
    /// </summary>
    public class FillOptions
    {
        /// <summary>
        ///     Gets or sets the fill color.
        /// </summary>
        public string? Color { get; set; }
    }

    /// <summary>
    ///     Image style options. See https://openlayers.org/en/latest/apidoc/module-ol_style_Image-ImageStyle.html
    /// </summary>
    public class ImageOptions
    {
        /// <summary>
        ///     Gets or sets the opacity of the image (0-1).
        /// </summary>
        public double Opactiy { get; set; } = 1;

        /// <summary>
        ///     Gets or sets whether the image should rotate with the map view.
        /// </summary>
        public bool RotateWithView { get; set; }

        /// <summary>
        ///     Gets or sets the rotation of the image in radians.
        /// </summary>
        public double? Rotation { get; set; }

        /// <summary>
        ///     Gets or sets the scale factor for the image.
        /// </summary>
        public double? Scale { get; set; }

        /// <summary>
        ///     Gets or sets the displacement of the image in pixels [x, y].
        /// </summary>
        public double[]? Displacement { get; set; }

        /// <summary>
        ///     Gets or sets the declutter mode for the image.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumKebabLowerConverter))]
        public DeclutterMode? DeclutterMode { get; set; }
    }

    /// <summary>
    ///     Stroke style options. See https://openlayers.org/en/latest/apidoc/module-ol_style_Stroke-Stroke.html
    /// </summary>
    public class StrokeOptions
    {
        /// <summary>
        ///     Gets or sets the stroke color.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        ///     Gets or sets the line cap style ('butt', 'round', or 'square').
        /// </summary>
        public string? LineCap { get; set; }

        /// <summary>
        ///     Gets or sets the line join style ('bevel', 'round', or 'miter').
        /// </summary>
        public string? LineJoin { get; set; }

        /// <summary>
        ///     Gets or sets the line dash pattern.
        /// </summary>
        public double[]? LineDash { get; set; }

        /// <summary>
        ///     Gets or sets the line dash offset.
        /// </summary>
        public double? LineDashOffset { get; set; }

        /// <summary>
        ///     Gets or sets the miter limit.
        /// </summary>
        public double? MiterLimit { get; set; }

        /// <summary>
        ///     Gets or sets the stroke width.
        /// </summary>
        public double Width { get; set; }
    }

    /// <summary>
    ///     Text style options. See https://openlayers.org/en/latest/apidoc/module-ol_style_Text-Text.html
    /// </summary>
    public class TextOptions
    {
        /// <summary>
        ///     Gets or sets the font definition (e.g., '12px sans-serif').
        /// </summary>
        public string? Font { get; set; }

        /// <summary>
        ///     Gets or sets the maximum angle for text placement along lines.
        /// </summary>
        public double? MaxAngle { get; set; }

        /// <summary>
        ///     Gets or sets the horizontal offset in pixels.
        /// </summary>
        public double? OffsetX { get; set; }

        /// <summary>
        ///     Gets or sets the vertical offset in pixels.
        /// </summary>
        public double? OffsetY { get; set; }

        /// <summary>
        ///     Gets or sets whether text should overflow beyond its container.
        /// </summary>
        public bool Overflow { get; set; } = false;

        /// <summary>
        /// Gets or sets the text placement strategy (Point or Line).
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumKebabLowerConverter))]
        public TextPlacement? Placement { get; set; }

        /// <summary>
        /// Gets or sets the pixel distance between repeated text labels.
        /// </summary>
        public double? Repeat { get; set; }
        
        /// <summary>
        /// Gets or sets the scale factor for the text.
        /// </summary>
        public double? Scale { get; set; }
        
        /// <summary>
        /// Gets or sets whether the text rotates with the map view.
        /// </summary>
        public bool? RotateWithView { get; set; }
        
        /// <summary>
        /// Gets or sets the rotation angle of the text in radians.
        /// </summary>
        public double? Rotation { get; set; }
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the horizontal text alignment.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumKebabLowerConverter))]
        public CanvasTextAlign? TextAlign { get; set; }

        /// <summary>
        /// Gets or sets the text justification.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumKebabLowerConverter))]
        public TextJustify? Justify { get; set; }

        /// <summary>
        /// Gets or sets the text baseline alignment.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumKebabLowerConverter))]
        public CanvasTextBaseline? TextBaseline { get; set; }

        /// <summary>
        /// Gets or sets the fill style for the text.
        /// </summary>
        public FillOptions? Fill { get; set; }
        
        /// <summary>
        /// Gets or sets the stroke style for the text outline.
        /// </summary>
        public StrokeOptions? Stroke { get; set; }
        
        /// <summary>
        /// Gets or sets the fill style for the text background.
        /// </summary>
        public FillOptions? BackgroundFill { get; set; }
        
        /// <summary>
        /// Gets or sets the stroke style for the text background outline.
        /// </summary>
        public StrokeOptions? BackgroundStroke { get; set; }

        /// <summary>
        /// Gets or sets the padding around the text in pixels [top, right, bottom, left].
        /// </summary>
        public double[] Padding { get; set; }
    }

    /// <summary>
    /// Base class for image-based style options.
    /// See: https://openlayers.org/en/latest/apidoc/module-ol_style_Image-ImageStyle.html
    /// </summary>
    public abstract class ImageStyleOptions
    {
        /// <summary>
        /// Gets or sets the opacity (0.0 to 1.0).
        /// </summary>
        public double? Opacity { get; set; }
        
        /// <summary>
        /// Gets or sets whether the image rotates with the map view.
        /// </summary>
        public bool? RotateWithView { get; set; }
        
        /// <summary>
        /// Gets or sets the rotation in radians.
        /// </summary>
        public double? Rotation { get; set; }
        
        /// <summary>
        /// Gets or sets the scale factor.
        /// </summary>
        public double? Scale { get; set; }
        
        /// <summary>
        /// Gets or sets the displacement offset [x, y] in pixels.
        /// </summary>
        public double[]? Displacement { get; set; }

        /// <summary>
        /// Gets or sets the declutter mode for handling overlapping images.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeclutterMode? DeclutterMode { get; set; }
    }

    /// <summary>
    /// Style options for icon images.
    /// </summary>
    public class IconStyleOptions : ImageStyleOptions
    {
        /// <summary>
        /// Gets or sets the anchor position [x, y] as a fraction of the icon size.
        /// </summary>
        public double[]? Anchor { get; set; }

        /// <summary>
        /// Gets or sets the origin point for the anchor (default: TopLeft).
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumKebabLowerConverter))]
        public IconOrigin AnchorOrigin { get; set; } = IconOrigin.TopLeft;

        /// <summary>
        /// Gets or sets the units for the anchor X coordinate.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumKebabLowerConverter))]
        public IconAnchorUnits AnchorXUnits { get; set; }

        /// <summary>
        /// Gets or sets the units for the anchor Y coordinate.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumKebabLowerConverter))]
        public IconAnchorUnits AnchorYUnits { get; set; }

        /// <summary>
        /// Gets or sets the icon color as RGBA [r, g, b, a] (0-255 for RGB, 0.0-1.0 for alpha).
        /// </summary>
        public int[] Color { get; set; }

        /// <summary>
        /// Gets or sets the cross-origin attribute for loading images.
        /// </summary>
        public string CrossOrigin { get; set; }
        
        /// <summary>
        /// Gets or sets the icon width in pixels.
        /// </summary>
        public double? Width { get; set; }
        
        /// <summary>
        /// Gets or sets the icon height in pixels.
        /// </summary>
        public double? Height { get; set; }
        
        /// <summary>
        /// Gets or sets the offset [x, y] in pixels for sprite sheets.
        /// </summary>
        public double[]? Offset { get; set; }

        /// <summary>
        /// Gets or sets the origin point for the offset (default: TopLeft).
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumKebabLowerConverter))]
        public IconOrigin OffsetOrigin { get; set; } = IconOrigin.TopLeft;

        /// <summary>
        /// Gets or sets the icon size in pixels [width, height].
        /// </summary>
        public double[]? Size { get; set; }

        /// <summary>
        /// Gets or sets the image source URL.
        /// </summary>
        [JsonPropertyName("src")]
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets the shape feature to use for rendering.
        /// </summary>
        [JsonIgnore]
        public Shape? Shape { get; set; }

        /// <summary>
        /// Gets the internal shape representation for JavaScript interop.
        /// </summary>
        public Internal.Shape? ShapeSource => Shape?.InternalFeature;
    }

    /// <summary>
    /// Style options for regular geometric shapes (stars, triangles, squares, etc.).
    /// See: https://openlayers.org/en/latest/apidoc/module-ol_style_RegularShape-RegularShape.html
    /// </summary>
    public class RegularShapeStyleOptions : ImageStyleOptions
    {
        /// <summary>
        /// Gets or sets the number of points for star shapes. Default is 3 (triangle).
        /// </summary>
        public int Points { get; set; } = 3;
        
        /// <summary>
        /// Gets or sets the radius of the regular shape in pixels.
        /// </summary>
        public double? Radius { get; set; }
        
        /// <summary>
        /// Gets or sets the first radius for star shapes (outer radius).
        /// </summary>
        public double? Radius1 { get; set; }
        
        /// <summary>
        /// Gets or sets the second radius for star shapes (inner radius).
        /// </summary>
        public double? Radius2 { get; set; }
        
        /// <summary>
        /// Gets or sets the rotation angle in radians.
        /// </summary>
        public double? Angle { get; set; }
        
        /// <summary>
        /// Gets or sets the stroke style for the shape outline.
        /// </summary>
        public StrokeOptions? Stroke { get; set; }
        
        /// <summary>
        /// Gets or sets the fill style for the shape interior.
        /// </summary>
        public FillOptions? Fill{ get; set; }
    }

    /// <summary>
    ///     https://openlayers.org/en/latest/apidoc/module-ol_style_Circle-CircleStyle.html
    /// </summary>
    public class CircleStyleOptions : RegularShapeStyleOptions
    {
    }
}