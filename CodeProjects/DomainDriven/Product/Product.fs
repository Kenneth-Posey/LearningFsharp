namespace EveOnline.ProductDomain

module Product = 
    open EveOnline.ProductDomain.Types
    open EveOnline.ProductDomain.UnionTypes
    open EveOnline.ProductDomain.Records

    let MineralTypeid x = 
        TypeId <| match x with
        | Tritanium -> 34
        | Pyerite   -> 35
        | Mexallon  -> 36
        | Isogen    -> 37
        | Nocxium   -> 38
        | Zydrine   -> 39
        | Megacyte  -> 40
        | Morphite  -> 11399

    let IceProductTypeid x = 
        TypeId <| match x with
        | HeavyWater           -> 16272
        | HeliumIsotopes       -> 16274
        | HydrogenIsotopes     -> 17889
        | LiquidOzone          -> 16273
        | NitrogenIsotopes     -> 17888
        | OxygenIsotopes       -> 17887
        | StrontiumClathrates  -> 16275

    let MineralName x =
        Name <| match x with
        | Tritanium -> "Tritanium"
        | Pyerite   -> "Pyerite"
        | Mexallon  -> "Mexallon"
        | Isogen    -> "Isogen"
        | Nocxium   -> "Nocxium"
        | Zydrine   -> "Zydrine"
        | Megacyte  -> "Megacyte"
        | Morphite  -> "Morphite"

    let IceProductName x =
        Name <| match x with
        | HeavyWater           -> "Heavy Water"
        | HeliumIsotopes       -> "Helium Isotopes"
        | HydrogenIsotopes     -> "Hydrogen Isotopes"
        | LiquidOzone          -> "Liquid Ozone"
        | NitrogenIsotopes     -> "Nitrogen Isotopes"
        | OxygenIsotopes       -> "Oxygen Isotopes"
        | StrontiumClathrates  -> "Strontium Clathrates"

        
    let MineralData x = 
        {
            Mineral = x
            TypeId  = MineralTypeid x 
            Name    = MineralName x
        }
            
    let IceProductData x =
        {
            IceProduct = x
            TypeId     = IceProductTypeid x
            Name       = IceProductName x
        }