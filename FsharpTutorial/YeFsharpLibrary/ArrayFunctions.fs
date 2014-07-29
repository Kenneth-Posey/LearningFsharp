namespace MathAlgorithms

module ArrayFunctions = 
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

                tSmallWidthIndex  <- 0
                tSmallHeightIndex <- tSmallHeightIndex + 1

            tMatch
        with
            | _ -> false