namespace EveOnline

module OreRecords = 
    open EveOnline.ProductTypes
    open EveOnline.ProductRecords
        
    type OreYield = {
        Tritanium   : Tritanium
        Pyerite     : Pyerite 
        Mexallon    : Mexallon
        Isogen      : Isogen  
        Nocxium     : Nocxium 
        Megacyte    : Megacyte
        Zydrine     : Zydrine 
        Morphite    : Morphite
    }

    let BaseOreYield = {
        Tritanium   = Tritanium 0
        Pyerite     = Pyerite 0
        Mexallon    = Mexallon 0
        Isogen      = Isogen 0
        Nocxium     = Nocxium 0
        Megacyte    = Megacyte 0
        Zydrine     = Zydrine 0
        Morphite    = Morphite 0
    }

    type OreData = {
        OreId  : OreId
        Name   : Name
        OreQty : OreQty
    }

    type RawOre = {
        Name    : Name
        OreId   : OreId
        Qty     : OreQty
        Yield   : OreYield
        Volume  : Volume
        OreType : OreType
    }