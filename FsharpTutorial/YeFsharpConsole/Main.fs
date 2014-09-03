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
        
        // Lists can only have one type so we have to 

        let Locations : List<string> = [
                string ( int SystemName.Jita    )
                string ( int SystemName.Amarr   )
                string ( int SystemName.Dodixie )
                string ( int SystemName.Rens    )
                string ( int SystemName.Hek     )
            ]

        let OreList : List<IOre> = [
                new Veldspar ()
                // new Scordite ()
                // new Pyroxeres ()
                // new Hedbergite ()
                // new Hemorphite ()
                // new Jaspet ()
                new CompVeldspar ()
                // new CompScordite ()
                // new CompPyroxeres ()
                // new CompHedbergite ()
                // new CompHemorphite ()
                // new CompJaspet ()
            ]

        let Buy x = MarketParser.RunBuy (string x) Locations.Head 1000.0f
        
        // let BuyValues = 
        //     OreList
        //     |> List.map  (fun x -> { Name   = x.GetName () 
        //                              TypeId = x.GetBase () 
        //                              Value  = Buy ( x.GetBase () )
        //                              IsTiny = x.IsTiny () } )
        //  |> List.iter (fun x -> printfn "Cost to buy 1000 %s is %0.2f" x.Name x.Value)
        
        printfn "Profit: %A" (MarketParser.FastProfit (new Veldspar () , new CompVeldspar ()))

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