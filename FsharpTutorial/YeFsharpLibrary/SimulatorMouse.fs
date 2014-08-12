namespace Simulator

module Mouse = 
    open System
    open System.Drawing
    open System.Windows.Forms

    open SimulatorTypes.Mouse.Constants
    open WindowsInterop.DllImports

    type Controller () =
        member this.Position 
            with get () = new Point( Cursor.Position.X, Cursor.Position.Y )
            and  set (value) = Cursor.Position <- value

        member this.X
            with get () = Cursor.Position.X
            and  set (value) = Cursor.Position <- new Point( value, this.Y )

        member this.Y
            with get () = Cursor.Position.Y
            and  set (value) = Cursor.Position <- new Point( this.X, value )
                        
        member this.MouseDown button = 
            MouseControl.MouseEvent button 0 0 0 0
            
        member this.MouseUp button = 
            MouseControl.MouseEvent (button * 2) 0 0 0 0
            
        member this.Click button = 
            this.MouseDown button
            this.MouseUp button

        member this.DoubleClick button = 
            this.Click button
            this.Click button

        member this.ScrollMouseWheel delta = 
            MouseControl.MouseEvent EventCode.WheelTurn 0 0 delta 0

        member this.ShowCursor () = 
            MouseControl.ShowCursor true

        member this.HideCursor () =
            MouseControl.ShowCursor false

            


        

