﻿namespace EveOnline.ProductDomain

module Types = 

    type Mineral = 
    | Isogen    
    | Megacyte  
    | Mexallon  
    | Morphite 
    | Nocxium   
    | Pyerite   
    | Tritanium 
    | Zydrine   
                
    type IceProduct = 
    | HeavyWater
    | HeliumIsotopes
    | HydrogenIsotopes
    | LiquidOzone
    | NitrogenIsotopes
    | OxygenIsotopes
    | StrontiumClathrates
    
    type Compressed = 
    | IsCompressed
    | IsNotCompressed

    type Refined = 
    | Mineral    of Mineral
    | IceProduct of IceProduct

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
