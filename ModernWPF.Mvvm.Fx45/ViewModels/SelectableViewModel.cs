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
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public abstract class SelectableViewModel<TModel> : ViewModelBase
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
}
