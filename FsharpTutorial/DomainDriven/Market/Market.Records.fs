namespace EveOnline.MarketDomain

module Records = 
    open EveOnline.ProductDomain.Types
    open EveOnline.OreDomain.Types
    open EveOnline.OreDomain.Records
    open EveOnline.OreDomain.Ore
    open EveOnline.IceDomain.Types
    open EveOnline.IceDomain.Records
    open EveOnline.IceDomain.Ice
    open Microsoft.FSharp.Reflection
    
    let RawOreList :RawOre list = [
        for rawOre in FSharpType.GetUnionCases typeof<OreType> do     
            yield FSharpValue.MakeUnion(rawOre, [| box 1 |])
                  |> unbox |> OreFactory (Common) (IsNotCompressed)

    ]

    let CompressedRawOreList :RawOre list= [
        for rawOre in FSharpType.GetUnionCases typeof<OreType> do     
            yield FSharpValue.MakeUnion(rawOre, [| box 1 |])
                  |> unbox |> OreFactory (Common) (IsCompressed)
    ]

    let RawIceList :RawIce list = [
        for rawIce in FSharpType.GetUnionCases typeof<IceType> do
            yield FSharpValue.MakeUnion(rawIce, [| box 1 |])
                  |> unbox |> IceFactory (IsNotCompressed)
    ]

    let CompressedRawIceList :RawIce list = [
        for rawIce in FSharpType.GetUnionCases typeof<IceType> do   
            yield FSharpValue.MakeUnion(rawIce, [| box 1 |])
                  |> unbox |> IceFactory (IsNotCompressed)
    ]


    let AllOrePairs = List.zip RawOreList CompressedRawOreList
    let AllOreList = RawOreList @ CompressedRawOreList

    let AllIcePairs = List.zip RawIceList CompressedRawIceList
    let AllIceList  = RawIceList @ CompressedRawIceList

    // let AllIDPairs = MineralIDPairs @ IceProductIDPairs @ RawIceIDPairs @ RawOreIDPairs




