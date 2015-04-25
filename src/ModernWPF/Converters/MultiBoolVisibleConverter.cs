using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ModernWPF.Converters
{
    /// <summary>
    /// Converts multiple <see cref="Boolean"/> or <see cref="Visibility"/> values into a single <see cref="Visibility"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
    public class MultiBoolVisibleConverter : IMultiValueConverter
    {
        static readonly MultiBoolVisibleConverter _instance = new MultiBoolVisibleConverter();

        /// <summary>
        /// Gets the singleton instance for this converter.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static MultiBoolVisibleConverter Instance { get { return _instance; } }

        #region IMultiValueConverter Members

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? retVal = null;
            if (values != null)
            {
                foreach (var val in values)
                {
                    var asBool = val as bool?;
                    if (asBool != null)
                    {
                        if (retVal.HasValue) { retVal = retVal.Value && asBool.Value; }
                        else { retVal = asBool.Value; }
                    }
                    else
                    {
                        var asVis = val as Visibility?;
                        if (asVis != null)
                        {
                            if (retVal.HasValue) { retVal = retVal.Value && (asVis.Value == Visibility.Visible); }
                            else { retVal = asVis.Value == Visibility.Visible; }
                        }
                    }
                }
            }
            if (parameter != null && string.Equals("not", parameter.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                retVal = !retVal.GetValueOrDefault();
            }
            return retVal.GetValueOrDefault() ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
