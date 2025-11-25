using System;
using Godot;
using System.Runtime.InteropServices;
using GodotTask;
using LGWCP.Nice;
using LGWCP.Nice.Godot;
using System.Text;
using System.Threading;


[GlobalClass]
public partial class SetParentDesktop : ComponentResource
{
    public override Type ComponentType => typeof(SetParentDesktop);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

    [DllImport("user32.dll")]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, IntPtr lpszWindow);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    private const int SW_SHOW = 5;
    private const uint SMTO_NORMAL = 0x0;

    private static IntPtr progman = IntPtr.Zero;
    private static IntPtr workerW = IntPtr.Zero;
    private static IntPtr original_WorkerW = IntPtr.Zero;
    private static IntPtr shellDLL_DefView = IntPtr.Zero;
    private static IntPtr shellDLL_DefView_Parent = IntPtr.Zero;

    public void Embed(Window godotWindow)
    {
        long rawHandle = DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle);
        IntPtr godotHWND = new nint(rawHandle);
        
        progman = FindWindow("Progman", null);

        // 发送 0x052C
        IntPtr result = IntPtr.Zero;
        SendMessageTimeout(
            progman,
            0x052C,
            new IntPtr(0x0),
            new IntPtr(0x0),
            SMTO_NORMAL,
            1000,
            out result);

        shellDLL_DefView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
        StringBuilder className = new StringBuilder(256);
        // if (shellDLL_DefView == IntPtr.Zero)
        EnumWindows(new EnumWindowsProc((topHandle, topParamHandle) =>
        {
            _ = GetClassName(topHandle, className, className.Capacity);
            if (className.ToString() == "Progman")
            {
                shellDLL_DefView = FindWindowEx(topHandle, IntPtr.Zero, "SHELLDLL_DefView", null);
                workerW = FindWindowEx(topHandle, shellDLL_DefView, "WorkerW", null);
                GD.Print(shellDLL_DefView);
                GD.Print(workerW);
                return false;
            }
            return true; // 继续枚举
        }), IntPtr.Zero);


        // E. 检查并挂载
        if (workerW == IntPtr.Zero)
        {
            GD.PushWarning("WorkerW not found, fallback to Progman. Transparency could be failed.");
            workerW = progman;
        }
        else
        {
            GD.Print($"WorkerW handle available: {workerW}.");
        }

        // E. 正式“嫁接”：把 Godot 窗口设为 WorkerW 的子窗口
        SetParent(godotHWND, workerW);
        
        // 可选：确保窗口显示
        ShowWindowAsync(godotHWND, 1);
        
        GD.Print("挂载完成！");
    }

    public override void OnEntityReady()
    {
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Transparent, true);
        Holder.TryGetEntity<Node>(out var node);
        var root = node.GetTree().Root;
        root.TransparentBg = true;
        // Embed(root);
        DelayCallEmbed(root);
        // Callable.From<Window>(Embed).CallDeferred(root);
    }

    public async void DelayCallEmbed(Window root)
    {
        await GDTask.Delay(TimeSpan.FromSeconds(1), DelayType.DeltaTime);
        Embed(root);
    }
}
