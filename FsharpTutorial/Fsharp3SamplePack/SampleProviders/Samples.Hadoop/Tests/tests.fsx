
#r "System.Transactions"
//#load @"C:\projects\openfsharp\Scripts\extlib--LinqEx-0.1.fsx"
#r @"..\Debug\net40\Samples.Hadoop.TypeProviders.dll"
#load "credentials.fsx"

let test msg a = if a then printfn "passed %s" msg else printfn "** FAILED: %s" msg
let check msg a b = if a = b then printfn "passed %s" msg else printfn "** FAILED: %s, got %A, expected %A" msg a b 

module BasicOdbcConnectivity = 
    open System.Data.Odbc

    //let cns = sprintf "Driver=HIVE;server=%s;Port=%d;AUTHENTICATION=3;UID=drs1004;PWD=jHadoop1" server port
    //let cns = sprintf "Driver=HIVE;server=%s;Port=%d;AUTHENTICATION=3;UID=cloudera;PWD=jHadoop1" server port
    let cns = sprintf "Driver=HIVE;server=%s;Port=%d;UID=%s;PWD=%s" server port user pwd
    let conn = new OdbcConnection(cns)

    conn.Open()
    let command = conn.CreateCommand(CommandText="SHOW TABLES;")
    let reader = command.ExecuteReader()
    let lines = [|  while reader.Read() do yield reader.GetString(0) |]
    command.Dispose()

    let command2 = conn.CreateCommand(CommandText="DESCRIBE extended abalone;")
    let reader2 = command2.ExecuteReader()
    let lines2 = [|  while reader2.Read() do yield reader2.GetString(0), (try reader2.GetString(1) with _ -> null) |]
    command2.Dispose()


    let command3 = conn.CreateCommand(CommandText="SELECT * FROM abalone;")
    let reader3 = command3.ExecuteReader()
    let lines3 = [|  while reader3.Read() do yield (try reader3.GetString(0) with _ -> null) |]
    command3.Dispose()

    
    //let command4 = conn.CreateCommand(CommandText="SELECT COUNT(*) FROM abalone;")
    //let reader4 = command4.ExecuteReader()
    //let lines4 = [|  while reader4.Read() do yield (try reader4.GetString(0) with _ -> null) |]
    //command4.Dispose()

module BasicTypeProviderTests = 
    open System.Linq
    open Samples.Hadoop.Hive
    open Microsoft.FSharp.Linq.NullableOperators

    type Data = Samples.Hadoop.HiveTypeProvider<Server=server,Port=port,UserName=user,Password=pwd>

    let ctxt = Data.GetDataContext(queryTimeout=100000)

    test "cwoioicjw1" (ctxt.DataContext.GetTableNames().Contains "abalone")
    test "cwoioicjw1" (ctxt.DataContext.GetTableDescription("abalone").StartsWith("<para>Abalone data</para><para>Number of Instances: 4177</para><para>Predicting"))
    
    check "cwoioicjw3" ctxt.DataContext.Server server
    check "cwoioicjw4" ctxt.DataContext.Port port

    check "colqeoihe1" ctxt.abalone.QueryString "SELECT * FROM abalone"
    check "colqeoihe5" ((hiveQuery { for x in ctxt.abalone do take 10; select x }).QueryString) "SELECT * FROM abalone LIMIT 10"
    //ctxt.GetType().GetField("actualTimeout", System.Reflection.BindingFlags.NonPublic ||| System.Reflection.BindingFlags.Instance).GetValue(ctxt)

    
    check "colqeoihe5" ((hiveQuery { for x in ctxt.abalone do take 10; select (id x) }).QueryString) "SELECT * FROM abalone LIMIT 10"
    // TODO: the column selections should  be folded into the Hive query 
    check "colqeoihe5" ((hiveQuery { for x in ctxt.abalone do take 10; select x.length }).QueryString) "SELECT * FROM abalone LIMIT 10"
    check "colqeoihe5" ((hiveQuery { for x in ctxt.abalone do take 10; select (x.length, x.diameter) }).QueryString) "SELECT * FROM abalone LIMIT 10"
    

    // FAIL: LINQ IQueryable not implemented
    //check "colqeoihe2" (ctxt.abalone.WhereQ(fun x -> x.sex = "M").ToString()) "Table \"abalone\""
    // check "colqeoihe3" (ctxt.abalone.Take(10).ToString()) "Table \"abalone\""

