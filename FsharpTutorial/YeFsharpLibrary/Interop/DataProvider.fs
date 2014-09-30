namespace EveOnlineInterop

module DataProvider = 
    
    open EveData
    open EveData.Ore.Types
    open EveData.Ice.Types

    type OreYieldClass (oreYield:OreYield) as this = 
        member this.Tritanium = oreYield.Tritanium
        member this.Pyerite   = oreYield.Pyerite
        member this.Mexallon  = oreYield.Mexallon
        member this.Isogen    = oreYield.Isogen
        member this.Nocxium   = oreYield.Nocxium
        member this.Megacyte  = oreYield.Megacyte
        member this.Zydrine   = oreYield.Zydrine
        member this.Morphite  = oreYield.Morphite


    type RawOreClass (ore:IRawOre) as this = 
        member private this.Ore = ore 
        member this.GetBase   () = this.Ore.GetBase()
        member this.GetBase5  () = this.Ore.GetBase5()
        member this.GetBase10 () = this.Ore.GetBase10()
        member this.IsTiny    () = this.Ore.IsTiny()
        member this.GetVolume () = this.Ore.GetVolume()
        member this.GetName   () = this.Ore.GetName()
        member this.GetYield  () = 
            new OreYieldClass (this.Ore.GetYield())


    type IceYieldClass (iceYield:IceYield) as this = 
        member this.HeavyWater          = iceYield.HeavyWater
        member this.HeliumIsotopes      = iceYield.HeliumIsotopes
        member this.HydrogenIsotopes    = iceYield.HydrogenIsotopes
        member this.LiquidOzone         = iceYield.LiquidOzone
        member this.NitrogenIsotopes    = iceYield.NitrogenIsotopes
        member this.OxygenIsotopes      = iceYield.OxygenIsotopes
        member this.StrontiumClathrates = iceYield.StrontiumClathrates


    type RawIceClass (ice:IRawIce) as this = 
        member private this.Ice = ice
        member this.GetBase   () = this.Ice.GetBase()
        member this.IsTiny    () = this.Ice.IsTiny()
        member this.GetVolume () = this.Ice.GetVolume()
        member this.GetName   () = this.Ice.GetName()
        member this.GetYield  () = 
            new IceYieldClass (this.Ice.GetYield())
        





