namespace TypeProviderConsumers

module Tests = 
    let private tests = ResizeArray()
    let AddTest caption test = tests.Add(caption, test)
    let Run() = 
        let ok = ref true
        for (caption, f) in tests do
            try f()
            with e -> 
                printfn "Test '%s' failed: %s" caption e.Message
                ok := false
        !ok

    let Assert v fmt = 
        if v then Printf.ksprintf ignore fmt
        else Printf.ksprintf (raise << Failure) fmt

module BaseConstructorCall = 
    type T = BaseContructorCall.SubclassController

//module DefaultArgumentValues = 
//    type T = DefaultArgumentValues.Generated

module ProvidedEnums = 
    type T = ProvidedEnums.Generated
    let v = T.Value1

    type Check = 
        static member Foo<'T>(a: 'T, b: 'T) = ()
        static member Foo<'T>(a: seq<'T>, b: seq<'T>) = ()

    Tests.AddTest "ProvidedEnums" <| fun() ->
        Check.Foo<T>(enum 1, T.Value1)
        Check.Foo(enum<T> 1, T.Value1)

module TypeValues = 
    type T = TypeValues.Generated
    Tests.AddTest "TypeValues" <| fun() ->
        let ty = T.TypeOfInt()
        Tests.Assert (ty = typeof<int>) "TypeOf<int> expected but got %A" ty

module InlinedOperatorsInFSCore =
    type T = InlinedOperatorsInFSCore.Generated
    Tests.AddTest "InlinedOperatorsInFSCore" <| fun () ->
        Tests.Assert(5 / 2 = T.DivInt32(5, 2)) "1"
        Tests.Assert(5uy / 2uy = T.DivByte(5uy, 2uy)) "2"
        Tests.Assert(5M / 2M = T.DivDecimal(5M, 2M)) "3"
        Tests.Assert(6.0 / 2.0 = T.DivDouble(6.0, 2.0)) "4"
        Tests.Assert(6s / 2s = T.DivInt16(6s, 2s)) "5"
        Tests.Assert(6L / 2L = T.DivInt64(6L, 2L)) "6"
        Tests.Assert(6y / 2y = T.DivSByte(6y, 2y)) "7"
        Tests.Assert(5 - 2 = T.SubInt32(5, 2)) "8"
        Tests.Assert(5uy - 2uy = T.SubByte(5uy, 2uy)) "9"
        Tests.Assert(5M - 2M = T.SubDecimal(5M, 2M)) "10"
        Tests.Assert(6.0 - 2.0 = T.SubDouble(6.0, 2.0)) "11"
        Tests.Assert(6s - 2s = T.SubInt16(6s, 2s)) "12"
        Tests.Assert(6L - 2L = T.SubInt64(6L, 2L)) "13"
        Tests.Assert(6y - 2y = T.SubSByte(6y, 2y)) "4"

module Constructors1 =
    type T = Constructors1.Generated
    Tests.AddTest "Constructors1" <| fun () ->
        let t = T("500")
        Tests.Assert(t.Capacity = 100) "Expected capacity 100"
        Tests.Assert(t.Count = 2) "Expected 2 element in the list, got %d" t.Count
        Tests.Assert(t.[0] = 500) "Expected 500 as the first value, got %d" t.[0] 
        Tests.Assert(t.[1] = 600) "Expected 600 as the second value, got %d" t.[1] 

module SimpleLambdas = 
    type T = SimpleLambdas.Generated
    Tests.AddTest "SimpleLambdas" <| fun() ->
        do
            let actual = T.Foo(5)
            let expected = "5 true foo"
            Tests.Assert (actual = expected) "result should be '%s' but was %s" expected actual
        do
            let actual = T.Bar("!!!")
            let expected = "\"!!!\" false foobar"
            Tests.Assert(actual = expected) "result should be '%s' but was '%s'" expected actual

module Generics1 = 
    type T = Generics1.Generated
    Tests.AddTest "Generic1" <| fun() ->
        let resTy = T.Producer.MakeAndGet().GetType()
        Tests.Assert(resTy.IsConstructedGenericType && resTy.GetGenericTypeDefinition() = typedefof<ResizeArray<_>>) "ResizeArray expected"
        Tests.Assert(resTy.GetGenericArguments().[0] = typeof<T.Element>) "ElementType Expected"
        ()



