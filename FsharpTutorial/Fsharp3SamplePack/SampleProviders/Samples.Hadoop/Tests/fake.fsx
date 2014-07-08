
#r @"C:\projects\openfsharp\TypeProviders\Debug\net40\Samples.Hadoop.TypeProviders.dll"

open System.Linq
open Samples.Hadoop.Hive
open Microsoft.FSharp.Linq.NullableOperators

module WithoutUnits = 
    type Data = Samples.Hadoop.HiveTypeProvider<Server="tryfsharp">

    let ctxt = Data.GetDataContext(queryTimeout=100000)
    


    hiveQuery { for x in ctxt.winequalitywhite do select x } |> Seq.toList |> List.map (fun x -> x.density)
    hiveQuery { for x in ctxt.abalone do select x } |> Seq.toList |> List.map (fun x -> x.length)
    hiveQuery { for x in ctxt.abalone do select x.length } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do select (x.length > 0.5) } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do select (x.length, x.rings) } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do where (x.length < 0.6); select (x.length, x.rings) } |> Seq.toList
    let x1 = hiveQuery { for x in ctxt.abalone do count } 
    let x2 = hiveQuery { for x in ctxt.abalone do where (x.length < 0.6); count } 

    type Iris = 
        { sepalLength : float
          sepalWidth : float
          petalLength : float
          petalWidth : float
          ``class`` : string }

    let irisDynamicCoercion = ctxt.DataContext.GetTable<Iris>("iris")
    irisDynamicCoercion |> Seq.length
    irisDynamicCoercion |> Seq.map (fun x -> x.petalLength) 

    irisDynamicCoercion |> Seq.map (fun x -> x.petalLength) |> Seq.truncate 3 |> Seq.toList |> List.map string = [ "1.4"; "1.4"; "1.3" ]

module WithUnits = 
    open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames
    open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

    type Data = Samples.Hadoop.HiveTypeProvider<Server="tryfsharp", UseUnitAnnotations=true>

    let ctxt = Data.GetDataContext (queryTimeout=100000)

    type Iris = 
        { sepalLength : float<m> 
          sepalWidth : float<m>
          petalLength : float<m>
          petalWidth : float<m>
          ``class`` : string }

    // Note, the units-of-measure must match those drawn from the Hive metadata. 
    // When using GetTable, nothing checks this statically or dynamically.
    let irisDynamicCoercion = ctxt.DataContext.GetTable<Iris>("iris")
    irisDynamicCoercion |> Seq.length
    irisDynamicCoercion |> Seq.map (fun x -> x.petalLength) 
    irisDynamicCoercion |> Seq.map (fun x -> x.``class``) 
    irisDynamicCoercion |> Seq.map (fun x -> x.petalLength) |> Seq.truncate 3 |> Seq.toList |> List.map string = [ "0.014"; "0.014"; "0.013" ]

    hiveQuery { for x in ctxt.winequalitywhite do select x } |> Seq.toList |> List.map (fun x -> x.density)


    hiveQuery { for x in ctxt.abalone do select x } |> Seq.toList |> List.map (fun x -> x.length)
    hiveQuery { for x in ctxt.abalone do select x.length } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do select (x.length, x.rings) } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do where (x.length < 0.0006<m>); select (x.length, x.rings) } |> Seq.toList
    let x1 = hiveQuery { for x in ctxt.abalone do count } 
    let x2 = hiveQuery { for x in ctxt.abalone do where (x.length < 0.0006<m>); count } 

