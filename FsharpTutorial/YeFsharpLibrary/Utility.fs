namespace Utility

module UtilityFunctions =
    
    let ComposeUrl (valuePairs:List<(string * string)>) =        
        let Composer (valuePairs:List<(string * string)>) =
            let rec ComposerRec (valuePairs:List<(string * string)>) (url:string) =
                match valuePairs.Length > 0 with
                | false -> url
                | true  -> 
                    let url = url + sprintf "&%s=%s" (fst valuePairs.[0]) (snd valuePairs.[0])
                    ComposerRec valuePairs.Tail url

            let url = sprintf "?%s=%s" (fst valuePairs.[0]) (snd valuePairs.[0])
            ComposerRec valuePairs.Tail url
            
        match valuePairs.Length > 0 with
        | false -> ""                     // No query string parameters
        | true  -> Composer valuePairs
