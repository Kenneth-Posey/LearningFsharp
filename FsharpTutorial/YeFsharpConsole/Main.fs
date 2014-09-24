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

                let fast = sprintf "%.2f" <| MarketParser.FastProfit orePair (string (int location))
                let best = sprintf "%.2f" <| MarketParser.BestProfit orePair (string (int location))

                printfn "For system %5s fast profit: %10s on ore type %s" system fast name
                printfn "For system %5s best profit: %10s on ore type %s" system best name
        

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

        // Refining formula: 
        // Equipment base                           maximum 0.60
        //  x (1 + processing skill x 0.03)         maximum 1.15
        //  x (1 + processing efficiency x 0.02)    maximum 1.10
        //  x (1 + ore processing x 0.02)           maximum 1.10
        //  x (1 + processing implant)              maximum 1.04

        // Best yield 0.8683       All lvl V with implants at 60% station
        // POS yield  0.7235       All lvl V with no implant at 52% station
        // NPC yield  0.6707       All lvl IV with no implant at 50% station
        
        // Must return from function
        0
