namespace EveOnline.MarketDomain

module Market = 
    open EveOnline.ProductDomain.Types
    open EveOnline.ProductDomain.Records
    open EveOnline.ProductDomain.Product
    open EveOnline.OreDomain.Types
    open EveOnline.OreDomain.Ore
    open EveOnline.IceDomain.Ice
    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Records
    open EveOnline.MarketDomain.Parser

    // functions for finding the typeid of a material type
    let IceTypeId x = (IceData (x) (IsNotCompressed)).IceId.Value
    let OreTypeId x = (OreData (x) (Common) (IsNotCompressed)).OreId.Value

    let TypeId x = 
        match x with
        | Mineral x     -> MineralTypeid x
        | IceProduct x  -> IceProductTypeid x
        | IceType x     -> IceTypeId x
        | OreType x     -> OreTypeId x

    let StringId x = 
        string (TypeId x)

    // functions for composing the parser urls
    let loadUrl = Utility.UtilityFunctions.LoadUrl
    let composeUrl = Utility.UtilityFunctions.ComposeUrl
    let parse = EveOnline.MarketDomain.Parser.ParseQuickLook
    let baseUrl = (fun loc item -> 
                      EveOnline.MarketDomain.Providers.QuickLook + 
                      "?typeid=" + StringId item + "&usesystem=" + 
                      string (SystemId loc) )

    // load a material's data
    let loadItem (location:System) (item:Material)= 
        item 
        |> baseUrl location
        |> loadUrl
        |> parse (TypeId item)

    // load a list of material's data
    let loadItems (loc:System) (items:Material list) = 
        items
        |> List.map (fun x -> loadItem loc x)

    // calculates the cost of buying X item based on available orders
    let calcbuy amount data =
        data.sellOrders
        |> List.sortWith SortSellFunc
        |> OrderProcessor amount

    // calculates the income of selling X item based on available orders
    let calcSell amount data = 
        data.buyOrders
        |> List.sortWith SortBuyFunc
        |> OrderProcessor amount


    let loadIceProductPrices (orderType:OrderType) (loc:System) :IceProductPrices = 
        let loadItem (item:Material) = Price <|
            match orderType with
            | BuyOrder  -> (loadItem loc item).prices.highBuy
            | SellOrder -> (loadItem loc item).prices.lowSell

        {
            HeavyWater          = loadItem <| IceProduct IceProduct.HeavyWater 
            HeliumIsotopes      = loadItem <| IceProduct IceProduct.HeliumIsotopes
            HydrogenIsotopes    = loadItem <| IceProduct IceProduct.HydrogenIsotopes
            LiquidOzone         = loadItem <| IceProduct IceProduct.LiquidOzone
            NitrogenIsotopes    = loadItem <| IceProduct IceProduct.NitrogenIsotopes
            OxygenIsotopes      = loadItem <| IceProduct IceProduct.OxygenIsotopes
            StrontiumClathrates = loadItem <| IceProduct IceProduct.StrontiumClathrates
        }

    let loadMineralPrices (orderType:OrderType) (loc:System) :MineralPrices =
        let loadItem (item:Material) = Price <|
            match orderType with
            | BuyOrder  -> (loadItem loc item).prices.highBuy
            | SellOrder -> (loadItem loc item).prices.lowSell

        {
            Tritanium = loadItem <| Mineral Mineral.Tritanium
            Pyerite   = loadItem <| Mineral Mineral.Pyerite
            Mexallon  = loadItem <| Mineral Mineral.Mexallon
            Isogen    = loadItem <| Mineral Mineral.Isogen
            Nocxium   = loadItem <| Mineral Mineral.Nocxium
            Megacyte  = loadItem <| Mineral Mineral.Megacyte
            Zydrine   = loadItem <| Mineral Mineral.Zydrine
            Morphite  = loadItem <| Mineral Mineral.Morphite
        }