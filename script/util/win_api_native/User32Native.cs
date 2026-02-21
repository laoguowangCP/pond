using System;
using System.Runtime.InteropServices;
using Godot;


namespace LGWCP.Util.WinApiNative;

public static class User32Native
{
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpWindowClass, string lpWindowName);
    
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

    [DllImport("user32.dll", SetLastError = true)]
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


    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);

    const int SM_XVIRTUALSCREEN = 76; // 虚拟屏幕左侧坐标
    const int SM_YVIRTUALSCREEN = 77; // 虚拟屏幕顶部坐标
    const int SM_CXVIRTUALSCREEN = 78; // 虚拟屏幕总宽度
    const int SM_CYVIRTUALSCREEN = 79; // 虚拟屏幕总高度

    public static Vector2I GetVirtualScreenOrigin()
    {
        return new Vector2I(GetSystemMetrics(SM_XVIRTUALSCREEN), GetSystemMetrics(SM_YVIRTUALSCREEN));
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LPPOINT
    {
        public int X;
        public int Y;

        public LPPOINT() {}

        public LPPOINT(Vector2I pt)
        {
            X = pt.X;
            Y = pt.Y;
        }
    }

    [DllImport("user32.dll")]
    public static extern bool ScreenToClient(IntPtr hWnd, ref LPPOINT lpPoint);

    [DllImport("user32.dll")]
    public static extern bool ClientToScreen(IntPtr hWnd, ref LPPOINT lpPoint);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_SHOWWINDOW = 0x0040;

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out LPRECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct LPRECT {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}

