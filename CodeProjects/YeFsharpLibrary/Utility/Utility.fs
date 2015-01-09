namespace Utility

module UtilityFunctions =
    open System
    open System.Text.RegularExpressions
    open System.Net
    
    let LoadUrl (url:string) = 
        use client = new WebClient()
        client.DownloadString( new Uri(url) )

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
            
    let IsNotEmpty (x:string , y:string) =
        (x.Length > 0) && (y.Length > 0)
        
    let FilterEmpty (text:List<(string * string)>) =
        text |> List.filter ( fun (x, y) -> IsNotEmpty (x, y) )
        
    let ParseLine (text:string) (regex:Regex) =
        let parsed = regex.Match text
        match parsed.Success with
        | true  -> parsed.Groups.[1].Value.Trim() , parsed.Groups.[2].Value.Trim()
        | false -> "" , ""
        // If it is unable to match then it returns 
        // an empty tuple to be filtered out later

    let SplitOnNewline (text:string) (regex:Regex) =
        [| 
            for line in text.Split [|'\n'|] do
                yield ParseLine (line.TrimEnd()) regex
        |] 
    
    let FilterByName (name:string) (tuple:List<(string * string)>) =
        tuple
        |> List.filter ( fun (x, y) -> y.Contains(name) )

    let FilterToOreOnly (tuple:List<(string * string)>) = 
        tuple
        |> List.filter ( fun (x, y) -> 
            y.Contains("Blueprint") && y.Contains("Processing") && y.Contains("Mining") 
                = false )
    

    let LoadTypeIdsFromUrl (url:string) =
        new Regex "([0-9]{1,9})[ ]{6}([\w '.-_]*)"
        |> SplitOnNewline (LoadUrl url)
        |> List.ofArray
        |> FilterEmpty
        