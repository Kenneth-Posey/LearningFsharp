open System
open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices


module ImageSearch = 
    let LoadBitmapIntoArray (pBitmap:Bitmap) =
        let tBitmapData = pBitmap.LockBits( Rectangle(Point.Empty, pBitmap.Size), 
                                            ImageLockMode.ReadOnly, 
                                            PixelFormat.Format24bppRgb )  
            
        let tImageArrayLength = Math.Abs(tBitmapData.Stride) * pBitmap.Height
        let tImageDataArray = Array.zeroCreate<byte> tImageArrayLength
            
        Marshal.Copy(tImageDataArray, 0, tBitmapData.Scan0, tImageArrayLength)
        pBitmap.UnlockBits(tBitmapData)

        pBitmap.Width, tImageDataArray

    let Transform2D ( (pArrayWidth:int), (pArray:byte[]) ) = 
        let tHeight = pArray.Length / ( pArrayWidth * 3 ) // 3 bytes per RGB

        let tImageArray = [|
            for tHeightIndex in [ 0 .. tHeight - 1] do
                let tStart  = tHeightIndex * pArrayWidth
                let tFinish = tStart + pArrayWidth - 1 
                yield [|    
                    for tWidthIndex in [tStart .. 3 .. tFinish] do
                        yield ( pArray.[tWidthIndex]
                              , pArray.[tWidthIndex + 1] 
                              , pArray.[tWidthIndex + 2] )
                |]
        |]
        
        tImageArray

    let SearchBitmap (pSmallBitmap:Bitmap) (pLargeBitmap:Bitmap) = 
        
        let tSmallArray = Transform2D <| LoadBitmapIntoArray pSmallBitmap 
        let tLargeArray = Transform2D <| LoadBitmapIntoArray pLargeBitmap

        let mutable tMatchFound = false
        



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
