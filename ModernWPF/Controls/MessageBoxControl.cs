using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ModernWPF.Controls
{
    /// <summary>
    /// A base message box type <see cref="DialogControl"/> with custom content.
    /// A message box has built-in messagebox icon/caption/buttons.
    /// </summary>
    public class MessageBoxControl : DialogControl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static MessageBoxControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageBoxControl), new FrameworkPropertyMetadata(typeof(MessageBoxControl)));
        }

        /// <summary>
        /// Gets or sets the minimum width of the content.
        /// </summary>
        /// <value>
        /// The minimum width of the content.
        /// </value>
        public double ContentMinWidth
        {
            get { return (double)GetValue(ContentMinWidthProperty); }
            set { SetValue(ContentMinWidthProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="ContentMinWidth"/>.
        /// </summary>
        public static readonly DependencyProperty ContentMinWidthProperty =
            DependencyProperty.Register("ContentMinWidth", typeof(double), typeof(MessageBoxControl), new PropertyMetadata(450d));




        /// <summary>
        /// Gets or sets the maximum width of the content.
        /// </summary>
        /// <value>
        /// The maximum width of the content.
        /// </value>
        public double ContentMaxWidth
        {
            get { return (double)GetValue(ContentMaxWidthProperty); }
            set { SetValue(ContentMaxWidthProperty, value); }
        }

        /// <summary>
        /// The dependency property for <see cref="ContentMaxWidth"/>.
        /// </summary>
        public static readonly DependencyProperty ContentMaxWidthProperty =
            DependencyProperty.Register("ContentMaxWidth", typeof(double), typeof(MessageBoxControl), new PropertyMetadata(600d));

        Image _iconWarning;
        Image _iconInfo;
        Image _iconError;
        Image _iconQuest;
        TextBlock _txtTitle;
        Button _btnOK;
        Button _btnYes;
        Button _btnNo;
        Button _btnCancel;
        Panel _btnPanel;
        MessageBoxResult _defResult;
        MessageBoxResult _result;
        MessageBoxImage _icon;
        MessageBoxButton _button;
        string _title;
        bool _templated;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (!_templated)
            {
                _iconWarning = GetTemplateChild("PART_iconWarning") as Image;
                _iconInfo = GetTemplateChild("PART_iconInfo") as Image;
                _iconError = GetTemplateChild("PART_iconError") as Image;
                _iconQuest = GetTemplateChild("PART_iconQuest") as Image;
                _txtTitle = GetTemplateChild("PART_txtTitle") as TextBlock;
                _btnOK = GetTemplateChild("PART_btnOK") as Button;
                _btnYes = GetTemplateChild("PART_btnYes") as Button;
                _btnNo = GetTemplateChild("PART_btnNo") as Button;
                _btnCancel = GetTemplateChild("PART_btnCancel") as Button;
                _btnPanel = GetTemplateChild("PART_btnPanel") as Panel;

                _btnOK.Click += _btnOK_Click;
                _btnCancel.Click += _btnCancel_Click;
                _btnNo.Click += _btnNo_Click;
                _btnYes.Click += _btnYes_Click;

                _templated = true;
                ApplyOptions();
            }
            base.OnApplyTemplate();
        }

        void _btnYes_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Yes;
            DialogResult = true;
        }

        void _btnNo_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.No;
            DialogResult = false;
        }

        void _btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Cancel;
            DialogResult = false;
        }

        void _btnOK_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.OK;
            DialogResult = true;
        }

        /// <summary>
        /// Called when dialog has been shown and focus needs to happen.
        /// </summary>
        protected override void OnFocus()
        {
            if (_templated)
            {
                bool focused = false;
                switch (_defResult)
                {
                    case System.Windows.MessageBoxResult.No:
                        if (_btnNo.Visibility == System.Windows.Visibility.Visible)
                        {
                            _btnNo.IsDefault = true;
                            _btnNo.Focus();
                            focused = true;
                        }
                        break;
                    case System.Windows.MessageBoxResult.Yes:
                        if (_btnYes.Visibility == System.Windows.Visibility.Visible)
                        {
                            _btnYes.IsDefault = true;
                            _btnYes.Focus();
                            focused = true;
                        }
                        break;
                    case System.Windows.MessageBoxResult.Cancel:
                        if (_btnCancel.Visibility == System.Windows.Visibility.Visible)
                        {
                            _btnCancel.IsDefault = true;
                            _btnCancel.Focus();
                            focused = true;
                        }
                        break;
                }

                if (!focused)
                {
                    foreach (Button c in _btnPanel.Children)
                    {
                        if (c.Visibility == System.Windows.Visibility.Visible)
                        {
                            c.IsDefault = true;
                            c.Focus();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Shows the dialog with icon/caption/buttons.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultResult">The default result.</param>
        public MessageBoxResult ShowDialogModal(DialogControlContainer owner, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            _title = caption;
            _button = button;
            _icon = icon;
            _defResult = defaultResult;
            ApplyOptions();
            ShowDialogModal(owner);
            return _result;
        }

        private void ApplyOptions()
        {
            if (_templated)
            {
                _txtTitle.Text = _title;

                _iconWarning.Visibility = System.Windows.Visibility.Collapsed;
                _iconInfo.Visibility = System.Windows.Visibility.Collapsed;
                _iconError.Visibility = System.Windows.Visibility.Collapsed;
                _iconQuest.Visibility = System.Windows.Visibility.Collapsed;

                switch (_icon)
                {
                    case MessageBoxImage.Error:
                        _iconError.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case MessageBoxImage.Question:
                        _iconQuest.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case MessageBoxImage.Warning:
                        _iconWarning.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case MessageBoxImage.Information:
                        _iconInfo.Visibility = System.Windows.Visibility.Visible;
                        break;
                }

                _btnCancel.Visibility = System.Windows.Visibility.Collapsed;
                _btnOK.Visibility = System.Windows.Visibility.Collapsed;
                _btnYes.Visibility = System.Windows.Visibility.Collapsed;
                _btnNo.Visibility = System.Windows.Visibility.Collapsed;
                switch (_button)
                {
                    case MessageBoxButton.YesNo:
                        _btnYes.Visibility = System.Windows.Visibility.Visible;
                        _btnNo.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case MessageBoxButton.YesNoCancel:
                        _btnYes.Visibility = System.Windows.Visibility.Visible;
                        _btnNo.Visibility = System.Windows.Visibility.Visible;
                        _btnCancel.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case MessageBoxButton.OKCancel:
                        _btnCancel.Visibility = System.Windows.Visibility.Visible;
                        _btnOK.Visibility = System.Windows.Visibility.Visible;
                        break;
                    default:
                        _btnOK.Visibility = System.Windows.Visibility.Visible;
                        break;
                }
            }
        }
    }
}
