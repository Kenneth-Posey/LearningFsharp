namespace EveData

module RawMaterials = 
    
    // Begin section for defining yield types
    type Tritanium = int
    type Pyerite   = int
    type Mexallon  = int
    type Isogen    = int
    type Nocxium   = int
    type Megacyte  = int
    type Zydrine   = int
    type Morphite  = int

    type HeavyWater          = int
    type HeliumIsotopes      = int
    type HydrogenIsotopes    = int
    type LiquidOzone         = int
    type NitrogenIsotopes    = int
    type OxygenIsotopes      = int
    type StrontiumClathrates = int

    type Mineral = 
    | Tritanium of int : Tritanium
    | Pyerite   of int : Pyerite 
    | Mexallon  of int : Mexallon
    | Isogen    of int : Isogen  
    | Nocxium   of int : Nocxium 
    | Megacyte  of int : Megacyte
    | Zydrine   of int : Zydrine 
    | Morphite  of int : Morphite
    
    type IceProduct = 
    | HeavyWater          of int : HeavyWater          
    | HeliumIsotopes      of int : HeliumIsotopes      
    | HydrogenIsotopes    of int : HydrogenIsotopes    
    | LiquidOzone         of int : LiquidOzone         
    | NitrogenIsotopes    of int : NitrogenIsotopes    
    | OxygenIsotopes      of int : OxygenIsotopes      
    | StrontiumClathrates of int : StrontiumClathrates 
    
    type Yield = {
        Tritanium : Tritanium
        Pyerite   : Pyerite 
        Mexallon  : Mexallon
        Isogen    : Isogen  
        Nocxium   : Nocxium 
        Megacyte  : Megacyte
        Zydrine   : Zydrine 
        Morphite  : Morphite
        HeavyWater          : HeavyWater         
        HeliumIsotopes      : HeliumIsotopes     
        HydrogenIsotopes    : HydrogenIsotopes   
        LiquidOzone         : LiquidOzone        
        NitrogenIsotopes    : NitrogenIsotopes   
        OxygenIsotopes      : OxygenIsotopes     
        StrontiumClathrates : StrontiumClathrates
    }

    let BaseYield = {
        Tritanium   = 0
        Pyerite     = 0
        Mexallon    = 0
        Isogen      = 0
        Nocxium     = 0
        Megacyte    = 0
        Zydrine     = 0
        Morphite    = 0
        HeavyWater          = 0
        HeliumIsotopes      = 0
        HydrogenIsotopes    = 0
        LiquidOzone         = 0
        NitrogenIsotopes    = 0
        OxygenIsotopes      = 0
        StrontiumClathrates = 0
    }
    
    type MaterialName = string
    type IsCompressed = Raw | Compressed
    type Volume = single
    type TypeId   = int
    type TypeId5  = int
    type TypeId10 = int
    type Name   = string
    type Name5  = string
    type Name10 = string
    
    type OreConcentration = 
    | Normal    of RawOre : OreConcentration
    | PlusFive  of RawOre : OreConcentration
    | PlusTen   of RawOre : OreConcentration

    type Compressed = 
    | IsCompressed
    | IsNotCompressed

    type RawMaterial = {
        IsCompressed : IsCompressed
        Yield        : Yield
        Volume       : Volume
        TypeId       : TypeId
        Name         : Name
    }

    type Ore = {        
        RawMaterial : RawMaterial
        TypeId5     : TypeId5
        TypeId10    : TypeId10
        Name5       : Name5
        Name10      : Name10
    }

    type Ice = {
        RawMaterial : RawMaterial
    }

    type OreRarity = 
    | Base   of Compressed : OreRarity
    | Base5  of Compressed : OreRarity  
    | Base10 of Compressed : OreRarity

    type RawOre = 
    | Arkonor     of OreConcentration : RawOre 
    | Bistot      of OreConcentration : RawOre 
    | Crokite     of OreConcentration : RawOre 
    | DarkOchre   of OreConcentration : RawOre 
    | Gneiss      of OreConcentration : RawOre 
    | Hedbergite  of OreConcentration : RawOre 
    | Hemorphite  of OreConcentration : RawOre 
    | Jaspet      of OreConcentration : RawOre 
    | Kernite     of OreConcentration : RawOre 
    | Mercoxit    of OreConcentration : RawOre 
    | Omber       of OreConcentration : RawOre 
    | Plagioclase of OreConcentration : RawOre 
    | Pyroxeres   of OreConcentration : RawOre 
    | Scordite    of OreConcentration : RawOre 
    | Spodumain   of OreConcentration : RawOre 
    | Veldspar    of OreConcentration : RawOre 
    
    type RawIce = 
    | BlueIce             
    | ClearIcicle         
    | DarkGlitter         
    | EnrichedClearIcicle 
    | Gelidus             
    | GlacialMass         
    | GlareCrust          
    | Krystallos          
    | PristineWhiteGlaze  
    | SmoothGlacialMass   
    | ThickBlueIce        
    | WhiteGlaze          
    
    type Refinable = 
    | RawOre of RawOre
    | RawIce of RawIce

    let RawOreYield (x:RawOre) :Yield = match x with
        | Veldspar x    -> { BaseYield with Tritanium = 415 }
        | Scordite x    -> { BaseYield with Tritanium = 346
                                            Pyerite   = 173 }
        | Pyroxeres x   -> { BaseYield with Tritanium = 315
                                            Pyerite   = 25
                                            Mexallon  = 50
                                            Nocxium   = 5  }
        | Plagioclase x -> { BaseYield with Tritanium = 107
                                            Pyerite   = 213
                                            Mexallon  = 107 }
        | Omber x       -> { BaseYield with Tritanium = 85
                                            Pyerite   = 34
                                            Isogen    = 85 }
        | Kernite x     -> { BaseYield with Tritanium = 134
                                            Mexallon  = 267
                                            Isogen    = 134 }
        | Jaspet x      -> { BaseYield with Tritanium = 72
                                            Pyerite   = 121
                                            Mexallon  = 144
                                            Nocxium   = 72
                                            Zydrine   = 3 }
        | Hedbergite x  -> { BaseYield with Tritanium = 180
                                            Pyerite   = 72
                                            Mexallon  = 17
                                            Isogen    = 59
                                            Nocxium   = 118
                                            Zydrine   = 8 }
        | Hemorphite x  -> { BaseYield with Pyerite   = 81
                                            Isogen    = 196
                                            Nocxium   = 98
                                            Zydrine   = 9 }
        | Gneiss x      -> { BaseYield with Tritanium = 1278
                                            Mexallon  = 1278
                                            Isogen    = 242
                                            Zydrine   = 60 }
        | DarkOchre x   -> { BaseYield with Tritanium = 8804
                                            Nocxium   = 173 
                                            Zydrine   = 87 }
        | Spodumain x   -> { BaseYield with Tritanium = 39221
                                            Pyerite   = 4972
                                            Megacyte  = 78 }
        | Arkonor x     -> { BaseYield with Tritanium = 6905
                                            Mexallon  = 1278
                                            Megacyte  = 230
                                            Zydrine   = 115 }
        | Crokite x     -> { BaseYield with Tritanium = 20992
                                            Nocxium   = 275
                                            Zydrine   = 367 }
        | Bistot x      -> { BaseYield with Pyerite   = 16572
                                            Megacyte  = 118
                                            Zydrine   = 236 }
        | Mercoxit x    -> { BaseYield with Morphite  = 293 }

    let RawIceYield (x:RawIce) :Yield = match x with
        | ClearIcicle           -> { BaseYield with HeavyWater          = 50
                                                    LiquidOzone         = 25
                                                    StrontiumClathrates = 1
                                                    HeliumIsotopes      = 300 }
        | EnrichedClearIcicle   -> { BaseYield with HeavyWater          = 75
                                                    LiquidOzone         = 40
                                                    StrontiumClathrates = 1
                                                    HeliumIsotopes      = 350 }
        | BlueIce               -> { BaseYield with HeavyWater          = 50
                                                    LiquidOzone         = 25
                                                    StrontiumClathrates = 1
                                                    OxygenIsotopes      = 300 }
        | ThickBlueIce          -> { BaseYield with HeavyWater          = 75
                                                    LiquidOzone         = 40
                                                    StrontiumClathrates = 1
                                                    OxygenIsotopes      = 350 }
        | GlacialMass           -> { BaseYield with HeavyWater          = 50
                                                    LiquidOzone         = 25
                                                    StrontiumClathrates = 1
                                                    HydrogenIsotopes    = 300 }
        | SmoothGlacialMass     -> { BaseYield with HeavyWater          = 75
                                                    LiquidOzone         = 40
                                                    StrontiumClathrates = 1
                                                    HydrogenIsotopes    = 350 }
        | WhiteGlaze            -> { BaseYield with HeavyWater          = 50
                                                    LiquidOzone         = 25
                                                    StrontiumClathrates = 1
                                                    NitrogenIsotopes    = 300 }
        | PristineWhiteGlaze    -> { BaseYield with HeavyWater          = 75
                                                    LiquidOzone         = 40
                                                    StrontiumClathrates = 1
                                                    NitrogenIsotopes    = 350 }
        | GlareCrust            -> { BaseYield with HeavyWater          = 1000
                                                    LiquidOzone         = 500
                                                    StrontiumClathrates = 25 }
        | DarkGlitter           -> { BaseYield with HeavyWater          = 500
                                                    LiquidOzone         = 1000
                                                    StrontiumClathrates = 50 }
        | Gelidus               -> { BaseYield with HeavyWater          = 250
                                                    LiquidOzone         = 500
                                                    StrontiumClathrates = 75 }
        | Krystallos            -> { BaseYield with HeavyWater          = 125
                                                    LiquidOzone         = 250
                                                    StrontiumClathrates = 125 }
        
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
        | ClearIcicle           -> 1000.0f
        | EnrichedClearIcicle   -> 1000.0f
        | BlueIce               -> 1000.0f
        | ThickBlueIce          -> 1000.0f
        | GlacialMass           -> 1000.0f
        | SmoothGlacialMass     -> 1000.0f
        | WhiteGlaze            -> 1000.0f
        | PristineWhiteGlaze    -> 1000.0f 
        | GlareCrust            -> 1000.0f 
        | DarkGlitter           -> 1000.0f 
        | Gelidus               -> 1000.0f
        | Krystallos            -> 1000.0f
    
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