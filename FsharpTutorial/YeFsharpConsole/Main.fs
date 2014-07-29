module Main

open FsharpImaging.ImageSearch
open Testing
open System.Drawing

    [<EntryPoint>]
    let main (args:string[]) = 
        
        ImageSearchTest.TestFunctions() |> ignore

        use tSmallBitmap = new Bitmap("testimage2.bmp")
        use tLargeBitmap = new Bitmap("testimage1.bmp")

        let tSuccess, xCoord, yCoord = SearchBitmap tSmallBitmap tLargeBitmap

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