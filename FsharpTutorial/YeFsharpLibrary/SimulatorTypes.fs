namespace Simulator

module SimulatorTypes = 
    open System.Drawing

    module Mouse = 
        module Constants = 
            /// Available mouse buttons
            type ButtonCode = 
                static member Left   = 0x2
                static member Right  = 0x8
                static member Middle = 0x20
                
            /// Available mouse events
            type EventCode = 
                static member Move       = 0x1
                static member LeftDown   = 0x2
                static member LeftUp     = 0x4
                static member RightDown  = 0x8
                static member RightUp    = 0x10
                static member MiddleDown = 0x20
                static member MiddleUp   = 0x40
                static member WheelTurn  = 0x800
                static member Absolute   = 0x8000

            type Hooks = 
                static member Mouse   = 7
                static member MouseLL = 14 // LL = low-level

            type Messages = 
                static member MouseMove   = 0x200
                static member LeftDown    = 0x201
                static member LeftUp      = 0x202
                static member RightDown   = 0x204
                static member RightUp     = 0x205
                static member MiddleDown  = 0x207
                static member MiddleUp    = 0x208
                static member LeftClick   = 0x203
                static member RightClick  = 0x206
                static member MiddleClick = 0x109
                static member MouseWheel  = 0x020A

        module Structures = 
            type MouseHook = 
            | Point        of Point
            | Hwnd         of int
            | WHitTestCode of int
            | DwExtraInfo  of int

            type MouseLLHook = 
            | Point       of Point
            | MouseData   of int
            | Flags       of int
            | Time        of int
            | DwExtraInfo of int


    module Keyboard = 
        module Constants =         
            /// Available keyboard shortcut commands
            type StandardShortcut = 
                static member Copy      = 0
                static member Cut       = 1
                static member Paste     = 2
                static member SelectAll = 3
                static member Save      = 4
                static member Open      = 5
                static member New       = 6
                static member Close     = 7
                static member Print     = 8

            /// Available keyboard events
            type EventCode = 
                static member KeyDown     = 0x0
                static member ExtendedKey = 0x1
                static member KeyUp       = 0x2

            type Hooks = 
                static member Keyboard   = 2
                static member KeyboardLL = 13 // low-level

            type Messages = 
                static member KeyDown    = 0x100
                static member KeyUp      = 0x101
                static member SysKeyDown = 0x104
                static member SysKeyUp   = 0x105
                static member Shift      = 0x10
                static member Capital    = 0x14
                static member Numlock    = 0x90
                static member LShift     = 0xA0
                static member RShift     = 0xA1
                static member LControl   = 0xA2
                static member RControl   = 0x3
                static member LAlt       = 0xA4
                static member RAlt       = 0xA5

        module Structures =         
            type KeyboardHook = 
            | VkCode      of int
            | ScanCode    of int
            | Flags       of int
            | Time        of int
            | DwExtraInfo of int

    // module Hook = 
    //     open System
    //     type Global () =
    //         type HookProc = delegate of int * int * IntPtr -> int
    // 
    //         member this.hookType
    //             with get ()      = this.hookType
    //             and  set (value) = this.hookType <- value
    //         member this.handleToHook 
    //             with get ()      = this.handleToHook
    //             and  set (value) = this.handleToHook <- value
    //         member this.isStarted
    //             with get ()      = this.isStarted
    //             and  set (value) = this.isStarted <- value
    //         new () = Global ()
    // 
    //         