module StaticConstructorAndStaticFields =
    type T = StaticConstructorAndStaticFields.Generated
    Tests.AddTest "StaticConstructorAndStaticFields " <| fun() ->
        let t = T.StateProperty
        Tests.Assert (t.Name = "StateProperty") "Unexpected property name %s" t.Name
        ()

module GeneratedTypesWithInstanceMethodsAndStaticParams =
    [<Literal>]
    let StaticParam = "Value3"
    type T = GeneratedTypesWithInstanceMethodsAndStaticParams.Generated<StaticParam>
    Tests.AddTest "GeneratedTypesWithInstanceMethodsAndStaticParams" <| fun() ->
        let expected = 5
        let t = T(expected)
        let prop = t.Value3
        Tests.Assert (prop = expected) "Property value should be %d but was %d" expected prop
        let v = t.Method_Value3()
        let expectedText = System.String.Format("StaticParam:{0}, Field:{1}", StaticParam, expected) 
        Tests.Assert (v = expectedText) "Expected text to be '%s' but got '%s'" expectedText v

module GeneratedTypesThatInheritFromNonObjTypeAndImplementInterface =
    [<Literal>]
    let StaticParam = "Value2"
    type T = GeneratedTypesThatInheritFromNonObjTypeAndImplementInterface.Generated<StaticParam>
    Tests.AddTest "GeneratedTypesThatInheritFromNonObjTypeAndImplementInterface" <| fun () ->
        let t = T()
        Tests.Assert (t.InitializeLifetimeService() = null) "Should be null"
        let counter = ref 0
        t.PropertyChanged.Add(fun arg -> 
            Tests.Assert (arg.PropertyName = StaticParam) "Expected property name '%s', got '%s'" StaticParam arg.PropertyName
            incr counter
        )
        t.Value2 <- "!"
        Tests.Assert (!counter = 1) "Increment expected"
        let r = t.Value2
        Tests.Assert (r = "!") "Expected value %s, got %s" "!" r
        t.Value2 <- "!"
        Tests.Assert (!counter = 1) "Increment not expected"

module GeneratedTypesWithStaticParams =
    [<Literal>]
    let StaticParam = "Value1"
    type T = GeneratedTypesWithStaticParams.Generated<StaticParam>
    Tests.AddTest "GeneratedTypesWithStaticParams" <| fun() ->
        let v = T.Method_Value1()
        Tests.Assert (v = StaticParam) "result should be %s but was %s" StaticParam v

module RefParameters = 
    type T = RefParams.Erased
    Tests.AddTest "RefParameters" <| fun() ->
        let f = ref Unchecked.defaultof<_>
        let x = T.Method1(f)
        Tests.Assert x "x should be true, but was %b" x
        Tests.Assert (!f = (1,2)) "f should be 1,2 but was %A" f

module VirtualCallsOnValueTypes = 
    type T = TestVirtualCallsOnValueTypes.Root
    Tests.AddTest "VirtualCallsOnValueTypes" <| fun () ->
        let dt = System.DateTime.Now
        let dtStr = dt.ToString()
        let dtStr2 = T.DateTimeToString(dt)
        if dtStr <> dtStr2 then failwithf "%s and %s should be equal" dtStr dtStr2

module TestDecimalValues =
    type T = TestDecimalValues.Generated
    Tests.AddTest "TestDecimalValues" <| fun() ->
        let a = T.``Get 1``()
        Tests.Assert (a = 1m) "Expected 1, got %A" a

        let a = T.``Get -1001``()
        Tests.Assert (a = -1001m) "Expected -1001m, got %A" a

        let a = T.``Get 1001001``()
        Tests.Assert (a = 1001001m) "Expected 1001001m, got %A" a

module DateTimeValuesTest = 
    type T = DateTimeValuesTest.Generated

    Tests.AddTest "DateTimeValuesTest" <| fun () ->
        let dateTime = T.GetDate()
        let dateTimeString = T.GetDateAsString()
        let expected = System.DateTime.Parse(dateTimeString, System.Globalization.CultureInfo.InvariantCulture)
        Tests.Assert(dateTime.Ticks = expected.Ticks) "Ticks: expected %A, got %A" expected.Ticks dateTime.Ticks
        Tests.Assert(dateTime.Kind = System.DateTimeKind.Utc) "Kind: expected Utc, got %A" dateTime.Kind

module DateTimeOffsetValuesTest = 
    type T = DateTimeOffsetValuesTest.Generated
    Tests.AddTest "DateTimeOffsetValuesTest" <| fun() ->
        let dto = T.GetDate()
        let ticks, offsetTicks = T.GetTicks()
        let expected = System.DateTimeOffset(ticks, System.TimeSpan(offsetTicks))
        Tests.Assert (dto = expected) "Expected %A, got %A" expected dto

