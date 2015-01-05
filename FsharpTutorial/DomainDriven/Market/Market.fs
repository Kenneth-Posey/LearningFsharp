namespace EveOnline.MarketDomain

module Market = 
    open EveOnline.OreDomain.Records
    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Records
    open EveOnline.MarketDomain.Parser
    




















































    // The actual prices for the minerals
    type OreValue = {
        Tritanium   : single
        Pyerite     : single
        Mexallon    : single
        Isogen      : single
        Nocxium     : single
        Megacyte    : single
        Zydrine     : single
        Morphite    : single
    }

    // The actual prices for the ice products
    type IceValue = {
        HeavyWater          : single
        HeliumIsotopes      : single
        HydrogenIsotopes    : single
        LiquidOzone         : single
        NitrogenIsotopes    : single
        OxygenIsotopes      : single
        StrontiumClathrates : single           
    }

    let LoadMineralPrices location quantity = 
        let LoadItem item = LoadMarketSnapshot (string location) quantity (string item)

        [
            int MineralIDs.Isogen
            int MineralIDs.Megacyte
            int MineralIDs.Mexallon
            int MineralIDs.Morphite
            int MineralIDs.Nocxium
            int MineralIDs.Pyerite
            int MineralIDs.Tritanium
            int MineralIDs.Zydrine
        ]
        |> List.map (fun x -> x, LoadItem x)


    let LoadMineralJitaSell () :OreValue = 
        let jita = int SystemName.Jita
        let priceMap = LoadMineralPrices jita 1
                       |> List.map (fun x -> fst x, (snd x).lowSell)
        let loadPrice id = snd (List.find (fun x -> (int(fst x)) = int (id)) priceMap)

        {
            Tritanium = loadPrice MineralIDs.Tritanium
            Pyerite   = loadPrice MineralIDs.Pyerite 
            Mexallon  = loadPrice MineralIDs.Mexallon
            Isogen    = loadPrice MineralIDs.Isogen  
            Nocxium   = loadPrice MineralIDs.Nocxium 
            Megacyte  = loadPrice MineralIDs.Megacyte
            Zydrine   = loadPrice MineralIDs.Zydrine 
            Morphite  = loadPrice MineralIDs.Morphite
        }


    let LoadIceProductPrices location quantity = 
        let LoadItem item = LoadMarketSnapshot (string location) quantity (string item)

        [
            int IceProductIDs.HeavyWater
            int IceProductIDs.HeliumIsotopes
            int IceProductIDs.HydrogenIsotopes
            int IceProductIDs.LiquidOzone
            int IceProductIDs.NitrogenIsotopes
            int IceProductIDs.OxygenIsotopes
            int IceProductIDs.StrontiumClathrates
        ]
        |> List.map (fun x -> x, LoadItem x)

        
    let LoadIceProductJita action = 
        let priceMap = LoadIceProductPrices (int SystemName.Jita) 1
                       |> List.map action
        let loadPrice id = snd (List.find (fun x -> (fst x) = int (id)) priceMap)

        {
            HeavyWater          = loadPrice IceProductIDs.HeavyWater
            HeliumIsotopes      = loadPrice IceProductIDs.HeliumIsotopes
            HydrogenIsotopes    = loadPrice IceProductIDs.HydrogenIsotopes
            LiquidOzone         = loadPrice IceProductIDs.LiquidOzone
            NitrogenIsotopes    = loadPrice IceProductIDs.NitrogenIsotopes
            OxygenIsotopes      = loadPrice IceProductIDs.OxygenIsotopes
            StrontiumClathrates = loadPrice IceProductIDs.StrontiumClathrates
        }

    let LoadIceProductJitaSell () = LoadIceProductJita (fun x -> fst x, (snd x).lowSell)
    let LoadIceProductJitaBuy  () = LoadIceProductJita (fun x -> fst x, (snd x).highBuy)

    let TransformOreForParser (oreList:List<RawOre>) (mineralPrices:OreValue) = 
        let LoadValue ore = Ore.RefineValueOre ore mineralPrices
        [
            for ore in oreList do
                yield {
                    name  = ore.Name.Value
                    price = LoadValue ore
                    id    = ore.OreId.Value
                }
        ]


    let TransformIceForParser (iceList:List<RawIce>) (iceProductPrices:IceValue) = 
        let LoadValue ice = Ice.Functions.RefineValueIce ice iceProductPrices
        [
            for ice in iceList do
                yield {
                    name  = ice.Name.Value
                    price = LoadValue ice
                    id    = ice.IceId.Value
                }
        ]


    let TransformIceProductsForParser (idPairs:List<(string*int)>) (iceProductPrices:IceValue) = 
        [
            for product in idPairs do
                let contains (name:string) = List.exists (fun x -> x = name.Trim()) Collections.IceProductNames
                if contains (fst product) then
                    let price = match (fst product) with 
                    | "Heavy Water"          -> iceProductPrices.HeavyWater
                    | "Helium Isotopes"      -> iceProductPrices.HeliumIsotopes
                    | "Hydrogen Isotopes"    -> iceProductPrices.HydrogenIsotopes
                    | "Liquid Ozone"         -> iceProductPrices.LiquidOzone
                    | "Nitrogen Isotopes"    -> iceProductPrices.NitrogenIsotopes
                    | "Oxygen Isotopes"      -> iceProductPrices.OxygenIsotopes
                    | "Strontium Clathrates" -> iceProductPrices.StrontiumClathrates

                    yield {
                        name  = fst product
                        price = price
                        id    = snd product
                    }
        ]


    let TransformMineralsForParser (idPairs:List<(string*int)>) (mineralPrices:OreValue) = 
        [
            for product in idPairs do
                let contains (name:string) = List.exists (fun x -> x = name.Trim()) Collections.MineralNames
                if contains (fst product) then
                    let price = match (fst product) with 
                    | "Tritanium" -> mineralPrices.Tritanium
                    | "Pyerite"   -> mineralPrices.Pyerite
                    | "Mexallon"  -> mineralPrices.Mexallon
                    | "Isogen"    -> mineralPrices.Isogen
                    | "Nocxium"   -> mineralPrices.Nocxium
                    | "Zydrine"   -> mineralPrices.Zydrine
                    | "Megacyte"  -> mineralPrices.Megacyte
                    | "Morphite"  -> mineralPrices.Morphite

                    yield {
                        name  = fst product
                        price = price
                        id    = snd product
                    }
        ]
    

    let LoadAllItemsForParser () = 
        let allOre = Collections.AllOreList
        let allIce = Collections.AllIceList
        let allMinerals = Collections.MineralIDPairs
        let allIceProducts = Collections.IceProductIDPairs

        let mineralPrices = LoadMineralJitaSell ()
        let iceProductPrices = LoadIceProductJitaSell ()

        let oreValues = TransformOreForParser allOre mineralPrices
        let iceValues = TransformIceForParser allIce iceProductPrices
        let mineralValues = TransformMineralsForParser allMinerals mineralPrices
        let iceProductValues = TransformIceProductsForParser allIceProducts iceProductPrices

        oreValues @ iceValues @ mineralValues @ iceProductValues


    let CalculateEstimate (rawList:List<string>) = 
        let SumItems (items:List<(string*int)>) = 
            let allItems = LoadAllItemsForParser ()
            let rec SumRec (items:List<(string*int)>) (total:double) = 
                match items.Length with 
                | length when length > 0 -> 
                    let parsedItem = List.find (fun item -> fst (items.Head) = item.name) allItems
                    let total = (total + double (parsedItem.price) * double (snd items.Head))
                    SumRec items.Tail total
                | length when length = 0 -> 
                    total
                | _ -> total 
            SumRec items 0.0

        rawList 
        |> List.map (fun x -> x.Split [|'\t'|])
        |> List.map (fun x -> string(x.[0]), int(x.[1]))
        |> SumItems 

    
    let GetCodes (item:RawOre * RawOre) = 
        string ( (fst item).OreId.Value ) , 
        string ( (snd item).OreId.Value )


    let FastProfit (item:RawOre * RawOre) (location:int) = 
        let raw, comp  = GetCodes item
        let location = string location
        let buy100kRaw = LoadBuyData  raw  location 100000
        let sell1kComp = LoadSellData comp location 1000

        sell1kComp - buy100kRaw


    let BestProfit (item: RawOre * RawOre) (location:int) =
        let raw, comp = GetCodes item
        let location = string location
        let rawSnapshot = LoadMarketSnapshot location 100000 raw
        let buy100kRaw  = rawSnapshot.highBuy

        let compSnapshot = LoadMarketSnapshot location 1000 comp
        let sell1kComp   = compSnapshot.lowSell

        sell1kComp - buy100kRaw

