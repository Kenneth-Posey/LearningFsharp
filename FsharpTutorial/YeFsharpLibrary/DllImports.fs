namespace WindowsInterop

module DllImports = 
    open System
    open System.Runtime.InteropServices

    module MouseControl =         
        [<DllImport( "user32.dll", CallingConvention = CallingConvention.Cdecl )>]
        extern void ShowCursor(bool show)

        module private Internal = 
            [<DllImport( "user32.dll", CallingConvention = CallingConvention.Cdecl )>]
            extern void mouse_event(int flags, int dX, int dY, int buttons, int extraInfo)
        
        let MouseEvent flags dX dY buttons extraInfo = Internal.mouse_event(flags, dX, dY, buttons, extraInfo)

    module KeyboardControl = 
        module private Internal = 
            [<DllImport( "user32.dll", CallingConvention = CallingConvention.Cdecl )>]
            extern void keybd_event(byte key, byte scan, int flags, int extraInfo)

        let KeyboardEvent key scan flags extraInfo = Internal.keybd_event(key, scan, flags, extraInfo)

        [<DllImport( "user32.dll" )>]
        extern int GetKeyboardState(byte[] keyState)

        [<DllImport( "user32.dll" , CharSet = CharSet.Auto
                                  , CallingConvention = CallingConvention.StdCall )>]
        extern int16 GetKeyState(int key)

    module WindowControl = 
        type HookProc = delegate of ( int * int * IntPtr ) -> int
        [<DllImport( "user32.dll" , CharSet = CharSet.Auto
                                  , CallingConvention = CallingConvention.StdCall
                                  , SetLastError = true )>]
        extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId)
        
        [<DllImport( "user32.dll" , CharSet = CharSet.Auto
                                  , CallingConvention = CallingConvention.StdCall
                                  , SetLastError = true )>]
        extern int UnhookWindowsHookEx(int idHook)

        [<DllImport( "user32.dll" , CharSet = CharSet.Auto
                                  , CallingConvention = CallingConvention.StdCall )>]
        extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam)

        [<DllImport( "user32.dll" )>]
        extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState)



