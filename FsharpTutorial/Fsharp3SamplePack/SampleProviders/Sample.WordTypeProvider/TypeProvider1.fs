// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

namespace Samples.FSharp.TypeProviderTemplate1

open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Samples.FSharp.ProvidedTypes

open WordLibrary

[<AutoOpen>]
module HelperModule = 
    let fields = System.Collections.Generic.Dictionary<string,bool>()

    let setField (doc:WordDoc) (name:string) (value:string) = 
        match doc.GetField(name) with
        | Some(field) ->             
            fields.[name] <- true
            doc.Replace field value
        | None ->             
            ()

    let copy source target = 
        System.IO.File.Copy(source, target, true)

    let combinePath a b = System.IO.Path.Combine(a,b) 
    
    let checkComplete() = 
        let unassignedField = 
            fields
            |> Seq.exists (fun keyPair -> keyPair.Value = false)
        if unassignedField then failwith "Please assign all fields" else ()

[<TypeProvider>]
type public TypeProvider1(cfg:TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces()
    
    // Get the assembly and namespace used to house the provided types
    let thisAssembly = Assembly.GetExecutingAssembly()
    let rootNamespace = "Samples.ShareInfo.TPTest"
    let baseTy = typeof<obj>

    let newT = ProvidedTypeDefinition(thisAssembly, rootNamespace, "TPTestType", Some baseTy)
    
    do
        let filename = ProvidedStaticParameter("filename", typeof<string>)
        newT.DefineStaticParameters(
            [filename], 
            fun tyName [| :? string as filename |] -> 
                let fn = System.IO.Path.Combine(cfg.ResolutionFolder, filename)
                let doc = WordDoc(fn)
                doc.FieldNames
                |> Seq.iter (fun name -> fields.[name] <- false)
                let properties = 
                    doc.FieldNames
                    |> Seq.map (fun name -> 
                                    ProvidedProperty(
                                        name, 
                                        typeof<string>,
                                        GetterCode = (fun [me] -> <@@ ((%%me:obj):?>WordDoc).GetFieldText(name) @@>),
                                        SetterCode = (fun [me; v] -> <@@ setField ((%%me:obj):?>WordDoc) name (%%v:string) @@>)
                                        ))
                
                let ty = ProvidedTypeDefinition(thisAssembly, rootNamespace, tyName, Some typeof<obj>)
                let ctor = ProvidedConstructor(
                            [ProvidedParameter("targetfile", typeof<string>)],
                            InvokeCode = fun [filename] -> 
                                            let targetFolder = cfg.ResolutionFolder
                                            <@@ 
                                                let target = combinePath targetFolder (%%filename:string)
                                                copy fn target
                                                new WordDoc (target, true) 
                                            @@>)
                let closeMethod = ProvidedMethod("Close", 
                                        [], 
                                        typeof<unit>, 
                                        InvokeCode = fun [me] -> <@@ ((%%me:obj):?>WordDoc).Close(); checkComplete() @@>)

                properties
                |> Seq.iter ty.AddMember

                ty.AddMember ctor
                ty.AddMember closeMethod
                
                ty
            ) 
    
    
    do this.AddNamespace(rootNamespace, [newT])

[<TypeProviderAssembly>]
do ()