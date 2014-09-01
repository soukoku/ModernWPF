using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ModernWPF.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ModernWPF.Sample.VM
{
    class SampleAppVM : ViewModelBase
    {
        public SampleAppVM()
        {
            Languages = new ObservableCollection<CultureInfoVM>();
            Languages.Add(new CultureInfoVM(new CultureInfo("en-US")) { IsSelected = true });
            Languages.Add(new CultureInfoVM(new CultureInfo("zh-TW")));
            Languages.Add(new CultureInfoVM(new CultureInfo("zh-CN")));
            Languages.Add(new CultureInfoVM(new CultureInfo("ja")));


            Strings = new List<string>();
            Items = new List<ItemVM>();
            for (int i = 1; i <= 1000; i++)
            {
                Strings.Add(string.Format("should virtual {0}", i));
                Items.Add(new ItemVM
                {
                    Boolean = i % 5 == 0,
                    Number = i,
                    String = string.Format("Item # {0}", i)
                });
            }
            Accents = ModernTheme.PredefinedAccents.Select(a => new AccentVM(a)).ToList();
        }

        public List<AccentVM> Accents { get; private set; }

        public ObservableCollection<CultureInfoVM> Languages { get; private set; }

        public List<ItemVM> Items { get; private set; }

        public List<string> Strings { get; private set; }

        private ICommand _toggleThemeCmd;
        public ICommand ToggleThemeCommand
        {
            get
            {
                if (_toggleThemeCmd == null)
                {
                    _toggleThemeCmd = new RelayCommand(() =>
                    {
                        if (ModernTheme.CurrentTheme.GetValueOrDefault() == ModernTheme.Theme.Dark)
                        {
                            ModernTheme.ApplyTheme(ModernTheme.Theme.Light, ModernTheme.CurrentAccent);
                        }
                        else
                        {
                            ModernTheme.ApplyTheme(ModernTheme.Theme.Dark, ModernTheme.CurrentAccent);
                        }
                    });
                }
                return _toggleThemeCmd;
            }
        }

        private ICommand _openFileCmd;
        public ICommand OpenFileCommand
        {
            get
            {
                if (_openFileCmd == null)
                {
                    _openFileCmd = new RelayCommand<object>(obj =>
                    {
                        Messenger.Default.Send(new ChooseFileMessage(obj, files =>
                        {
                            if (files.Count() > 1)
                            {
                                Messenger.Default.Send(new DialogMessage(obj, string.Format("Selected {0} files.", files.Count()), null) { Caption = "Open file result" });
                            }
                            else
                            {
                                Messenger.Default.Send(new DialogMessage(obj, "Selected " + files.FirstOrDefault(), null) { Caption = "Open file result" });
                            }
                        })
                        {
                            Caption = "Open File Dialog",
                            Purpose = ChooseFileMessage.FilePurpose.OpenMultiple,
                        });
                    });
                }
                return _openFileCmd;
            }
        }

        private ICommand _saveFileCmd;
        public ICommand SaveFileCommand
        {
            get
            {
                if (_saveFileCmd == null)
                {
                    _saveFileCmd = new RelayCommand<object>(obj =>
                    {
                        Messenger.Default.Send(new ChooseFileMessage(obj, files =>
                        {
                            Messenger.Default.Send(new DialogMessage(obj, "Selected " + files.FirstOrDefault(), null) { Caption = "Save file result" });
                        })
                        {
                            Caption = "Save File Dialog",
                            Purpose = ChooseFileMessage.FilePurpose.Save
                        });
                    });
                }
                return _saveFileCmd;
            }
        }

        private ICommand _chooseFolderCmd;
        public ICommand ChooseFolderCommand
        {
            get
            {
                if (_chooseFolderCmd == null)
                {
                    _chooseFolderCmd = new RelayCommand<object>(obj =>
                    {
                        Messenger.Default.Send(new ChooseFolderMessage(obj, folder =>
                        {
                            Messenger.Default.Send(new DialogMessage(obj, string.Format("Selected {0}.", folder), null) { Caption = "Folder result" });
                        })
                        {
                            Caption = "Choose Folder Dialog"
                        });
                    });
                }
                return _chooseFolderCmd;
            }
        }
    }
}
