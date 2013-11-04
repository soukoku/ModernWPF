using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ModernWPF
{
    /// <summary>
    /// Allows retrieval of theme resources based on an accent color.
    /// </summary>
    public static class ModernTheme
    {
        static readonly ResourceDictionary LIGHT_THEME = GetResource(@"/ModernWPF;component/themes/ModernLight.xaml");
        static readonly ResourceDictionary DARK_THEME = GetResource(@"/ModernWPF;component/themes/ModernDark.xaml");

        internal static ResourceDictionary GetResource(string url)
        {
            var style = new ResourceDictionary();
            style.Source = new Uri(url, UriKind.Relative);
            return style;
        }

        /// <summary>
        /// Gets the predefined accent with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Accent GetPredefinedAccent(string name)
        {
            return PredefinedAccents.Where(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }


        #region known accent names

        /// <summary>
        /// Pre-defined name for the red accent.
        /// </summary>
        public const string RED = "Red";
        /// <summary>
        /// Pre-defined name for the orange accent.
        /// </summary>
        public const string ORANGE = "Orange";
        /// <summary>
        /// Pre-defined name for the green accent.
        /// </summary>
        public const string GREEN = "Green";
        /// <summary>
        /// Pre-defined name for the teal accent.
        /// </summary>
        public const string TEAL = "Teal";
        /// <summary>
        /// Pre-defined name for the olive accent.
        /// </summary>
        public const string OLIVE = "Olive";
        /// <summary>
        /// Pre-defined name for the gold accent.
        /// </summary>
        public const string GOLD = "Gold";

        /// <summary>
        /// Pre-defined name for the light blue accent.
        /// </summary>
        public const string LIGHTBLUE = "Light Blue";
        /// <summary>
        /// Pre-defined name for the dark blue accent.
        /// </summary>
        public const string DARKBLUE = "Dark Blue";
        /// <summary>
        /// Pre-defined name for the purple accent.
        /// </summary>
        public const string PURPLE = "Purple";

        #endregion

        /// <summary>
        /// Gets the predefined accents colors.
        /// </summary>
        public static readonly IEnumerable<Accent> PredefinedAccents = CreateDefaultAccents();

        private static IEnumerable<Accent> CreateDefaultAccents()
        {
            return new Accent[]{
                new Accent(RED, (Color)ColorConverter.ConvertFromString("#CD3333")),
                new Accent(ORANGE, Colors.Chocolate),
                new Accent(GOLD,(Color)ColorConverter.ConvertFromString("#CDAD00")),
                new Accent(OLIVE,(Color)ColorConverter.ConvertFromString("#6B8E23")),
                new Accent(TEAL,(Color)ColorConverter.ConvertFromString("#00959D")),
                new Accent(GREEN, Colors.ForestGreen),
                new Accent(LIGHTBLUE, Colors.DodgerBlue),
                new Accent(DARKBLUE,(Color)ColorConverter.ConvertFromString("#007ACC")),
                new Accent(PURPLE, Colors.MediumOrchid),
            };
        }



        /// <summary>
        /// Indicates the main theme style.
        /// </summary>
        public enum ThemeType
        {
            /// <summary>
            /// Theme has light background.
            /// </summary>
            Light,
            /// <summary>
            /// Theme has dark background.
            /// </summary>
            Dark
        }


        /// <summary>
        /// Applies the theme with the accent color theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <param name="accent">The accent.</param>
        public static void ApplyTheme(ThemeType theme, Accent accent)
        {
            if (accent == null) { throw new ArgumentNullException("accent"); }

            ApplyResources(theme == ThemeType.Light ? LIGHT_THEME : DARK_THEME);
            Application.Current.Resources["ModernAccent"] = accent.Brush;
            Application.Current.Resources["ModernAccent2"] = accent.Brush2;
            Application.Current.Resources["ModernAccent3"] = accent.Brush3;
            Application.Current.Resources["ModernAccent4"] = accent.Brush4;
            Application.Current.Resources["ModernAccent5"] = accent.Brush5;
            Application.Current.Resources["ModernAccent6"] = accent.Brush6;
            Application.Current.Resources["ModernAccent7"] = accent.Brush7;
            Application.Current.Resources["ModernAccent8"] = accent.Brush8;
            Application.Current.Resources["ModernAccent9"] = accent.Brush9;
            Application.Current.Resources["ModernAccent10"] = accent.Brush10;
        }

        private static void ApplyResources(ResourceDictionary resources)
        {
            foreach (var k in resources.Keys)
            {
                Application.Current.Resources[k] = resources[k];
            }
        }
    }
}
