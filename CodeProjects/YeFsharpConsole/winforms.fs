namespace WinformsPractice



module Winforms = 
    open System.Windows.Forms
    open System.Drawing
    open System.Threading
    
    let walk start (random:System.Random) =
        match random.NextDouble() > 0.5 with
        | true -> start - 1
        | false -> start + 1

    let moveInvader (invader:PictureBox) = 
        let random = new System.Random () 
        
        let setPosition (invader:PictureBox) horiz vert =
            invader.Left <- horiz
            invader.Top <- vert
            Thread.Sleep 100
            horiz, vert

        let rec move (invader:PictureBox) horiz vert iter =
            match iter > 0 with
            | true ->
                (horiz, vert)
                |> (fun (horiz, vert) -> setPosition invader horiz vert)
                |> (fun (horiz, vert) -> walk horiz random, walk vert random)
                |> (fun (horiz, vert) -> horiz % 500, vert % 500)
                |> (fun (horiz, vert) -> move invader horiz vert (iter - 1))
            | false -> ()
            
        move invader 250 250 100


    type TestForm () as this =
        inherit Form()

        do
            this.SetupForm ()

        member this.SetupForm () =
            
            this.BackColor <- Color.Black
            this.Height <- 600
            this.Width <- 600

            let invader = new PictureBox ()
            invader.Load("../../resources/alien.jpg")
            invader.Height <- 50
            invader.Width  <- 50
            invader.Left <- 250
            invader.Top  <- 250
            invader.Show()

            let startbutton = new Button ()
            startbutton.Text <- "Start"
            startbutton.Width <- 80
            startbutton.Height <- 20

            startbutton.Left <- 150
            startbutton.Top <- 50

            startbutton.BackColor <- Color.Aqua
            
            startbutton.Click.Add (fun _ -> moveInvader invader)

            this.Controls.Add invader
            this.Controls.Add startbutton

            this.WindowState <- FormWindowState.Normal
            this.CenterToScreen ()
            this.Show ()

            

            