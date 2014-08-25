using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernWPF.ViewModels
{
    /// <summary>
    /// A view-model for something that can be selected.
    /// </summary>
    public abstract class SelectableViewModel : ViewModelBase
    {
        private bool _isSelected;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnSelectedChanged();
                RaisePropertyChanged(() => this.IsSelected);
            }
        }

        /// <summary>
        /// Called when IsSelected has changed.
        /// </summary>
        protected virtual void OnSelectedChanged()
        {
        }

    }

    /// <summary>
    /// A selectable view-model wrapper for another model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public abstract class SelectableViewModel<TModel> : SelectableViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableViewModel{TModel}"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        protected SelectableViewModel(TModel model)
        {
            Model = model;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        protected TModel Model { get; private set; }


        /// <summary>
        /// Raises property changed event on the model.
        /// </summary>
        public void UpdateModelProperties()
        {
            RaisePropertyChanged(() => this.Model);
        }
    }
}
