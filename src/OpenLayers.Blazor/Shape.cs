using Microsoft.AspNetCore.Components;

namespace OpenLayers.Blazor;

public class Shape<T> : Shape where T : Internal.Shape, new()
{
    public Shape() : base(new T())
    {
    }

    public Shape(T shape) : base(shape)
    {
    }

    internal new T InternalFeature => (T)base.InternalFeature;
}

public class Shape : Feature, IDisposable
{
    internal Shape(Internal.Shape shape) : base(shape)
    {
    }

    internal new Internal.Shape InternalFeature => (Internal.Shape)base.InternalFeature;

    [CascadingParameter] public Map? ParentMap { get; set; }

    [Parameter]
    public string? Title
    {
        get => InternalFeature.Title;
        set => InternalFeature.Title = value;
    }

    [Parameter]
    public string? Label
    {
        get => InternalFeature.Label;
        set => InternalFeature.Label = value;
    }

    [Parameter]
    public bool Popup
    {
        get => InternalFeature.Popup;
        set => InternalFeature.Popup = value;
    }

    [Parameter]
    public double Radius
    {
        get => InternalFeature.Radius / 1000;
        set => InternalFeature.Radius = value * 1000;
    }

    [Parameter]
    public string Color
    {
        get => InternalFeature.Color;
        set => InternalFeature.Color = value;
    }

    [Parameter]
    public string BorderColor
    {
        get => InternalFeature.BorderColor;
        set => InternalFeature.BorderColor = value;
    }


    [Parameter]
    public string BackgroundColor
    {
        get => InternalFeature.BackgroundColor;
        set => InternalFeature.BackgroundColor = value;
    }

    [Parameter]
    public double Scale
    {
        get => InternalFeature.Scale;
        set => InternalFeature.Scale = value;
    }

    [Parameter]
    public string? Content
    {
        get => InternalFeature.Content;
        set => InternalFeature.Content = value;
    }

    public void Dispose()
    {
        if (this is Marker)
            ParentMap?.MarkersList.Remove((Marker)this);
        else
            ParentMap?.ShapesList.Remove(this);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (this is Marker)
            ParentMap?.MarkersList.Add((Marker)this);
        else
            ParentMap?.ShapesList.Add(this);
    }
}