module ConstantValues = 
    module Erased = 
        type T = TestConstantValues.Erased
        Tests.AddTest "ConstantValues.Erased" <| fun () ->
            T.GetNull() |> ignore
            T.GetSystem_Boolean() |> ignore
            T.GetSystem_Byte() |> ignore
            T.GetSystem_Char() |> ignore
            T.GetSystem_Double() |> ignore
            T.GetSystem_IO_FileShare() |> ignore
            T.GetSystem_Int16() |> ignore
            T.GetSystem_Int32() |> ignore
            T.GetSystem_Int64() |> ignore
            T.GetSystem_SByte() |> ignore
            T.GetSystem_Single() |> ignore
            T.GetSystem_String() |> ignore
            T.GetSystem_UInt16() |> ignore
            T.GetSystem_UInt32() |> ignore
            T.GetSystem_UInt64() |> ignore
                
            T.GetBoxedNull() |> ignore
            T.GetBoxedSystem_Boolean() |> ignore
            T.GetBoxedSystem_Byte() |> ignore
            T.GetBoxedSystem_Char() |> ignore
            T.GetBoxedSystem_Double() |> ignore
            T.GetBoxedSystem_IO_FileShare() |> ignore
            T.GetBoxedSystem_Int16() |> ignore
            T.GetBoxedSystem_Int32() |> ignore
            T.GetBoxedSystem_Int64() |> ignore
            T.GetBoxedSystem_SByte() |> ignore
            T.GetBoxedSystem_Single() |> ignore
            T.GetBoxedSystem_String() |> ignore
            T.GetBoxedSystem_UInt16() |> ignore
            T.GetBoxedSystem_UInt32() |> ignore
            T.GetBoxedSystem_UInt64() |> ignore
    module Generated = 
        type T = TestConstantValues.Generated
        Tests.AddTest "ConstantValues.Generated" <| fun () ->
            T.GetNull() |> ignore
            T.GetSystem_Boolean() |> ignore
            T.GetSystem_Byte() |> ignore
            T.GetSystem_Char() |> ignore
            T.GetSystem_Double() |> ignore
            T.GetSystem_IO_FileShare() |> ignore
            T.GetSystem_Int16() |> ignore
            T.GetSystem_Int32() |> ignore
            T.GetSystem_Int64() |> ignore
            T.GetSystem_SByte() |> ignore
            T.GetSystem_Single() |> ignore
            T.GetSystem_String() |> ignore
            T.GetSystem_UInt16() |> ignore
            T.GetSystem_UInt32() |> ignore
            T.GetSystem_UInt64() |> ignore
                
            T.GetBoxedNull() |> ignore
            T.GetBoxedSystem_Boolean() |> ignore
            T.GetBoxedSystem_Byte() |> ignore
            T.GetBoxedSystem_Char() |> ignore
            T.GetBoxedSystem_Double() |> ignore
            T.GetBoxedSystem_IO_FileShare() |> ignore
            T.GetBoxedSystem_Int16() |> ignore
            T.GetBoxedSystem_Int32() |> ignore
            T.GetBoxedSystem_Int64() |> ignore
            T.GetBoxedSystem_SByte() |> ignore
            T.GetBoxedSystem_Single() |> ignore
            T.GetBoxedSystem_String() |> ignore
            T.GetBoxedSystem_UInt16() |> ignore
            T.GetBoxedSystem_UInt32() |> ignore
            T.GetBoxedSystem_UInt64() |> ignore

module EffectsInArgumentsOfErasedMethods =
    type T = EffectsInArgumentsOfErasedMethods.Root
    Tests.AddTest "EffectsInArgumentsOfErasedMethods" <| fun () ->
        let counter = ref 0
        let effect () = 
            incr counter
            obj()

        let res = T.Ignore(effect())
        Tests.Assert res "True expected"
        Tests.Assert (!counter = 1) "Counter should be 1"

        let res = T.Twice(effect())
        Tests.Assert res "True expected"
        Tests.Assert (!counter = 2) "Counter should be 2"



