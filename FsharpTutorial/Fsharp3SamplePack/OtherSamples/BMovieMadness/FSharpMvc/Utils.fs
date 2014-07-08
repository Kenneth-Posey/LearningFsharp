namespace FSharpMvc.Utils

// some small helper functions and wrappers
module Utils =

    let rng = new System.Random()

    /// Knuth shuffle
    let shuffle (array : 'a array) = 
        let n = array.Length
        for x in 1..n do
            let i = n - x
            let j = rng.Next(i + 1)
            let tmp = array.[i]
            array.[i] <- array.[j]
            array.[j] <- tmp
        array

    /// get a random subset by taking the first subsetSize elements from the shuffled array
    let randomSubset(array : 'a array, subsetSize: int) =
        array 
            |> shuffle 
            |> Seq.truncate subsetSize
            |> Seq.toArray