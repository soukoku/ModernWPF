using System;
using System.Windows.Data;

namespace ModernWPF.Converters
{
    /// <summary>
    /// A converter that tests if a double value considered a small font size to allow changing TextOptions.
    /// This can be removed from the fonts style if using high dpi displays.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used in xaml.")]
    class IsSmallFontConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                return (double)value <= 14;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
