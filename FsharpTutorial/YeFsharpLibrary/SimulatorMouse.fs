namespace Simulator

module Mouse = 
    open System
    open WindowsInterop.DllImports

    module Constants = 
        /// Available mouse buttons
        type ButtonCode = 
        | Left       = 0x2
        | Right      = 0x8
        | Middle     = 0x20

        /// Available mouse events
        type EventCode = 
        | Move       = 0x1
        | LeftDown   = 0x2
        | LeftUp     = 0x4
        | RightDown  = 0x8
        | RightUp    = 0x10
        | MiddleDown = 0x20
        | MiddleUp   = 0x40
        | WheelTurn  = 0x800
        | Absolute   = 0x8000

