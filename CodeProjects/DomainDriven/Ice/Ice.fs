namespace EveOnline.IceDomain

module Ice = 
    open EveOnline.ProductDomain.UnionTypes
    open EveOnline.ProductDomain.Types
    open EveOnline.IceDomain.Types
    open EveOnline.IceDomain.Records

    // All ice has the same volume so as long as the type matches, we're good
    let RawIceVolume (y:Compressed) :Volume = 
        Volume <|
            match y with 
            | (IsCompressed)    -> 100.0f
            | (IsNotCompressed) -> 1000.0f

        
    let RawIceYield (x:IceType) :IceYield = 
        match x with
        | ClearIcicle -> 
            { BaseIceYield with HeavyWater          = HeavyWater           50  
                                LiquidOzone         = LiquidOzone          25  
                                StrontiumClathrates = StrontiumClathrates  1   
                                HeliumIsotopes      = HeliumIsotopes       300  }
        | EnrichedClearIcicle ->                                           
            { BaseIceYield with HeavyWater          = HeavyWater           75  
                                LiquidOzone         = LiquidOzone          40  
                                StrontiumClathrates = StrontiumClathrates  1   
                                HeliumIsotopes      = HeliumIsotopes       350  }
        | BlueIce ->                                                       
            { BaseIceYield with HeavyWater          = HeavyWater           50  
                                LiquidOzone         = LiquidOzone          25  
                                StrontiumClathrates = StrontiumClathrates  1   
                                OxygenIsotopes      = OxygenIsotopes       300  }
        | ThickBlueIce ->                                                  
            { BaseIceYield with HeavyWater          = HeavyWater           75  
                                LiquidOzone         = LiquidOzone          40  
                                StrontiumClathrates = StrontiumClathrates  1   
                                OxygenIsotopes      = OxygenIsotopes       350  }
        | GlacialMass ->                                                   
            { BaseIceYield with HeavyWater          = HeavyWater           50  
                                LiquidOzone         = LiquidOzone          25  
                                StrontiumClathrates = StrontiumClathrates  1   
                                HydrogenIsotopes    = HydrogenIsotopes     300  }
        | SmoothGlacialMass ->                                             
            { BaseIceYield with HeavyWater          = HeavyWater           75  
                                LiquidOzone         = LiquidOzone          40  
                                StrontiumClathrates = StrontiumClathrates  1   
                                HydrogenIsotopes    = HydrogenIsotopes     350  }
        | WhiteGlaze ->                                                    
            { BaseIceYield with HeavyWater          = HeavyWater           50  
                                LiquidOzone         = LiquidOzone          25  
                                StrontiumClathrates = StrontiumClathrates  1   
                                NitrogenIsotopes    = NitrogenIsotopes     300  }
        | PristineWhiteGlaze ->                                            
            { BaseIceYield with HeavyWater          = HeavyWater           75  
                                LiquidOzone         = LiquidOzone          40  
                                StrontiumClathrates = StrontiumClathrates  1   
                                NitrogenIsotopes    = NitrogenIsotopes     350  }
        | GlareCrust ->                                                    
            { BaseIceYield with HeavyWater          = HeavyWater           1000
                                LiquidOzone         = LiquidOzone          500 
                                StrontiumClathrates = StrontiumClathrates  25   }
        | DarkGlitter ->                                                   
            { BaseIceYield with HeavyWater          = HeavyWater           500 
                                LiquidOzone         = LiquidOzone          1000
                                StrontiumClathrates = StrontiumClathrates  50   }
        | Gelidus ->                                                       
            { BaseIceYield with HeavyWater          = HeavyWater           250 
                                LiquidOzone         = LiquidOzone          500 
                                StrontiumClathrates = StrontiumClathrates  75   }
        | Krystallos ->                                                    
            { BaseIceYield with HeavyWater          = HeavyWater           125 
                                LiquidOzone         = LiquidOzone          250 
                                StrontiumClathrates = StrontiumClathrates  125  }
    
    let RawIceTypeId (x:IceType) :TypeId =
        TypeId <| 
            match x with
            | BlueIce       -> 16264
            | ClearIcicle   -> 16262
            | GlacialMass   -> 16263
            | WhiteGlaze    -> 16265
            | GlareCrust    -> 16266
            | DarkGlitter   -> 16267
            | Gelidus       -> 16268
            | Krystallos    -> 16268
            | ThickBlueIce          -> 17975
            | EnrichedClearIcicle   -> 17978
            | SmoothGlacialMass     -> 17977
            | PristineWhiteGlaze    -> 17976

    let CompressedIceTypeId (x:IceType) :TypeId = 
        TypeId <|
            match x with
            | BlueIce       -> 28433
            | ClearIcicle   -> 28434
            | GlacialMass   -> 28438
            | WhiteGlaze    -> 28444
            | GlareCrust    -> 28439
            | DarkGlitter   -> 28435
            | Gelidus       -> 28437
            | Krystallos    -> 28440
            | ThickBlueIce          -> 28443
            | EnrichedClearIcicle   -> 28436
            | SmoothGlacialMass     -> 28442
            | PristineWhiteGlaze    -> 28441

    let IceTypeId (x:IceType) (y:Compressed) :TypeId =
        match y with 
        | IsNotCompressed -> RawIceTypeId x
        | IsCompressed -> CompressedIceTypeId x
                

    let RawIceName (x:IceType) :Name = 
        Name <| 
            match x with 
            | ClearIcicle   -> "Clear Icicle"
            | BlueIce       -> "Blue Ice"
            | GlacialMass   -> "Glacial Mass"
            | WhiteGlaze    -> "White Glaze"
            | GlareCrust    -> "Glare Crust"
            | DarkGlitter   -> "Dark Glitter"
            | Gelidus       -> "Gelidus"
            | Krystallos    -> "Krystallos"
            | ThickBlueIce          -> "Thick Blue Ice"
            | EnrichedClearIcicle   -> "Enriched Clear Icicle"
            | SmoothGlacialMass     -> "Smooth Glacial Mass"
            | PristineWhiteGlaze    -> "Pristine White Glaze"


    let CompressedIceName (x:IceType) :Name = 
        Name <| "Compressed " + (RawIceName x).Value

    let IceName (x:IceType) (y:Compressed) :Name=
        match y with
        | IsCompressed -> RawIceName x
        | IsNotCompressed -> CompressedIceName x
        
    
    let IceFactory (c:Compressed) (q:Qty) (n:IceType) :RawIce=
        {
            IceType = n
            Qty     = q
            Name    = IceName n c
            IceId   = IceTypeId n c
            Yield   = RawIceYield n 
            Volume  = RawIceVolume c
        }
