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
        
        [<AbstractClass>]
        type Ore () = 
            abstract member GetName : unit -> string

        [<AbstractClass>]
        type CompressedOre () =
            inherit Ore ()

        type Veldspar () =
            inherit Ore ()
                override this.GetName () = "Veldspar"
                
            static member val Base   = 1230  with get
            static member val Base5  = 17470 with get
            static member val Base10 = 17471 with get

            static member val Concentrated = 17470 with get
            static member val Dense        = 17471 with get

        
        type Scordite () =
            inherit Ore ()
                override this.GetName () = "Scordite"

            static member val Base      = 1228  with get
            static member val Base5     = 17463 with get
            static member val Base10    = 17464 with get

            static member val Condensed = 17463 with get
            static member val Massive   = 17464 with get
        

        type Pyroxeres () =
            inherit Ore ()
                override this.GetName () = "Pyroxeres"

            static member val Base   = 1224    with get
            static member val Base5  = 17459   with get
            static member val Base10 = 17460   with get

            static member val Solid    = 17459 with get
            static member val Viscous  = 17460 with get
        
        
        type Hedbergite () =
            inherit Ore ()
                override this.GetName () = "Hedbergite"

            static member val Base   = 21      with get
            static member val Base5  = 17440   with get
            static member val Base10 = 17441   with get

            static member val Vitric   = 17440 with get
            static member val Glazed   = 17441 with get
        

        type Hemorphite () = 
            inherit Ore ()
                override this.GetName () = "Hemorphite"

            static member val Base   = 1231    with get
            static member val Base5  = 17444   with get
            static member val Base10 = 17445   with get

            static member val Vivid    = 17444 with get
            static member val Radiant  = 17445 with get
        

        type Jaspet () = 
            inherit Ore ()
                override this.GetName () = "Jaspet"

            static member val Base   = 1226    with get
            static member val Base5  = 17448   with get
            static member val Base10 = 17449   with get

            static member val Pure     = 17448 with get
            static member val Pristine = 17449 with get
        
        type CompVeldspar () =
            inherit CompressedOre ()
                override this.GetName () = "Veldspar"

            static member val Base       = 28430   with get
            static member val Base5      = 28431   with get
            static member val Base10     = 28432   with get

            static member val Concentrated = 28431 with get
            static member val Dense        = 28432 with get

        type CompScordite () =
            inherit CompressedOre ()
                override this.GetName () = "Scordite"

            static member val Base    = 28427    with get
            static member val Base5   = 28428    with get
            static member val Base10  = 28429    with get

            static member val Condensed = 28428  with get
            static member val Massive   = 28429  with get
                                                 
        type CompPyroxeres () =                  
            inherit CompressedOre ()             
                override this.GetName () = "Pyroxeres"

            static member val Base    = 28424    with get
            static member val Base5   = 28425    with get
            static member val Base10  = 28426    with get

            static member val Solid   = 28425    with get
            static member val Viscous = 28426    with get
                                                 
        type CompHedbergite () =                 
            inherit CompressedOre ()             
                override this.GetName () = "Hedbergite"

            static member val Base    = 28400    with get
            static member val Base5   = 28401    with get
            static member val Base10  = 28402    with get

            static member val Vitric  = 28401    with get
            static member val Glazed  = 28402    with get
                                                 
        type CompHemorphite () =                 
            inherit CompressedOre ()             
                override this.GetName () = "Hemorphite"

            static member val Base    = 18403    with get
            static member val Base5   = 28404    with get
            static member val Base10  = 28405    with get

            static member val Vivid   = 28404    with get
            static member val Radiant = 28405    with get
                                                 
        type CompJaspet () =                     
            inherit CompressedOre ()     
                override this.GetName () = "Jaspet"
        
            static member val Base    = 28406    with get
            static member val Base5   = 28407    with get
            static member val Base10  = 28408    with get

            static member val Pure     = 28407   with get
            static member val Pristine = 28408   with get
                    
