using GalaSoft.MvvmLight.Messaging;
using ModernWPF.Resources;
using ModernWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace ModernWPF.Sample.VM
{
    class LangVM : SelectableViewModel<CultureInfo>
    {
        public LangVM(CultureInfo info)
            : base(info)
        {

        }

        protected override void OnSelectedChanged()
        {
            if (IsSelected)
            {
                Thread.CurrentThread.CurrentUICulture = Model;
                CommandTextBinder.Instance.UpdateCulture(Model);
                Messenger.Default.Send(new LangChangedMsg { Selected = this });
            }
        }

        public override string ToString()
        {
            return Model.NativeName;
        }
    }

    class SupportedLangs
    {
        public SupportedLangs()
        {
            Items = new ObservableCollection<LangVM>();
            Items.Add(new LangVM(new CultureInfo("en-US")) { IsSelected = true });
            Items.Add(new LangVM(new CultureInfo("zh-TW")));
            Items.Add(new LangVM(new CultureInfo("zh-CN")));
            Items.Add(new LangVM(new CultureInfo("ja")));


            // use this to uncheck previous items
            Messenger.Default.Register<LangChangedMsg>(this, msg =>
            {
                foreach (var it in Items)
                {
                    if (it != msg.Selected) { it.IsSelected = false; }
                }
            });
        }


        public ObservableCollection<LangVM> Items { get; private set; }
    }

    class LangChangedMsg
    {
        public LangVM Selected { get; set; }
    }
}
