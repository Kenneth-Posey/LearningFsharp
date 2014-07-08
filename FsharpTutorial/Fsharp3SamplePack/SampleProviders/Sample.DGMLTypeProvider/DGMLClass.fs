﻿namespace StateMahcineTypeProvider

open System.Xml
open System.Xml.Linq

type IState = interface
    abstract member EnterFunction : unit -> unit
    abstract member ExitFunction : unit->unit
end

// define a node record
type Node = { Name : string; NextNodes : Node list}

// define DGML DU
type DGML = 
    | Node of string
    | Link of string * string

// define DGML class
type DGMLClass() = class
    
    let mutable nodes = Unchecked.defaultof<Node list>
    let mutable links = Unchecked.defaultof<DGML list>    
    let mutable currentState = System.String.Empty

    // current state
    member this.CurrentState 
        with get() = currentState
        and private set(v) = currentState <- v

    // all links in the DGML file
    member this.Links 
        with get() = links
        and private set(v) = links <- v

    // all nodes in the DGML file
    member this.Nodes 
        with get() = nodes
        and private set(v) = nodes <- v              

    // initialize the state machien from fileName and set the initial state to initState
    member this.Init(fileName:string, initState) = 
        let file = XDocument.Load(fileName, LoadOptions.None)
        this.Links <- 
            file.Descendants() 
            |> Seq.filter (fun node -> node.Name.LocalName = "Link")
            |> Seq.map (fun node -> 
                            let sourceName = node.Attribute(XName.Get("Source")).Value
                            let targetName = node.Attribute(XName.Get("Target")).Value
                            DGML.Link(sourceName, targetName))
            |> Seq.toList

        let getNextNodes fromNodeName= 
                this.Links 
                |> Seq.filter (fun (Link(a, b)) -> a = fromNodeName)
                |> Seq.map (fun (Link(a,b)) -> this.FindNode(b))
                |> Seq.filter (fun n -> match n with Some(x) -> true | None -> false)
                |> Seq.map (fun (Some(n)) -> n)
                |> Seq.toList

        this.Nodes <-
            file.Descendants() 
            |> Seq.filter (fun node -> node.Name.LocalName = "Node")
            |> Seq.map (fun node -> DGML.Node( node.Attribute(XName.Get("Id")).Value) )
            |> Seq.map (fun (Node(n)) -> { Node.Name=n; NextNodes = [] })
            |> Seq.toList
        
        this.Nodes <-    
            this.Nodes
            |> Seq.map (fun n -> { n with NextNodes = (getNextNodes n.Name) } )  
            |> Seq.toList

        this.CurrentState <- initState

    // fine the node by given nodeName
    member this.FindNode(nodeName) : Node option= 
        let result = 
            this.Nodes 
            |> Seq.filter (fun n -> n.Name = nodeName)
        if result |> Seq.isEmpty then None
        else result |> Seq.head |> Some    

    // current node
    member this.CurrentNode
        with get() = 
            this.Nodes 
            |> Seq.filter (fun n -> n.Name = this.CurrentState)
            |> Seq.head
            
    // determine if can transit to a node represented by the nodeName
    member this.CanTransitTo(nodeName:string) =
        this.CurrentNode.NextNodes |> Seq.exists (fun n -> n.Name = nodeName)

    // force current state to a new state
    member this.ForceStateTo(args) = 
        this.CurrentState <- args
        
    // assert function used for debugging
    member this.Assert(state) = 
        if this.CurrentState <> state then
            failwith "assertion failed"
end

// state machine class which inherit the DGML class
// and use a MailboxProcessor to perform asynchronous message processing
type StateMachine() as this= class
    inherit DGMLClass()

    let functions = System.Collections.Generic.Dictionary<string, IState>()
    let processor = new MailboxProcessor<string>(fun inbox ->
                    let rec loop () = 
                        async {
                            let! msg = inbox.Receive()
                            if this.CanTransitTo(msg) then
                                this.InvokeExit(this.CurrentNode.Name)
                                this.ForceStateTo(msg)                
                                this.InvokeEnter(msg)            
                            return! loop ()
                        }
                    loop ())    

    do 
        processor.Start()

    // define the second constructor taking the file name and initial state name
    new(fileName, initState) as secondCtor= 
        new StateMachine()
        then
            secondCtor.Init(fileName, initState)

    // asynchronously transit to a new state
    member this.TransitTo(state) = 
        processor.Post(state)

    // set the transition function
    member this.SetFunction(name:string, state:IState) = 
        if functions.ContainsKey(name) then 
            functions.[name] <- state
        else
            functions.Add(name, state)

    // invoke the Exit function
    member private this.InvokeExit(name:string) = 
        if functions.ContainsKey(name) then
            functions.[name].ExitFunction()

    // invoke the Etner function
    member private this.InvokeEnter(name:string) = 
        if functions.ContainsKey(name) then
            functions.[name].EnterFunction()
end

