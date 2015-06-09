using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ModernWPF.Messages;
using ModernWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

            Progress = new ProgressViewModel();

            TreeItems = new ObservableCollection<HierarchyVM>();
            for (int i = 0; i < 50; i++)
            {
                TreeItems.Add(new HierarchyVM(5));
            }
        }

        public List<AccentVM> Accents { get; private set; }

        public ObservableCollection<CultureInfoVM> Languages { get; private set; }

        public ObservableCollection<HierarchyVM> TreeItems { get; private set; }

        public List<ItemVM> Items { get; private set; }

        private ICommand _sortItemsCommand;

        public ICommand SortItemsCommand
        {
            get
            {
                return _sortItemsCommand ?? (
                    _sortItemsCommand = new RelayCommand<GridViewSortParameter>(e =>
                    {
                        var head = e.Header;
                        if (head != null)
                        {
                            string field = null;
                            var bind = head.Column.DisplayMemberBinding as Binding;
                            if (bind != null)
                            {
                                field = bind.Path.Path;
                            }

                            if (!string.IsNullOrEmpty(field))
                            {
                                Debug.WriteLine("Sort command exec on " + field);

                                var view = CollectionViewSource.GetDefaultView(Items);
                                if (view.CanSort)
                                {
                                    //view.DeferRefresh();

                                    view.SortDescriptions.Clear();
                                    if (e.NewSortDirection.HasValue)
                                    {
                                        view.SortDescriptions.Add(new System.ComponentModel.SortDescription(field, e.NewSortDirection.Value));
                                    }
                                }
                            }
                        }
                    })
                );
            }
        }


        public List<string> Strings { get; private set; }

        public ProgressViewModel Progress { get; private set; }

        private RelayCommand _testProgressCmd;
        public ICommand TestProgressCommand
        {
            get
            {
                if (_testProgressCmd == null)
                {
                    _testProgressCmd = new RelayCommand(() =>
                    {
                        ThreadPool.QueueUserWorkItem(o =>
                        {
                            for (double i = 0; i < 100; i++)
                            {
                                Progress.UpdateState(System.Windows.Shell.TaskbarItemProgressState.Normal, i / 100, "Progres = " + i);
                                Thread.Sleep(60);
                            }
                            Progress.UpdateState(System.Windows.Shell.TaskbarItemProgressState.None);
                            App.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                _testProgressCmd.RaiseCanExecuteChanged();
                            }));
                        });
                    }, () =>
                    {
                        return !Progress.IsBusy;
                    });
                }
                return _testProgressCmd;
            }
        }


        static readonly ResourceDictionary desktopSize = GetResource("/ModernWPF;component/themes/ModernBaseDesktop.xaml");
        static readonly ResourceDictionary modernSize = GetResource("/ModernWPF;component/themes/ModernBase.xaml");
        internal static ResourceDictionary GetResource(string url)
        {
            var style = new ResourceDictionary();
            style.Source = new Uri(url, UriKind.Relative);
            return style;
        }
        private static void ApplyResources(ResourceDictionary resources)
        {
            foreach (var k in resources.Keys)
            {
                Application.Current.Resources[k] = resources[k];
            }
        }



        private ICommand _toggleThemeCmd;
        public ICommand ToggleThemeCommand
        {
            get
            {
                if (_toggleThemeCmd == null)
                {
                    _toggleThemeCmd = new RelayCommand<string>(param =>
                    {
                        switch (param)
                        {
                            case "dark":
                                ModernTheme.ApplyTheme(ModernTheme.Theme.Dark, ModernTheme.CurrentAccent);
                                break;
                            case "light":
                                ModernTheme.ApplyTheme(ModernTheme.Theme.Light, ModernTheme.CurrentAccent);
                                break;
                            case "modern":
                                ApplyResources(modernSize);
                                break;
                            case "desktop":
                                ApplyResources(desktopSize);
                                break;
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
                                Messenger.Default.Send(new Messages.MessageBoxMessage(obj, string.Format("Selected {0} files.", files.Count()), null) { Caption = "Open file result" });
                            }
                            else
                            {
                                Messenger.Default.Send(new Messages.MessageBoxMessage(obj, "Selected " + files.FirstOrDefault(), null) { Caption = "Open file result" });
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
                            Messenger.Default.Send(new Messages.MessageBoxMessage(obj, "Selected " + files.FirstOrDefault(), null) { Caption = "Save file result" });
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
                            Messenger.Default.Send(new Messages.MessageBoxMessage(obj, string.Format("Selected {0}.", folder), null) { Caption = "Folder result" });
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
