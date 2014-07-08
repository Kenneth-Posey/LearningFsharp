// Copyright (c) Microsoft Corporation 2005-2011.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

[<Support.Helper.SampleAttributes.Sample("Others")>]
module Samples.Others

open System
open System.Collections.Generic
open System.Runtime.Serialization.Json
open System.Runtime.Serialization 
open Support.Helper

[<Support("OtherCompositePatternSample")>]
[<Support("OtherCompositePatternSample2")>]
[<Support("OtherCompositePatternSample3")>]
[<Support("OtherCompositePatternSample4")>]
let dummy() = ()
type CompositeNode<'T> = 
    | Node of 'T
    | Tree of 'T * CompositeNode<'T> * CompositeNode<'T>
    with 
        member this.InOrder f = 
            match this with
            | Tree(n, left, right) -> 
                left.InOrder f
                f n
                right.InOrder f
            | Node n -> f n
        member this.PreOrder f =
            match this with
            | Tree(n, left, right) ->                 
                f n
                left.PreOrder f
                right.PreOrder f
            | Node n -> f n
        member this.PostOrder f =
            match this with
            | Tree(n, left, right) -> 
                left.PostOrder f
                right.PostOrder f
                f n
            | Node n -> f n

[<Category("OO Design Patterns");
  Title("Composite pattern");
  Description("For more information about composite pattern, please go to http://en.wikipedia.org/wiki/Composite_pattern")>]
let OtherCompositePatternSample() = 
    let tree = Tree(1, Tree(11, Node 12, Node 13), Node 2)
    let nodeAccessFunc = printf "%A,"

    printf "in order process: "
    tree.InOrder nodeAccessFunc
    printfn ""

    printf "pre order process: "
    tree.PreOrder nodeAccessFunc
    printfn ""

    printf "post order process: "
    tree.PostOrder nodeAccessFunc
    printfn ""

[<Category("OO Design Patterns");
  Title("Composite pattern with variable");
  Description("This implementation uses a global variable resultRef to hold the result. For more information, please go to http://en.wikipedia.org/wiki/Composite_pattern")>]
let OtherCompositePatternSample2() = 
    let tree = Tree(1, Tree(11, Node 12, Node 13), Node 2)
    let resultRef = ref 0
    let nodeAccessFunc n = resultRef := !resultRef + n
    tree.PreOrder nodeAccessFunc
    printfn "result = %d" !resultRef

    Assert.AreEqual(!resultRef, 39)

[<Support("OtherCompositePatternSample3")>]
let dumm2() = ()
type IA<'T> =
        abstract member Do : 'T -> unit
        abstract member Result : unit -> 'T

[<Category("OO Design Patterns");
  Title("Composite pattern with variable in a class");
  Description("After define a variable in the class-like structure, use a property to retrieve the result back when comptation is finished. For more information, please go to http://en.wikipedia.org/wiki/Composite_pattern")>]
let OtherCompositePatternSample3() = 
    let tree = Tree(1, Tree(11, Node 12, Node 13), Node 2)
    let wrapper = 
        let result = ref 0
        { new IA<int> with                
                member this.Do n = 
                    result := !result + n
                member this.Result() = !result
        }
    tree.PreOrder wrapper.Do
    printfn "result = %d" (wrapper.Result())

[<Support("OtherCompositePatternSample4")>]
let dumm3() = ()
type IA2<'T> =
    abstract member Do : 'T -> unit        

[<Category("OO Design Patterns");
  Title("Composite pattern with variable in a class-like structure");
  Description("Composite pattern with variable in a class-like structure. The computation result will be brought back by the class-like structure. For more information, please go to http://en.wikipedia.org/wiki/Composite_pattern")>]
let OtherCompositePatternSample4() = 
    let tree = Tree(1, Tree(11, Node 12, Node 13), Node 2)
    let wrapper = 
        let result = ref 0
        ({ new IA2<int> with                
                member this.Do n = 
                    result := !result + n                
        }, result)
    tree.PreOrder (fst wrapper).Do
    printfn "result = %d" !(snd wrapper)

[<Support("OtherCompositePatternSample5")>]
let dummy2() = ()
type TreeStructure<'T> = 
    | Node of 'T * TreeStructure<'T> * TreeStructure<'T> 
    | Leaf

[<Category("OO Design Patterns");
  Title("Composite pattern & continuation");
  Description("Composite pattern with continuation implementation to get the sum of all tree nodes.  For more information, please go to http://en.wikipedia.org/wiki/Composite_pattern")>]
