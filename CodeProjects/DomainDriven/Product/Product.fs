namespace EveOnline.ProductDomain

module Product = 
    open EveOnline.ProductDomain.Types
    open EveOnline.ProductDomain.Records

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


    let MineralData x = 
        (fun (x, y, z) -> 
            {
                Mineral = x
                TypeId  = TypeId y
                Name    = Name z
            }) <|
                match x with
                | Mineral.Tritanium  -> x, MineralTypeid x, "Tritanium"
                | Mineral.Pyerite    -> x, MineralTypeid x, "Pyerite"
                | Mineral.Mexallon   -> x, MineralTypeid x, "Mexallon"
                | Mineral.Isogen     -> x, MineralTypeid x, "Isogen"
                | Mineral.Nocxium    -> x, MineralTypeid x, "Nocxium"
                | Mineral.Zydrine    -> x, MineralTypeid x, "Zydrine"
                | Mineral.Megacyte   -> x, MineralTypeid x, "Megacyte"
                | Mineral.Morphite   -> x, MineralTypeid x, "Morphite"
            
    let IceProductData x =
        (fun (x, y, z) -> 
            {
                IceProduct = x
                TypeId     = TypeId y
                Name       = Name z
            }) <|
                match x with
                | IceProduct.HeavyWater       -> x, IceProductTypeid x, "Heavy Water"
                | IceProduct.HeliumIsotopes   -> x, IceProductTypeid x, "Helium Isotopes"
                | IceProduct.HydrogenIsotopes -> x, IceProductTypeid x, "Hydrogen Isotopes"
                | IceProduct.LiquidOzone      -> x, IceProductTypeid x, "Liquid Ozone"
                | IceProduct.NitrogenIsotopes -> x, IceProductTypeid x, "Nitrogen Isotopes"
                | IceProduct.OxygenIsotopes   -> x, IceProductTypeid x, "Oxygen Isotopes"
                | IceProduct.StrontiumClathrates -> x, IceProductTypeid x, "Strontium Clathrates"