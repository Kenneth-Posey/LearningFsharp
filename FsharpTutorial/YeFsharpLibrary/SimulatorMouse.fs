﻿namespace Simulator

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

    module Structures = 
        type Point =
        | X of int
        | Y of int

        type MouseHook = 
        | Point         of Point
        | Hwnd          of int
        | WHitTestCode  of int
        | DwExtraInfo   of int

        type MouseLLHook = 
        | Point         of Point
        | MouseData     of int
        | Flags         of int
        | Time          of int
        | DwExtraInfo   of int

        type KeyboardHook = 
        | VkCode        of int
        | ScanCode      of int
        | Flags         of int
        | Time          of int
        | DwExtraInfo   of int

        
    module WindowsAPI = 
        
        type Hooks = 
        | Mouse = 7
        | MouseLL = 14

        type Messages = 
        | MouseMove   = 0x200
        | LeftDown    = 0x201
        | LeftUp      = 0x202
        | RightDown   = 0x204
        | RightUp     = 0x205
        | MiddleDown  = 0x207
        | MiddleUp    = 0x208
        | LeftClick   = 0x203
        | RightClick  = 0x206
        | MiddleClick = 0x109
        | MouseWheel  = 0x020A

