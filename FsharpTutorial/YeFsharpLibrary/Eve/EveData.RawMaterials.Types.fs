namespace EveData

module RawMaterialTypes = 
    // Minerals
    type Tritanium = Tritanium of int with
        member this.Value = 
            this |> (fun (Tritanium x) -> x)

    type Pyerite = Pyerite of int with
        member this.Value = 
            this |> (fun (Pyerite x) -> x)

    type Mexallon = Mexallon of int with
        member this.Value = 
            this |> (fun (Mexallon x) -> x)

    type Isogen = Isogen of int with
        member this.Value = 
            this |> (fun (Isogen x) -> x)

    type Nocxium = Nocxium of int with
        member this.Value = 
            this |> (fun (Nocxium x) -> x)

    type Megacyte = Megacyte of int with
        member this.Value =
            this |> (fun (Megacyte x) -> x)

    type Zydrine = Zydrine of int with
        member this.Value = 
            this |> (fun (Zydrine x) -> x)

    type Morphite = Morphite of int with
        member this.Value = 
            this |> (fun (Morphite x) -> x)


    // Ice Products
    type HeavyWater = HeavyWater of int with
        member this.Value = 
            this |> (fun (HeavyWater x) -> x)

    type HeliumIsotopes = HeliumIsotopes of int with
        member this.Value = 
            this |> (fun (HeliumIsotopes x) -> x)

    type HydrogenIsotopes = HydrogenIsotopes of int with
        member this.Value = 
            this |> (fun (HydrogenIsotopes x) -> x)

    type LiquidOzone = LiquidOzone of int with
        member this.Value = 
            this |> (fun (LiquidOzone x) -> x)

    type NitrogenIsotopes = NitrogenIsotopes of int with
        member this.Value = 
            this |> (fun (NitrogenIsotopes x) -> x)

    type OxygenIsotopes = OxygenIsotopes of int with
        member this.Value = 
            this |> (fun (OxygenIsotopes x) -> x)

    type StrontiumClathrates = StrontiumClathrates of int with
        member this.Value = 
            this |> (fun (StrontiumClathrates x) -> x)


    // General Properties
    type TypeId = TypeId of int with
        member this.Value = 
            this |> (fun (TypeId x) -> x)

    type Name = Name of string with
        member this.Value = 
            this |> (fun (Name x) -> x)

    type Volume = Volume of single with
        member this.Value = 
            this |> (fun (Volume x) -> x)
        

    type Compressed = 
    | IsCompressed
    | IsNotCompressed


    type OreRarity = 
    | Common 
    | Uncommon
    | Rare


    type OreType = 
    | Arkonor     of int
    | Bistot      of int  
    | Crokite     of int  
    | DarkOchre   of int  
    | Gneiss      of int  
    | Hedbergite  of int  
    | Hemorphite  of int  
    | Jaspet      of int  
    | Kernite     of int  
    | Mercoxit    of int  
    | Omber       of int  
    | Plagioclase of int  
    | Pyroxeres   of int  
    | Scordite    of int  
    | Spodumain   of int  
    | Veldspar    of int  
    
        
    type IceType = 
    | BlueIce             of int
    | ClearIcicle         of int
    | DarkGlitter         of int
    | EnrichedClearIcicle of int
    | Gelidus             of int
    | GlacialMass         of int
    | GlareCrust          of int
    | Krystallos          of int
    | PristineWhiteGlaze  of int
    | SmoothGlacialMass   of int
    | ThickBlueIce        of int 
    | WhiteGlaze          of int 
    
    
    type Id = Id of int with
        member this.Value = 
            this |> (fun (Id x) -> x)

    type OreId = OreId of Id with
        member this.Value = 
            this |> (fun (OreId (Id x)) -> x)

    type IceId = IceId of Id with
        member this.Value = 
            this |> (fun (IceId (Id x)) -> x)

    type Qty = Qty of int with
        member this.Value = 
            this |> (fun (Qty x) -> x)

    type OreQty = OreQty of Qty with
        member this.Value = 
            this |> (fun (OreQty (Qty x)) -> x)

    type IceQty = IceQty of Qty with
        member this.Value = 
            this |> (fun (IceQty (Qty x)) -> x)
    
    
    type Ice = 
    | RegularBlueIce
    | RegularThickBlueIce
    | RegularClearIcicle
    | RegularEnrichedClearIcicle
    | RegularGlacialMass
    | RegularSmoothGlacialMass
    | RegularWhiteGlaze
    | RegularPristineWhiteGlaze
    | RegularGlareCrust
    | RegularDarkGlitter
    | RegularGelidus
    | RegularKrystallos
    | CompressedBlueIce
    | CompressedThickBlueIce
    | CompressedClearIcicle
    | CompressedEnrichedClearIcicle
    | CompressedGlacialMass
    | CompressedSmoothGlacialMass
    | CompressedWhiteGlaze
    | CompressedPristineWhiteGlaze
    | CompressedGlareCrust
    | CompressedDarkGlitter
    | CompressedGelidus
    | CompressedKrystallos
    
    
    type Ore = 
    | CommonVeldspar    
    | UncommonVeldspar  
    | RareVeldspar      
    | CompressedCommonVeldspar   
    | CompressedUncommonVeldspar 
    | CompressedRareVeldspar     
    | CommonScordite    
    | UncommonScordite  
    | RareScordite      
    | CompressedCommonScordite    
    | CompressedUncommonScordite  
    | CompressedRareScordite      
    | CommonPyroxeres   
    | UncommonPyroxeres 
    | RarePyroxeres     
    | CompressedCommonPyroxeres   
    | CompressedUncommonPyroxeres 
    | CompressedRarePyroxeres     
    | CommonPlagioclase     
    | UncommonPlagioclase   
    | RarePlagioclase       
    | CompressedCommonPlagioclase    
    | CompressedUncommonPlagioclase  
    | CompressedRarePlagioclase      
    | CommonKernite         
    | UncommonKernite       
    | RareKernite           
    | CompressedCommonKernite   
    | CompressedUncommonKernite 
    | CompressedRareKernite     
    | CommonOmber           
    | UncommonOmber         
    | RareOmber             
    | CompressedCommonOmber   
    | CompressedUncommonOmber 
    | CompressedRareOmber     
    | CommonHedbergite      
    | UncommonHedbergite    
    | RareHedbergite        
    | CompressedCommonHedbergite   
    | CompressedUncommonHedbergite 
    | CompressedRareHedbergite     
    | CommonHemorphite      
    | UncommonHemorphite    
    | RareHemorphite        
    | CompressedCommonHemorphite   
    | CompressedUncommonHemorphite 
    | CompressedRareHemorphite     
    | CommonJaspet           
    | UncommonJaspet         
    | RareJaspet             
    | CompressedCommonJaspet    
    | CompressedUncommonJaspet  
    | CompressedRareJaspet      
    | CommonGneiss           
    | UncommonGneiss         
    | RareGneiss             
    | CompressedCommonGneiss   
    | CompressedUncommonGneiss 
    | CompressedRareGneiss     
    | CommonDarkOchre       
    | UncommonDarkOchre     
    | RareDarkOchre         
    | CompressedCommonDarkOchre   
    | CompressedUncommonDarkOchre 
    | CompressedRareDarkOchre     
    | CommonCrokite         
    | UncommonCrokite       
    | RareCrokite           
    | CompressedCommonCrokite  
    | CompressedUncommonCrokite
    | CompressedRareCrokite    
    | CommonBistot          
    | UncommonBistot        
    | RareBistot            
    | CompressedCommonBistot  
    | CompressedUncommonBistot
    | CompressedRareBistot  
    | CommonSpodumain       
    | UncommonSpodumain     
    | RareSpodumain         
    | CompressedCommonSpodumain      
    | CompressedUncommonSpodumain    
    | CompressedRareSpodumain   
    | CommonArkonor         
    | UncommonArkonor       
    | RareArkonor           
    | CompressedCommonArkonor   
    | CompressedUncommonArkonor 
    | CompressedRareArkonor     
    | CommonMercoxit        
    | UncommonMercoxit      
    | RareMercoxit          
    | CompressedCommonMercoxit  
    | CompressedUncommonMercoxit
    | CompressedRareMercoxit    