using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


namespace LGWCP.Util.Regexy;

public static partial class Regexy
{

    [GeneratedRegex("(?>[a-zA-Z]+://)?(?>([a-zA-Z0-9\\-_]+\\.)+[a-zA-Z]{2,}){1}(?>:[0-9]{1,5})?(?>/[a-zA-Z0-9_\\-%\\!\\#&=?\\.]+)*/?")]
    private static partial Regex GetUriRegex();
    public readonly static Regex UriRegex = GetUriRegex();

    [GeneratedRegex("[a-zA-Z]:\\\\(?>[^\\\\/:*?\"<>|\r\n ](?>[^\\\\/:*?\"<>|\r\n]*[^\\\\/:*?\"<>|\r\n ])?\\\\)*(?>[^\\\\/:*?\"<>|\r\n ](?>[^\\\\/:*?\"<>|\r\n]*[^\\\\/:*?\"<>|\r\n ])?)?")]
    private static partial Regex GetWinFileRegex();
    public readonly static Regex WinFileRegex = GetWinFileRegex();

}