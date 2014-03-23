using GalaSoft.MvvmLight.Messaging;
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
    public sealed partial class ModernMessageBox : DialogControl
    {
        #region static stuff

        /// <summary>
        /// Displays a message box in front of the specified window.
        /// </summary>
        /// <param name="owner">A <see cref="Window" /> that contains <see cref="DialogControlContainer" /> in its visual tree.</param>
        /// <param name="message">The message object from MvvmLight.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">owner</exception>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public static MessageBoxResult Show(Window owner, DialogMessage message)
        {
            return Show(owner.FindInVisualTree<DialogControlContainer>(), message);
        }

        /// <summary>
        /// Displays a message box in front of the specified <see cref="DialogControlContainer"/>.
        /// </summary>
        /// <param name="owner">A <see cref="DialogControlContainer"/> to host this message box.</param>
        /// <param name="message">The message object from MvvmLight.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">owner</exception>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public static MessageBoxResult Show(DialogControlContainer owner, DialogMessage message)
        {
            if (message == null) { throw new ArgumentNullException("message"); }

            return Show(owner, message.Content, message.Caption, message.Button, message.Icon, message.DefaultResult);
        }

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
            diag.ShowDialogReal(owner, messageBoxText, caption, button, icon, defaultResult);
            return diag._result;
        }

        #endregion

        #region instance stuff

        private ModernMessageBox()
        {
            InitializeComponent();
        }

        MessageBoxResult _defResult;
        MessageBoxResult _result;


        /// <summary>
        /// Called when dialog has been shown and focus needs to happen.
        /// </summary>
        protected override void OnFocus()
        {
            switch (_defResult)
            {
                case System.Windows.MessageBoxResult.No:
                    btnNo.Focus();
                    break;
                case System.Windows.MessageBoxResult.OK:
                    btnOK.Focus();
                    break;
                case System.Windows.MessageBoxResult.Yes:
                    btnYes.Focus();
                    break;
                case System.Windows.MessageBoxResult.Cancel:
                    btnCancel.Focus();
                    break;
            }
        }

        void ShowDialogReal(DialogControlContainer owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            _defResult = defaultResult;
            txtTitle.Text = caption;
            txtMsg.Text = messageBoxText;


            iconWarning.Visibility = System.Windows.Visibility.Collapsed;
            iconInfo.Visibility = System.Windows.Visibility.Collapsed;
            iconError.Visibility = System.Windows.Visibility.Collapsed;
            iconQuest.Visibility = System.Windows.Visibility.Collapsed;

            switch (icon)
            {
                case MessageBoxImage.Error:
                    iconError.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageBoxImage.Question:
                    iconQuest.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageBoxImage.Warning:
                    iconWarning.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageBoxImage.Information:
                    iconInfo.Visibility = System.Windows.Visibility.Visible;
                    break;
            }

            btnCancel.Visibility = System.Windows.Visibility.Collapsed;
            btnOK.Visibility = System.Windows.Visibility.Collapsed;
            btnYes.Visibility = System.Windows.Visibility.Collapsed;
            btnNo.Visibility = System.Windows.Visibility.Collapsed;
            switch (button)
            {
                case MessageBoxButton.YesNo:
                    btnYes.Visibility = System.Windows.Visibility.Visible;
                    btnNo.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    btnYes.Visibility = System.Windows.Visibility.Visible;
                    btnNo.Visibility = System.Windows.Visibility.Visible;
                    btnCancel.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageBoxButton.OKCancel:
                    btnCancel.Visibility = System.Windows.Visibility.Visible;
                    btnOK.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageBoxButton.OK:
                    btnOK.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
            ShowDialogModal(owner);
        }


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.OK;
            DialogResult = true;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Yes;
            DialogResult = true;
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.No;
            DialogResult = false;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Cancel;
            DialogResult = false;
        }

        #endregion

    }
}
