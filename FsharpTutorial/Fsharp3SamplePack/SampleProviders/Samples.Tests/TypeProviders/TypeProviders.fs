namespace TypeProviders

open System
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices

[<assembly : TypeProviderAssembly>]
do()

module Shared = 
    type private T = interface end
    let ThisAssembly =typeof<T>.Assembly

open Samples.FSharp.ProvidedTypes

type internal ProvidedTypes = 
    static member DefineProvidedType(namespaceName, className, ?baseType, ?assembly, ?isErased) = 
        let assembly = defaultArg assembly Shared.ThisAssembly
        let t = ProvidedTypeDefinition(assembly, namespaceName, className, baseType)
        isErased |> Option.iter (fun v -> t.IsErased <- v)
        t
    static member ConvertToGenerated(ty : ProvidedTypeDefinition) = 
        if ty.IsErased then failwith "Erased type is not expected"
        let asm = new ProvidedAssembly(IO.Path.GetTempFileName() + ".dll")
        asm.AddTypes([ty])
    static member CreateSimpleErasedType(ns) =
        ProvidedTypes.DefineProvidedType(ns, "Erased", baseType = typeof<obj>)
    static member CreateSimpleGeneratedType(ns) =
        let ty = ProvidedTypes.DefineProvidedType(ns, "Generated", baseType = typeof<obj>, isErased = false)
        ProvidedTypes.ConvertToGenerated ty
        ty

[<AutoOpen>]
module internal ProvidedTypesExtensions = 
    let (-?>) o f = Option.iter f o
    type ProvidedTypeDefinition with
        member this.DefineMethod(name, parameters, returnType, ?isStatic, ?invokeCode) = 
            let m = ProvidedMethod(name, parameters, returnType)
            isStatic    -?> fun v -> m.IsStaticMethod <- v
            invokeCode  -?> fun v -> m.InvokeCode <- v
            this.AddMember m
            m

type E = Quotations.Expr

module ProvidedEnums =
    let Namespace = "ProvidedEnums"

    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()

        let ty = ProvidedTypes.CreateSimpleGeneratedType(Namespace)
        do 
            ty.SetBaseType typeof<Enum>
        
        let f1 = ProvidedLiteralField("Value1", ty, 1)
        do ty.AddMember f1
        
        let f2 = ProvidedLiteralField("Value2", ty, 2)
        do ty.AddMember f2

        do this.AddNamespace(Namespace, [ty])

module TestDecimalValues =
    let Namespace = "TestDecimalValues"

    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces() 

        let ty = ProvidedTypes.CreateSimpleGeneratedType(Namespace)
        do for i in [1m; -1001m; 1001001m] do
            ty.DefineMethod((sprintf "Get %O" i), [], typeof<decimal>, isStatic = true, invokeCode = fun [] -> <@@ i @@>)
            |> ignore
        do this.AddNamespace(Namespace, [ty])

module TestConstantValues = 
    
    let Namespace = "TestConstantValues"

    [<TypeProvider>]
    type TypeProvider() as this = 
        inherit TypeProviderForNamespaces()


        let addMethods (ty : ProvidedTypeDefinition) =
            let constants = 
                [
                    box ""
                    box (int8 1)
                    box (uint8 1)
                    box 1s
                    box 1us
                    box 1
                    box 1u
                    box 1L
                    box 1UL
                    box 'c'
                    box true
                    box System.IO.FileShare.Delete
                    box null
                    box 1.0
                    box 1.0F
                ]
            do 
                let nameOfType c = 
                    if c <> null then 
                        let ty = c.GetType()
                        (ty.FullName.Replace(".", "_")), ty
                    else 
                        "Null", typeof<obj>

                for c in constants do
                    let name, retTy = nameOfType c
                    ty.DefineMethod("Get" + name, [], retTy, isStatic = true, invokeCode = fun _ -> E.Value(c, retTy)) // exact type
                    |> ignore

                    ty.DefineMethod("GetBoxed" + name, [], typeof<obj>, isStatic = true, invokeCode = fun _ -> E.Value(c, typeof<obj>)) // boxed representation
                    |> ignore

        let erasedTy = ProvidedTypes.CreateSimpleErasedType Namespace
        let generatedTy = ProvidedTypes.CreateSimpleGeneratedType Namespace

        do 
            addMethods erasedTy
            addMethods generatedTy
            this.AddNamespace(Namespace, [erasedTy; generatedTy])

