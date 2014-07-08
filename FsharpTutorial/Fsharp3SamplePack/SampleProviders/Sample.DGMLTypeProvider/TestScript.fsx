#r @".\bin\Debug\TypeProviderTemplate1.dll"

type T = Samples.ShareInfo.TPTest.TPTestType<"""Graph1.dgml""", "State0">
let t = T()

// define print function
let syncRoot = ref 0
let print str = lock(syncRoot) (fun _ -> printfn "%s" str)

// object expression for transition
let printObj = { new StateMahcineTypeProvider.IState with
                            member this.EnterFunction() = print ("Enter " + t.CurrentState)
                            member this.ExitFunction() = print ("Exit " + t.CurrentState) }

// set the transition functions
t.SetFunction(t.State0, printObj)
t.SetFunction(t.State1, printObj)
t.SetFunction(t.State2, printObj)
t.SetFunction(t.State3, printObj)

// valid transitions
t.TransitTo_State1()
t.TransitTo_State2()
t.TransitTo_State3()

// invalid transition
t.TransitTo_State2()
