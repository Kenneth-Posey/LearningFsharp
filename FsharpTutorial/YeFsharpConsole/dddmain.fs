namespace MainProgramDDD

open Format.Text

module Main = 
    open EveOnline.ProductDomain.Types
    open EveOnline.MarketDomain.Types
    open EveOnline.MarketDomain.Market

    [<EntryPoint>]
    let main (_) = 
        
        let result = loadItems Jita <| 
            [
                IceProduct IceProduct.HeavyWater
                IceProduct IceProduct.HeliumIsotopes
                IceProduct IceProduct.HydrogenIsotopes
                IceProduct IceProduct.LiquidOzone
                IceProduct IceProduct.NitrogenIsotopes
                IceProduct IceProduct.OxygenIsotopes
                IceProduct IceProduct.StrontiumClathrates

                Mineral Mineral.Isogen
                Mineral Mineral.Megacyte
                Mineral Mineral.Mexallon
                Mineral Mineral.Morphite
                Mineral Mineral.Nocxium
                Mineral Mineral.Pyerite
                Mineral Mineral.Tritanium
                Mineral Mineral.Zydrine
            ]















        0