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

