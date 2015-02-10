namespace WinformsPractice

module MainProgramTemplate = 
    open System
    open System.Windows.Forms
    open System.Drawing
    open System.Threading

    type Repeat (func) =
        member this.Bind(x, f) = func (f x)
        member this.Return(x) = x
        member this.Zero() = ()

    type MainForm () as this = 
        inherit Form()            
        
        // obj -> EventArgs -> unit
        let exit_Click (_:obj) (_:EventArgs) :unit=
            Application.Exit ()

        do 
            this.SetupForm ()
            this.SetupMenu ()

        member this.SetupForm () = 
            
            this.BackColor <- Color.Black
            this.Height <- 600
            this.Width <- 600
            
            
        member this.SetupMenu () = 
            this.Menu <- new MainMenu ()

            let menu1 = new MenuItem ("File")
            let menu2 = new MenuItem ("About")
            
            let exit = new EventHandler(exit_Click)

            (new Repeat (ignore)) {
                menu1.MenuItems.Add("Item 1")       
                menu1.MenuItems.Add("Exit", exit)   
                menu2.MenuItems.Add("About")        
                this.Menu.MenuItems.Add(menu1)      
                this.Menu.MenuItems.Add(menu2)     
            }