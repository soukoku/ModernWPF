using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ModernWPF.Sample.VM
{
    class HierarchyVM
    {
        public HierarchyVM(int children)
        {
            Children = new ObservableCollection<HierarchyVM>();
            for (int i = 0; i < children; i++)
            {
                Children.Add(new HierarchyVM(children - 1));
            }
            Name = string.Format("has {0} nodes", children);
        }

        public string Name { get; set; }

        public ObservableCollection<HierarchyVM> Children { get; set; }
    }
}
