

#r @"..\..\Debug\net45\Samples.DataStore.Freebase.dll"

[<Literal>]
let API_KEY = "<enter your freebase-enabled google API key here, you will get errors before you do>"

module ApiWithKeyAndSnapshotData = 

    type T1 = Samples.DataStore.Freebase.FreebaseDataProvider<SnapshotDate="2009-09-21",Key=API_KEY>
    let data = T1.GetDataContext()
    data.DataContext.UseLocalCache <- false
    data.DataContext.SendingRequest.Add (fun e -> printfn "request: %A" e.RequestUri)

    // Obama is here:
    let presidents = query { for e in data.Society.Government.``US Presidents`` do select e.Name } |> Seq.toList

    data.``Science and Technology``.Chemistry.``Chemical Elements`` |> Seq.toList
    data.``Science and Technology``.Chemistry.``Chemical Elements``.Individuals.Actinium.``Boiling Point``

    // Go to 2008:
    type T2 = Samples.DataStore.Freebase.FreebaseDataProvider<SnapshotDate="2008-09-21",Key=API_KEY>
    let ctxt2 = T2.GetDataContext()
    ctxt2.DataContext.UseLocalCache <- false
    ctxt2.DataContext.SendingRequest.Add (fun e -> printfn "request: %A" e.RequestUri)

    let presidents2 = query { for e in ctxt2.Society.Government.``US Presidents`` do select e.Name } |> Seq.toList

        // Note, no Obama!



module MoreRandomScripting = 

    open Samples.DataStore.Freebase
    open System.Linq

    type T = Samples.DataStore.Freebase.FreebaseDataProvider<Key=API_KEY>
    let data = T.GetDataContext()
    data.DataContext.UseLocalCache <- false

    data.DataContext.SendingRequest.Add (fun s -> printfn "requesting: %A" s.RequestUri)

    query { for e in data.``Science and Technology``.Chemistry.``Chemical Elements`` do
            count }

    data.``Science and Technology``.Chemistry.``Chemical Elements`` |> Seq.toList
    data.DataContext.LocalCacheLocation


    let biology = data.``Science and Technology``.Biology
    let computers = data.``Science and Technology``.Computers
    let chemistry = data.``Science and Technology``.Chemistry
    let astronomy = data.``Science and Technology``.Astronomy
    let books = data.``Arts and Entertainment``.Books

    biology.``Amino Acids`` |> Seq.toList
    biology.``Animal breeds`` |> Seq.toList
    biology.``Animal owners`` |> Seq.toList
    biology.Animals |> Seq.toList
    biology.``Breed groups`` |> Seq.toList
    biology.``Breed origins`` |> Seq.toList
    biology.``Breed registries`` |> Seq.toList
    biology.Chromosomes |> Seq.toList
    biology.``Cytogenetic Bands`` |> Seq.toList
    biology.``Deceased Organisms`` |> Seq.toList
    biology.``Domesticated animals`` |> Seq.toList
    biology.``Fossil sites`` |> Seq.toList
    biology .``Fossil specimens`` |> Seq.toList
    biology.``Gene Group Membership Evidences``.Count() 
    biology.``Gene Group Membership Evidences``.ApproximateCount()
    biology.``Gene Groups``.Count()
    biology.``Gene Ontology Data Sources`` |> Seq.toList
    biology.``Gene Ontology Group Membership Evidence Types`` |> Seq.toList
    biology.``Gene Ontology Group Types`` |> Seq.toList
    biology.``Gene Ontology Groups``.Count()
    biology.Genes.Count()
    biology.``Genome Builds`` |> Seq.toList
    biology.``Genome Curators`` |> Seq.toList
    biology.Genomes |> Seq.toList
    biology.``Genomic Locus``.Count()
    biology.``Hybrid parent genders`` |> Seq.toList
    biology.``Hybrid parent classifications`` |> Seq.toList
    biology.Hybrids |> Seq.toList
    biology.``Informal biological groupings`` |> Seq.toList
    biology.``Organism Classification Ranks`` |> Seq.toList
    biology.``Organism Classifications``.Count()  // 202,000!!!!
    biology.``Organisms``.Count()  // 5212
    query { for x in biology.``Organism Classifications`` do take 100 } |> Seq.toList
    query { for x in biology.``Organisms`` do take 100 } |> Seq.toList
    biology.``Organism parts`` |> Seq.toList
    biology.``Owned animals``.Count()
    biology.``Pedigreed animals``.Count()
    biology.``Plant Disease Causes``.Count()
    biology.``Plant Disease Conditions``.Count()
    biology.``Plant Disease Hosts``.Count()
    biology.``Plant Diseases``.Count()
    biology.``Plant disease documentations``.Count()
    biology.Proteins.Count()

    data.``Science and Technology``.Astronomy.Asteroids.Count()
    data.``Science and Technology``.Astronomy.Asteroids.Take(400) |> Seq.toList
    data.``Science and Technology``.Chemistry.``Chemical Element Discoverers`` |> Seq.toList
    data.``Science and Technology``.Computers.``Computer Designers`` |> Seq.toList
    data.``Science and Technology``.Computers.Computers |> Seq.toList

    chemistry.``Chemical Elements`` 
      |> Seq.where (fun e -> e.``Atomic number``.HasValue)
      |> Seq.sortBy (fun e -> e.``Atomic number``.GetValueOrDefault())
      |> Seq.toList

    chemistry.``Chemical Elements`` 
       |> Seq.where (fun e -> e.``Atomic number``.HasValue)
       |> Seq.sortBy (fun e -> e.``Atomic number``.GetValueOrDefault())
       |> Seq.map (fun e -> e.Name, Seq.length e.Isotopes)
       |> Seq.toList

    data.``Science and Technology``.Chemistry.``Chemical Elements`` |> Seq.toList
    data.Sports.Baseball.``Baseball Teams``.Count()

    //--------------------------------------------------------------------------------------
    // Using remote-executing queries instead....

    query { for e in chemistry.``Chemical Elements`` do
            where e.``Atomic number``.HasValue
            sortBy e.``Atomic number``.Value }
      |> Seq.toList


    query { for e in chemistry.``Chemical Elements`` do
            where e.``Atomic number``.HasValue
            sortBy e.``Atomic number``.Value 
            count }

    query { for e in chemistry.``Chemical Elements`` do
            where e.``Atomic number``.HasValue
            sortBy e.``Atomic number``.Value
            select (e.Name, e.Isotopes.Count()) }
      |> Seq.toList




