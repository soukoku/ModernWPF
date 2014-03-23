using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernWPF.ViewModels
{
    public abstract class SelectableViewModel<TModel> : ViewModelBase
    {
        public SelectableViewModel(TModel model)
        {
            Model = model;
        }

        protected TModel Model { get; private set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => this.IsSelected);
                OnSelectedChanged();
            }
        }

        protected virtual void OnSelectedChanged()
        {
        }

    }
}
