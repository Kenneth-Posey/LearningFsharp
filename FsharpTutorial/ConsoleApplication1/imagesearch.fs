open System
open System.Drawing
open System.Drawing.Imaging

module imagesearch = 
    let searchBitmap (pSmallBitmap:Bitmap) (pLargeBitmap:Bitmap) = 
        let tSmallRectangle = Rectangle(0, 0, pSmallBitmap.Width, pSmallBitmap.Height)
        let tLargeRectangle = Rectangle(0, 0, pLargeBitmap.Width, pLargeBitmap.Height)
        
        let tLockMode = ImageLockMode.ReadOnly
        let tImageFormat = PixelFormat.Format24bppRgb

        let tSmallBitmapData = pSmallBitmap.LockBits(tSmallRectangle, tLockMode, tImageFormat)