//data.DataContext.SendingRequest.Add(fun e -> printfn "requesting '%A'" e.RequestUri) 
//data.DataContext.UseLocalCache <- false


    query { for x in data.Commons.Meteorology.``Tropical Cyclones`` do
            where (x.Damages <> null) } 
        |> Seq.length

    query {
        for x in data.Commons.Meteorology.``Tropical Cyclones`` do
        where (x.Damages <> null)
        select (x.Name, x.Damages.Currency) }
      |> Seq.length

    query { for e in computers.``Computer Scientists`` do
            where (e.Name.ApproximatelyMatches "Jones")
            sortBy e.Name }
      |> Seq.length

    query { for e in chemistry.``Chemical Elements`` do
            where e.``Atomic number``.HasValue
            sortBy e.``Atomic number``.Value }
      |> Seq.toList

    query { for e in chemistry.``Chemical Elements`` do
            where e.``Atomic number``.HasValue
            sortBy e.Name
            select e.``Boiling Point`` }
      |> Seq.length

    query { for e in computers.``Computer Scientists`` do
            where (e.Name.ApproximatelyMatches "Jones")
            sortBy e.Name }
      |> Seq.length

    query { for e in computers.``Computer Scientists`` do
            where (e.Name.ApproximatelyMatches "Jones")
            count }

    query { for e in books.Books do
            where (e.Name.ApproximatelyMatches "Jones")
            count }

    [ for i in 1980..2012 -> 
        let s = (string i)
        i, query { for e in books.Books do
                   where (e.Name.ApproximatelyMatches s)
                   count } ]


    let cyclones = [ for x in data.Commons.Meteorology.``Tropical Cyclones`` -> x ]



    query { for b in books.Books do 
            where (b.Name.ApproximatelyMatches "Uranium") 
            select b }
      |> Seq.toList

    open Microsoft.FSharp.Linq.NullableOperators


    query { for i in chemistry.Isotopes do 
            where (i.``Mass number`` ?<= 4)
            groupBy (i.``Isotope of``.``Atomic number``) }
        |> Seq.toList


    query { for i in chemistry.Isotopes do 
            where (i.``Mass number``.Value <= 4)
            groupBy (i.``Isotope of``.``Atomic number``) }
        |> Seq.toList

    query { for i in chemistry.Isotopes do 
            where (i.``Mass number`` ?<= 4)
            join e in chemistry.``Chemical Elements``  on (i.``Isotope of``.Name = e.Name) 
            select i.Name }
        |> Seq.toList


    let elements = chemistry.``Chemical Elements``  
    let isotopes = chemistry.Isotopes
    // TODO: specific overloads of Where etc. for collection types
    //elements.Where(fun x -> x.Symbol = "H")

    query { for elem in (elements |> Seq.readonly) do 
            where (elem.Symbol = "U") 
            select elem }
        |> Seq.toList

    query { for elem in elements do 
            where (elem.Symbol = "U") 
            select elem }
        |> Seq.toList

    query { for elem in elements.AsEnumerable() do 
            where (elem.Symbol = "U") 
            select elem }
        |> Seq.toList

    query { for elem in elements do 
            where (elem.Symbol.ApproximatelyMatches("C")) 
            select elem }
        |> Seq.toList

    query { for elem in elements do 
            sortByNullable elem.``Atomic number``
            select elem }
        |> Seq.toList

    query { for elem in elements do 
            sortByNullableDescending elem.``Atomic number``
            select elem }
        |> Seq.toList

    query { for i in isotopes do 
            sortByNullable i.``Mass number``
            select i }
        |> Seq.toList

    query { for i in isotopes do 
            sortByNullable i.``Mass number``
            thenBy i.Name }
        |> Seq.toList

    query { for i in isotopes do 
            sortBy i.Name }
        |> Seq.toList

