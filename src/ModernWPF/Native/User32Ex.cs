using CommonWin32.API;
using CommonWin32.Rectangles;
using CommonWin32.Shells;
using CommonWin32.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ModernWPF.Native
{
    static class User32Ex
    {
        //public static bool ModifyStyle(IntPtr hwnd, WindowStyles removeStyle, WindowStyles addStyle)
        //{
        //    var oldStyle = User32.GetWindowLong(hwnd, CommonWin32.WindowClasses.WindowLong.GWL_STYLE).ToInt32();
        //    var newStyle = oldStyle & (int)(~removeStyle | addStyle);
        //    if (oldStyle != newStyle)
        //    {
        //        return User32.SetWindowLong(hwnd, CommonWin32.WindowClasses.WindowLong.GWL_STYLE, new IntPtr(newStyle)) != IntPtr.Zero;
        //    }
        //    return false;
        //}


        // autohide taskbar fix from http://codekong.wordpress.com/2010/11/10/custom-window-style-and-accounting-for-the-taskbar/

        const int ABS_AUTOHIDE = 1;

        public static void AdjustForAutoHideTaskbar(IntPtr hAppMonitor, ref RECT workspace)
        {
            // NOTE: for xp the adjustment for autohidden taskbar makes maximized window movable 
            // but I don't know the way to fix it.
            IntPtr htaskbar = User32.FindWindow("Shell_TrayWnd", null);
            if (htaskbar != IntPtr.Zero)
            {
                IntPtr monitorWithTaskbarOnIt = User32.MonitorFromWindow(htaskbar, MonitorOption.MONITOR_DEFAULTTONEAREST);
                if (hAppMonitor.Equals(monitorWithTaskbarOnIt))
                {
                    APPBARDATA abd = new APPBARDATA();
                    abd.cbSize = (uint)Marshal.SizeOf(abd);
                    abd.hWnd = htaskbar;
                    bool autoHide = (Shell32.SHAppBarMessage(AppBarMessage.ABM_GETSTATE, ref abd).ToUInt32() & ABS_AUTOHIDE) == ABS_AUTOHIDE;

                    if (autoHide)
                    {
                        Shell32.SHAppBarMessage(AppBarMessage.ABM_GETTASKBARPOS, ref abd);
                        var uEdge = GetEdge(ref abd.rc);

                        switch (uEdge)
                        {
                            case AppBarEdge.ABE_LEFT:
                                workspace.left += 2;
                                break;
                            case AppBarEdge.ABE_RIGHT:
                                workspace.right -= 2;
                                break;
                            case AppBarEdge.ABE_TOP:
                                workspace.top += 2;
                                break;
                            case AppBarEdge.ABE_BOTTOM:
                                workspace.bottom -= 2;
                                break;
                        }
                    }
                }
            }
        }
        static AppBarEdge GetEdge(ref RECT rc)
        {
            if (rc.top == rc.left && rc.bottom > rc.right)
                return AppBarEdge.ABE_LEFT;
            else if (rc.top == rc.left && rc.bottom < rc.right)
                return AppBarEdge.ABE_TOP;
            else if (rc.top > rc.left)
                return AppBarEdge.ABE_BOTTOM;
            else
                return AppBarEdge.ABE_RIGHT;
        }
    }
}
