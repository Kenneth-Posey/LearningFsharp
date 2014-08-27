namespace MainProgram

module Main = 

    open FsharpImaging
    open Testing
    open EveOnline
    open EveOnline.EveData
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

    let RunVeldsparUniversal () =
        

        ()
        

    [<EntryPoint>]
    let main (args:string[]) = 
        
        // Lists can only have one type so we have to 

        let Locations = [
            string ( int EveData.SystemName.Jita    )
            string ( int EveData.SystemName.Amarr   )
            string ( int EveData.SystemName.Dodixie )
            string ( int EveData.SystemName.Rens    )
            string ( int EveData.SystemName.Hek     )
            ]

        let Ore = [
            string ( int EveData.RawMaterials.Veldspar.Base  )
            string ( int EveData.RawMaterials.Scordite.Base  )
            string ( int EveData.RawMaterials.Pyroxeres.Base )
            ]

        let CompressedOre = [
            string ( int EveData.RawMaterials.CompVeldspar.Base  )
            string ( int EveData.RawMaterials.CompScordite.Base  )
            string ( int EveData.RawMaterials.CompPyroxeres.Base )
            ]





        // shim to compile
        let location = ""

        printfn "Buy 1 Compressed Veld:  %A" <| MarketParser.TestBuyCompressedVeld(location).ToString("F2")
        printfn "Sell 1 Compressed Veld: %A" <| MarketParser.TestSellCompressedVeld(location).ToString("F2")
        
        printfn "Buy 100 Veldspar:       %A" <| MarketParser.TestBuy100Veld(location).ToString("F2")
        printfn "Sell 100 Veldspar:      %A" <| MarketParser.TestSell100Veld(location).ToString("F2")
        // Must return from function
        0