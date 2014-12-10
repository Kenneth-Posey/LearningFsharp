
namespace GithubNote

module GithubNote = 

    type Class = 
    | Frigate
    | Cruiser

    type Ships = 
    | Merlin 
    | Condor

    type Tier = 
    | Tier1
    | Tier2

    let Tier x = 
        match x with 
        | Tier1 -> "Tier 1"
        | Tier2 -> "Tier 2"

    type ShipName = ShipName of string with
        member this.Value = 
            this |> (fun (ShipName x) -> x)

    let ShipName x =
        ShipName <| match x with
        | Merlin -> "Merlin"
        | Condor -> "Condor"

    type Ship = {
        Class : Class
        Name : ShipName
        Tier : Tier
    }

    let Ship x = 
        let ShipFactory c t n = {
            Class = c
            Name = ShipName (n)
            Tier = t
        }

        match x with
        | Merlin -> (Merlin) |> ShipFactory (Frigate) (Tier1)
        | Condor -> (Condor) |> ShipFactory (Frigate) (Tier1)

    let name = (Ship Merlin).Name.Value