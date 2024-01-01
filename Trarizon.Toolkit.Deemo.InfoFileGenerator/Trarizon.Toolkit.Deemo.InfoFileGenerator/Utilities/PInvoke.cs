using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Utilities;
internal static partial class PInvoke
{
    [LibraryImport("user32.dll")]
    private static partial int GetDpiForWindow(nint hwnd);

    public static int GetDpiForWindow(Window window) 
        => GetDpiForWindow(WindowNative.GetWindowHandle(window));
}
