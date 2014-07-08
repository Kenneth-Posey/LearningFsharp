// Copyright (c) Microsoft Corporation 2005-2011.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 
[<Support.Helper.SampleAttributes.Sample("Intermediate")>]
module Samples.Intermediate

open System
open System.Collections.Generic
open System.Runtime.InteropServices
open System.Globalization
open System.Runtime.CompilerServices
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns
open System.Windows.Input
open Support.Helper

//--------------------------------------------------------
#nowarn "686"
  
[<Support("ObjectExpression")>]
let dummy21() = ()

let createCommand action canExecute=
            let event1 = Event<_, _>()
            {
                new ICommand with
                    member this.CanExecute(obj) = canExecute(obj)
                    member this.Execute(obj) = action(obj)
                    member this.add_CanExecuteChanged(handler) = event1.Publish.AddHandler(handler)
                    member this.remove_CanExecuteChanged(handler) = event1.Publish.RemoveHandler(handler)
            }

let myCommand = createCommand
                        (fun _ -> ())
                        (fun _ -> true)

[<Category("Object Expression");
  Title("Object Expression");
  Description("Create WPF command using ObjectExpression.")>]
let ObjectExpression() =
    printfn "WPF command using object expression"
  
[<Category("Data Types");
  Title("Using the Set type");
  Description("Create a set of particular unicode characters using a Microsoft.FSharp.Collections.Set.")>]
let SampleSet1() =
    let data = "The quick brown fox jumps over the lazy dog" 
    let set = 
        data.ToCharArray()
        |> Set.ofSeq
    for c in set do 
        printfn "Char : '%c'" c 
  

[<Category("Data Types");
  Title("Using the Map type");
  Description("Create a histogram of the occurrences of particular unicode characters using a Microsoft.FSharp.Collections.Map.")>]
let SampleMap1() =
    let data = "The quick brown fox jumps over the lazy dog" 
    let histogram = 
        data.ToCharArray()
        |> Seq.groupBy (fun c -> c)
        |> Map.ofSeq
        |> Map.map (fun k v -> Seq.length v)
    for (KeyValue(c,n)) in histogram do 
        printfn "Number of '%c' characters = %d" c n 

[<Category("Data Types");
  Title("typeof operator");
  Description("Demostrate how to use typeof operator.")>]
let SampleTypeOf() = 
    let intType = typeof<int>
    printfn "%s can be retrieved by using typeof" (intType.Name)

[<Category("Data Types");
  Title("typedefof operator");
  Description("Demostrate how to use typedefof operator to create generic type.")>]
let SampleTypeDefof() = 
    let intType = typeof<int>
    let listType = typedefof<List<_>>
    let listIntType = listType.MakeGenericType(intType) 
    printfn "make a List<int> type using typedefof operator."
    printfn "the newly created type's full name is %s" (listIntType.FullName)

[<Category("Data Types");
  Title("lazy");
  Description("Demostrate how to use lazy keyword.")>]
let SampleLazy() = 
    let a = lazy 4
    let b = lazy 5
    let c = lazy (a.Force() + b.Force())
    printfn "c = %d" (c.Force())
  
[<Support("InterfaceSample3")>]
let dummy4() = ()
type IPoint = 
    abstract X : float
    abstract Y : float


/// Implementing an interface with an object expression.
let Point(x,y) =
    { new IPoint with 
         member obj.X=x 
         member obj.Y=y }

/// Implementing an interface with an object expression that has mutable state
let MutablePoint(x,y) =
    let currX = ref x 
    let currY = ref y
    { new IPoint with 
         member obj.X= currX.Value 
         member obj.Y= currY.Value }

/// This interface is really just a function since it only has one 
/// member, but we give it a name here as an example. It represents 
/// a function from some variable to (X,Y)
type ILine = 
    abstract Get : float -> IPoint

/// Implementing an interface with an object expression.
///
/// Here a line is specified by gradient/intercept
let Line(a:float,c:float) = 
    let y(x) = a * x + c
    { new ILine with 
        member l.Get(x) = Point(x, y(x)) }
 
/// Implementing an interface with a class.
///
/// Here a line is specified by gradient/intercept
type GradientInterceptLine(a:float,c:float) = 
    // Some local bindings
    let y(x) = a * x + c

    // Publish additional properties of the object
    member x.Gradient = a
    member x.Intercept = c

    // Also implement the interface
    interface ILine with 
        member l.Get(x) = Point (x,y(x))

