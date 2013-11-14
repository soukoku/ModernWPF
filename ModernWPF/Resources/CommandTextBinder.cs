using System.ComponentModel;
using System.Globalization;

namespace ModernWPF.Resources
{
    /// <summary>
    /// Provides bindable text for window commands.
    /// The purpose of this is to support on-the-fly language changes.
    /// </summary>
    public class CommandTextBinder : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the singleton <see cref="CommandTextBinder"/> object.
        /// </summary>
        public static readonly CommandTextBinder Instance = new CommandTextBinder();

        private CommandTextBinder() { }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <value></value>
        public string this[string key]
        {
            get
            {
                return CommandText.ResourceManager.GetString(key, CommandText.Culture);
            }
        }

        /// <summary>
        /// Updates the culture used for the command text. Pass null to use current culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        public void UpdateCulture(CultureInfo culture)
        {
            CommandText.Culture = culture;
            PropertyChanged(this, new PropertyChangedEventArgs("Item[]"));
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion
    }
}
