namespace EveOnline.MarketDomain

module Market = 
    open EveOnline.ProductDomain.Types
    open EveOnline.ProductDomain.UnionTypes
    open EveOnline.ProductDomain.Records
    open EveOnline.ProductDomain.Product
    open EveOnline.OreDomain.Types
    open EveOnline.OreDomain.Records
    open EveOnline.OreDomain.Ore
    open EveOnline.IceDomain.Types
    open EveOnline.IceDomain.Records
    open EveOnline.IceDomain.Ice
    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Records
    open EveOnline.MarketDomain.Parser
    open EveOnline.DataDomain.Collections

    // functions for shifting material type functions into their subtype functions

    // functions for finding the typeid of a material type
    let IceTypeId x = IceTypeId (x) (IsNotCompressed)
    let OreTypeId x = (OreData (x) (Common) (IsNotCompressed)).OreId

    let TypeId x = 
        match x with
        | Mineral x     -> MineralTypeid x
        | IceProduct x  -> IceProductTypeid x
        | IceType x     -> IceTypeId x
        | OreType x     -> OreTypeId x

    let StringId x = 
        string (TypeId x).Value

    // functions for finding the name of a material type
    let IceName x = RawIceName (x)
    let OreName x = RawOreName (x)

    let Name x = 
        match x with
        | Mineral x    -> MineralName x
        | IceProduct x -> IceProductName x
        | IceType x -> IceName x
        | OreType x -> OreName x

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
        |> parse (TypeId item).Value

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


    // loads ice product prices based on the highest buy offer or lowest sell offer in system
    let loadIceProductPrices (orderType:OrderType) (loc:System) :IceProductPrices = 
        let loadItem (item:Material) = Price <|
            match orderType with
            | BuyOrder  -> (loadItem loc item).prices.highBuy
            | SellOrder -> (loadItem loc item).prices.lowSell

        {
            HeavyWater          = loadItem <| IceProduct HeavyWater 
            HeliumIsotopes      = loadItem <| IceProduct HeliumIsotopes
            HydrogenIsotopes    = loadItem <| IceProduct HydrogenIsotopes
            LiquidOzone         = loadItem <| IceProduct LiquidOzone
            NitrogenIsotopes    = loadItem <| IceProduct NitrogenIsotopes
            OxygenIsotopes      = loadItem <| IceProduct OxygenIsotopes
            StrontiumClathrates = loadItem <| IceProduct StrontiumClathrates
        }

    // loads mineral prices based on the highest buy offer or lowest sell offer in system
    let loadMineralPrices (orderType:OrderType) (loc:System) :MineralPrices =
        let loadItem (item:Material) = Price <|
            match orderType with
            | BuyOrder  -> (loadItem loc item).prices.highBuy
            | SellOrder -> (loadItem loc item).prices.lowSell

        {
            Tritanium = loadItem <| Mineral Tritanium
            Pyerite   = loadItem <| Mineral Pyerite
            Mexallon  = loadItem <| Mineral Mexallon
            Isogen    = loadItem <| Mineral Isogen
            Nocxium   = loadItem <| Mineral Nocxium
            Megacyte  = loadItem <| Mineral Megacyte
            Zydrine   = loadItem <| Mineral Zydrine
            Morphite  = loadItem <| Mineral Morphite
        }
        
    
    type RefinePrice = 
    | MineralPrices of MineralPrices
    | IceProductPrices of IceProductPrices


    // functions for finding the yield of a refinable type
    type RefineYield = 
    | IceYield of IceYield 
    | OreYield of OreYield

    let GetYield x :RefineYield = 
        match x with
        | IceType x -> IceYield <| RawIceYield x
        | OreType x -> OreYield <| RawOreYield x
        | IceProduct _ -> IceYield <| BaseIceYield
        | Mineral _ -> OreYield <| BaseOreYield
    
    type RefinedProduct = 
    | Mineral
    | IceProduct

    let GetPrice material order loc :RefinePrice = 
        match material with
        | Mineral     -> MineralPrices    <| loadMineralPrices order loc
        | IceProduct  -> IceProductPrices <| loadIceProductPrices order loc
        
    let refineValueProcessor (pairs:(int *single) list) :Price =
        let accumulator = (fun total (refine, price) -> total + (single refine * price))
        pairs |> List.fold accumulator (0.0f) |> Price
    
    // calculates the maximum market value of the yield of a single ice block
    let refineIceValue (refine:IceYield) (price:RefinePrice) :Price =          
        let price = price |> fun x -> match x with
                     | IceProductPrices x -> x
                     | _ -> BaseIceProductPrices
        [
            refine.HeliumIsotopes.Value,    price.HeliumIsotopes.Value
            refine.HydrogenIsotopes.Value,  price.HydrogenIsotopes.Value
            refine.NitrogenIsotopes.Value,  price.NitrogenIsotopes.Value
            refine.OxygenIsotopes.Value,    price.OxygenIsotopes.Value
            
            refine.HeavyWater.Value,        price.HeavyWater.Value
            refine.LiquidOzone.Value,       price.LiquidOzone.Value
            refine.StrontiumClathrates.Value,   price.StrontiumClathrates.Value
        ]
        |> refineValueProcessor
        
    let refineOreValue (refine:OreYield) (price:RefinePrice) :Price =
        let price = price |> fun x -> match x with
                     | MineralPrices x -> x
                     | _ -> BaseMineralPrices
        [
            refine.Isogen.Value,      price.Isogen.Value
            refine.Megacyte.Value,    price.Megacyte.Value
            refine.Mexallon.Value,    price.Mexallon.Value
            refine.Morphite.Value,    price.Morphite.Value
            refine.Nocxium.Value,     price.Nocxium.Value
            refine.Pyerite.Value,     price.Pyerite.Value
            refine.Tritanium.Value,   price.Tritanium.Value
            refine.Zydrine.Value,     price.Zydrine.Value
        ]
        |> refineValueProcessor
    
    open EveOnline.ProductDomain.Records
    let refineValue (refine:RefineYield) (price:RefinePrice) :Price =
        match refine with
        | OreYield x -> refineOreValue x price
        | IceYield x -> refineIceValue x price

        
    // calculates the maximum market value of the yield of a single ice block
    let refineIceValuev1 (refine:IceYield) (prices:IceProductPrices) :Price = 
        single refine.HeavyWater.Value * prices.HeavyWater.Value
        + single refine.HeliumIsotopes.Value * prices.HeliumIsotopes.Value
        + single refine.HydrogenIsotopes.Value * prices.HydrogenIsotopes.Value
        + single refine.LiquidOzone.Value * prices.LiquidOzone.Value
        + single refine.NitrogenIsotopes.Value * prices.NitrogenIsotopes.Value
        + single refine.OxygenIsotopes.Value * prices.OxygenIsotopes.Value
        + single refine.StrontiumClathrates.Value * prices.StrontiumClathrates.Value
        |> Price

    // calculates the maximum market value of the yield of a single refining run (100 units)
    let refineOreValuev1 (refine:OreYield) (prices:MineralPrices) :Price =
        single refine.Isogen.Value * prices.Isogen.Value
        + single refine.Megacyte.Value * prices.Megacyte.Value
        + single refine.Mexallon.Value * prices.Mexallon.Value
        + single refine.Morphite.Value * prices.Morphite.Value
        + single refine.Nocxium.Value * prices.Nocxium.Value
        + single refine.Pyerite.Value * prices.Pyerite.Value
        + single refine.Tritanium.Value * prices.Tritanium.Value
        + single refine.Zydrine.Value * prices.Zydrine.Value
        |> Price
