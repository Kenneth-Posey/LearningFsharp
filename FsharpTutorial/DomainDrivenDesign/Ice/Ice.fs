namespace EveOnline.IceDomain

module Ice = 
    open EveOnline.ProductDomain.Types
    open EveOnline.IceDomain.Types
    open EveOnline.IceDomain.Records

    // All ice has the same volume so as long as the type matches, we're good
    let RawIceVolume (x:IceType) (y:Compressed) :Volume = Volume <| match x, y with 
        | (BlueIce x),      (IsCompressed)  -> 100.0f * single x
        | (ClearIcicle x),  (IsCompressed)  -> 100.0f * single x
        | (DarkGlitter x),  (IsCompressed)  -> 100.0f * single x
        | (Gelidus x),      (IsCompressed)  -> 100.0f * single x
        | (GlacialMass x),  (IsCompressed)  -> 100.0f * single x
        | (GlareCrust x),   (IsCompressed)  -> 100.0f * single x
        | (Krystallos x),   (IsCompressed)  -> 100.0f * single x
        | (WhiteGlaze x),   (IsCompressed)  -> 100.0f * single x

        | (EnrichedClearIcicle x),  (IsCompressed) -> 100.0f * single x
        | (PristineWhiteGlaze x),   (IsCompressed) -> 100.0f * single x
        | (SmoothGlacialMass x),    (IsCompressed) -> 100.0f * single x
        | (ThickBlueIce x),         (IsCompressed) -> 100.0f * single x

        | (BlueIce x),      (IsNotCompressed)   -> 1000.0f * single x
        | (ClearIcicle x),  (IsNotCompressed)   -> 1000.0f * single x
        | (DarkGlitter x),  (IsNotCompressed)   -> 1000.0f * single x
        | (Gelidus x),      (IsNotCompressed)   -> 1000.0f * single x
        | (GlacialMass x),  (IsNotCompressed)   -> 1000.0f * single x
        | (GlareCrust x),   (IsNotCompressed)   -> 1000.0f * single x
        | (Krystallos x),   (IsNotCompressed)   -> 1000.0f * single x
        | (WhiteGlaze x),   (IsNotCompressed)   -> 1000.0f * single x

        | (EnrichedClearIcicle x),  (IsNotCompressed)   -> 1000.0f * single x
        | (PristineWhiteGlaze x),   (IsNotCompressed)   -> 1000.0f * single x
        | (SmoothGlacialMass x),    (IsNotCompressed)   -> 1000.0f * single x
        | (ThickBlueIce x),         (IsNotCompressed)   -> 1000.0f * single x

        
    let RawIceYield (x:IceType) :IceYield = match x with
        | ClearIcicle qty -> 
            { BaseIceYield with HeavyWater          = HeavyWater          (50   * qty)
                                LiquidOzone         = LiquidOzone         (25   * qty)
                                StrontiumClathrates = StrontiumClathrates (1    * qty)
                                HeliumIsotopes      = HeliumIsotopes      (300  * qty) }
        | EnrichedClearIcicle qty -> 
            { BaseIceYield with HeavyWater          = HeavyWater          (75   * qty)
                                LiquidOzone         = LiquidOzone         (40   * qty)
                                StrontiumClathrates = StrontiumClathrates (1    * qty)
                                HeliumIsotopes      = HeliumIsotopes      (350  * qty) }
        | BlueIce qty -> 
            { BaseIceYield with HeavyWater          = HeavyWater          (50   * qty)
                                LiquidOzone         = LiquidOzone         (25   * qty)
                                StrontiumClathrates = StrontiumClathrates (1    * qty)
                                OxygenIsotopes      = OxygenIsotopes      (300  * qty) }
        | ThickBlueIce qty -> 
            { BaseIceYield with HeavyWater          = HeavyWater          (75   * qty)
                                LiquidOzone         = LiquidOzone         (40   * qty)
                                StrontiumClathrates = StrontiumClathrates (1    * qty)
                                OxygenIsotopes      = OxygenIsotopes      (350  * qty) }
        | GlacialMass qty -> 
            { BaseIceYield with HeavyWater          = HeavyWater          (50   * qty)
                                LiquidOzone         = LiquidOzone         (25   * qty)
                                StrontiumClathrates = StrontiumClathrates (1    * qty)
                                HydrogenIsotopes    = HydrogenIsotopes    (300  * qty) }
        | SmoothGlacialMass qty -> 
            { BaseIceYield with HeavyWater          = HeavyWater          (75   * qty)
                                LiquidOzone         = LiquidOzone         (40   * qty)
                                StrontiumClathrates = StrontiumClathrates (1    * qty)
                                HydrogenIsotopes    = HydrogenIsotopes    (350  * qty) }
        | WhiteGlaze qty -> 
            { BaseIceYield with HeavyWater          = HeavyWater          (50   * qty)
                                LiquidOzone         = LiquidOzone         (25   * qty)
                                StrontiumClathrates = StrontiumClathrates (1    * qty)
                                NitrogenIsotopes    = NitrogenIsotopes    (300  * qty) }
        | PristineWhiteGlaze qty -> 
            { BaseIceYield with HeavyWater          = HeavyWater          (75   * qty)
                                LiquidOzone         = LiquidOzone         (40   * qty)
                                StrontiumClathrates = StrontiumClathrates (1    * qty)
                                NitrogenIsotopes    = NitrogenIsotopes    (350  * qty) }
        | GlareCrust qty -> 
            { BaseIceYield with HeavyWater          = HeavyWater          (1000 * qty)
                                LiquidOzone         = LiquidOzone         (500  * qty)
                                StrontiumClathrates = StrontiumClathrates (25   * qty) }
        | DarkGlitter qty -> 
            { BaseIceYield with HeavyWater          = HeavyWater          (500  * qty)
                                LiquidOzone         = LiquidOzone         (1000 * qty)
                                StrontiumClathrates = StrontiumClathrates (50   * qty) }
        | Gelidus qty -> 
            { BaseIceYield with HeavyWater          = HeavyWater          (250  * qty)
                                LiquidOzone         = LiquidOzone         (500  * qty)
                                StrontiumClathrates = StrontiumClathrates (75   * qty) }
        | Krystallos qty -> 
            { BaseIceYield with HeavyWater          = HeavyWater          (125  * qty)
                                LiquidOzone         = LiquidOzone         (250  * qty)
                                StrontiumClathrates = StrontiumClathrates (125  * qty) }
        

    let IceData (x:IceType) (y:Compressed) :IceData  =
        (fun (x, y, z) -> {
            IceId = IceId (Id x)
            Name  = Name y
            IceQty = IceQty (Qty z)
            }) <| match (x, y) with
                | BlueIce qty,  IsNotCompressed -> 16264, "Blue Ice", qty
                | BlueIce qty,  IsCompressed    -> 28433, "Compressed Blue Ice", qty 

                | ClearIcicle qty,  IsNotCompressed -> 16262, "Clear Icicle", qty
                | ClearIcicle qty,  IsCompressed    -> 28434, "Compressed Clear Icicle", qty

                | DarkGlitter qty,  IsNotCompressed -> 16267, "Dark Glitter", qty
                | DarkGlitter qty,  IsCompressed    -> 28435, "Compressed Dark Glitter", qty

                | EnrichedClearIcicle qty,  IsNotCompressed -> 17978, "Enriched Clear Icicle", qty
                | EnrichedClearIcicle qty,  IsCompressed    -> 28436, "Compressed Enriched Clear Icicle", qty

                | Gelidus qty,  IsNotCompressed -> 16268, "Gelidus", qty
                | Gelidus qty,  IsCompressed    -> 28437, "Compressed Gelidus", qty

                | GlacialMass qty,  IsNotCompressed -> 16263, "Glacial Mass", qty
                | GlacialMass qty,  IsCompressed    -> 28438, "Compressed Glacial Mass", qty

                | GlareCrust qty,  IsNotCompressed -> 16266, "Glare Crust", qty
                | GlareCrust qty,  IsCompressed    -> 28439, "Compressed Glare Crust", qty

                | Krystallos qty,  IsNotCompressed -> 16268, "Krystallos", qty
                | Krystallos qty,  IsCompressed    -> 28440, "Compressed Krystallos", qty

                | PristineWhiteGlaze qty,  IsNotCompressed -> 17976, "Pristine White Glaze", qty
                | PristineWhiteGlaze qty,  IsCompressed    -> 28441, "Compressed Pristine White Glaze", qty

                | SmoothGlacialMass qty,  IsNotCompressed -> 17977, "Smooth Glacial Mass", qty
                | SmoothGlacialMass qty,  IsCompressed    -> 28442, "Compressed Smooth Glacial Mass", qty

                | ThickBlueIce qty,  IsNotCompressed -> 17975, "Thick Blue Ice", qty
                | ThickBlueIce qty,  IsCompressed    -> 28443, "Compressed Thick Blue Ice", qty

                | WhiteGlaze qty,  IsNotCompressed -> 16265, "White Glaze", qty
                | WhiteGlaze qty,  IsCompressed    -> 28444, "Compressed White Glaze", qty
            


    let IceCategoryName (x:IceType) :Name = Name <| match x with 
        | ClearIcicle _         -> "Clear Icicle"
        | EnrichedClearIcicle _ -> "Enriched Clear Icicle"
        | BlueIce _             -> "Blue Ice"
        | ThickBlueIce _        -> "Thick Blue Ice"
        | GlacialMass _         -> "Glacial Mass"
        | SmoothGlacialMass _   -> "Smooth Glacial Mass"
        | WhiteGlaze _          -> "White Glaze"
        | PristineWhiteGlaze _  -> "Pristine White Glaze"
        | GlareCrust _          -> "Glare Crust"
        | DarkGlitter _         -> "Dark Glitter"
        | Gelidus _             -> "Gelidus"
        | Krystallos _          -> "Krystallos"

        
    
    let IceFactory (c:Compressed) (n:IceType) :RawIce=
        let data = IceData (n) (c)
        {
            Name    = data.Name
            IceId   = data.IceId
            Qty     = data.IceQty
            Yield   = RawIceYield n 
            Volume  = RawIceVolume n c
            IceType = n
        }

    let Ice (x:Ice) (q:Qty) :RawIce = 
        match x with 
        | RegularBlueIce         -> (IceType.BlueIce q.Value)        |> IceFactory (IsNotCompressed)
        | RegularClearIcicle     -> (IceType.ClearIcicle q.Value)    |> IceFactory (IsNotCompressed)
        | RegularGlacialMass     -> (IceType.GlacialMass q.Value)    |> IceFactory (IsNotCompressed)
        | RegularWhiteGlaze      -> (IceType.WhiteGlaze q.Value)     |> IceFactory (IsNotCompressed)
        | RegularThickBlueIce          -> (IceType.ThickBlueIce q.Value)           |> IceFactory (IsNotCompressed)
        | RegularEnrichedClearIcicle   -> (IceType.EnrichedClearIcicle q.Value)    |> IceFactory (IsNotCompressed)
        | RegularSmoothGlacialMass     -> (IceType.SmoothGlacialMass q.Value)      |> IceFactory (IsNotCompressed)
        | RegularPristineWhiteGlaze    -> (IceType.PristineWhiteGlaze q.Value)     |> IceFactory (IsNotCompressed)
        | RegularGlareCrust      -> (IceType.GlareCrust q.Value)     |> IceFactory (IsNotCompressed)
        | RegularDarkGlitter     -> (IceType.DarkGlitter q.Value)    |> IceFactory (IsNotCompressed)
        | RegularGelidus         -> (IceType.Gelidus q.Value)        |> IceFactory (IsNotCompressed)
        | RegularKrystallos      -> (IceType.Krystallos q.Value)     |> IceFactory (IsNotCompressed)
        | CompressedBlueIce      -> (IceType.BlueIce q.Value)        |> IceFactory (IsCompressed)
        | CompressedClearIcicle  -> (IceType.ClearIcicle q.Value)    |> IceFactory (IsCompressed)
        | CompressedGlacialMass  -> (IceType.GlacialMass q.Value)    |> IceFactory (IsCompressed)
        | CompressedWhiteGlaze   -> (IceType.WhiteGlaze q.Value)     |> IceFactory (IsCompressed)
        | CompressedThickBlueIce         -> (IceType.ThickBlueIce q.Value)           |> IceFactory (IsCompressed)
        | CompressedEnrichedClearIcicle  -> (IceType.EnrichedClearIcicle q.Value)    |> IceFactory (IsCompressed)
        | CompressedSmoothGlacialMass    -> (IceType.SmoothGlacialMass q.Value)      |> IceFactory (IsCompressed)
        | CompressedPristineWhiteGlaze   -> (IceType.PristineWhiteGlaze q.Value)     |> IceFactory (IsCompressed)
        | CompressedGlareCrust   -> (IceType.GlareCrust q.Value)     |> IceFactory (IsCompressed)
        | CompressedDarkGlitter  -> (IceType.DarkGlitter q.Value)    |> IceFactory (IsCompressed)
        | CompressedGelidus      -> (IceType.Gelidus q.Value)        |> IceFactory (IsCompressed)
        | CompressedKrystallos   -> (IceType.Krystallos q.Value)     |> IceFactory (IsCompressed) 
    
