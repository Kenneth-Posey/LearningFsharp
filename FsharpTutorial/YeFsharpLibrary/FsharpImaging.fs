namespace FsharpImaging

open System
open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices

open MathAlgorithms

module ImageFunctions = 
    let LoadBitmapIntoArray (pBitmap:Bitmap) =
        let tBitmapData = pBitmap.LockBits( Rectangle(Point.Empty, pBitmap.Size) 
                                          , ImageLockMode.ReadOnly
                                          , PixelFormat.Format24bppRgb )  
        let tImageArrayLength = Math.Abs(tBitmapData.Stride) * pBitmap.Height
        let tImageDataArray = Array.zeroCreate<byte> tImageArrayLength
            
        Marshal.Copy(tBitmapData.Scan0, tImageDataArray, 0, tImageArrayLength)
        pBitmap.UnlockBits(tBitmapData)

        ( pBitmap.Width , pBitmap.Height , tBitmapData.Stride ) , tImageDataArray

        // Notes:
        // Image pixel data is stored BGR ( blue green red )
        // Image data is padded to be divisible by 4 (int32 width)
        
    let Transform2D ( (pDimension:int*int*int) , (pArray:byte[]) ) = 
        let tWidth , tHeight , tStride = pDimension

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
            let rec WidthLoopRec heightIndex widthIndex =
                let ContinueLoop () = WidthLoopRec heightIndex (widthIndex + 1)
                let currentLargePixel = largeArray.[heightIndex].[widthIndex]

                match ( widthIndex < searchWidth , currentLargePixel = firstSmallPixel ) with
                | ( true  , true  ) ->  let foundImage = ArrayFunctions.SearchSubset smallArray largeArray ( heightIndex, widthIndex )
                                        if foundImage then widthIndex , foundImage
                                        else ContinueLoop ()
                | ( true  , false ) -> ContinueLoop ()
                | ( false , _     ) -> widthIndex , false

            WidthLoopRec heightIndex 0 

        let HeightLoop () =
            let rec HeightLoopRec heightIndex =
                let widthIndex, foundImage = WidthLoop heightIndex

                match ( foundImage , heightIndex < searchHeight ) with
                | ( false , true  ) -> HeightLoopRec ( heightIndex + 1 ) 
                | ( _     , _     ) -> foundImage , widthIndex , heightIndex
                
            HeightLoopRec 0 
            
        HeightLoop ()
