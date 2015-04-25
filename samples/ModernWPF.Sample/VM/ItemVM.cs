using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModernWPF.Sample.VM
{
    class ItemVM
    {
        public bool Boolean { get; set; }

        public string String { get; set; }
        
        public int Number { get; set; }
        
        
        public DateTime Date { get { return DateTime.Now; } }
    }
}
