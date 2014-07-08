// Learn more about F# at http://fsharp.net

namespace Samples.Csv.Runtime

open System.Reflection
open System.IO
open Microsoft.FSharp.Core.CompilerServices
open System.Text.RegularExpressions

module Helpers =
    let makeOpt (f:System.Func<_,_>) s =
        if System.String.IsNullOrEmpty s then
            None
        else
            Some(f.Invoke s)

    let getData(uri:System.Uri) =
#if FX_NO_LOCAL_FILESYSTEM
#else
        if uri.IsFile then
            File.ReadAllLines(uri.LocalPath)
        else
#endif
            let req = System.Net.WebRequest.Create(uri)
            use resp = req.AsyncGetResponse() |> Async.RunSynchronously
            use reader = new StreamReader(resp.GetResponseStream())
            reader.ReadToEnd().Split([|'\n';'\r'|], System.StringSplitOptions.RemoveEmptyEntries)

/// Infrastructure not intended to be directly used by user code
type CsvFileImpl<'t>(uri, delim:char, quote:char, ignoreErrors,  f : System.Func<string[], 't>) =

    let splitReg = Regex(sprintf "^((^|(?<=%c))((%c(?<data>[^%c]*)%c)|(?<data>[^%c]*))(%c|$))*$" delim quote quote quote delim delim, RegexOptions.ExplicitCapture)
    let lines = Helpers.getData uri |> Seq.cache
    // Cache the sequence of all data lines (all lines but the first)
    let data = 
        seq { for line in lines |> Seq.skip 1 do
                if line <> "" then
                    let fields = [| for field in splitReg.Match(line).Groups.["data"].Captures -> field.Value |]
                    let result = try Some(f.Invoke fields) with _ -> None
                    match result, ignoreErrors with
                    | Some(result), _ -> yield result
                    | None, true -> ()
                    | _ -> failwith (sprintf "Couldn't parse row according to schema: %s" line) }
        |> Seq.cache
    member __.Data = data 
    member __.HeaderRow = lines |> Seq.head

// This is needed in the type provider assembly when the type provider runtime is .NET 3.5
// This is because FSharp.Core for .NET 2.0-3.5 does not define this attribute.
#if NO_FSHARP_CORE_TYPE_PROVIDER_ASSEMBLY_ATTRIBUTE
namespace Microsoft.FSharp.Core.CompilerServices

open System
open System.Reflection

[<AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)>]
type internal TypeProviderAssemblyAttribute(assemblyName : string) = 
    inherit System.Attribute()
    new () = TypeProviderAssemblyAttribute(null)
    member __.AssemblyName = assemblyName

#endif

[<TypeProviderAssembly("Samples.Csv.DesignTime")>]
do()