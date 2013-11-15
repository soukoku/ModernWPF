using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernWPF.Converters;
using System.Windows;
using System.Globalization;

namespace ModernWPF.Tests
{
    [TestClass]
    public class NullEmptyCollapsedConverterTest
    {
        [TestMethod]
        public void Singleton_Is_Not_Null()
        {
            Assert.IsNotNull(NullEmptyCollapsedConverter.Instance);
        }
        [TestMethod]
        public void Singleton_Returns_The_Same_Instance()
        {
            Assert.AreSame(NullEmptyCollapsedConverter.Instance, NullEmptyCollapsedConverter.Instance);
        }

        [TestMethod]
        public void ConvertBack_Converts_To_Unset()
        {
            var conv = new NullEmptyCollapsedConverter();

            var result = conv.ConvertBack(Visibility.Collapsed, typeof(bool), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(DependencyProperty.UnsetValue, result, "No longer an unsupported operation?");
        }

        [TestMethod]
        public void Null_Converts_To_Collapsed()
        {
            var conv = new NullEmptyCollapsedConverter();

            var result = conv.Convert(null, typeof(Visibility), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Collapsed, result);
        }
        [TestMethod]
        public void Null_With_Not_Param_Converts_To_Visible()
        {
            var conv = new NullEmptyCollapsedConverter();

            var result = conv.Convert(null, typeof(Visibility), "not", CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestMethod]
        public void Empty_String_Converts_To_Collapsed()
        {
            var conv = new NullEmptyCollapsedConverter();

            var result = conv.Convert(string.Empty, typeof(Visibility), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Collapsed, result);
        }
        [TestMethod]
        public void Empty_String_With_Not_Param_Converts_To_Visible()
        {
            var conv = new NullEmptyCollapsedConverter();

            var result = conv.Convert(string.Empty, typeof(Visibility), "not", CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestMethod]
        public void Any_Object_Converts_To_Visible()
        {
            var conv = new NullEmptyCollapsedConverter();

            var result = conv.Convert(new { blah = "blah" }, typeof(Visibility), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Visible, result);
        }
        [TestMethod]
        public void Any_Object_With_Not_Param_Converts_To_Collapsed()
        {
            var conv = new NullEmptyCollapsedConverter();

            var result = conv.Convert(new { blah = "blah" }, typeof(Visibility), "not", CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Collapsed, result);
        }

    }
}
