namespace MathAlgorithms

module ArrayFunctions = 
    let Transform2D ( (pDimension:int*int*int), (pArray:byte[]) ) = 
        let tWidth, tHeight, tStride = pDimension

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

    // Generic because it makes testing easier, yay math
    let SearchSubset (tSmallArray:'a[][]) (tLargeArray:'a[][]) (pCoordinate:(int * int)) =
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