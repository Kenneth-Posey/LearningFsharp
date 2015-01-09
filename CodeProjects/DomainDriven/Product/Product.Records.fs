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

