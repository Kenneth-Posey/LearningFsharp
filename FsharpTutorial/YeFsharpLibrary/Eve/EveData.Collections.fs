namespace EveData

open EveData.RawMaterials
open EveData.Ore
open EveData.Ice

module Collections = 
    type SystemName =
    | Jita    = 30000142
    | Dodixie = 30002659
    | Amarr   = 30002187
    | Hek     = 30002053
    | Rens    = 30002510
        
    let Locations = [
        int SystemName.Jita    
        int SystemName.Amarr   
        int SystemName.Dodixie 
        int SystemName.Rens    
        int SystemName.Hek     
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

        
    let RawOreList = [
        new RawMaterials.Arkonor     () :> RawMaterials.IRawOre
        new RawMaterials.Bistot      () :> RawMaterials.IRawOre
        new RawMaterials.Crokite     () :> RawMaterials.IRawOre
        new RawMaterials.DarkOchre   () :> RawMaterials.IRawOre
        new RawMaterials.Gneiss      () :> RawMaterials.IRawOre
        new RawMaterials.Hedbergite  () :> RawMaterials.IRawOre
        new RawMaterials.Hemorphite  () :> RawMaterials.IRawOre
        new RawMaterials.Jaspet      () :> RawMaterials.IRawOre
        new RawMaterials.Kernite     () :> RawMaterials.IRawOre
        new RawMaterials.Mercoxit    () :> RawMaterials.IRawOre
        new RawMaterials.Omber       () :> RawMaterials.IRawOre
        new RawMaterials.Plagioclase () :> RawMaterials.IRawOre
        new RawMaterials.Pyroxeres   () :> RawMaterials.IRawOre
        new RawMaterials.Scordite    () :> RawMaterials.IRawOre
        new RawMaterials.Spodumain   () :> RawMaterials.IRawOre
        new RawMaterials.Veldspar    () :> RawMaterials.IRawOre
    ]

    let CompressedRawOreList = [
        new RawMaterials.CompArkonor     () :> RawMaterials.IRawOre
        new RawMaterials.CompBistot      () :> RawMaterials.IRawOre
        new RawMaterials.CompCrokite     () :> RawMaterials.IRawOre
        new RawMaterials.CompDarkOchre   () :> RawMaterials.IRawOre
        new RawMaterials.CompGneiss      () :> RawMaterials.IRawOre
        new RawMaterials.CompHedbergite  () :> RawMaterials.IRawOre
        new RawMaterials.CompHemorphite  () :> RawMaterials.IRawOre
        new RawMaterials.CompJaspet      () :> RawMaterials.IRawOre
        new RawMaterials.CompKernite     () :> RawMaterials.IRawOre
        new RawMaterials.CompMercoxit    () :> RawMaterials.IRawOre
        new RawMaterials.CompOmber       () :> RawMaterials.IRawOre
        new RawMaterials.CompPlagioclase () :> RawMaterials.IRawOre
        new RawMaterials.CompPyroxeres   () :> RawMaterials.IRawOre
        new RawMaterials.CompScordite    () :> RawMaterials.IRawOre
        new RawMaterials.CompSpodumain   () :> RawMaterials.IRawOre
        new RawMaterials.CompVeldspar    () :> RawMaterials.IRawOre
    ]

    let AllOrePairs = List.zip RawOreList CompressedRawOreList

    type RawIce = RawMaterials.IRawMat<RawMaterials.IceYield>
    let RawIceList = [
        new RawMaterials.BlueIce             () :> RawMaterials.IRawIce
        new RawMaterials.ClearIcicle         () :> RawMaterials.IRawIce
        new RawMaterials.DarkGlitter         () :> RawMaterials.IRawIce
        new RawMaterials.EnrichedClearIcicle () :> RawMaterials.IRawIce
        new RawMaterials.Gelidus             () :> RawMaterials.IRawIce
        new RawMaterials.GlacialMass         () :> RawMaterials.IRawIce
        new RawMaterials.GlareCrust          () :> RawMaterials.IRawIce
        new RawMaterials.Krystallos          () :> RawMaterials.IRawIce
        new RawMaterials.SmoothGlacialMass   () :> RawMaterials.IRawIce
        new RawMaterials.PristineWhiteGlaze  () :> RawMaterials.IRawIce
        new RawMaterials.ThickBlueIce        () :> RawMaterials.IRawIce
        new RawMaterials.WhiteGlaze          () :> RawMaterials.IRawIce
    ]

    let CompressedRawIceList = [
        new RawMaterials.CompBlueIce             () :> RawMaterials.IRawIce
        new RawMaterials.CompClearIcicle         () :> RawMaterials.IRawIce
        new RawMaterials.CompDarkGlitter         () :> RawMaterials.IRawIce
        new RawMaterials.CompEnrichedClearIcicle () :> RawMaterials.IRawIce
        new RawMaterials.CompGelidus             () :> RawMaterials.IRawIce
        new RawMaterials.CompGlacialMass         () :> RawMaterials.IRawIce
        new RawMaterials.CompGlareCrust          () :> RawMaterials.IRawIce
        new RawMaterials.CompKrystallos          () :> RawMaterials.IRawIce
        new RawMaterials.CompSmoothGlacialMass   () :> RawMaterials.IRawIce
        new RawMaterials.CompPristineWhiteGlaze  () :> RawMaterials.IRawIce
        new RawMaterials.CompThickBlueIce        () :> RawMaterials.IRawIce
        new RawMaterials.CompWhiteGlaze          () :> RawMaterials.IRawIce
    ]

    let AllIcePairs = List.zip RawIceList CompressedRawIceList

    module Dynamic = 
        let LoadBaseOreValues (multiplier:single) (value:RawMaterials.OreValue) = 
            [
                for ore in RawOreList do
                    yield ore.GetName(), ore.GetBase(), RawMaterials.RefineValueOre multiplier ore value
            ]


        let LoadBaseIceValues (multiplier: single) (value:RawMaterials.IceValue) = 
            [
                for ice in RawIceList do
                    yield ice.GetName(), ice.GetBase(), RawMaterials.RefineValueIce multiplier ice value
            ]