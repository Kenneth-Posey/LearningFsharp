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
        
        type ParsedData<'a, 'b> = 
            {
                buyOrders  : 'a
                sellOrders : 'b
                lowSell    : single
                highSell   : single
                lowBuy     : single
                highBuy    : single
            }

    module RawMaterials =    
        type Minerals =
        | Tritanium = 34
        | Pyerite   = 35
        | Mexallon  = 36
        | Isogen    = 37
        | Nocxium   = 38
        | Zydrine   = 39
        | Megacyte  = 40
        
        
        type SimpleOre = {
            Name   : string
            TypeId : int
            Value  : single
            IsTiny : bool
        }

        type IOre = 
            abstract member GetName   : unit -> string
            abstract member GetBase   : unit -> int
            abstract member GetBase5  : unit -> int
            abstract member GetBase10 : unit -> int
            abstract member IsTiny    : unit -> bool

        type IRawOre =
            inherit IOre

        type ICompressedOre =
            inherit IOre

        let AllOreNames = [
                "Veldspar"
                "Scordite"
                "Pyroxeres"
                "Hedbergite"
                "Hemorphite"
                "Jaspet"
                "Plagioclase"
                "Spodumain"
                "Kernite"
                "Arkonor"
                "Bistot"
                "Crokite"
                "Omber"
                "Gneiss"
                "Dark Ochre"
                "Mercoxit"
            ]

        type Veldspar () =
            interface IRawOre with
                override this.GetName () = "Veldspar"
                override this.GetBase () = Veldspar.Base
                override this.GetBase5 () = Veldspar.Base5
                override this.GetBase10 () = Veldspar.Base10
                override this.IsTiny () = false

            static member val Base   = 1230  with get
            static member val Base5  = 17470 with get
            static member val Base10 = 17471 with get

        
        type Scordite () =
            interface IRawOre with
                override this.GetName () = "Scordite"
                override this.GetBase () = Scordite.Base
                override this.GetBase5 () = Scordite.Base5
                override this.GetBase10 () = Scordite.Base10
                override this.IsTiny () = false

            static member val Base   = 1228  with get
            static member val Base5  = 17463 with get
            static member val Base10 = 17464 with get
        

        type Pyroxeres () =
            interface IRawOre with
                override this.GetName () = "Pyroxeres"
                override this.GetBase () = Pyroxeres.Base
                override this.GetBase5 () = Pyroxeres.Base5
                override this.GetBase10 () = Pyroxeres.Base10
                override this.IsTiny () = false

            static member val Base   = 1224  with get
            static member val Base5  = 17459 with get
            static member val Base10 = 17460 with get
        
        
        type Hedbergite () =
            interface IRawOre with
                override this.GetName () = "Hedbergite"
                override this.GetBase () = Hedbergite.Base
                override this.GetBase5 () = Hedbergite.Base5
                override this.GetBase10 () = Hedbergite.Base10
                override this.IsTiny () = false

            static member val Base   = 21    with get
            static member val Base5  = 17440 with get
            static member val Base10 = 17441 with get
       

        type Hemorphite () = 
            interface IRawOre with
                override this.GetName () = "Hemorphite"
                override this.GetBase () = Hemorphite.Base
                override this.GetBase5 () = Hemorphite.Base5
                override this.GetBase10 () = Hemorphite.Base10
                override this.IsTiny () = false

            static member val Base   = 1231  with get
            static member val Base5  = 17444 with get
            static member val Base10 = 17445 with get
        

        type Jaspet () = 
            interface IRawOre with
                override this.GetName () = "Jaspet"
                override this.GetBase () = Jaspet.Base
                override this.GetBase5 () = Jaspet.Base5
                override this.GetBase10 () = Jaspet.Base10
                override this.IsTiny () = false

            static member val Base   = 1226  with get
            static member val Base5  = 17448 with get
            static member val Base10 = 17449 with get
        

        type Plagioclase () =
            interface IRawOre with
                override this.GetName () = "Plagioclase"
                override this.GetBase () = Plagioclase.Base
                override this.GetBase5 () = Plagioclase.Base5
                override this.GetBase10 () = Plagioclase.Base10
                override this.IsTiny () = false

            static member val Base   = 18 with get
            static member val Base5  = 17455 with get
            static member val Base10 = 17456 with get


        type Spodumain () =
            interface IRawOre with
                override this.GetName () = "Spodumain"
                override this.GetBase () = Spodumain.Base
                override this.GetBase5 () = Spodumain.Base5
                override this.GetBase10 () = Spodumain.Base10
                override this.IsTiny () = false

            static member val Base   = 19 with get
            static member val Base5  = 17466 with get
            static member val Base10 = 17467 with get


        type Kernite () =
            interface IRawOre with
                override this.GetName () = "Kernite"
                override this.GetBase () = Kernite.Base
                override this.GetBase5 () = Kernite.Base5
                override this.GetBase10 () = Kernite.Base10
                override this.IsTiny () = false

            static member val Base   = 20 with get
            static member val Base5  = 17452 with get
            static member val Base10 = 17453 with get


        type Arkonor () =
            interface IRawOre with
                override this.GetName () = "Arkonor"
                override this.GetBase () = Arkonor.Base
                override this.GetBase5 () = Arkonor.Base5
                override this.GetBase10 () = Arkonor.Base10
                override this.IsTiny () = false

            static member val Base   = 22 with get
            static member val Base5  = 17425 with get
            static member val Base10 = 17426 with get


        type Bistot () =
            interface IRawOre with
                override this.GetName () = "Bistot"
                override this.GetBase () = Bistot.Base
                override this.GetBase5 () = Bistot.Base5
                override this.GetBase10 () = Bistot.Base10
                override this.IsTiny () = false

            static member val Base   = 1223 with get
            static member val Base5  = 17428 with get
            static member val Base10 = 17429 with get


        type Crokite () =
            interface IRawOre with
                override this.GetName () = "Crokite"
                override this.GetBase () = Crokite.Base
                override this.GetBase5 () = Crokite.Base5
                override this.GetBase10 () = Crokite.Base10
                override this.IsTiny () = false

            static member val Base   = 1225 with get
            static member val Base5  = 17432 with get
            static member val Base10 = 17433 with get


        type Omber () =
            interface IRawOre with
                override this.GetName () = "Omber"
                override this.GetBase () = Omber.Base
                override this.GetBase5 () = Omber.Base5
                override this.GetBase10 () = Omber.Base10
                override this.IsTiny () = false

            static member val Base   = 1227 with get
            static member val Base5  = 17867 with get
            static member val Base10 = 17868 with get


        type Gneiss () =
            interface IRawOre with
                override this.GetName () = "Gneiss"
                override this.GetBase () = Gneiss.Base
                override this.GetBase5 () = Gneiss.Base5
                override this.GetBase10 () = Gneiss.Base10
                override this.IsTiny () = false

            static member val Base   = 1229 with get
            static member val Base5  = 17865 with get
            static member val Base10 = 17866 with get


        type ``Dark Ochre`` () =
            interface IRawOre with
                override this.GetName () = "Dark Ochre"
                override this.GetBase () = ``Dark Ochre``.Base
                override this.GetBase5 () = ``Dark Ochre``.Base5
                override this.GetBase10 () = ``Dark Ochre``.Base10
                override this.IsTiny () = false

            static member val Base   = 1232 with get
            static member val Base5  = 17436 with get
            static member val Base10 = 17437 with get


        type Mercoxit () =
            interface IRawOre with
                override this.GetName () = "Mercoxit"
                override this.GetBase () = Mercoxit.Base
                override this.GetBase5 () = Mercoxit.Base5
                override this.GetBase10 () = Mercoxit.Base10
                override this.IsTiny () = false

            static member val Base   = 11396 with get
            static member val Base5  = 17869 with get
            static member val Base10 = 17870 with get


        type CompVeldspar () =
            interface ICompressedOre with
                override this.GetName () = "Veldspar"
                override this.GetBase () = CompVeldspar.Base
                override this.GetBase5 () = CompVeldspar.Base5
                override this.GetBase10 () = CompVeldspar.Base10
                override this.IsTiny () = true

            static member val Base   = 28430 with get
            static member val Base5  = 28431 with get
            static member val Base10 = 28432 with get


        type CompScordite () =
            interface ICompressedOre with
                override this.GetName () = "Scordite"
                override this.GetBase () = CompScordite.Base
                override this.GetBase5 () = CompScordite.Base5
                override this.GetBase10 () = CompScordite.Base10
                override this.IsTiny () = true

            static member val Base   = 28427 with get
            static member val Base5  = 28428 with get
            static member val Base10 = 28429 with get
                                                 

        type CompPyroxeres () =       
            interface ICompressedOre with           
                override this.GetName () = "Pyroxeres"
                override this.GetBase () = CompPyroxeres.Base
                override this.GetBase5 () = CompPyroxeres.Base5
                override this.GetBase10 () = CompPyroxeres.Base10
                override this.IsTiny () = true

            static member val Base   = 28424 with get
            static member val Base5  = 28425 with get
            static member val Base10 = 28426 with get
                                                 

        type CompHedbergite () =        
            interface ICompressedOre with          
                override this.GetName () = "Hedbergite"
                override this.GetBase () = CompHedbergite.Base
                override this.GetBase5 () = CompHedbergite.Base5
                override this.GetBase10 () = CompHedbergite.Base10
                override this.IsTiny () = true

            static member val Base   = 28400 with get
            static member val Base5  = 28401 with get
            static member val Base10 = 28402 with get
                                                 

        type CompHemorphite () =             
            interface ICompressedOre with     
                override this.GetName () = "Hemorphite"
                override this.GetBase () = CompHemorphite.Base
                override this.GetBase5 () = CompHemorphite.Base5
                override this.GetBase10 () = CompHemorphite.Base10
                override this.IsTiny () = true

            static member val Base   = 28403 with get
            static member val Base5  = 28404 with get
            static member val Base10 = 28405 with get
                                                 

        type CompJaspet () =                
            interface ICompressedOre with
                override this.GetName () = "Jaspet"
                override this.GetBase () = CompJaspet.Base
                override this.GetBase5 () = CompJaspet.Base5
                override this.GetBase10 () = CompJaspet.Base10
                override this.IsTiny () = true
        
            static member val Base   = 28406 with get
            static member val Base5  = 28407 with get
            static member val Base10 = 28408 with get
                    
