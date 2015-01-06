namespace EveOnline.MarketDomain

module Market = 
    open EveOnline.ProductDomain.Types
    open EveOnline.ProductDomain.Product
    open EveOnline.OreDomain.Types
    open EveOnline.OreDomain.Ore
    open EveOnline.IceDomain.Ice
    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Records
    open EveOnline.MarketDomain.Parser
    

    let MineralTypeid x = 
        match x with
        | Mineral.Tritanium -> 34
        | Mineral.Pyerite   -> 35
        | Mineral.Mexallon  -> 36
        | Mineral.Isogen    -> 37
        | Mineral.Nocxium   -> 38
        | Mineral.Zydrine   -> 39
        | Mineral.Megacyte  -> 40
        | Mineral.Morphite  -> 11399

    let IceProductTypeid x = 
        match x with
        | IceProduct.HeavyWater           -> 16272
        | IceProduct.HeliumIsotopes       -> 16274
        | IceProduct.HydrogenIsotopes     -> 17889
        | IceProduct.LiquidOzone          -> 16273
        | IceProduct.NitrogenIsotopes     -> 17888
        | IceProduct.OxygenIsotopes       -> 17887
        | IceProduct.StrontiumClathrates  -> 16275

    let IceTypeId x = (IceData (x) (IsNotCompressed)).IceId.Value
    let OreTypeId x = (OreData (x) (Common) (IsNotCompressed)).OreId.Value

    let TypeId x = 
        match x with
        | Mineral x     -> MineralTypeid x
        | IceProduct x  -> IceProductTypeid x
        | IceType x     -> IceTypeId x
        | OreType x     -> OreTypeId x


    let loadUrl = Utility.UtilityFunctions.LoadUrl
    let composeUrl = Utility.UtilityFunctions.ComposeUrl
    let parse = EveOnline.MarketDomain.Parser.ParseQuickLook
    let baseUrl = (fun loc (item:int) -> 
                      EveOnline.MarketDomain.Providers.QuickLook + 
                      "?typeid=" + (string item) + "&usesystem=" + 
                      string (SystemId loc) )

    let loadItem (item:int) (location:System) = 
        item
        |> baseUrl location
        |> loadUrl
        |> parse item

    let loadItems (loc:System) (items:Material list) = 
        items
        |> List.map (fun x -> loadItem (TypeId x) loc)

    let calcbuy amount data =
        data.sellOrders
        |> List.sortWith SortSellFunc
        |> OrderProcessor amount

    let calcSell amount data = 
        data.buyOrders
        |> List.sortWith SortBuyFunc
        |> OrderProcessor amount
