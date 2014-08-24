﻿using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using ModernWPF.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public static MessageBoxResult HandleDialogMessageModern(this Window owner, DialogMessage message)
        {
            return ModernMessageBox.Show(owner, message.Content, message.Caption, message.Button, message.Icon, message.DefaultResult);
        }

        /// <summary>
        /// Handles a basic <see cref="DialogMessage" /> on a window by showing built-in <see cref="MessageBox"/>.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static MessageBoxResult HandleDialogMessagePlatform(this Window owner, DialogMessage message)
        {
            return MessageBox.Show(owner, message.Content, message.Caption, message.Button, message.Icon, message.DefaultResult, message.Options);
        }


        /// <summary>
        /// Handles the <see cref="ChooseFileMessage" /> on a window by showing a <see cref="FileDialog" /> based on the message options.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="message">The MSG.</param>
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
        /// Handles the <see cref="ChooseFileMessage"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void HandleOpenExplorer(this OpenExplorerMessage message)
        {
            if (!string.IsNullOrEmpty(message.SelectedPath))
            {
                using (Process.Start("explorer", string.Format("/select,{0}", message.SelectedPath))) { }
            }
            else if (string.IsNullOrEmpty(message.FolderPath))
            {
                using (Process.Start("explorer", message.FolderPath)) { }
            }
            using (Process.Start("explorer")) { }
        }
    }
}