module TestVirtualCallsOnValueTypes = 

    let Namespace = "TestVirtualCallsOnValueTypes"

    [<TypeProvider>]
    type TypeProvider() as this = 
        inherit TypeProviderForNamespaces()
        let ty = ProvidedTypes.DefineProvidedType(Namespace, "Root", baseType = typeof<obj>, isErased = false)
        
        do
            let m = ty.DefineMethod("DateTimeToString", [ProvidedParameter("dt", typeof<System.DateTime>)], typeof<string>, isStatic = true)
            m.InvokeCode <- fun [dt] -> <@@ (%%dt : System.DateTime).ToString() @@>
        do ProvidedTypes.ConvertToGenerated ty
        do this.AddNamespace(Namespace, [ty])

module EffectsInArgumentsOfErasedMethods = 
    let Namespace = "EffectsInArgumentsOfErasedMethods"

    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()

        let ty = ProvidedTypes.DefineProvidedType(Namespace, "Root")
        do 
            ty.DefineMethod("Ignore", [ProvidedParameter("o", typeof<obj>)], typeof<bool>, isStatic = true, invokeCode = fun[_] -> <@@ true @@>)
            |> ignore

            ty.DefineMethod("Twice", [ProvidedParameter("o", typeof<obj>)], typeof<bool>, isStatic = true, invokeCode = fun[o] -> <@@ System.Object.ReferenceEquals(%%o, %%o) @@>)
            |> ignore

        do this.AddNamespace(Namespace, [ty])

module ParamsArray = 
    let Namespace = "ParamsArray"

    [<TypeProvider>]
    type TypeProvider() as this = 
        inherit TypeProviderForNamespaces()

        let erasedTy = ProvidedTypes.CreateSimpleErasedType Namespace
        let generatedTy = ProvidedTypes.CreateSimpleGeneratedType Namespace

        let addMethodWithParams(ty : ProvidedTypeDefinition) = 
            let p = ProvidedParameter("vals", typeof<int[]>, IsParamArray = true)
            ty.DefineMethod("Get", [p], typeof<int[]>, isStatic = true, invokeCode = fun[v] -> v) |> ignore

        do 
            addMethodWithParams erasedTy
            addMethodWithParams generatedTy

        do this.AddNamespace(Namespace, [erasedTy; generatedTy])

module Tuples = 
    let Namespace = "Tuples"

    let internal addMethodThatAcceptTuple (t : ProvidedTypeDefinition) = 
        t.DefineMethod(
            "MethodThatAcceptsTuple", 
            [ProvidedParameter("x", typeof<int * int>)], 
            typeof<int>, 
            isStatic = true, 
            invokeCode = fun[i] -> <@@ let (a : int, _b : int) = %%i in a @@>
            )
        |> ignore

    let internal addMethodThatAcceptBigTuple (t : ProvidedTypeDefinition) = 
        t.DefineMethod(
            "MethodThatAcceptsBigTuple", 
            [ProvidedParameter("x", typeof<int * int * int * int * int * int * int * int * int * int>)], 
            typeof<int>, 
            isStatic = true, 
            invokeCode = fun[i] -> <@@ let (_b0 : int, _b1 : int, _b2 : int, _b3 : int, _b4 : int, _b5 : int, _b6 : int, _b7 : int, _b8 : int, a : int) = %%i in a @@>
            )
        |> ignore
    let internal addMethodThatReturnsTuple(t : ProvidedTypeDefinition) =
        t.DefineMethod(
            "MethodThatReturnsTuple", 
            [ProvidedParameter("x", typeof<int>)], 
            typeof<int * int>, 
            isStatic = true, 
            invokeCode = fun [i] -> <@@ (%%i : int) , (%%i : int)@@>
            )
        |> ignore

    let internal addMethodThatReturnsBigTuple(t : ProvidedTypeDefinition) =
        t.DefineMethod(
            "MethodThatReturnsBigTuple", 
            [ProvidedParameter("x", typeof<int>)], 
            typeof<int * int * int * int * int * int * int * int * int * int>, 
            isStatic = true, 
            invokeCode = fun [i] -> <@@ (%%i : int) , (%%i : int), (%%i : int), (%%i : int), (%%i : int), (%%i : int), (%%i : int), (%%i : int), (%%i : int), (%%i : int)@@>
            )
        |> ignore


    [<TypeProvider>]
    type TypeProvider() as this = 
        inherit TypeProviderForNamespaces()

        let erasedTy = ProvidedTypes.CreateSimpleErasedType Namespace
        let generatedTy = ProvidedTypes.CreateSimpleGeneratedType Namespace
        do 
            addMethodThatAcceptTuple erasedTy
            addMethodThatAcceptTuple generatedTy

            addMethodThatAcceptBigTuple erasedTy
            addMethodThatAcceptBigTuple generatedTy

            addMethodThatReturnsTuple erasedTy
            addMethodThatReturnsTuple generatedTy

            addMethodThatReturnsBigTuple erasedTy
            addMethodThatReturnsBigTuple generatedTy

        do this.AddNamespace(Namespace, [erasedTy; generatedTy])

