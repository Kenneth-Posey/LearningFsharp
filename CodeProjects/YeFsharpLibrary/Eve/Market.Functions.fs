namespace Market

module Functions = 
    open EveData
    open EveData.RawMaterials
    open EveData.Ore.Types
    open EveData.Ice.Types
    open Market.Parser

    let LoadMineralPrices location quantity = 
        let LoadItem item = LoadMarketSnapshot (string location) quantity (string item)

        [
            int Collections.MineralIDs.Isogen
            int Collections.MineralIDs.Megacyte
            int Collections.MineralIDs.Mexallon
            int Collections.MineralIDs.Morphite
            int Collections.MineralIDs.Nocxium
            int Collections.MineralIDs.Pyerite
            int Collections.MineralIDs.Tritanium
            int Collections.MineralIDs.Zydrine
        ]
        |> List.map (fun x -> x, LoadItem x)


    let LoadMineralJitaSell () = 
        let jita = int EveData.Collections.SystemName.Jita
        let priceMap = LoadMineralPrices jita 1
                       |> List.map (fun x -> fst x, (snd x).lowSell)
        let loadPrice id = snd (List.find (fun x -> (int(fst x)) = int (id)) priceMap)

        {
            Tritanium = loadPrice Collections.MineralIDs.Tritanium
            Pyerite   = loadPrice Collections.MineralIDs.Pyerite 
            Mexallon  = loadPrice Collections.MineralIDs.Mexallon
            Isogen    = loadPrice Collections.MineralIDs.Isogen  
            Nocxium   = loadPrice Collections.MineralIDs.Nocxium 
            Megacyte  = loadPrice Collections.MineralIDs.Megacyte
            Zydrine   = loadPrice Collections.MineralIDs.Zydrine 
            Morphite  = loadPrice Collections.MineralIDs.Morphite
        }


    let LoadIceProductPrices location quantity = 
        let LoadItem item = LoadMarketSnapshot (string location) quantity (string item)

        [
            int Collections.IceProductIDs.HeavyWater
            int Collections.IceProductIDs.HeliumIsotopes
            int Collections.IceProductIDs.HydrogenIsotopes
            int Collections.IceProductIDs.LiquidOzone
            int Collections.IceProductIDs.NitrogenIsotopes
            int Collections.IceProductIDs.OxygenIsotopes
            int Collections.IceProductIDs.StrontiumClathrates
        ]
        |> List.map (fun x -> x, LoadItem x)

        
    let LoadIceProductJita action = 
        let priceMap = LoadIceProductPrices (int Collections.SystemName.Jita) 1
                       |> List.map action
        let loadPrice id = snd (List.find (fun x -> (fst x) = int (id)) priceMap)

        {
            HeavyWater          = loadPrice Collections.IceProductIDs.HeavyWater
            HeliumIsotopes      = loadPrice Collections.IceProductIDs.HeliumIsotopes
            HydrogenIsotopes    = loadPrice Collections.IceProductIDs.HydrogenIsotopes
            LiquidOzone         = loadPrice Collections.IceProductIDs.LiquidOzone
            NitrogenIsotopes    = loadPrice Collections.IceProductIDs.NitrogenIsotopes
            OxygenIsotopes      = loadPrice Collections.IceProductIDs.OxygenIsotopes
            StrontiumClathrates = loadPrice Collections.IceProductIDs.StrontiumClathrates
        }

    let LoadIceProductJitaSell () = LoadIceProductJita (fun x -> fst x, (snd x).lowSell)
    let LoadIceProductJitaBuy  () = LoadIceProductJita (fun x -> fst x, (snd x).highBuy)

    let TransformOreForParser (oreList:List<IRawOre>) (mineralPrices:OreValue) = 
        let LoadValue multiplier ore = Ore.Functions.RefineValueOre multiplier ore mineralPrices
        [
            for ore in oreList do
                let multiplier = match ore.IsTiny () with 
                                 | true  ->  100.0f
                                 | false ->  1.0f  
                yield {
                    name  = ore.GetName ()
                    price = LoadValue multiplier ore
                    id    = ore.GetBase ()
                }
                yield {
                    name  = ore.GetName5 ()
                    price = LoadValue (multiplier * 1.05f) ore
                    id    = ore.GetBase5 ()
                }
                yield {
                    name  = ore.GetName10 ()
                    price = LoadValue (multiplier * 1.10f) ore
                    id    = ore.GetBase10 ()
                }
        ]


    let TransformIceForParser (iceList:List<IRawIce>) (iceProductPrices:IceValue) = 
        let LoadValue ice = Ice.Functions.RefineValueIce 1.0f ice iceProductPrices
        [
            for ice in iceList do
                let multiplier = 1.0f
                yield {
                    name  = ice.GetName ()
                    price = LoadValue ice
                    id    = ice.GetBase ()
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
            SumRec items 0.0

        rawList 
        |> List.map (fun x -> x.Split [|'\t'|])
        |> List.map (fun x -> string(x.[0]), int(x.[1]))
        |> SumItems 

    
    let GetCodes (item:IOre * IOre) = 
        string ( (fst item).GetBase () ) , 
        string ( (snd item).GetBase () )


    let FastProfit (item:IOre * IOre) (location:int) = 
        let raw, comp  = GetCodes item
        let location = string location
        let buy100kRaw = LoadBuyData  raw  location 100000
        let sell1kComp = LoadSellData comp location 1000

        sell1kComp - buy100kRaw


    let BestProfit (item: IOre * IOre) (location:int) =
        let raw, comp = GetCodes item
        let location = string location
        let rawSnapshot = LoadMarketSnapshot location 100000 raw
        let buy100kRaw  = rawSnapshot.highBuy

        let compSnapshot = LoadMarketSnapshot location 1000 comp
        let sell1kComp   = compSnapshot.lowSell

        sell1kComp - buy100kRaw