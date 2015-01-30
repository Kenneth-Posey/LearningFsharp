namespace EveOnline.Industry

module Blueprint = 
    open EveOnline.ProductDomain.Types
    
    type Efficiency = Efficiency of int with
        member this.Value =             
            this |> (fun (Efficiency x) -> x)
            
    type EfficiencyType = 
    | Material of Efficiency
    | Time     of Efficiency

    // If a blueprint has a limit of "none" then it's a BPO
    type Limit = Limit of int option with
        member this.Value =
            this |> (fun (Limit x) -> x)

    type Blueprint = {
        ME : EfficiencyType
        TE : EfficiencyType
        Materials : (TypeId * Qty) list
        PrintId : TypeId
        ResultId : TypeId
        RunLimit : Limit
    }
