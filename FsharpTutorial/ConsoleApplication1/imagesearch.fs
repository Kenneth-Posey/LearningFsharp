open System
open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices


module ImageSearch = 
    type BitmapSearch() = 
        // Largely inspired by this article
        // http://msdn.microsoft.com/en-us/library/system.drawing.imaging.bitmapdata.aspx
        static member LoadBitmapIntoArray (pBitmap:Bitmap) =
            let tBitmapData = pBitmap.LockBits( Rectangle(Point.Empty, pBitmap.Size), 
                                                ImageLockMode.ReadOnly, 
                                                PixelFormat.Format24bppRgb )  
            
            let tImageArrayLength = Math.Abs(tBitmapData.Stride) * pBitmap.Height
            let tImageDataArray = Array.zeroCreate<byte> tImageArrayLength
            
            Marshal.Copy(tImageDataArray, 0, tBitmapData.Scan0, tImageArrayLength)
            pBitmap.UnlockBits(tBitmapData)

            tImageDataArray, pBitmap.Width, pBitmap.Height

        member this.searchBitmap (pSmallBitmap:Bitmap) (pLargeBitmap:Bitmap) (pTolerance:Double) = 
            
            let tSmallBytes, tSmallWidth, tSmallHeight = BitmapSearch.LoadBitmapIntoArray pSmallBitmap
            let tLargeBytes, tLargeWidth, tLargeHeight = BitmapSearch.LoadBitmapIntoArray pLargeBitmap

            let tMargin = Convert.ToInt32(255.0 * pTolerance)
            let mutable tLocation = Rectangle.Empty
            let mutable tMatchFound = false
            
            let mutable tMatchFound = false
            let mutable tLargeRange = 0
            let mutable tLargeDomain = 0

            let mutable tSmallRange = 0
            let mutable tSmallDomain = 0


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
