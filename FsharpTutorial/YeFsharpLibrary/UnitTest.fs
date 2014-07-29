namespace Testing

open FsharpImaging
open System.Diagnostics

module ImageSearchTest =
    
    let Test_SearchSubset () =
        try 
            let tTestSmall = [|
                                [| 0; 0; 0; 0 |]
                                [| 0; 1; 1; 0 |]
                                [| 0; 1; 1; 0 |]
                                [| 0; 0; 0; 0 |]
                             |]
            
            // Contains smaller array at 2, 2
            let tTestLarge = [|
                                [| 1; 1; 1; 1; 1; 1; 1; 0; 2; 3; 4;|]
                                [| 1; 1; 1; 1; 1; 1; 1; 0; 2; 3; 4;|]
                                [| 1; 1; 0; 0; 0; 0; 1; 0; 2; 3; 4;|]
                                [| 1; 1; 0; 1; 1; 0; 1; 0; 2; 3; 4;|]
                                [| 1; 1; 0; 1; 1; 0; 1; 0; 2; 3; 4;|]
                                [| 1; 1; 0; 0; 0; 0; 1; 0; 2; 3; 4;|]
                                [| 1; 1; 1; 1; 1; 1; 1; 0; 2; 3; 4;|]
                                [| 1; 1; 1; 1; 1; 1; 1; 0; 2; 3; 4;|]
                                [| 1; 1; 1; 1; 1; 1; 1; 0; 2; 3; 4;|]
                             |]
        
            let tTrueResult = ImageSearch.SearchSubset tTestSmall tTestLarge ( 1, 1 )
            let tFalseResult = ImageSearch.SearchSubset tTestSmall tTestLarge ( 2, 2 )

            Debug.Assert( ( tTrueResult = true ), "Failed to find sub-array" )
            Debug.Assert( ( tFalseResult = false ), "False positive in finding sub-array")
            
            true // Successfully completed tests

        with
        | _ -> false // Something borked

       
    let TestFunctions () =
        try
            Test_SearchSubset() |> ignore

            true
        with
        | _ -> false