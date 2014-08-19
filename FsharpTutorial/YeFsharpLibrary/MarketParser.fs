namespace EveOnline

module MarketParser =
    open System
    open System.Text.RegularExpressions
    open System.Net

    let LoadUrl url = 
        use client = new WebClient()
        client.DownloadString( new Uri(url) )

    let ParseItem (text:string) (regex:Regex) =
        let parsed = regex.Match text

        match parsed.Success with
        | true -> parsed.Groups.[1].Value.Trim() , parsed.Groups.[2].Value.Trim()
        | false -> "" , ""

    let IsNotEmpty (x:string , y:string) =
        (x.Length > 0) && (y.Length > 0)

    let SplitOnNewline (text:string) (regex:Regex) =
        let splitted = text.Split [|'\n'|]

        Array.filter (fun (x, y) -> IsNotEmpty (x, y))
        <| [| // Filters out "empty" tuples (non-matches)
            for line in splitted do
                yield ParseItem (line.TrimEnd()) regex
        |] 

    let LoadItemsFromUrl url =
        let itemText = LoadUrl url
        let regex = new Regex "([0-9]{1,9})[ ]{6}([\w '.-_]*)"

        SplitOnNewline itemText regex