open System
open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices


module imagesearch = 
    type BitmapSearchClass() = 
        // Largely inspired by this article
        // http://msdn.microsoft.com/en-us/library/system.drawing.imaging.bitmapdata.aspx
        static member loadBitmapIntoArray (pBitmap:Bitmap) =
            let tBitmapRectangle = Rectangle(0, 0, pBitmap.Width, pBitmap.Height)
            
            let tLockMode = ImageLockMode.ReadOnly
            let tImageFormat = PixelFormat.Format24bppRgb
            
            let tBitmapData = pBitmap.LockBits(tBitmapRectangle, tLockMode, tImageFormat) 
            let tIntPtr = tBitmapData.Scan0

            let tImageByteLength = Math.Abs(tBitmapData.Stride) * pBitmap.Height

            // Zerocreate creates the "empty" array
            let tImageRGBValues : byte array = Array.zeroCreate tImageByteLength

            do Marshal.Copy(tImageRGBValues, 0, tIntPtr, tImageByteLength)

            do pBitmap.UnlockBits(tBitmapData)

            tImageRGBValues, pBitmap.Width, pBitmap.Height

        member this.searchBitmap (pSmallBitmap:Bitmap) (pLargeBitmap:Bitmap) (pTolerance:Double) = 
            
            let tSmallBytes, tSmallWidth, tSmallHeight = BitmapSearchClass.loadBitmapIntoArray pSmallBitmap
            let tLargeBytes, tLargeWidth, tLargeHeight = BitmapSearchClass.loadBitmapIntoArray pLargeBitmap

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