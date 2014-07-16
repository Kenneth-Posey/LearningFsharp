open System
open System.Drawing
open System.Drawing.Imaging

module imagesearch = 
    type BitmapSearchClass() = 
        let searchBitmap (pSmallBitmap:Bitmap) (pLargeBitmap:Bitmap) (pTolerance:Double) = 
            let tSmallRectangle = Rectangle(0, 0, pSmallBitmap.Width, pSmallBitmap.Height)
            let tLargeRectangle = Rectangle(0, 0, pLargeBitmap.Width, pLargeBitmap.Height)
        
            let tLockMode = ImageLockMode.ReadOnly
            let tImageFormat = PixelFormat.Format24bppRgb

            let tSmallBitmapData = pSmallBitmap.LockBits(tSmallRectangle, tLockMode, tImageFormat)
            let tLargeBitmapData = pLargeBitmap.LockBits(tLargeRectangle, tLockMode, tImageFormat)

            let tSmallScanWidth = tSmallBitmapData.Stride
            let tLargeScanWidth = tLargeBitmapData.Stride

            let tSmallWidth = pSmallBitmap.Width * 3
            let tSmallHeight = pSmallBitmap.Height

            let tLargeWidth = pLargeBitmap.Width
            let tLargeHeight = pLargeBitmap.Height - pSmallBitmap.Height + 1

            let tMargin = Convert.ToInt32(255.0 * pTolerance)
            let mutable tLocation = Rectangle.Empty
            
            let mutable tSmallScan : nativeint = tSmallBitmapData.Scan0
            let mutable tLargeScan : nativeint = tLargeBitmapData.Scan0 

            let tSmallOffset = tSmallScanWidth - pSmallBitmap.Width * 3
            let tLargeOffset = tLargeScanWidth - pLargeBitmap.Width * 3

            let mutable tSmallBackup = tSmallScan
            let mutable tLargeBackup = tLargeScan

            let mutable tMatchFound = false
            let mutable tLargeRange = 0
            let mutable tLargeDomain = 0

            let mutable tSmallRange = 0
            let mutable tSmallDomain = 0

            while tMatchFound = false && tLargeRange < tLargeHeight do
                while tMatchFound = false && tLargeDomain < tLargeWidth do
                    
                    tSmallBackup <- tSmallScan
                    tLargeBackup <- tLargeScan

                    while tMatchFound = false && tSmallRange < tSmallHeight do
                        while tMatchFound = false && tSmallDomain < tSmallWidth do
                            if tSmallScan.ToInt32() = tLargeScan.ToInt32() then 
                                tMatchFound <- true
                            else
                                tSmallScan <- tSmallScan + 1n
                                tLargeScan <- tLargeScan + 1n

                        tSmallScan <- tSmallBackup + tSmallScanWidth * (1n + tSmallRange :> nativeint)
                        tLargeScan <- tLargeBackup + tLargeScanWidth * (1 + tSmallRange)

                                
                    tLargeRange <- tLargeRange + 1
                    tLargeDomain <- tLargeDomain + 1

            // Dat return
            0