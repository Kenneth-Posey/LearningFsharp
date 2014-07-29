namespace FsharpImaging

open System
open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices

open MathAlgorithms

module ImageSearch = 
    let LoadBitmapIntoArray (pBitmap:Bitmap) =
        let tBitmapData = pBitmap.LockBits( Rectangle(Point.Empty, pBitmap.Size), 
                                            ImageLockMode.ReadOnly, 
                                            PixelFormat.Format24bppRgb )  
        let tImageArrayLength = Math.Abs(tBitmapData.Stride) * pBitmap.Height
        let tImageDataArray = Array.zeroCreate<byte> tImageArrayLength
            
        Marshal.Copy(tBitmapData.Scan0, tImageDataArray, 0, tImageArrayLength)
        pBitmap.UnlockBits(tBitmapData)

        printfn "%A" tImageDataArray

        pBitmap.Width, tImageDataArray

        // Notes:
        // Image pixel data is stored BGR ( blue green red )
        // If stride is positive, then image is upside down ( bottom right -> upper left )
        // If stride is negative, then image is rightside up ( upper left -> bottom right )
        // Image data is padded to be divisible by 4 (int32 width)

    let SearchBitmap (pSmallBitmap:Bitmap) (pLargeBitmap:Bitmap) = 
        
        let tSmallArray = ArrayFunctions.Transform2D <| LoadBitmapIntoArray pSmallBitmap 
        let tLargeArray = ArrayFunctions.Transform2D <| LoadBitmapIntoArray pLargeBitmap
        
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
                tHeightIndex <- tHeightIndex + 1

        tMatch, tWidthIndex, tHeightIndex