using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Godot;


namespace LGWCP.Util.WinApiNative;

public static class Shell32Native
{
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern uint DragQueryFile(IntPtr hDrop, uint iFile, [Out] StringBuilder lpszFile, uint cch);
}
