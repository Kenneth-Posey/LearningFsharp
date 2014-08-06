namespace FsharpImaging

open System
open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices

open MathAlgorithms

module ImageFunctions = 
    let LoadBitmapIntoArray (pBitmap:Bitmap) =
        let tBitmapData = pBitmap.LockBits( Rectangle(Point.Empty, pBitmap.Size), 
                                            ImageLockMode.ReadOnly, 
                                            PixelFormat.Format24bppRgb )  
        let tImageArrayLength = Math.Abs(tBitmapData.Stride) * pBitmap.Height
        let tImageDataArray = Array.zeroCreate<byte> tImageArrayLength
            
        Marshal.Copy(tBitmapData.Scan0, tImageDataArray, 0, tImageArrayLength)
        pBitmap.UnlockBits(tBitmapData)

        ( pBitmap.Width, pBitmap.Height, tBitmapData.Stride ), tImageDataArray

        // Notes:
        // Image pixel data is stored BGR ( blue green red )
        // Image data is padded to be divisible by 4 (int32 width)
        
    let Transform2D ( (pDimension:int*int*int), (pArray:byte[]) ) = 
        let tWidth, tHeight, tStride = pDimension

        [|
            for tHeightIndex in 0 .. ( tHeight - 1 ) do
                let tStart  = tHeightIndex * tStride
                let tFinish = ( tStart + tWidth * 3 ) - 1
                yield [|    
                    for tWidthIndex in tStart .. 3 .. tFinish do
                        yield ( pArray.[tWidthIndex]
                              , pArray.[tWidthIndex + 1] 
                              , pArray.[tWidthIndex + 2] )
                |]
        |]



module ImageSearch = 
    open ImageFunctions

    let SearchBitmap (smallBitmap:Bitmap) (largeBitmap:Bitmap) = 
        
        let smallArray = Transform2D <| LoadBitmapIntoArray smallBitmap 
        let largeArray = Transform2D <| LoadBitmapIntoArray largeBitmap
        
        let searchWidth = largeBitmap.Width - smallBitmap.Width
        let searchHeight = largeBitmap.Height - smallBitmap.Height

        let firstSmallPixel = smallArray.[0].[0]
        
        let WidthLoop heightIndex =
            let rec WidthLoopRec heightIndex widthIndex widthContinue =
                let ContinueLoop () = WidthLoopRec heightIndex (widthIndex + 1) true

                match ( widthIndex < searchWidth , widthContinue ) with
                // Currently within image search bounds and no image found
                | ( true , true ) -> 
                    if largeArray.[heightIndex].[widthIndex] = firstSmallPixel then 
                        let foundImage = ArrayFunctions.SearchSubset smallArray largeArray ( heightIndex, widthIndex )
                        if foundImage then widthIndex, false, true
                        else ContinueLoop ()
                    else ContinueLoop ()
                // Currently inside of image search bounds and NO continue => image found  
                | ( true , false ) -> widthIndex, false, true
                // Currently outside of image search bounds and no image found
                | ( false, false ) -> 0, false, false
                | ( _ , _ )        -> 0, false, false

            WidthLoopRec heightIndex 0 true

        let HeightLoop () =
            let rec HeightLoopRec heightIndex heightContinue =
                let widthIndex, widthContinue, foundImage = 
                    match ( heightIndex < searchHeight , heightContinue ) with
                    | ( true  , true  ) -> WidthLoop heightIndex // Image not found, continue
                    | ( true  , false ) -> 0, false, false // Image found, don't continue
                    | ( false , _     ) -> 0, false, false // Image not found, don't continue

                match (widthContinue, foundImage) with
                | ( true  , _     ) -> HeightLoopRec ( heightIndex + 1 ) true
                | ( false , true  ) -> true, widthIndex, heightIndex
                | ( false , false ) -> false, widthIndex, heightIndex
                
            HeightLoopRec 0 true    // pHeightIndex pContinue

        do HeightLoop |> ignore

        false, 0, 0

        // while ( tHeightIndex < tSearchHeight - 1 ) && tContinue do
        //     while ( tWidthIndex < tSearchWidth - 1 ) && tContinue do
        //         let tCurrentValue = tLargeArray.[tHeightIndex].[tWidthIndex]
        // 
        //         if tCurrentValue = tFirstSmallPixel then
        //             tMatch <- ArrayFunctions.SearchSubset tSmallArray tLargeArray ( tHeightIndex, tWidthIndex )
        //             if tMatch then tContinue <- false
        // 
        //         if tMatch = false && tContinue = true then
        //             tWidthIndex <- tWidthIndex + 1
        // 
        //     if tMatch = false && tContinue = true then
        //         tWidthIndex  <- 0                       // Reset to search next row
        //         tHeightIndex <- tHeightIndex + 1
        // 
        // tMatch, tWidthIndex, tHeightIndex