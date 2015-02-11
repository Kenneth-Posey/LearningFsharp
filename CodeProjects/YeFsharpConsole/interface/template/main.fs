namespace WinformsPractice

module MainProgramTemplate = 
    open System
    open System.Windows.Forms
    open System.Drawing
    open System.Threading

    type Repeat (func) =
        member this.Bind(x, f) =          
            func (f x)
            ()
        member this.Return(x) = x
        member this.Zero() = ()

    type MainForm () as this = 
        inherit Form()            
        
        // obj -> EventArgs -> unit
        let Event e = new EventHandler (e)

        let Exit_Click s e = 
            Application.Exit ()

        let About_Click s e = 
            ()

        do 
            this.SetupForm ()
            this.SetupMenu ()

        member this.SetupForm () = 
            
            this.BackColor <- Color.Black
            this.Height <- 600
            this.Width <- 600
            
            
        member this.SetupMenu () = 
            let fileMenu = new MenuItem ("File")
            let helpMenu = new MenuItem ("Help")
            
            [
                fileMenu, [
                    // new MenuItem("Item1")
                    new MenuItem("Exit", Event Exit_Click)
                ]
                helpMenu, [
                    new MenuItem("About", Event About_Click)
                ]
            ] 
            |> List.iter (fun (menu, items) -> menu.MenuItems.AddRange (Array.ofList items))

            this.Menu <- new MainMenu [| fileMenu; helpMenu |]