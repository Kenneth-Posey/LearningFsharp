namespace EveOnline.ProductDomain

module Records = 
    open EveOnline.ProductDomain.Types
    open EveOnline.ProductDomain.UnionTypes

    type MineralData = {
        TypeId  : TypeId
        Name    : Name
        Mineral : Mineral
    }

    type MineralPrices = {
        Tritanium   : Price
        Pyerite     : Price
        Mexallon    : Price
        Isogen      : Price
        Nocxium     : Price
        Megacyte    : Price
        Zydrine     : Price
        Morphite    : Price
    }

    let BaseMineralPrices = {
        Tritanium   = Price 0.0f
        Pyerite     = Price 0.0f
        Mexallon    = Price 0.0f
        Isogen      = Price 0.0f
        Nocxium     = Price 0.0f
        Megacyte    = Price 0.0f
        Zydrine     = Price 0.0f
        Morphite    = Price 0.0f
    }

    type IceProductData = {
        TypeId     : TypeId
        Name       : Name
        IceProduct : IceProduct
    }
    
    type IceProductPrices = {
        HeavyWater          : Price
        HeliumIsotopes      : Price
        HydrogenIsotopes    : Price
        LiquidOzone         : Price
        NitrogenIsotopes    : Price
        OxygenIsotopes      : Price
        StrontiumClathrates : Price
    }

    let BaseIceProductPrices = {
        HeavyWater          = Price 0.0f
        HeliumIsotopes      = Price 0.0f
        HydrogenIsotopes    = Price 0.0f
        LiquidOzone         = Price 0.0f
        NitrogenIsotopes    = Price 0.0f
        OxygenIsotopes      = Price 0.0f
        StrontiumClathrates = Price 0.0f
    }
