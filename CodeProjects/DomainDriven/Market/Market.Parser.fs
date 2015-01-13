namespace EveOnline.MarketDomain

module Parser = 
    open Utility.UtilityFunctions
    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Providers
    open EveOnline.MarketDomain.Records
    
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


    let OrderProcessor (quantity:int) (orders:Order list) =
        let Iterate (quantity:int) (orders:Order list) =
            let rec IterateRec (total) (quantity:int) (orders:Order list) =
                match quantity <= 0 || orders.Length = 0 with
                | true  -> total
                | false -> let total = total + orders.Head.Price * single quantity
                           let quant = quantity - orders.Head.VolRemain
                           IterateRec total quant orders.Tail
                                               
            IterateRec 0.0f quantity orders
            
        match (quantity = 0) || (orders.Length = 0) with
        | true  -> 0.0f                                                 
        | false -> Iterate quantity orders


    let ParseQuickLook (data:string) :ParsedData<Order list, Order list> =
        let providerData = MarketOrder.QuickLookResult.Parse(data).Quicklook
        
        let buyOrders = 
            providerData.BuyOrders.Orders 
            |> List.ofArray
            |> List.sortWith (fun x y -> 
                match x.Price <> y.Price with
                | true when x.Price > y.Price -> 1
                | true when x.Price < y.Price -> -1
                | _ -> 0 )

        let sellOrders = 
            providerData.SellOrders.Orders 
            |> List.ofArray
            |> List.sortWith (fun x y -> 
                match x.Price <> y.Price with
                | true when x.Price < y.Price -> 1
                | true when x.Price > y.Price -> -1
                | _ -> 0 )

        let lowBuy = single buyOrders.Head.Price * 0.8f
        let highSell = single sellOrders.Head.Price * 1.2f
        
        let boundedBuyOrders = 
            buyOrders  
            |> List.filter (fun x -> lowBuy < single x.Price)
            |> List.map (fun x -> new Order(x, new BuyOrder()))

        let boundedSellOrders = 
            sellOrders 
            |> List.filter (fun x -> highSell > single x.Price)
            |> List.map (fun x -> new Order(x, new SellOrder()))

        let highBuy = boundedBuyOrders  |> List.rev |> List.head |> (fun x -> single x.Price * 0.8f)
        let lowSell = boundedSellOrders |> List.rev |> List.head |> (fun x -> single x.Price * 1.2f)

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
    

    // functions for composing the parser urls
    let loadUrl = Utility.UtilityFunctions.LoadUrl
    let composeUrl = Utility.UtilityFunctions.ComposeUrl
    let parse = ParseQuickLook
    let baseUrl = (fun loc item -> 
                        EveOnline.MarketDomain.Providers.QuickLook + "?typeid=" + item 
                        + "&usesystem=" + string (SystemId loc) 
                    )


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
