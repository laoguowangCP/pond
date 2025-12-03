using System;
using System.Runtime.InteropServices;


namespace LGWCP.Util.User32Native;

public static class User32Native
{
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpWindowClass, string lpWindowName);
    
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

    [DllImport("user32.dll")]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    
    [Flags]
    public enum SendMessageTimeoutFlags : uint
    {
        SMTO_NORMAL = 0x0,
        SMTO_BLOCK = 0x1,
        SMTO_ABORTIFHUNG = 0x2,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
        SMTO_ERRORONEXIT = 0x20
    }

    public const uint WM_SPAWN_WORKER = 0x052C;

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out IntPtr lpdwResult);

}

