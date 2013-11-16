using System;
using System.Windows;
using System.Windows.Data;

namespace ModernWPF.Converters
{
    /// <summary>
    /// A converter that tests if a double value considered a small font size to allow changing TextOptions.
    /// This can be removed from the fonts style if using high dpi displays.
    /// </summary>
    [ValueConversion(typeof(double), typeof(bool))]
    public class IsSmallFontConverter : IValueConverter
    {
        static readonly IsSmallFontConverter _instance = new IsSmallFontConverter();

        /// <summary>
        /// Gets the singleton instance for this converter.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static IsSmallFontConverter Instance { get { return _instance; } }

        static double _threshold = 14;

        /// <summary>
        /// Gets or sets the threshold size for small.
        /// </summary>
        /// <value>
        /// The threshold.
        /// </value>
        public static double Threshold { get { return _threshold; } set { _threshold = value; } }

        /// <summary>
        /// Converts a double value to boolean true if too small.
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
            if (value is double)
            {
                return (double)value <= Threshold;
            }
            return false;
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
    }
}