[<Category("Defining Types and Functions");
  Title("Using interfaces");
  Description("A longer sample showing how to use interfaces to model 'data' objects such as abstract points.  Somewhat contrived, since multiple repreentations of points are unlikely practice, but for larger computational objects maintaining flexibility of representation through using interfaces or function values is often crucial.")>]
/// Using the above types
let InterfaceSample3() =
    
    // This creates an object using a function
    let line1 = Line(1.0, 0.344)

    // This creates a similar object using a type. 
    let line2 = new GradientInterceptLine(2.0,1.5) :> ILine 
    let origin =  { new IPoint with 
                        member p.X =0.0 
                        member p.Y = 0.0 }
    let point1 = line1.Get(-1.0)
    let point2 = line2.Get(0.0)
    let point3 = line2.Get(1.0)
    let outputPoint os (p:IPoint) = fprintf os "(%f,%f)" p.X p.Y 
    printfn "origin = %a" outputPoint origin;
    printfn "point1 = %a" outputPoint point1;
    printfn "point2 = %a" outputPoint point2;
    printfn "point3 = %a" outputPoint point3

[<Category("Defining Types and Functions");
  Title("Upcast and downcast");
  Description("Use upcast and downcast")>]
let UpcastDownCastSample1() = 
    let a = 1
    let upcastFunction(x) : obj = upcast x
    let downcastFunction(x:obj) : int = downcast x
    let objValue = upcastFunction(a)
    printfn "name = %A" objValue
    printfn "int = %d" (downcastFunction(objValue))

[<Support("GenericSample1")>]
let dummy12() = ()
type TrackedValue<'Kind>(v : 'Kind) =
  let mutable value = v 
  let mutable reads  = 0 
  let mutable writes = 0 

  member this.Value
    with get() =
      reads <- reads + 1
      value
    and set newVal =
      writes <- writes + 1
      value <- newVal

[<Category("Defining Types and Functions");
  Title("Generic Type");
  Description("Define a generic type")>]
let GenericSample1() = 
    let a = TrackedValue(10)
    let b = TrackedValue<float>(20.0)
    let c = TrackedValue<_>("Hello")
    printfn "%d %f %s" a.Value b.Value c.Value

[<Support("TypeConstraintsSample1")>]
let dummy17() = ()
let inline parseFunction< ^T when ^T : (static member Parse: string -> ^T) > s =
        (^T: (static member Parse: string -> ^T) s)

[<Category("Defining Types and Functions");
  Title("Function with type contraints");
  Description("Define function with type contraints")>]
let TypeConstraintsSample1() =     
    let ``seq`` = [ "1"; "2"; "3" ] |> List.map parseFunction<int>
    ``seq`` |> Seq.iter (printfn "%d")

[<Support("TypeConstraintsSample2")>]
let dummy18() = ()
let inline tryParseFunction< ^T when ^T : (static member TryParse: string * ^T byref -> bool) > s =
    let mutable x = Unchecked.defaultof< ^T>
    if (^T: (static member TryParse: string * ^T byref -> bool) (s, &x)) 
    then Some x 
    else None

[<Category("Defining Types and Functions");
  Title("Function with type contraints");
  Description("Define function with type contraints")>]
let TypeConstraintsSample2() =     
    let ``seq`` = [ "1"; "2"; "3"; "NotInt" ] |> List.map tryParseFunction<int>
    ``seq`` |> Seq.iter (printfn "%A")

[<Support("TypeConstraintsSample3")>]
let dummy19() = ()
let inline tryParseFunction2 s =
    let mutable x = Unchecked.defaultof< ^T>
    if (^T: (static member TryParse: string * ^T byref -> bool) (s, &x)) 
    then Some x 
    else None

[<Category("Defining Types and Functions");
  Title("Function with type contraints");
  Description("Define function with type contraints")>]
let TypeConstraintsSample3() =     
    let ``seq`` = [ "1"; "2"; "3"; "NotInt" ] |> List.map tryParseFunction2<int>
    ``seq`` |> Seq.iter (printfn "%A")

[<Category("Defining Types and Functions");
  Title("Infinite sequence");
  Description("Infinite sequence")>]
let InfiniteSeqSample1() = 
    let evenNumbers:bigint seq = 
        let rec f n = seq { yield n; yield! f (n+2I) }
        f 0I
    printfn "infinite seq = %A" evenNumbers
        
[<Category("Input/Output");
  Title("Lazily Enumerate CSV File");
  Description("Build an IEnumerable<string list> for on-demand reading of a CSV file using .NET I/O utilities and abstractions")>]
