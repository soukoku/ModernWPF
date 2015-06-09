using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using ModernWPF.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModernWPF.Messages
{
    /// <summary>
    /// Contains common routines for handling messages on a view.
    /// </summary>
    public static class MessageRoutine
    {
        /// <summary>
        /// Handles the <see cref="ChooseFolderMessage" /> on a window by showing a folder dialog based on the message options.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">
        /// message
        /// </exception>
        public static void HandleChooseFolder(this Window owner, ChooseFolderMessage message)
        {
            //if (owner == null) { throw new ArgumentNullException("owner"); }
            if (message == null) { throw new ArgumentNullException("message"); }

            if (CommonOpenFileDialog.IsPlatformSupported)
            {
                using (var diag = new CommonOpenFileDialog())
                {
                    diag.InitialDirectory = message.InitialFolder;
                    diag.IsFolderPicker = true;
                    diag.Title = message.Caption;
                    diag.Multiselect = false;
                    // allow this for desktop, but now opens other locations up so need to check those
                    diag.AllowNonFileSystemItems = true;

                REOPEN:

                    var result = owner == null ? diag.ShowDialog() : diag.ShowDialog(owner);

                    if (result == CommonFileDialogResult.Ok)
                    {
                        ShellObject selectedSO = null;

                        try
                        {
                            // Try to get a valid selected item
                            selectedSO = diag.FileAsShellObject as ShellObject;
                        }
                        catch
                        {
                            MessageBox.Show("Could not create a ShellObject from the selected item");
                        }
                        if (selectedSO != null)
                        {
                            string name = selectedSO.Name;
                            string path = selectedSO.ParsingName;
                            bool notReal = selectedSO is ShellNonFileSystemFolder;
                            selectedSO.Dispose();
                            if (notReal)
                            {
                                if (path.EndsWith(".library-ms", StringComparison.OrdinalIgnoreCase))
                                {
                                    using (var lib = ShellLibrary.Load(name, true))
                                    {
                                        if (lib != null)
                                            path = lib.DefaultSaveFolder;
                                    }
                                }
                                else
                                {
                                    if (MessageBox.Show(string.Format("The location \"{0}\" is not valid, please select another.", name),
                                        "Invalid Location", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                                    {
                                        goto REOPEN;
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }
                            }

                            if (owner == null || owner.Dispatcher.CheckAccess())
                            {
                                message.DoCallback(path);
                            }
                            else
                            {
                                owner.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    message.DoCallback(path);
                                }));
                            }
                        }
                    }
                }
            }
            else
            {
                message.HandleWithPlatform(owner);
            }
        }
    }
}
