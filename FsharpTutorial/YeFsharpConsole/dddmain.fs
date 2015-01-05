namespace MainProgramDDD

open Format.Text

module Main = 
    open EveOnline.DataDomain.Collections
    open EveOnline.OreDomain.Types

    [<EntryPoint>]
    let main (args:string[]) = 
        
        let Locations = [
            SystemName.Jita    
            SystemName.Amarr   
            SystemName.Dodixie 
            SystemName.Rens    
            SystemName.Hek     
        ]

        let Ores = [
            Ore.CommonArkonor
            Ore.CommonBistot
            Ore.CommonCrokite
            Ore.CommonDarkOchre
            Ore.CommonGneiss
            Ore.CommonHedbergite
            Ore.CommonHemorphite
            Ore.CommonJaspet
            Ore.CommonKernite
            Ore.CommonMercoxit
            Ore.CommonOmber
            Ore.CommonPlagioclase
            Ore.CommonPyroxeres
            Ore.CommonScordite
            Ore.CommonSpodumain
            Ore.CommonVeldspar
        ]















        0