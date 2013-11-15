using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernWPF.Converters;
using System.Windows;
using System.Globalization;

namespace ModernWPF.Tests
{
    [TestClass]
    public class BoolVisibleConverterTest
    {
        [TestMethod]
        public void ConvertBack_Converts_To_Unset()
        {
            var conv = new BoolVisibleConverter();

            var result = conv.ConvertBack(Visibility.Collapsed, typeof(bool), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(DependencyProperty.UnsetValue, result, "No longer an unsupported operation?");
        }

        [TestMethod]
        public void Null_Converts_To_Collapsed()
        {
            var conv = new BoolVisibleConverter();

            var result = conv.Convert(null, typeof(Visibility), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Collapsed, result, "Failed to convert null.");
        }
        [TestMethod]
        public void Null_With_Not_Param_Converts_To_Visible()
        {
            var conv = new BoolVisibleConverter();

            var result = conv.Convert(null, typeof(Visibility), "not", CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Visible, result, "Failed to convert not null.");
        }

        [TestMethod]
        public void Unsupported_Obj_Converts_To_Collapsed()
        {
            var conv = new BoolVisibleConverter();

            var result = conv.Convert(new { blah = "blah" }, typeof(Visibility), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Collapsed, result, "Failed to convert null.");
        }
        [TestMethod]
        public void Unsupported_Obj_With_Not_Param_Converts_To_Visible()
        {
            var conv = new BoolVisibleConverter();

            var result = conv.Convert(new { blah = "blah" }, typeof(Visibility), "not", CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Visible, result, "Failed to convert not null.");
        }

        [TestMethod]
        public void False_Converts_To_Collapsed()
        {
            var conv = new BoolVisibleConverter();

            var result = conv.Convert(false, typeof(Visibility), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Collapsed, result, "Failed to convert false.");
        }
        [TestMethod]
        public void False_With_Not_Param_Converts_To_Visible()
        {
            var conv = new BoolVisibleConverter();

            var result = conv.Convert(false, typeof(Visibility), "not", CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Visible, result, "Failed to convert not false.");
        }

        [TestMethod]
        public void True_Converts_To_Visible()
        {
            var conv = new BoolVisibleConverter();

            var result = conv.Convert(true, typeof(Visibility), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Visible, result, "Failed to convert true.");
        }
        [TestMethod]
        public void True_With_Not_Param_Converts_To_Collapsed()
        {
            var conv = new BoolVisibleConverter();

            var result = conv.Convert(true, typeof(Visibility), "not", CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Collapsed, result, "Failed to convert not true.");
        }
    }
}
