namespace EveOnline.OreDomain

module Ore = 
    open EveOnline.ProductDomain.Types
    open EveOnline.OreDomain.Types
    open EveOnline.OreDomain.Records
    
    let RawOreVolume (x:OreType) (y:Compressed) :Volume = Volume <| match x, y with
        | (Arkonor x),      (IsNotCompressed)  -> 16.0f * single x
        | (Arkonor x),      (IsCompressed)     -> 3.08f * single x

        | (Bistot x),       (IsNotCompressed)  -> 16.0f * single x
        | (Bistot x),       (IsCompressed)     -> 6.11f * single x

        | (Crokite x),      (IsNotCompressed)  -> 16.0f * single x
        | (Crokite x),      (IsCompressed)     -> 7.81f * single x

        | (DarkOchre x),    (IsNotCompressed)  -> 8.00f * single x
        | (DarkOchre x),    (IsCompressed)     -> 3.27f * single x

        | (Gneiss x),       (IsNotCompressed)  -> 5.00f * single x
        | (Gneiss x),       (IsCompressed)     -> 1.03f * single x

        | (Hedbergite x),   (IsNotCompressed)  -> 3.00f * single x
        | (Hedbergite x),   (IsCompressed)     -> 0.14f * single x

        | (Hemorphite x),   (IsNotCompressed)  -> 3.00f * single x
        | (Hemorphite x),   (IsCompressed)     -> 0.16f * single x

        | (Jaspet x),       (IsNotCompressed)  -> 2.00f * single x
        | (Jaspet x),       (IsCompressed)     -> 0.15f * single x

        | (Kernite x),      (IsNotCompressed)  -> 1.20f * single x
        | (Kernite x),      (IsCompressed)     -> 0.19f * single x

        | (Mercoxit x),     (IsNotCompressed)  -> 40.0f * single x
        | (Mercoxit x),     (IsCompressed)     -> 0.10f * single x

        | (Omber x),        (IsNotCompressed)  -> 0.60f * single x
        | (Omber x),        (IsCompressed)     -> 0.07f * single x

        | (Plagioclase x),  (IsNotCompressed)  -> 0.35f * single x
        | (Plagioclase x),  (IsCompressed)     -> 0.15f * single x

        | (Pyroxeres x),    (IsNotCompressed)  -> 0.30f * single x
        | (Pyroxeres x),    (IsCompressed)     -> 0.16f * single x

        | (Scordite x),     (IsNotCompressed)  -> 0.15f * single x
        | (Scordite x),     (IsCompressed)     -> 0.19f * single x

        | (Spodumain x),    (IsNotCompressed)  -> 16.0f * single x
        | (Spodumain x),    (IsCompressed)     -> 16.0f * single x 

        | (Veldspar x),     (IsNotCompressed)  -> 0.10f * single x
        | (Veldspar x),     (IsCompressed)     -> 0.15f * single x

    let MinimumRefineQty (x:OreType) :int = match x with
        | Arkonor _     -> 100
        | Bistot _      -> 100
        | Crokite _     -> 100
        | DarkOchre _   -> 100
        | Gneiss _      -> 100  
        | Hedbergite _  -> 100
        | Hemorphite _  -> 100
        | Jaspet _      -> 100  
        | Kernite _     -> 100 
        | Mercoxit _    -> 100
        | Omber _       -> 100   
        | Plagioclase _ -> 100
        | Pyroxeres _   -> 100
        | Scordite _    -> 100
        | Spodumain _   -> 100
        | Veldspar _    -> 100

    let RawOreYield (ore:OreType) :OreYield = 
        let qty x t = (x - (x % (MinimumRefineQty t)) / (x % (MinimumRefineQty t)))
        match ore with
        | Arkonor q     -> qty q ore |> fun qty ->
                           { BaseOreYield with Tritanium = Tritanium (6905 * qty)
                                               Mexallon  = Mexallon  (1278 * qty)
                                               Megacyte  = Megacyte  (230  * qty)
                                               Zydrine   = Zydrine   (115  * qty) }
        | Bistot q      -> qty q ore |> fun qty ->
                           { BaseOreYield with Pyerite   = Pyerite   (16572 * qty)
                                               Megacyte  = Megacyte  (118   * qty)
                                               Zydrine   = Zydrine   (236   * qty) }
        | Crokite q     -> qty q ore |> fun qty ->
                           { BaseOreYield with Tritanium = Tritanium (20992 * qty)
                                               Nocxium   = Nocxium   (275   * qty)
                                               Zydrine   = Zydrine   (367   * qty) }
        | DarkOchre q   -> qty q ore |> fun qty ->
                           { BaseOreYield with Tritanium = Tritanium (8804 * qty)
                                               Nocxium   = Nocxium   (173  * qty)
                                               Zydrine   = Zydrine   (87   * qty) }
        | Gneiss q      -> qty q ore |> fun qty ->
                           { BaseOreYield with Tritanium = Tritanium (1278 * qty)
                                               Mexallon  = Mexallon  (1278 * qty)
                                               Isogen    = Isogen    (242  * qty)
                                               Zydrine   = Zydrine   (60   * qty) }
        | Hedbergite q  -> qty q ore |> fun qty ->
                           { BaseOreYield with Tritanium = Tritanium (180 * qty)
                                               Pyerite   = Pyerite   (72  * qty)
                                               Mexallon  = Mexallon  (17  * qty)
                                               Isogen    = Isogen    (59  * qty)
                                               Nocxium   = Nocxium   (118 * qty)
                                               Zydrine   = Zydrine   (8   * qty) }
        | Hemorphite q  -> qty q ore |> fun qty ->
                           { BaseOreYield with Pyerite   = Pyerite   (81  * qty)
                                               Isogen    = Isogen    (196 * qty)
                                               Nocxium   = Nocxium   (98  * qty)
                                               Zydrine   = Zydrine   (9   * qty) }
        | Jaspet q      -> qty q ore |> fun qty ->
                           { BaseOreYield with Tritanium = Tritanium (72  * qty)
                                               Pyerite   = Pyerite   (121 * qty)
                                               Mexallon  = Mexallon  (144 * qty)
                                               Nocxium   = Nocxium   (72  * qty)
                                               Zydrine   = Zydrine   (3   * qty) }
        | Kernite q     -> qty q ore |> fun qty ->
                           { BaseOreYield with Tritanium = Tritanium (134 * qty)
                                               Mexallon  = Mexallon  (267 * qty)
                                               Isogen    = Isogen    (134 * qty) }
        | Mercoxit q    -> qty q ore |> fun qty ->
                           { BaseOreYield with Morphite  = Morphite  (293 * qty) }
        | Omber q       -> qty q ore |> fun qty ->
                           { BaseOreYield with Tritanium = Tritanium (85 * qty)
                                               Pyerite   = Pyerite   (34 * qty)
                                               Isogen    = Isogen    (85 * qty) }
        | Plagioclase q -> qty q ore |> fun qty -> 
                           { BaseOreYield with Tritanium = Tritanium (107 * qty)
                                               Pyerite   = Pyerite   (213 * qty)
                                               Mexallon  = Mexallon  (107 * qty) }
        | Pyroxeres q   -> qty q ore |> fun qty -> 
                           { BaseOreYield with Tritanium = Tritanium (315 * qty)
                                               Pyerite   = Pyerite   (25  * qty)
                                               Mexallon  = Mexallon  (50  * qty)
                                               Nocxium   = Nocxium   (5   * qty) }
        | Scordite q    -> qty q ore |> fun qty -> 
                           { BaseOreYield with Tritanium = Tritanium (346 * qty)
                                               Pyerite   = Pyerite   (173 * qty) }
        | Spodumain q   -> qty q ore |> fun qty ->
                           { BaseOreYield with Tritanium = Tritanium (39221 * qty)
                                               Pyerite   = Pyerite   (4972  * qty)
                                               Megacyte  = Megacyte  (78    * qty) }
        | Veldspar q    -> qty q ore |> fun qty -> 
                            { BaseOreYield with Tritanium = Tritanium (415 * qty) }


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


    let OreFactory (r:OreRarity) (c:Compressed) (n:OreType) :RawOre= 
        OreData (n) (r) (c) |> fun x -> 
        {
            Name    = x.Name
            OreId   = x.OreId
            Qty     = x.OreQty
            Yield   = RawOreYield n 
            Volume  = RawOreVolume n c
            OreType = n 
        }


    let Ore (x:Ore) (q:Qty) :RawOre=         
        match x with 
        | CommonArkonor       -> (Arkonor q.Value)     |> OreFactory (Common)   (IsNotCompressed)
        | UncommonArkonor     -> (Arkonor q.Value)     |> OreFactory (Uncommon) (IsNotCompressed)
        | RareArkonor         -> (Arkonor q.Value)     |> OreFactory (Rare)     (IsNotCompressed)
        | CommonBistot        -> (Bistot q.Value)      |> OreFactory (Common)   (IsNotCompressed)
        | UncommonBistot      -> (Bistot q.Value)      |> OreFactory (Uncommon) (IsNotCompressed)
        | RareBistot          -> (Bistot q.Value)      |> OreFactory (Rare)     (IsNotCompressed)
        | CommonCrokite       -> (Crokite q.Value)     |> OreFactory (Common)   (IsNotCompressed)
        | UncommonCrokite     -> (Crokite q.Value)     |> OreFactory (Uncommon) (IsNotCompressed)
        | RareCrokite         -> (Crokite q.Value)     |> OreFactory (Rare)     (IsNotCompressed)
        | CommonDarkOchre     -> (DarkOchre q.Value)   |> OreFactory (Common)   (IsNotCompressed)
        | UncommonDarkOchre   -> (DarkOchre q.Value)   |> OreFactory (Uncommon) (IsNotCompressed)
        | RareDarkOchre       -> (DarkOchre q.Value)   |> OreFactory (Rare)     (IsNotCompressed)
        | CommonGneiss        -> (Gneiss q.Value)      |> OreFactory (Common)   (IsNotCompressed)
        | UncommonGneiss      -> (Gneiss q.Value)      |> OreFactory (Uncommon) (IsNotCompressed)
        | RareGneiss          -> (Gneiss q.Value)      |> OreFactory (Rare)     (IsNotCompressed)
        | CommonHedbergite    -> (Hedbergite q.Value)  |> OreFactory (Common)   (IsNotCompressed)
        | UncommonHedbergite  -> (Hedbergite q.Value)  |> OreFactory (Uncommon) (IsNotCompressed)
        | RareHedbergite      -> (Hedbergite q.Value)  |> OreFactory (Rare)     (IsNotCompressed)
        | CommonHemorphite    -> (Hemorphite q.Value)  |> OreFactory (Common)   (IsNotCompressed)
        | UncommonHemorphite  -> (Hemorphite q.Value)  |> OreFactory (Uncommon) (IsNotCompressed)
        | RareHemorphite      -> (Hemorphite q.Value)  |> OreFactory (Rare)     (IsNotCompressed)
        | CommonJaspet        -> (Jaspet q.Value)      |> OreFactory (Common)   (IsNotCompressed)
        | UncommonJaspet      -> (Jaspet q.Value)      |> OreFactory (Uncommon) (IsNotCompressed)
        | RareJaspet          -> (Jaspet q.Value)      |> OreFactory (Rare)     (IsNotCompressed)
        | CommonKernite       -> (Kernite q.Value)     |> OreFactory (Common)   (IsNotCompressed)
        | UncommonKernite     -> (Kernite q.Value)     |> OreFactory (Uncommon) (IsNotCompressed)
        | RareKernite         -> (Kernite q.Value)     |> OreFactory (Rare)     (IsNotCompressed)
        | CommonMercoxit      -> (Mercoxit q.Value)    |> OreFactory (Common)   (IsNotCompressed)
        | UncommonMercoxit    -> (Mercoxit q.Value)    |> OreFactory (Uncommon) (IsNotCompressed)
        | RareMercoxit        -> (Mercoxit q.Value)    |> OreFactory (Rare)     (IsNotCompressed)
        | CommonOmber         -> (Omber q.Value)       |> OreFactory (Common)   (IsNotCompressed)
        | UncommonOmber       -> (Omber q.Value)       |> OreFactory (Uncommon) (IsNotCompressed)
        | RareOmber           -> (Omber q.Value)       |> OreFactory (Rare)     (IsNotCompressed)
        | CommonPlagioclase   -> (Plagioclase q.Value) |> OreFactory (Common)   (IsNotCompressed)
        | UncommonPlagioclase -> (Plagioclase q.Value) |> OreFactory (Uncommon) (IsNotCompressed)
        | RarePlagioclase     -> (Plagioclase q.Value) |> OreFactory (Rare)     (IsNotCompressed)
        | CommonPyroxeres     -> (Pyroxeres q.Value)   |> OreFactory (Common)   (IsNotCompressed)
        | UncommonPyroxeres   -> (Pyroxeres q.Value)   |> OreFactory (Uncommon) (IsNotCompressed)
        | RarePyroxeres       -> (Pyroxeres q.Value)   |> OreFactory (Rare)     (IsNotCompressed)
        | CommonScordite      -> (Scordite q.Value)    |> OreFactory (Common)   (IsNotCompressed)
        | UncommonScordite    -> (Scordite q.Value)    |> OreFactory (Uncommon) (IsNotCompressed)
        | RareScordite        -> (Scordite q.Value)    |> OreFactory (Rare)     (IsNotCompressed)
        | CommonSpodumain     -> (Spodumain q.Value)   |> OreFactory (Common)   (IsNotCompressed)
        | UncommonSpodumain   -> (Spodumain q.Value)   |> OreFactory (Uncommon) (IsNotCompressed)
        | RareSpodumain       -> (Spodumain q.Value)   |> OreFactory (Rare)     (IsNotCompressed)
        | CommonVeldspar      -> (Veldspar q.Value)    |> OreFactory (Common)   (IsNotCompressed)
        | UncommonVeldspar    -> (Veldspar q.Value)    |> OreFactory (Uncommon) (IsNotCompressed)
        | RareVeldspar        -> (Veldspar q.Value)    |> OreFactory (Rare)     (IsNotCompressed)
        | CompressedCommonHedbergite    -> (Hedbergite q.Value)  |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonHedbergite  -> (Hedbergite q.Value)  |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRareHedbergite      -> (Hedbergite q.Value)  |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonHemorphite    -> (Hemorphite q.Value)  |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonHemorphite  -> (Hemorphite q.Value)  |> OreFactory (Uncommon) (IsCompressed) 
        | CompressedRareHemorphite      -> (Hemorphite q.Value)  |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonJaspet        -> (Jaspet q.Value)      |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonJaspet      -> (Jaspet q.Value)      |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRareJaspet          -> (Jaspet q.Value)      |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonKernite       -> (Kernite q.Value)     |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonKernite     -> (Kernite q.Value)     |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRareKernite         -> (Kernite q.Value)     |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonOmber         -> (Omber q.Value)       |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonOmber       -> (Omber q.Value)       |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRareOmber           -> (Omber q.Value)       |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonPlagioclase   -> (Plagioclase q.Value) |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonPlagioclase -> (Plagioclase q.Value) |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRarePlagioclase     -> (Plagioclase q.Value) |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonPyroxeres     -> (Pyroxeres q.Value)   |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonPyroxeres   -> (Pyroxeres q.Value)   |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRarePyroxeres       -> (Pyroxeres q.Value)   |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonScordite      -> (Scordite q.Value)    |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonScordite    -> (Scordite q.Value)    |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRareScordite        -> (Scordite q.Value)    |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonVeldspar      -> (Veldspar q.Value)    |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonVeldspar    -> (Veldspar q.Value)    |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRareVeldspar        -> (Veldspar q.Value)    |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonGneiss        -> (Gneiss q.Value)      |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonGneiss      -> (Gneiss q.Value)      |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRareGneiss          -> (Gneiss q.Value)      |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonDarkOchre     -> (DarkOchre q.Value)   |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonDarkOchre   -> (DarkOchre q.Value)   |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRareDarkOchre       -> (DarkOchre q.Value)   |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonCrokite       -> (Crokite q.Value)     |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonCrokite     -> (Crokite q.Value)     |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRareCrokite         -> (Crokite q.Value)     |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonBistot        -> (Bistot q.Value)      |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonBistot      -> (Bistot q.Value)      |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRareBistot          -> (Bistot q.Value)      |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonSpodumain     -> (Spodumain q.Value)   |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonSpodumain   -> (Spodumain q.Value)   |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRareSpodumain       -> (Spodumain q.Value)   |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonArkonor       -> (Arkonor q.Value)     |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonArkonor     -> (Arkonor q.Value)     |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRareArkonor         -> (Arkonor q.Value)     |> OreFactory (Rare)     (IsCompressed)
        | CompressedCommonMercoxit      -> (Mercoxit q.Value)    |> OreFactory (Common)   (IsCompressed)
        | CompressedUncommonMercoxit    -> (Mercoxit q.Value)    |> OreFactory (Uncommon) (IsCompressed)
        | CompressedRareMercoxit        -> (Mercoxit q.Value)    |> OreFactory (Rare)     (IsCompressed)


