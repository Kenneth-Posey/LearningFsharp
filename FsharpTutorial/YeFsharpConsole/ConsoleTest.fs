module ConsoleTest
    let read = System.Console.ReadLine
    let consoletest (args:string[]) = 
        while true do
            let input = read ()
            

            printfn "%A" input
            ()
        ()