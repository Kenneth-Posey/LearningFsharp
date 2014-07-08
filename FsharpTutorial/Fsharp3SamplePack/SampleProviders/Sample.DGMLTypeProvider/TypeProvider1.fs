// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

namespace Samples.FSharp.TypeProviderTemplate1

open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Samples.FSharp.ProvidedTypes

open StateMahcineTypeProvider

[<TypeProvider>]
type public TypeProvider1(cfg : Microsoft.FSharp.Core.CompilerServices.TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces()
    
    // Get the assembly and namespace used to house the provided types
    let thisAssembly = Assembly.GetExecutingAssembly()
    let rootNamespace = "Samples.ShareInfo.TPTest"
    let baseTy = typeof<StateMachine>    

    let newT = ProvidedTypeDefinition(thisAssembly, rootNamespace, "TPTestType", Some baseTy)
    

    let staticParams = [ProvidedStaticParameter("dgml file name", typeof<string>); ProvidedStaticParameter("init state", typeof<string>)]
    do newT.DefineStaticParameters(
        parameters=staticParams, 
        instantiationFunction=(fun typeName parameterValues ->
                let ty = ProvidedTypeDefinition(
                            thisAssembly, 
                            rootNamespace, 
                            typeName, 
                            baseType = Some baseTy)

                let dgml, initState = System.IO.Path.Combine(cfg.ResolutionFolder, parameterValues.[0] :?> string), parameterValues.[1] :?> string

                let stateMachine = StateMachine()
                stateMachine.Init(dgml, initState)

                let stateProperties = 
                    stateMachine.Nodes
                    |> Seq.map (fun n ->
                                let name = n.Name
                                let prop1 = ProvidedProperty(propertyName = n.Name, 
                                             propertyType = typeof<string>, 
                                             IsStatic=false,
                                             GetterCode= (fun args -> <@@ name @@>))
                                prop1
                                )

                stateProperties
                |> Seq.iter ty.AddMember

                let asserts = 
                    stateMachine.Nodes
                    |> Seq.map (fun n ->
                                    let name = n.Name
                                    let assertFunction = ProvidedMethod(
                                            methodName = sprintf "Assert_%s" name,
                                            parameters = [],
                                            returnType = typeof<unit>,
                                            IsStaticMethod = false,
                                            InvokeCode = fun args -> 
                                                <@@ (%%args.[0] :> StateMachine).Assert(name)  @@>
                                            )
                                    assertFunction)
                asserts 
                |> Seq.iter ty.AddMember

                let transits = 
                    stateMachine.Nodes
                    |> Seq.map (fun node -> 
                                    let name = node.Name
                                    let m = ProvidedMethod(
                                                methodName = sprintf "TransitTo_%s" name,
                                                parameters = [],
                                                returnType = typeof<unit>,
                                                IsStaticMethod = false,
                                                InvokeCode = fun args -> 
                                                    <@@ (%%args.[0] :> StateMachine).TransitTo(name) @@>
                                                )
                                    m)
                transits
                |> Seq.iter ty.AddMember

                let setFunction = ProvidedMethod(
                                              methodName = "SetFunction",
                                              parameters = [ProvidedParameter("name", typeof<string>); ProvidedParameter("state class", typeof<IState>)],
                                              returnType = typeof<unit>,
                                              IsStaticMethod = false,
                                              InvokeCode = fun args -> 
                                                  <@@ (%%args.[0] :> StateMachine).SetFunction(%%args.[1] :> string, %%args.[2] :> IState) @@>
                                              )
                ty.AddMember(setFunction)

                                    
                let ctor = ProvidedConstructor(
                                        parameters = [], 
                                        InvokeCode = fun args -> <@@ StateMachine(dgml, initState) @@>) 
                ty.AddMember(ctor)
    
                ty.AddXmlDoc "xml comment"
                ty))    
    
    do this.AddNamespace(rootNamespace, [newT])

[<TypeProviderAssembly>]
do ()