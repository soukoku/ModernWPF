using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ModernWPF
{
    /// <summary>
    /// Some lame properties for workarounds.
    /// </summary>
    public static class PlatformInfo
    {
        /// <summary>
        /// Flag for legacy OS that don't display things correctly.
        /// Mostly for XP and like.
        /// </summary>
        public static bool IsLegacyOS
        {
            get
            {
                return !CommonWin32.PlatformInfo.IsWinVistaUp;
            }
        }
        
    }
}
