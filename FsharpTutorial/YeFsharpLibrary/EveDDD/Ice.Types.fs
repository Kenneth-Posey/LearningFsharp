namespace EveOnline

module IceTypes = 
    open EveOnline.ProductTypes
    
        
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
    
    