namespace EveData

module RawMaterial = 
    open EveData.RawMaterials.Types
    open EveData.RawMaterials.Records
    
    let RawOreYield (x:RawOre) :Yield = match x with
        | Veldspar _    -> { BaseYield with Tritanium = Tritanium 415 }
        | Scordite _    -> { BaseYield with Tritanium = Tritanium 346
                                            Pyerite   = Pyerite   173 }
        | Pyroxeres _   -> { BaseYield with Tritanium = Tritanium 315
                                            Pyerite   = Pyerite   25
                                            Mexallon  = Mexallon  50
                                            Nocxium   = Nocxium   5  }
        | Plagioclase _ -> { BaseYield with Tritanium = Tritanium 107
                                            Pyerite   = Pyerite   213
                                            Mexallon  = Mexallon  107 }
        | Omber _       -> { BaseYield with Tritanium = Tritanium 85
                                            Pyerite   = Pyerite   34
                                            Isogen    = Isogen    85 }
        | Kernite _     -> { BaseYield with Tritanium = Tritanium 134
                                            Mexallon  = Mexallon  267
                                            Isogen    = Isogen    134 }
        | Jaspet _      -> { BaseYield with Tritanium = Tritanium 72
                                            Pyerite   = Pyerite   121
                                            Mexallon  = Mexallon  144
                                            Nocxium   = Nocxium   72
                                            Zydrine   = Zydrine   3 }
        | Hedbergite _  -> { BaseYield with Tritanium = Tritanium 180
                                            Pyerite   = Pyerite   72
                                            Mexallon  = Mexallon  17
                                            Isogen    = Isogen    59
                                            Nocxium   = Nocxium   118
                                            Zydrine   = Zydrine   8 }
        | Hemorphite _  -> { BaseYield with Pyerite   = Pyerite   81
                                            Isogen    = Isogen    196
                                            Nocxium   = Nocxium   98
                                            Zydrine   = Zydrine   9 }
        | Gneiss _      -> { BaseYield with Tritanium = Tritanium 1278
                                            Mexallon  = Mexallon  1278
                                            Isogen    = Isogen    242
                                            Zydrine   = Zydrine   60 }
        | DarkOchre _   -> { BaseYield with Tritanium = Tritanium 8804
                                            Nocxium   = Nocxium   173 
                                            Zydrine   = Zydrine   87 }
        | Spodumain _   -> { BaseYield with Tritanium = Tritanium 39221
                                            Pyerite   = Pyerite   4972
                                            Megacyte  = Megacyte  78 }
        | Arkonor _     -> { BaseYield with Tritanium = Tritanium 6905
                                            Mexallon  = Mexallon  1278
                                            Megacyte  = Megacyte  230
                                            Zydrine   = Zydrine   115 }
        | Crokite _     -> { BaseYield with Tritanium = Tritanium 20992
                                            Nocxium   = Nocxium   275
                                            Zydrine   = Zydrine   367 }
        | Bistot _      -> { BaseYield with Pyerite   = Pyerite   16572
                                            Megacyte  = Megacyte  118
                                            Zydrine   = Zydrine   236 }
        | Mercoxit _    -> { BaseYield with Morphite  = Morphite  293 }

    let RawIceYield (x:RawIce) :Yield = match x with
        | ClearIcicle _         -> { BaseYield with HeavyWater          = HeavyWater          50
                                                    LiquidOzone         = LiquidOzone         25
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    HeliumIsotopes      = HeliumIsotopes      300 }
        | EnrichedClearIcicle _ -> { BaseYield with HeavyWater          = HeavyWater          75
                                                    LiquidOzone         = LiquidOzone         40
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    HeliumIsotopes      = HeliumIsotopes      350 }
        | BlueIce _             -> { BaseYield with HeavyWater          = HeavyWater          50
                                                    LiquidOzone         = LiquidOzone         25
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    OxygenIsotopes      = OxygenIsotopes      300 }
        | ThickBlueIce _        -> { BaseYield with HeavyWater          = HeavyWater          75
                                                    LiquidOzone         = LiquidOzone         40
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    OxygenIsotopes      = OxygenIsotopes      350 }
        | GlacialMass _         -> { BaseYield with HeavyWater          = HeavyWater          50
                                                    LiquidOzone         = LiquidOzone         25
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    HydrogenIsotopes    = HydrogenIsotopes    300 }
        | SmoothGlacialMass _   -> { BaseYield with HeavyWater          = HeavyWater          75
                                                    LiquidOzone         = LiquidOzone         40
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    HydrogenIsotopes    = HydrogenIsotopes    350 }
        | WhiteGlaze _          -> { BaseYield with HeavyWater          = HeavyWater          50
                                                    LiquidOzone         = LiquidOzone         25
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    NitrogenIsotopes    = NitrogenIsotopes    300 }
        | PristineWhiteGlaze _  -> { BaseYield with HeavyWater          = HeavyWater          75
                                                    LiquidOzone         = LiquidOzone         40
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    NitrogenIsotopes    = NitrogenIsotopes    350 }
        | GlareCrust _          -> { BaseYield with HeavyWater          = HeavyWater          1000
                                                    LiquidOzone         = LiquidOzone         500
                                                    StrontiumClathrates = StrontiumClathrates 25 }
        | DarkGlitter _         -> { BaseYield with HeavyWater          = HeavyWater          500
                                                    LiquidOzone         = LiquidOzone         1000
                                                    StrontiumClathrates = StrontiumClathrates 50 }
        | Gelidus _             -> { BaseYield with HeavyWater          = HeavyWater          250
                                                    LiquidOzone         = LiquidOzone         500
                                                    StrontiumClathrates = StrontiumClathrates 75 }
        | Krystallos _          -> { BaseYield with HeavyWater          = HeavyWater          125
                                                    LiquidOzone         = LiquidOzone         250
                                                    StrontiumClathrates = StrontiumClathrates 125 }
        
    let RawOreVolume (x:RawOre) :single = match x with
        | Veldspar _    -> 0.1f
        | Scordite _    -> 0.15f
        | Pyroxeres _   -> 0.3f
        | Plagioclase _ -> 0.35f
        | Omber _       -> 0.6f
        | Kernite _     -> 1.2f
        | Jaspet _      -> 2.0f
        | Hedbergite _  -> 3.0f
        | Hemorphite _  -> 3.0f
        | Gneiss _      -> 5.0f
        | DarkOchre _   -> 8.0f
        | Spodumain _   -> 16.0f
        | Arkonor _     -> 16.0f
        | Crokite _     -> 16.0f
        | Bistot _      -> 16.0f
        | Mercoxit _    -> 40.0f

    let RawIceVolume (x:RawIce) :single = match x with
        | ClearIcicle _         -> 1000.0f
        | EnrichedClearIcicle _ -> 1000.0f
        | BlueIce _             -> 1000.0f
        | ThickBlueIce _        -> 1000.0f
        | GlacialMass _         -> 1000.0f
        | SmoothGlacialMass _   -> 1000.0f
        | WhiteGlaze _          -> 1000.0f
        | PristineWhiteGlaze _  -> 1000.0f 
        | GlareCrust _          -> 1000.0f 
        | DarkGlitter _         -> 1000.0f 
        | Gelidus _             -> 1000.0f
        | Krystallos _          -> 1000.0f
    
    let RawOreName (x) :string = match x with
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
        
    let RawIceName (x) :string = match x with 
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

    type OreId = OreId of int with
        member this.Value = 
            this |> (fun (OreId x) -> x)

    type OreQty = OreQty of int with
        member this.Value = 
            this |> (fun (OreQty x) -> x)

    type OreData = {
        OreId  : OreId
        Name   : Name
        OreQty : OreQty
    }
    // Note: this takes a tuple because you NEED all of those DUs to identify the ore.
    // It's useful to be able to identify the ore and maintain the quantity.    
    type OreDataType = RawOre -> OreRarity -> Compressed -> OreData
    let (OreData:OreDataType) = (fun x y z -> 
        (fun (x, y, z) -> { 
            OreId  = OreId x
            Name   = Name y
            OreQty = OreQty z            
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
            )

    let Volume (x) :single = match x with
        | RawOre x -> RawOreVolume x
        | RawIce x -> RawIceVolume x
            
    let Yield (x) :Yield = match x with 
        | RawOre x -> RawOreYield x
        | RawIce x -> RawIceYield x

    let Name (x) :string = match x with
        | RawOre x -> RawOreName x
        | RawIce x -> RawIceName x

    


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