let OtherCompositePatternSample5() = 
    let tree = Node(4, Node(2, Node(1, Leaf, Leaf), Node(3, Leaf, Leaf)), 
                    Node(6, Node(5, Leaf, Leaf), Node(7, Leaf, Leaf)))
    let foldTree nodeF leafV t = 
        let rec Loop t cont = 
            match t with 
            | Node(x,left,right) -> Loop left  (fun lacc ->  
                                    Loop right (fun racc -> 
                                    cont (nodeF x lacc racc))) 
            | Leaf -> cont leafV 
        Loop t (fun x -> x)
 
    let sumTree = foldTree (fun x l r -> x + l + r)   0 
    let inOrder = foldTree (fun x l r -> l @ [x] @ r) [] 
    let height  = foldTree (fun _ l r -> 1 + max l r) 0
    printfn "sum = %d" (sumTree tree)
    printfn "inorder = %A" (inOrder tree)
    printfn "height = %d" (height tree)

[<Support("CommandPatternSample")>]
let dummy4() = ()
type Command = { Redo: unit->unit
                 Undo: unit->unit }

[<Category("OO Design Patterns");
  Title("Command pattern");
  Description("Command pattern to demostrate a redo-undo framework. Each command contains both Do and Undo section. For more information, please go to http://en.wikipedia.org/wiki/Command_pattern")>]
let CommandPatternSample() = 
    let result = ref 7
    let add n = { Redo = (fun _ -> result:= !result + n); Undo = (fun _ -> result := !result - n) }
    let minus n = { Redo = (fun _ -> result:= !result - n); Undo = (fun _ -> result := !result + n) }
    let cmd = add 3
    printfn "current state = %d" !result
    cmd.Redo()
    printfn "after redo: %d" !result
    cmd.Undo()
    printfn "after undo: %d" !result 

[<Support("CommandPatternSample2")>]
let dummy5() = ()
type CommandType = 
    | Deposit
    | Withdraw
type TCommand = 
    | Command of CommandType * int

[<Category("OO Design Patterns");
  Title("Command pattern");
  Description("Command pattern to demostrate a redo-undo framework. This implementation group the commands under Do/Undo category. For more information, please go to http://en.wikipedia.org/wiki/Command_pattern")>]
let CommandPatternSample2() = 
    let result = ref 7
    let deposit x = result := !result + x
    let withdraw x = result := !result - x
    let Do = fun (cmd:TCommand) ->
        match cmd with
        | Command(CommandType.Deposit, n) -> deposit n
        | Command(CommandType.Withdraw,n) -> withdraw n
    let Undo = fun (cmd:TCommand) ->
        match cmd with
        | Command(CommandType.Deposit, n) -> withdraw n
        | Command(CommandType.Withdraw,n) -> deposit n
    printfn "current balance %d" !result
    let depositCmd = Command(Deposit, 3)
    Do depositCmd
    printfn "after deposit: %d" !result
    Undo depositCmd
    printfn "after undo: %d" !result

[<Support("DesignPatter1")>]
let dummy6() = ()
type A private () =
    static let instance = A()
    static member val Instance = instance
    member this.Action() = printfn "action"

[<Category("OO Design Patterns");
  Title("Singleton pattern");
  Description("The key point is the private constructor which makes sure only one instance is created. For more information, please go to http://en.wikipedia.org/wiki/Singleton_pattern")>]
let DesignPatter1() = 
    let a = A.Instance
    a.Action()

[<Support("factory")>]
let dummy61() = ()
type IA = 
  abstract Action : unit -> unit
type Type = 
  | TypeA 
  | TypeB

[<Category("OO Design Patterns");
  Title("Factory pattern");
  Description("Factory pattern implementation: it returns different objects based on inputs. For more information, please go to http://en.wikipedia.org/wiki/Factory_method_pattern")>]
let factorySample() = 
    let factory = function
      | TypeA -> { new IA with 
                       member this.Action() = printfn "type A" }
      | TypeB -> { new IA with 
                      member this.Action() = printfn "type B" }
    let output = factory Type.TypeA
    output.Action()

[<Support("state")>]
let dummy7() = ()
type AccountState = 
    | Overdrawn
    | Silver
    | Gold
[<Measure>] type USD
type Account<[<Measure>] 'U>() =
    let mutable balance = 0.0<_>   
    member this.State = 
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
    member this.Deposit(x:float<_>) =  
        let (a:float<_>) = x
        balance <- balance + a
    member this.Withdraw(x:float<_>) = 
        balance <- balance - x

[<Category("OO Design Patterns");
  Title("State pattern");
  Description("State pattern implementation shows how a class's internal state can change its behavior. For more information, please go to http://en.wikipedia.org/wiki/Factory_method_pattern")>]
