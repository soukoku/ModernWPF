using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ModernWPF.Converters
{
    /// <summary>
    /// Converts to <see cref="Visibility.Visible"/> if the value is null or empty string.
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullEmptyNotVisibleConverter : IValueConverter
    {
        static NullEmptyNotVisibleConverter()
        {
            Instance = new NullEmptyNotVisibleConverter();
        }

        /// <summary>
        /// Gets the singleton instance for this converter.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static NullEmptyNotVisibleConverter Instance { get; private set; }

        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visible = value != null && !string.IsNullOrEmpty(value.ToString());
            if (parameter != null && string.Equals("not", parameter.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                visible = !visible;
            }
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}
