﻿namespace EveOnline.OreDomain

module Records = 
    open EveOnline.ProductDomain.Types
    open EveOnline.OreDomain.Types
    
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
        OreId  : TypeId
        Name   : Name
    }

    type RawOre = {
        Name    : Name
        OreId   : TypeId
        Qty     : Qty
        Yield   : OreYield
        Volume  : Volume
        OreType : OreType
    }


