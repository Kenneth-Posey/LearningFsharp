namespace CocosSharpPractice

open System
open CocosSharp

module Main = 
    [<EntryPoint>]
    let main (_) = 
        let application = new CCApplication ()
        application.ApplicationDelegate <- new CCApplicationDelegate ()
        application.StartGame ()





        0