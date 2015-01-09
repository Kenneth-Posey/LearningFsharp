﻿namespace EveOnline.ProductDomain

module Product = 
    open EveOnline.ProductDomain.Types
    open EveOnline.ProductDomain.Records

    let MineralData x = 
        (fun (x, y, z) -> 
            {
                Mineral = x
                TypeId  = TypeId y
                Name    = Name z
            }) <|
                match x with
                | Mineral.Tritanium  -> x, 34, "Tritanium"
                | Mineral.Pyerite    -> x, 35, "Pyerite"
                | Mineral.Mexallon   -> x, 36, "Mexallon"
                | Mineral.Isogen     -> x, 37, "Isogen"
                | Mineral.Nocxium    -> x, 38, "Nocxium"
                | Mineral.Zydrine    -> x, 39, "Zydrine"
                | Mineral.Megacyte   -> x, 40, "Megacyte"
                | Mineral.Morphite   -> x, 11399, "Morphite"
            
    let IceProductData x =
        (fun (x, y, z) -> 
            {
                IceProduct = x
                TypeId     = TypeId y
                Name       = Name z
            }) <|
                match x with
                | IceProduct.HeavyWater       -> x, 16272, "Heavy Water"
                | IceProduct.HeliumIsotopes   -> x, 16274, "Helium Isotopes"
                | IceProduct.HydrogenIsotopes -> x, 17889, "Hydrogen Isotopes"
                | IceProduct.LiquidOzone      -> x, 16273, "Liquid Ozone"
                | IceProduct.NitrogenIsotopes -> x, 17888, "Nitrogen Isotopes"
                | IceProduct.OxygenIsotopes   -> x, 17887, "Oxygen Isotopes"
                | IceProduct.StrontiumClathrates -> x, 16275, "Strontium Clathrates"

    let MineralTypeid x = 
        match x with
        | Mineral.Tritanium -> 34
        | Mineral.Pyerite   -> 35
        | Mineral.Mexallon  -> 36
        | Mineral.Isogen    -> 37
        | Mineral.Nocxium   -> 38
        | Mineral.Zydrine   -> 39
        | Mineral.Megacyte  -> 40
        | Mineral.Morphite  -> 11399

    let IceProductTypeid x = 
        match x with
        | IceProduct.HeavyWater           -> 16272
        | IceProduct.HeliumIsotopes       -> 16274
        | IceProduct.HydrogenIsotopes     -> 17889
        | IceProduct.LiquidOzone          -> 16273
        | IceProduct.NitrogenIsotopes     -> 17888
        | IceProduct.OxygenIsotopes       -> 17887
        | IceProduct.StrontiumClathrates  -> 16275