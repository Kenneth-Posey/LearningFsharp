open System
open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices


module ImageSearch = 
        // Largely inspired by this article
        // http://msdn.microsoft.com/en-us/library/system.drawing.imaging.bitmapdata.aspx
    let LoadBitmapIntoArray (pBitmap:Bitmap) =
        let tBitmapData = pBitmap.LockBits( Rectangle(Point.Empty, pBitmap.Size), 
                                            ImageLockMode.ReadOnly, 
                                            PixelFormat.Format24bppRgb )  
            
        let tImageArrayLength = Math.Abs(tBitmapData.Stride) * pBitmap.Height
        let tImageDataArray = Array.zeroCreate<byte> tImageArrayLength
            
        Marshal.Copy(tImageDataArray, 0, tBitmapData.Scan0, tImageArrayLength)
        pBitmap.UnlockBits(tBitmapData)

        tImageDataArray, pBitmap.Width, pBitmap.Height

    let Transform2D (pArrayWidth:int) (pArray:byte[]) = 
        let tHeight = pArray.Length / pArrayWidth

        let tImageArray = [|
            for tRowIndex in [ 0 .. tHeight - 1] do
                let tStart  = tRowIndex * pArrayWidth
                let tFinish = tStart + pArrayWidth - 1 
                yield pArray.[tStart .. tFinish]
        |]
        
        tImageArray

    let SearchBitmap (pSmallBitmap:Bitmap) (pLargeBitmap:Bitmap) = 
            
        let tSmallBytes, tSmallWidth, tSmallHeight = LoadBitmapIntoArray pSmallBitmap
        let tLargeBytes, tLargeWidth, tLargeHeight = LoadBitmapIntoArray pLargeBitmap
    
        let mutable tMatchFound = false
        let mutable tLargeDomain = tLargeWidth - tSmallWidth 
        let mutable tLargeRange = tLargeHeight - tSmallHeight
        



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
