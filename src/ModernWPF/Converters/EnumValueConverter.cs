using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ModernWPF.Converters
{
    /// <summary>
    /// Converts an enum to its underlying integer value.
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(int))]
    public class EnumValueConverter : IValueConverter
    {
        static readonly EnumValueConverter _instance = new EnumValueConverter();

        /// <summary>
        /// Gets the singleton instance for this converter.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static EnumValueConverter Instance { get { return _instance; } }

        /// <summary>
        /// Converts the enum to integer value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) { return value; }
            var type = value.GetType();
            return System.Convert.ChangeType(value, Type.GetTypeCode(type));
        }

        /// <summary>
        /// Converts the value back to enum.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) { return value; }
            return Enum.Parse(targetType, Enum.GetName(targetType, value));
        }
    }
}
