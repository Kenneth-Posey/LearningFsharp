namespace designing_with_types

//    // Generic version?
//    type Union<'a> = Union of 'a with
//        member this.Value = 
//            this |> (fun (Union x) -> x)

module designing_with_types =
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

    type Tier = 
    | Tier1
    | Tier2
    | Tier3
    | Navy
    | Pirate

    type Ships = 
    | Merlin
    | Condor
    | Bantam
    | Hawk
    | Velator

    let Tier x = 
        match x with
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
        | Merlin -> 100 
        | Condor -> 200
        | Bantam -> 300
        | Hawk -> 400
        | Velator -> 0
    
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
                Tier = t
                ShipName = ShipName n
                ShipId = ShipId n
            }

        match x with
        | Merlin  -> (Merlin) |> ShipFactory (Frigate) (Caldari) (Tier1)
        | Condor  -> (Condor) |> ShipFactory (Frigate) (Caldari) (Tier1)
        | Bantam  -> (Bantam) |> ShipFactory (Frigate) (Caldari) (Tier1)
        | Hawk    -> (Hawk) |> ShipFactory (Frigate) (Caldari) (Tier2)
        | Velator -> (Velator) |> ShipFactory (Frigate) (Gallente) (Tier1)


    let name = (Ship Merlin).ShipName.Value
    let tier = Tier (Ship Merlin).Tier

    let hawk = (Ship Hawk).ShipName.Value