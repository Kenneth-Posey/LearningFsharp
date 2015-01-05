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

    
        
    type MarketPrices =  {
        typeId   : int
        highBuy  : single
        lowSell  : single
        lowBuy   : single
        highSell : single
    }
            
    type ParsedData<'a, 'b> = {
        buyOrders  : 'a
        sellOrders : 'b
        prices     : MarketPrices
    }


    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Providers

    type QuickLookProvider = MarketOrder.QuickLookResult
    type SellOrderProvider = MarketOrder.QuickLookResult.Order
    type BuyOrderProvider  = MarketOrder.QuickLookResult.Order2

    type BuyOrder () =
        member val OrderType = "Buy" with get
        member this.GetType () = typeof<BuyOrderProvider>

    type SellOrder () =
        member val OrderType = "Sell" with get
        member this.GetType () = typeof<SellOrderProvider>

    type Order (region, station, stationName, security, range, price, volRemain, minVolume, orderType) as this =
        member val Region      = region
        member val Station     = station    
        member val StationName = stationName
        member val Security    = security   
        member val Range       = range      
        member val Price       = single price      
        member val VolRemain   = volRemain  
        member val MinVolume   = minVolume  
        member val OrderType   = orderType

        // F# requires that we use different constructor signatures
        // which is why we use a mostly empty class to force the
        // signatures to be different
        new (order:BuyOrderProvider, typeDef:BuyOrder) = 
            Order (   region      = order.Region
                    , station     = order.Station
                    , stationName = order.StationName
                    , security    = order.Security
                    , range       = order.Range
                    , price       = order.Price
                    , volRemain   = order.VolRemain
                    , minVolume   = order.MinVolume
                    , orderType   = typeDef.OrderType   )

        new (order:SellOrderProvider, typeDef:SellOrder) = 
            Order (   region      = order.Region
                    , station     = order.Station
                    , stationName = order.StationName
                    , security    = order.Security
                    , range       = order.Range
                    , price       = order.Price
                    , volRemain   = order.VolRemain
                    , minVolume   = order.MinVolume 
                    , orderType   = typeDef.OrderType   )