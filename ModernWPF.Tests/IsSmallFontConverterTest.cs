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
    public class IsSmallFontConverterTest
    {
        [TestMethod]
        public void Singleton_Is_Not_Null()
        {
            Assert.IsNotNull(IsSmallFontConverter.Instance);
        }
        [TestMethod]
        public void Singleton_Returns_The_Same_Instance()
        {
            Assert.AreSame(IsSmallFontConverter.Instance, IsSmallFontConverter.Instance);
        }

        [TestMethod]
        public void ConvertBack_Converts_To_Unset()
        {
            var conv = new IsSmallFontConverter();

            var result = conv.ConvertBack(true, typeof(double), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(DependencyProperty.UnsetValue, result, "No longer an unsupported operation?");
        }

        [TestMethod]
        public void Null_Converts_To_False()
        {
            var conv = new IsSmallFontConverter();
            
            var result = (bool)conv.Convert(null, typeof(bool), null, CultureInfo.CurrentCulture);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Greater_Than_Threshold_Converts_To_False()
        {
            var conv = new IsSmallFontConverter();
            var input = IsSmallFontConverter.Threshold + 1;

            var result = (bool)conv.Convert(input, typeof(bool), null, CultureInfo.CurrentCulture);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Less_Than_Equal_Threshold_Converts_To_True()
        {
            var conv = new IsSmallFontConverter();
            var input1 = IsSmallFontConverter.Threshold;
            var input2 = IsSmallFontConverter.Threshold - 1;

            Assert.IsTrue((bool)conv.Convert(input1, typeof(bool), null, CultureInfo.CurrentCulture));

            Assert.IsTrue((bool)conv.Convert(input2, typeof(bool), null, CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void Can_Change_Threshold()
        {
            var conv = new IsSmallFontConverter();
            var input = IsSmallFontConverter.Threshold + 1;

            Assert.IsFalse((bool)conv.Convert(input, typeof(bool), null, CultureInfo.CurrentCulture));

            IsSmallFontConverter.Threshold = input;

            Assert.IsTrue((bool)conv.Convert(input, typeof(bool), null, CultureInfo.CurrentCulture));
        }
    }
}