module ValuesInQuotations = 
    let Namespace = "ValuesInQuotations"

    [<TypeProvider>]
    type TypeProvider() as this = 
        inherit TypeProviderForNamespaces()

        let erasedTy = ProvidedTypes.CreateSimpleErasedType Namespace
        let generatedTy = ProvidedTypes.CreateSimpleGeneratedType Namespace

        let addMethodThatUsesArray(ty : ProvidedTypeDefinition) = 
            let arr = [|"a"; "b"|]
            ty.DefineMethod("Get", [], typeof<string>, isStatic = true, invokeCode = fun _ -> <@@ String.Concat(arr) @@>)
            |> ignore

        let addMethodThatUsesList(ty : ProvidedTypeDefinition) = 
            let arr = ["a"; "b"]
            ty.DefineMethod("GetList", [], typeof<string>, isStatic = true, invokeCode = fun _ -> <@@ String.Concat(Array.ofList arr) @@>)
            |> ignore

        do
            addMethodThatUsesArray erasedTy
            addMethodThatUsesArray generatedTy

            addMethodThatUsesList erasedTy
            addMethodThatUsesList generatedTy

            this.AddNamespace(Namespace, [erasedTy; generatedTy])

module TypesWithLoops = 
    let Namespace = "TypesWithLoops"

    [<TypeProvider>]
    type TypeProvider() as this = 
        inherit TypeProviderForNamespaces()

        let erasedTy = ProvidedTypes.CreateSimpleErasedType Namespace
        let generatedTy = ProvidedTypes.CreateSimpleGeneratedType Namespace
        
        let addMethodWithWhile(ty : ProvidedTypeDefinition) =
            let p = ProvidedParameter("n", typeof<int>)
            let m = ty.DefineMethod("MethodWithWhile",[p],typeof<string>, isStatic = true)
            m.InvokeCode <- fun[i] -> 
                <@@
                    let mutable r = ""
                    while(r.Length < (%%i : int)) do
                        r <- r + "!"
                    r
                @@>

        let addMethodWithFor(ty : ProvidedTypeDefinition) =
            let first = ProvidedParameter("first", typeof<int>)
            let last = ProvidedParameter("last", typeof<int>)
            let m = ty.DefineMethod("MethodWithFor",[first;last],typeof<string>, isStatic = true)
            m.InvokeCode <- fun[first; last] -> 
                <@@
                    let mutable r = ""
                    for i in (%%first : int)..(%%last : int) do
                        r <- r + (string i)
                    r
                @@>
        do
            addMethodWithWhile erasedTy
            addMethodWithWhile generatedTy
            addMethodWithFor erasedTy
            addMethodWithFor generatedTy

        do this.AddNamespace(Namespace, [erasedTy; generatedTy])

module NewArrayExpr = 
    let Namespace = "NewArrayExpr"

    [<TypeProvider>]
    type TypeProvider() as this = 
        inherit TypeProviderForNamespaces()

        let erasedTy = ProvidedTypes.CreateSimpleErasedType Namespace
        let generatedTy = ProvidedTypes.CreateSimpleGeneratedType Namespace

        let addMethodThatCreatesArray(ty : ProvidedTypeDefinition) =
            ty.DefineMethod("Get", [], typeof<int[]>, isStatic = true, invokeCode = fun[] -> <@@ [|1;2;3;4;5|] @@>)
            |> ignore

        do
            addMethodThatCreatesArray erasedTy
            addMethodThatCreatesArray generatedTy

        do this.AddNamespace(Namespace, [erasedTy; generatedTy])

module Unboxing = 
    let Namespace = "Unboxing"

    [<TypeProvider>]
    type TypeProvider() as this = 
        inherit TypeProviderForNamespaces()

        let erasedTy = ProvidedTypes.CreateSimpleErasedType Namespace
        let generatedTy = ProvidedTypes.CreateSimpleGeneratedType Namespace

        let addMethodThatUsesUnboxing(ty : ProvidedTypeDefinition) =
            ty.DefineMethod("Run", [ProvidedParameter("o", typeof<obj>)], typeof<int>, isStatic = true, invokeCode = fun [p] -> E.Coerce(p, typeof<int>))
            |> ignore
        do 
            addMethodThatUsesUnboxing erasedTy
            addMethodThatUsesUnboxing generatedTy
        do this.AddNamespace(Namespace, [erasedTy; generatedTy])

