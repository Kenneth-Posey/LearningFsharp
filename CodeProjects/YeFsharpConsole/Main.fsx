 namespace MainProgram

open EveData
open Market

open Format.Text

module Main = 

    // [<EntryPoint>]
    let main (args:string[]) = 
        
        let Locations = Collections.Locations
        let OreList = Collections.AllOrePairs
           
        let mineralValue = Functions.LoadMineralJitaSell ()
        let oreValue = Collections.Dynamic.LoadBaseOreValues 1.0f mineralValue

        let iceProductValue = Functions.LoadIceProductJitaSell ()
        let iceValue = Collections.Dynamic.LoadBaseIceValues 1.0f iceProductValue




        for location in Locations do
            for orePair in OreList do
                let system = string location           // Gets the enum name
                let name   = (fst orePair).GetName()

                let fast = PrettyPrintFromSingle <| Functions.FastProfit orePair (int location)
                let best = PrettyPrintFromSingle <| Functions.BestProfit orePair (int location)

                printfn "For system %5s fast profit: %6s on ore type %s" system fast name
                printfn "For system %5s best profit: %6s on ore type %s" system best name
        

        
        // Must return from program
        0
