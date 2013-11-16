using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernWPF.Converters;
using System.Windows;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ModernWPF.Tests
{
    [TestClass]
    public class AppIconImageConverterTest
    {
        [TestMethod]
        public void Singleton_Is_Not_Null()
        {
            Assert.IsNotNull(AppIconImageConverter.Instance);
        }
        [TestMethod]
        public void Singleton_Returns_The_Same_Instance()
        {
            Assert.AreSame(AppIconImageConverter.Instance, AppIconImageConverter.Instance);
        }

        [TestMethod]
        public void ConvertBack_Converts_To_Unset()
        {
            var conv = new AppIconImageConverter();

            var result = conv.ConvertBack(null, typeof(bool), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(DependencyProperty.UnsetValue, result, "No longer an unsupported operation?");
        }

        [TestMethod]
        public void Null_Converts_To_Detected_AppIcon()
        {
            var conv = new AppIconImageConverter();

            var result = conv.Convert(null, typeof(ImageSource), null, CultureInfo.CurrentCulture);

            Assert.AreSame(AppIconImageConverter.AppIcon, result);
        }

        [TestMethod]
        public void Not_Null_Converts_To_Input()
        {
            var conv = new AppIconImageConverter();
            // an 8x3 bw image
            var input = BitmapSource.Create(8, 3, 96, 96, PixelFormats.BlackWhite, BitmapPalettes.BlackAndWhite, new byte[] { 0, 255, 0 }, 1);

            var result = conv.Convert(input, typeof(ImageSource), "not", CultureInfo.CurrentCulture);

            Assert.AreSame(input, result);
        }
    }
}
