﻿namespace EveData

open EveData.RawMaterials
open EveData.Ore.Types
open EveData.Ice.Types

module Collections = 
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
        new Ore.RawMaterials.Arkonor     () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Bistot      () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Crokite     () :> Ore.Types.IRawOre
        new Ore.RawMaterials.DarkOchre   () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Gneiss      () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Hedbergite  () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Hemorphite  () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Jaspet      () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Kernite     () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Mercoxit    () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Omber       () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Plagioclase () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Pyroxeres   () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Scordite    () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Spodumain   () :> Ore.Types.IRawOre
        new Ore.RawMaterials.Veldspar    () :> Ore.Types.IRawOre
    ]

    let CompressedRawOreList = [
        new Ore.RawMaterials.CompArkonor     () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompBistot      () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompCrokite     () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompDarkOchre   () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompGneiss      () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompHedbergite  () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompHemorphite  () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompJaspet      () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompKernite     () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompMercoxit    () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompOmber       () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompPlagioclase () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompPyroxeres   () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompScordite    () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompSpodumain   () :> Ore.Types.IRawOre
        new Ore.RawMaterials.CompVeldspar    () :> Ore.Types.IRawOre
    ]

    let AllOrePairs = List.zip RawOreList CompressedRawOreList

    let RawIceList = [
        new Ice.RawMaterials.BlueIce             () :> Ice.Types.IRawIce
        new Ice.RawMaterials.ClearIcicle         () :> Ice.Types.IRawIce
        new Ice.RawMaterials.DarkGlitter         () :> Ice.Types.IRawIce
        new Ice.RawMaterials.EnrichedClearIcicle () :> Ice.Types.IRawIce
        new Ice.RawMaterials.Gelidus             () :> Ice.Types.IRawIce
        new Ice.RawMaterials.GlacialMass         () :> Ice.Types.IRawIce
        new Ice.RawMaterials.GlareCrust          () :> Ice.Types.IRawIce
        new Ice.RawMaterials.Krystallos          () :> Ice.Types.IRawIce
        new Ice.RawMaterials.SmoothGlacialMass   () :> Ice.Types.IRawIce
        new Ice.RawMaterials.PristineWhiteGlaze  () :> Ice.Types.IRawIce
        new Ice.RawMaterials.ThickBlueIce        () :> Ice.Types.IRawIce
        new Ice.RawMaterials.WhiteGlaze          () :> Ice.Types.IRawIce
    ]

    let CompressedRawIceList = [
        new Ice.RawMaterials.CompBlueIce             () :> Ice.Types.IRawIce
        new Ice.RawMaterials.CompClearIcicle         () :> Ice.Types.IRawIce
        new Ice.RawMaterials.CompDarkGlitter         () :> Ice.Types.IRawIce
        new Ice.RawMaterials.CompEnrichedClearIcicle () :> Ice.Types.IRawIce
        new Ice.RawMaterials.CompGelidus             () :> Ice.Types.IRawIce
        new Ice.RawMaterials.CompGlacialMass         () :> Ice.Types.IRawIce
        new Ice.RawMaterials.CompGlareCrust          () :> Ice.Types.IRawIce
        new Ice.RawMaterials.CompKrystallos          () :> Ice.Types.IRawIce
        new Ice.RawMaterials.CompSmoothGlacialMass   () :> Ice.Types.IRawIce
        new Ice.RawMaterials.CompPristineWhiteGlaze  () :> Ice.Types.IRawIce
        new Ice.RawMaterials.CompThickBlueIce        () :> Ice.Types.IRawIce
        new Ice.RawMaterials.CompWhiteGlaze          () :> Ice.Types.IRawIce
    ]

    let AllIcePairs = List.zip RawIceList CompressedRawIceList

    module Dynamic = 
        open Ore.Types
        open Ice.Types
        let LoadBaseOreValues (multiplier:single) (value:OreValue) = 
            [
                for ore in RawOreList do
                    yield ore.GetName(), ore.GetBase(), Ore.Functions.RefineValueOre multiplier ore value
            ]


        let LoadBaseIceValues (multiplier: single) (value:IceValue) = 
            [
                for ice in RawIceList do
                    yield ice.GetName(), ice.GetBase(), Ice.Functions.RefineValueIce multiplier ice value
            ]