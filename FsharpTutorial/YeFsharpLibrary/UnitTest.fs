namespace Testing

open FsharpImaging
open MathAlgorithms
open System.Diagnostics
open System.Drawing

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
        
            let tTrueResult = ArrayFunctions.SearchSubset tTestSmall tTestLarge ( 2, 2 )
            let tFalseResult = ArrayFunctions.SearchSubset tTestSmall tTestLarge ( 3, 3 )

            Debug.Assert( ( tTrueResult = true ), "Failed to find sub-array" )
            Debug.Assert( ( tFalseResult = false ), "False positive in finding sub-array")
            
            true // Successfully completed tests

        with
        | _ -> false // Something borked

    let Test_LoadBitmapIntoArray () =      
        try
            let tSmallBitmap = new Bitmap("C:\Users\kposey\Pictures\searchimage.bmp")
            let tResultHeight, tResultArray = ImageSearch.LoadBitmapIntoArray tSmallBitmap

            true
        with
        | _ -> false

    let Test_Transform2D () =
        try
            let tSmallBitmap = new Bitmap("C:\Users\kposey\Pictures\searchimage.bmp")
            let tResultArray = ArrayFunctions.Transform2D <| ImageSearch.LoadBitmapIntoArray tSmallBitmap
            
            true
        with
        | _ -> false

    let TestFunctions () =
        try
            Test_SearchSubset() |> ignore
            Test_LoadBitmapIntoArray() |> ignore
            Test_Transform2D() |> ignore

            true
        with
        | _ -> false