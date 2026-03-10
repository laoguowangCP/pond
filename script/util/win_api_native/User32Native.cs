using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
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

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool CloseClipboard();

    [DllImport("user32.dll")]
    public static extern bool IsClipboardFormatAvailable(uint format);

    [DllImport("user32.dll")]
    public static extern IntPtr GetClipboardData(uint uFormat);

    private const uint CF_HDROP = 15;

    public static void GetCopiedFiles(ref List<string> files)
    {
        files.Clear();
        // 检查剪贴板中是否有文件数据
        if (!User32Native.IsClipboardFormatAvailable(CF_HDROP))
        {
            return;
        }

        // 打开剪贴板 (传入 IntPtr.Zero 表示当前任务)
        if (OpenClipboard(IntPtr.Zero))
        {
            try
            {
                // 获取文件数据句柄
                IntPtr hDrop = GetClipboardData(CF_HDROP);
                if (hDrop != IntPtr.Zero)
                {
                    // Windows 最大路径长度通常为 260
                    StringBuilder sb = new();

                    // 当第二个参数为 0xFFFFFFFF 时，返回的是文件数量
                    uint fileCount = Shell32Native.DragQueryFile(hDrop, 0xFFFFFFFF, null, 0);
                    
                    for (uint i = 0; i < fileCount; ++i)
                    {
                        uint pathLength = Shell32Native.DragQueryFile(hDrop, i, null, 0);
                        sb.EnsureCapacity((int)pathLength+1);
                        if (Shell32Native.DragQueryFile(hDrop, i, sb, pathLength+1) > 0)
                        {
                            files.Add(sb.ToString());
                            sb.Clear();
                        }
                    }
                }
            }
            finally
            {
                // 无论如何，最后必须释放/关闭剪贴板，否则其他程序将无法复制粘贴
                CloseClipboard();
            }
        }
        
        return;
    }
}

