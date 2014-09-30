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


    module RawMaterials =    
        type SimpleOre = {
            Name   : string
            TypeId : int
            Value  : single
            IsTiny : bool
        }
        
        type OreYield = {
            Tritanium   : int
            Pyerite     : int
            Mexallon    : int
            Isogen      : int
            Nocxium     : int
            Megacyte    : int
            Zydrine     : int
            Morphite    : int
        }

        let BaseOreYield = {
            Tritanium   = 0
            Pyerite     = 0
            Mexallon    = 0
            Isogen      = 0
            Nocxium     = 0
            Megacyte    = 0
            Zydrine     = 0
            Morphite    = 0
        }

        type IceYield = {
            HeavyWater          : int
            HeliumIsotopes      : int
            HydrogenIsotopes    : int
            LiquidOzone         : int
            NitrogenIsotopes    : int
            OxygenIsotopes      : int
            StrontiumClathrates : int
        }

        let BaseIceYield = {
            HeavyWater          = 0
            HeliumIsotopes      = 0
            HydrogenIsotopes    = 0
            LiquidOzone         = 0
            NitrogenIsotopes    = 0
            OxygenIsotopes      = 0
            StrontiumClathrates = 0
        }

        let FactorOreYield (baseYield:OreYield) (factor:single) =
            let multiply x = int (single x * factor)
            {   
                BaseOreYield with
                Tritanium   = multiply baseYield.Tritanium
                Pyerite     = multiply baseYield.Pyerite  
                Mexallon    = multiply baseYield.Mexallon 
                Isogen      = multiply baseYield.Isogen   
                Nocxium     = multiply baseYield.Nocxium  
                Megacyte    = multiply baseYield.Megacyte 
                Zydrine     = multiply baseYield.Zydrine
                Morphite    = multiply baseYield.Morphite
            }

        let CompressedYield (baseYield:OreYield) =
            FactorOreYield baseYield 100.0f

        type IRawMat<'T> = 
            abstract member GetName   : unit -> string
            abstract member IsTiny    : unit -> bool
            abstract member GetYield  : unit -> 'T
            abstract member GetVolume : unit -> single
            abstract member GetBase   : unit -> int

        type IOre = 
            inherit IRawMat<OreYield>
            abstract member GetBase5  : unit -> int
            abstract member GetBase10 : unit -> int

        type IIce = 
            inherit IRawMat<IceYield>
            
        // The actual prices for the minerals
        type OreValue = {
            Tritanium   : single
            Pyerite     : single
            Mexallon    : single
            Isogen      : single
            Nocxium     : single
            Megacyte    : single
            Zydrine     : single
            Morphite    : single
        }

        // The actual prices for the ice products
        type IceValue = {
            HeavyWater          : single
            HeliumIsotopes      : single
            HydrogenIsotopes    : single
            LiquidOzone         : single
            NitrogenIsotopes    : single
            OxygenIsotopes      : single
            StrontiumClathrates : single           
        }

        type IRawOre =
            inherit IOre

        type ICompressedOre =
            inherit IRawOre

        type IRawIce =
            inherit IIce

        type ICompressedIce = 
            inherit IRawIce

        type Veldspar () =
            interface IRawOre with
                override this.GetName () = "Veldspar"
                override this.GetBase () = Veldspar.Base
                override this.GetBase5 () = Veldspar.Base5
                override this.GetBase10 () = Veldspar.Base10
                override this.IsTiny () = false
                override this.GetYield () = Veldspar.Yield
                override this.GetVolume () = Veldspar.Volume

            static member val Base   = 1230  with get
            static member val Base5  = 17470 with get
            static member val Base10 = 17471 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 415
                } with get
            static member val Volume = 0.1f with get

        
        type Scordite () =
            interface IRawOre with
                override this.GetName () = "Scordite"
                override this.GetBase () = Scordite.Base
                override this.GetBase5 () = Scordite.Base5
                override this.GetBase10 () = Scordite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Scordite.Yield
                override this.GetVolume () = Scordite.Volume

            static member val Base   = 1228  with get
            static member val Base5  = 17463 with get
            static member val Base10 = 17464 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 346
                    Pyerite     = 173
                } with get
            static member val Volume = 0.15f with get
        

        type Pyroxeres () =
            interface IRawOre with
                override this.GetName () = "Pyroxeres"
                override this.GetBase () = Pyroxeres.Base
                override this.GetBase5 () = Pyroxeres.Base5
                override this.GetBase10 () = Pyroxeres.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Pyroxeres.Yield
                override this.GetVolume () = Pyroxeres.Volume

            static member val Base   = 1224  with get
            static member val Base5  = 17459 with get
            static member val Base10 = 17460 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 315
                    Pyerite     = 25
                    Mexallon    = 50
                    Nocxium     = 5
                } with get
            static member val Volume = 0.3f with get
        
        
        type Plagioclase () =
            interface IRawOre with
                override this.GetName () = "Plagioclase"
                override this.GetBase () = Plagioclase.Base
                override this.GetBase5 () = Plagioclase.Base5
                override this.GetBase10 () = Plagioclase.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Plagioclase.Yield
                override this.GetVolume () = Plagioclase.Volume

            static member val Base   = 18 with get
            static member val Base5  = 17455 with get
            static member val Base10 = 17456 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 107
                    Pyerite     = 213
                    Mexallon    = 107
                } with get
            static member val Volume = 0.35f with get


        type Omber () =
            interface IRawOre with
                override this.GetName () = "Omber"
                override this.GetBase () = Omber.Base
                override this.GetBase5 () = Omber.Base5
                override this.GetBase10 () = Omber.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Omber.Yield
                override this.GetVolume () = Omber.Volume

            static member val Base   = 1227 with get
            static member val Base5  = 17867 with get
            static member val Base10 = 17868 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 85
                    Pyerite     = 34
                    Isogen      = 85
                } with get
            static member val Volume = 0.6f with get


        type Kernite () =
            interface IRawOre with
                override this.GetName () = "Kernite"
                override this.GetBase () = Kernite.Base
                override this.GetBase5 () = Kernite.Base5
                override this.GetBase10 () = Kernite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Kernite.Yield
                override this.GetVolume () = Kernite.Volume

            static member val Base   = 20 with get
            static member val Base5  = 17452 with get
            static member val Base10 = 17453 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 134
                    Mexallon    = 267
                    Isogen      = 134
                } with get
            static member val Volume = 1.2f with get
        

        type Jaspet () = 
            interface IRawOre with
                override this.GetName () = "Jaspet"
                override this.GetBase () = Jaspet.Base
                override this.GetBase5 () = Jaspet.Base5
                override this.GetBase10 () = Jaspet.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Jaspet.Yield
                override this.GetVolume () = Jaspet.Volume

            static member val Base   = 1226  with get
            static member val Base5  = 17448 with get
            static member val Base10 = 17449 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 72
                    Pyerite     = 121
                    Mexallon    = 144
                    Nocxium     = 72
                    Zydrine     = 3
                } with get
            static member val Volume = 2.0f with get
       

        type Hemorphite () = 
            interface IRawOre with
                override this.GetName () = "Hemorphite"
                override this.GetBase () = Hemorphite.Base
                override this.GetBase5 () = Hemorphite.Base5
                override this.GetBase10 () = Hemorphite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Hemorphite.Yield
                override this.GetVolume () = Hemorphite.Volume

            static member val Base   = 1231  with get
            static member val Base5  = 17444 with get
            static member val Base10 = 17445 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 180
                    Pyerite     = 72
                    Mexallon    = 17
                    Isogen      = 59
                    Nocxium     = 118
                    Zydrine     = 8
                } with get
            static member val Volume = 3.0f with get
            

        type Hedbergite () =
            interface IRawOre with
                override this.GetName () = "Hedbergite"
                override this.GetBase () = Hedbergite.Base
                override this.GetBase5 () = Hedbergite.Base5
                override this.GetBase10 () = Hedbergite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Hedbergite.Yield
                override this.GetVolume () = Hedbergite.Volume

            static member val Base   = 21    with get
            static member val Base5  = 17440 with get
            static member val Base10 = 17441 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Pyerite     = 81
                    Isogen      = 196
                    Nocxium     = 98
                    Zydrine     = 9
                } with get
            static member val Volume = 3.0f with get


        type Gneiss () =
            interface IRawOre with
                override this.GetName () = "Gneiss"
                override this.GetBase () = Gneiss.Base
                override this.GetBase5 () = Gneiss.Base5
                override this.GetBase10 () = Gneiss.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Gneiss.Yield
                override this.GetVolume () = Gneiss.Volume

            static member val Base   = 1229 with get
            static member val Base5  = 17865 with get
            static member val Base10 = 17866 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 1278
                    Mexallon    = 1278
                    Isogen      = 242
                    Zydrine     = 60
                } with get
            static member val Volume = 5.0f with get


        type DarkOchre () =
            interface IRawOre with
                override this.GetName () = "Dark Ochre"
                override this.GetBase () = DarkOchre.Base
                override this.GetBase5 () = DarkOchre.Base5
                override this.GetBase10 () = DarkOchre.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = DarkOchre.Yield
                override this.GetVolume () = DarkOchre.Volume

            static member val Base   = 1232 with get
            static member val Base5  = 17436 with get
            static member val Base10 = 17437 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 8804
                    Nocxium     = 173
                    Zydrine     = 87
                } with get
            static member val Volume = 8.0f with get


        type Spodumain () =
            interface IRawOre with
                override this.GetName () = "Spodumain"
                override this.GetBase () = Spodumain.Base
                override this.GetBase5 () = Spodumain.Base5
                override this.GetBase10 () = Spodumain.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Spodumain.Yield
                override this.GetVolume () = Spodumain.Volume

            static member val Base   = 19 with get
            static member val Base5  = 17466 with get
            static member val Base10 = 17467 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 39221
                    Pyerite     = 4972
                    Megacyte    = 78
                } with get
            static member val Volume = 16.0f with get


        type Arkonor () =
            interface IRawOre with
                override this.GetName () = "Arkonor"
                override this.GetBase () = Arkonor.Base
                override this.GetBase5 () = Arkonor.Base5
                override this.GetBase10 () = Arkonor.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Arkonor.Yield
                override this.GetVolume () = Arkonor.Volume

            static member val Base   = 22 with get
            static member val Base5  = 17425 with get
            static member val Base10 = 17426 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 6905
                    Mexallon    = 1278
                    Megacyte    = 230
                    Zydrine     = 115
                } with get
            static member val Volume = 16.0f with get


        type Bistot () =
            interface IRawOre with
                override this.GetName () = "Bistot"
                override this.GetBase () = Bistot.Base
                override this.GetBase5 () = Bistot.Base5
                override this.GetBase10 () = Bistot.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Bistot.Yield
                override this.GetVolume () = Bistot.Volume

            static member val Base   = 1223 with get
            static member val Base5  = 17428 with get
            static member val Base10 = 17429 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Pyerite     = 16572
                    Megacyte    = 118
                    Zydrine     = 236
                } with get
            static member val Volume = 16.0f with get


        type Crokite () =
            interface IRawOre with
                override this.GetName () = "Crokite"
                override this.GetBase () = Crokite.Base
                override this.GetBase5 () = Crokite.Base5
                override this.GetBase10 () = Crokite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Crokite.Yield
                override this.GetVolume () = Crokite.Volume

            static member val Base   = 1225 with get
            static member val Base5  = 17432 with get
            static member val Base10 = 17433 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 20992
                    Nocxium     = 275
                    Zydrine     = 367
                } with get
            static member val Volume = 16.0f with get


        type Mercoxit () =
            interface IRawOre with
                override this.GetName () = "Mercoxit"
                override this.GetBase () = Mercoxit.Base
                override this.GetBase5 () = Mercoxit.Base5
                override this.GetBase10 () = Mercoxit.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Mercoxit.Yield
                override this.GetVolume () = Mercoxit.Volume

            static member val Base   = 11396 with get
            static member val Base5  = 17869 with get
            static member val Base10 = 17870 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Morphite    = 293
                } with get
            static member val Volume = 40.0f with get


        type CompVeldspar () =
            interface ICompressedOre with
                override this.GetName () = "Veldspar"
                override this.GetBase () = CompVeldspar.Base
                override this.GetBase5 () = CompVeldspar.Base5
                override this.GetBase10 () = CompVeldspar.Base10
                override this.IsTiny () = true
                override this.GetYield ()  = CompVeldspar.Yield
                override this.GetVolume () = CompVeldspar.Volume

            static member val Base   = 28430 with get
            static member val Base5  = 28431 with get
            static member val Base10 = 28432 with get
            static member val Yield  = CompressedYield Veldspar.Yield with get
            static member val Volume = 0.15f with get


        type CompScordite () =
            interface ICompressedOre with
                override this.GetName () = "Scordite"
                override this.GetBase () = CompScordite.Base
                override this.GetBase5 () = CompScordite.Base5
                override this.GetBase10 () = CompScordite.Base10
                override this.IsTiny () = true
                override this.GetYield ()  = CompScordite.Yield
                override this.GetVolume () = CompScordite.Volume

            static member val Base   = 28427 with get
            static member val Base5  = 28428 with get
            static member val Base10 = 28429 with get
            static member val Yield  = CompressedYield Scordite.Yield with get
            static member val Volume = 0.19f with get
                                                 

        type CompPyroxeres () =       
            interface ICompressedOre with           
                override this.GetName () = "Pyroxeres"
                override this.GetBase () = CompPyroxeres.Base
                override this.GetBase5 () = CompPyroxeres.Base5
                override this.GetBase10 () = CompPyroxeres.Base10
                override this.IsTiny () = true
                override this.GetYield ()  = CompPyroxeres.Yield
                override this.GetVolume () = CompPyroxeres.Volume

            static member val Base   = 28424 with get
            static member val Base5  = 28425 with get
            static member val Base10 = 28426 with get
            static member val Yield  = CompressedYield Pyroxeres.Yield with get
            static member val Volume = 0.16f with get
            
        
        type CompPlagioclase () =
            interface ICompressedOre with
                override this.GetName () = "Plagioclase"
                override this.GetBase () = CompPlagioclase.Base
                override this.GetBase5 () = CompPlagioclase.Base5
                override this.GetBase10 () = CompPlagioclase.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompPlagioclase.Yield
                override this.GetVolume () = CompPlagioclase.Volume

            static member val Base   = 28422 with get
            static member val Base5  = 28421 with get
            static member val Base10 = 28423 with get
            static member val Yield  = CompressedYield Plagioclase.Yield with get
            static member val Volume = 0.15f with get


        type CompOmber () =
            interface ICompressedOre with
                override this.GetName () = "Omber"
                override this.GetBase () = CompOmber.Base
                override this.GetBase5 () = CompOmber.Base5
                override this.GetBase10 () = CompOmber.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompOmber.Yield
                override this.GetVolume () = CompOmber.Volume

            static member val Base   = 28416 with get
            static member val Base5  = 28417 with get
            static member val Base10 = 28415 with get
            static member val Yield  = CompressedYield Omber.Yield with get
            static member val Volume = 0.07f with get


        type CompKernite () =
            interface ICompressedOre with
                override this.GetName () = "Kernite"
                override this.GetBase () = CompKernite.Base
                override this.GetBase5 () = CompKernite.Base5
                override this.GetBase10 () = CompKernite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompKernite.Yield
                override this.GetVolume () = CompKernite.Volume

            static member val Base   = 28410 with get
            static member val Base5  = 28411 with get
            static member val Base10 = 28409 with get
            static member val Yield  = CompressedYield Kernite.Yield with get
            static member val Volume = 0.19f with get
                                                 

        type CompJaspet () =                
            interface ICompressedOre with
                override this.GetName () = "Jaspet"
                override this.GetBase () = CompJaspet.Base
                override this.GetBase5 () = CompJaspet.Base5
                override this.GetBase10 () = CompJaspet.Base10
                override this.IsTiny () = true
                override this.GetYield ()  = CompJaspet.Yield
                override this.GetVolume () = CompJaspet.Volume
        
            static member val Base   = 28406 with get
            static member val Base5  = 28407 with get
            static member val Base10 = 28408 with get
            static member val Yield  = CompressedYield Jaspet.Yield with get
            static member val Volume = 0.15f with get
                                                 

        type CompHemorphite () =             
            interface ICompressedOre with     
                override this.GetName () = "Hemorphite"
                override this.GetBase () = CompHemorphite.Base
                override this.GetBase5 () = CompHemorphite.Base5
                override this.GetBase10 () = CompHemorphite.Base10
                override this.IsTiny () = true
                override this.GetYield ()  = CompHemorphite.Yield
                override this.GetVolume () = CompHemorphite.Volume

            static member val Base   = 28403 with get
            static member val Base5  = 28404 with get
            static member val Base10 = 28405 with get
            static member val Yield  = CompressedYield Hemorphite.Yield with get
            static member val Volume = 0.16f with get


        type CompHedbergite () =        
            interface ICompressedOre with          
                override this.GetName () = "Hedbergite"
                override this.GetBase () = CompHedbergite.Base
                override this.GetBase5 () = CompHedbergite.Base5
                override this.GetBase10 () = CompHedbergite.Base10
                override this.IsTiny () = true
                override this.GetYield ()  = CompHedbergite.Yield
                override this.GetVolume () = CompHedbergite.Volume

            static member val Base   = 28400 with get
            static member val Base5  = 28401 with get
            static member val Base10 = 28402 with get
            static member val Yield  = CompressedYield Hedbergite.Yield with get
            static member val Volume = 0.14f with get


        type CompGneiss () =
            interface ICompressedOre with
                override this.GetName () = "Gneiss"
                override this.GetBase () = CompGneiss.Base
                override this.GetBase5 () = CompGneiss.Base5
                override this.GetBase10 () = CompGneiss.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompGneiss.Yield
                override this.GetVolume () = CompGneiss.Volume

            static member val Base   = 28397 with get
            static member val Base5  = 28398 with get
            static member val Base10 = 28399 with get
            static member val Yield  = CompressedYield Gneiss.Yield with get
            static member val Volume = 1.03f with get


        type CompDarkOchre () =
            interface ICompressedOre with
                override this.GetName () = "Dark Ochre"
                override this.GetBase () = CompDarkOchre.Base
                override this.GetBase5 () = CompDarkOchre.Base5
                override this.GetBase10 () = CompDarkOchre.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompDarkOchre.Yield
                override this.GetVolume () = CompDarkOchre.Volume

            static member val Base   = 28394 with get
            static member val Base5  = 28395 with get
            static member val Base10 = 28396 with get
            static member val Yield  = CompressedYield DarkOchre.Yield with get
            static member val Volume = 3.27f with get


        type CompSpodumain () =
            interface ICompressedOre with
                override this.GetName () = "Spodumain"
                override this.GetBase () = CompSpodumain.Base
                override this.GetBase5 () = CompSpodumain.Base5
                override this.GetBase10 () = CompSpodumain.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompSpodumain.Yield
                override this.GetVolume () = CompSpodumain.Volume

            static member val Base   = 28420 with get
            static member val Base5  = 28418 with get
            static member val Base10 = 28419 with get
            static member val Yield  = CompressedYield Spodumain.Yield with get
            static member val Volume = 16.0f with get


        type CompCrokite () =
            interface ICompressedOre with
                override this.GetName () = "Crokite"
                override this.GetBase () = CompCrokite.Base
                override this.GetBase5 () = CompCrokite.Base5
                override this.GetBase10 () = CompCrokite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompCrokite.Yield
                override this.GetVolume () = CompCrokite.Volume

            static member val Base   = 28391 with get
            static member val Base5  = 28392 with get
            static member val Base10 = 28393 with get
            static member val Yield  = CompressedYield Crokite.Yield with get
            static member val Volume = 7.81f with get


        type CompBistot () =
            interface ICompressedOre with
                override this.GetName () = "Bistot"
                override this.GetBase () = CompBistot.Base
                override this.GetBase5 () = CompBistot.Base5
                override this.GetBase10 () = CompBistot.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompBistot.Yield
                override this.GetVolume () = CompBistot.Volume

            static member val Base   = 28388 with get
            static member val Base5  = 28389 with get
            static member val Base10 = 28390 with get
            static member val Yield  = CompressedYield Bistot.Yield with get
            static member val Volume = 6.11f with get


        type CompArkonor () =
            interface ICompressedOre with
                override this.GetName () = "Arkonor"
                override this.GetBase () = CompArkonor.Base
                override this.GetBase5 () = CompArkonor.Base5
                override this.GetBase10 () = CompArkonor.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompArkonor.Yield
                override this.GetVolume () = CompArkonor.Volume

            static member val Base   = 28367 with get
            static member val Base5  = 28385 with get
            static member val Base10 = 28387 with get
            static member val Yield  = CompressedYield Arkonor.Yield with get
            static member val Volume = 3.08f with get


        type CompMercoxit () =
            interface ICompressedOre with
                override this.GetName () = "Mercoxit"
                override this.GetBase () = CompMercoxit.Base
                override this.GetBase5 () = CompMercoxit.Base5
                override this.GetBase10 () = CompMercoxit.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompMercoxit.Yield
                override this.GetVolume () = CompMercoxit.Volume

            static member val Base   = 28413 with get
            static member val Base5  = 28412 with get
            static member val Base10 = 28414 with get
            static member val Yield  = CompressedYield Mercoxit.Yield with get
            static member val Volume = 0.1f with get
            

        type ClearIcicle () = 
            interface IRawIce with 
                override this.GetName   () = "Clear Icicle"
                override this.GetBase   () = ClearIcicle.Base
                override this.GetVolume () = ClearIcicle.Volume
                override this.GetYield  () = ClearIcicle.Yield
                override this.IsTiny    () = false

            static member val Base   = 16262 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 50
                    LiquidOzone         = 25
                    StrontiumClathrates = 1
                    HeliumIsotopes      = 300
                } with get
            

        type CompClearIcicle () = 
            interface ICompressedIce with 
                override this.GetName   () = "Compressed Clear Icicle"
                override this.GetBase   () = CompClearIcicle.Base
                override this.GetVolume () = CompClearIcicle.Volume
                override this.GetYield  () = CompClearIcicle.Yield
                override this.IsTiny    () = true

            static member val Base   = 28434 with get
            static member val Volume = 100.0f with get
            static member val Yield  = ClearIcicle.Yield with get
            

        type EnrichedClearIcicle () = 
            interface IRawIce with 
                override this.GetName   () = "Enriched Clear Icicle"
                override this.GetBase   () = EnrichedClearIcicle.Base
                override this.GetVolume () = EnrichedClearIcicle.Volume
                override this.GetYield  () = EnrichedClearIcicle.Yield
                override this.IsTiny    () = false

            static member val Base   = 17978 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 75
                    LiquidOzone         = 40
                    StrontiumClathrates = 1
                    HeliumIsotopes      = 350
                } with get
            

        type CompEnrichedClearIcicle () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Enriched Clear Icicle"
                override this.GetBase   () = CompEnrichedClearIcicle.Base
                override this.GetVolume () = CompEnrichedClearIcicle.Volume
                override this.GetYield  () = CompEnrichedClearIcicle.Yield
                override this.IsTiny    () = true

            static member val Base   = 28436 with get
            static member val Volume = 100.0f with get
            static member val Yield  = EnrichedClearIcicle.Yield with get


        type GlacialMass () = 
            interface IRawIce with 
                override this.GetName   () = "Glacial Mass"
                override this.GetBase   () = GlacialMass.Base
                override this.GetVolume () = GlacialMass.Volume
                override this.GetYield  () = GlacialMass.Yield
                override this.IsTiny    () = false

            static member val Base   = 16263 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 50
                    LiquidOzone         = 25
                    StrontiumClathrates = 1
                    HydrogenIsotopes    = 300
                } with get

                
        type CompGlacialMass () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Glacial Mass"
                override this.GetBase   () = CompGlacialMass.Base
                override this.GetVolume () = CompGlacialMass.Volume
                override this.GetYield  () = CompGlacialMass.Yield
                override this.IsTiny    () = true

            static member val Base   = 28438 with get
            static member val Volume = 100.0f with get
            static member val Yield  = GlacialMass.Yield with get


        type SmoothGlacialMass () = 
            interface IRawIce with 
                override this.GetName   () = "Smooth Glacial Mass"
                override this.GetBase   () = SmoothGlacialMass.Base
                override this.GetVolume () = SmoothGlacialMass.Volume
                override this.GetYield  () = SmoothGlacialMass.Yield
                override this.IsTiny    () = false

            static member val Base   = 17977 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 75
                    LiquidOzone         = 40
                    StrontiumClathrates = 1
                    HydrogenIsotopes    = 350
                } with get


        type CompSmoothGlacialMass () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Smooth Glacial Mass"
                override this.GetBase   () = CompSmoothGlacialMass.Base
                override this.GetVolume () = CompSmoothGlacialMass.Volume
                override this.GetYield  () = CompSmoothGlacialMass.Yield
                override this.IsTiny    () = true

            static member val Base   = 28442 with get
            static member val Volume = 100.0f with get
            static member val Yield  = SmoothGlacialMass.Yield with get

                
        type BlueIce () = 
            interface IRawIce with 
                override this.GetName   () = "Blue Ice"
                override this.GetBase   () = BlueIce.Base
                override this.GetVolume () = BlueIce.Volume
                override this.GetYield  () = BlueIce.Yield
                override this.IsTiny    () = false

            static member val Base   = 16264 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 50
                    LiquidOzone         = 25
                    StrontiumClathrates = 1
                    OxygenIsotopes      = 300
                } with get

                
        type CompBlueIce () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Blue Ice"
                override this.GetBase   () = CompBlueIce.Base
                override this.GetVolume () = CompBlueIce.Volume
                override this.GetYield  () = CompBlueIce.Yield
                override this.IsTiny    () = true

            static member val Base   = 28433 with get
            static member val Volume = 100.0f with get
            static member val Yield  = BlueIce.Yield with get

                
        type ThickBlueIce () = 
            interface IRawIce with 
                override this.GetName   () = "Thick Blue Ice"
                override this.GetBase   () = ThickBlueIce.Base
                override this.GetVolume () = ThickBlueIce.Volume
                override this.GetYield  () = ThickBlueIce.Yield
                override this.IsTiny    () = false

            static member val Base   = 17975 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 75
                    LiquidOzone         = 40
                    StrontiumClathrates = 1
                    OxygenIsotopes      = 350
                } with get

                
        type CompThickBlueIce () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Thick Blue Ice"
                override this.GetBase   () = CompThickBlueIce.Base
                override this.GetVolume () = CompThickBlueIce.Volume
                override this.GetYield  () = CompThickBlueIce.Yield
                override this.IsTiny    () = true

            static member val Base   = 28443 with get
            static member val Volume = 100.0f with get
            static member val Yield  = ThickBlueIce.Yield with get


        type WhiteGlaze () = 
            interface IRawIce with 
                override this.GetName   () = "White Glaze"
                override this.GetBase   () = WhiteGlaze.Base
                override this.GetVolume () = WhiteGlaze.Volume
                override this.GetYield  () = WhiteGlaze.Yield
                override this.IsTiny    () = false

            static member val Base   = 16265 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 50
                    LiquidOzone         = 25
                    StrontiumClathrates = 1
                    NitrogenIsotopes    = 300
                } with get


        type CompWhiteGlaze () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed White Glaze"
                override this.GetBase   () = CompWhiteGlaze.Base
                override this.GetVolume () = CompWhiteGlaze.Volume
                override this.GetYield  () = CompWhiteGlaze.Yield
                override this.IsTiny    () = true

            static member val Base   = 28444 with get
            static member val Volume = 100.0f with get
            static member val Yield  = WhiteGlaze.Yield with get


        type PristineWhiteGlaze () = 
            interface IRawIce with 
                override this.GetName   () = "Pristine White Glaze"
                override this.GetBase   () = PristineWhiteGlaze.Base
                override this.GetVolume () = PristineWhiteGlaze.Volume
                override this.GetYield  () = PristineWhiteGlaze.Yield
                override this.IsTiny    () = false

            static member val Base   = 17976 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 75
                    LiquidOzone         = 40
                    StrontiumClathrates = 1
                    NitrogenIsotopes    = 350
                } with get


        type CompPristineWhiteGlaze () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Pristine White Glaze"
                override this.GetBase   () = CompPristineWhiteGlaze.Base
                override this.GetVolume () = CompPristineWhiteGlaze.Volume
                override this.GetYield  () = CompPristineWhiteGlaze.Yield
                override this.IsTiny    () = true

            static member val Base   = 28441 with get
            static member val Volume = 100.0f with get
            static member val Yield  = PristineWhiteGlaze.Yield with get


        type GlareCrust () = 
            interface IRawIce with 
                override this.GetName   () = "Glare Crust"
                override this.GetBase   () = GlareCrust.Base
                override this.GetVolume () = GlareCrust.Volume
                override this.GetYield  () = GlareCrust.Yield
                override this.IsTiny    () = false

            static member val Base   = 16266 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 1000
                    LiquidOzone         = 500
                    StrontiumClathrates = 25
                } with get


        type CompGlareCrust () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Glare Crust"
                override this.GetBase   () = CompGlareCrust.Base
                override this.GetVolume () = CompGlareCrust.Volume
                override this.GetYield  () = CompGlareCrust.Yield
                override this.IsTiny    () = true

            static member val Base   = 28439 with get
            static member val Volume = 100.0f with get
            static member val Yield  = GlareCrust.Yield with get


        type DarkGlitter () = 
            interface IRawIce with 
                override this.GetName   () = "Dark Glitter"
                override this.GetBase   () = DarkGlitter.Base
                override this.GetVolume () = DarkGlitter.Volume
                override this.GetYield  () = DarkGlitter.Yield
                override this.IsTiny    () = false

            static member val Base   = 16267 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 500
                    LiquidOzone         = 1000
                    StrontiumClathrates = 50
                } with get


        type CompDarkGlitter () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Dark Glitter"
                override this.GetBase   () = CompDarkGlitter.Base
                override this.GetVolume () = CompDarkGlitter.Volume
                override this.GetYield  () = CompDarkGlitter.Yield
                override this.IsTiny    () = true

            static member val Base   = 28435 with get
            static member val Volume = 100.0f with get
            static member val Yield  = DarkGlitter.Yield with get


        type Gelidus () = 
            interface IRawIce with 
                override this.GetName   () = "Gelidus"
                override this.GetBase   () = Gelidus.Base
                override this.GetVolume () = Gelidus.Volume
                override this.GetYield  () = Gelidus.Yield
                override this.IsTiny    () = false

            static member val Base   = 16268 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 250
                    LiquidOzone         = 500
                    StrontiumClathrates = 75
                } with get


        type CompGelidus () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Gelidus"
                override this.GetBase   () = CompGelidus.Base
                override this.GetVolume () = CompGelidus.Volume
                override this.GetYield  () = CompGelidus.Yield
                override this.IsTiny    () = true

            static member val Base   = 28437 with get
            static member val Volume = 100.0f with get
            static member val Yield  = Gelidus.Yield with get


        type Krystallos () = 
            interface IRawIce with 
                override this.GetName   () = "Krystallos"
                override this.GetBase   () = Krystallos.Base
                override this.GetVolume () = Krystallos.Volume
                override this.GetYield  () = Krystallos.Yield
                override this.IsTiny    () = false

            static member val Base   = 16268 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 125
                    LiquidOzone         = 250
                    StrontiumClathrates = 125
                } with get


        type CompKrystallos () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Krystallos"
                override this.GetBase   () = CompKrystallos.Base
                override this.GetVolume () = CompKrystallos.Volume
                override this.GetYield  () = CompKrystallos.Yield
                override this.IsTiny    () = true

            static member val Base   = 28440 with get
            static member val Volume = 100.0f with get
            static member val Yield  = Krystallos.Yield with get
                

    module Collections = 
        type MineralIDs =
        | Tritanium = 34
        | Pyerite   = 35
        | Mexallon  = 36
        | Isogen    = 37
        | Nocxium   = 38
        | Zydrine   = 39
        | Megacyte  = 40
        | Morphite  = 11399
        
        let MineralNames = [
           "Tritanium"
           "Pyerite"
           "Mexallon"
           "Isogen"
           "Nocxium"
           "Zydrine"
           "Megacyte"
        ]
        
        type IceProductIDs = 
        | HeavyWater          = 16272
        | HeliumIsotopes      = 16274
        | HydrogenIsotopes    = 17889
        | LiquidOzone         = 16273
        | NitrogenIsotopes    = 17888
        | OxygenIsotopes      = 17887
        | StrontiumClathrates = 16275

        let IceProductNames = [
            "Heavy Water"
            "Helium Isotopes"
            "Hydrogen Isotopes"
            "Liquid Ozone"
            "Nitrogen Isotopes"
            "Oxygen Isotopes"
            "Strontium Clathrates"
        ]
                
        type RawOreIDs = 
        | Veldspar    = 1230
        | Scordite    = 1228
        | Pyroxeres   = 1224
        | Plagioclase = 18
        | Omber       = 1227
        | Kernite     = 20
        | Jaspet      = 1226
        | Hemorphite  = 1229
        | Hedbergite  = 21
        | Spodumain   = 19
        | Arkonor     = 22
        | Bistot      = 1223
        | Crokite     = 1225
        | Gneiss      = 1229
        | DarkOchre   = 1242
        | Mercoxit    = 11396

        let RawOreNames = [
            "Veldspar"
            "Scordite"
            "Pyroxeres"
            "Plagioclase"
            "Omber"
            "Kernite"
            "Jaspet"
            "Hemorphite"
            "Hedbergite"
            "Spodumain"
            "Arkonor"
            "Bistot"
            "Crokite"
            "Gneiss"
            "Dark Ochre"
            "Mercoxit"
        ]
        
        type RawIceIDs =
        | ClearIcicle         = 16262
        | EnrichedClearIcicle = 17978
        | GlacialMass         = 16263
        | SmoothGlacialMass   = 17977
        | WhiteGlaze          = 16265
        | PristineWhiteGlaze  = 17976
        | BlueIce             = 16264
        | ThickBlueIce        = 17975
        | GlareCrust          = 16266
        | DarkGlitter         = 16267
        | Gelidus             = 16268
        | Krystallos          = 16269

        let RawIceNames = [
            "Clear Icicle"
            "Enriched Clear Icicle"
            "Glacial Mass"
            "Smooth Glacial Mass"
            "White Glaze"
            "Pristine White Glaze"
            "Blue Ice"
            "Thick Blue Ice"
            "Glare Crust"
            "Dark Glitter"
            "Gelidus"
            "Krystallos"
        ]

        
        let RawOreList = [
            new RawMaterials.Arkonor     () :> RawMaterials.IRawOre
            new RawMaterials.Bistot      () :> RawMaterials.IRawOre
            new RawMaterials.Crokite     () :> RawMaterials.IRawOre
            new RawMaterials.DarkOchre   () :> RawMaterials.IRawOre
            new RawMaterials.Gneiss      () :> RawMaterials.IRawOre
            new RawMaterials.Hedbergite  () :> RawMaterials.IRawOre
            new RawMaterials.Hemorphite  () :> RawMaterials.IRawOre
            new RawMaterials.Jaspet      () :> RawMaterials.IRawOre
            new RawMaterials.Kernite     () :> RawMaterials.IRawOre
            new RawMaterials.Mercoxit    () :> RawMaterials.IRawOre
            new RawMaterials.Omber       () :> RawMaterials.IRawOre
            new RawMaterials.Plagioclase () :> RawMaterials.IRawOre
            new RawMaterials.Pyroxeres   () :> RawMaterials.IRawOre
            new RawMaterials.Scordite    () :> RawMaterials.IRawOre
            new RawMaterials.Spodumain   () :> RawMaterials.IRawOre
            new RawMaterials.Veldspar    () :> RawMaterials.IRawOre
        ]

        let CompressedRawOreList = [
            new RawMaterials.CompArkonor     () :> RawMaterials.IRawOre
            new RawMaterials.CompBistot      () :> RawMaterials.IRawOre
            new RawMaterials.CompCrokite     () :> RawMaterials.IRawOre
            new RawMaterials.CompDarkOchre   () :> RawMaterials.IRawOre
            new RawMaterials.CompGneiss      () :> RawMaterials.IRawOre
            new RawMaterials.CompHedbergite  () :> RawMaterials.IRawOre
            new RawMaterials.CompHemorphite  () :> RawMaterials.IRawOre
            new RawMaterials.CompJaspet      () :> RawMaterials.IRawOre
            new RawMaterials.CompKernite     () :> RawMaterials.IRawOre
            new RawMaterials.CompMercoxit    () :> RawMaterials.IRawOre
            new RawMaterials.CompOmber       () :> RawMaterials.IRawOre
            new RawMaterials.CompPlagioclase () :> RawMaterials.IRawOre
            new RawMaterials.CompPyroxeres   () :> RawMaterials.IRawOre
            new RawMaterials.CompScordite    () :> RawMaterials.IRawOre
            new RawMaterials.CompSpodumain   () :> RawMaterials.IRawOre
            new RawMaterials.CompVeldspar    () :> RawMaterials.IRawOre
        ]

        type RawIce = RawMaterials.IRawMat<RawMaterials.IceYield>
        let RawIceList = [
            new RawMaterials.BlueIce             () :> RawMaterials.IRawIce
            new RawMaterials.ClearIcicle         () :> RawMaterials.IRawIce
            new RawMaterials.DarkGlitter         () :> RawMaterials.IRawIce
            new RawMaterials.EnrichedClearIcicle () :> RawMaterials.IRawIce
            new RawMaterials.Gelidus             () :> RawMaterials.IRawIce
            new RawMaterials.GlacialMass         () :> RawMaterials.IRawIce
            new RawMaterials.GlareCrust          () :> RawMaterials.IRawIce
            new RawMaterials.Krystallos          () :> RawMaterials.IRawIce
            new RawMaterials.SmoothGlacialMass   () :> RawMaterials.IRawIce
            new RawMaterials.PristineWhiteGlaze  () :> RawMaterials.IRawIce
            new RawMaterials.ThickBlueIce        () :> RawMaterials.IRawIce
            new RawMaterials.WhiteGlaze          () :> RawMaterials.IRawIce
        ]

        let CompressedRawIceList = [
            new RawMaterials.CompBlueIce             () :> RawMaterials.IRawIce
            new RawMaterials.CompClearIcicle         () :> RawMaterials.IRawIce
            new RawMaterials.CompDarkGlitter         () :> RawMaterials.IRawIce
            new RawMaterials.CompEnrichedClearIcicle () :> RawMaterials.IRawIce
            new RawMaterials.CompGelidus             () :> RawMaterials.IRawIce
            new RawMaterials.CompGlacialMass         () :> RawMaterials.IRawIce
            new RawMaterials.CompGlareCrust          () :> RawMaterials.IRawIce
            new RawMaterials.CompKrystallos          () :> RawMaterials.IRawIce
            new RawMaterials.CompSmoothGlacialMass   () :> RawMaterials.IRawIce
            new RawMaterials.CompPristineWhiteGlaze  () :> RawMaterials.IRawIce
            new RawMaterials.CompThickBlueIce        () :> RawMaterials.IRawIce
            new RawMaterials.CompWhiteGlaze          () :> RawMaterials.IRawIce
        ]