namespace EveData

module RawMaterialRecords =     
    open RawMaterialTypes

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

    type IceYield = {
        HeavyWater          : HeavyWater         
        HeliumIsotopes      : HeliumIsotopes     
        HydrogenIsotopes    : HydrogenIsotopes   
        LiquidOzone         : LiquidOzone        
        NitrogenIsotopes    : NitrogenIsotopes   
        OxygenIsotopes      : OxygenIsotopes     
        StrontiumClathrates : StrontiumClathrates
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

    let BaseIceYield = {
        HeavyWater          = HeavyWater 0         
        HeliumIsotopes      = HeliumIsotopes 0    
        HydrogenIsotopes    = HydrogenIsotopes 0   
        LiquidOzone         = LiquidOzone 0       
        NitrogenIsotopes    = NitrogenIsotopes 0  
        OxygenIsotopes      = OxygenIsotopes 0    
        StrontiumClathrates = StrontiumClathrates 0
    }


    type IceData = {
        IceId : IceId
        Name : Name
        IceQty : IceQty
    }
        
    type RawIce = {
        Name    : Name
        IceId   : IceId
        Qty     : IceQty
        Yield   : IceYield
        Volume  : Volume        
        Ice     : IceType
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
        Ore     : OreType
    }



























    type IRawMat<'T> = 
        abstract member GetName   : unit -> string
        abstract member IsTiny    : unit -> bool
        abstract member GetYield  : unit -> 'T
        abstract member GetVolume : unit -> single
        abstract member GetBase   : unit -> int

    type ParserMaterial = {
        name  : string
        price : single
        id    : int
    }