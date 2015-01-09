namespace EveOnline

module MarketRecords =  
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

    let AllIDPairs = MineralIDPairs @ IceProductIDPairs @ RawIceIDPairs @ RawOreIDPairs


    module Dynamic = 
        open Ore.Types
        open Ice.Types
        open Ore.Functions
        open Ice.Functions
        let LoadBaseOreValues (value:OreValue) = 
            [
                for ore in RawOreList do
                    yield   ore.Name.Value
                          , ore.OreId.Value
                          , RefineValueOre ore value
            ]


        let LoadBaseIceValues (value:IceValue) = 
            [
                for ice in RawIceList do
                    yield   ice.Name.Value
                          , ice.IceId.Value
                          , RefineValueIce ice value
            ]