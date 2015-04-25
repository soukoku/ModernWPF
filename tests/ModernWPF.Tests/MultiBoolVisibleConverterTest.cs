using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernWPF.Converters;
using System.Windows;
using System.Globalization;

namespace ModernWPF.Tests
{
    [TestClass]
    public class MultiBoolVisibleConverterTest
    {
        [TestMethod]
        public void Singleton_Is_Not_Null()
        {
            Assert.IsNotNull(MultiBoolVisibleConverter.Instance);
        }
        [TestMethod]
        public void Singleton_Returns_The_Same_Instance()
        {
            Assert.AreSame(MultiBoolVisibleConverter.Instance, MultiBoolVisibleConverter.Instance);
        }

        [TestMethod]
        public void ConvertBack_Returns_Null()
        {
            var conv = new MultiBoolVisibleConverter();

            var result = conv.ConvertBack(null, null, null, CultureInfo.CurrentCulture);

            Assert.IsNull(result, "No longer an unsupported operation?");
        }

        [TestMethod]
        public void Null_Converts_To_Collapsed()
        {
            var conv = new MultiBoolVisibleConverter();

            var result = conv.Convert(null, typeof(Visibility), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Collapsed, result);
        }
        [TestMethod]
        public void Null_With_Not_Param_Converts_To_Visible()
        {
            var conv = new MultiBoolVisibleConverter();

            var result = conv.Convert(null, typeof(Visibility), "not", CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestMethod]
        public void False_Array_Converts_To_Collapsed()
        {
            var conv = new MultiBoolVisibleConverter();

            var result = conv.Convert(new object[] { false, false }, typeof(Visibility), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Collapsed, result);
        }
        [TestMethod]
        public void False_Array_With_Not_Param_Converts_To_Visible()
        {
            var conv = new MultiBoolVisibleConverter();

            var result = conv.Convert(new object[] { false, false }, typeof(Visibility), "not", CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestMethod]
        public void True_Array_Converts_To_Visible()
        {
            var conv = new MultiBoolVisibleConverter();

            var result = conv.Convert(new object[] { true, true }, typeof(Visibility), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Visible, result);
        }
        [TestMethod]
        public void True_Array_With_Not_Param_Converts_To_Collapsed()
        {
            var conv = new MultiBoolVisibleConverter();

            var result = conv.Convert(new object[] { true, true }, typeof(Visibility), "not", CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestMethod]
        public void True_False_Array_Converts_To_Collapsed()
        {
            var conv = new MultiBoolVisibleConverter();

            var result = conv.Convert(new object[] { true, false }, typeof(Visibility), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Collapsed, result);
        }
        [TestMethod]
        public void True_False_Array_With_Not_Param_Converts_To_Visible()
        {
            var conv = new MultiBoolVisibleConverter();

            var result = conv.Convert(new object[] { true, false }, typeof(Visibility), "not", CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestMethod]
        public void True_Visible_Array_Converts_To_Visible()
        {
            var conv = new MultiBoolVisibleConverter();

            var result = conv.Convert(new object[] { true, Visibility.Visible }, typeof(Visibility), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Visible, result);
        }
        [TestMethod]
        public void True_Visible_Array_With_Not_Param_Converts_To_Collapsed()
        {
            var conv = new MultiBoolVisibleConverter();

            var result = conv.Convert(new object[] { true, Visibility.Visible }, typeof(Visibility), "not", CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestMethod]
        public void True_Collapsed_Array_Converts_To_Collapsed()
        {
            var conv = new MultiBoolVisibleConverter();

            var result = conv.Convert(new object[] { true, Visibility.Collapsed }, typeof(Visibility), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Collapsed, result);
        }
        [TestMethod]
        public void True_Collapsed_Array_With_Not_Param_Converts_To_Visible()
        {
            var conv = new MultiBoolVisibleConverter();

            var result = conv.Convert(new object[] { true, Visibility.Collapsed }, typeof(Visibility), "not", CultureInfo.CurrentCulture);

            Assert.AreEqual(Visibility.Visible, result);
        }
    }
}
