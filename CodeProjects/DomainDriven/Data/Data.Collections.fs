namespace EveOnline.DataDomain

module Collections = 
    open EveOnline.ProductDomain.UnionTypes
    open EveOnline.OreDomain.Types
    open EveOnline.IceDomain.Types

    type SystemName =
    | Jita    = 30000142
    | Dodixie = 30002659
    | Amarr   = 30002187
    | Hek     = 30002053
    | Rens    = 30002510
    
    type Material = 
    | Mineral    of Mineral
    | IceProduct of IceProduct
    | OreType    of OreType
    | IceType    of IceType

    type OrderType = 
    | BuyOrder
    | SellOrder

    let MineralList = 
        [
            Mineral Isogen
            Mineral Megacyte 
            Mineral Mexallon 
            Mineral Morphite 
            Mineral Nocxium  
            Mineral Pyerite  
            Mineral Tritanium
            Mineral Zydrine  
        ]

    let IceProductList = 
        [
            IceProduct HeavyWater
            IceProduct HeliumIsotopes
            IceProduct HydrogenIsotopes
            IceProduct LiquidOzone
            IceProduct NitrogenIsotopes
            IceProduct OxygenIsotopes
            IceProduct StrontiumClathrates
        ]

    let RefinedProducts = 
        MineralList @ IceProductList

    let IceList = 
        [   
            IceType BlueIce             
            IceType ClearIcicle         
            IceType DarkGlitter         
            IceType EnrichedClearIcicle 
            IceType Gelidus             
            IceType GlacialMass         
            IceType GlareCrust          
            IceType Krystallos          
            IceType PristineWhiteGlaze  
            IceType SmoothGlacialMass   
            IceType ThickBlueIce        
            IceType WhiteGlaze          
        ]

    let OreList = 
        [
            OreType Arkonor    
            OreType Bistot     
            OreType Crokite    
            OreType DarkOchre  
            OreType Gneiss     
            OreType Hedbergite 
            OreType Hemorphite 
            OreType Jaspet     
            OreType Kernite    
            OreType Mercoxit   
            OreType Omber      
            OreType Plagioclase
            OreType Pyroxeres  
            OreType Scordite   
            OreType Spodumain  
            OreType Veldspar   
        ]
    
//    let RawOreList :RawOre list = [
//        for rawOre in FSharpType.GetUnionCases typeof<OreType> do     
//            yield FSharpValue.MakeUnion(rawOre, [| box 1 |])
//                  |> unbox |> OreFactory (Common) (IsNotCompressed) (Qty 1)
//    ]
//
//    let CompressedRawOreList :RawOre list= [
//        for rawOre in FSharpType.GetUnionCases typeof<OreType> do     
//            yield FSharpValue.MakeUnion(rawOre, [| box 1 |])
//                  |> unbox |> OreFactory (Common) (IsCompressed) (Qty 1)
//    ]
//
//    let RawIceList :RawIce list = [
//        for rawIce in FSharpType.GetUnionCases typeof<IceType> do
//            yield FSharpValue.MakeUnion(rawIce, [| box 1 |])
//                  |> unbox |> IceFactory (IsNotCompressed) (Qty 1)
//    ]
//
//    let CompressedRawIceList :RawIce list = [
//        for rawIce in FSharpType.GetUnionCases typeof<IceType> do   
//            yield FSharpValue.MakeUnion(rawIce, [| box 1 |])
//                  |> unbox |> IceFactory (IsNotCompressed) (Qty 1)
//    ]
//
//
//    let AllOrePairs = List.zip RawOreList CompressedRawOreList
//    let AllOreList = RawOreList @ CompressedRawOreList
//
//    let AllIcePairs = List.zip RawIceList CompressedRawIceList
//    let AllIceList  = RawIceList @ CompressedRawIceList

    // let AllIDPairs = MineralIDPairs @ IceProductIDPairs @ RawIceIDPairs @ RawOreIDPairs