module GeneratedTypesWithStaticParams =
    let Namespace = "GeneratedTypesWithStaticParams"

    let csCodeProvider = new Microsoft.CSharp.CSharpCodeProvider()

    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()

        let top = ProvidedTypes.CreateSimpleGeneratedType(Namespace)
        do 
            let p = ProvidedStaticParameter("name", typeof<string>)
            top.DefineStaticParameters
                (
                    parameters = [p],
                    instantiationFunction = 
                        (
                            fun typeName [|:? string as name|] ->
                                let ty = ProvidedTypes.DefineProvidedType(Namespace, typeName, typeof<obj>, isErased = false)
                                ProvidedTypes.ConvertToGenerated ty
                                if not (csCodeProvider.IsValidIdentifier name) then failwithf "'%s' is not an identifier" name
                                ty.DefineMethod("Method_" + name, [], typeof<string>, isStatic = true, invokeCode = fun [] -> <@@ name @@>)
                                |> ignore
                                ty
                        )
                )
        do this.AddNamespace(Namespace, [top])

module TypeValues =
    let Namespace = "TypeValues"
    
    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()

        let top = ProvidedTypes.CreateSimpleGeneratedType(Namespace)
        do
            let t = typeof<int>
            top.DefineMethod("TypeOfInt", [], typeof<Type>, isStatic = true, invokeCode = fun _ -> <@@ t @@>)
            |> ignore
        do this.AddNamespace(Namespace, [top])

module SimpleLambdas =
    let Namespace = "SimpleLambdas"
    
    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()

        let top = ProvidedTypes.CreateSimpleGeneratedType(Namespace)
        do
            top.DefineMethod("Foo", [ProvidedParameter("x", typeof<int>)], typeof<string>, isStatic = true, invokeCode = fun [p] -> <@@ sprintf "%d %b %s" (%%p : int) true "foo" @@>)
            |> ignore
            let m = top.DefineMethod("Bar", [ProvidedParameter("y", typeof<string>)], typeof<string>, isStatic = true)
            m.InvokeCode <-
                fun [p] ->
                    <@@
                        let x = sprintf "%A %b %s" (%%p : string) false
                        let y = "foo"
                        let z = "bar"
                        x (y + z)
                    @@>
        do this.AddNamespace(Namespace, [top])

module InlinedOperatorsInFSCore = 
    let Namespace = "InlinedOperatorsInFSCore"
    let private addMethod<'a> prefix f (ty : ProvidedTypeDefinition) = 
        ty.DefineMethod(prefix  + typeof<'a>.Name, [ProvidedParameter("a", typeof<'a>); ProvidedParameter("b", typeof<'a>)], typeof<'a>, isStatic = true, invokeCode = fun [a; b] -> f (E.Cast<'a>(a)) (E.Cast<'a>(b)))
        |> ignore
        ty
    [<TypeProvider>]
    type TypeProvider() as this = 
        inherit TypeProviderForNamespaces()
        let top = 
            ProvidedTypes.CreateSimpleGeneratedType(Namespace)
            |> addMethod<int32>      "Sub" (fun a b -> <@@ %a - %b @@>)
            |> addMethod<float>      "Sub" (fun a b -> <@@ %a - %b @@>)
            |> addMethod<float32>    "Sub" (fun a b -> <@@ %a - %b @@>)
            |> addMethod<int64>      "Sub" (fun a b -> <@@ %a - %b @@>)
            |> addMethod<uint64>     "Sub" (fun a b -> <@@ %a - %b @@>)
            |> addMethod<uint32>     "Sub" (fun a b -> <@@ %a - %b @@>)
            |> addMethod<nativeint>  "Sub" (fun a b -> <@@ %a - %b @@>)
            |> addMethod<unativeint> "Sub" (fun a b -> <@@ %a - %b @@>)
            |> addMethod<int16>      "Sub" (fun a b -> <@@ %a - %b @@>)
            |> addMethod<uint16>     "Sub" (fun a b -> <@@ %a - %b @@>)
            |> addMethod<sbyte>      "Sub" (fun a b -> <@@ %a - %b @@>)
            |> addMethod<byte>       "Sub" (fun a b -> <@@ %a - %b @@>)
            |> addMethod<decimal>    "Sub" (fun a b -> <@@ %a - %b @@>)

            |> addMethod<int32>      "Div" (fun a b -> <@@ %a / %b @@>)
            |> addMethod<float>      "Div" (fun a b -> <@@ %a / %b @@>)
            |> addMethod<float32>    "Div" (fun a b -> <@@ %a / %b @@>)
            |> addMethod<int64>      "Div" (fun a b -> <@@ %a / %b @@>)
            |> addMethod<uint64>     "Div" (fun a b -> <@@ %a / %b @@>)
            |> addMethod<uint32>     "Div" (fun a b -> <@@ %a / %b @@>)
            |> addMethod<nativeint>  "Div" (fun a b -> <@@ %a / %b @@>)
            |> addMethod<unativeint> "Div" (fun a b -> <@@ %a / %b @@>)
            |> addMethod<int16>      "Div" (fun a b -> <@@ %a / %b @@>)
            |> addMethod<uint16>     "Div" (fun a b -> <@@ %a / %b @@>)
            |> addMethod<sbyte>      "Div" (fun a b -> <@@ %a / %b @@>)
            |> addMethod<byte>       "Div" (fun a b -> <@@ %a / %b @@>)
            |> addMethod<decimal>    "Div" (fun a b -> <@@ %a / %b @@>)
       
        do this.AddNamespace(Namespace, [top])


