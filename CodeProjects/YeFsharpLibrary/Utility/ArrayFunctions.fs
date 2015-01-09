namespace MathAlgorithms

module ArrayFunctions = 
    // Generic because it makes testing easier, yay math
    let IsSubMatrix (smallArray:'a[][]) (largeArray:'a[][]) (startCoordinate:(int * int)) =
        let searchHeight , searchWidth = smallArray.Length - 1 , smallArray.[0].Length - 1
        let startHeight  , startWidth  = startCoordinate

        try 
            let WidthLoop heightIndex = 
                let rec WidthLoopRec heightIndex widthIndex =
                    let largeValue = largeArray.[startHeight + heightIndex].[startWidth + widthIndex]
                    let smallValue = smallArray.[heightIndex].[widthIndex]

                    match ( smallValue = largeValue , widthIndex < searchWidth ) with
                    | ( true  , true  ) -> WidthLoopRec heightIndex ( widthIndex + 1 )
                    | ( true  , false ) -> true 
                    | ( false , _     ) -> false
                WidthLoopRec heightIndex 0

            let HeightLoop () =
                let rec HeightLoopRec heightIndex = 
                    let isMatch = WidthLoop heightIndex

                    match ( isMatch , heightIndex < searchHeight ) with
                    | ( true  , true  ) -> HeightLoopRec ( heightIndex + 1 )
                    | ( true  , false ) -> true
                    | ( false , _     ) -> false
                HeightLoopRec 0

                // let asyncGroup = Async.Parallel [ for heightIndex in 0 .. searchHeight -> 
                //                                     async { return HeightLoopRec heightIndex , heightIndex } ]
                // let asyncResult = Async.RunSynchronously asyncGroup
                // 
                // let resultBools =  asyncResult |> Array.map (fun (x, y) -> x)
                // let result = resultBools |> Array.fold (fun acc elem -> ())
                // 
                // false

            HeightLoop ()
            
        with // Not really sure what I want to do with error handling atm
            | :? System.ArgumentOutOfRangeException -> false
            | :? System.ArgumentNullException       -> false
            | :? System.ArgumentException           -> false