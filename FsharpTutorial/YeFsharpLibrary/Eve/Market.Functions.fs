namespace Market

module Functions = 
    open EveData
    open EveData.Ore.RawMaterials
    open EveData.Ice.RawMaterials
    open Market.Parser

    let LoadMineralPrices location = 
        let LoadItem item = LoadMarketSnapshot location 1 (string item)

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


    type mineralIDs = EveData.Collections.MineralIDs
    let LoadMineralJitaSell () = 
        let jita = string (int EveData.Collections.SystemName.Jita)
        let priceMap = LoadMineralPrices jita
                       |> List.map (fun x -> fst x, (snd x).lowSell)
        let loadPrice id = snd (List.find (fun x -> (int(fst x)) = int (id)) priceMap)

        {
            Tritanium = loadPrice mineralIDs.Tritanium
            Pyerite   = loadPrice mineralIDs.Pyerite 
            Mexallon  = loadPrice mineralIDs.Mexallon
            Isogen    = loadPrice mineralIDs.Isogen  
            Nocxium   = loadPrice mineralIDs.Nocxium 
            Megacyte  = loadPrice mineralIDs.Megacyte
            Zydrine   = loadPrice mineralIDs.Zydrine 
            Morphite  = loadPrice mineralIDs.Morphite
        }


    let LoadIceProductPrices location = 
        let LoadItem item = LoadMarketSnapshot location 1 (string item)

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

        
    type iceProductIDs = Collections.IceProductIDs
    let LoadIceProductJitaSell () = 
        let jita = string (int Collections.SystemName.Jita)
        let priceMap = LoadIceProductPrices jita
                       |> List.map (fun x -> fst x, (snd x).lowSell)
        let loadPrice id = snd (List.find (fun x -> (fst x) = int (id)) priceMap)

        {
            HeavyWater          = loadPrice iceProductIDs.HeavyWater
            HeliumIsotopes      = loadPrice iceProductIDs.HeliumIsotopes
            HydrogenIsotopes    = loadPrice iceProductIDs.HydrogenIsotopes
            LiquidOzone         = loadPrice iceProductIDs.LiquidOzone
            NitrogenIsotopes    = loadPrice iceProductIDs.NitrogenIsotopes
            OxygenIsotopes      = loadPrice iceProductIDs.OxygenIsotopes
            StrontiumClathrates = loadPrice iceProductIDs.StrontiumClathrates
        }

    
    let GetCodes (item:IOre * IOre) = 
        string ( (fst item).GetBase () ) , 
        string ( (snd item).GetBase () )


    let FastProfit (item:IOre * IOre) (location:string) = 
        let raw, comp  = GetCodes item

        let buy100kRaw = LoadBuyData  raw  location 100000
        let sell1kComp = LoadSellData comp location 1000

        sell1kComp - buy100kRaw


    let BestProfit (item: IOre * IOre) (location:string) =
        let raw, comp = GetCodes item

        let rawSnapshot = LoadMarketSnapshot location 100000 raw
        let buy100kRaw  = rawSnapshot.highBuy

        let compSnapshot = LoadMarketSnapshot location 1000 comp
        let sell1kComp   = compSnapshot.lowSell

        sell1kComp - buy100kRaw