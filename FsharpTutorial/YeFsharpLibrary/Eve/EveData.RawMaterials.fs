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

    type Mineral = 
    | Tritanium of int : Tritanium
    | Pyerite   of int : Pyerite 
    | Mexallon  of int : Mexallon
    | Isogen    of int : Isogen  
    | Nocxium   of int : Nocxium 
    | Megacyte  of int : Megacyte
    | Zydrine   of int : Zydrine 
    | Morphite  of int : Morphite

    type OreYield = {
        Tritanium : Tritanium
        Pyerite   : Pyerite 
        Mexallon  : Mexallon
        Isogen    : Isogen  
        Nocxium   : Nocxium 
        Megacyte  : Megacyte
        Zydrine   : Zydrine 
        Morphite  : Morphite
    }

    let BaseOreYield = {
        Tritanium   = 0
        Pyerite     = 0
        Mexallon    = 0
        Isogen      = 0
        Nocxium     = 0
        Megacyte    = 0
        Zydrine     = 0
        Morphite    = 0
    }
    
    type HeavyWater          = int
    type HeliumIsotopes      = int
    type HydrogenIsotopes    = int
    type LiquidOzone         = int
    type NitrogenIsotopes    = int
    type OxygenIsotopes      = int
    type StrontiumClathrates = int

    type IceProduct = 
    | HeavyWater          of int : HeavyWater          
    | HeliumIsotopes      of int : HeliumIsotopes      
    | HydrogenIsotopes    of int : HydrogenIsotopes    
    | LiquidOzone         of int : LiquidOzone         
    | NitrogenIsotopes    of int : NitrogenIsotopes    
    | OxygenIsotopes      of int : OxygenIsotopes      
    | StrontiumClathrates of int : StrontiumClathrates 

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

    type Yield = 
    | OreYield of Yield : OreYield
    | IceYield of Yield : IceYield

    type RefinedProduct = 
    | Mineral of Mineral
    | IceProduct of IceProduct

    type OreConcentration = 
    | Normal
    | PlusFive
    | PlusTen

    type Compressed = 
    | IsCompressed
    | IsNotCompressed

    type RawMaterial = {
        IsCompressed    : IsCompressed
        Yield           : Yield
        Volume          : Volume
        TypeId          : TypeId
        Name            : Name
    }

    type RawOre = {        
        RawMaterial : RawMaterial
        TypeId5     : TypeId5
        TypeId10    : TypeId10
        Name5       : Name5
        Name10      : Name10
    }

    type RawIce = {
        RawMaterial : RawMaterial
    }

    type Ore = 
    | Arkonor     of Ore : RawOre
    | Bistot      of Ore : RawOre
    | Crokite     of Ore : RawOre
    | DarkOchre   of Ore : RawOre
    | Gneiss      of Ore : RawOre
    | Hedbergite  of Ore : RawOre
    | Hemorphite  of Ore : RawOre
    | Jaspet      of Ore : RawOre
    | Kernite     of Ore : RawOre
    | Mercoxit    of Ore : RawOre
    | Omber       of Ore : RawOre
    | Plagioclase of Ore : RawOre
    | Pyroxeres   of Ore : RawOre
    | Scordite    of Ore : RawOre
    | Spodumain   of Ore : RawOre
    | Veldspar    of Ore : RawOre
    
    type Ice = 
    | BlueIce             of Ice : RawIce
    | ClearIcicle         of Ice : RawIce
    | DarkGlitter         of Ice : RawIce
    | EnrichedClearIcicle of Ice : RawIce
    | Gelidus             of Ice : RawIce
    | GlacialMass         of Ice : RawIce
    | GlareCrust          of Ice : RawIce
    | Krystallos          of Ice : RawIce
    | PristineWhiteGlaze  of Ice : RawIce
    | SmoothGlacialMass   of Ice : RawIce
    | ThickBlueIce        of Ice : RawIce
    | WhiteGlaze          of Ice : RawIce

    let TestBlueIce = {
        RawMaterial = {
            IsCompressed = IsCompressed.Raw
            Yield = Yield.IceYield({
                BaseIceYield with
                HeavyWater = 100
            })
            Volume = 100.0f
            TypeId = 10000
            Name = "BlueIce"
        }
    }

    let blerhg = 
        match TestBlueIce.RawMaterial.Yield with 
        | IceYield x -> x 
        | OreYield _ -> BaseIceYield

    let ThisIs100 = blerhg.HeavyWater

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