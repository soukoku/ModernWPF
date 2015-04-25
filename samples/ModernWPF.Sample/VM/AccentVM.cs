using ModernWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModernWPF.Sample.VM
{
    class AccentVM : SelectableViewModel<Accent>
    {
        public AccentVM(Accent a)
            : base(a)
        {

        }

        protected override void OnSelectedChanged()
        {
            if (IsSelected)
            {
                ModernTheme.ApplyTheme(ModernTheme.CurrentTheme.GetValueOrDefault(), Model);
            }
        }
    }
}
