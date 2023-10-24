using System.Text.Json.Serialization;

namespace OpenLayers.Blazor;

public class StyleOptions
{
    public enum CanvasTextAlign
    {
        Left,
        Right,
        Center,
        End,
        Start
    }

    public enum CanvasTextBaseline
    {
        Bottom,
        Top,
        Middle,
        Alphabetic,
        Hanging,
        Ideographic
    }

    public enum DeclutterMode
    {
        Declutter,
        Obstacle,
        None
    }

    public enum IconAnchorUnits
    {
        Fraction,
        Pixels
    }

    public enum IconOrigin
    {
        BottomLeft,
        BottomRight,
        TopLeft,
        TopRight
    }

    public enum TextJustify
    {
        Left,
        Center,
        Right
    }

    public enum TextPlacement
    {
        Point,
        Line
    }

    public FillOptions? Fill { get; set; }
    public ImageOptions? Image { get; set; }

    public StrokeOptions? Stroke { get; set; }

    public TextOptions? Text { get; set; }

    public CircleStyleOptions? Circle { get; set; }

    public IconStyleOptions? Icon { get; set; }

    public double? zIndex { get; set; }

    /// <summary>
    ///     https://openlayers.org/en/latest/apidoc/module-ol_style_Fill-Fill.html
    /// </summary>
    public class FillOptions
    {
        public string Color { get; set; }
    }

    /// <summary>
    ///     https://openlayers.org/en/latest/apidoc/module-ol_style_Image-ImageStyle.html
    /// </summary>
    public class ImageOptions
    {
        public double Opactiy { get; set; } = 1;
        public bool RotateWithView { get; set; }
        public double? Rotation { get; set; }
        public double? Scale { get; set; }
        public double[]? Displacement { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeclutterMode? DeclutterMode { get; set; }
    }

    /// <summary>
    ///     see https://openlayers.org/en/latest/apidoc/module-ol_style_Stroke-Stroke.html
    /// </summary>
    public class StrokeOptions
    {
        public string Color { get; set; }
        public string? LineCap { get; set; }
        public string? LineJoin { get; set; }
        public double[]? LineDash { get; set; }
        public double? LineDashOffset { get; set; }
        public double? MiterLimit { get; set; }
        public double Width { get; set; }
    }

    /// <summary>
    ///     https://openlayers.org/en/latest/apidoc/module-ol_style_Text-Text.html
    /// </summary>
    public class TextOptions
    {
        public string Font { get; set; }
        public double? MaxAngle { get; set; }
        public double? OffsetX { get; set; }
        public double? OffsetY { get; set; }
        public bool Overflow { get; set; } = false;
        public string? Placement { get; set; }
        public double? Repeat { get; set; }
        public double? Scale { get; set; }
        public bool? RotateWithView { get; set; }
        public double? Rotation { get; set; }
        public string Text { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CanvasTextAlign? TextAlign { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TextJustify? Justify { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CanvasTextBaseline? TextBaseline { get; set; }

        public FillOptions? Fill { get; set; }
        public StrokeOptions? Stroke { get; set; }
        public FillOptions? BackgroundFill { get; set; }
        public StrokeOptions? BackgroundStroke { get; set; }
        public List<double> Padding { get; set; } = new() { 0, 0, 0, 0 };
    }

    /// <summary>
    ///     https://openlayers.org/en/latest/apidoc/module-ol_style_Image-ImageStyle.html
    /// </summary>
    public abstract class ImageStyleOptions
    {
        public double? Opacity { get; set; }
        public bool? RotateWithView { get; set; }
        public double? Rotation { get; set; }
        public double? Scale { get; set; }
        public double[]? Displacement { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeclutterMode? DeclutterMode { get; set; }
    }

    public class IconStyleOptions : ImageStyleOptions
    {
        public List<double> Anchor { get; set; } = new() { 0.5, 0.5 };

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public IconOrigin AnchorOrigin { get; set; } = IconOrigin.TopLeft;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public IconAnchorUnits AnchorXUnits { get; set; } = IconAnchorUnits.Fraction;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public IconAnchorUnits AnchorYUnits { get; set; } = IconAnchorUnits.Fraction;

        public string Color { get; set; }

        public string CrossOrigin { get; set; }

        // public object Img { get; set; } // You can specify the type based on the actual image object types you plan to use
        public double[]? Displacement { get; set; }
        public double Opacity { get; set; } = 1;
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double Scale { get; set; } = 1;
        public bool RotateWithView { get; set; } = false;
        public double Rotation { get; set; } = 0;
        public List<double> Offset { get; set; } = new() { 0, 0 };

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public IconOrigin OffsetOrigin { get; set; } = IconOrigin.TopLeft;

        public string Size { get; set; }
        public string Source { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeclutterMode? DeclutterMode { get; set; }
    }

    /// <summary>
    ///     https://openlayers.org/en/latest/apidoc/module-ol_style_RegularShape-RegularShape.html
    /// </summary>
    public class RegularShapeStyleOptions : ImageStyleOptions
    {
        public int Points { get; set; } = 3; // Default to a triangle
        public double? Radius { get; set; }
        public double? Radius1 { get; set; }
        public double? Radius2 { get; set; }
        public double? Angle { get; set; }
        public double[] Displacement { get; set; }
        public StrokeOptions? Stroke { get; set; }
        public double? Rotation { get; set; } = 0;
        public bool? RotateWithView { get; set; } = false;
        public double? Scale { get; set; } = 1;
        public DeclutterMode? DeclutterMode { get; set; }
    }

    /// <summary>
    ///     https://openlayers.org/en/latest/apidoc/module-ol_style_Circle-CircleStyle.html
    /// </summary>
    public class CircleStyleOptions : RegularShapeStyleOptions
    {
        public FillOptions? Fill { get; set; }
        public double Radius { get; set; }
        public StrokeOptions? Stroke { get; set; }
        public double[]? Displacement { get; set; }
        public double? Scale { get; set; } = 1;
        public double? Rotation { get; set; } = 0;
        public bool? RotateWithView { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeclutterMode? DeclutterMode { get; set; }
    }
}