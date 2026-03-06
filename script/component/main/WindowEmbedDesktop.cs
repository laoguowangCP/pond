using System;
using Godot;
using System.Runtime.InteropServices;
using GodotTask;
using LGWCP.NiceGD;
using LGWCP.Util.WinApiNative;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class WindowEmbedDesktop : ComponentResource
{
    public override Type ComponentType => typeof(WindowEmbedDesktop);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    public bool IsEmbeded { get; protected set; } = false;
    protected Vector2I WindowPosBeforeEmbeded;
    protected BgMainPopupMenu BgMainPopupMenu;

    public override void OnEntityReady()
    {
        // DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Transparent, true);
        // root.TransparentBg = true;
        // Embed();
        Holder.TryGetComponent<BgMainPopupMenu>(out BgMainPopupMenu);
        // BgMainPopupMenu.PopupMenu.AddItem()
        // BgMainPopupMenu.PopupMenu.IdPressed += OnIdPressed;
    }

    public void SwitchEmbed()
    {
        if (IsEmbeded)
        {
            // Embed
            UnEmbed();
        }
        else 
        {
            // Embed
            // WindowEmbedDesktop.EmbedAsWallPaper();
            EmbedAsDesktopOverlay();
        }
    }

    public void EmbedAsWallPaper()
    {
        if (IsEmbeded)
        {
            return;
        }

        long godotWRaw = DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle);
        IntPtr godotW = new(godotWRaw);

        IntPtr workerW = GetWallpaperWindowHandle();

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
        var window = Holder.GetTree().Root.GetWindow();
        WindowPosBeforeEmbeded = window.Position;

        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
        // Holder.GetWindow().Borderless = true;

        long godotWRaw = DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle);
        IntPtr godotW = new(godotWRaw);

        IntPtr workerW = GetDesktopOverlayWindowHandle();

        // Calculate virtual desktop position
        User32Native.GetWindowRect(godotW, out var lpRect);
        Vector2I windowPos = new(lpRect.Left, lpRect.Top);

        // windowPos -= User32Native.GetVirtualScreenOrigin();

        User32Native.SetParent(godotW, workerW);
        // SetWindowLongPtr(wd.hWnd, GWLP_HWNDPARENT, (LONG_PTR)get_wp_host_hwnd());

        User32Native.LPPOINT lPPOINT = new(windowPos);
        User32Native.ScreenToClient(workerW, ref lPPOINT);
        User32Native.SetWindowPos(godotW, IntPtr.Zero, lPPOINT.X, lPPOINT.Y, 0, 0, User32Native.SWP_NOSIZE | User32Native.SWP_NOZORDER | User32Native.SWP_SHOWWINDOW);
        
        IsEmbeded = true;
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
        // User32Native.SetWindowPos(godotW, IntPtr.Zero, WindowPosBeforeEmbeded.X, WindowPosBeforeEmbeded.Y, 0, 0, User32Native.SWP_NOSIZE | User32Native.SWP_NOZORDER | User32Native.SWP_SHOWWINDOW);

        // Holder.GetTree().Root.GetWindow().Borderless = false;
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
        // Holder.GetWindow().Borderless = false;
        Holder.GetWindow().Position = WindowPosBeforeEmbeded;
        IsEmbeded = false;
    }

    protected IntPtr GetDesktopOverlayWindowHandle()
    {
        /*
        Quoted from: https://github.com/godotengine/godot/pull/106478/files
        win11 26200 has changed in SendMessageTimeout part.
        */
        int osVer = OS.GetVersion().Split('.')[^1].ToInt();
        IntPtr progman = User32Native.FindWindow("Progman", null);
        // Show background but unshow desktop icons
        User32Native.SendMessageTimeout(progman, User32Native.WM_SPAWN_WORKER, new IntPtr(0x0D), new IntPtr(0x01), User32Native.SendMessageTimeoutFlags.SMTO_NORMAL, 2000, out var result);
        return progman;
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
}
