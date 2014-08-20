namespace EveOnline

module EveData = 
    let TypeIdUrl   = "http://eve-files.com/chribba/typeid.txt"
    let QuickLook   = "http://api.eve-central.com/api/quicklook"

    type SystemName =
        | Jita    = 30000142
        | Dodixie = 30002659
        | Amarr   = 30002187
        | Hek     = 30002053
        | Rens    = 30002510

    type MarketAPIRequest = {
        typeid      : int
        usesystem   : int
        regionlimit : int
        sethours    : int
        setminQ     : int
        }

    module MarketOrder = 
        open FSharp.Data

        type QuickLook = 
            XmlProvider<"""
                <evec_api version="2.0" method="quicklook">
                    <quicklook>
                        <item>1230</item>
                        <itemname>Veldspar</itemname>
                        <regions></regions>
                        <hours>360</hours>
                        <minqty>1</minqty>
                        <sell_orders>
                            <order id="3717046692">
                                <region>10000043</region>
                                <station>60008494</station>
                                <station_name>Amarr VIII (Oris) - Emperor Family Academy</station_name>
                                <security>1.0</security>
                                <range>32767</range>
                                <price>20.95</price>
                                <vol_remain>307487</vol_remain>
                                <min_volume>1</min_volume>
                                <expires>2014-08-20</expires>
                                <reported_time>08-19 23:00:07</reported_time>
                            </order>
                            <order id="3717046692">
                                <region>10000043</region>
                                <station>60008494</station>
                                <station_name>Amarr VIII (Oris) - Emperor Family Academy</station_name>
                                <security>1.0</security>
                                <range>32767</range>
                                <price>20.95</price>
                                <vol_remain>307487</vol_remain>
                                <min_volume>1</min_volume>
                                <expires>2014-08-20</expires>
                                <reported_time>08-19 23:00:07</reported_time>
                            </order>
                        </sell_orders>
                        <buy_orders>
                            <order id="3717057403">
                                <region>10000043</region>
                                <station>60008494</station>
                                <station_name>Amarr VIII (Oris) - Emperor Family Academy</station_name>
                                <security>1.0</security>
                                <range>-1</range>
                                <price>15.38</price>
                                <vol_remain>546063</vol_remain>
                                <min_volume>1</min_volume>
                                <expires>2014-11-17</expires>
                                <reported_time>08-19 23:00:07</reported_time>
                            </order>
                            <order id="3717057403">
                                <region>10000043</region>
                                <station>60008494</station>
                                <station_name>Amarr VIII (Oris) - Emperor Family Academy</station_name>
                                <security>1.0</security>
                                <range>-1</range>
                                <price>15.38</price>
                                <vol_remain>546063</vol_remain>
                                <min_volume>1</min_volume>
                                <expires>2014-11-17</expires>
                                <reported_time>08-19 23:00:07</reported_time>
                            </order>
                        </buy_orders>
                    </quicklook>
                </evec_api>
            """>

    module RawMaterials =    
        type Minerals =
        | Tritanium = 34
        | Pyerite   = 35
        | Mexallon  = 36
        | Isogen    = 37
        | Nocxium   = 38
        | Zydrine   = 39
        | Megacyte  = 40

        type Veldspar =
        | Default      = 1230
        | Concentrated = 17470
        | Dense        = 17471

        type CompVeldspar =
        | Default      = 28430
        | Concentrated = 28431
        | Dense        = 28432
        
        type Scordite =
        | Default   = 1228
        | Condensed = 17463
        | Massive   = 17464

        type CompScordite =
        | Default   = 28427
        | Condensed = 28428
        | Massive   = 28429

        type Pyroxeres =
        | Default   = 1224
        | Solid     = 17459
        | Viscous   = 17460

        type CompPyroxeres =
        | Default   = 28424
        | Solid     = 28425
        | Viscous   = 28426
        
        type Hedbergite =
        | Default   = 21
        | Vitric    = 17440
        | Glazed    = 17441

        type CompHedbergite = 
        | Default   = 28400
        | Vitric    = 28401
        | Glazed    = 28402

        type Hemorphite = 
        | Default   = 1231
        | Vivid     = 17444
        | Radiant   = 17445

        type CompHemorphite = 
        | Default   = 18403
        | Vivid     = 28404
        | Radiant   = 28405

        type Jaspet = 
        | Default   = 1226
        | Pure      = 17448
        | Pristine  = 17449

        type CompJaspet = 
        | Default   = 28406
        | Pure      = 28407
        | Pristine  = 28408
                    

module MarketParser =
    open System
    open System.Text.RegularExpressions
    open System.Net

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
        text |> Array.filter (fun (x, y) -> IsNotEmpty (x, y))

    let SplitOnNewline (text:string) (regex:Regex) =
        [| 
            for line in text.Split [|'\n'|] do
                yield ParseLine (line.TrimEnd()) regex
        |] 

    let LoadTypeIdsFromUrl (url:string) =
        new Regex "([0-9]{1,9})[ ]{6}([\w '.-_]*)"
        |> SplitOnNewline (LoadUrl url)
        |> FilterEmpty
        
    type iProvider = EveData.MarketOrder.QuickLook
    let LoadQuickLook (data:string) =
        let providerData = iProvider.Parse(data).Quicklook
        let buyOrders = providerData.BuyOrders.Orders
        let sellOrders = providerData.SellOrders.Orders

        let mutable totalBuyLimited = 0
        let mutable totalSellLimited = 0
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

        // for buy in buyOrders do
        //     if lowBuy < single buy.Price then
        //         totalBuyLimited <- totalBuyLimited + buy.VolRemain
        // 
        // for sell in sellOrders do
        //     if highSell > single sell.Price then
        //         totalSellLimited <- totalSellLimited + sell.VolRemain

        // The upper buy and lower sell is implicit to the list
        // so we just need to manually remove the out-of-bounds orders
        let boundedBuyOrders  = buyOrders  |> Array.filter (fun x -> lowBuy < single x.Price)
        let boundedSellOrders = sellOrders |> Array.filter (fun x -> highSell > single x.Price)



        ()