module Generics1 =
    let Namespace = "Generics1"

    type Maker =
        static member Make<'T>(arg : 'T) = 
            let l = ResizeArray()
            l.Add (arg)
            l

    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()
        let top = ProvidedTypes.CreateSimpleGeneratedType(Namespace)
        let elementTy = new ProvidedTypeDefinition("Element", Some typeof<obj>, IsErased = false)
        let elementCtor = ProvidedConstructor([], IsImplicitCtor = true)
        do 
            elementTy.AddMember(elementCtor)
            top.AddMember elementTy

        let producerTy = new ProvidedTypeDefinition("Producer", Some typeof<obj>, IsErased = false)
        do
            let resultTy = ProvidedTypeBuilder.MakeGenericType(typedefof<ResizeArray<_>>, [elementTy])
            let meth = producerTy.DefineMethod("Get", [ProvidedParameter("x", elementTy)], resultTy, isStatic = true)
            meth.InvokeCode <- fun [arg] ->
                let mi = typeof<Maker>.GetMethod "Make"
                let mi = ProvidedTypeBuilder.MakeGenericMethod(mi, [elementTy])
                E.Call(mi, [arg])

            let meth2 = producerTy.DefineMethod("MakeAndGet", [], resultTy, isStatic = true)
            meth2.InvokeCode <- fun _ ->
                E.Call(meth, [E.NewObject(elementCtor, [])])

            top.AddMember(producerTy)
        do this.AddNamespace(Namespace, [top])

module Generics2 =
    let Namespace = "Generics2"

    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()
        let top = ProvidedTypes.CreateSimpleGeneratedType(Namespace)
        do
            let element = ProvidedTypeDefinition("Element", Some typeof<obj>)
            element.AddMember(ProvidedConstructor([], IsImplicitCtor = true))
            top.AddMember element
        do this.AddNamespace(Namespace, [top])

module DateTimeValuesTest =
    let Namespace = "DateTimeValuesTest"

    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()

        let v = DateTime(2012, 10, 5, 5, 4, 3, DateTimeKind.Utc)
        let str = v.ToString(System.Globalization.CultureInfo.InvariantCulture)
        let ty = ProvidedTypes.CreateSimpleGeneratedType(Namespace)

        do ty.DefineMethod("GetDate", [], typeof<DateTime>, isStatic = true, invokeCode = fun _ -> <@@ v @@>)
            |> ignore
        do ty.DefineMethod("GetDateAsString", [], typeof<string>, isStatic = true, invokeCode = fun _ -> <@@ str @@>)
            |> ignore

        do this.AddNamespace(Namespace, [ty])

module DateTimeOffsetValuesTest =
    let Namespace = "DateTimeOffsetValuesTest"

    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()

        let dt = DateTimeOffset.Now
        let ticks = dt.Ticks
        let timespanTicks = dt.Offset.Ticks
        let ty = ProvidedTypes.CreateSimpleGeneratedType(Namespace)

        do ty.DefineMethod("GetDate", [], typeof<DateTimeOffset>, isStatic = true, invokeCode = fun _ -> <@@ dt @@>)
            |> ignore
        do ty.DefineMethod("GetTicks", [], typeof<int64 * int64>, isStatic = true, invokeCode = fun _ -> <@@ ticks, timespanTicks @@>)
            |> ignore

        do this.AddNamespace(Namespace, [ty])


module Constructors1 = 
    let Namespace = "Constructors1"

    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()
        let resizeArrayTy = typeof<ResizeArray<int>>
        let ty = ProvidedTypes.DefineProvidedType(Namespace, "Generated", baseType = resizeArrayTy, isErased = false)
        do
            ProvidedTypes.ConvertToGenerated ty
            let ctor = ProvidedConstructor([ProvidedParameter("x", typeof<string>)])
            ctor.BaseConstructorCall <-
                fun [this; _] ->
                    let ci = resizeArrayTy.GetConstructor([| typeof<int>|])
                    ci, [ this; <@@ 100 @@>]
            ctor.InvokeCode <-
                fun [this; arg] ->
                    let addMethod = ty.GetMethod "Add"
                    let add = E.Call(E.Coerce(this, resizeArrayTy), addMethod, [ <@@ int (%%arg : string) @@>])
                    <@@
                        (%%add : unit)
                        (%%E.Coerce(this, resizeArrayTy) : ResizeArray<int>).Add(int (%%arg : string) + 100)
                    @@>
            ty.AddMember ctor
            this.AddNamespace(Namespace, [ty])