#if NO_REAL_HADOOP_EXECUTION
#else
    // These are all quick as full map/reduce are not scheduled for
    //    SELECT * FROM table
    //    SELECT * FROM table LIMIT n
    check "eclnewio1" (hiveQuery { for x in ctxt.abalone do select x } |> Seq.length) 4177
    check "eclnewio1" (hiveQuery { for x in ctxt.abalone do select x.length } |> Seq.length) 4177
    check "eclnewio1" (hiveQuery { for x in ctxt.abalone do select x.length } |> Seq.length) 4177
    check "eclnewio2" (hiveQuery { for x in ctxt.abalone do take 10; select x } |> Seq.length) 10
    check "eclnewio3" (hiveQuery { for x in ctxt.abalone do take 1; select x } |> Seq.length) 1
    check "eclnewio4" (hiveQuery { for x in ctxt.abalone do take 0; select x } |> Seq.length) 0
    check "eclnewio5" (hiveQuery { for x in ctxt.abalone do take 1000; select x } |> Seq.length) 1000

    // This is the first 'real' Hadoop query that actually queues a map-reduce job.
    //
    // This takes > 15sec.
    check "eclnewio6" (hiveQuery { for x in ctxt.abalone do where (x.sex = "M"); select x } |> Seq.length) 1528


(*
    let run (query: HiveQuery<_>) = 
        async { let! results = query.AsAsync()
                printfn "done!"
                return results }

    query |> runBlocking
    query |> runAsTask 
    query |> treatAsAsync
    query |> treatAsObservable // (but not much point as results are not streaming in our impl.)

    // Cancellation: For now, we expect the user to manually cancel jobs via the Hadoop head node web UI
    //
    // Long running queries: 
    //     - For now, we provide a 'convert to async' and the user looks after the rest
    //     - When 'AsAsync' is used, the timeout is removed, or can be specified as optional
    //       argument in AsAsync(timeout=1000)
    //     - Hence the default timeout is 'default timeout for synchronous operations'

*)
    
#endif

    check "colqeoihe4" ((hiveQuery { for x in ctxt.abalone do select x }).QueryString) "SELECT * FROM abalone"
    check "colqeoihe4" ((hiveQuery { for x in ctxt.abalone do where (x.sex = "M"); select x }).QueryString) "SELECT * FROM abalone WHERE sex = 'M'"
    
    check "colqeoihe61" ((hiveQuery { for x in ctxt.bankmarketing do where (x.age > 4); select x }).QueryString) "SELECT * FROM bankmarketing WHERE age > 4"
    check "colqeoihe62" ((hiveQuery { for x in ctxt.bankmarketing do where (x.balance > 4.0); select x }).QueryString) "SELECT * FROM bankmarketing WHERE balance > 4"
    check "colqeoihe63" ((hiveQuery { for x in ctxt.bankmarketing do where (x.balance < 4.0000001); select x }).QueryString) "SELECT * FROM bankmarketing WHERE balance < 4.0000001"
    check "colqeoihe64" ((hiveQuery { for x in ctxt.bankmarketing do where (x.balance <= 4.0000000001); select x }).QueryString) "SELECT * FROM bankmarketing WHERE balance <= 4.0000000001"
    check "colqeoihe65" ((hiveQuery { for x in ctxt.bankmarketing do where (x.balance >= 4.0000000001); select x }).QueryString) "SELECT * FROM bankmarketing WHERE balance >= 4.0000000001"

    check "colqeoihe4" ((hiveQuery { for x in ctxt.sample_07 do where (x.salary ?> 4); select x }).QueryString) "SELECT * FROM sample_07 WHERE salary > 4"
    
    // TODO: allow .Value, .HasValue
    //check "colqeoihe4" ((hiveQuery { for x in ctxt.sample_07 do where (x.salary.Value > 4); select x }).QueryString) "SELECT * FROM abalone WHERE sex = 'M'"
    //check "colqeoihe4" ((hiveQuery { for x in ctxt.sample_07 do where x.salary.HasValue; select x }).QueryString) "SELECT * FROM abalone WHERE sex = 'M'"
    // TODO: nullable query operators
    
    // NOTE: I don't know if this use of 'NaN' runs OK
    check "colqeoihe65" ((hiveQuery { for x in ctxt.bankmarketing do where (x.balance >= nan); select x }).QueryString) "SELECT * FROM bankmarketing WHERE balance >= NaN"
    check "colqeoihe65" ((hiveQuery { for x in ctxt.bankmarketing do where (x.housing = "yes"); select x }).QueryString) "SELECT * FROM bankmarketing WHERE housing = 'yes'"



