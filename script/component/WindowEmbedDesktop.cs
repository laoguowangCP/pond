using System;
using Godot;
using System.Runtime.InteropServices;
using GodotTask;
using LGWCP.NiceGD;
using LGWCP.Util.User32Native;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class WindowEmbedDesktop : ComponentResource
{
    public override Type ComponentType => typeof(WindowEmbedDesktop);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public bool IsEmbeded { get; protected set; } = false;

    public void EmbedAsWallPaper()
    {
        if (IsEmbeded)
        {
            return;
        }

        long godotWRaw = DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle);
        IntPtr godotW = new(godotWRaw);

        IntPtr workerW = GetWallpaperWindowHandle();
        // GD.Print(workerW);

        User32Native.SetParent(godotW, workerW);
        // SetWindowLongPtr(wd.hWnd, GWLP_HWNDPARENT, (LONG_PTR)get_wp_host_hwnd());
        IsEmbeded = true;
    }
    
    public void EmbedAsDesktopOverlay()
    {
        if (IsEmbeded)
        {
            return;
        }

        long godotWRaw = DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle);
        IntPtr godotW = new(godotWRaw);

        IntPtr workerW = GetDesktopOverlayWindowHandle();
        // GD.Print(workerW);

        User32Native.SetParent(godotW, workerW);
        // SetWindowLongPtr(wd.hWnd, GWLP_HWNDPARENT, (LONG_PTR)get_wp_host_hwnd());
        
        IsEmbeded = true;
        Holder.GetTree().Root.GetWindow().Borderless = true;
    }

    public void UnEmbed()
    {
        if (!IsEmbeded)
        {
            return;
        }
        long godotWRaw = DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle);
        IntPtr godotW = new(godotWRaw);
        User32Native.SetParent(godotW, IntPtr.Zero);

        IsEmbeded = false;
        Holder.GetTree().Root.GetWindow().Borderless = false;
    }

    protected IntPtr GetDesktopOverlayWindowHandle()
    {
        /*
        Quoted from: https://github.com/godotengine/godot/pull/106478/files
        win11 26200 has changed in SendMessageTimeout part.
        */
        int osVer = OS.GetVersion().Split('.')[^1].ToInt();
        GD.Print("OS version: ", osVer);
        if (osVer >= 26200)
        {
            IntPtr progman = User32Native.FindWindow("Progman", null);
            // User32Native.SendMessageTimeout(progman, User32Native.WM_SPAWN_WORKER, new IntPtr(0x0D), new IntPtr(0x01), User32Native.SendMessageTimeoutFlags.SMTO_NORMAL, 2000, out var result);
            GD.Print(progman);
            return progman;
        }
        else
        {
            return IntPtr.Zero;
        }
    }

    protected IntPtr GetWallpaperWindowHandle()
    {
        /*
        Quoted from: https://github.com/godotengine/godot/pull/106478/files
        win11 26200 has changed in SendMessageTimeout part.
        */
        int osVer = OS.GetVersion().Split('.')[^1].ToInt();
        GD.Print("OS version: ", osVer);
        if (osVer >= 26200)
        {
            IntPtr progman = User32Native.FindWindow("Progman", null);
            GD.Print(progman);
            IntPtr defview = User32Native.FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
            GD.Print(defview);

            IntPtr workerW;
            if (defview != IntPtr.Zero)
            {
                workerW = User32Native.FindWindowEx(progman, IntPtr.Zero, "WorkerW", null);
                while (workerW != IntPtr.Zero)
                {
                    if (User32Native.FindWindowEx(workerW, IntPtr.Zero, "SHELLDLL_DefView", null) == IntPtr.Zero)
                    {
                        // return progman;
                        return workerW;
                    }
                    workerW = User32Native.FindWindowEx(progman, workerW, "WorkerW", null);
                }
            }

            // User32Native.SendMessageTimeout(progman, User32Native.WM_SPAWN_WORKER, new IntPtr(0x00), new IntPtr(0x00), User32Native.SendMessageTimeoutFlags.SMTO_NORMAL, 2000, out var result);
            User32Native.SendMessageTimeout(progman, User32Native.WM_SPAWN_WORKER, new IntPtr(0x0D), new IntPtr(0x01), User32Native.SendMessageTimeoutFlags.SMTO_NORMAL, 2000, out var result);
            workerW = User32Native.FindWindowEx(progman, IntPtr.Zero, "WorkerW", null);
            // GD.Print(workerW);
            while (workerW != IntPtr.Zero)
            {
                if (User32Native.FindWindowEx(workerW, IntPtr.Zero, "SHELLDLL_DefView", null) == IntPtr.Zero)
                {
                    // return progman;
                    return workerW;
                }
                workerW = User32Native.FindWindowEx(progman, IntPtr.Zero, "WorkerW", null);
            }
            return progman;
        }
        else
        {
            /*
            IntPtr progman = User32Native.FindWindow("Progman", "Program Manager");
            User32Native.SendMessageTimeout(progman, User32Native.WM_SPAWN_WORKER, new IntPtr(0x0D), new IntPtr(0x01), User32Native.SendMessageTimeoutFlags.SMTO_NORMAL, 2000, out _);

            IntPtr iconview = 0;
            EnumWindows(find_iconview, (LPARAM)(&iconview));

            return GetWindow(iconview, GW_HWNDNEXT);
            */

            return IntPtr.Zero;
        }
    }

    public override void OnEntityReady()
    {
        // DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Transparent, true);
        // root.TransparentBg = true;
        // Embed();
    }
}