let EnumerateCSVFile1() = 

    // Write a test file
    System.IO.File.WriteAllLines(@"test.csv", [| "Desmond, Barrow, Market Place, 2"; 
                                                 "Molly, Singer, Band, 12" |]);

    /// This function builds an IEnumerable<string list> object that enumerates the CSV-split
    /// lines of the given file on-demand 
    let CSVFileEnumerator(fileName) = 

        // The function is implemented using a sequence expression
        seq { use sr = System.IO.File.OpenText(fileName)
              while not sr.EndOfStream do
                 let line = sr.ReadLine() 
                 let words = line.Split [|',';' ';'\t'|] 
                 yield words }

    // Now test this out on our test file, iterating the entire file
    let test = CSVFileEnumerator(@"test.csv")  
    printfn "-------Enumeration 1------";
    test |> Seq.iter (string >> printfn "line %s");
    // Now do it again, this time determining the numer of entries on each line.
    // Note how the file is read from the start again, since each enumeration is 
    // independent.
    printfn "-------Enumeration 2------";
    test |> Seq.iter (Array.length >> printfn "line has %d entries");
    // Now do it again, this time determining the numer of entries on each line.
    // Note how the file is read from the start again, since each enumeration is 
    // independent.
    printfn "-------Enumeration 3------";
    test |> Seq.iter (Array.map (fun s -> s.Length) >> string >> printfn "lengths of entries: %s")
        
[<Category("Event");
  Title("Define event");
  Description("Define event")>]
let defineEvent() = 
    let e = new Event<int>()
    //trigger event, but not subscriber. Nothing happens.
    e.Trigger(5)
    //subscribe to the event
    Event.add (fun n->printfn "%d" n) e.Publish
    //trigger the event again
    e.Trigger(10);

[<Support("ExtensionSample")>]
let dummy6() = ()
type System.String with
        member this.ToSecureString() = 
            let r = new System.Security.SecureString()
            this |> Seq.iter (fun c -> r.AppendChar(c))
            r
[<Category("Extension Methods");
  Title("Define extension methods");
  Description("Define extension methods")>]
let ExtensionSample() = 
    let a = "password";
    let secureString = a.ToSecureString()
    Assert.AreEqual(secureString.Length, a.Length)

[<Support("ExtensionSample2")>]
let dummy7() = ()
type System.String with
    member this.Versions =
        if System.String.IsNullOrEmpty(this) then
            failwith "empty or null string"
        else
            this.Split([|'.'|], System.StringSplitOptions.RemoveEmptyEntries)

[<Category("Extension Methods");
  Title("Define extension methods");
  Description("Define extension methods to get version list from string")>]
let ExtensionSample2() = 
    let a = "1.2.4";
    let versions = a.Versions
    versions |> Seq.iter (fun n -> printfn "%s" n)
    Assert.AreEqual(versions.Length, 3)
    Assert.AreEqual(versions.[0], "1")

[<Support("ExtensionSample3")>]
let dummy71() = ()

[< System.Runtime.CompilerServices.ExtensionAttribute >]
[< AutoOpen >]
module ExtensionMethodForCSharp = 
    [< System.Runtime.CompilerServices.ExtensionAttribute >]
    let IsFSharp(str:string) = str = "F#"

[<Category("Extension Methods");
  Title("Define C# extension method");
  Description("Define C# extension methods")>]
let ExtensionSample3() = 
    printfn "the method is listed on shown as extension method on C# side" 

[<Support("MeasureSample1")>]
[<Support("MeasureSample2")>]
[<Support("MeasureSample3")>]
let dummy8() = ()
[<Measure>]type litre
[<Measure>]type pint

[<Category("Units of Measure");
  Title("Define units of measure");
  Description("Define and use measure unit. The units of measure is a compile time feature, so it won't go into the binary file.")>]
let MeasureSample1() = 
    let v1 = 2.<litre>
    let v2 = 1.<pint>
    let ratio =  1.0<litre> / 1.76056338<pint>
    let pintToLitre pints =
        pints * ratio
    let newVol = v1 + (pintToLitre v2)
    printfn "converted value is %A" newVol
 
[<Category("Units of Measure");
  Title("convert units of measure to float");
  Description("convert measure unit to float.")>]   
let MeasureSample2() = 
    let a = 1.<litre>
    let (b:float) = float(a)
    printfn "measure unit %A" a
    printfn "float value %f" b

[<Category("Units of Measure");
  Title("convert units of measure and LanguagePrimitives");
  Description("convert measure and LanguagePrimitives. The conversion ratio is defined and the conversion is based on the ratio.")>]
