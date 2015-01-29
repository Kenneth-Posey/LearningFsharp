namespace EveOnline.Industry

module Blueprint = 
    
    type Quality = 
    | Original
    | Copy

    type EfficiencyType = 
    | Material
    | Time

    type Efficiency = Efficiency of int with
        member this.Value =             
            this |> (fun (Efficiency x) -> x)

    type Blueprint = {
        ME : EfficiencyType
        TE : EfficiencyType
        Quality : Quality
    }
