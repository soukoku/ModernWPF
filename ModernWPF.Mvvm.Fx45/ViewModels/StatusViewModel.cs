using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernWPF.ViewModels
{
    /// <summary>
    /// A view model for status string.
    /// </summary>
    public class StatusViewModel : ViewModelBase
    {
        /// <summary>
        /// Updates status with the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="type">The type.</param>
        public void Update(string text, StatusType type = StatusType.Info)
        {
            Text = text;
            Type = type;
            RaisePropertyChanged(() => Text);
            RaisePropertyChanged(() => Type);
            RaisePropertyChanged(() => IsError);
            RaisePropertyChanged(() => IsWarning);
            RaisePropertyChanged(() => IsSuccess);
            RaisePropertyChanged(() => IsInfo);
        }

        /// <summary>
        /// Gets the status type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public StatusType Type { get; private set; }

        /// <summary>
        /// Gets the status text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; private set; }

        /// <summary>
        /// Gets a value indicating whether current status is error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if current status is error; otherwise, <c>false</c>.
        /// </value>
        public bool IsError { get { return Type == StatusType.Error; } }
        /// <summary>
        /// Gets a value indicating whether current status is warning.
        /// </summary>
        /// <value>
        ///   <c>true</c> if current status is warning; otherwise, <c>false</c>.
        /// </value>
        public bool IsWarning { get { return Type == StatusType.Warning; } }
        /// <summary>
        /// Gets a value indicating whether current status is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if current status is success; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess { get { return Type == StatusType.Success; } }
        /// <summary>
        /// Gets a value indicating whether current status is info.
        /// </summary>
        /// <value>
        ///   <c>true</c> if current status is info; otherwise, <c>false</c>.
        /// </value>
        public bool IsInfo { get { return Type == StatusType.Info; } }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Text;
        }
    }

    /// <summary>
    /// Indicates the status type.
    /// </summary>
    public enum StatusType
    {
        /// <summary>
        /// The information type.
        /// </summary>
        Info,
        /// <summary>
        /// The success type.
        /// </summary>
        Success,
        /// <summary>
        /// The warning type.
        /// </summary>
        Warning,
        /// <summary>
        /// The error type.
        /// </summary>
        Error
    }
}