module TupleTests = 
    module Erased = 
        type T = Tuples.Erased
        Tests.AddTest "TupleTests.Erased" <| fun() ->
            let t = T.MethodThatAcceptsTuple((1,2))
            Tests.Assert (t = 1) "Expected 1"

            let t = T.MethodThatAcceptsBigTuple((1,2, 3, 4, 5, 6, 7, 8, 9, 10))
            Tests.Assert (t = 10) "Expected 10, got %d" t

            let a,b = T.MethodThatReturnsTuple 100
            Tests.Assert(a = 100 && b = 100) "100 , 100 expected"

            let a,b, c,d, e, f, g, h, i, j = T.MethodThatReturnsBigTuple 100
            Tests.Assert(a = 100 && b = 100 && c = 100 && d = 100 && e = 100 && f = 100 && g = 100 && h = 100 && i = 100) "100 , 100 expected"

    module Generated = 
        type T = Tuples.Generated
        Tests.AddTest "TupleTests.Generated" <| fun() ->
            let t = T.MethodThatAcceptsTuple((1,2))
            Tests.Assert (t = 1) "Expected 1"

            let t = T.MethodThatAcceptsBigTuple((1,2, 3, 4, 5, 6, 7, 8, 9, 10))
            Tests.Assert (t = 10) "Expected 10, got %d" t

            let a,b = T.MethodThatReturnsTuple 100
            Tests.Assert(a = 100 && b = 100) "100 , 100 expected"

            let a,b, c,d, e, f, g, h, i, j = T.MethodThatReturnsBigTuple 100
            Tests.Assert(a = 100 && b = 100 && c = 100 && d = 100 && e = 100 && f = 100 && g = 100 && h = 100 && i = 100) "100 , 100 expected"

module ParamsArrayTests = 
    module Erased =
        type T = ParamsArray.Erased
        
        Tests.AddTest "ParamsArrayTests.Erased" <| fun () ->
            match T.Get(1,2,3) with
            | [|1;2;3|] -> ()
            | x ->  Tests.Assert false "Unexpected result, got %A" x

    module Generated =
        type T = ParamsArray.Generated
        Tests.AddTest "ParamsArrayTests.Generated" <| fun () ->
            match T.Get(1,2,3) with
            | [|1;2;3|] -> ()
            | x ->  Tests.Assert false "Unexpected result, got %A" x

module TypesWithLoopsTests = 
    module Erased =
        type T = TypesWithLoops.Erased
        Tests.AddTest "TypesWithLoopsTests.Erased" <| fun () ->
            let v = T.MethodWithFor(5, 10)
            Tests.Assert (v = "5678910") "MethodWithFor:Unexpected result, got %s" v
            let v = T.MethodWithWhile(5)
            Tests.Assert (v = "!!!!!") "MethodWithWhile:Unexpected result, got %s" v
    module Generated =
        type T = TypesWithLoops.Generated
        Tests.AddTest "TypesWithLoopsTests.Generated" <| fun () ->
            let v = T.MethodWithFor(5, 10)
            Tests.Assert (v = "5678910") "MethodWithFor:Unexpected result, got %s" v
            let v = T.MethodWithWhile(5)
            Tests.Assert (v = "!!!!!") "MethodWithWhile:Unexpected result, got %s" v

