namespace FsharpImaging

open System
open System.Drawing
open System.Drawing.Imaging
open System.Runtime.InteropServices

open MathAlgorithms

module ImageFunctions = 
    let LoadBitmapIntoArray (sourceBitmap:Bitmap) =
        let sourceBitmapData = sourceBitmap.LockBits( Rectangle(Point.Empty, sourceBitmap.Size) 
                                                    , ImageLockMode.ReadOnly
                                                    , PixelFormat.Format24bppRgb )  
        let imageArrayLength = Math.Abs(sourceBitmapData.Stride) * sourceBitmap.Height
        let imageDataArray = Array.zeroCreate<byte> imageArrayLength
            
        Marshal.Copy(sourceBitmapData.Scan0, imageDataArray, 0, imageArrayLength)
        sourceBitmap.UnlockBits(sourceBitmapData)

        ( sourceBitmap.Width , sourceBitmap.Height , sourceBitmapData.Stride ) , imageDataArray

        // Notes:
        // Image pixel data is stored BGR ( blue green red )
        // Image data is padded to be divisible by 4 (int32 width)
        
    let TransformImageArrayInto2D ( (imageData:int*int*int) , (sourceArray:byte[]) ) = 
        let width , height , stride = imageData

        [|
            for heightIndex in 0 .. ( height - 1 ) do
                let startIndex  = heightIndex * stride
                let finishIndex = ( startIndex + width * 3 ) - 1
                yield [|    
                    for widthIndex in startIndex .. 3 .. finishIndex do
                        yield ( sourceArray.[widthIndex]
                              , sourceArray.[widthIndex + 1] 
                              , sourceArray.[widthIndex + 2] )
                |]
        |]



module ImageSearch = 
    open ImageFunctions

    let SearchBitmap (smallBitmap:Bitmap) (largeBitmap:Bitmap) = 
        
        let smallArray = TransformImageArrayInto2D <| LoadBitmapIntoArray smallBitmap 
        let largeArray = TransformImageArrayInto2D <| LoadBitmapIntoArray largeBitmap
        
        let searchWidth = largeBitmap.Width - smallBitmap.Width
        let searchHeight = largeBitmap.Height - smallBitmap.Height

        let firstSmallPixel = smallArray.[0].[0]
        
        let WidthLoop heightIndex =
            let rec WidthLoopRec heightIndex widthIndex =
                let ContinueLoop () = WidthLoopRec heightIndex (widthIndex + 1)
                let currentLargePixel = largeArray.[heightIndex].[widthIndex]

                match ( widthIndex < searchWidth , currentLargePixel = firstSmallPixel ) with
                | ( true  , true  ) ->  let foundImage = ArrayFunctions.IsSubMatrix smallArray largeArray ( heightIndex, widthIndex )
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