let state() = 
    let account = Account()
    account.Deposit(10000.<USD>)
    printfn "interest = %A" (account.PayInterest())
    account.Withdraw(20000.<USD>)
    printfn "interest = %A" (account.PayInterest())

[<Support("stragegy")>]
let dummy8() = ()
let quicksort l = 
    printfn "quick sort"
let shellsort l = 
    printfn "shell short"
let bubblesort l = 
    printfn "bubble sort"
type Strategy() = 
    let mutable sortFunction = fun _ -> ()
    member this.SetStrategy f = sortFunction <- f
    member this.Execute n = sortFunction n

[<Category("OO Design Patterns");
  Title("Strategy pattern");
  Description("Strategy pattern shows the underlying algorithm can be changed dynamically by setting the function from outside. For more information, please go to http://en.wikipedia.org/wiki/Strategy_pattern")>]
let stragegy() = 
    let s = Strategy()
    s.SetStrategy(quicksort)
    s.Execute [1..6]

[<Support("proxy")>]
let dummy9() = ()
type CoreComputation() = 
    member this.Add x = x + 1
    member this.Sub x = x - 1
    member this.GetProxy name = 
        match name with
        | "Add" -> (this.Add, "add")
        | "Sub" -> (this.Sub, "sub")
        | _ -> failwith "not supported"

[<Category("OO Design Patterns");
  Title("Proxy pattern");
  Description("Proxy pattern provides a placeholder to expose different methods. For more information, please go to http://en.wikipedia.org/wiki/Proxy_pattern")>]
let proxy() = 
    let core = CoreComputation()
    let proxy = core.GetProxy "Add"
    printfn "result = %d" ((fst proxy) 1)

[<Support("adapter")>]
let dummy11() = ()
type Cat() = 
    member this.Walk() = printfn "cat walk"
type Dog() = 
    member this.Walk() = printfn "dog walk"

[<Category("OO Design Patterns");
  Title("Adapter pattern");
  Description("Adapter pattern make incompatible types work (walk) together without changing existing code. In the sample the dog and cat are imcompatible types. For more information, please go to http://en.wikipedia.org/wiki/Adapter_pattern")>]
let adapter() = 
    let cat = Cat()
    let dog = Dog()
    let inline walk (x : ^T) = (^T : (member Walk : unit->unit) (x))
    walk cat
    walk dog


[<Support("ChainOfResponsibility")>]
let dummy10() = ()
type Record = {
    Name : string;
    Age : int;
    Weight: float;
    Height: float;
}
[<Category("OO Design Patterns");
  Title("Chain of responsibility pattern");
  Description("Chain of responsibility pattern shows how a request can go through different function (responsibility) by using function composition. For more information, please go to http://en.wikipedia.org/wiki/Chain-of-responsibility_pattern")>]
let ChainOfResponsibility() = 
    let validAge (record:Record) = 
        record.Age < 65 && record.Age > 18
    let validWeight (record:Record) = 
        record.Weight < 200.
    let validHeight (record:Record) = 
        record.Height > 120.

    let check (f:Record->bool) (record:Record, result:bool) = 
        if result=false then (record, false)
        else (record, f(record))

    let chainOfResponsibility = check validAge >> check validWeight >> check validHeight

    let john = { Name = "John"; Age = 80; Weight = 180.; Height=180. }
    let dan = { Name = "Dan"; Age = 20; Weight = 160.; Height=190. }

    printfn "john result = %b" ((chainOfResponsibility (john, true)) |> snd)
    printfn "dan result = %b" ((chainOfResponsibility (dan, true)) |> snd)

[<Category("OO Design Patterns");
  Title("Chain of responsibility pattern");
  Description("Chain of responsibility pattern implementation shows how a request can go through different function (responsibility) by using pipeline.  For more information, please go to http://en.wikipedia.org/wiki/Chain-of-responsibility_pattern")>]
let ChainOfResponsibility2() = 
    let chainTemplate processFunction canContinue s = 
        if canContinue s then processFunction s
        else s

    let canContinueF _ = true
    let processF x = x + 1

    let chainFunction = chainTemplate processF canContinueF   //combine two functions to get a chainFunction
    let s = 1 |> chainFunction |> chainFunction
    printfn "%A" s

[<Support("decorate")>]
let dummy12() = ()
type Divide() = 
    let mutable divide = fun (a,b) -> a / b
    member this.Function
        with get() = divide
        and set(v) = divide <- v
    member this.Invoke(a,b) = divide (a,b)

[<Category("OO Design Patterns");
  Title("Decorate pattern");
  Description("Decorate pattern implementation shows how we can add new functionality (check zero) dynamically. For more information, please go to http://en.wikipedia.org/wiki/Decorator_pattern")>]
