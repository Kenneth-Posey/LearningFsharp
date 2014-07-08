// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System
open System.Threading

[<EntryPoint>]
let main argv = 
    "Enter text>> " |> Console.Write
    let maininput = Console.ReadLine()
    printfn "%A" maininput
    Thread.Sleep(2000)
    0 // return an integer exit code
