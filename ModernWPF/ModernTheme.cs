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
        static readonly ResourceDictionary LIGHT_THEME = GetResource("/ModernWPF;component/themes/ModernLight.xaml");
        static readonly ResourceDictionary DARK_THEME = GetResource("/ModernWPF;component/themes/ModernDark.xaml");

        internal static ResourceDictionary GetResource(string url)
        {
            var style = new ResourceDictionary();
            style.Source = new Uri(url, UriKind.Relative);
            return style;
        }

        #region predefined accents

        /// <summary>
        /// Gets the predefined accent with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Accent GetPredefinedAccent(string name)
        {
            return PredefinedAccents.Where(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        static ModernTheme()
        {
            PredefinedAccents = new Accent[]{
                new Accent(Accent.RED, (Color)ColorConverter.ConvertFromString("#CD3333")),
                new Accent(Accent.ORANGE, Colors.Chocolate),
                //new Accent(Accent.GOLD,(Color)ColorConverter.ConvertFromString("#CDAD00")),
                new Accent(Accent.GOLD,Colors.Goldenrod),
                new Accent(Accent.OLIVE,(Color)ColorConverter.ConvertFromString("#6B8E23")),
                new Accent(Accent.TEAL,(Color)ColorConverter.ConvertFromString("#00959D")),
                new Accent(Accent.GREEN, Colors.ForestGreen),
                new Accent(Accent.LIGHT_BLUE, Colors.DodgerBlue),
                new Accent(Accent.DARK_BLUE,(Color)ColorConverter.ConvertFromString("#007ACC")),
                new Accent(Accent.LIGHT_PURPLE, Colors.MediumOrchid),
                new Accent(Accent.DARK_PURPLE, Colors.BlueViolet),
            };
        }

        /// <summary>
        /// Gets the predefined accents colors.
        /// </summary>
        public static IEnumerable<Accent> PredefinedAccents { get; private set; }

        #endregion

        /// <summary>
        /// Indicates the main theme style.
        /// </summary>
        public enum Theme
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

        static Accent _curAccent;
        /// <summary>
        /// Gets the current accent.
        /// </summary>
        /// <value>
        /// The current accent.
        /// </value>
        public static Accent CurrentAccent
        {
            get
            {
                if (_curAccent == null) { _curAccent = GetPredefinedAccent(Accent.LIGHT_BLUE); }
                return _curAccent;
            }
        }


        /// <summary>
        /// Gets the current theme.
        /// </summary>
        /// <value>
        /// The current theme.
        /// </value>
        public static Theme? CurrentTheme { get; private set; }


        /// <summary>
        /// Applies the theme with the name of the accent color theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <param name="predefinedAccentName">Name of the predefined accent. These can be found in the <see cref="Accent"/> class.</param>
        public static void ApplyTheme(Theme theme, string predefinedAccentName)
        {
            var accent = GetPredefinedAccent(predefinedAccentName);
            if (accent != null)
            {
                ApplyTheme(theme, accent);
            }
        }

        /// <summary>
        /// Applies the theme with the accent color theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <param name="accent">The accent.</param>
        public static void ApplyTheme(Theme theme, Accent accent)
        {
            if (accent == null) { throw new ArgumentNullException("accent"); }

            _curAccent = accent;
            CurrentTheme = theme;

            ApplyResources(theme == Theme.Light ? LIGHT_THEME : DARK_THEME);
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
