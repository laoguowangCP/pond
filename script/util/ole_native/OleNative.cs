using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;


// using System.Windows.Forms;
using Godot;


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
    public const int DRAGDROP_S_USEDEFAULTCURSORS = 0x00040102;
    public const int DROPEFFECT_NONE = 0;
    public const int DROPEFFECT_COPY = 1;
    public const int DROPEFFECT_MOVE = 2;
    public const int DROPEFFECT_LINK = 4;


    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern ushort RegisterClipboardFormat(string lpszFormat);
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

public class SimpleDropSource : IDropSource
{
    public int QueryContinueDrag(int fEscapePressed, int grfKeyState)
    {
        // 如果按了 ESC，取消拖拽
        if (fEscapePressed != 0) return OleNative.DRAGDROP_S_CANCEL;
        
        // 如果鼠标左键(MK_LBUTTON = 1)松开了，就执行 Drop
        // 注意：grfKeyState 的标志位检查需要根据具体需求写
        if ((grfKeyState & 1) == 0) return OleNative.DRAGDROP_S_DROP;

        return 0; // S_OK, 继续拖拽
    }

    public int GiveFeedback(int dwEffect)
    {
        // 返回 DRAGDROP_S_USEDEFAULTCURSORS 让系统自动处理光标
        return OleNative.DRAGDROP_S_USEDEFAULTCURSORS; 
    }
}

// 2. 实现一个极简的 DataObject (只支持文本)
// 如果需要支持文件拖拽，结构会更复杂，我们先从文本开始
public class TextDataObject : IDataObject
{
    private string _data;
    public TextDataObject(string text) => _data = text;

    public void GetData(ref FORMATETC format, out STGMEDIUM medium)
    {
        medium = new STGMEDIUM();
        if (format.cfFormat == (short)1) // CF_TEXT (简单文本格式)
        {
            medium.tymed = TYMED.TYMED_HGLOBAL;
            medium.unionmember = Marshal.StringToHGlobalAnsi(_data);
        }
    }

    // 以下是必须实现但通常可以返回“未实现”的接口方法
    public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink pAdvSink, out int pdwConnection) => throw new NotImplementedException();
    public void DUnadvise(int dwConnection) => throw new NotImplementedException();
    public int EnumDAdvise(out IEnumSTATDATA ppenumAdvise) => throw new NotImplementedException();
    public IEnumFORMATETC EnumFormatEtc(DATADIR direction) => throw new NotImplementedException();
    public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut) { formatOut = new FORMATETC(); return 1; }
    public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium) => throw new NotImplementedException();
    public int QueryGetData(ref FORMATETC format) => 1; // S_OK
    public void SetData(ref FORMATETC format, ref STGMEDIUM medium, bool release) => throw new NotImplementedException();
}

public class FileDataObject : IDataObject
{
    private readonly string[] _filePaths;
    private readonly short _cfPreferredDropEffect;

    public FileDataObject(string[] filePaths)
    {
        _filePaths = filePaths;
        _cfPreferredDropEffect = (short)OleNative.RegisterClipboardFormat("Preferred DropEffect");
    }

    public void GetData(ref FORMATETC format, out STGMEDIUM medium)
    {
        medium = new STGMEDIUM();
        if (format.cfFormat == 15) // 文件
        {
            medium.tymed = TYMED.TYMED_HGLOBAL;
            medium.unionmember = CreateDropFilesStructure(_filePaths);
        }
        // 2. 处理 Chrome/IM 偏好 (Preferred DropEffect)
        else if (format.cfFormat == _cfPreferredDropEffect)
        {
            IntPtr hGlobal = Marshal.AllocHGlobal(4);
            Marshal.WriteInt32(hGlobal, OleNative.DROPEFFECT_COPY);
            medium.unionmember = hGlobal;
        }
        else if (format.cfFormat == 13) // Unicode 文本
        {
            medium.tymed = TYMED.TYMED_HGLOBAL;
            // 把第一个文件的路径传给它，或者传你想要的文本
            medium.unionmember = Marshal.StringToHGlobalUni(_filePaths[0]);
        }
        else
        {
            throw new COMException("Unsupported format", -2147221404); // DV_E_FORMATETC
        }
    }

