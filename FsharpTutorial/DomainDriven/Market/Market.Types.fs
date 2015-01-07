namespace EveOnline.MarketDomain

module Types = 
    open EveOnline.ProductDomain.Types
    open EveOnline.OreDomain.Types
    open EveOnline.IceDomain.Types
    open EveOnline.ProductDomain.Records

    
    type System =
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
        

    type Material = 
    | Mineral    of Mineral
    | IceProduct of IceProduct
    | OreType    of OreType
    | IceType    of IceType
    
                


