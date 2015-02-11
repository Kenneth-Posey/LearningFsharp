// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

namespace Monogame

module Program = 
    open Game1

        
    type Program () = 
        [<EntryPoint>]
        let main argv = 
            use game = new Game1()
            printfn "%A" argv
            0 // return an integer exit code
