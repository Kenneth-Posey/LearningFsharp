namespace MainProgramDDD

open Format.Text

module Main = 
    open EveOnline.ProductDomain.Types
    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Market

    [<EntryPoint>]
    let main (args:string[]) = 
        
        let result = loadItems Jita <| 
            [
                IceProduct IceProduct.HeavyWater
                IceProduct IceProduct.HeliumIsotopes

            ]















        0