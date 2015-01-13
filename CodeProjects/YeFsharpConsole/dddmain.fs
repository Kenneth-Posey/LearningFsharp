namespace MainProgramDDD

open Format.Text

module Main = 
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

        for ice in IceList do
            System.Console.WriteLine ( "Ice " + (Name ice).Value
                + " with value: " 
                + string (GetRefineValue (GetYield ice) (iceProductPrices)).Value
            )
            
            
        for ore in OreList do
            System.Console.WriteLine ( "Ore " + (Name ore).Value
                + " with value: "
                + string (GetRefineValue (GetYield ore) (mineralPrices)).Value
            )

        0 