    private IntPtr CreateDropFilesStructure(string[] files)
    {
        // DROPFILES 结构体定义
        // struct DROPFILES {
        //    DWORD pFiles; // 文件列表起始偏移量
        //    POINT pt;     // 鼠标坐标
        //    BOOL  fNC;    // 是否是非客户区坐标
        //    BOOL  fWide;  // 是否是 Unicode
        // }
        
        int structSize = Marshal.SizeOf<DROPFILES>();
        // 计算所有字符串的长度（Unicode 需要每个字符 2 字节，最后以两个 \0 结尾）
        int contentSize = 0;
        foreach (var file in files) contentSize += (file.Length + 1) * 2;
        contentSize += 2; // 最终的终止符

        IntPtr hGlobal = Marshal.AllocHGlobal(structSize + contentSize);
        
        DROPFILES df = new DROPFILES();
        df.pFiles = structSize;
        df.fWide = 1; // 使用 Unicode

        // 将结构体拷贝到内存
        Marshal.StructureToPtr(df, hGlobal, false);

        // 将路径字符串拷贝到结构体之后的内存空间
        IntPtr charPtr = hGlobal + structSize;
        foreach (var file in files)
        {
            byte[] data = Encoding.Unicode.GetBytes(file + "\0");
            Marshal.Copy(data, 0, charPtr, data.Length);
            charPtr += data.Length;
        }
        // 写入最后的双 null 终止符
        Marshal.WriteInt16(charPtr, 0);

        return hGlobal;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DROPFILES
    {
        public int pFiles;
        public int x;
        public int y;
        public int fNC;
        public int fWide;
    }

    // 其他未实现接口保持原样 (参见上一个回复)
    public int QueryGetData(ref FORMATETC format)
    {
        GD.Print($"外部程序询问格式: {format.cfFormat}");
        // GD.Print($"CF_PREFERREDDROPEFFECT: { (short)OleNative.RegisterClipboardFormat("Preferred DropEffect") }");
        if (format.cfFormat == 13
            || format.cfFormat == 15
            || format.cfFormat == _cfPreferredDropEffect)
        {
            return 0; // S_OK
        }
        return 1; // S_FALSE (我不认识这个自定义格式，请用标准格式拿数据)
    }
    public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium) { }
    public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut) { formatOut = new FORMATETC(); return 1; }
    public void SetData(ref FORMATETC format, ref STGMEDIUM medium, bool release) { }
    public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
    {
        // DATADIR_GET 表示外部程序想知道你能“提供”什么数据
        if (direction == DATADIR.DATADIR_GET)
        {
            // 返回我们刚刚定义好的枚举器
            // 15 是 CF_HDROP，如果你支持文本，可以扩展枚举器支持多个格式
            return new SimpleFormatEnumerator(_cfPreferredDropEffect); 
        }
        
        // 如果是 DATADIR_SET，通常用于接收数据，我们这里可以抛出未实现异常
        // 或者返回一个特定的 HRESULT 错误码：E_NOTIMPL (0x80004001)
        throw new COMException("Not implemented", -2147467263);
    }
    public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink pAdvSink, out int pdwConnection) => throw new NotImplementedException();
    public void DUnadvise(int dwConnection) => throw new NotImplementedException();
    public int EnumDAdvise(out IEnumSTATDATA ppenumAdvise) => throw new NotImplementedException();
}

/*
[Guid("00000103-0000-0000-C000-000000000046"), InterfaceType(1)]
[ComImport]
public interface IEnumFormatEtc
{
    [PreserveSig]
    int Next(
        [In] uint celt,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [Out] FORMATETC[] rgelt,
        [MarshalAs(UnmanagedType.LPArray)] [Out] uint[] pceltFetched);
    [PreserveSig]
    int Skip(
        [In] uint celt);
    [PreserveSig]
    int Reset();
    [PreserveSig]
    int Clone(
        [MarshalAs(UnmanagedType.Interface)] out IEnumFormatEtc ppEnum);
}*/

// 2. 实现类
public class SimpleFormatEnumerator : IEnumFORMATETC
{
    private FORMATETC[] _formats;
    private int _currentIndex = 0;

    public SimpleFormatEnumerator(short preferredDropEffect)
    {
        _formats = new FORMATETC[]
        {
            // 格式 15: 文件
            CreateFE(15),
            // 格式 13: Unicode 文本 (Chrome 喜欢这个)
            CreateFE(13),
            CreateFE(preferredDropEffect)
        };
    }

    private FORMATETC CreateFE(short cf) => new FORMATETC {
        cfFormat = cf,
        ptd = IntPtr.Zero,
        dwAspect = DVASPECT.DVASPECT_CONTENT,
        lindex = -1,
        tymed = TYMED.TYMED_HGLOBAL
    };

    public int Next(int celt, FORMATETC[] rgelt, int[] pceltFetched)
    {
        int count = 0;
        while (_currentIndex < _formats.Length && count < celt)
        {
            rgelt[count] = _formats[_currentIndex];
            _currentIndex++;
            count++;
        }

        if (pceltFetched != null) pceltFetched[0] = count;
        
        return count == celt ? 0 : 1; // S_OK : S_FALSE
    }

    public int Skip(int celt)
    {
        _currentIndex += celt;
        return _currentIndex <= _formats.Length ? 0 : 1;
    }

    public int Reset()
    {
        _currentIndex = 0;
        return 0;
    }

    public void Clone(out IEnumFORMATETC ppenum)
    {
        ppenum = new SimpleFormatEnumerator(_formats[^1].cfFormat);
    }
}

public class DragDropUtil
{
    public static void StartDragDrop(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
        {
            return;
        }

        int initResult = OleNative.OleInitialize(IntPtr.Zero);
        // GD.Print(initResult);
        IDataObject data = new FileDataObject(new string[] { filePath });
        var dropSource = new SimpleDropSource();

        // 调用 API
        OleNative.DoDragDrop(data, dropSource,
            OleNative.DROPEFFECT_COPY | OleNative.DROPEFFECT_MOVE,
            out int finalEffect);
        GD.Print(finalEffect);
    }
}
