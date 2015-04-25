using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ModernWPF
{
    /// <summary>
    /// An observable collection that will dispose items when they are removed.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IDisposable" />.</typeparam>
    public class AutoDisposeObservableCollection<T> : ObservableCollection<T> where T : IDisposable
    {
        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        protected override void SetItem(int index, T item)
        {
            T it = this[index];
            try
            {
                base.SetItem(index, item);
            }
            finally
            {
                if (it != null) { it.Dispose(); }
            }
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            // use a handle since the items may still be in UI.
            var handle = new List<T>(this);
            try
            {
                base.ClearItems();
            }
            finally
            {
                foreach (var it in handle) { it.Dispose(); }
            }
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The index.</param>
        protected override void RemoveItem(int index)
        {
            T it = this[index];
            try
            {
                base.RemoveItem(index);
            }
            finally
            {
                if (it != null) { it.Dispose(); }
            }
        }
    }
}