let decorate() = 
    let d = Divide()
    let checkZero (a,b) = if b = 0 then failwith "a/b and b is 0" else (a,b)
    try 
        d.Invoke(1, 0) |> ignore
    with e -> printfn "without check, the error is = %s" e.Message
    d.Function <- checkZero >> d.Function 
    try
        d.Invoke(1,0) |> ignore
    with e -> printfn "after add check, error is = %s" e.Message

[<Support("observer")>]
let dummy13() = ()
type Subject() = 
    let mutable notify = fun _ -> ()
    member this.Subscribe (notifyFunction) = 
        let wrap f i = f(i); i
        notify <- (wrap notifyFunction) >> notify
    member this.Reset() = notify <- fun _ -> ()
    member this.SomethingHappen(k) = 
        notify k

type ObserverA() =
    member this.NotifyMe(i) = printfn "notified A %A" i
type ObserverB() = 
    member this.NotifyMeB(i) = printfn "notified B %A" i
[<Category("OO Design Patterns");
  Title("Observer pattern");
  Description("This sample show how a change can notify different subscribers. For more information, please go to http://en.wikipedia.org/wiki/Observer_pattern")>]
let observer() = 
    let a = ObserverA()
    let b = ObserverB()
    let subject = Subject()
    subject.Subscribe a.NotifyMe
    subject.Subscribe b.NotifyMeB
    subject.SomethingHappen "good"


[<Support("Application1")>]
let dummy14() = ()
type IAccount = 
    abstract Withdraw : int -> unit
    abstract Deposit : int -> unit
type AccountInfo = {
    Balance: int
}

type BankAccount (initialBalance) = 
    let mutable accountInfo = { Balance = initialBalance }
    member this.AccountInfo
        with get() = accountInfo
    member this.GetCommand() = 
        if accountInfo.Balance >= 0 then
             { new IAccount with
                 member this.Withdraw(amount) = 
                    if (amount > accountInfo.Balance) then printfn "cannot withdraw"
                    else accountInfo <- { accountInfo with Balance = accountInfo.Balance - amount }
                 member this.Deposit(amount) = 
                    accountInfo <- { accountInfo with Balance = accountInfo.Balance + amount } }
        else
             { new IAccount with
                 member this.Withdraw(amount) = 
                    printfn "not enough fund"
                 member this.Deposit(amount) = 
                    accountInfo <- { accountInfo with Balance = accountInfo.Balance + amount } }

[<Category("Application");
  Title("Bank account");
  Description("Bank account with threadsafe deposit and withdraw feature. This implementation generates the object according to its internal state.")>]
let Application1() = 
    let account = BankAccount(-1)
    account.GetCommand().Withdraw(1)
    account.GetCommand().Deposit(100)
    account.GetCommand().Withdraw(5)

[<Support("Application2")>]
let dummy15() = ()
type private BankBase() = 
    let mutable accountInfo = { Balance = 0 }
    member this.AccountInfo = accountInfo
    member this.Deposit amount = 
        printfn "will deposit %A" amount
        accountInfo <- { this.AccountInfo with Balance = this.AccountInfo.Balance + amount }
    member this.Withdraw amount = 
        printfn "will withdraw %A" amount
        accountInfo <- { this.AccountInfo with Balance = this.AccountInfo.Balance - amount }

type IQueryAccount = 
    abstract Query : unit -> AccountInfo
type IFullControlAccount =
    inherit IQueryAccount
    abstract Deposit : int -> unit
    abstract Withdraw : int -> unit
type AccessType = 
    | QueryOnly
    | Full

let private createAccess(account:BankBase, ``access type``) = 
        match ``access type`` with
            | QueryOnly -> { new IQueryAccount with
                                member this.Query() = account.AccountInfo }
            | Full -> { new IFullControlAccount with
                            member this.Deposit(amount) = account.Deposit(amount)
                            member this.Withdraw(amount) = account.Withdraw(amount)
                            member this.Query() = account.AccountInfo } :> IQueryAccount

[<Category("Application");
  Title("Bank account and access control");
  Description("This is a Factory Pattern usage. The function returns different types of object when given different access control level. ")>]
let Application2() =     
    let account = BankBase()
    let access = createAccess (account, AccessType.Full) :?> IFullControlAccount
    let queryOnly = createAccess (account, AccessType.QueryOnly)
    printfn "balance is %d" (queryOnly.Query().Balance)
    access.Deposit 100
    printfn "balance is %d" (queryOnly.Query().Balance)
    

                                
    