module StaticConstructorAndStaticFields =
    let Namespace = "StaticConstructorAndStaticFields"

    [<TypeProvider>]
    type TypeProvider() as this = 
        inherit TypeProviderForNamespaces()
        let top = ProvidedTypes.DefineProvidedType(Namespace, "Generated", baseType = typeof<System.Windows.Controls.Button>, isErased = false)
        do ProvidedTypes.ConvertToGenerated top
        let propertyName = "StateProperty"
        let f = ProvidedField(propertyName, typeof<System.Windows.DependencyProperty>)
        do 
            f.SetFieldAttributes (FieldAttributes.Public ||| FieldAttributes.InitOnly ||| FieldAttributes.Static)
            top.AddMember f
        do
            let typeInit = ProvidedConstructor([], IsTypeInitializer = true)
            typeInit.InvokeCode <- 
                fun [] -> 
                    E.FieldSet
                        (
                            f, 
                            <@@ 
                                System.Windows.DependencyProperty.Register(propertyName, typeof<bool>, top, Windows.PropertyMetadata(false)) 
                            @@>
                        )
            top.AddMember typeInit
        do this.AddNamespace(Namespace, [top])
module GeneratedTypesThatInheritFromNonObjTypeAndImplementInterface =
    let Namespace = "GeneratedTypesThatInheritFromNonObjTypeAndImplementInterface"
    type PCEH = System.ComponentModel.PropertyChangedEventHandler
    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()

        let top = ProvidedTypes.CreateSimpleGeneratedType(Namespace)
        do 
            top.DefineStaticParameters
                (
                    parameters = [ProvidedStaticParameter("PropertyName", typeof<string>)],
                    instantiationFunction = 
                        (
                            fun typeName [|:? string as propName|] -> 
                                let ty = ProvidedTypes.DefineProvidedType(Namespace, typeName, typeof<System.MarshalByRefObject>, isErased = false)
                                ProvidedTypes.ConvertToGenerated ty

                                let fld = ProvidedField("f", typeof<string>)
                                ty.AddMember fld

                                let handler = ProvidedField("handler", typeof<PCEH>)
                                ty.AddMember handler

                                do
                                    let ctor = ProvidedConstructor([], IsImplicitCtor = true)
                                    ty.AddMember ctor
                                
                                do 
                                    let decl = typeof<MarshalByRefObject>.GetMethod "InitializeLifetimeService"

                                    let meth = ProvidedMethod("InitializeLifetimeService", [], typeof<obj>)
                                    meth.SetMethodAttrs(meth.Attributes ||| MethodAttributes.Virtual)
                                    meth.InvokeCode <- fun [this] -> <@@ null @@>
                                    ty.AddMember meth
                                    ty.DefineMethodOverride(meth, decl)
                                
                                do
                                    ty.AddInterfaceImplementation typeof<System.ComponentModel.INotifyPropertyChanged>
                                    let evt = ProvidedEvent("PropertyChanged", typeof<PCEH>)
                                    ty.AddMember evt
                                    evt.AdderCode <- 
                                        fun [this; value] ->
                                            E.FieldSet(this, handler, <@@ System.Delegate.Combine((%%E.FieldGet(this, handler) : PCEH), (%%value : PCEH)) :?> PCEH @@>)
                                    evt.RemoverCode <- 
                                        fun [this; value] -> 
                                            E.FieldSet(this, handler, <@@ System.Delegate.Remove((%%E.FieldGet(this, handler) : PCEH), (%%value : PCEH)) :?> PCEH @@>)
                                    let addDecl = typeof<System.ComponentModel.INotifyPropertyChanged>.GetMethod "add_PropertyChanged"
                                    let removeDecl = typeof<System.ComponentModel.INotifyPropertyChanged>.GetMethod "remove_PropertyChanged"
                                    
                                    let addMethod = evt.GetAddMethod(true) :?> ProvidedMethod
                                    addMethod.SetMethodAttrs (addMethod.Attributes ||| MethodAttributes.Virtual)

                                    let removeMethod = evt.GetRemoveMethod(true) :?> ProvidedMethod
                                    removeMethod.SetMethodAttrs (removeMethod.Attributes ||| MethodAttributes.Virtual)

                                    ty.DefineMethodOverride(addMethod, addDecl)
                                    ty.DefineMethodOverride(removeMethod, removeDecl)
                                do
                                    let prop = ProvidedProperty(propName, typeof<string>)
                                    ty.AddMember prop
                                    prop.GetterCode <- fun [this] -> E.FieldGet(this, fld)
                                    prop.SetterCode <- 
                                        fun [this; value] -> 
                                            <@@
                                                let oldValue = (%%E.FieldGet(this, fld) : string)
                                                (%%E.FieldSet(this, fld, value) : unit)
                                                if oldValue <> (%%value : string) then
                                                    let h = (%%E.FieldGet(this, handler) : PCEH)
                                                    if h <> null then
                                                        h.Invoke(null, ComponentModel.PropertyChangedEventArgs(propName))
                                            @@>
                                ty
                        )
                )
        do this.AddNamespace(Namespace, [top])

