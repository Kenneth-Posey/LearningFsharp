namespace EveData

open EveData.RawMaterial

module Ice = 
    open EveData.RawMaterial
    open EveData.RawMaterialTypes
    open EveData.RawMaterialRecords
    
    let IceFactory (c:Compressed) (n:RawIce) :IceType=
        let data = IceData (n, c)
        {
            Name    = data.Name
            IceId   = data.IceId
            Qty     = data.IceQty
            Yield   = RawIceYield n 
            Volume  = RawIceVolume n c
            Ice     = n
        }

    let Ice (x:Ice) (q:Qty) :IceType = 
        match x with 
        | RegularBlueIce         -> (RawIce.BlueIce q.Value)        |> IceFactory (IsNotCompressed)
        | RegularClearIcicle     -> (RawIce.ClearIcicle q.Value)    |> IceFactory (IsNotCompressed)
        | RegularGlacialMass     -> (RawIce.GlacialMass q.Value)    |> IceFactory (IsNotCompressed)
        | RegularWhiteGlaze      -> (RawIce.WhiteGlaze q.Value)     |> IceFactory (IsNotCompressed)
        | RegularThickBlueIce          -> (RawIce.ThickBlueIce q.Value)           |> IceFactory (IsNotCompressed)
        | RegularEnrichedClearIcicle   -> (RawIce.EnrichedClearIcicle q.Value)    |> IceFactory (IsNotCompressed)
        | RegularSmoothGlacialMass     -> (RawIce.SmoothGlacialMass q.Value)      |> IceFactory (IsNotCompressed)
        | RegularPristineWhiteGlaze    -> (RawIce.PristineWhiteGlaze q.Value)     |> IceFactory (IsNotCompressed)
        | RegularGlareCrust      -> (RawIce.GlareCrust q.Value)     |> IceFactory (IsNotCompressed)
        | RegularDarkGlitter     -> (RawIce.DarkGlitter q.Value)    |> IceFactory (IsNotCompressed)
        | RegularGelidus         -> (RawIce.Gelidus q.Value)        |> IceFactory (IsNotCompressed)
        | RegularKrystallos      -> (RawIce.Krystallos q.Value)     |> IceFactory (IsNotCompressed)
        | CompressedBlueIce      -> (RawIce.BlueIce q.Value)        |> IceFactory (IsCompressed)
        | CompressedClearIcicle  -> (RawIce.ClearIcicle q.Value)    |> IceFactory (IsCompressed)
        | CompressedGlacialMass  -> (RawIce.GlacialMass q.Value)    |> IceFactory (IsCompressed)
        | CompressedWhiteGlaze   -> (RawIce.WhiteGlaze q.Value)     |> IceFactory (IsCompressed)
        | CompressedThickBlueIce         -> (RawIce.ThickBlueIce q.Value)           |> IceFactory (IsCompressed)
        | CompressedEnrichedClearIcicle  -> (RawIce.EnrichedClearIcicle q.Value)    |> IceFactory (IsCompressed)
        | CompressedSmoothGlacialMass    -> (RawIce.SmoothGlacialMass q.Value)      |> IceFactory (IsCompressed)
        | CompressedPristineWhiteGlaze   -> (RawIce.PristineWhiteGlaze q.Value)     |> IceFactory (IsCompressed)
        | CompressedGlareCrust   -> (RawIce.GlareCrust q.Value)     |> IceFactory (IsCompressed)
        | CompressedDarkGlitter  -> (RawIce.DarkGlitter q.Value)    |> IceFactory (IsCompressed)
        | CompressedGelidus      -> (RawIce.Gelidus q.Value)        |> IceFactory (IsCompressed)
        | CompressedKrystallos   -> (RawIce.Krystallos q.Value)     |> IceFactory (IsCompressed) 


    // Old OOP style version
    module Types = 
        type SimpleIce = {
            Name   : string
            TypeId : int
            Value  : single
            IsTiny : bool
        }

        type IceYield = {
            HeavyWater          : int
            HeliumIsotopes      : int
            HydrogenIsotopes    : int
            LiquidOzone         : int
            NitrogenIsotopes    : int
            OxygenIsotopes      : int
            StrontiumClathrates : int
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

        // The actual prices for the ice products
        type IceValue = {
            HeavyWater          : single
            HeliumIsotopes      : single
            HydrogenIsotopes    : single
            LiquidOzone         : single
            NitrogenIsotopes    : single
            OxygenIsotopes      : single
            StrontiumClathrates : single           
        }
        
        type IIce = 
            inherit IRawMat<IceYield>

        type IRawIce =
            inherit IIce

        type ICompressedIce = 
            inherit IRawIce
            
    
    module Functions =    
        open Types
        let RefineValueIce (multiplier:single) (item:IRawIce) (value:IceValue) = 
            let iceYield = item.GetYield ()
            (   single iceYield.HeavyWater          * value.HeavyWater
              + single iceYield.HeliumIsotopes      * value.HeliumIsotopes
              + single iceYield.HydrogenIsotopes    * value.HydrogenIsotopes
              + single iceYield.LiquidOzone         * value.LiquidOzone
              + single iceYield.NitrogenIsotopes    * value.NitrogenIsotopes
              + single iceYield.OxygenIsotopes      * value.OxygenIsotopes
              + single iceYield.StrontiumClathrates * value.StrontiumClathrates
            ) * multiplier

            
    module RawMaterials = 
        open Types
        type ClearIcicle () = 
            interface IRawIce with 
                override this.GetName   () = "Clear Icicle"
                override this.GetBase   () = ClearIcicle.Base
                override this.GetVolume () = ClearIcicle.Volume
                override this.GetYield  () = ClearIcicle.Yield
                override this.IsTiny    () = false

            static member val Base   = 16262 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 50
                    LiquidOzone         = 25
                    StrontiumClathrates = 1
                    HeliumIsotopes      = 300
                } with get
            

        type CompClearIcicle () = 
            interface ICompressedIce with 
                override this.GetName   () = "Compressed Clear Icicle"
                override this.GetBase   () = CompClearIcicle.Base
                override this.GetVolume () = CompClearIcicle.Volume
                override this.GetYield  () = CompClearIcicle.Yield
                override this.IsTiny    () = true

            static member val Base   = 28434 with get
            static member val Volume = 100.0f with get
            static member val Yield  = ClearIcicle.Yield with get
            

        type EnrichedClearIcicle () = 
            interface IRawIce with 
                override this.GetName   () = "Enriched Clear Icicle"
                override this.GetBase   () = EnrichedClearIcicle.Base
                override this.GetVolume () = EnrichedClearIcicle.Volume
                override this.GetYield  () = EnrichedClearIcicle.Yield
                override this.IsTiny    () = false

            static member val Base   = 17978 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 75
                    LiquidOzone         = 40
                    StrontiumClathrates = 1
                    HeliumIsotopes      = 350
                } with get
            

        type CompEnrichedClearIcicle () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Enriched Clear Icicle"
                override this.GetBase   () = CompEnrichedClearIcicle.Base
                override this.GetVolume () = CompEnrichedClearIcicle.Volume
                override this.GetYield  () = CompEnrichedClearIcicle.Yield
                override this.IsTiny    () = true

            static member val Base   = 28436 with get
            static member val Volume = 100.0f with get
            static member val Yield  = EnrichedClearIcicle.Yield with get


        type GlacialMass () = 
            interface IRawIce with 
                override this.GetName   () = "Glacial Mass"
                override this.GetBase   () = GlacialMass.Base
                override this.GetVolume () = GlacialMass.Volume
                override this.GetYield  () = GlacialMass.Yield
                override this.IsTiny    () = false

            static member val Base   = 16263 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 50
                    LiquidOzone         = 25
                    StrontiumClathrates = 1
                    HydrogenIsotopes    = 300
                } with get

                
        type CompGlacialMass () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Glacial Mass"
                override this.GetBase   () = CompGlacialMass.Base
                override this.GetVolume () = CompGlacialMass.Volume
                override this.GetYield  () = CompGlacialMass.Yield
                override this.IsTiny    () = true

            static member val Base   = 28438 with get
            static member val Volume = 100.0f with get
            static member val Yield  = GlacialMass.Yield with get


        type SmoothGlacialMass () = 
            interface IRawIce with 
                override this.GetName   () = "Smooth Glacial Mass"
                override this.GetBase   () = SmoothGlacialMass.Base
                override this.GetVolume () = SmoothGlacialMass.Volume
                override this.GetYield  () = SmoothGlacialMass.Yield
                override this.IsTiny    () = false

            static member val Base   = 17977 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 75
                    LiquidOzone         = 40
                    StrontiumClathrates = 1
                    HydrogenIsotopes    = 350
                } with get


        type CompSmoothGlacialMass () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Smooth Glacial Mass"
                override this.GetBase   () = CompSmoothGlacialMass.Base
                override this.GetVolume () = CompSmoothGlacialMass.Volume
                override this.GetYield  () = CompSmoothGlacialMass.Yield
                override this.IsTiny    () = true

            static member val Base   = 28442 with get
            static member val Volume = 100.0f with get
            static member val Yield  = SmoothGlacialMass.Yield with get

                
        type BlueIce () = 
            interface IRawIce with 
                override this.GetName   () = "Blue Ice"
                override this.GetBase   () = BlueIce.Base
                override this.GetVolume () = BlueIce.Volume
                override this.GetYield  () = BlueIce.Yield
                override this.IsTiny    () = false

            static member val Base   = 16264 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 50
                    LiquidOzone         = 25
                    StrontiumClathrates = 1
                    OxygenIsotopes      = 300
                } with get

                
        type CompBlueIce () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Blue Ice"
                override this.GetBase   () = CompBlueIce.Base
                override this.GetVolume () = CompBlueIce.Volume
                override this.GetYield  () = CompBlueIce.Yield
                override this.IsTiny    () = true

            static member val Base   = 28433 with get
            static member val Volume = 100.0f with get
            static member val Yield  = BlueIce.Yield with get

                
        type ThickBlueIce () = 
            interface IRawIce with 
                override this.GetName   () = "Thick Blue Ice"
                override this.GetBase   () = ThickBlueIce.Base
                override this.GetVolume () = ThickBlueIce.Volume
                override this.GetYield  () = ThickBlueIce.Yield
                override this.IsTiny    () = false

            static member val Base   = 17975 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 75
                    LiquidOzone         = 40
                    StrontiumClathrates = 1
                    OxygenIsotopes      = 350
                } with get

                
        type CompThickBlueIce () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Thick Blue Ice"
                override this.GetBase   () = CompThickBlueIce.Base
                override this.GetVolume () = CompThickBlueIce.Volume
                override this.GetYield  () = CompThickBlueIce.Yield
                override this.IsTiny    () = true

            static member val Base   = 28443 with get
            static member val Volume = 100.0f with get
            static member val Yield  = ThickBlueIce.Yield with get


        type WhiteGlaze () = 
            interface IRawIce with 
                override this.GetName   () = "White Glaze"
                override this.GetBase   () = WhiteGlaze.Base
                override this.GetVolume () = WhiteGlaze.Volume
                override this.GetYield  () = WhiteGlaze.Yield
                override this.IsTiny    () = false

            static member val Base   = 16265 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 50
                    LiquidOzone         = 25
                    StrontiumClathrates = 1
                    NitrogenIsotopes    = 300
                } with get


        type CompWhiteGlaze () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed White Glaze"
                override this.GetBase   () = CompWhiteGlaze.Base
                override this.GetVolume () = CompWhiteGlaze.Volume
                override this.GetYield  () = CompWhiteGlaze.Yield
                override this.IsTiny    () = true

            static member val Base   = 28444 with get
            static member val Volume = 100.0f with get
            static member val Yield  = WhiteGlaze.Yield with get


        type PristineWhiteGlaze () = 
            interface IRawIce with 
                override this.GetName   () = "Pristine White Glaze"
                override this.GetBase   () = PristineWhiteGlaze.Base
                override this.GetVolume () = PristineWhiteGlaze.Volume
                override this.GetYield  () = PristineWhiteGlaze.Yield
                override this.IsTiny    () = false

            static member val Base   = 17976 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 75
                    LiquidOzone         = 40
                    StrontiumClathrates = 1
                    NitrogenIsotopes    = 350
                } with get


        type CompPristineWhiteGlaze () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Pristine White Glaze"
                override this.GetBase   () = CompPristineWhiteGlaze.Base
                override this.GetVolume () = CompPristineWhiteGlaze.Volume
                override this.GetYield  () = CompPristineWhiteGlaze.Yield
                override this.IsTiny    () = true

            static member val Base   = 28441 with get
            static member val Volume = 100.0f with get
            static member val Yield  = PristineWhiteGlaze.Yield with get


        type GlareCrust () = 
            interface IRawIce with 
                override this.GetName   () = "Glare Crust"
                override this.GetBase   () = GlareCrust.Base
                override this.GetVolume () = GlareCrust.Volume
                override this.GetYield  () = GlareCrust.Yield
                override this.IsTiny    () = false

            static member val Base   = 16266 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 1000
                    LiquidOzone         = 500
                    StrontiumClathrates = 25
                } with get


        type CompGlareCrust () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Glare Crust"
                override this.GetBase   () = CompGlareCrust.Base
                override this.GetVolume () = CompGlareCrust.Volume
                override this.GetYield  () = CompGlareCrust.Yield
                override this.IsTiny    () = true

            static member val Base   = 28439 with get
            static member val Volume = 100.0f with get
            static member val Yield  = GlareCrust.Yield with get


        type DarkGlitter () = 
            interface IRawIce with 
                override this.GetName   () = "Dark Glitter"
                override this.GetBase   () = DarkGlitter.Base
                override this.GetVolume () = DarkGlitter.Volume
                override this.GetYield  () = DarkGlitter.Yield
                override this.IsTiny    () = false

            static member val Base   = 16267 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 500
                    LiquidOzone         = 1000
                    StrontiumClathrates = 50
                } with get


        type CompDarkGlitter () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Dark Glitter"
                override this.GetBase   () = CompDarkGlitter.Base
                override this.GetVolume () = CompDarkGlitter.Volume
                override this.GetYield  () = CompDarkGlitter.Yield
                override this.IsTiny    () = true

            static member val Base   = 28435 with get
            static member val Volume = 100.0f with get
            static member val Yield  = DarkGlitter.Yield with get


        type Gelidus () = 
            interface IRawIce with 
                override this.GetName   () = "Gelidus"
                override this.GetBase   () = Gelidus.Base
                override this.GetVolume () = Gelidus.Volume
                override this.GetYield  () = Gelidus.Yield
                override this.IsTiny    () = false

            static member val Base   = 16268 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 250
                    LiquidOzone         = 500
                    StrontiumClathrates = 75
                } with get


        type CompGelidus () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Gelidus"
                override this.GetBase   () = CompGelidus.Base
                override this.GetVolume () = CompGelidus.Volume
                override this.GetYield  () = CompGelidus.Yield
                override this.IsTiny    () = true

            static member val Base   = 28437 with get
            static member val Volume = 100.0f with get
            static member val Yield  = Gelidus.Yield with get


        type Krystallos () = 
            interface IRawIce with 
                override this.GetName   () = "Krystallos"
                override this.GetBase   () = Krystallos.Base
                override this.GetVolume () = Krystallos.Volume
                override this.GetYield  () = Krystallos.Yield
                override this.IsTiny    () = false

            static member val Base   = 16268 with get
            static member val Volume = 1000.0f with get
            static member val Yield  = {
                    BaseIceYield with 
                    HeavyWater          = 125
                    LiquidOzone         = 250
                    StrontiumClathrates = 125
                } with get


        type CompKrystallos () = 
            interface IRawIce with 
                override this.GetName   () = "Compressed Krystallos"
                override this.GetBase   () = CompKrystallos.Base
                override this.GetVolume () = CompKrystallos.Volume
                override this.GetYield  () = CompKrystallos.Yield
                override this.IsTiny    () = true

            static member val Base   = 28440 with get
            static member val Volume = 100.0f with get
            static member val Yield  = Krystallos.Yield with get
            