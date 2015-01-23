namespace MainProgramDDD

open Format.Text

module Main = 
    open EveOnline.ProductDomain.Types
    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Records
    open EveOnline.MarketDomain.Providers
    open EveOnline.MarketDomain.Parser
    open EveOnline.MarketDomain.Market
    open EveOnline.DataDomain.Collections

    open EveOnline.IceDomain.Types
    [<EntryPoint>]
    let main (_) = 
        let getPrice x = GetPrice x (OrderType.BuyOrder) (Jita)
        let iceProductPrices = getPrice (RefinedProduct.IceProduct) 
        let mineralPrices = getPrice (RefinedProduct.Mineral)
        let refineValue x y = string (GetRefineValue x y).Value
        let vol = Volume 1000.0f

        for ice in IceList do
            let value = sprintf "%0.2f" (GetVolumePrice vol iceProductPrices IsNotCompressed ice).Value
            System.Console.WriteLine ( "Ice " + (Name ice).Value
                + " with value: " + value
            )
            
            
        for ore in OreList do
            let value = sprintf "%0.2f" (GetVolumePrice vol mineralPrices IsNotCompressed ore).Value
            System.Console.WriteLine ( "Ore " + (Name ore).Value
                + " with value: " + value
            )

        0 