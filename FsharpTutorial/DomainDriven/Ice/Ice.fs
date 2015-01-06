namespace EveOnline.IceDomain

module Ice = 
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
        

    let IceData (x:IceType) (y:Compressed) :IceData  =
        (fun (x, y) -> { IceId = TypeId x; Name  = Name y }) <| 
            match (x, y) with
            | BlueIce,  IsNotCompressed -> 16264, "Blue Ice"
            | BlueIce,  IsCompressed    -> 28433, "Compressed Blue Ice" 
                
            | ClearIcicle,  IsNotCompressed -> 16262, "Clear Icicle"
            | ClearIcicle,  IsCompressed    -> 28434, "Compressed Clear Icicle"
                
            | DarkGlitter,  IsNotCompressed -> 16267, "Dark Glitter"
            | DarkGlitter,  IsCompressed    -> 28435, "Compressed Dark Glitter"
                
            | EnrichedClearIcicle,  IsNotCompressed -> 17978, "Enriched Clear Icicle"
            | EnrichedClearIcicle,  IsCompressed    -> 28436, "Compressed Enriched Clear Icicle"
                
            | Gelidus,  IsNotCompressed -> 16268, "Gelidus"
            | Gelidus,  IsCompressed    -> 28437, "Compressed Gelidus"
                
            | GlacialMass,  IsNotCompressed -> 16263, "Glacial Mass"
            | GlacialMass,  IsCompressed    -> 28438, "Compressed Glacial Mass"
                
            | GlareCrust,  IsNotCompressed -> 16266, "Glare Crust"
            | GlareCrust,  IsCompressed    -> 28439, "Compressed Glare Crust"
                
            | Krystallos,  IsNotCompressed -> 16268, "Krystallos"
            | Krystallos,  IsCompressed    -> 28440, "Compressed Krystallos"
                
            | PristineWhiteGlaze,  IsNotCompressed -> 17976, "Pristine White Glaze"
            | PristineWhiteGlaze,  IsCompressed    -> 28441, "Compressed Pristine White Glaze"
                
            | SmoothGlacialMass,  IsNotCompressed -> 17977, "Smooth Glacial Mass"
            | SmoothGlacialMass,  IsCompressed    -> 28442, "Compressed Smooth Glacial Mass"
                
            | ThickBlueIce,  IsNotCompressed -> 17975, "Thick Blue Ice"
            | ThickBlueIce,  IsCompressed    -> 28443, "Compressed Thick Blue Ice"
                
            | WhiteGlaze,  IsNotCompressed -> 16265, "White Glaze"
            | WhiteGlaze,  IsCompressed    -> 28444, "Compressed White Glaze"
            


    let IceCategoryName (x:IceType) :Name = 
        Name <| 
            match x with 
            | ClearIcicle           -> "Clear Icicle"
            | EnrichedClearIcicle   -> "Enriched Clear Icicle"
            | BlueIce               -> "Blue Ice"
            | ThickBlueIce          -> "Thick Blue Ice"
            | GlacialMass           -> "Glacial Mass"
            | SmoothGlacialMass     -> "Smooth Glacial Mass"
            | WhiteGlaze            -> "White Glaze"
            | PristineWhiteGlaze    -> "Pristine White Glaze"
            | GlareCrust            -> "Glare Crust"
            | DarkGlitter           -> "Dark Glitter"
            | Gelidus               -> "Gelidus"
            | Krystallos            -> "Krystallos"

        
    
    let IceFactory (c:Compressed) (q:Qty) (n:IceType) :RawIce=
        let data = IceData (n) (c)

        {
            Name    = data.Name
            IceId   = data.IceId
            IceType = n
            Qty     = q
            Yield   = RawIceYield n 
            Volume  = RawIceVolume c
        }

//    let Ice (x:Ice) (q:Qty) :RawIce = 
//        match x with 
//        | RegularBlueIce         -> (IceType.BlueIce q.Value)        |> IceFactory (IsNotCompressed)
//        | RegularClearIcicle     -> (IceType.ClearIcicle q.Value)    |> IceFactory (IsNotCompressed)
//        | RegularGlacialMass     -> (IceType.GlacialMass q.Value)    |> IceFactory (IsNotCompressed)
//        | RegularWhiteGlaze      -> (IceType.WhiteGlaze q.Value)     |> IceFactory (IsNotCompressed)
//        | RegularThickBlueIce          -> (IceType.ThickBlueIce q.Value)           |> IceFactory (IsNotCompressed)
//        | RegularEnrichedClearIcicle   -> (IceType.EnrichedClearIcicle q.Value)    |> IceFactory (IsNotCompressed)
//        | RegularSmoothGlacialMass     -> (IceType.SmoothGlacialMass q.Value)      |> IceFactory (IsNotCompressed)
//        | RegularPristineWhiteGlaze    -> (IceType.PristineWhiteGlaze q.Value)     |> IceFactory (IsNotCompressed)
//        | RegularGlareCrust      -> (IceType.GlareCrust q.Value)     |> IceFactory (IsNotCompressed)
//        | RegularDarkGlitter     -> (IceType.DarkGlitter q.Value)    |> IceFactory (IsNotCompressed)
//        | RegularGelidus         -> (IceType.Gelidus q.Value)        |> IceFactory (IsNotCompressed)
//        | RegularKrystallos      -> (IceType.Krystallos q.Value)     |> IceFactory (IsNotCompressed)
//        | CompressedBlueIce      -> (IceType.BlueIce q.Value)        |> IceFactory (IsCompressed)
//        | CompressedClearIcicle  -> (IceType.ClearIcicle q.Value)    |> IceFactory (IsCompressed)
//        | CompressedGlacialMass  -> (IceType.GlacialMass q.Value)    |> IceFactory (IsCompressed)
//        | CompressedWhiteGlaze   -> (IceType.WhiteGlaze q.Value)     |> IceFactory (IsCompressed)
//        | CompressedThickBlueIce         -> (IceType.ThickBlueIce q.Value)           |> IceFactory (IsCompressed)
//        | CompressedEnrichedClearIcicle  -> (IceType.EnrichedClearIcicle q.Value)    |> IceFactory (IsCompressed)
//        | CompressedSmoothGlacialMass    -> (IceType.SmoothGlacialMass q.Value)      |> IceFactory (IsCompressed)
//        | CompressedPristineWhiteGlaze   -> (IceType.PristineWhiteGlaze q.Value)     |> IceFactory (IsCompressed)
//        | CompressedGlareCrust   -> (IceType.GlareCrust q.Value)     |> IceFactory (IsCompressed)
//        | CompressedDarkGlitter  -> (IceType.DarkGlitter q.Value)    |> IceFactory (IsCompressed)
//        | CompressedGelidus      -> (IceType.Gelidus q.Value)        |> IceFactory (IsCompressed)
//        | CompressedKrystallos   -> (IceType.Krystallos q.Value)     |> IceFactory (IsCompressed) 
    
