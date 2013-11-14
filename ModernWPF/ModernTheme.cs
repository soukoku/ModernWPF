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
                new Accent(Accent.Red, (Color)ColorConverter.ConvertFromString("#CD3333")),
                new Accent(Accent.Orange, Colors.Chocolate),
                //new Accent(Accent.GOLD,(Color)ColorConverter.ConvertFromString("#CDAD00")),
                new Accent(Accent.Gold,Colors.Goldenrod),
                new Accent(Accent.Olive,(Color)ColorConverter.ConvertFromString("#6B8E23")),
                new Accent(Accent.Teal,(Color)ColorConverter.ConvertFromString("#00959D")),
                new Accent(Accent.Green, Colors.ForestGreen),
                new Accent(Accent.LightBlue, Colors.DodgerBlue),
                new Accent(Accent.DarkBlue,(Color)ColorConverter.ConvertFromString("#007ACC")),
                new Accent(Accent.LightPurple, Colors.MediumOrchid),
                new Accent(Accent.DarkPurple, Colors.BlueViolet),
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
                if (_curAccent == null) { _curAccent = GetPredefinedAccent(Accent.LightBlue); }
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
            Application.Current.Resources["ModernAccent"] = accent.MainBrush;
            Application.Current.Resources["ModernAccentLight1"] = accent.LightBrush1;
            Application.Current.Resources["ModernAccentLight2"] = accent.LightBrush2;
            Application.Current.Resources["ModernAccentLight3"] = accent.LightBrush3;
            Application.Current.Resources["ModernAccentLight4"] = accent.LightBrush4;

            Application.Current.Resources["ModernAccentAlpha1"] = accent.AlphaBrush1;
            Application.Current.Resources["ModernAccentAlpha2"] = accent.AlphaBrush2;
            Application.Current.Resources["ModernAccentAlpha3"] = accent.AlphaBrush3;
            Application.Current.Resources["ModernAccentAlpha4"] = accent.AlphaBrush4;
            Application.Current.Resources["ModernAccentAlpha5"] = accent.AlphaBrush5;
            Application.Current.Resources["ModernAccentAlpha6"] = accent.AlphaBrush6;
            Application.Current.Resources["ModernAccentAlpha7"] = accent.AlphaBrush7;
            Application.Current.Resources["ModernAccentAlpha8"] = accent.AlphaBrush8;
            Application.Current.Resources["ModernAccentAlpha9"] = accent.AlphaBrush9;
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
