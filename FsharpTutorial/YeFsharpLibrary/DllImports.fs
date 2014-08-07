namespace WindowsInterop

module DllImports = 
    open System.Runtime.InteropServices

    module MouseControl = 
        [<DllImport( "user32.dll", CallingConvention = CallingConvention.Cdecl )>]
        extern void ShowCursor(bool show)

        [<DllImport( "user32.dll", CallingConvention = CallingConvention.Cdecl )>]
        extern void mouse_event(int flags, int dX, int dY, int buttons, int extraInfo)

    module KeyboardControl = 
        [<DllImport( "user32.dll", CallingConvention = CallingConvention.Cdecl )>]
        extern void keybd_event(byte key, byte scan, int flags, int extraInfo)