let MeasureSample3() = 
    let l:float<litre> = LanguagePrimitives.FloatWithMeasure 10.
    let ratio =  1.0<litre> / 1.76056338<pint>
    let pints = l / ratio
    printfn "%A pints = %A liter" pints l

[<Support("MeasureSample4")>]
let dummy3() = ()
type AccountState = 
    | Overdrawn
    | Silver
    | Gold

[<Measure>] type USD
[<Measure>] type CND

type Account<[<Measure>] 'u>() =
    let mutable balance = 0.0<_>   
    member this.State
        with get() = 
            match balance with
            | _ when balance <= 0.0<_> -> Overdrawn
            | _ when balance > 0.0<_> && balance < 10000.0<_> -> Silver
            | _ -> Gold
    member this.PayInterest() = 
        let interest = 
            match this.State with
                | Overdrawn -> 0.
                | Silver -> 0.01
                | Gold -> 0.02
        interest * balance
    member this.Deposit(x:float<_>) =  balance <- balance + x
    member this.Withdraw(x:float<_>) = balance <- balance - x

[<Category("Units of Measure");
  Title("Aggreate type and LanguagePrimitives");
  Description("Aggreate type and LanguagePrimitives.")>]
let MeasureSample4() = 
    let account = Account<USD>()
    account.Deposit(LanguagePrimitives.FloatWithMeasure 10000.)
    printfn "us interest = %A" (account.PayInterest())
    account.Withdraw(LanguagePrimitives.FloatWithMeasure 20000.)
    printfn "us interest = %A" (account.PayInterest())

    let canadaAccount = Account<CND>()
    canadaAccount.Deposit(LanguagePrimitives.FloatWithMeasure 10000.)
    canadaAccount.Withdraw(LanguagePrimitives.FloatWithMeasure 500.)
    printfn "canadian interest = %A" (canadaAccount.PayInterest())

[<Support("MeasureSample5")>]
let dummy11() = ()
[<Measure>]type JPY
[<Measure>]type CNY
type BankAccount<[<Measure>]'u>() =
    let mutable balance : float<'u> = 0.0<_>
    member acct.HasLotsOfMoney = balance > (LanguagePrimitives.FloatWithMeasure 10000.0) //10000 units
    member acct.Deposit(amt) = balance <- balance + amt
    member acct.Withdraw(amt) = balance <- balance - amt

[<Category("Units of Measure");
  Title("Aggreate type and LanguagePrimitives");
  Description("Aggreate type and LanguagePrimitives. The LanguagePrimitives is used to make a number (e.g. float) into units of measure.")>]
let MeasureSample5() =
    let a = BankAccount<JPY>()
    a.Deposit(10.<JPY>)

    let b = BankAccount<CNY>()
    b.Deposit(10.<CNY>)

    printfn "show how to deposit right currency to right account"

[<Category("Quotation");
  Title("Use quotation");
  Description("Basic quotation sample")>]
let QuotationSample1() = 
    let expr : Expr<int> = <@ 1 + 1 @>
    let expr2 : Expr = <@@ 1 + 1 @@>
    printfn "expr type is %s" expr.Type.Name
    printfn "expr2 type is %s" expr.Type.Name

[<Support("QuotationSample2")>]
let dummy9() = ()
[<ReflectedDefinition>]
let myFunction p0 p2 =
    printf "hello quotation"

[<Category("Quotation");
  Title("Use quotation to get function name and parameters");
  Description("Use quotation to get function name and parameters")>]
let QuotationSample2() =
    let rec get_parms exp =
        match exp with
        | Lambda (var,body) ->
            let index = if (var.Name.IndexOf("@") - 1) < 0 then var.Name.Length-1 else (var.Name.IndexOf("@") - 1)
            let name = [var.Name.[..index]]
            let p = get_parms body
            name @ p        
        | _ -> []
 
    List.iter (printf "%A\r\n") (get_parms <@ myFunction @>)

[<Support("ComputationExpressionSample1")>]
let dummy51() = ()
type Result<'T> = 
    | Success of 'T
    | Failure 

