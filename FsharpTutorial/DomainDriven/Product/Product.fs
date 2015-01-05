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

    let TypeId x = 
        match x with
        | Mineral x -> MineralTypeid x
        | IceProduct x -> IceProductTypeid x