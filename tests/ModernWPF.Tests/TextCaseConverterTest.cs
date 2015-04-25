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
    public class TextCaseConverterTest
    {
        [TestMethod]
        public void Singleton_Is_Not_Null()
        {
            Assert.IsNotNull(TextCaseConverter.Instance);
        }
        [TestMethod]
        public void Singleton_Returns_The_Same_Instance()
        {
            Assert.AreSame(TextCaseConverter.Instance, TextCaseConverter.Instance);
        }

        [TestMethod]
        public void ConvertBack_Converts_To_Unset()
        {
            var conv = new TextCaseConverter();

            var result = conv.ConvertBack(true, typeof(double), null, CultureInfo.CurrentCulture);

            Assert.AreEqual(DependencyProperty.UnsetValue, result, "No longer an unsupported operation?");
        }

        [TestMethod]
        public void Null_Converts_To_Null()
        {
            var conv = new TextCaseConverter();

            var result = conv.Convert(null, typeof(string), null, CultureInfo.CurrentCulture);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void No_Parameter_Converts_To_Upper_Case()
        {
            var conv = new TextCaseConverter();

            var result = conv.Convert("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus ut porta lectus, nec tincidunt mauris.", typeof(string), null, CultureInfo.CurrentCulture);

            Assert.AreEqual("LOREM IPSUM DOLOR SIT AMET, CONSECTETUR ADIPISCING ELIT. VIVAMUS UT PORTA LECTUS, NEC TINCIDUNT MAURIS.", result);
        }

        [TestMethod]
        public void Lower_Parameter_Converts_To_Lower_Case()
        {
            var conv = new TextCaseConverter();

            var result = conv.Convert("LOREM IPSUM DOLOR SIT AMET, CONSECTETUR ADIPISCING ELIT. VIVAMUS UT PORTA LECTUS, NEC TINCIDUNT MAURIS.", typeof(string), "lower", CultureInfo.CurrentCulture);

            Assert.AreEqual("lorem ipsum dolor sit amet, consectetur adipiscing elit. vivamus ut porta lectus, nec tincidunt mauris.", result);
        }

        [TestMethod]
        public void Title_Parameter_Converts_To_Title_Case()
        {
            var conv = new TextCaseConverter();
            var input = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus ut porta lectus, nec tincidunt mauris.";

            var result = conv.Convert(input, typeof(string), "title", CultureInfo.CurrentCulture);

            Assert.AreEqual("Lorem Ipsum Dolor Sit Amet, Consectetur Adipiscing Elit. Vivamus Ut Porta Lectus, Nec Tincidunt Mauris.", result);
        }
    }
}