type MaybeBuilder() = 
    member this.Return x = Success x
    member this.Bind (x:Result<'T>, f: 'T->Result<'u>) = 
        match x with
        | Success(x) -> f x
        | Failure -> Failure

let maybe = MaybeBuilder()

let a = 
    maybe {
        let! b = Success(4);
        let! y = Success true
        return b, y
    }

//the above sample can be rewritten as the following
let r = maybe.Bind(Success 4, fun b -> maybe.Bind(Success true, fun y -> maybe.Return(b, y)))

[<Category("Computation Expression");
  Title("Computation Expression sample");
  Description("This sample shows a Computation Expression sample and how it can be rewritten.")>]
let ComputationExpressionSample1() = 
    printfn "the above sample is a Computation Expression sample"

[<Support("InterOpSample1")>]
let dummy5() = ()
[<DllImport("User32.dll", SetLastError=true)>]
extern bool MessageBeep(UInt32 beepType);

[<Category("InterOp");
  Title("Invoke win32 API");
  Description("invoke win23 API to beep on the computer. Please make sure you have sound card installed.")>]
let InterOpSample1() = 
    let r = MessageBeep(0xFFFFFFFFu)
    printfn "message beep result = %A" r

[<Category("InterOp");
  Title("Call .NET functions");
  Description("Invoke other .NET functions")>]
let InterOpSample2() = 
    let today = DateTime.Today
    let tomorrow = today.AddDays(1.0)
    let nextWeek = 7.0 |> today.AddDays
    printfn "tomorrow = %A" tomorrow
    printfn "nextWeek = %A" nextWeek

[<Category("InterOp");
  Title("Use List<T>");
  Description("Use List<T> in .NET")>]
let InterOpSample3() = 
    let shopping = new System.Collections.Generic.List<string>()
    shopping.Add("eggs")
    shopping.Add("milk")
    shopping.Add("bread")
    printfn "List: %A" shopping

    shopping.Insert(1, "wine")
    printfn "List: %A" shopping

    shopping.Remove("milk")  |> ignore
    printfn "List: %A" shopping

[<Category("InterOp");
  Title("Parse number and handle out parameter in C#");
  Description("Parse number and handle out parameter in C#")>]
let InterOpSample4() =
    let parse str =
        match System.Decimal.TryParse(str, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture) with
             | (true, r) -> Some(r)
             | (false, _) -> None

    printfn "%A" (parse "123")
    printfn "%A" (parse "acd")

[<Support("pritnfnSample0")>]
let dummy10() = ()
[<StructuredFormatDisplayAttribute("MyType is {Contents}")>]
type C(elems:int list) =
   member x.Contents = elems

[<Category("Format");
  Title("Use structure format display attribute");
  Description("Use structure format display attribute. Use this attribute we can customerize the printf content.")>]
let pritnfnSample0() = 
    printfn "%A" (C [1..4])

[<Support("printfnSample1")>]
let dummy13() = ()
[<StructuredFormatDisplayAttribute("Tree {Contents}")>]
type Tree(node: int, elems: Tree list) =
   member x.Contents = (node, elems)

[<Category("Format");
  Title("Use structure format display complex structure");
  Description("Use structure format display complex structure")>]
let printfnSample1() = 
    let c = Tree (1, [ Tree (2, []); Tree (3, [ Tree (4, []) ]) ])
    let c2 = Tree (1, [ c; c])
    let c3 = Tree (1, [ c2; c2])
    printfn "%A" c3

[<Support("agentSample0")>]
[<Support("agentSample1")>]
let dummy14() = ()
type Agent<'T> = MailboxProcessor<'T>

[<Category("Mailbox Processor");
  Title("Use mail box processor to process simple message");
  Description("Use mail box processor to process simple message")>]
let agentSample0() = 
    let agent =
       Agent.Start(fun inbox ->
         async { for i in [1..3] do
                   let! msg = inbox.Receive()
                   printfn "got message '%s'" msg } )
    printfn "because the message is running on the background thread, please see the result at the console window behind"
    agent.Post("msg1");
    agent.Post("msg2");
    agent.Post("msg3");

[<Category("Mailbox Processor");
  Title("Use mail box processor and isolation");
  Description("Use mail box processor and isolation. ")>]
let agentSample1() = 
    let agent =
       Agent.Start(fun inbox ->
         async { let strings = Dictionary<string,int>()
                 for i in [1..5] do
                   let! msg = inbox.Receive()
                   if strings.ContainsKey msg then
                       strings.[msg] <- strings.[msg] + 1
                   else
                       strings.[msg] <- 0
                   printfn "message '%s' now seen '%d' times" msg strings.[msg] } )
    printfn "because the message is running on the background thread, please see the result at the console window behind"
    agent.Post("msg1");
    agent.Post("msg2");
    agent.Post("msg3");
    agent.Post("msg1");
    agent.Post("msg2");