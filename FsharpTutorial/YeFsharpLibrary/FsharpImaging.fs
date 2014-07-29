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

    let SearchBitmap (pSmallBitmap:Bitmap) (pLargeBitmap:Bitmap) = 
        
        let tSmallArray = Transform2D <| LoadBitmapIntoArray pSmallBitmap 
        let tLargeArray = Transform2D <| LoadBitmapIntoArray pLargeBitmap
        
        let tSearchWidth = pLargeBitmap.Width - pSmallBitmap.Width
        let tSearchHeight = pLargeBitmap.Height - pSmallBitmap.Height

        let mutable tHeightIndex = 0
        let mutable tWidthIndex = 0
        let mutable tMatch = false
        let mutable tContinue = true

        while ( tHeightIndex < tSearchHeight - 1 ) && tContinue do
            while ( tWidthIndex < tSearchWidth - 1 ) && tContinue do
                let tCurrentValue = tLargeArray.[tHeightIndex].[tWidthIndex]
                let tFirstSmallPixel = tSmallArray.[0].[0]

                if tCurrentValue = tFirstSmallPixel then
                    tMatch <- ArrayFunctions.SearchSubset tSmallArray tLargeArray ( tHeightIndex, tWidthIndex )
                    if tMatch then tContinue <- false

                if tMatch = false && tContinue = true then
                    tWidthIndex <- tWidthIndex + 1

            if tMatch = false && tContinue = true then
                tWidthIndex  <- 0                       // Reset to search next row
                tHeightIndex <- tHeightIndex + 1

        tMatch, tWidthIndex, tHeightIndex