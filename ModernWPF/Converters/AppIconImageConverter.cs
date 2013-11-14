using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernWPF.Converters
{
    /// <summary>
    /// A converter for the <see cref="Window.Icon"/> property that if not set, will return the application's icon (i.e. from the exe file).
    /// </summary>
    public class AppIconImageConverter : IValueConverter
    {
        static readonly AppIconImageConverter _instance = new AppIconImageConverter();

        /// <summary>
        /// Gets the singleton instance for this converter.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static AppIconImageConverter Instance { get { return _instance; } }

        static readonly ImageSource __appIcon = TryGetAppIcon();

        private static ImageSource TryGetAppIcon()
        {
            try
            {
                var exe = Assembly.GetEntryAssembly().Location;
                using (var icon = Icon.ExtractAssociatedIcon(exe))
                {
                    var img = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    if (img.CanFreeze)
                    {
                        img.Freeze();
                    }
                    return img;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to extract icon: {0}", ex);
            }
            return null;
        }

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
            if (value == null)
            {
                return __appIcon;
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
