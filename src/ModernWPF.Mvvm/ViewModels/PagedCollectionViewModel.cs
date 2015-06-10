using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace ModernWPF.ViewModels
{
    /// <summary>
    /// A highly experimental view model for a collection of items with pageable source.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public abstract class PagedCollectionViewModel<TItem> : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedCollectionViewModel{TItem}"/> class
        /// with a default page size of 100.
        /// </summary>
        protected PagedCollectionViewModel() : this(100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedCollectionViewModel{TItem}"/> class.
        /// </summary>
        /// <param name="pageSize">Size of a page.</param>
        protected PagedCollectionViewModel(int pageSize)
        {
            Pager = new PagerViewModel(async (pager, newPage) => await GoToPageAsync(newPage), pageSize);
            _items = new ObservableCollection<TItem>();
            Items = new ReadOnlyObservableCollection<TItem>(_items);
            View = CollectionViewSource.GetDefaultView(Items);
        }

        #region properties

        /// <summary>
        /// Gets the <see cref="ICollectionView"/> on the items.
        /// </summary>
        /// <value>
        /// The collection view.
        /// </value>
        protected ICollectionView View { get; private set; }

        /// <summary>
        /// Gets the pager.
        /// </summary>
        /// <value>
        /// The pager.
        /// </value>
        public PagerViewModel Pager { get; private set; }


        private ObservableCollection<TItem> _items;
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public ReadOnlyObservableCollection<TItem> Items { get; private set; }

        private bool _isLoading;
        /// <summary>
        /// Gets a value indicating whether this instance is loading items.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading items; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                _isLoading = value;
                RaisePropertyChanged(() => this.IsLoading);
            }
        }

        #endregion

        #region work methods

        /// <summary>
        /// Reset by clearing items and setting page to 1.
        /// </summary>
        protected void Reset()
        {
            _items.Clear();
            Pager.CurrentPage = 1;
        }

        private async Task GoToPageAsync(int page)
        {
            if (IsLoading) { return; }
            IsLoading = true;

            RETRY:

            NewPageData<TItem> data = null;

            try
            {
                data = await OnRetrieveItems(page);
            }
            catch (Exception ex)
            {
                OnLoadError(ex);
            }
            if (data != null)
            {
                var newTotalPgs = ((data.Total - 1) / Pager.PageSize) + 1;

                if (page > newTotalPgs)
                {
                    // auto retry a previous pg
                    page = newTotalPgs - 1;
                    goto RETRY;
                }

                if (data.Behavior == NewItemBehavior.Replace)
                {
                    _items.Clear();
                }
                if (data.Items != null)
                {
                    foreach (var it in data.Items)
                    {
                        _items.Add(it);
                    }
                }
                Pager.UpdateStat(page, data.Total);
            }

            IsLoading = false;
        }

        /// <summary>
        /// Called when an uncaught error occurs during <see cref="OnRetrieveItems" />.
        /// </summary>
        /// <param name="exception">The exception.</param>
        protected virtual void OnLoadError(Exception exception)
        {
        }

        /// <summary>
        /// Called when items needs to be retrieved. Populate the new items on the provided data object.
        /// </summary>
        /// <param name="newPage">the new page number to retrive items for.</param>
        /// <returns></returns>
        protected abstract Task<NewPageData<TItem>> OnRetrieveItems(int newPage);

        #endregion
    }

    /// <summary>
    /// Data for retrieving a new page of items.
    /// </summary>
    public class NewPageData<TItem>
    {
        private int _total;
        /// <summary>
        /// Gets/sets the total items count across all pages.
        /// </summary>
        /// <value>
        /// The total count.
        /// </value>
        public int Total
        {
            get { return _total; }
            set
            {
                if (value >= 0)
                {
                    _total = value;
                }
            }
        }

        /// <summary>
        /// Gets the new items in this page.
        /// </summary>
        /// <value>
        /// The new items.
        /// </value>
        public ICollection<TItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the collection behavior on new items.
        /// </summary>
        /// <value>
        /// The behavior.
        /// </value>
        public NewItemBehavior Behavior { get; set; }
    }

    /// <summary>
    /// Indicates the collection behavior on new items.
    /// </summary>
    public enum NewItemBehavior
    {
        /// <summary>
        /// Replace current items with new items.
        /// </summary>
        Replace,
        /// <summary>
        /// Append new items to the current list.
        /// </summary>
        Append
    }
}
