namespace EveData

module RawMaterials = 
    open EveData.RawMaterials.Types
    open EveData.RawMaterials.Records
    
    let RawOreYield (x:RawOre) :Yield = match x with
        | Veldspar x    -> { BaseYield with Tritanium = Tritanium 415 }
        | Scordite x    -> { BaseYield with Tritanium = Tritanium 346
                                            Pyerite   = Pyerite   173 }
        | Pyroxeres x   -> { BaseYield with Tritanium = Tritanium 315
                                            Pyerite   = Pyerite   25
                                            Mexallon  = Mexallon  50
                                            Nocxium   = Nocxium   5  }
        | Plagioclase x -> { BaseYield with Tritanium = Tritanium 107
                                            Pyerite   = Pyerite   213
                                            Mexallon  = Mexallon  107 }
        | Omber x       -> { BaseYield with Tritanium = Tritanium 85
                                            Pyerite   = Pyerite   34
                                            Isogen    = Isogen    85 }
        | Kernite x     -> { BaseYield with Tritanium = Tritanium 134
                                            Mexallon  = Mexallon  267
                                            Isogen    = Isogen    134 }
        | Jaspet x      -> { BaseYield with Tritanium = Tritanium 72
                                            Pyerite   = Pyerite   121
                                            Mexallon  = Mexallon  144
                                            Nocxium   = Nocxium   72
                                            Zydrine   = Zydrine   3 }
        | Hedbergite x  -> { BaseYield with Tritanium = Tritanium 180
                                            Pyerite   = Pyerite   72
                                            Mexallon  = Mexallon  17
                                            Isogen    = Isogen    59
                                            Nocxium   = Nocxium   118
                                            Zydrine   = Zydrine   8 }
        | Hemorphite x  -> { BaseYield with Pyerite   = Pyerite   81
                                            Isogen    = Isogen    196
                                            Nocxium   = Nocxium   98
                                            Zydrine   = Zydrine   9 }
        | Gneiss x      -> { BaseYield with Tritanium = Tritanium 1278
                                            Mexallon  = Mexallon  1278
                                            Isogen    = Isogen    242
                                            Zydrine   = Zydrine   60 }
        | DarkOchre x   -> { BaseYield with Tritanium = Tritanium 8804
                                            Nocxium   = Nocxium   173 
                                            Zydrine   = Zydrine   87 }
        | Spodumain x   -> { BaseYield with Tritanium = Tritanium 39221
                                            Pyerite   = Pyerite   4972
                                            Megacyte  = Megacyte  78 }
        | Arkonor x     -> { BaseYield with Tritanium = Tritanium 6905
                                            Mexallon  = Mexallon  1278
                                            Megacyte  = Megacyte  230
                                            Zydrine   = Zydrine   115 }
        | Crokite x     -> { BaseYield with Tritanium = Tritanium 20992
                                            Nocxium   = Nocxium   275
                                            Zydrine   = Zydrine   367 }
        | Bistot x      -> { BaseYield with Pyerite   = Pyerite   16572
                                            Megacyte  = Megacyte  118
                                            Zydrine   = Zydrine   236 }
        | Mercoxit x    -> { BaseYield with Morphite  = Morphite  293 }

    let RawIceYield (x:RawIce) :Yield = match x with
        | ClearIcicle x         -> { BaseYield with HeavyWater          = HeavyWater          50
                                                    LiquidOzone         = LiquidOzone         25
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    HeliumIsotopes      = HeliumIsotopes      300 }
        | EnrichedClearIcicle x -> { BaseYield with HeavyWater          = HeavyWater          75
                                                    LiquidOzone         = LiquidOzone         40
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    HeliumIsotopes      = HeliumIsotopes      350 }
        | BlueIce x             -> { BaseYield with HeavyWater          = HeavyWater          50
                                                    LiquidOzone         = LiquidOzone         25
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    OxygenIsotopes      = OxygenIsotopes      300 }
        | ThickBlueIce x        -> { BaseYield with HeavyWater          = HeavyWater          75
                                                    LiquidOzone         = LiquidOzone         40
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    OxygenIsotopes      = OxygenIsotopes      350 }
        | GlacialMass x         -> { BaseYield with HeavyWater          = HeavyWater          50
                                                    LiquidOzone         = LiquidOzone         25
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    HydrogenIsotopes    = HydrogenIsotopes    300 }
        | SmoothGlacialMass x   -> { BaseYield with HeavyWater          = HeavyWater          75
                                                    LiquidOzone         = LiquidOzone         40
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    HydrogenIsotopes    = HydrogenIsotopes    350 }
        | WhiteGlaze x          -> { BaseYield with HeavyWater          = HeavyWater          50
                                                    LiquidOzone         = LiquidOzone         25
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    NitrogenIsotopes    = NitrogenIsotopes    300 }
        | PristineWhiteGlaze x  -> { BaseYield with HeavyWater          = HeavyWater          75
                                                    LiquidOzone         = LiquidOzone         40
                                                    StrontiumClathrates = StrontiumClathrates 1
                                                    NitrogenIsotopes    = NitrogenIsotopes    350 }
        | GlareCrust x          -> { BaseYield with HeavyWater          = HeavyWater          1000
                                                    LiquidOzone         = LiquidOzone         500
                                                    StrontiumClathrates = StrontiumClathrates 25 }
        | DarkGlitter x         -> { BaseYield with HeavyWater          = HeavyWater          500
                                                    LiquidOzone         = LiquidOzone         1000
                                                    StrontiumClathrates = StrontiumClathrates 50 }
        | Gelidus x             -> { BaseYield with HeavyWater          = HeavyWater          250
                                                    LiquidOzone         = LiquidOzone         500
                                                    StrontiumClathrates = StrontiumClathrates 75 }
        | Krystallos x          -> { BaseYield with HeavyWater          = HeavyWater          125
                                                    LiquidOzone         = LiquidOzone         250
                                                    StrontiumClathrates = StrontiumClathrates 125 }
        
    let RawOreVolume (x:RawOre) :single = match x with
        | Veldspar x    -> 0.1f
        | Scordite x    -> 0.15f
        | Pyroxeres x   -> 0.3f
        | Plagioclase x -> 0.35f
        | Omber x       -> 0.6f
        | Kernite x     -> 1.2f
        | Jaspet x      -> 2.0f
        | Hedbergite x  -> 3.0f
        | Hemorphite x  -> 3.0f
        | Gneiss x      -> 5.0f
        | DarkOchre x   -> 8.0f
        | Spodumain x   -> 16.0f
        | Arkonor x     -> 16.0f
        | Crokite x     -> 16.0f
        | Bistot x      -> 16.0f
        | Mercoxit x    -> 40.0f

    let RawIceVolume (x:RawIce) :single = match x with
        | ClearIcicle x         -> 1000.0f
        | EnrichedClearIcicle x -> 1000.0f
        | BlueIce x             -> 1000.0f
        | ThickBlueIce x        -> 1000.0f
        | GlacialMass x         -> 1000.0f
        | SmoothGlacialMass x   -> 1000.0f
        | WhiteGlaze x          -> 1000.0f
        | PristineWhiteGlaze x  -> 1000.0f 
        | GlareCrust x          -> 1000.0f 
        | DarkGlitter x         -> 1000.0f 
        | Gelidus x             -> 1000.0f
        | Krystallos x          -> 1000.0f
    
    let RawOreName (x) :string = match x with
        | Veldspar x    -> "Veldspar"
        | Scordite x    -> "Scordite"
        | Pyroxeres x   -> "Pyroxeres"
        | Plagioclase x -> "Plagioclase"
        | Omber x       -> "Omber"
        | Kernite x     -> "Kernite"
        | Jaspet x      -> "Jaspet"
        | Hedbergite x  -> "Hedbergite"
        | Hemorphite x  -> "Hemorphite"
        | Gneiss x      -> "Gneiss"
        | DarkOchre x   -> "Dark Ochre"
        | Spodumain x   -> "Spodumain"
        | Arkonor x     -> "Arkonor"
        | Crokite x     -> "Crokite"
        | Bistot x      -> "Bistot"
        | Mercoxit x    -> "Mercoxit"     
        
    let RawIceName (x) :string = match x with 
        | ClearIcicle x         -> "Clear Icicle"
        | EnrichedClearIcicle x -> "Enriched Clear Icicle"
        | BlueIce x             -> "Blue Ice"
        | ThickBlueIce x        -> "Thick Blue Ice"
        | GlacialMass x         -> "Glacial Mass"
        | SmoothGlacialMass x   -> "Smooth Glacial Mass"
        | WhiteGlaze x          -> "White Glaze"
        | PristineWhiteGlaze x  -> "Pristine White Glaze"
        | GlareCrust x          -> "Glare Crust"
        | DarkGlitter x         -> "Dark Glitter"
        | Gelidus x             -> "Gelidus"
        | Krystallos x          -> "Krystallos"

    let VeldsparTypeId (x) :int = match x with
    | Normal x   -> 10
    | PlusFive x -> 10
    | PlusTen x  -> 10
        
    // let RawOreTypeId (x) :int = match x with 
        // | Veldspar x -> Veldspar(x) |> RawOre |> OreConcentration |> VeldsparTypeId
        // | Scordite x    
        // | Pyroxeres x   
        // | Plagioclase x 
        // | Omber x       
        // | Kernite x     
        // | Jaspet x      
        // | Hedbergite x  
        // | Hemorphite x  
        // | Gneiss x      
        // | DarkOchre x   
        // | Spodumain x   
        // | Arkonor x     
        // | Crokite x     
        // | Bistot x      
        // | Mercoxit x    

    let Volume (x) :single = match x with
        | RawOre x -> RawOreVolume x
        | RawIce x -> RawIceVolume x
            
    let Yield (x) :Yield = match x with 
        | RawOre x -> RawOreYield x
        | RawIce x -> RawIceYield x

    let Name (x) :string = match x with
        | RawOre x -> RawOreName x
        | RawIce x -> RawIceName x

    

    let TestBlueIce = 
        let BlueIce = BlueIce |> RawIce
        {
            IsCompressed = IsCompressed.Raw
            Yield = BlueIce |> Yield
            Volume = BlueIce |> Volume
            TypeId = 10000
            Name = "BlueIce"
        }


    let ThisIs100 = TestBlueIce.Yield.HeavyWater

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