using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernWPF.Messages
{
    /// <summary>
    /// Message for choosing a folder.
    /// </summary>
    class ChooseFolderMsg : MessageBase
    {
        public ChooseFolderMsg(Action<string> callback)
        {
            _callback = callback;
        }
        Action<string> _callback;

        public string Title { get; set; }

        public string InitialFolder { get; set; }

        public void DoCallback(string folder)
        {
            if (_callback != null)
            {
                _callback(folder);
            }
        }
    }
}
