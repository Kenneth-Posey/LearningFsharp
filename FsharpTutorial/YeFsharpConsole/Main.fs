namespace MainProgram

open EveOnline
open EveOnline.EveData
open EveOnline.EveData.RawMaterials

open Format.Text

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
            //  ( new Hedbergite  () :> IOre , new CompHedbergite  () :> IOre )
            //  ( new Hemorphite  () :> IOre , new CompHemorphite  () :> IOre )
            //  ( new Jaspet      () :> IOre , new CompJaspet      () :> IOre )   
            //  ( new Plagioclase () :> IOre , new CompPlagioclase () :> IOre )
            //  ( new Spodumain   () :> IOre , new CompSpodumain   () :> IOre )        
            //  ( new Kernite     () :> IOre , new CompKernite     () :> IOre )
            //  ( new Arkonor     () :> IOre , new CompArkonor     () :> IOre )
            //  ( new Bistot      () :> IOre , new CompBistot      () :> IOre )
            //  ( new Crokite     () :> IOre , new CompCrokite     () :> IOre )
            //  ( new Omber       () :> IOre , new CompOmber       () :> IOre )
            //  ( new Gneiss      () :> IOre , new CompGneiss      () :> IOre )
            //  ( new DarkOchre   () :> IOre , new CompDarkOchre   () :> IOre )
            //  ( new Mercoxit    () :> IOre , new CompMercoxit    () :> IOre )
            ]
            
        for location in Locations do
            for orePair in OreList do
                let system = string location           // Gets the enum name
                let name   = (fst orePair).GetName()

                let fast = PrettyPrintFromSingle <| MarketParser.FastProfit orePair (string (int location))
                let best = PrettyPrintFromSingle <| MarketParser.BestProfit orePair (string (int location))

                printfn "For system %5s fast profit: %6s on ore type %s" system fast name
                printfn "For system %5s best profit: %6s on ore type %s" system best name
        

        
        // Must return from program
        0