module BasicTypeProviderTestsOverMockHadoopSystem = 
    open System.Linq
    // TODO: put 'hiveQuery' in a better place
    open Samples.Hadoop.Hive
    open Microsoft.FSharp.Linq.NullableOperators

    type Data = Samples.Hadoop.HiveTypeProvider<Server="tryfsharp">

    let ctxt = Data.GetDataContext()

    test "cwoioicjw1" (ctxt.DataContext.GetTableNames().Contains "abalone")
    
    test "cwoioicjw2" (ctxt.DataContext.GetTableDescription "abalone" |> fun c -> c.StartsWith "<para>Abalone")
    check "cwoioicjw3" ctxt.DataContext.Server "tryfsharp"
    check "cwoioicjw4" ctxt.DataContext.Port 10000 // default port

    check "colqeoihe1" ctxt.abalone.QueryString "SELECT * FROM abalone"
    check "colqeoihe5" ((hiveQuery { for x in ctxt.abalone do take 10; select x }).QueryString) "SELECT * FROM abalone LIMIT 10"

    check "eclnewio1" (hiveQuery { for x in ctxt.abalone do select x } |> Seq.length) 4177

    check "eclnewio2" (hiveQuery { for x in ctxt.abalone do take 10; select x } |> Seq.length) 10
    check "eclnewio3" (hiveQuery { for x in ctxt.abalone do take 1; select x } |> Seq.length) 1
    check "eclnewio4" (hiveQuery { for x in ctxt.abalone do take 0; select x } |> Seq.length) 0
    check "eclnewio5" (hiveQuery { for x in ctxt.abalone do take 1000; select x } |> Seq.length) 1000
    check "eclnewio6" (hiveQuery { for x in ctxt.abalone do where (x.sex = "M"); select x } |> Seq.length) 1528

    check "colqeoihe4" ((hiveQuery { for x in ctxt.abalone do select x }).QueryString) "SELECT * FROM abalone"
    check "colqeoihe4" ((hiveQuery { for x in ctxt.abalone do where (x.sex = "M"); select x }).QueryString) "SELECT * FROM abalone WHERE sex = 'M'"
    
    check "colqeoihe61" ((hiveQuery { for x in ctxt.bankmarketing do where (x.age > 4); select x }).QueryString) "SELECT * FROM bankmarketing WHERE age > 4"
    check "colqeoihe62" ((hiveQuery { for x in ctxt.bankmarketing do where (x.balance > 4.0); select x }).QueryString) "SELECT * FROM bankmarketing WHERE balance > 4"
    check "colqeoihe63" ((hiveQuery { for x in ctxt.bankmarketing do where (x.balance < 4.0000001); select x }).QueryString) "SELECT * FROM bankmarketing WHERE balance < 4.0000001"
    check "colqeoihe64" ((hiveQuery { for x in ctxt.bankmarketing do where (x.balance <= 4.0000000001); select x }).QueryString) "SELECT * FROM bankmarketing WHERE balance <= 4.0000000001"
    check "colqeoihe65" ((hiveQuery { for x in ctxt.bankmarketing do where (x.balance >= 4.0000000001); select x }).QueryString) "SELECT * FROM bankmarketing WHERE balance >= 4.0000000001"

(*
    // FAILS: Nullable operators are not yet translated
    // Gives 'the variable 'x' was not found in the translation context', a misleading error I think
    check "colqeoihe4" ((hiveQuery { for x in ctxt.sample_07 do where (x.salary ?> 4); select x }).QueryString) "SELECT * FROM abalone WHERE sex = 'M'"
    check "colqeoihe4" ((hiveQuery { for x in ctxt.sample_07 do where (x.salary.Value > 4); select x }).QueryString) "SELECT * FROM abalone WHERE sex = 'M'"
    check "colqeoihe4" ((hiveQuery { for x in ctxt.sample_07 do where x.salary.HasValue; select x }).QueryString) "SELECT * FROM abalone WHERE sex = 'M'"
*)
    
    // NOTE: I don't know if this use of 'NaN' runs OK
    check "colqeoihe65" ((hiveQuery { for x in ctxt.bankmarketing do where (x.balance >= nan); select x }).QueryString) "SELECT * FROM bankmarketing WHERE balance >= NaN"
    check "colqeoihe65" ((hiveQuery { for x in ctxt.bankmarketing do where (x.housing = "yes"); select x }).QueryString) "SELECT * FROM bankmarketing WHERE housing = 'yes'"
