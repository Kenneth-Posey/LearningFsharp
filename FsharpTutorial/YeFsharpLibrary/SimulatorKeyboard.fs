namespace Simulator

module Keyboard = 
    open System
    open WindowsInterop.DllImports

    module Constants = 
        
        /// Available keyboard shortcut commands
        type StandardShortcut = 
        | Copy        = 0
        | Cut         = 1
        | Paste       = 2
        | SelectAll   = 3
        | Save        = 4
        | Open        = 5
        | New         = 6
        | Close       = 7
        | Print       = 8

        type KeyboardEvent = 
        | ExtendedKey = 0x1
        | KeyUp       = 0x2

    module WindowsAPI = 
        // From GlobalHook.cs
        type Hooks = 
        | Keyboard    = 2
        | Keyboard_LL = 13 // low-level

        type Messages = 
        | KeyDown     = 0x100
        | KeyUp       = 0x101
        | SysKeyDown  = 0x104
        | SysKeyUp    = 0x105
        | Shift       = 0x10
        | Capital     = 0x14
        | Numlock     = 0x90
        | LShift      = 0xA0
        | RShift      = 0xA1
        | LControl    = 0xA2
        | RControl    = 0x3
        | LAlt        = 0xA4
        | RAlt        = 0xA5