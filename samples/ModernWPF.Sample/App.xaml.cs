using CommonWin32;
using CommonWin32.API;
using CommonWin32.HighDPI;
using ModernWPF.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace ModernWPF.Sample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Global Exception: " + e.Exception.ToString());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Shcore.ProcessDpiAwareness = PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE;

            base.OnStartup(e);
        }
    }
}