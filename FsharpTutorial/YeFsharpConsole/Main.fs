namespace MainProgram

open EveOnline
open EveOnline.EveData
open EveOnline.EveData.RawMaterials

module Main = 
    [<EntryPoint>]
    let main (args:string[]) = 
        
        let Locations = [
                SystemName.Jita    
            //  SystemName.Amarr   
            //  SystemName.Dodixie 
            //  SystemName.Rens    
            //  SystemName.Hek     
            ]

        let OreList : List<IOre * IOre> = [
                ( new Veldspar    () :> IOre , new CompVeldspar    () :> IOre )
                ( new Scordite    () :> IOre , new CompScordite    () :> IOre )
                ( new Pyroxeres   () :> IOre , new CompPyroxeres   () :> IOre )
             // ( new Hedbergite  () :> IOre , new CompHedbergite  () :> IOre )
             // ( new Hemorphite  () :> IOre , new CompHemorphite  () :> IOre )
             // ( new Jaspet      () :> IOre , new CompJaspet      () :> IOre )   
             // ( new Plagioclase () :> IOre , new CompPlagioclase () :> IOre )
             // ( new Spodumain   () :> IOre , new CompSpodumain   () :> IOre )        
             // ( new Kernite     () :> IOre , new CompKernite     () :> IOre )
             // ( new Arkonor     () :> IOre , new CompArkonor     () :> IOre )
             // ( new Bistot      () :> IOre , new CompBistot      () :> IOre )
             // ( new Crokite     () :> IOre , new CompCrokite     () :> IOre )
             // ( new Omber       () :> IOre , new CompOmber       () :> IOre )
             // ( new Gneiss      () :> IOre , new CompGneiss      () :> IOre )
             // ( new DarkOchre   () :> IOre , new CompDarkOchre   () :> IOre )
             // ( new Mercoxit    () :> IOre , new CompMercoxit    () :> IOre )
            ]
            
        for location in Locations do
            for orePair in OreList do
                let system = string location           // Gets the enum name
                let name   = (fst orePair).GetName()

                let fast = MarketParser.FastProfit orePair (string (int location))
                let best = MarketParser.BestProfit orePair (string (int location))

                printfn "For system %8s fast profit: %.2f on ore type %s" system fast name
                printfn "For system %8s best profit: %.2f on ore type %s" system best name
        

        // There's four values per ore type to work with:
        // - Comp highest buy / lowest sell
        // - Raw  highest buy / lowest sell

        // The profit is sell 1 compressed - buy * 100 raw

        // There's two potential sell values
        // - Comp highest buy / lowest sell
        // There's two potential buy values
        // - Raw  highest buy / lowest sell

        // Slow (max profit)
        // Cost   : buy  raw  at highest buy + 0.01
        // Profit : sell comp at lowest sell - 0.01

        // Fast (instant profit)
        // Cost   : buy  raw  at lowest sell (real cost)
        // Profit : sell comp at highest buy (real profit)

        // We want to know the profit per unit and per m^3

        // Compressed ores require 1 unit to refine, uncompressed require 100



        
        // Must return from function
        0