using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernWPF.Converters;
using System.Windows;
using System.Globalization;

namespace ModernWPF.Tests
{
    [TestClass]
    public class ThicknessZeroSideConverterTest
    {
        [TestMethod]
        public void Singleton_Is_Not_Null()
        {
            Assert.IsNotNull(ThicknessZeroSideConverter.Instance);
        }
        [TestMethod]
        public void Singleton_Returns_The_Same_Instance()
        {
            Assert.AreSame(ThicknessZeroSideConverter.Instance, ThicknessZeroSideConverter.Instance);
        }

        [TestMethod]
        public void ConvertBack_Converts_To_Unset()
        {
            var conv = new ThicknessZeroSideConverter();

            var result = conv.ConvertBack(new Thickness(), typeof(Thickness), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(DependencyProperty.UnsetValue, result, "No longer an unsupported operation?");
        }

        [TestMethod]
        public void Single_Number_Converts_To_Regular_Thickness()
        {
            var conv = new ThicknessZeroSideConverter();

            var result = conv.Convert(10, typeof(Thickness), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(new Thickness(10), result);
        }

        [TestMethod]
        public void Thickness_Converts_To_Same_Thickness()
        {
            var conv = new ThicknessZeroSideConverter();

            var result = conv.Convert(new Thickness(20, 15, 10, 5), typeof(Thickness), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(new Thickness(20, 15, 10, 5), result);
        }

        [TestMethod]
        public void Convet_Can_Set_Top_To_Zero()
        {
            var conv = new ThicknessZeroSideConverter();

            var result = conv.Convert(new Thickness(20, 15, 10, 5), typeof(Thickness), "top", CultureInfo.CurrentCulture);

            Assert.AreEqual(new Thickness(20, 0, 10, 5), result);
        }

        [TestMethod]
        public void Convet_Can_Set_Left_To_Zero()
        {
            var conv = new ThicknessZeroSideConverter();

            var result = conv.Convert(new Thickness(20, 15, 10, 5), typeof(Thickness), "left", CultureInfo.CurrentCulture);

            Assert.AreEqual(new Thickness(0, 15, 10, 5), result);
        }

        [TestMethod]
        public void Convet_Can_Set_Right_To_Zero()
        {
            var conv = new ThicknessZeroSideConverter();

            var result = conv.Convert(new Thickness(20, 15, 10, 5), typeof(Thickness), "right", CultureInfo.CurrentCulture);

            Assert.AreEqual(new Thickness(20, 15, 0, 5), result);
        }

        [TestMethod]
        public void Convet_Can_Set_Bottom_To_Zero()
        {
            var conv = new ThicknessZeroSideConverter();

            var result = conv.Convert(new Thickness(20, 15, 10, 5), typeof(Thickness), "bottom", CultureInfo.CurrentCulture);

            Assert.AreEqual(new Thickness(20, 15, 10, 0), result);
        }

        [TestMethod]
        public void Convet_Can_Set_All_To_Zero_With_Comma()
        {
            var conv = new ThicknessZeroSideConverter();

            var result = conv.Convert(new Thickness(20, 15, 10, 5), typeof(Thickness), "top,left,bottom,right", CultureInfo.CurrentCulture);

            Assert.AreEqual(new Thickness(), result);
        }

        [TestMethod]
        public void Convet_Can_Set_All_To_Zero_With_Space()
        {
            var conv = new ThicknessZeroSideConverter();

            var result = conv.Convert(new Thickness(20, 15, 10, 5), typeof(Thickness), "right bottom left top", CultureInfo.CurrentCulture);

            Assert.AreEqual(new Thickness(), result);
        }
    }
}
