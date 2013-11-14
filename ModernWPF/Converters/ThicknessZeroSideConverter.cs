using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ModernWPF.Converters
{
    /// <summary>
    /// Converts a <see cref="Thickness"/> or number to another <see cref="Thickness"/> with the specified sides to 0. 
    /// Side parameters can be top, left, right, or bottom.
    /// </summary>
    [ValueConversion(typeof(Thickness), typeof(Thickness))]
    [ValueConversion(typeof(double), typeof(Thickness))]
    public class ThicknessZeroSideConverter : IValueConverter
    {
        static readonly char[] __splitChars = new char[] { ',', ' ' };

        static ThicknessZeroSideConverter()
        {
            Instance = new ThicknessZeroSideConverter();
        }

        /// <summary>
        /// Gets the singleton instance for this converter.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ThicknessZeroSideConverter Instance { get; private set; }

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
            if (value != null && value is Thickness && parameter != null)
            {
                Thickness target = default(Thickness);

                if (value is Thickness)
                {
                    target = (Thickness)value;
                }
                else
                {
                    double test = 0;
                    if (double.TryParse(value.ToString(), out test))
                    {
                        target = new Thickness(test);
                    }
                }
                foreach (var para in parameter.ToString().Split(__splitChars, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (string.Equals(para, "top", StringComparison.OrdinalIgnoreCase))
                    {
                        target.Top = 0;
                    }
                    else if (string.Equals(para, "left", StringComparison.OrdinalIgnoreCase))
                    {
                        target.Left = 0;
                    }
                    else if (string.Equals(para, "right", StringComparison.OrdinalIgnoreCase))
                    {
                        target.Right = 0;
                    }
                    else if (string.Equals(para, "bottom", StringComparison.OrdinalIgnoreCase))
                    {
                        target.Bottom = 0;
                    }
                }
                return target;
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
