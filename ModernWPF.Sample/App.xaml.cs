using ModernWPF.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;

namespace ModernWPF.Sample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        bool testCulture = true;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (testCulture)
            {
                var zhTW = new System.Globalization.CultureInfo("zh-TW");
                Thread.CurrentThread.CurrentUICulture = zhTW;
                //CommandTextBinder.Current.UpdateCulture(zhTW);
            }
            base.OnStartup(e);
        }
    }
}
