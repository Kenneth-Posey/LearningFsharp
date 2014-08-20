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
        type QuickLook = MarketOrder.QuickLookResult
        type BuyOrder  = MarketOrder.QuickLookResult.Order2
        type SellOrder = MarketOrder.QuickLookResult.Order

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
                    
