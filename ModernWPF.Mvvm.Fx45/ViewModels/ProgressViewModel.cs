using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;

namespace ModernWPF.ViewModels
{
    /// <summary>
    /// A view-model for reporting progress.
    /// </summary>
    public class ProgressViewModel : ViewModelBase
    {
        /// <summary>
        /// Updates the progress state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="progressPercent">The progress percent (0 to 1).</param>
        /// <param name="info">The extra information.</param>
        public void UpdateState(TaskbarItemProgressState state, double progressPercent, string info = null)
        {
            Info = info;
            State = state;

            var val = progressPercent * Maximum;
            if (val < Minimum) { val = Minimum; }
            else if (val > Maximum) { val = Maximum; }
            Value = val;

            RaisePropertyChanged(() => State);
            RaisePropertyChanged(() => IsIndeterminate);
            RaisePropertyChanged(() => IsBusy);
            RaisePropertyChanged(() => Info);
            RaisePropertyChanged(() => Value);
        }


        /// <summary>
        /// Gets the progress state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public TaskbarItemProgressState State { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="State"/> is indeterminate.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="State"/> is indeterminate; otherwise, <c>false</c>.
        /// </value>
        public bool IsIndeterminate { get { return State == TaskbarItemProgressState.Indeterminate; } }

        /// <summary>
        /// Gets a value indicating whether the <see cref="State"/> is reporting progress.
        /// </summary>
        /// <value>
        ///   <c>true</c> if  the <see cref="State"/> is reporting progress; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy { get { return State != TaskbarItemProgressState.None; } }

        /// <summary>
        /// Gets the extra information.
        /// </summary>
        /// <value>
        /// The information.
        /// </value>
        public string Info { get; private set; }

        /// <summary>
        /// Gets the maximum for data-binding purposes.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public double Maximum { get { return 100; } }
        /// <summary>
        /// Gets the minimum for data-binding purposes.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public double Minimum { get { return 0; } }
        /// <summary>
        /// Gets the current progress value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public double Value { get; private set; }
    }
}