// MINOR FAILURE
//query { for elem in elements do 
//        where (elem.Symbol.ApproximatelyOneOf()) 
//        select elem }

// OK - timeout
//query { for i in books do
//        sortBy i.Name }

    query { for elem in elements do 
            where (elem.Symbol.ApproximatelyMatches("C*")) 
            select elem }
        |> Seq.toList

    // TODO: nested query on compund object should be formatted correctly

    query { for elem in elements do 
            where (elem.Symbol.ApproximatelyOneOf("U")) 
            select elem }
        |> Seq.toList

    query { for elem in elements do 
            where (elem.Symbol.ApproximatelyOneOf("U", "Na")) 
            select elem }
        |> Seq.toList

    query { for elem in elements do 
            where (elem.Symbol = "U") 
            select elem.Name }
        |> Seq.toList

    query { for elem in elements do 
            where (elem.Symbol = "U") 
            select (elem.Name, elem.``Atomic mass``) }
        |> Seq.toList

    query { for elem in elements do 
            select elem.``Atomic mass`` } 
        |> Seq.toList

    query { for elem in elements do 
            select (elem.Name, elem.Discoverer) } 
        |> Seq.toList

    query { for elem in elements do 
            let discoverers = query { for x in elem.Discoverer -> x }
            select (elem.Name, discoverers) } 
        |> Seq.toList

    query { for elem in elements do 
            where (elem.Discoverer.Count() > 0)
            select (elem.Name, elem.Discoverer) } 
        |> Seq.toList

    query { for elem in elements do 
            where (elem.Symbol = "Na") 
            select elem }
        |> Seq.toList

    query { for elem in elements do 
            where (elem.Symbol = "H") 
            select elem }
        |> Seq.toList

    query { for elem in elements do 
            where (elem.Name = "Hydrogen") 
            select elem }
        |> Seq.toList

    query { for elem in elements do 
            where (elem.Name.ApproximatelyMatches "*anium") 
            select elem }
        |> Seq.toList

    query { for book in books.Books do 
            where (book.``Date written`` = "1998")
            select book }
        |> Seq.toList

    query { for elem in elements do 
            where (elem.``Atomic number`` ?= 6)
            select elem }
            |> Seq.toList

    query { for elem in elements do 
            where (elem.``Atomic number`` ?<> 6)
            select elem }
        |> Seq.toList

    query { for elem in elements do 
            where (elem.``Atomic number`` ?< 6)
            select elem }
            |> Seq.toList

// MINOR BUG: should return zero elements
    query { for elem in elements do 
            where (elem.``Atomic number`` ?= 6)
            where (elem.``Atomic number`` ?= 7)
            select elem }
            |> Seq.length

    query { for i in isotopes do 
            where (i.``Mass number`` ?> 100)
            select i }
            |> Seq.toList

    query { for i in isotopes do 
            where (i.``Mass number`` ?>= 100)
            select i }
            |> Seq.toList

    query { for i in isotopes do 
            where (i.``Mass number`` ?< 100)
            select i }
        |> Seq.toList

    query { for i in isotopes do 
            where (i.``Mass number`` ?<= 100)
            select i }
        |> Seq.toList

    query { for i in isotopes do 
            where (i.``Mass number`` ?> 150)
            select i }
        |> Seq.toList

    query { for i in isotopes do 
            where (i.``Mass number`` ?> 200)
            select i }
        |> Seq.toList

    query { for i in isotopes do 
            where (i.``Mass number`` ?> 220)
            select i }
        |> Seq.toList


// get formulae
    query { for elem in biology.``Amino Acids`` do 
            select elem.Formula }
        |> Seq.toList

    query { for elem in biology.``Amino Acids`` do 
            where (elem.Formula = "C5H9NO2") 
            select elem }
        |> Seq.toList

    data.``Arts and Entertainment``.Books.Books.Individuals.``Brave New World``.Editions
    data.Sports.Baseball.``Baseball Teams``.Individuals.``New York Mets``.``Current Coaches``  
    data.Sports.Soccer.``Football players``.Individuals.``David Beckham``.``Country of nationality``
