namespace MainProgramDDD

open Format.Text

module Main = 
    open EveOnline.ProductDomain.Types
    open EveOnline.ProductDomain.Records
    open EveOnline.IceDomain.Types
    open EveOnline.IceDomain.Records
    open EveOnline.IceDomain.Ice
    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Records
    open EveOnline.MarketDomain.Market
    open EveOnline.DataDomain.Collections

    [<EntryPoint>]
    let main (_) = 
        
        let iceProductPrices = loadIceProductPrices (OrderType.SellOrder) (Jita)
        // let mineralPrices = loadMineralPrices (OrderType.BuyOrder) (Jita)

        for ice in IceList do
            System.Console.WriteLine ( "Ice " + (RawIceName ice).Value
                + " with value: " 
                + string (refineIceValue (RawIceYield ice) (iceProductPrices)).Value
            )
            


        0 