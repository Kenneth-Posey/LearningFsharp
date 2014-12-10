namespace EveData

module RawMaterials =     
    module Records = 
        open RawMaterials.Types

        type Yield = {
            Tritanium           : Tritanium
            Pyerite             : Pyerite 
            Mexallon            : Mexallon
            Isogen              : Isogen  
            Nocxium             : Nocxium 
            Megacyte            : Megacyte
            Zydrine             : Zydrine 
            Morphite            : Morphite
            HeavyWater          : HeavyWater         
            HeliumIsotopes      : HeliumIsotopes     
            HydrogenIsotopes    : HydrogenIsotopes   
            LiquidOzone         : LiquidOzone        
            NitrogenIsotopes    : NitrogenIsotopes   
            OxygenIsotopes      : OxygenIsotopes     
            StrontiumClathrates : StrontiumClathrates
        }
        
        let BaseYield = {
            Tritanium           = Tritanium 0
            Pyerite             = Pyerite 0
            Mexallon            = Mexallon 0
            Isogen              = Isogen 0
            Nocxium             = Nocxium 0
            Megacyte            = Megacyte 0
            Zydrine             = Zydrine 0
            Morphite            = Morphite 0
            HeavyWater          = HeavyWater 0         
            HeliumIsotopes      = HeliumIsotopes 0    
            HydrogenIsotopes    = HydrogenIsotopes 0   
            LiquidOzone         = LiquidOzone 0       
            NitrogenIsotopes    = NitrogenIsotopes 0  
            OxygenIsotopes      = OxygenIsotopes 0    
            StrontiumClathrates = StrontiumClathrates 0
        }

        type RawMaterial = {
            IsCompressed : IsCompressed
            Yield        : Yield
            Volume       : Volume
            TypeId       : TypeId
            Name         : Name
        }
    