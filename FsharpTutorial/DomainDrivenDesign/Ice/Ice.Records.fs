﻿namespace EveOnline.IceDomain

module Records = 
    open EveOnline.ProductDomain.Types
    open EveOnline.IceDomain.Types

    type IceYield = {
        HeavyWater          : HeavyWater         
        HeliumIsotopes      : HeliumIsotopes     
        HydrogenIsotopes    : HydrogenIsotopes   
        LiquidOzone         : LiquidOzone        
        NitrogenIsotopes    : NitrogenIsotopes   
        OxygenIsotopes      : OxygenIsotopes     
        StrontiumClathrates : StrontiumClathrates
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
        IceType : IceType
    }

