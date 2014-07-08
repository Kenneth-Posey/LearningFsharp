namespace Samples.FSharp.ShareInfoProvider

open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Samples.FSharp.ProvidedTypes
open System.IO
open System.Collections.Generic
open Microsoft.FSharp.Quotations

[<TypeProvider>]
type public CheckedRegexProvider(cfg:TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces()

    // Get the assembly and namespace used to house the provided types
    let thisAssembly =  Assembly.GetExecutingAssembly()
    let rootNamespace = "Samples.ShareInfo.TPTest"
    let baseTy = typeof<obj>

    let regexTy = ProvidedTypeDefinition(thisAssembly, rootNamespace, "TPTestType", Some baseTy)
    let providedAssembly = new ProvidedAssembly(System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".dll"))

    let methods = ProvidedMethod("F1", [ProvidedParameter("i", typeof<int>)], typeof<int>, InvokeCode = fun args -> Expr.Value(1))
                            
    let ctor = ProvidedConstructor(
                        parameters = [], 
                        InvokeCode = fun args -> <@@ obj() @@>)  

    do 
        regexTy.IsErased <- false
        regexTy.SuppressRelocation <- false
        providedAssembly.AddTypes([regexTy])

        regexTy.AddMember ctor
        regexTy.AddMember methods

    do System.AppDomain.CurrentDomain.add_AssemblyResolve(fun _ args ->
        let name = System.Reflection.AssemblyName(args.Name)
        let existingAssembly = 
            System.AppDomain.CurrentDomain.GetAssemblies()
            |> Seq.tryFind(fun a -> System.Reflection.AssemblyName.ReferenceMatchesDefinition(name, a.GetName()))
        match existingAssembly with
        | Some a -> a
        | None -> null
        )

    do 
        this.AddNamespace(rootNamespace, [regexTy])
        

[<TypeProviderAssembly>]
do ()