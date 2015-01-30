namespace EveOnline.Geography

module System =     
    type SystemId = SystemId of int with
        member this.Value = 
            this |> (fun (SystemId x) -> x)

    type SystemName = SystemName of string with
        member this.Value =     
            this |> (fun (SystemName x) -> x)

    type TradeHub =
    | Jita    
    | Dodixie 
    | Amarr   
    | Hek     
    | Rens    
    
    let SystemId x = 
        match x with
        | Jita    -> 30000142
        | Dodixie -> 30002659
        | Amarr   -> 30002187
        | Hek     -> 30002053
        | Rens    -> 30002510

    let SystemName x = 
        match x with
        | Jita    -> "Jita"
        | Dodixie -> "Dodixie"
        | Amarr   -> "Amarr"
        | Hek     -> "Hek"
        | Rens    -> "Rens"


    let Locations = [
        Jita    
        Amarr   
        Dodixie 
        Rens    
        Hek     
    ]
        