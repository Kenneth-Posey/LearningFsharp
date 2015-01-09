namespace EveOnline.DataDomain

module Collections = 
    open EveOnline.ProductDomain.Types
    open EveOnline.ProductDomain.Records
    open EveOnline.ProductDomain.Product
    open EveOnline.OreDomain.Types
    open EveOnline.OreDomain.Records
    open EveOnline.OreDomain.Ore
    open EveOnline.IceDomain.Types
    open EveOnline.IceDomain.Records
    open EveOnline.IceDomain.Ice
    open Microsoft.FSharp.Reflection

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

    let RefinedProducts = 
        [
            Mineral Mineral.Isogen
            Mineral Mineral.Megacyte 
            Mineral Mineral.Mexallon 
            Mineral Mineral.Morphite 
            Mineral Mineral.Nocxium  
            Mineral Mineral.Pyerite  
            Mineral Mineral.Tritanium
            Mineral Mineral.Zydrine  

            IceProduct IceProduct.HeavyWater
            IceProduct IceProduct.HeliumIsotopes
            IceProduct IceProduct.HydrogenIsotopes
            IceProduct IceProduct.LiquidOzone
            IceProduct IceProduct.NitrogenIsotopes
            IceProduct IceProduct.OxygenIsotopes
            IceProduct IceProduct.StrontiumClathrates
        ]

    let IceList = 
        [   
            BlueIce             
            ClearIcicle         
            DarkGlitter         
            EnrichedClearIcicle 
            Gelidus             
            GlacialMass         
            GlareCrust          
            Krystallos          
            PristineWhiteGlaze  
            SmoothGlacialMass   
            ThickBlueIce        
            WhiteGlaze          
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
