namespace EveOnline

module EveData = 
    type SystemName =
        | Jita    = 30000142
        | Dodixie = 30002659
        | Amarr   = 30002187
        | Hek     = 30002053
        | Rens    = 30002510

    type MarketAPIRequest = {
        typeid      : int
        usesystem   : int
        regionlimit : int
        sethours    : int
        setminQ     : int
        }

    let TypeIdUrl   = "http://eve-files.com/chribba/typeid.txt"
    let QuickLook   = "http://api.eve-central.com/api/quicklook"

    module RawMaterials =    
        type Minerals =
        | Tritanium = 34
        | Pyerite   = 35
        | Mexallon  = 36
        | Isogen    = 37
        | Nocxium   = 38
        | Zydrine   = 39
        | Megacyte  = 40

        type Veldspar =
        | Default      = 1230
        | Concentrated = 17470
        | Dense        = 17471

        type CompVeldspar =
        | Default      = 28430
        | Concentrated = 28431
        | Dense        = 28432
        
        type Scordite =
        | Default   = 1228
        | Condensed = 17463
        | Massive   = 17464

        type CompScordite =
        | Default   = 28427
        | Condensed = 28428
        | Massive   = 28429

        type Pyroxeres =
        | Default   = 1224
        | Solid     = 17459
        | Viscous   = 17460

        type CompPyroxeres =
        | Default   = 28424
        | Solid     = 28425
        | Viscous   = 28426
        
        type Hedbergite =
        | Default   = 21
        | Vitric    = 17440
        | Glazed    = 17441

        type CompHedbergite = 
        | Default   = 28400
        | Vitric    = 28401
        | Glazed    = 28402

        type Hemorphite = 
        | Default   = 1231
        | Vivid     = 17444
        | Radiant   = 17445

        type CompHemorphite = 
        | Default   = 18403
        | Vivid     = 28404
        | Radiant   = 28405

        type Jaspet = 
        | Default   = 1226
        | Pure      = 17448
        | Pristine  = 17449

        type CompJaspet = 
        | Default   = 28406
        | Pure      = 28407
        | Pristine  = 28408

                    

module MarketParser =
    open System
    open System.Text.RegularExpressions
    open System.Net

    let LoadUrl (url:string) = 
        use client = new WebClient()
        client.DownloadString( new Uri(url) )

    let ParseItem (text:string) (regex:Regex) =
        let parsed = regex.Match text
        match parsed.Success with
        | true  -> parsed.Groups.[1].Value.Trim() , parsed.Groups.[2].Value.Trim()
        | false -> "" , ""
        // If it is unable to match then it returns 
        // an empty tuple to be filtered out later

    let IsNotEmpty (x:string , y:string) =
        (x.Length > 0) && (y.Length > 0)
        
    let FilterEmpty (text:(string * string)[]) =
        text |> Array.filter (fun (x, y) -> IsNotEmpty (x, y))

    let SplitOnNewline (text:string) (regex:Regex) =
        [| 
            for line in text.Split [|'\n'|] do
                yield ParseItem (line.TrimEnd()) regex
        |] 

    let LoadTypeIdsFromUrl (url:string) =
        new Regex "([0-9]{1,9})[ ]{6}([\w '.-_]*)"
        |> SplitOnNewline (LoadUrl url)
        |> FilterEmpty