namespace OpenLayers.Blazor;

internal class CoordinatesEqualityComparer : IEqualityComparer<IList<Coordinate>>
{
    public bool Equals(IList<Coordinate> x, IList<Coordinate> y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.SequenceEqual(y);

    }

    public int GetHashCode(IList<Coordinate> obj)
    {
        return HashCode.Combine(obj.Count, obj.IsReadOnly);
    }
}