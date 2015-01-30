namespace EveOnline.Industry

module Job = 
    
    // Length in seconds
    type JobLength = JobLength of int with
        member this.Value = 
            this |> (fun (JobLength x) -> x)



    type JobType = 
    | ResearchME
    | ResearchTE
    | Copy
    | Invent
    | Manufacture
    | ReverseEngineer


