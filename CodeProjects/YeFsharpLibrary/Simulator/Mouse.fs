﻿namespace Simulator

module Mouse = 
    open System.Drawing
    open System.Windows.Forms

    open SimulatorTypes.Mouse.Constants
    open WindowsInterop.DllImports

    // These are all static because they're calling external DLLs, so 
    // there's really no use for object instantiation
    type Controller =
        static member Position 
            with get () = new Point( Cursor.Position.X, Cursor.Position.Y )
            and  set (value) = Cursor.Position <- value

        static member X
            with get () = Cursor.Position.X
            and  set (value) = Cursor.Position <- new Point( value, Controller.Y )

        static member Y
            with get () = Cursor.Position.Y
            and  set (value) = Cursor.Position <- new Point( Controller.X, value )
                        
        static member MouseDown button = 
            MouseControl.MouseEvent button 0 0 0 0
            
        static member MouseUp button = 
            MouseControl.MouseEvent (button * 2) 0 0 0 0
            
        static member Click button = 
            Controller.MouseDown button
            Controller.MouseUp button

        static member DoubleClick button = 
            Controller.Click button
            Controller.Click button

        // 120  = one "click" up
        // -120 = one "click" down
        static member ScrollMouseWheel turns = 
            MouseControl.MouseEvent EventCode.WheelTurn 0 0 (turns*120) 0

        static member ShowCursor () = 
            MouseControl.ShowCursor true

        static member HideCursor () =
            MouseControl.ShowCursor false

            


        

