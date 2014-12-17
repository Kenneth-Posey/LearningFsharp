namespace EveData

module RawMaterial = 
    open EveData.RawMaterialTypes
    open EveData.RawMaterialRecords
    
    let MinimumRefineQty (x:OreType) :int = match x with
        | Arkonor _     -> 200
        | Bistot _      -> 200
        | Crokite _     -> 1  
        | DarkOchre _   -> 1 
        | Gneiss _      -> 1    
        | Hedbergite _  -> 1
        | Hemorphite _  -> 1
        | Jaspet _      -> 1    
        | Kernite _     -> 1   
        | Mercoxit _    -> 1  
        | Omber _       -> 1     
        | Plagioclase _ -> 1
        | Pyroxeres _   -> 1 
        | Scordite _    -> 1  
        | Spodumain _   -> 1 
        | Veldspar _    -> 1  

    let RawOreYield (ore:OreType) :OreYield = 
        let qty x t = (x - (x % (MinimumRefineQty t)) / (x % (MinimumRefineQty t)))
        match ore with
        | Arkonor q      -> qty q ore |> fun qty ->
                            { BaseOreYield with Tritanium = Tritanium (6905 * qty)
                                                Mexallon  = Mexallon  (1278 * qty)
                                                Megacyte  = Megacyte  (230  * qty)
                                                Zydrine   = Zydrine   (115  * qty) }
        | Bistot q       -> qty q ore |> fun qty ->
                            { BaseOreYield with Pyerite   = Pyerite   (16572 * qty)
                                                Megacyte  = Megacyte  (118   * qty)
                                                Zydrine   = Zydrine   (236   * qty) }
        | Crokite q      -> qty q ore |> fun qty ->
                            { BaseOreYield with Tritanium = Tritanium (20992 * qty)
                                                Nocxium   = Nocxium   (275   * qty)
                                                Zydrine   = Zydrine   (367   * qty) }
        | DarkOchre q    -> qty q ore |> fun qty ->
                            { BaseOreYield with Tritanium = Tritanium (8804 * qty)
                                                Nocxium   = Nocxium   (173  * qty)
                                                Zydrine   = Zydrine   (87   * qty) }
        | Gneiss q       -> qty q ore |> fun qty ->
                            { BaseOreYield with Tritanium = Tritanium (1278 * qty)
                                                Mexallon  = Mexallon  (1278 * qty)
                                                Isogen    = Isogen    (242  * qty)
                                                Zydrine   = Zydrine   (60   * qty) }
        | Hedbergite q   -> qty q ore |> fun qty ->
                            { BaseOreYield with Tritanium = Tritanium (180 * qty)
                                                Pyerite   = Pyerite   (72  * qty)
                                                Mexallon  = Mexallon  (17  * qty)
                                                Isogen    = Isogen    (59  * qty)
                                                Nocxium   = Nocxium   (118 * qty)
                                                Zydrine   = Zydrine   (8   * qty) }
        | Hemorphite q   -> qty q ore |> fun qty ->
                            { BaseOreYield with Pyerite   = Pyerite   (81  * qty)
                                                Isogen    = Isogen    (196 * qty)
                                                Nocxium   = Nocxium   (98  * qty)
                                                Zydrine   = Zydrine   (9   * qty) }
        | Jaspet q       -> qty q ore |> fun qty ->
                            { BaseOreYield with Tritanium = Tritanium (72  * qty)
                                                Pyerite   = Pyerite   (121 * qty)
                                                Mexallon  = Mexallon  (144 * qty)
                                                Nocxium   = Nocxium   (72  * qty)
                                                Zydrine   = Zydrine   (3   * qty) }
        | Kernite q      -> qty q ore |> fun qty ->
                            { BaseOreYield with Tritanium = Tritanium (134 * qty)
                                                Mexallon  = Mexallon  (267 * qty)
                                                Isogen    = Isogen    (134 * qty) }
        | Mercoxit q     -> qty q ore |> fun qty ->
                            { BaseOreYield with Morphite  = Morphite  (293 * qty) }
        | Omber q        -> qty q ore |> fun qty ->
                            { BaseOreYield with Tritanium = Tritanium (85 * qty)
                                                Pyerite   = Pyerite   (34 * qty)
                                                Isogen    = Isogen    (85 * qty) }
        | Plagioclase q  -> qty q ore |> fun qty -> 
                            { BaseOreYield with Tritanium = Tritanium (107 * qty)
                                                Pyerite   = Pyerite   (213 * qty)
                                                Mexallon  = Mexallon  (107 * qty) }
        | Pyroxeres q    -> qty q ore |> fun qty -> 
                            { BaseOreYield with Tritanium = Tritanium (315 * qty)
                                                Pyerite   = Pyerite   (25  * qty)
                                                Mexallon  = Mexallon  (50  * qty)
                                                Nocxium   = Nocxium   (5   * qty) }
        | Scordite q     -> qty q ore |> fun qty -> 
                            { BaseOreYield with Tritanium = Tritanium (346 * qty)
                                                Pyerite   = Pyerite   (173 * qty) }
        | Spodumain q    -> qty q ore |> fun qty ->
                            { BaseOreYield with Tritanium = Tritanium (39221 * qty)
                                                Pyerite   = Pyerite   (4972  * qty)
                                                Megacyte  = Megacyte  (78    * qty) }
        | Veldspar q     -> qty q ore |> fun qty -> 
                            { BaseOreYield with Tritanium = Tritanium (415 * qty) }


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
        

    let RawOreVolume (x:OreType) (y:Compressed) :Volume = Volume <| match x, y with
        | (Veldspar x),     (IsNotCompressed)  -> 0.1f * single x
        | (Veldspar x),     (IsCompressed)     -> 0.15f * single x
        | (Scordite x),     (IsNotCompressed)  -> 0.15f * single x
        | (Scordite x),     (IsCompressed)     -> 0.19f * single x
        | (Pyroxeres x),    (IsNotCompressed)  -> 0.3f * single x
        | (Pyroxeres x),    (IsCompressed)     -> 0.16f * single x
        | (Plagioclase x),  (IsNotCompressed)  -> 0.35f * single x
        | (Plagioclase x),  (IsCompressed)     -> 0.15f * single x
        | (Omber x),        (IsNotCompressed)  -> 0.6f * single x
        | (Omber x),        (IsCompressed)     -> 0.07f * single x
        | (Kernite x),      (IsNotCompressed)  -> 1.2f * single x
        | (Kernite x),      (IsCompressed)     -> 0.19f * single x
        | (Jaspet x),       (IsNotCompressed)  -> 2.0f * single x
        | (Jaspet x),       (IsCompressed)     -> 0.15f * single x
        | (Hedbergite x),   (IsNotCompressed)  -> 3.0f * single x
        | (Hedbergite x),   (IsCompressed)     -> 0.14f * single x
        | (Hemorphite x),   (IsNotCompressed)  -> 3.0f * single x
        | (Hemorphite x),   (IsCompressed)     -> 0.16f * single x
        | (Gneiss x),       (IsNotCompressed)  -> 5.0f * single x
        | (Gneiss x),       (IsCompressed)     -> 1.03f * single x
        | (DarkOchre x),    (IsNotCompressed)  -> 8.0f * single x
        | (DarkOchre x),    (IsCompressed)     -> 3.27f * single x
        | (Spodumain x),    (IsNotCompressed)  -> 16.0f * single x
        | (Spodumain x),    (IsCompressed)     -> 16.0f * single x
        | (Arkonor x),      (IsNotCompressed)  -> 16.0f * single x
        | (Arkonor x),      (IsCompressed)     -> 3.08f * single x
        | (Crokite x),      (IsNotCompressed)  -> 16.0f * single x
        | (Crokite x),      (IsCompressed)     -> 7.81f * single x
        | (Bistot x),       (IsNotCompressed)  -> 16.0f * single x
        | (Bistot x),       (IsCompressed)     -> 6.11f * single x
        | (Mercoxit x),     (IsNotCompressed)  -> 40.0f * single x
        | (Mercoxit x),     (IsCompressed)     -> 0.1f * single x


    // All ice has the same volume so as long as the type matches, we're good
    let RawIceVolume (x:IceType) (y:Compressed) :Volume = Volume <| match x, y with 
        | (BlueIce x),              (IsCompressed)      -> 1000.0f * single x
        | (BlueIce x),              (IsNotCompressed)   -> 100.0f * single x
        | (ClearIcicle x),          (IsCompressed)      -> 1000.0f * single x
        | (ClearIcicle x),          (IsNotCompressed)   -> 100.0f * single x
        | (DarkGlitter x),          (IsCompressed)      -> 1000.0f * single x
        | (DarkGlitter x),          (IsNotCompressed)   -> 100.0f * single x
        | (EnrichedClearIcicle x),  (IsCompressed)      -> 1000.0f * single x
        | (EnrichedClearIcicle x),  (IsNotCompressed)   -> 100.0f * single x
        | (Gelidus x),              (IsCompressed)      -> 1000.0f * single x
        | (Gelidus x),              (IsNotCompressed)   -> 100.0f * single x
        | (GlacialMass x),          (IsCompressed)      -> 1000.0f * single x
        | (GlacialMass x),          (IsNotCompressed)   -> 100.0f * single x
        | (GlareCrust x),           (IsCompressed)      -> 1000.0f * single x
        | (GlareCrust x),           (IsNotCompressed)   -> 100.0f * single x
        | (Krystallos x),           (IsCompressed)      -> 1000.0f * single x
        | (Krystallos x),           (IsNotCompressed)   -> 100.0f * single x
        | (PristineWhiteGlaze x),   (IsCompressed)      -> 1000.0f * single x
        | (PristineWhiteGlaze x),   (IsNotCompressed)   -> 100.0f * single x
        | (SmoothGlacialMass x),    (IsCompressed)      -> 1000.0f * single x
        | (SmoothGlacialMass x),    (IsNotCompressed)   -> 100.0f * single x
        | (ThickBlueIce x),         (IsCompressed)      -> 1000.0f * single x
        | (ThickBlueIce x),         (IsNotCompressed)   -> 100.0f * single x
        | (WhiteGlaze x),           (IsCompressed)      -> 1000.0f * single x
        | (WhiteGlaze x),           (IsNotCompressed)   -> 100.0f * single x


    let OreCategoryName (x:OreType) :Name = Name <| match x with
        | Veldspar _    -> "Veldspar"
        | Scordite _    -> "Scordite"
        | Pyroxeres _   -> "Pyroxeres"
        | Plagioclase _ -> "Plagioclase"
        | Omber _       -> "Omber"
        | Kernite _     -> "Kernite"
        | Jaspet _      -> "Jaspet"
        | Hedbergite _  -> "Hedbergite"
        | Hemorphite _  -> "Hemorphite"
        | Gneiss _      -> "Gneiss"
        | DarkOchre _   -> "Dark Ochre"
        | Spodumain _   -> "Spodumain"
        | Arkonor _     -> "Arkonor"
        | Crokite _     -> "Crokite"
        | Bistot _      -> "Bistot"
        | Mercoxit _    -> "Mercoxit"     


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


    let OreData (x:OreType) (y:OreRarity) (z:Compressed) :OreData = 
        (fun (x, y, z) -> { 
            OreId  = OreId (Id x)
            Name   = Name y
            OreQty = OreQty (Qty z)
            }) <| match (x, y, z) with
                | Arkonor qty,  Common,     IsNotCompressed   -> 22, "Arkonor", qty     
                | Arkonor qty,  Uncommon,   IsNotCompressed   -> 17425, "Crimson Arkonor", qty 
                | Arkonor qty,  Rare,       IsNotCompressed   -> 17426, "Prime Arkonor", qty 
                | Arkonor qty,  Common,     IsCompressed      -> 28367, "Compressed Arkonor", qty 
                | Arkonor qty,  Uncommon,   IsCompressed      -> 28385, "Compressed Crimson Arkonor", qty 
                | Arkonor qty,  Rare,       IsCompressed      -> 28387, "Compressed Prime Arkonor", qty 
                             
                | Bistot qty,  Common,     IsNotCompressed   -> 1223, "Bistot", qty 
                | Bistot qty,  Uncommon,   IsNotCompressed   -> 17428, "Triclinic Bistot", qty 
                | Bistot qty,  Rare,       IsNotCompressed   -> 17429, "Monoclinic Bistot", qty 
                | Bistot qty,  Common,     IsCompressed      -> 28388, "Compressed Bistot", qty 
                | Bistot qty,  Uncommon,   IsCompressed      -> 28389, "Compressed Triclinic Bistot", qty 
                | Bistot qty,  Rare,       IsCompressed      -> 28390, "Compressed Monoclinic Bistot", qty 
                               
                | Crokite qty,  Common,    IsNotCompressed   -> 1225, "Crokite", qty 
                | Crokite qty,  Uncommon,  IsNotCompressed   -> 17432, "Sharp Crokite", qty 
                | Crokite qty,  Rare,      IsNotCompressed   -> 17433, "Crystalline Crokite", qty 
                | Crokite qty,  Common,    IsCompressed      -> 28391, "Compressed Crokite", qty 
                | Crokite qty,  Uncommon,  IsCompressed      -> 28392, "Compressed Sharp Crokite", qty 
                | Crokite qty,  Rare,      IsCompressed      -> 28393, "Compressed Crystalline Crokite", qty 
                                    
                | DarkOchre qty,  Common,    IsNotCompressed   -> 1232, "Dark Ochre", qty 
                | DarkOchre qty,  Uncommon,  IsNotCompressed   -> 17436, "Onyx Ochre", qty 
                | DarkOchre qty,  Rare,      IsNotCompressed   -> 17437, "Obsidian Ochre", qty 
                | DarkOchre qty,  Common,    IsCompressed      -> 28394, "Compressed Dark Ochre", qty 
                | DarkOchre qty,  Uncommon,  IsCompressed      -> 28395, "Compressed Onyx Ochre", qty 
                | DarkOchre qty,  Rare ,     IsCompressed      -> 28396, "Compressed Obsidian Ochre", qty 
                                    
                | Gneiss qty,  Common,    IsNotCompressed   -> 1229, "Gneiss", qty 
                | Gneiss qty,  Uncommon,  IsNotCompressed   -> 17865, "Iridescent Gneiss", qty 
                | Gneiss qty,  Rare,      IsNotCompressed   -> 17866, "Prismatic Gneiss", qty 
                | Gneiss qty,  Common,    IsCompressed      -> 28397, "Compressed Gneiss", qty 
                | Gneiss qty,  Uncommon,  IsCompressed      -> 28398, "Compressed Iridescent Gneiss", qty 
                | Gneiss qty,  Rare,      IsCompressed      -> 28399, "Compressed Prismatic Gneiss", qty 
                                    
                | Hedbergite qty,  Common,    IsNotCompressed   -> 21, "Hedbergite", qty 
                | Hedbergite qty,  Uncommon,  IsNotCompressed   -> 17440, "Vitric Hedbergite", qty 
                | Hedbergite qty,  Rare,      IsNotCompressed   -> 17441, "Glazed Hedbergite", qty 
                | Hedbergite qty,  Common,    IsCompressed      -> 28400, "Compressed Hedbergite", qty 
                | Hedbergite qty,  Uncommon,  IsCompressed      -> 28401, "Compressed Vitric Hedbergite", qty 
                | Hedbergite qty,  Rare,      IsCompressed      -> 28402, "Compressed Glazed Hedbergite", qty 
                                    
                | Hemorphite qty,  Common,    IsNotCompressed   -> 1231, "Hemorphite", qty 
                | Hemorphite qty,  Uncommon,  IsNotCompressed   -> 17444, "Vivid Hemorphite", qty 
                | Hemorphite qty,  Rare,      IsNotCompressed   -> 17445, "Radiant Hemorphite", qty 
                | Hemorphite qty,  Common,    IsCompressed      -> 28403, "Compressed Hemorphite", qty 
                | Hemorphite qty,  Uncommon,  IsCompressed      -> 28404, "Compressed Vivid Hemorphite", qty 
                | Hemorphite qty,  Rare,      IsCompressed      -> 28405, "Compressed Radiant Hemorphite", qty 
                                    
                | Jaspet qty,  Common,    IsNotCompressed   -> 1226, "Jaspet", qty 
                | Jaspet qty,  Uncommon,  IsNotCompressed   -> 17448, "Pure Jaspet", qty 
                | Jaspet qty,  Rare,      IsNotCompressed   -> 17449, "Pristine Jaspet", qty 
                | Jaspet qty,  Common,    IsCompressed      -> 28406, "Compressed Jaspet", qty 
                | Jaspet qty,  Uncommon,  IsCompressed      -> 28407, "Compressed Pure Jaspet", qty 
                | Jaspet qty,  Rare,      IsCompressed      -> 28408, "Compressed Pristine Jaspet", qty 
                                    
                | Kernite qty,  Common,    IsNotCompressed   -> 20, "Kernite", qty 
                | Kernite qty,  Uncommon,  IsNotCompressed   -> 17452, "Luminous Kernite", qty 
                | Kernite qty,  Rare,      IsNotCompressed   -> 17453, "Fiery Kernite", qty 
                | Kernite qty,  Common,    IsCompressed      -> 28410, "Compressed Kernite", qty 
                | Kernite qty,  Uncommon,  IsCompressed      -> 28411, "Compressed Luminous Kernite", qty 
                | Kernite qty,  Rare,      IsCompressed      -> 28409, "Compressed Fiery Kernite", qty 
                                    
                | Mercoxit qty,  Common,    IsNotCompressed   -> 11396, "Mercoxit", qty 
                | Mercoxit qty,  Uncommon,  IsNotCompressed   -> 17869, "Magma Mercoxit", qty 
                | Mercoxit qty,  Rare,      IsNotCompressed   -> 17870, "Vitreous Mercoxit", qty 
                | Mercoxit qty,  Common,    IsCompressed      -> 28413, "Compressed Mercoxit", qty 
                | Mercoxit qty,  Uncommon,  IsCompressed      -> 28412, "Compressed Magma Mercoxit", qty 
                | Mercoxit qty,  Rare,      IsCompressed      -> 28414, "Compressed Vitreous Mercoxit", qty 
                                    
                | Omber qty,  Common,    IsNotCompressed   -> 1227, "Omber", qty 
                | Omber qty,  Uncommon,  IsNotCompressed   -> 17867, "Silvery Omber", qty 
                | Omber qty,  Rare,      IsNotCompressed   -> 17868, "Golden Omber", qty 
                | Omber qty,  Common,    IsCompressed      -> 28416, "Compressed Omber", qty 
                | Omber qty,  Uncommon,  IsCompressed      -> 28417, "Compressed Silvery Omber", qty 
                | Omber qty,  Rare,      IsCompressed      -> 28415, "Compressed Golden Omber", qty 
                                    
                | Plagioclase qty,  Common,    IsNotCompressed   -> 18, "Plagioclase", qty 
                | Plagioclase qty,  Uncommon,  IsNotCompressed   -> 17455, "Azure Plagioclase", qty 
                | Plagioclase qty,  Rare,      IsNotCompressed   -> 17456, "Rich Plagioclase", qty 
                | Plagioclase qty,  Common,    IsCompressed      -> 28422, "Compressed Plagioclase", qty 
                | Plagioclase qty,  Uncommon,  IsCompressed      -> 28421, "Compressed Azure Plagioclase", qty 
                | Plagioclase qty,  Rare,      IsCompressed      -> 28423, "Compressed Rich Plagioclase", qty 
                                    
                | Pyroxeres qty,  Common,    IsNotCompressed   -> 1224, "Pyroxeres", qty 
                | Pyroxeres qty,  Uncommon,  IsNotCompressed   -> 17459, "Solid Pyroxeres", qty 
                | Pyroxeres qty,  Rare,      IsNotCompressed   -> 17460, "Viscous Pyroxeres", qty 
                | Pyroxeres qty,  Common,    IsCompressed      -> 28424, "Compressed Pyroxeres", qty
                | Pyroxeres qty,  Uncommon,  IsCompressed      -> 28425, "Compressed Solid Pyroxeres", qty
                | Pyroxeres qty,  Rare,      IsCompressed      -> 28426, "Compressed Viscous Pyroxeres", qty
                                    
                | Scordite qty,  Common,     IsNotCompressed   -> 1228, "Scordite", qty 
                | Scordite qty,  Uncommon,   IsNotCompressed   -> 17463, "Condensed Scordite", qty
                | Scordite qty,  Rare,       IsNotCompressed   -> 17464, "Massive Scordite", qty  
                | Scordite qty,  Common,     IsCompressed      -> 28427, "Compressed Scordite", qty 
                | Scordite qty,  Uncommon,   IsCompressed      -> 28428, "Compressed Condensed Scordite", qty 
                | Scordite qty,  Rare,       IsCompressed      -> 28429, "Compressed Massive Scordite", qty 
                                    
                | Spodumain qty,  Common,    IsNotCompressed   -> 19, "Spodumain", qty 
                | Spodumain qty,  Uncommon,  IsNotCompressed   -> 17466, "Bright Spodumain", qty 
                | Spodumain qty,  Rare,      IsNotCompressed   -> 17467, "Gleaming Spodumain", qty 
                | Spodumain qty,  Common,    IsCompressed      -> 28420, "Compressed Spodumain", qty 
                | Spodumain qty,  Uncommon,  IsCompressed      -> 28418, "Compressed Bright Spodumain", qty 
                | Spodumain qty,  Rare,      IsCompressed      -> 28419, "Compressed Gleaming Spodumain", qty 
                                    
                | Veldspar qty,  Common,    IsNotCompressed   -> 1230, "Veldspar", qty 
                | Veldspar qty,  Uncommon,  IsNotCompressed   -> 17470, "Concentrated Veldspar", qty 
                | Veldspar qty,  Rare,      IsNotCompressed   -> 17471, "Dense Veldspar", qty 
                | Veldspar qty,  Common,    IsCompressed      -> 28430, "Compressed Veldspar", qty 
                | Veldspar qty,  Uncommon,  IsCompressed      -> 28431, "Compressed Concentrated Veldspar", qty
                | Veldspar qty,  Rare,      IsCompressed      -> 28432, "Compressed Dense Veldspar", qty 
            

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
            

