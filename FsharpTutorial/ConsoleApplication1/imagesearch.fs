open System
open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices


module ImageSearch = 
    let LoadBitmapIntoArray (pBitmap:Bitmap) =
        let tBitmapData = pBitmap.LockBits( Rectangle(Point.Empty, pBitmap.Size), 
                                            ImageLockMode.ReadOnly, 
                                            PixelFormat.Format24bppRgb )  
        // Bitmap.Stride can be negative to indicate different orientation of the bitmap
        let tImageArrayLength = Math.Abs(tBitmapData.Stride) * pBitmap.Height
        let tImageDataArray = Array.zeroCreate<byte> tImageArrayLength
            
        // Marshal.Copy(tImageDataArray, 0, tBitmapData.Scan0, tImageArrayLength)
        Marshal.Copy(tBitmapData.Scan0, tImageDataArray, 0, tImageArrayLength)
        pBitmap.UnlockBits(tBitmapData)

        pBitmap.Width, tImageDataArray

    let Transform2D ( (pArrayWidth:int), (pArray:byte[]) ) = 
        let tHeight = pArray.Length / ( pArrayWidth * 3 ) // 3 bytes per RGB

        [|
            for tHeightIndex in 0 .. tHeight - 1 do
                let tStart  = tHeightIndex * ( pArrayWidth * 3 - 1 )
                let tFinish = tStart + pArrayWidth * 3 - 1 
                yield [|    
                    for tWidthIndex in tStart .. 3 .. tFinish do
                        yield ( pArray.[tWidthIndex]
                              , pArray.[tWidthIndex + 1] 
                              , pArray.[tWidthIndex + 2] )
                |]
        |]

    let SearchSubset (tSmallArray:(byte * byte * byte)[][]) (tLargeArray:(byte * byte * byte)[][]) (pCoordinate:(int * int)) =
        let tSmallHeight = tSmallArray.Length
        let tSmallWidth = tSmallArray.[0].Length

        let tHeightIndex = fst pCoordinate
        let tWidthIndex = snd pCoordinate
        let mutable tSmallHeightIndex = 0
        let mutable tSmallWidthIndex = 0
        let mutable tMatch = true

        try 
            while ( tSmallHeightIndex < tSmallHeight - 1 ) && tMatch do
                while ( tSmallWidthIndex < tSmallWidth - 1 ) && tMatch do
                    let tLargeCurrentValue = tLargeArray.[tHeightIndex + tSmallHeightIndex].[tWidthIndex + tSmallWidthIndex]
                    let tSmallCurrentValue = tSmallArray.[tSmallHeightIndex].[tSmallWidthIndex]

                    if tSmallCurrentValue = tLargeCurrentValue then
                        tSmallWidthIndex <- tSmallWidthIndex + 1         
                    else
                        tMatch <- false
                tSmallHeightIndex <- tSmallHeightIndex + 1

            tMatch
        with
            | _ -> false

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
                    tMatch <- SearchSubset tSmallArray tLargeArray ( tHeightIndex, tWidthIndex )
                    if tMatch then tContinue <- false

                if tMatch = false && tContinue = true then
                    tWidthIndex <- tWidthIndex + 1

            if tMatch = false && tContinue = true then
                tHeightIndex <- tHeightIndex + 1

        tMatch, tWidthIndex, tHeightIndex

    [<EntryPoint>]
    let main (args:string[]) = 
        
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