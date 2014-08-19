namespace MainProgram

module Main = 

    open FsharpImaging
    open Testing
    open EveOnline
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

        let FilterByName (name:string) (tuple:(string * string)[]) =
            tuple
            |> Array.filter (fun (x, y) -> y.Contains(name))

        let FilterOreOnly (tuple:(string * string)[]) = 
            tuple
            |> Array.filter (fun (x, y) -> y.Contains("Blue") = false)
            |> Array.filter (fun (x, y) -> y.Contains("Processing") = false)
            |> Array.filter (fun (x, y) -> y.Contains("Mining") = false)
            
        let itemArray = MarketParser.LoadTypeIdsFromUrl EveData.TypeIdUrl

        let tritItems  = itemArray |> FilterOreOnly |> FilterByName "Tritanium"
        let veldItems  = itemArray |> FilterOreOnly |> FilterByName "Veldspar"
        let scordItems = itemArray |> FilterOreOnly |> FilterByName "Scordite"
        let pyroItems  = itemArray |> FilterOreOnly |> FilterByName "Pyroxeres"
        let kernItems  = itemArray |> FilterOreOnly |> FilterByName "Kernite"
        let hemItems   = itemArray |> FilterOreOnly |> FilterByName "Hemorphite"
        let hedItems   = itemArray |> FilterOreOnly |> FilterByName "Hedbergite"
        let jaspItems  = itemArray |> FilterOreOnly |> FilterByName "Jaspet"

        // Must return from function
        0