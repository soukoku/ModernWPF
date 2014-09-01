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
    class CultureInfoVM : SelectableViewModel<CultureInfo>
    {
        public CultureInfoVM(CultureInfo info)
            : base(info)
        {

        }

        protected override void OnSelectedChanged()
        {
            if (IsSelected)
            {
                Thread.CurrentThread.CurrentUICulture = Model;
                CommandTextBinder.Instance.UpdateCulture(Model);
            }
        }

        public override string ToString()
        {
            return Model.NativeName;
        }
    }
}
