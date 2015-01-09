namespace designing_with_types

//    // Generic version?
//    type Union<'a> = Union of 'a with
//        member this.Value = 
//            this |> (fun (Union x) -> x)

module designing_with_types =

    type Chasses = 
    | Sedan
    | Truck
    
    type Makes = 
    | Ford
    | Dodge
    
    type Chasse = Chasse of string with
        member this.Value = 
            this |> (fun (Chasse x) -> x)

    let Chasse x = 
        Chasse <| match x with
        | Sedan -> "Sedan"
        | Truck -> "Truck"

    type Make = Make of string with
        member this.Value = 
            this |> (fun (Make x) -> x)

    let Make x = 
        Make <| match x with
        | Ford -> "Ford"
        | Dodge -> "Dodge"
        
    type Model = 
    | F150
    | Taurus
    | Ram
    | Viper

    type Car = {
        Chasse : Chasse
        Make : Make
        Model : Model
    }

    let Car x = 
        let CarFactory c ma mo = 
            {
                Chasse = Chasse c
                Make = Make ma
                Model = mo
            }

        match x with
        | F150   -> F150   |> CarFactory Truck Ford
        | Taurus -> Taurus |> CarFactory Sedan Ford
        | Ram    -> Ram    |> CarFactory Truck Dodge
        | Viper  -> Viper  |> CarFactory Sedan Dodge


    
    type Tiers = 
    | Tier1
    | Tier2
    | Tier3
    | Navy
    | Pirate

//    type Hull = 
//    | Frigate
//    | Destroyer
//    | Cruiser
//    | BattleCruiser
//    | Battleship 
//    | Dreadnaught
//    | Carrier
//    | SuperCarrier
//    | Titan

    type Class = 
    | Frigate        
    | Destroyer     
    | Cruiser       
    | BattleCruiser 
    | Battleship    
    | Dreadnaught   
    | Carrier       
    | SuperCarrier  
    | Titan         

    type Race = 
    | Amarr
    | Caldari
    | Gallente
    | Minmatar

    type Ships = 
    | Merlin
    | Condor
    | Bantam
    | Hawk
    | Velator
    
    type Tier = Tier of string with
        member this.Value = 
            this |> (fun (Tier x) -> x)
            
    let Tier x = 
        Tier <| match x with
        | Tier1 -> "Tier 1"
        | Tier2 -> "Tier 2"
        | Tier3 -> "Tier 3"
        | Navy -> "Navy"
        | Pirate -> "Pirate"    
        
    // "Unwrapper" for ship name properties
    type ShipName = ShipName of string with
        member this.Value = 
            this |> (fun (ShipName x) -> x)

    let ShipName x =
        ShipName <| match x with 
        | Merlin -> "Merlin"
        | Condor -> "Condor"
        | Bantam -> "Bantam"
        | Hawk -> "Hawk"
        | Velator -> "Velator"

    // "Unwrapper" for ship id properties
    type ShipId = ShipId of int with
        member this.Value = 
            this |> (fun (ShipId x) -> x)

    let ShipId x = 
        ShipId <| match x with 
        | Merlin -> 603 
        | Condor -> 200
        | Bantam -> 300
        | Hawk -> 400
        | Velator -> 0
    
//    type ShipClass = ShipClass of Class with
//        member this.Value = 
//            this |> (fun (ShipClass (x)) -> x)
//
//    let ShipClass (x, y) = 
//        ShipClass <| match x, y with
//        | Hull.Frigate, Tier1 -> Class.Frigate
//        | Hull.Frigate, Tier2

    type Ship = {
        Class : Class
        Race : Race
        Tier : Tier
        ShipName : ShipName
        ShipId : ShipId
    }
    
    let Ship x = 
        let ShipFactory c r t n =
            { 
                Class = c
                Race = r
                Tier = Tier t
                ShipName = ShipName n
                ShipId = ShipId n
            }

        match x with
        | Merlin  -> (Merlin) |> ShipFactory (Frigate) (Caldari) (Tier1)
        | Condor  -> (Condor) |> ShipFactory (Frigate) (Caldari) (Tier1)
        | Bantam  -> (Bantam) |> ShipFactory (Frigate) (Caldari) (Tier1)
        | Hawk    -> (Hawk) |> ShipFactory (Frigate) (Caldari) (Tier2)
        | Velator -> (Velator) |> ShipFactory (Frigate) (Gallente) (Tier1)

    let merlin = Ship Merlin
    let name = merlin.ShipName.Value
    let tier = merlin.Tier.Value
