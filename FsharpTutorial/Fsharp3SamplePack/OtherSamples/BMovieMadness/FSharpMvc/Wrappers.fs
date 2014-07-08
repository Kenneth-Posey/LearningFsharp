namespace FSharpMvc.Utils

open System.Web.Security

 /// wrapper object for both data and error messages    
[<CLIMutable>] 
type DataOrError<'T> = 
    {
        data : 'T
        error : string[]
    }
    static member Data(d : 'T) = { data = d; error = null }
    static member Error(text : string) = {data = Unchecked.defaultof<_>; error = [|text|]}
    static member Errors(errors : string[]) = {data = Unchecked.defaultof<_>; error = errors}

[<AutoOpen>]
module Extensions =

    type System.Security.Principal.IPrincipal with
        /// helper extension method for getting guid version of the user key
        member user.GetProviderUserKeyAsGuid() = 
            let userName = user.Identity.Name
            let user = Membership.GetUser(userName)
            user.ProviderUserKey :?> System.Guid

/// wrapper to add a label to arbitrary data
[<CLIMutable>]
type LabeledValue<'T> = 
    {
        label : string
        value : string
        data : 'T
    }
    static member New label data = 
        {
            label = label
            value = label
            data = data
        }
