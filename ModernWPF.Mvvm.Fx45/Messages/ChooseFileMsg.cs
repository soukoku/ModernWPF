using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernWPF.Messages
{
    /// <summary>
    /// Message for choosing file(s) for opening/saving.
    /// </summary>
    class ChooseFileMsg : MessageBase
    {
        public ChooseFileMsg(Action<IEnumerable<string>> callback)
        {
            _callback = callback;
        }
        Action<string[]> _callback;

        public string Title { get; set; }

        public string InitialFileName { get; set; }

        public string InitialFolder { get; set; }

        public string Filters { get; set; }

        public FilePurpose Purpose { get; set; }


        public enum FilePurpose
        {
            OpenSingle,
            OpenMultiple,
            Save
        }

        public void DoCallback(params string[] files)
        {
            if (_callback != null)
            {
                _callback(files);
            }
        }
    }
}
