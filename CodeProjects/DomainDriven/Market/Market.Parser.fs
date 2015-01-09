﻿namespace EveOnline.MarketDomain

module Parser = 
    open Utility.UtilityFunctions
    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Providers
    open EveOnline.MarketDomain.Records

    let ParseQuickLook (typeId:int) (data:string) :ParsedData<Order list, Order list> =
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
            buyOrders  = boundedBuyOrders  |> List.ofArray |> List.map (fun x -> new Order(x, new BuyOrder()))
            sellOrders = boundedSellOrders |> List.ofArray |> List.map (fun x -> new Order(x, new SellOrder()))
            prices = {
                typeId     = typeId
                lowSell    = lowSell
                highSell   = highSell
                lowBuy     = lowBuy
                highBuy    = highBuy
            }
        }
    
    
        
    let SortBuyFunc (x:Order) (y:Order) =
        match x.Price <> y.Price with
        | true when x.Price > y.Price -> 1
        | true when x.Price < y.Price -> -1
        | _ -> 0

       
    let SortSellFunc (x:Order) (y:Order) =
        match x.Price <> y.Price with
        | true when x.Price > y.Price -> -1
        | true when x.Price < y.Price -> 1
        | _ -> 0


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

