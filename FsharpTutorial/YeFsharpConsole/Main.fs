namespace MainProgram

module Main = 

    open FsharpImaging
    open Testing
    open EveOnline
    open EveOnline.EveData
    open System.Drawing
    
    [<EntryPoint>]
    let main (args:string[]) = 
        
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

        // Have to cast enum to int then to string to get actual value
        // Then construct into tuple for passing into lambda expression
        // (string (int RawMaterials.Veldspar.Default), string (int SystemName.Amarr))
        // |> (fun (veld,amarr) -> EveData.QuickLook + "?typeid=" + veld + "&usesystem=" + amarr)
        // |> MarketParser.LoadUrl 
        // |> MarketParser.ParseQuickLook
        // |> ignore

        printfn "%A" <| ( MarketParser.RunVeldsparBuy().ToString("F2") )
        printfn "%A" <| ( MarketParser.RunVeldsparSell().ToString("F2") )
        printfn "%A" <| ( MarketParser.RunPyroxeres().ToString("F2") )
        // Must return from function
        0