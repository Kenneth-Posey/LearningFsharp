namespace EveOnline

module MarketParser =
    open EveData
    open EveData.RawMaterials
    open EveData.Types
    open Utility.UtilityFunctions
            
    /// For reading and parsing the text file with typeIDs
    // Example use:
    // let itemArray = MarketParser.LoadTypeIdsFromUrl EveData.TypeIdUrl
    // let tritItems  = itemArray 
    //                  |> FilterToOreOnly 
    //                  |> FilterByName "Tritanium"

    let ParseQuickLook (data:string) =
        let providerData = MarketOrder.QuickLookResult.Parse(data).Quicklook

        let buyOrders = providerData.BuyOrders.Orders
        let sellOrders = providerData.SellOrders.Orders

        let mutable totalBuy = 0
        let mutable totalSell = 0

        // To find the lowest sell price of the goods
        // you have to test each value and replace the lowest sell
        // each time the price is *lower*. Start high, work down.
        let mutable lowSell = System.Single.MaxValue

        // To find the highest buy price of the goods
        // you have to test each value and replace the highest sell
        // each time the price is *higher*. Start low, work up.
        let mutable highBuy = 0.0f

        for buy in buyOrders do
            totalBuy <- totalBuy + buy.VolRemain
            if highBuy < single buy.Price then
                highBuy <- single buy.Price

        for sell in sellOrders do
            totalSell <- totalSell + sell.VolRemain
            if lowSell > single sell.Price then
                lowSell <- single sell.Price

        // We've found the upper buy limit for the goods
        // so we want to know what the highest buy -20% is
        // for the lower buy limit
        let lowBuy = (highBuy * 0.8f)

        // We've found the lower sell limit for the goods
        // so we want to know what the lowest sell +20% is
        // for the upper sell limit
        let highSell = (lowSell * 1.2f)
        
        // The upper buy and lower sell is implicit to the list
        // so we just need to manually remove the out-of-bounds orders
        let boundedBuyOrders  = buyOrders  |> Array.filter ( fun x -> lowBuy   < single x.Price )
        let boundedSellOrders = sellOrders |> Array.filter ( fun x -> highSell > single x.Price )
        
        // Implicit record creation
        {
            buyOrders  = boundedBuyOrders
            sellOrders = boundedSellOrders
            prices = {
                lowSell    = lowSell
                highSell   = highSell
                lowBuy     = lowBuy
                highBuy    = highBuy
            }
        }
    

    let OrderProcessor (quantity:int) (orders:List<Order>) =
        let Iterate (quantity:int) (orders:List<Order>) =
            let rec IterateRec (total) (quantity:int) (orders:List<Order>) =
                match quantity <= 0 || orders.Length = 0 with
                | true  -> total
                | false -> let total = total + orders.Head.Price * single quantity
                           let quant = quantity - orders.Head.VolRemain
                           IterateRec total quant orders.Tail
                                               
            IterateRec 0.0f quantity orders
            
        match (quantity = 0) || (orders.Length = 0) with
        | true  -> 0.0f                                                 
        | false -> Iterate quantity orders


    let SortBuyFunc (x:Order) (y:Order) =
        match x.Price <> y.Price with
        | true when x.Price > y.Price -> 1
        | true when x.Price < y.Price -> -1
        | false -> 0
       

    let SortSellFunc (x:Order) (y:Order) =
        match x.Price <> y.Price with
        | true when x.Price > y.Price -> -1
        | true when x.Price < y.Price -> 1
        | false -> 0
        

    let LoadData item location = 
        (item, location)
        ||> (fun item loc -> EveData.QuickLook + "?typeid=" + item + "&usesystem=" + loc)
        |> LoadUrl 
        |> ParseQuickLook


    let LoadMarketSnapshot location amount item = 
        LoadData item location
        |> (fun x -> 
            {
                lowBuy   = x.prices.lowBuy   * single amount
                highBuy  = x.prices.highBuy  * single amount
                lowSell  = x.prices.lowSell  * single amount
                highSell = x.prices.highSell * single amount
            })


    let LoadMineralPrices location = 
        let LoadItem item = LoadMarketSnapshot location 1 (string item)

        [
            int Collections.MineralIDs.Isogen
            int Collections.MineralIDs.Megacyte
            int Collections.MineralIDs.Mexallon
            int Collections.MineralIDs.Nocxium
            int Collections.MineralIDs.Pyerite
            int Collections.MineralIDs.Tritanium
            int Collections.MineralIDs.Zydrine
        ]
        |> List.map (fun x -> x, LoadItem x)


    type mineralIDs = EveData.Collections.MineralIDs
    let LoadMineralJitaSell () = 
        let jita = string (int EveData.SystemName.Jita)
        let priceMap = LoadMineralPrices jita
                       |> List.map (fun x -> fst x, (snd x).lowSell)
        let loadPrice id = snd (List.find (fun x -> (fst x) = int (id)) priceMap)

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

        
    type iceProductIDs = EveData.Collections.IceProductIDs
    let LoadIceProductJitaSell () = 
        let jita = string (int EveData.SystemName.Jita)
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

        
    let LoadBuyData item location amount = 
        LoadData item location
        |> (fun x -> x.sellOrders)
        |> List.ofArray
        |> List.map (fun x -> new Order(x, new SellOrder()))
        |> List.sortWith SortSellFunc
        |> OrderProcessor amount


    let LoadSellData item location amount = 
        LoadData item location
        |> (fun x -> x.buyOrders)
        |> List.ofArray
        |> List.map (fun x -> new Order(x, new BuyOrder()))
        |> List.sortWith SortBuyFunc
        |> OrderProcessor amount

    
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

        
    let RefineValueOre (multiplier:single) (item:IRawOre) (value:OreValue) =
        let mineralYield = item.GetYield ()
        (   single mineralYield.Isogen    * value.Isogen 
          + single mineralYield.Megacyte  * value.Megacyte
          + single mineralYield.Mexallon  * value.Mexallon
          + single mineralYield.Morphite  * value.Morphite
          + single mineralYield.Nocxium   * value.Nocxium
          + single mineralYield.Pyerite   * value.Pyerite
          + single mineralYield.Tritanium * value.Tritanium
          + single mineralYield.Zydrine   * value.Zydrine
        ) * multiplier


    let RefineValueIce (multiplier:single) (item:IRawIce) (value:IceValue) = 
        let iceYield = item.GetYield ()
        (   single iceYield.HeavyWater          * value.HeavyWater
          + single iceYield.HeliumIsotopes      * value.HeliumIsotopes
          + single iceYield.HydrogenIsotopes    * value.HydrogenIsotopes
          + single iceYield.LiquidOzone         * value.LiquidOzone
          + single iceYield.NitrogenIsotopes    * value.NitrogenIsotopes
          + single iceYield.OxygenIsotopes      * value.OxygenIsotopes
          + single iceYield.StrontiumClathrates * value.StrontiumClathrates
        ) * multiplier


    let LoadBaseOreValues (multiplier:single) (value:OreValue) = 
        [
            for ore in Collections.RawOreList do
                yield ore.GetBase(), RefineValueOre multiplier ore value
        ]