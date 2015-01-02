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
    
//
    let RawOreList :RawOre list = [
        for rawOre in FSharpType.GetUnionCases typeof<OreType> do     
            let ore = FSharpValue.MakeUnion(rawOre, [|1|]) :?> OreType
            yield OreFactory (Common) (IsNotCompressed) (ore)

    ]

    let CompressedRawOreList :RawOre list= [
        for rawOre in FSharpType.GetUnionCases typeof<OreType> do     
            let ore = FSharpValue.MakeUnion(rawOre, [|1|]) :?> OreType
            yield OreFactory (Common) (IsCompressed) (ore)
    ]

    let RawIceList :RawIce list = [
        for rawIce in FSharpType.GetUnionCases typeof<IceType> do
            let ice = FSharpValue.MakeUnion(rawIce, [|1|]) :?> IceType
            yield IceFactory (IsNotCompressed) (ice)
    ]

    let CompressedRawIceList :RawIce list = [
        for rawIce in FSharpType.GetUnionCases typeof<IceType> do   
            let ice = FSharpValue.MakeUnion(rawIce, [|1|]) :?> IceType
            yield IceFactory (IsCompressed) (ice)
    ]


    let AllOrePairs = List.zip RawOreList CompressedRawOreList
    let AllOreList = RawOreList @ CompressedRawOreList

    let AllIcePairs = List.zip RawIceList CompressedRawIceList
    let AllIceList  = RawIceList @ CompressedRawIceList

    // let AllIDPairs = MineralIDPairs @ IceProductIDPairs @ RawIceIDPairs @ RawOreIDPairs




