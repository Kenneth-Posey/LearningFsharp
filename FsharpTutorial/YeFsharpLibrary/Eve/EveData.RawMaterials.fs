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

    type MaterialName = MaterialName of string
    type IsCompressed = IsCompressed of bool
    type Volume       = Volume of single
    type BaseId       = BaseId of int

    type Yield = 
    | OreYield of OreYield
    | IceYield of IceYield

    type RefinedProduct = 
    | Mineral of Mineral
    | IceProduct of IceProduct
        
    type RawMaterial = {
        MaterialName    : MaterialName
        IsCompressed    : IsCompressed
        Yield           : Yield
        Volume          : Volume
        BaseId          : BaseId
    }

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