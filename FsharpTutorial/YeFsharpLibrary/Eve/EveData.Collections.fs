namespace EveData

module Collections = 
    open EveData.Ice
    open EveData.Ore
    open EveData.RawMaterial
    open EveData.RawMaterialRecords
    open EveData.RawMaterialTypes
    open Microsoft.FSharp.Reflection

    type SystemName =
    | Jita    = 30000142
    | Dodixie = 30002659
    | Amarr   = 30002187
    | Hek     = 30002053
    | Rens    = 30002510
        

    let Locations = [
        SystemName.Jita    
        SystemName.Amarr   
        SystemName.Dodixie 
        SystemName.Rens    
        SystemName.Hek     
    ]
    

    type MineralIDs =
    | Tritanium = 34
    | Pyerite   = 35
    | Mexallon  = 36
    | Isogen    = 37
    | Nocxium   = 38
    | Zydrine   = 39
    | Megacyte  = 40
    | Morphite  = 11399
        

    let MineralNames = [
        "Tritanium"
        "Pyerite"
        "Mexallon"
        "Isogen"
        "Nocxium"
        "Zydrine"
        "Megacyte"
        "Morphite"
    ]


    let MineralIDPairs = [
        "Tritanium" , int MineralIDs.Tritanium 
        "Pyerite"   , int MineralIDs.Pyerite   
        "Mexallon"  , int MineralIDs.Mexallon  
        "Isogen"    , int MineralIDs.Isogen    
        "Nocxium"   , int MineralIDs.Nocxium   
        "Zydrine"   , int MineralIDs.Zydrine   
        "Megacyte"  , int MineralIDs.Megacyte  
        "Morphite"  , int MineralIDs.Morphite  
    ]
        

    type IceProductIDs = 
    | HeavyWater          = 16272
    | HeliumIsotopes      = 16274
    | HydrogenIsotopes    = 17889
    | LiquidOzone         = 16273
    | NitrogenIsotopes    = 17888
    | OxygenIsotopes      = 17887
    | StrontiumClathrates = 16275


    let IceProductNames = [
        "Heavy Water"
        "Helium Isotopes"
        "Hydrogen Isotopes"
        "Liquid Ozone"
        "Nitrogen Isotopes"
        "Oxygen Isotopes"
        "Strontium Clathrates"
    ]
               

    let IceProductIDPairs = [
        "Heavy Water"          , int IceProductIDs.HeavyWater         
        "Helium Isotopes"      , int IceProductIDs.HeliumIsotopes     
        "Hydrogen Isotopes"    , int IceProductIDs.HydrogenIsotopes   
        "Liquid Ozone"         , int IceProductIDs.LiquidOzone        
        "Nitrogen Isotopes"    , int IceProductIDs.NitrogenIsotopes   
        "Oxygen Isotopes"      , int IceProductIDs.OxygenIsotopes     
        "Strontium Clathrates" , int IceProductIDs.StrontiumClathrates
    ]

                
    type RawOreIDs = 
    | Veldspar    = 1230
    | Scordite    = 1228
    | Pyroxeres   = 1224
    | Plagioclase = 18
    | Omber       = 1227
    | Kernite     = 20
    | Jaspet      = 1226
    | Hemorphite  = 1229
    | Hedbergite  = 21
    | Spodumain   = 19
    | Arkonor     = 22
    | Bistot      = 1223
    | Crokite     = 1225
    | Gneiss      = 1229
    | DarkOchre   = 1242
    | Mercoxit    = 11396

    
    let RawOreNames = [
        "Veldspar"
        "Scordite"
        "Pyroxeres"
        "Plagioclase"
        "Omber"
        "Kernite"
        "Jaspet"
        "Hemorphite"
        "Hedbergite"
        "Spodumain"
        "Arkonor"
        "Bistot"
        "Crokite"
        "Gneiss"
        "Dark Ochre"
        "Mercoxit"
    ]


    let RawOreIDPairs = [
        "Veldspar"    , int RawOreIDs.Veldspar
        "Scordite"    , int RawOreIDs.Scordite
        "Pyroxeres"   , int RawOreIDs.Pyroxeres
        "Plagioclase" , int RawOreIDs.Plagioclase
        "Omber"       , int RawOreIDs.Omber
        "Kernite"     , int RawOreIDs.Kernite
        "Jaspet"      , int RawOreIDs.Jaspet
        "Hemorphite"  , int RawOreIDs.Hemorphite
        "Hedbergite"  , int RawOreIDs.Hedbergite
        "Spodumain"   , int RawOreIDs.Spodumain
        "Arkonor"     , int RawOreIDs.Arkonor
        "Bistot"      , int RawOreIDs.Bistot
        "Crokite"     , int RawOreIDs.Crokite
        "Gneiss"      , int RawOreIDs.Gneiss
        "Dark Ochre"  , int RawOreIDs.DarkOchre
        "Mercoxit"    , int RawOreIDs.Mercoxit
    ]


    type RawIceIDs =
    | ClearIcicle         = 16262
    | EnrichedClearIcicle = 17978
    | GlacialMass         = 16263
    | SmoothGlacialMass   = 17977
    | WhiteGlaze          = 16265
    | PristineWhiteGlaze  = 17976
    | BlueIce             = 16264
    | ThickBlueIce        = 17975
    | GlareCrust          = 16266
    | DarkGlitter         = 16267
    | Gelidus             = 16268
    | Krystallos          = 16269


    let RawIceNames = [
        "Clear Icicle"
        "Enriched Clear Icicle"
        "Glacial Mass"
        "Smooth Glacial Mass"
        "White Glaze"
        "Pristine White Glaze"
        "Blue Ice"
        "Thick Blue Ice"
        "Glare Crust"
        "Dark Glitter"
        "Gelidus"
        "Krystallos"
    ]


    let RawIceIDPairs = [
        "Clear Icicle"          , int RawIceIDs.ClearIcicle
        "Enriched Clear Icicle" , int RawIceIDs.EnrichedClearIcicle
        "Glacial Mass"          , int RawIceIDs.GlacialMass
        "Smooth Glacial Mass"   , int RawIceIDs.SmoothGlacialMass
        "White Glaze"           , int RawIceIDs.WhiteGlaze
        "Pristine White Glaze"  , int RawIceIDs.PristineWhiteGlaze
        "Blue Ice"              , int RawIceIDs.BlueIce
        "Thick Blue Ice"        , int RawIceIDs.ThickBlueIce
        "Glare Crust"           , int RawIceIDs.GlareCrust
        "Dark Glitter"          , int RawIceIDs.DarkGlitter
        "Gelidus"               , int RawIceIDs.Gelidus
        "Krystallos"            , int RawIceIDs.Krystallos
    ]
