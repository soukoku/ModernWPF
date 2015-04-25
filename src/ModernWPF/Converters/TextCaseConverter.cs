using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace ModernWPF.Converters
{
    /// <summary>
    /// Provides conversion of text to upper (default), lower, or title cases.
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class TextCaseConverter : IValueConverter
    {
        static readonly TextCaseConverter _instance = new TextCaseConverter();

        /// <summary>
        /// Gets the singleton instance for this converter.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static TextCaseConverter Instance { get { return _instance; } }

        #region IValueConverter Members

        /// <summary>
        /// Converts a value to the string representation.
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
            if (value != null)
            {
                if (culture == null) { culture = System.Globalization.CultureInfo.CurrentCulture; }
                if (parameter != null)
                {
                    if (string.Equals("lower", parameter.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        return culture.TextInfo.ToLower(value.ToString());
                    }
                    if (string.Equals("title", parameter.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        return culture.TextInfo.ToTitleCase(value.ToString());
                    }
                }
                return culture.TextInfo.ToUpper(value.ToString());
            }
            return value;
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
