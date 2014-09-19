namespace MainProgram

module Main = 

    open FsharpImaging
    open Testing
    open EveOnline
    open EveOnline.EveData
    open EveOnline.EveData.RawMaterials
    open System.Drawing
    
    let RunImageSearch () =
        // consoletest([|""|]) |> ignore

        ImageSearchTest.TestFunctions() |> ignore
        
        // General plan for looping
        // - Split single array for small image into 2d array of arrays
        // - Split single array for large image into 2d array of arrays
        // - Iterate through each array in large array looking for first value from small array
        // - If first value is found, then check for complete row
        // - If row is complete then check for second row (and so on)
        // - Stop searching horizontally when remaining pixels are smaller than search image width
        // - Stop searching vertically when remaining pixels are smaller than search image height 
        
        try
            use tSmallBitmap = new Bitmap("testimage2.bmp")
            use tLargeBitmap = new Bitmap("testimage1.bmp")
        
            let tSuccess, xCoord, yCoord = ImageSearch.SearchBitmap tSmallBitmap tLargeBitmap
            
            ()
        with
            | _ -> ()
    
    [<EntryPoint>]
    let main (args:string[]) = 
        
        let Locations = [
                SystemName.Jita    
                SystemName.Amarr   
                SystemName.Dodixie 
                SystemName.Rens    
                SystemName.Hek     
            ]

        let OreList : List<IOre * IOre> = [
                ( new Veldspar   () :> IOre , new CompVeldspar   () :> IOre )
                ( new Scordite   () :> IOre , new CompScordite   () :> IOre )
                ( new Pyroxeres  () :> IOre , new CompPyroxeres  () :> IOre )
                ( new Hedbergite () :> IOre , new CompHedbergite () :> IOre )
                ( new Hemorphite () :> IOre , new CompHemorphite () :> IOre )
                ( new Jaspet     () :> IOre , new CompJaspet     () :> IOre )           
            ]
            
        for location in Locations do
            for orePair in OreList do
                let system = string location       // Gets the enum name
                let name   = (fst orePair).GetName()

                let fast = MarketParser.FastProfit orePair (string (int location))
                let best = MarketParser.BestProfit orePair (string (int location))

                printfn "For system %8s fast profit: %.2f on ore type %s" system fast name
                printfn "For system %8s best profit: %.2f on ore type %s" system best name
            
        // There's four values per ore type to work with:
        // - Comp highest buy / lowest sell
        // - Raw  highest buy / lowest sell

        // The profit is sell 1 compressed - buy * 100 raw

        // There's two potential sell values
        // - Comp highest buy / lowest sell
        // There's two potential buy values
        // - Raw  highest buy / lowest sell

        // Slow (max profit)
        // Cost   : buy  raw  at highest buy + 0.01
        // Profit : sell comp at lowest sell - 0.01

        // Fast (instant profit)
        // Cost   : buy  raw  at lowest sell (real cost)
        // Profit : sell comp at highest buy (real profit)

        // We want to know the profit per unit and per m^3





        
        // Must return from function
        0