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
    /// A view-model for reporting progress. This is also suitable for databinding to <see cref="TaskbarItemInfo"/>
    /// in a wpf window.
    /// </summary>
    public class ProgressViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressViewModel"/> class.
        /// </summary>
        public ProgressViewModel()
        {
            Info = new StatusViewModel();
        }

        /// <summary>
        /// Updates the progress state.
        /// </summary>
        /// <param name="state">The state.</param>
        public void UpdateState(TaskbarItemProgressState state)
        {
            UpdateState(state, 0, null, StatusType.Info);
        }
        /// <summary>
        /// Updates the progress state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="progressPercent">The progress percent (0 to 1).</param>
        public void UpdateState(TaskbarItemProgressState state, double progressPercent)
        {
            UpdateState(state, progressPercent, null, StatusType.Info);
        }
        /// <summary>
        /// Updates the progress state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="progressPercent">The progress percent (0 to 1).</param>
        /// <param name="info">The extra information.</param>
        public void UpdateState(TaskbarItemProgressState state, double progressPercent, string info)
        {
            UpdateState(state, progressPercent, info, StatusType.Info);
        }

        /// <summary>
        /// Updates the progress state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="progressPercent">The progress percent (0 to 1).</param>
        /// <param name="info">The extra information.</param>
        /// <param name="infoType">Type of the information.</param>
        public void UpdateState(TaskbarItemProgressState state, double progressPercent, string info, StatusType infoType)
        {
            Info.Update(info, infoType);
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
        public StatusViewModel Info { get; private set; }

        /// <summary>
        /// Gets the maximum for data-binding purposes.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public static double Maximum { get { return 1; } }
        /// <summary>
        /// Gets the minimum for data-binding purposes.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public static double Minimum { get { return 0; } }
        /// <summary>
        /// Gets the current progress value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public double Value { get; private set; }
    }
}
