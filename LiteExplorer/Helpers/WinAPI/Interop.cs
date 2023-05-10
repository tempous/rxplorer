using System;
using System.Runtime.InteropServices;

namespace LiteExplorer.Helpers.WinAPI;

public static class Interop
{
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SHGetFileInfo(string path,
        uint attributes,
        out ShellFileInfo fileInfo,
        uint size,
        uint flags);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyIcon(IntPtr pointer);
}