module GeneratedTypesWithInstanceMethodsAndStaticParams =
    let Namespace = "GeneratedTypesWithInstanceMethodsAndStaticParams"

    let csCodeProvider = new Microsoft.CSharp.CSharpCodeProvider()

    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()

        let top = ProvidedTypes.CreateSimpleGeneratedType(Namespace)
        do 
            let p = ProvidedStaticParameter("name", typeof<string>)
            top.DefineStaticParameters
                (
                    parameters = [p],
                    instantiationFunction = 
                        (
                            fun typeName [|:? string as name|] ->
                                let ty = ProvidedTypes.DefineProvidedType(Namespace, typeName, typeof<obj>, isErased = false)
                                ProvidedTypes.ConvertToGenerated ty
                                if not (csCodeProvider.IsValidIdentifier name) then failwithf "'%s' is not an identifier" name
                                do
                                    let ctor = ProvidedConstructor([ProvidedParameter("v", typeof<int>)], IsImplicitCtor = true)
                                    ty.AddMember ctor
                                do
                                    let prop = ProvidedProperty(name, typeof<int>)
                                    prop.GetterCode <- fun [this] -> E.GlobalVar<int>("v") :> _
                                    ty.AddMember prop
                                do
                                    ty.DefineMethod("Method_" + name, [], typeof<string>, isStatic = false, invokeCode = fun [this] -> <@@ System.String.Format("StaticParam:{0}, Field:{1}", name, (%E.GlobalVar<int>("v"))) @@>)
                                    |> ignore
                                ty
                        )
                )
        do this.AddNamespace(Namespace, [top])

module RefParams = 
    let Namespace = "RefParams"

    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()

        let erased = ProvidedTypes.CreateSimpleErasedType Namespace
        do
            let p = ProvidedParameter("x", typeof<(int * int) ref>)
            erased.DefineMethod
                (
                    "Method1", 
                    [p], 
                    typeof<bool>, 
                    isStatic = true, 
                    invokeCode = fun [dt] -> <@@ (%%dt := (1, 2)) ; true @@>
                )
            |> ignore
        do this.AddNamespace(Namespace, [erased])

module AsyncsInGeneratedTypes = 
    let Namespace = "AsyncsInGeneratedTypes"

    [<TypeProvider>]
    type TypeProvider() as this = 
        inherit TypeProviderForNamespaces()
        let ty = 
            let ty = ProvidedTypes.CreateSimpleGeneratedType(Namespace)
            let m = ty.DefineMethod("Run", [], typeof<int>, isStatic = true)
            m.InvokeCode <- fun [] ->
                <@@
                    async {
                        return 100
                    } |> Async.RunSynchronously
                @@>
            ty

        do this.AddNamespace(Namespace, [ty])

module ErasedTypesOutParameter = 
    let Namespace = "ErasedTypesOutParameter"
    
    type RuntimeIntrinsics private() =
        static let setMethod = typeof<RuntimeIntrinsics>.GetMethod("Set") 
        static member Set<'t>(r : byref<'t>, v) = r <- v
        static member SetMethod = setMethod

    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()

        let ty = ProvidedTypes.CreateSimpleErasedType(Namespace)
        do
            let p = ProvidedParameter("value", typeof<string>.MakeByRefType(), isOut = true)
            let m = ty.DefineMethod("Run", [p], typeof<bool>, isStatic = true)
            m.InvokeCode <- fun [p] ->
                
                <@@
                    %%(E.Call(RuntimeIntrinsics.SetMethod.MakeGenericMethod(p.Type.GetElementType()), [p; E.Value("some text")]))
                    true
                @@>
        do this.AddNamespace(Namespace, [ty])

module BaseContructorCall = 
    let Namespace = "BaseContructorCall"

    type BaseClass(handle: System.IntPtr) = class end

    [<TypeProvider>]
    type TypeProvider() as this =
        inherit TypeProviderForNamespaces()

        let ty = ProvidedTypes.DefineProvidedType(Namespace, "SubclassController", baseType = typeof<BaseClass>, isErased = false)
        do ProvidedTypes.ConvertToGenerated ty
        do
            let baseCtor = typeof<BaseClass>.GetConstructors().[0]
            let ctor = ProvidedConstructor([ProvidedParameter("handle", typeof<System.IntPtr>)])
            ctor.BaseConstructorCall <- fun args -> baseCtor, args
            ctor.InvokeCode <- fun _ -> <@@ () @@>
            ty.AddMember ctor
        do this.AddNamespace(Namespace, [ty])

