namespace EveData

module RawMaterials = 
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

    // Note: this takes a tuple because you NEED all of those DUs to identify the ore.
    // It's useful to be able to identify the ore and maintain the quantity.    
    // type OreDataType = RawOre * OreRarity * Compressed -> OreId * Name * OreQty
    let OreData ore= 
        (fun (x, y, z) -> OreId x , Name y, OreQty z) <| match ore with
        | Arkonor qty     ,Common     ,IsNotCompressed   -> 0, "", qty 
        | Arkonor qty     ,Common     ,IsCompressed      -> 0, "", qty 
        | Arkonor qty     ,Uncommon   ,IsNotCompressed   -> 0, "", qty 
        | Arkonor qty     ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Arkonor qty     ,Rare       ,IsNotCompressed   -> 0, "", qty 
        | Arkonor qty     ,Rare       ,IsCompressed      -> 0, "", qty 

        | Bistot qty      ,Common     ,IsNotCompressed   -> 0, "", qty 
        | Bistot qty      ,Common     ,IsCompressed      -> 0, "", qty 
        | Bistot qty      ,Uncommon   ,IsNotCompressed   -> 0, "", qty 
        | Bistot qty      ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Bistot qty      ,Rare       ,IsNotCompressed   -> 0, "", qty 
        | Bistot qty      ,Rare       ,IsCompressed      -> 0, "", qty 

        | Crokite qty     ,Common     ,IsNotCompressed   -> 0, "", qty 
        | Crokite qty     ,Common     ,IsCompressed      -> 0, "", qty 
        | Crokite qty     ,Uncommon   ,IsNotCompressed   -> 0, "", qty 
        | Crokite qty     ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Crokite qty     ,Rare       ,IsNotCompressed   -> 0, "", qty 
        | Crokite qty     ,Rare       ,IsCompressed      -> 0, "", qty 

        | DarkOchre qty   ,Common     ,IsNotCompressed   -> 0, "", qty 
        | DarkOchre qty   ,Common     ,IsCompressed      -> 0, "", qty 
        | DarkOchre qty   ,Uncommon   ,IsNotCompressed   -> 0, "", qty 
        | DarkOchre qty   ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | DarkOchre qty   ,Rare       ,IsNotCompressed   -> 0, "", qty 
        | DarkOchre qty   ,Rare       ,IsCompressed      -> 0, "", qty 

        | Gneiss qty      ,Common     ,IsNotCompressed   -> 0, "", qty 
        | Gneiss qty      ,Common     ,IsCompressed      -> 0, "", qty 
        | Gneiss qty      ,Uncommon   ,IsNotCompressed   -> 0, "", qty 
        | Gneiss qty      ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Gneiss qty      ,Rare       ,IsNotCompressed   -> 0, "", qty 
        | Gneiss qty      ,Rare       ,IsCompressed      -> 0, "", qty 

        | Hedbergite qty  ,Common     ,IsNotCompressed   -> 0, "", qty 
        | Hedbergite qty  ,Common     ,IsCompressed      -> 0, "", qty 
        | Hedbergite qty  ,Uncommon   ,IsNotCompressed   -> 0, "", qty 
        | Hedbergite qty  ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Hedbergite qty  ,Rare       ,IsNotCompressed   -> 0, "", qty 
        | Hedbergite qty  ,Rare       ,IsCompressed      -> 0, "", qty 

        | Hemorphite qty  ,Common     ,IsNotCompressed   -> 0, "", qty 
        | Hemorphite qty  ,Common     ,IsCompressed      -> 0, "", qty 
        | Hemorphite qty  ,Uncommon   ,IsNotCompressed   -> 0, "", qty 
        | Hemorphite qty  ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Hemorphite qty  ,Rare       ,IsNotCompressed   -> 0, "", qty 
        | Hemorphite qty  ,Rare       ,IsCompressed      -> 0, "", qty 

        | Jaspet qty      ,Common     ,IsNotCompressed   -> 0, "", qty 
        | Jaspet qty      ,Common     ,IsCompressed      -> 0, "", qty 
        | Jaspet qty      ,Uncommon   ,IsNotCompressed   -> 0, "", qty 
        | Jaspet qty      ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Jaspet qty      ,Rare       ,IsNotCompressed   -> 0, "", qty 
        | Jaspet qty      ,Rare       ,IsCompressed      -> 0, "", qty 

        | Kernite qty     ,Common     ,IsNotCompressed   -> 0, "", qty 
        | Kernite qty     ,Common     ,IsCompressed      -> 0, "", qty 
        | Kernite qty     ,Uncommon   ,IsNotCompressed   -> 0, "", qty 
        | Kernite qty     ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Kernite qty     ,Rare       ,IsNotCompressed   -> 0, "", qty 
        | Kernite qty     ,Rare       ,IsCompressed      -> 0, "", qty 

        | Mercoxit qty    ,Common     ,IsNotCompressed   -> 0, "", qty 
        | Mercoxit qty    ,Common     ,IsCompressed      -> 0, "", qty 
        | Mercoxit qty    ,Uncommon   ,IsNotCompressed   -> 0, "", qty 
        | Mercoxit qty    ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Mercoxit qty    ,Rare       ,IsNotCompressed   -> 0, "", qty 
        | Mercoxit qty    ,Rare       ,IsCompressed      -> 0, "", qty 

        | Omber qty       ,Common     ,IsNotCompressed   -> 0, "", qty 
        | Omber qty       ,Common     ,IsCompressed      -> 0, "", qty 
        | Omber qty       ,Uncommon   ,IsNotCompressed   -> 0, "", qty 
        | Omber qty       ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Omber qty       ,Rare       ,IsNotCompressed   -> 0, "", qty 
        | Omber qty       ,Rare       ,IsCompressed      -> 0, "", qty 

        | Plagioclase qty ,Common     ,IsNotCompressed   -> 18, "Plagioclase", qty 
        | Plagioclase qty ,Common     ,IsCompressed      -> 0, "", qty 
        | Plagioclase qty ,Uncommon   ,IsNotCompressed   -> 0, "Azure Plagioclase", qty 
        | Plagioclase qty ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Plagioclase qty ,Rare       ,IsNotCompressed   -> 0, "Rich Plagioclase", qty 
        | Plagioclase qty ,Rare       ,IsCompressed      -> 0, "", qty 

        | Pyroxeres qty   ,Common     ,IsNotCompressed   -> 0, "Pyroxeres", qty 
        | Pyroxeres qty   ,Common     ,IsCompressed      -> 0, "", qty 
        | Pyroxeres qty   ,Uncommon   ,IsNotCompressed   -> 0, "Solid Pyroxeres", qty 
        | Pyroxeres qty   ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Pyroxeres qty   ,Rare       ,IsNotCompressed   -> 0, "Viscous Pyroxeres", qty 
        | Pyroxeres qty   ,Rare       ,IsCompressed      -> 0, "", qty 

        | Scordite qty    ,Common     ,IsNotCompressed   -> 1228, "Scordite", qty 
        | Scordite qty    ,Common     ,IsCompressed      -> 0, "", qty 
        | Scordite qty    ,Uncommon   ,IsNotCompressed   -> 17463, "Condensed Scordite", qty 
        | Scordite qty    ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Scordite qty    ,Rare       ,IsNotCompressed   -> 17464, "Massive Scordite", qty 
        | Scordite qty    ,Rare       ,IsCompressed      -> 0, "", qty 

        | Spodumain qty   ,Common     ,IsNotCompressed   -> 0, "", qty 
        | Spodumain qty   ,Common     ,IsCompressed      -> 0, "", qty 
        | Spodumain qty   ,Uncommon   ,IsNotCompressed   -> 0, "", qty 
        | Spodumain qty   ,Uncommon   ,IsCompressed      -> 0, "", qty 
        | Spodumain qty   ,Rare       ,IsNotCompressed   -> 0, "", qty 
        | Spodumain qty   ,Rare       ,IsCompressed      -> 0, "", qty 

        | Veldspar qty    ,Common     ,IsNotCompressed   -> 1230   , "", qty 
        | Veldspar qty    ,Common     ,IsCompressed      -> 28432  , "", qty 
        | Veldspar qty    ,Uncommon   ,IsNotCompressed   -> 17470  , "", qty 
        | Veldspar qty    ,Uncommon   ,IsCompressed      -> 0      , "", qty 
        | Veldspar qty    ,Rare       ,IsNotCompressed   -> 17471  , "", qty 
        | Veldspar qty    ,Rare       ,IsCompressed      -> 0      , "", qty 



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