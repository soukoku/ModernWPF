using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModernWPF.Controls
{
    /// <summary>
    /// An in-window <see cref="MessageBox"/> replacment when using ModernWPF.
    /// </summary>
    public sealed partial class ModernMessageBox : MessageBoxControl
    {
        #region static stuff


        /// <summary>
        /// Displays a message box in front of the specified window.
        /// </summary>
        /// <param name="owner">A <see cref="Window" /> that contains <see cref="DialogControlContainer" /> in its visual tree.</param>
        /// <param name="messageBoxText">The message box text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button to display.</param>
        /// <param name="icon">The icon to display.</param>
        /// <param name="defaultResult">The default result.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">owner</exception>
        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption = null, MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            return Show(owner.FindInVisualTree<DialogControlContainer>(), messageBoxText, caption, button, icon, defaultResult);
        }
        /// <summary>
        /// Displays a message box in front of the specified <see cref="DialogControlContainer" />.
        /// </summary>
        /// <param name="owner">A <see cref="DialogControlContainer" /> to host this message box.</param>
        /// <param name="messageBoxText">The message box text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button to display.</param>
        /// <param name="icon">The icon to display.</param>
        /// <param name="defaultResult">The default result.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">owner</exception>
        public static MessageBoxResult Show(DialogControlContainer owner, string messageBoxText, string caption = null, MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            if (owner == null) { throw new ArgumentNullException("owner"); }

            var diag = new ModernMessageBox();
            return diag.ShowDialogReal(owner, messageBoxText, caption, button, icon, defaultResult);
        }

        #endregion

        #region instance stuff

        private ModernMessageBox()
        {
            InitializeComponent();
        }

        MessageBoxResult ShowDialogReal(DialogControlContainer owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            txtMsg.Text = messageBoxText;
            return base.ShowDialogModal(owner, caption, button, icon, defaultResult);
        }


        #endregion

    }
}
