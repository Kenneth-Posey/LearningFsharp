namespace WinformsPractice



module Winforms = 
    open System.Windows.Forms

    type TestForm () as this =
        inherit Form()

        do
            this.SetupForm ()

        member this.SetupForm () =
            
            this.BackColor <- System.Drawing.Color.Black
            this.Height <- 100
            this.Width <- 100

            this.WindowState <- FormWindowState.Normal
            this.CenterToScreen ()
            this.Show ()