module ArraysOfValueTypesTests = 
    
    let now = System.DateTime.Now
    let nowStr = now.ToString()
    let text = System.Guid.NewGuid().ToString()

    let stringArray = [|text|]
    let dateTimeArray = [|now|]
    let stringArray2D = Array2D.init 1 1 (fun _ _ -> text)
    let dateTimeArray2D = Array2D.init 1 1 (fun _ _ -> now)
    let stringArray3D = Array3D.init 1 1 1 (fun _ _ _ -> text)
    let dateTimeArray3D = Array3D.init 1 1 1 (fun _ _ _ -> now)
    let stringArray4D = Array4D.init 1 1 1 1 (fun _ _ _ _ -> text)
    let dateTimeArray4D = Array4D.init 1 1 1 1 (fun _ _ _ _ -> now)

    module Erased = 
        type T = ArraysOfValueTypes.Erased
        Tests.AddTest "ArraysOfValueTypesTests.Erased" <| fun () ->
            let v = T.DateTimeGet1 dateTimeArray
            Tests.Assert (v = nowStr) "DateTimeGet1: unexpected %s" v
            let v = T.DateTimeGet2 dateTimeArray2D
            Tests.Assert (v = nowStr) "DateTimeGet2: unexpected %s" v
            let v = T.DateTimeGet3 dateTimeArray3D
            Tests.Assert (v = nowStr) "DateTimeGet3: unexpected %s" v
            let v = T.DateTimeGet4 dateTimeArray4D
            Tests.Assert (v = nowStr) "DateTimeGet4: unexpected %s" v

            let v = T.StringGet1 stringArray
            Tests.Assert (v = text) "StringGet1: unexpected %s" v
            let v = T.StringGet2 stringArray2D
            Tests.Assert (v = text) "StringGet2: unexpected %s" v
            let v = T.StringGet3 stringArray3D
            Tests.Assert (v = text) "StringGet3: unexpected %s" v
            let v = T.StringGet4 stringArray4D
            Tests.Assert (v = text) "StringGet4: unexpected %s" v

    module Generated = 
        type T = ArraysOfValueTypes.Generated
        Tests.AddTest "ArraysOfValueTypesTests.Generated" <| fun () ->
            let v = T.DateTimeGet1 dateTimeArray
            Tests.Assert (v = nowStr) "DateTimeGet1: unexpected %s" v
            let v = T.DateTimeGet2 dateTimeArray2D
            Tests.Assert (v = nowStr) "DateTimeGet2: unexpected %s" v
            let v = T.DateTimeGet3 dateTimeArray3D
            Tests.Assert (v = nowStr) "DateTimeGet3: unexpected %s" v
            let v = T.DateTimeGet4 dateTimeArray4D
            Tests.Assert (v = nowStr) "DateTimeGet4: unexpected %s" v

            let v = T.StringGet1 stringArray
            Tests.Assert (v = text) "StringGet1: unexpected %s" v
            let v = T.StringGet2 stringArray2D
            Tests.Assert (v = text) "StringGet2: unexpected %s" v
            let v = T.StringGet3 stringArray3D
            Tests.Assert (v = text) "StringGet3: unexpected %s" v
            let v = T.StringGet4 stringArray4D
            Tests.Assert (v = text) "StringGet4: unexpected %s" v

module NewArrayExprTests = 
    module Erased = 
        type T = NewArrayExpr.Erased
        Tests.AddTest "NewArrayExprTests.Erased" <| fun () ->
            let v = T.Get()
            Tests.Assert (v = [|1;2;3;4;5|]) "Unexpected result: %A" v

    module Generated = 
        type T = NewArrayExpr.Generated
        Tests.AddTest "NewArrayExprTests.Generated" <| fun () ->
            let v = T.Get()
            Tests.Assert (v = [|1;2;3;4;5|]) "Unexpected result: %A" v


module UnboxingTests = 
    module Erased = 
        type T = Unboxing.Erased
        Tests.AddTest "UnboxingTests.Erased" <| fun () ->
            let t = T.Run(box 10)
            Tests.Assert (t = 10) "Unexpected result: %d" t

    module Generated = 
        type T = Unboxing.Generated
        Tests.AddTest "UnboxingTests.Generated" <| fun () ->
            let t = T.Run(box 10)
            Tests.Assert (t = 10) "Unexpected result: %d" t

module AsyncsInGeneratedTypesTests = 
    type T = AsyncsInGeneratedTypes.Generated
    Tests.AddTest "AsyncsInGeneratedTypesTests" <| fun () ->
        let r = T.Run()
        Tests.Assert (r = 100) "Unexpected result %d" r


module ValuesInQuotationsTests = 
    module Erased = 
        type T = ValuesInQuotations.Erased
        Tests.AddTest "ValuesInQuotationsTests.Erased" <| fun () ->
            let v = T.Get()
            Tests.Assert (v = "ab") "Erased.Get::Unexpected result %A" v
            let v = T.GetList()
            Tests.Assert (v = "ab") "Erased.GetList::Unexpected result %A" v

    module Generated = 
        type T = ValuesInQuotations.Generated
        Tests.AddTest "ValuesInQuotationsTests.Generated" <| fun () ->
            let v = T.Get()
            Tests.Assert (v = "ab") "Generated.Get::Unexpected result %A" v
            let v = T.GetList()
            Tests.Assert (v = "ab") "Generated.GetList::Unexpected result %A" v


module ErasedTypesOutParameter = 
    type T = ErasedTypesOutParameter.Erased

    Tests.AddTest "ErasedTypesOutParameter" <| fun() ->
        let x,y = T.Run()
        Tests.Assert x "true expected"
        Tests.Assert (y = "some text") "Expected 'some text', got %s" y

module Program =
    [<EntryPoint>]
    let main(_) = 
        if Tests.Run() 
        then 0 
        else 1
