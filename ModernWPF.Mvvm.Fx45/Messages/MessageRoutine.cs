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
        /// Handles a basic <see cref="DialogMessage" /> on a window by showing a <see cref="ModernMessageBox" />.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// owner
        /// or
        /// message
        /// </exception>
        public static MessageBoxResult HandleDialogMessageModern(this Window owner, DialogMessage message)
        {
            if (owner == null) { throw new ArgumentNullException("owner"); }
            if (message == null) { throw new ArgumentNullException("message"); }

            return ModernMessageBox.Show(owner, message.Content, message.Caption, message.Button, message.Icon, message.DefaultResult);
        }

        /// <summary>
        /// Handles a basic <see cref="DialogMessage" /> on a window by showing built-in <see cref="MessageBox"/>.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// owner
        /// or
        /// message
        /// </exception>
        public static MessageBoxResult HandleDialogMessagePlatform(this Window owner, DialogMessage message)
        {
            if (owner == null) { throw new ArgumentNullException("owner"); }
            if (message == null) { throw new ArgumentNullException("message"); }

            return MessageBox.Show(owner, message.Content, message.Caption, message.Button, message.Icon, message.DefaultResult, message.Options);
        }


        /// <summary>
        /// Handles the <see cref="ChooseFileMessage" /> on a window by showing a <see cref="FileDialog" /> based on the message options.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">
        /// owner
        /// or
        /// message
        /// </exception>
        public static void HandleChooseFile(this Window owner, ChooseFileMessage message)
        {
            if (owner == null) { throw new ArgumentNullException("owner"); }
            if (message == null) { throw new ArgumentNullException("message"); }

            FileDialog dialog = null;

            switch (message.Purpose)
            {
                case ChooseFileMessage.FilePurpose.OpenMultiple:
                    var d = new OpenFileDialog();
                    d.Multiselect = true;
                    dialog = d;
                    break;
                case ChooseFileMessage.FilePurpose.OpenSingle:
                    dialog = new OpenFileDialog();
                    break;
                case ChooseFileMessage.FilePurpose.Save:
                    dialog = new SaveFileDialog();
                    break;
            }

            if (dialog != null)
            {
                dialog.Title = message.Caption;

                if (!string.IsNullOrEmpty(message.InitialFolder))
                    dialog.InitialDirectory = message.InitialFolder;
                if (!string.IsNullOrEmpty(message.InitialFileName))
                    dialog.FileName = message.InitialFileName;
                if (!string.IsNullOrEmpty(message.Filters))
                    dialog.Filter = message.Filters;


                if (dialog.ShowDialog(owner).GetValueOrDefault())
                {
                    owner.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        message.DoCallback(dialog.FileNames);
                    }));
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="RefreshCommandsMessage"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void HandleRefreshCommands(this RefreshCommandsMessage message)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    CommandManager.InvalidateRequerySuggested();
                }));
            }
        }

        /// <summary>
        /// Handles the <see cref="ChooseFileMessage"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void HandleOpenExplorer(this OpenExplorerMessage message)
        {
            if (message == null) { throw new ArgumentNullException("message"); }

            if (!string.IsNullOrEmpty(message.SelectedPath))
            {
                using (Process.Start("explorer", string.Format("/select,{0}", message.SelectedPath))) { }
            }
            else if (!string.IsNullOrEmpty(message.FolderPath))
            {
                using (Process.Start("explorer", message.FolderPath)) { }
            }
            else
            {
                using (Process.Start("explorer")) { }
            }
        }


        /// <summary>
        /// Handles the <see cref="ChooseFolderMessage" /> on a window by showing a folder dialog based on the message options.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">
        /// owner
        /// or
        /// message
        /// </exception>
        public static void HandleChooseFolder(this Window owner, ChooseFolderMessage message)
        {
            if (owner == null) { throw new ArgumentNullException("owner"); }
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
                    if (diag.ShowDialog(owner) == CommonFileDialogResult.Ok)
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
                                    if (MessageBox.Show(owner,
                                        string.Format("The location \"{0}\" is not valid, please select another.", name),
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

                            owner.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                message.DoCallback(path);
                            }));
                        }
                    }
                }
            }
            else
            {
                using (var diag = new System.Windows.Forms.FolderBrowserDialog())
                {
                    diag.ShowNewFolderButton = true;
                    diag.SelectedPath = message.InitialFolder;
                    if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        owner.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            message.DoCallback(diag.SelectedPath);
                        }));
                    }
                }
            }
        }
    }
}
