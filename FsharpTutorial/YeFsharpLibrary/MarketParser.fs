namespace EveOnline

module MarketParser =
    open System
    open System.Text.RegularExpressions
    open System.Net

    open EveData
            
    /// For reading and parsing the text file with typeIDs
    // Example use:
    // let itemArray = MarketParser.LoadTypeIdsFromUrl EveData.TypeIdUrl
    // let tritItems  = itemArray 
    //                  |> FilterToOreOnly 
    //                  |> FilterByName "Tritanium"

    let FilterByName (name:string) (tuple:(string * string)[]) =
        tuple
        |> Array.filter ( fun (x, y) -> y.Contains(name) )

    let FilterToOreOnly (tuple:(string * string)[]) = 
        tuple
        |> Array.filter ( fun (x, y) -> 
            y.Contains("Blueprint") && y.Contains("Processing") && y.Contains("Mining") 
                = false )
    
    let LoadUrl (url:string) = 
        use client = new WebClient()
        client.DownloadString( new Uri(url) )

    let ParseLine (text:string) (regex:Regex) =
        let parsed = regex.Match text
        match parsed.Success with
        | true  -> parsed.Groups.[1].Value.Trim() , parsed.Groups.[2].Value.Trim()
        | false -> "" , ""
        // If it is unable to match then it returns 
        // an empty tuple to be filtered out later

    let IsNotEmpty (x:string , y:string) =
        (x.Length > 0) && (y.Length > 0)
        
    let FilterEmpty (text:(string * string)[]) =
        text |> Array.filter ( fun (x, y) -> IsNotEmpty (x, y) )

    let SplitOnNewline (text:string) (regex:Regex) =
        [| 
            for line in text.Split [|'\n'|] do
                yield ParseLine (line.TrimEnd()) regex
        |] 

    let LoadTypeIdsFromUrl (url:string) =
        new Regex "([0-9]{1,9})[ ]{6}([\w '.-_]*)"
        |> SplitOnNewline (LoadUrl url)
        |> FilterEmpty
        
    type ParsedData<'a, 'b> = 
        {
            buyOrders  : 'a
            sellOrders : 'b
            lowSell    : single
            highSell   : single
            lowBuy     : single
            highBuy    : single
        }

    type iProvider = EveData.MarketOrder.QuickLookResult
    let ParseQuickLook (data:string) =
        let providerData = iProvider.Parse(data).Quicklook
        let buyOrders = providerData.BuyOrders.Orders
        let sellOrders = providerData.SellOrders.Orders

        let mutable totalBuy = 0
        let mutable totalSell = 0

        // To find the lowest sell price of the goods
        // you have to test each value and replace the lowest sell
        // each time the price is *lower*. Start high work down.
        let mutable lowSell = System.Single.MaxValue

        // To find the highest buy price of the goods
        // you have to test each value and replace the highest sell
        // each time the price is *higher*. Start low work up.
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
            lowSell    = lowSell
            highSell   = highSell
            lowBuy     = lowBuy
            highBuy    = highBuy
        }
        
    let FindRealCost (quantity:single) (orders:Types.SellOrder[]) =
        let Iterate (quantity:single) (orders:Types.SellOrder[]) =
            let rec IterateRec (quantity:single) (orders:Types.SellOrder[]) (total:single) =
                match quantity <= 0.0f || orders.Length = 0 with
                | true  -> total
                | false -> let total = total + single orders.[0].Price * single quantity
                           let quant = quantity - single orders.[0].VolRemain

                           IterateRec quant orders.[1 .. orders.Length - 1] total
                
            IterateRec quantity orders 0.0f

        if (quantity = 0.0f) || (orders.Length = 0)
           then 0.0f
           else Iterate quantity orders

    let FindRealIncome (quantity:single) (orders:Types.BuyOrder[]) =
        let Iterate (quantity:single) (orders:Types.BuyOrder[]) =
            let rec IterateRec (quantity:single) (orders:Types.BuyOrder[]) (total:single) =
                // If we don't need to "add" any more or if there's no more orders then return
                match quantity <= 0.0f || orders.Length = 0 with
                | true  -> total
                | false -> let total = total + single orders.[0].Price * single quantity
                           let quant = quantity - single orders.[0].VolRemain

                           IterateRec quant orders.[1 .. orders.Length - 1] total
                
            IterateRec quantity orders 0.0f

        if (quantity = 0.0f) || (orders.Length = 0)
           then 0.0f
           else Iterate quantity orders

    let SortBuyFunc (x:Types.BuyOrder) (y:Types.BuyOrder) =
        match x.Price <> y.Price with
        | true when x.Price > y.Price -> 1
        | true when x.Price < y.Price -> -1
        | false -> 0
       
    let SortSellFunc (x:Types.SellOrder) (y:Types.SellOrder) =
        match x.Price <> y.Price with
        | true when x.Price > y.Price -> -1
        | true when x.Price < y.Price -> 1
        | false -> 0

    let RunVeldsparBuy () = 
        // Have to cast enum to int then to string to get actual value
        // Then construct into tuple for passing into lambda expression
        (string (int EveData.RawMaterials.Veldspar.Default), string (int EveData.SystemName.Amarr))
        |> (fun (veld,amarr) -> EveData.QuickLook + "?typeid=" + veld + "&usesystem=" + amarr)
        |> LoadUrl 
        |> ParseQuickLook
        |> (fun x -> x.sellOrders)
        |> Array.sortWith SortSellFunc
        |> FindRealCost 100.0f

    let RunVeldsparSell () = 
        // Have to cast enum to int then to string to get actual value
        // Then construct into tuple for passing into lambda expression
        (string (int EveData.RawMaterials.Veldspar.Default), string (int EveData.SystemName.Amarr))
        |> (fun (veld,amarr) -> EveData.QuickLook + "?typeid=" + veld + "&usesystem=" + amarr)
        |> LoadUrl 
        |> ParseQuickLook
        |> (fun x -> x.buyOrders)
        |> Array.sortWith SortBuyFunc
        |> FindRealIncome 100.0f
                
    let RunPyroxeres () = 
        // Have to cast enum to int then to string to get actual value
        // Then construct into tuple for passing into lambda expression
        (string (int EveData.RawMaterials.Pyroxeres.Default), string (int EveData.SystemName.Amarr))
        |> (fun (veld,amarr) -> EveData.QuickLook + "?typeid=" + veld + "&usesystem=" + amarr)
        |> LoadUrl 
        |> ParseQuickLook
        |> (fun x -> x.sellOrders)
        // NEEDS SORTING!!!
        |> FindRealCost 100.0f