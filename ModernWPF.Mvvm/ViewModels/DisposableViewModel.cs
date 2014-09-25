using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModernWPF.ViewModels
{
    /// <summary>
    /// A view model base class that also implements <see cref="IDisposable"/>.
    /// </summary>
    public class DisposableViewModel : ViewModelBase, IDisposable
    {

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Throws exception if this instance is disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException"></exception>
        protected void VerifyNotDisposed()
        {
            if (IsDisposed) { throw new ObjectDisposedException(this.ToString()); }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Finalizes an instance of the <see cref="DisposableViewModel"/> class.
        /// </summary>
        ~DisposableViewModel()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            IsDisposed = true;
            if (disposing)
            {
                OnDisposingManaged();
            }
            OnDisposingNative();
        }

        /// <summary>
        /// Called when disposing managed resources.
        /// </summary>
        protected virtual void OnDisposingManaged()
        {
            Cleanup();
        }

        /// <summary>
        /// Called when disposing native resources.
        /// </summary>
        protected virtual void OnDisposingNative() { }

    }
}
