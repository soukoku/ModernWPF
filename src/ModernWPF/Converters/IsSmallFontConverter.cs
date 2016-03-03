using CommonWin32.API;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Globalization;

namespace ModernWPF.Converters
{
    /// <summary>
    /// A converter that tests if a TextBlock's font size is considered a small font size to allow changing TextOptions.
    /// </summary>
    public class IsSmallFontConverter : IMultiValueConverter
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
        /// Converts a TextBlock value to boolean true if too small.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var tooSmall = false;
            if (values != null)
            {
                foreach (var value in values)
                {
                    var tb = value as TextBlock;
                    if (tb != null)
                    {
                        var dpi = DpiEvents.GetWindowDpi(tb);

                        tooSmall = dpi <= 96 && tb.FontSize <= Threshold;
                        break;
                    }
                }
            }
            return tooSmall;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetTypes">The target types.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
