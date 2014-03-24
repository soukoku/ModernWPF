using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
    /// A view model for a collection of items with pageable source.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public abstract class PagedCollectionViewModel<TItem> : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedCollectionViewModel{TItem}"/> class.
        /// </summary>
        /// <param name="pageSize">Size of a page.</param>
        protected PagedCollectionViewModel(int pageSize = 15)
        {
            _currentPage = 1;
            _totalPages = 1;
            _pageSize = pageSize > 0 ? pageSize : 15;
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

        private int _currentPage;
        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        public int CurrentPage
        {
            get { return _currentPage > _totalPages ? _totalPages : _currentPage; }
            set
            {
                if (value > 0)
                    GoToPageAsync(value);
            }
        }

        private int _totalPages;
        /// <summary>
        /// Gets the total pages.
        /// </summary>
        /// <value>
        /// The total pages.
        /// </value>
        public int TotalPages
        {
            get { return _totalPages; }
            private set
            {
                _totalPages = value;
                RaisePropertyChanged(() => this.TotalPages);
            }
        }

        private int _pageSize;
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (_pageSize > 0)
                {
                    _pageSize = value;
                    RaisePropertyChanged(() => this.PageSize);
                    GoToPageAsync(CurrentPage);
                }
            }
        }

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

        private RelayCommand _reloadCommand;
        /// <summary>
        /// Gets the reload command that reloads the current page.
        /// </summary>
        /// <value>
        /// The reload command.
        /// </value>
        public RelayCommand ReloadCommand
        {
            get
            {
                if (_reloadCommand == null)
                {
                    _reloadCommand = new RelayCommand(() =>
                    {
                        GoToPageAsync(CurrentPage);
                    }, () => !IsLoading);
                }
                return _reloadCommand;
            }
        }


        private RelayCommand _firstPageCommand;
        /// <summary>
        /// Gets the command to go to the first page
        /// </summary>
        /// <value>
        /// The first page command.
        /// </value>
        public ICommand FirstPageCommand
        {
            get
            {
                if (_firstPageCommand == null)
                {
                    _firstPageCommand = new RelayCommand(() =>
                    {
                        GoToPageAsync(1);
                    }, () =>
                    {
                        if (!IsLoading)
                        {
                            return CurrentPage > 1;
                        }
                        return false;
                    });
                }
                return _firstPageCommand;
            }
        }


        private RelayCommand _prevPageCommand;
        /// <summary>
        /// Gets the command to go to the previous page
        /// </summary>
        /// <value>
        /// The previous page command.
        /// </value>
        public ICommand PrevPageCommand
        {
            get
            {
                if (_prevPageCommand == null)
                {
                    _prevPageCommand = new RelayCommand(() =>
                    {
                        GoToPageAsync(CurrentPage - 1);
                    }, () =>
                    {
                        if (!IsLoading)
                        {
                            return CurrentPage > 1;
                        }
                        return false;
                    });
                }
                return _prevPageCommand;
            }
        }


        private RelayCommand _nextPageCommand;
        /// <summary>
        /// Gets the command to go to the next page
        /// </summary>
        /// <value>
        /// The next page command.
        /// </value>
        public ICommand NextPageCommand
        {
            get
            {
                if (_nextPageCommand == null)
                {
                    _nextPageCommand = new RelayCommand(() =>
                    {
                        GoToPageAsync(CurrentPage + 1);
                    }, () =>
                    {
                        if (!IsLoading)
                        {
                            return CurrentPage < TotalPages;
                        }
                        return false;
                    });
                }
                return _nextPageCommand;
            }
        }


        private RelayCommand _lastPageCommand;
        /// <summary>
        /// Gets the command to go to the last page
        /// </summary>
        /// <value>
        /// The last page command.
        /// </value>
        public ICommand LastPageCommand
        {
            get
            {
                if (_lastPageCommand == null)
                {
                    _lastPageCommand = new RelayCommand(() =>
                    {
                        GoToPageAsync(TotalPages);
                    }, () =>
                    {
                        if (!IsLoading)
                        {
                            return CurrentPage < TotalPages;
                        }
                        return false;
                    });
                }
                return _lastPageCommand;
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
            CurrentPage = 1;
        }

        private async void GoToPageAsync(int page)
        {
            if (IsLoading) { return; }
            IsLoading = true;

        RETRY:

            var data = new NewPageData<TItem>(page);

            try
            {
                await OnRetrieveItems(data);
            }
            catch (Exception ex)
            {
                OnLoadError(ex);
            }

            var newTotalPgs = ((data.TotalCount - 1) / PageSize) + 1;

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
            foreach (var it in data.NewItems)
            {
                _items.Add(it);
            }

            _currentPage = page;
            _totalPages = newTotalPgs;

            IsLoading = false;
            RaisePropertyChanged(() => this.CurrentPage);
            RaisePropertyChanged(() => this.TotalPages);
            if (_firstPageCommand != null) { _firstPageCommand.RaiseCanExecuteChanged(); }
            if (_prevPageCommand != null) { _prevPageCommand.RaiseCanExecuteChanged(); }
            if (_nextPageCommand != null) { _nextPageCommand.RaiseCanExecuteChanged(); }
            if (_lastPageCommand != null) { _lastPageCommand.RaiseCanExecuteChanged(); }
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
        /// <param name="data">The data to populate the retrieved item info.</param>
        /// <returns></returns>
        protected abstract Task OnRetrieveItems(NewPageData<TItem> data);

        #endregion
    }

    /// <summary>
    /// Data for retrieving a new page of items.
    /// </summary>
    public class NewPageData<TItem>
    {
        internal NewPageData(int newPage)
        {
            NewItems = new List<TItem>();
            NewPage = newPage;
        }

        /// <summary>
        /// Gets the new page number to retrive items for.
        /// </summary>
        /// <value>
        /// The new page.
        /// </value>
        public int NewPage { get; private set; }

        private int _total;
        /// <summary>
        /// Gets/sets the total items count across all pages.
        /// </summary>
        /// <value>
        /// The total count.
        /// </value>
        public int TotalCount
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
        public ICollection<TItem> NewItems { get; private set; }

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
