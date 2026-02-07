using System;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;


namespace LGWCP.Util.OleNative;

public static class OleNative
{
    [DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern int DoDragDrop(
        IDataObject pDataObj, // 数据对象
        IDropSource pDropSource, // 拖拽源反馈控制
        int dwOKEffects,         // 允许的操作（复制/移动/链接）
        out int pdwEffect        // 最终执行的操作
    );

    // 初始化 OLE 库（通常 Godot 启动时可能已经初始化，但在某些线程可能需要手动）
    [DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern int OleInitialize(IntPtr pvReserved);

    [DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern void OleUninitialize();
    
    
    // 常量定义
    public const int DRAGDROP_S_DROP = 0x00040100;
    public const int DRAGDROP_S_CANCEL = 0x00040101;
    public const int DROPEFFECT_NONE = 0;
    public const int DROPEFFECT_COPY = 1;
    public const int DROPEFFECT_MOVE = 2;
    public const int DROPEFFECT_LINK = 4;
}

[ComImport]
[Guid("00000121-0000-0000-c000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IDropSource
{
    // 询问是否继续拖拽（检测 ESC 键或鼠标释放）
    [PreserveSig]
    int QueryContinueDrag(int fEscapePressed, int grfKeyState);

    // 给出视觉反馈（改变鼠标光标）
    [PreserveSig]
    int GiveFeedback(int dwEffect);
}

