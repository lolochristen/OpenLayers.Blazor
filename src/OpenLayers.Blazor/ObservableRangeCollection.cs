using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLayers.Blazor;

/// <summary>
/// An observable collection that supports adding and removing a range of items.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public class ObservableRangeCollection<T> : ObservableCollection<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableRangeCollection{T}"/> class that is empty.
    /// </summary>
    public ObservableRangeCollection() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableRangeCollection{T}"/> class that contains elements copied from the specified collection.
    /// </summary>
    /// <param name="collection">The collection from which the elements are copied.</param>
    public ObservableRangeCollection(IEnumerable<T> collection) : base(collection) { }

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="ObservableRangeCollection{T}"/>.
    /// </summary>
    /// <param name="collection">The collection whose elements should be added to the end of the <see cref="ObservableRangeCollection{T}"/>.</param>
    public void AddRange(IEnumerable<T> collection)
    {
        CheckReentrancy();
        var enumerable = collection as T[] ?? collection.ToArray();
        foreach (var item in enumerable)
        {
            Items.Add(item);
        }
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, enumerable.ToList()));
    }

    /// <summary>
    /// Removes the elements of the specified collection from the <see cref="ObservableRangeCollection{T}"/>.
    /// </summary>
    /// <param name="collection">The collection whose elements should be removed from the <see cref="ObservableRangeCollection{T}"/>.</param>
    public void RemoveRange(IEnumerable<T> collection)
    {
        CheckReentrancy();
        var enumerable = collection as T[] ?? collection.ToArray();
        foreach (var item in enumerable)
        {
            Items.Remove(item);
        }
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, enumerable.ToList()));
    }
}

