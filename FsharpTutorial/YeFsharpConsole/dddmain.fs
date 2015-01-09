namespace MainProgramDDD

open Format.Text

module Main = 
    open EveOnline.ProductDomain.Types
    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Records
    open EveOnline.MarketDomain.Market

    [<EntryPoint>]
    let main (_) = 
        
        let iceProductPrices = loadIceProductPrices (OrderType.SellOrder) (Jita)
        let mineralPrices    = loadMineralPrices (OrderType.BuyOrder) (Jita)





        0