// TODO: enable this test after fixing issue in F# compiler: default parameter values are not copied for generated types
//module DefaultArgumentValues = 
//    let Namespace = "DefaultArgumentValues"
//
//    [<TypeProvider>]
//    type TypeProvider() as this =
//        inherit TypeProviderForNamespaces()
//        let ty = ProvidedTypes.CreateSimpleGeneratedType(Namespace)
//        do
//            let parameters = [ ProvidedParameter("i", typeof<int>, optionalValue = 42); ProvidedParameter("i", typeof<Nullable<int>>, optionalValue = 420)]
//            ty.DefineMethod("Get", [ ProvidedParameter("i", typeof<int>, optionalValue = 42); ProvidedParameter("i", typeof<Nullable<int>>, optionalValue = 420)], typeof<unit>, isStatic = true, invokeCode = fun _ -> <@@ () @@>)
//            |> ignore
//        do this.AddNamespace (Namespace, [ty])
    

module ArraysOfValueTypes = 
    let Namespace = "ArraysOfValueTypes"

    let internal addMethodThatCreatesArray1D<'t>(name, ty : ProvidedTypeDefinition) =
        let p = ProvidedParameter("dt", typeof<'t[]>)
        ty.DefineMethod(name, [p], typeof<string>, isStatic = true, invokeCode = fun[dt] -> <@@ (%%dt : 't[]).[0].ToString() @@>)
        |> ignore
    let internal addMethodThatCreatesArray2D<'t>(name, ty : ProvidedTypeDefinition) =
        let p = ProvidedParameter("dt", typeof<'t[,]>)
        ty.DefineMethod(name, [p], typeof<string>, isStatic = true, invokeCode = fun[dt] -> <@@ (%%dt : 't[,]).[0,0].ToString() @@>)
        |> ignore
    let internal addMethodThatCreatesArray3D<'t>(name, ty : ProvidedTypeDefinition) =
        let p = ProvidedParameter("dt", typeof<'t[,,]>)
        ty.DefineMethod(name, [p], typeof<string>, isStatic = true, invokeCode = fun[dt] -> <@@ (%%dt : 't[,,]).[0, 0, 0].ToString() @@>)
        |> ignore
    let internal addMethodThatCreatesArray4D<'t>(name, ty : ProvidedTypeDefinition) =
        let p = ProvidedParameter("dt", typeof<'t[,,,]>)
        ty.DefineMethod(name, [p], typeof<string>, isStatic = true, invokeCode = fun[dt] -> <@@ (%%dt : 't[,,,]).[0, 0, 0, 0].ToString() @@>)
        |> ignore

    [<TypeProvider>]
    type TypeProvider() as this = 
        inherit TypeProviderForNamespaces()

        let erasedTy = ProvidedTypes.CreateSimpleErasedType Namespace
        let generatedTy = ProvidedTypes.CreateSimpleGeneratedType Namespace

        do
            // 1D
            addMethodThatCreatesArray1D<System.DateTime> ("DateTimeGet1", erasedTy)
            addMethodThatCreatesArray1D<System.DateTime> ("DateTimeGet1", generatedTy)
            addMethodThatCreatesArray1D<string> ("StringGet1", erasedTy)
            addMethodThatCreatesArray1D<string> ("StringGet1", generatedTy)
            // 2D
            addMethodThatCreatesArray2D<System.DateTime> ("DateTimeGet2", erasedTy)
            addMethodThatCreatesArray2D<System.DateTime> ("DateTimeGet2", generatedTy)
            addMethodThatCreatesArray2D<string> ("StringGet2", erasedTy)
            addMethodThatCreatesArray2D<string> ("StringGet2", generatedTy)
            // 3D
            addMethodThatCreatesArray3D<System.DateTime> ("DateTimeGet3", erasedTy)
            addMethodThatCreatesArray3D<System.DateTime> ("DateTimeGet3", generatedTy)
            addMethodThatCreatesArray3D<string> ("StringGet3", erasedTy)
            addMethodThatCreatesArray3D<string> ("StringGet3", generatedTy)
            // 4D
            addMethodThatCreatesArray4D<System.DateTime> ("DateTimeGet4", erasedTy)
            addMethodThatCreatesArray4D<System.DateTime> ("DateTimeGet4", generatedTy)
            addMethodThatCreatesArray4D<string> ("StringGet4", erasedTy)
            addMethodThatCreatesArray4D<string> ("StringGet4", generatedTy)

        do this.AddNamespace(Namespace, [erasedTy; generatedTy])