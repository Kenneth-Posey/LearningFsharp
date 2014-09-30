namespace EveData

module Market = 
    let TypeIdUrl   = "http://eve-files.com/chribba/typeid.txt"
    let QuickLook   = "http://api.eve-central.com/api/quicklook"

    type MarketAPIRequest = {
        typeid      : int
        usesystem   : int
        regionlimit : int
        sethours    : int
        setminQ     : int
    }

    module ApiFeed =
        open FSharp.Data

        type CrestAPI =
            JsonProvider<"http://public-crest.eveonline.com/">
        
        type IndustryFacilities = 
            JsonProvider<"http://public-crest.eveonline.com/industry/facilities/">


    module MarketOrder = 
        open FSharp.Data

        type QuickLookResult = 
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

    module Types =
        /// These are aliases to much simplify the syntax around these provided types
        type QuickLookProvider = MarketOrder.QuickLookResult
        type SellOrderProvider = MarketOrder.QuickLookResult.Order
        type BuyOrderProvider  = MarketOrder.QuickLookResult.Order2

        type BuyOrder () =
            member val OrderType = "Buy" with get
            member this.GetType () = typeof<BuyOrderProvider>

        type SellOrder () =
            member val OrderType = "Sell" with get
            member this.GetType () = typeof<SellOrderProvider>

        type Order (region, station, stationName, security, range, price, volRemain, minVolume, orderType) as this =
            member val Region      = region
            member val Station     = station    
            member val StationName = stationName
            member val Security    = security   
            member val Range       = range      
            member val Price       = single price      
            member val VolRemain   = volRemain  
            member val MinVolume   = minVolume  
            member val OrderType   = orderType

            // F# requires that we use different constructor signatures
            // which is why we use a mostly empty class to force the
            // signatures to be different
            new (order:BuyOrderProvider, typeDef:BuyOrder) = 
                Order (   region      = order.Region
                        , station     = order.Station
                        , stationName = order.StationName
                        , security    = order.Security
                        , range       = order.Range
                        , price       = order.Price
                        , volRemain   = order.VolRemain
                        , minVolume   = order.MinVolume
                        , orderType   = typeDef.OrderType   )

            new (order:SellOrderProvider, typeDef:SellOrder) = 
                Order (   region      = order.Region
                        , station     = order.Station
                        , stationName = order.StationName
                        , security    = order.Security
                        , range       = order.Range
                        , price       = order.Price
                        , volRemain   = order.VolRemain
                        , minVolume   = order.MinVolume 
                        , orderType   = typeDef.OrderType   )
        
        type MarketPrices =  {
            highBuy  : single
            lowSell  : single
            lowBuy   : single
            highSell : single
        }
            
        type ParsedData<'a, 'b> = {
            buyOrders  : 'a
            sellOrders : 'b
            prices     : MarketPrices
        }