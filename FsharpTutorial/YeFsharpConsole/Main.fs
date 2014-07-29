module Main

open FsharpImaging.ImageSearch
open Testing
open System.Drawing
open System.Diagnostics

    [<EntryPoint>]
    let main (args:string[]) = 
        
        ImageSearchTest.TestFunctions() |> ignore

        // use tSmallBitmap = new Bitmap("testimage2.bmp")
        // use tLargeBitmap = new Bitmap("testimage1.bmp")
        
        use tSmallBitmap = new Bitmap("searchimage.bmp")
        use tLargeBitmap = new Bitmap("containingimage.bmp")

        let stopW = new Stopwatch()
        
        let mutable it = 0
        stopW.Start() |> ignore
        while it < 20 do
            let tSuccess, xCoord, yCoord = SearchBitmap tSmallBitmap tLargeBitmap
            it <- it + 1    
        stopW.Stop() |> ignore
        
        printfn "%A" stopW.ElapsedMilliseconds

        // General plan for looping
        // - Split single array for small image into 2d array of arrays
        // - Split single array for large image into 2d array of arrays
        // - Iterate through each array in large array looking for first value from small array
        // - If first value is found, then check for complete row
        // - If row is complete then check for second row (and so on)
        // - Stop searching horizontally when remaining pixels are smaller than search image width
        // - Stop searching vertically when remaining pixels are smaller than search image height 

        // Must return from function
        0