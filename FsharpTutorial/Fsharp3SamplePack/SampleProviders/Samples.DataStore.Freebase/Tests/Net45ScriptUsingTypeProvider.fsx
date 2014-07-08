// Test file showing sample Freebase scripting, without using an API key 


#r @"..\..\Debug\net45\Samples.DataStore.Freebase.dll"

module BasicDataScripting = 

    open System.Linq
    open Samples.DataStore.Freebase

    let data = FreebaseData.GetDataContext()
    data.DataContext.SendingRequest.Add (fun e -> printfn "request: %A" e.RequestUri)

    let chemistry = data.``Science and Technology``.Chemistry

    let elements = chemistry.``Chemical Elements`` |> Seq.toList
    let bonds = chemistry.``Chemical Bonds`` |> Seq.toList
    let aminoAcids = data.``Science and Technology``.Biology.``Amino Acids`` |> Seq.toList
    let actinium = chemistry.``Chemical Elements``.Individuals.Actinium

    query { for i in data.``Science and Technology``.Meteorology.``Tropical Cyclones`` do 
            where (i.Damages <> null ) 
            count}

    let findCountryByFifaCode = 
        query { for x in data.``Time and Space``.Location.Countries do 
                where (x.``FIFA Code`` = "AUS") 
                exactlyOne }

    data.``Time and Space``.Location.Countries.ApproximateCount()
    data.``Time and Space``.Location.Countries.Count()
    
    // Around three million books to search:
    data.``Arts and Entertainment``.Books.Books.ApproximateCount()
    
    // Around three million books to search:
    data.``Arts and Entertainment``.Film.Films.ApproximateCount()

    let dateFounded = data.``Time and Space``.Location.Countries.Individuals.Australia.``Date founded``
    let adjForm = data.``Time and Space``.Location.Countries.Individuals.Australia.``Adjectival form``.FirstOrDefault()
    let contains = data.``Time and Space``.Location.Countries.Individuals.Germany.Contains.FirstOrDefault().Name
    let exports = data.``Time and Space``.Location.Countries.Individuals.Germany.``Exports as percent of GDP``.FirstOrDefault()

