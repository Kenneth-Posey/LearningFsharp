
#r @"..\Debug\net40\Samples.Hadoop.TypeProviders.dll"

#load "credentials.fsx"

open Samples.Hadoop.Hive
open System.Collections.Generic
open Microsoft.FSharp.Linq.NullableOperators
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

[<Literal>]
let CreatePageView = """
    CREATE TABLE page_view(viewTime INT COMMENT 'View time of the request', 
                           userid BIGINT COMMENT 'ID of the User',
                           page_url STRING COMMENT 'URL of the request', 
                           referrer_url STRING COMMENT 'Referrer of the User',
                           friends ARRAY<BIGINT> COMMENT 'Friends of the User', 
                           properties MAP<STRING, STRING> COMMENT 'Properties of the User',
                           ip STRING COMMENT 'IP Address of the User')
    COMMENT 'This is the page view table'
    PARTITIONED BY(dt STRING, country STRING)
    CLUSTERED BY(userid) SORTED BY(viewTime) INTO 32 BUCKETS
    ROW FORMAT DELIMITED
            FIELDS TERMINATED BY '1'
            COLLECTION ITEMS TERMINATED BY '2'
            MAP KEYS TERMINATED BY '3'
    STORED AS SEQUENCEFILE;
"""

type Data = Samples.Hadoop.HiveTypeProvider<Server=server,Port=port,UserName=user,Password=pwd (* ,AssumedTable=CreatePageView  *) >

let ctxt = Data.GetDataContext()

ctxt.DataContext.ExecuteCommand("DROP TABLE page_view")
ctxt.DataContext.ExecuteCommand(CreatePageView)
ctxt.DataContext.GetTableNames()

//----------------------------------------------------------------
// Create a paritioned iris table
let i = ctxt.iris |> Seq.head

ctxt.DataContext.ExecuteCommand("DROP TABLE iris_partitioned")
ctxt.DataContext.ExecuteCommand("
    CREATE TABLE iris_partitioned(sepallength DOUBLE COMMENT '(required)', 
                                  sepalwidth DOUBLE COMMENT '(required)', 
                                  petallength DOUBLE COMMENT '(required)', 
                                  petalwidth DOUBLE COMMENT '(required)', 
                                  class STRING COMMENT '(required)')
    COMMENT 'This is a partitioned iris table'
    PARTITIONED BY(loadKey1 STRING COMMENT '(required)', loadKey2 STRING COMMENT '(required)')
    CLUSTERED BY(class) SORTED BY(sepallength) INTO 2 BUCKETS
    ROW FORMAT DELIMITED
            FIELDS TERMINATED BY '1'
            COLLECTION ITEMS TERMINATED BY '2'
            MAP KEYS TERMINATED BY '3'
    STORED AS SEQUENCEFILE;
  ")
ctxt.DataContext.GetTableDescription("iris_partitioned")
ctxt.DataContext.GetTableSchema("iris_partitioned")
[ for x in ctxt.iris -> x.``class`` ]

//----------------------------------------------------------------
// Create a iris2 table
ctxt.DataContext.ExecuteCommand("DROP TABLE iris2")
ctxt.DataContext.ExecuteCommand("
    CREATE TABLE iris2(sepallength DOUBLE COMMENT '(required)', 
                       sepalwidth DOUBLE COMMENT '(required)', 
                       petallength DOUBLE COMMENT '(required)', 
                       petalwidth DOUBLE COMMENT '(required)', 
                       class STRING COMMENT '(required)')
    COMMENT 'This is a second iris table'
    ROW FORMAT DELIMITED
            FIELDS TERMINATED BY '1'
            COLLECTION ITEMS TERMINATED BY '2'
            MAP KEYS TERMINATED BY '3'
    STORED AS SEQUENCEFILE;
  ")



//-----------------------------------------------------------------------------------
// Write rows inro iris2 and iris_partitioned. Assumes the target table pre-exists.

hiveQuery {
    for i in ctxt.iris do
    where (i.``class`` = "Iris-setosa")
    writeRows (ctxt.iris2.NewRow(sepallength=i.sepallength, sepalwidth=i.sepalwidth, petallength=i.petallength, petalwidth=i.petalwidth, ``class``=i.``class``))
}

hiveQuery {
    for i in ctxt.iris do
    where (i.``class`` = "Iris-setosa")
    insertRows (ctxt.iris2.NewRow(sepallength=i.sepallength, sepalwidth=i.sepalwidth, petallength=i.petallength, petalwidth=i.petalwidth, ``class``=i.``class``))
}

// TODO: make subsequent queries to intermediateTable work correctly - the table uses a tuple type as its
// row type, but tuple types are not yet valid row types for queries.
let intermediateTable = 
    hiveQuery {
        for i in ctxt.iris do
        where (i.``class`` = "Iris-setosa")
        newTable "foo" (i.sepallength, i.sepalwidth, i.petallength, i.petalwidth)
    }

ctxt.iris2.Run() |> Seq.toList

// Overwrite a partition of a partitioned table. 
hiveQuery {
    for i in ctxt.iris do
    where (i.``class`` = "Iris-setosa")
    // The partition keys must not depend on 'i'
    writeRows (ctxt.iris_partitioned.NewRow(loadkey1="2013-02-12", loadkey2="Iris-setosa",sepallength=i.sepallength, sepalwidth=i.sepalwidth, petallength=i.petallength, petalwidth=i.petalwidth, ``class``=i.``class``))
} 

// SELECT * from a partitioned table based on a partition key. This is fast.
hiveQuery {
    for i in ctxt.iris_partitioned do
    where (i.loadkey1 = "2013-02-12")
    // The partition keys must not depend on 'i'
    select i 
} |> Seq.toList

// SELECT * from a partitioned table based on a non-partition key. This is slow.
hiveQuery {
    for i in ctxt.iris_partitioned do
    where (i.petalwidth >= 0.2)
    select i 
} |> Seq.toList

// SUM from a partitioned table. This is slow.
hiveQuery {
    for i in ctxt.iris_partitioned do
    where (i.loadkey1 = "2013-02-12")
    // The partition keys must not depend on 'i'
    sumBy i.petallength 
} 

// SELECT 'column' from a partitioned table based on a partition key. This is slow.
hiveQuery {
    for i in ctxt.iris_partitioned do
    where (i.loadkey1 = "2013-02-12")
    select i.loadkey1
}  |> Seq.toList

// SELECT sample bucket 
hiveQuery {
    for i in ctxt.iris_partitioned do
    sampleBucket 3 16
    select i.petallength
}  |> Seq.toList

hiveQuery { for x in ctxt.abalone do
            groupBy x.sex into g
            select g.Key }
  |> run

let data3 = 
    hiveQuery { for x in ctxt.bankmarketing do
                groupBy x.y into g
                let avgDuration = hiveQuery { for v in g do sumBy v.duration }
                select (g.Key, avgDuration) }
      |> queryString

//SELECT *,AVG(CONVERT(double,duration)) FROM bankmarketing GROUP BY y
//hiveQuery {
//    for i in ctxt.iris_partitioned do
//    writeLocalFile @"/home/cloudera/a.csv"
//}  |> Seq.toList

//-----------------------------------------------------------------------------------
// General queries

//hiveQuery.Where

let bucket3 = 
    hiveQuery { for x in ctxt.abalone do 
                timeout 120
                where (x.diameter > 1.0)
                sampleBucket 3 16
                select x.sex 
                writeTable "bucket3" } 


hiveQuery { for x in ctxt.abalone do select x.length } |> Seq.toList
hiveQuery { for x in ctxt.abalone do select (x.length, x.rings) } |> Seq.toList
hiveQuery { for x in ctxt.abalone do where (x.length < 0.6); select (x.length, x.rings) } |> Seq.toList
let x0 = hiveQuery { for x in ctxt.abalone do select x } 

let x1 = hiveQuery { for x in ctxt.abalone do count } 
let x2 = hiveQuery { for x in ctxt.abalone do where (x.length < 0.6); count } 


// Test a simple 'let'
hiveQuery { for x in ctxt.abalone do let a = 1.0 in select (x.diameter, 1.0) } |> Seq.toList
hiveQuery { for x in ctxt.abalone do let a = 1.0 in select (x.diameter, a) } |> Seq.toList
hiveQuery { for x in ctxt.abalone do select (x.diameter, 1.0) } |> Seq.toList

(*

[<Literal>]
let CreatePageViewEx3 = """                           
    CREATE TABLE page_view_ex3(viewTime INT, userid BIGINT,
                    page_url STRING, referrer_url STRING,
                    ip STRING COMMENT 'IP Address of the User')
    COMMENT 'This is the page view table'
    PARTITIONED BY(dt STRING, country STRING)
    CLUSTERED BY(userid) SORTED BY(viewTime) INTO 32 BUCKETS
    ROW FORMAT DELIMITED
            FIELDS TERMINATED BY '1'
            COLLECTION ITEMS TERMINATED BY '2'
            MAP KEYS TERMINATED BY '3'
    STORED AS SEQUENCEFILE;"""

ctxt.DataContext.ExecuteCommand("DROP TABLE page_view_ex3")
ctxt.DataContext.ExecuteCommand(CreatePageViewEx3)
ctxt.DataContext.GetTableNames()

ctxt.DataContext.GetTableNames()
ctxt.DataContext.ExecuteCommand("DROP TABLE page_view_ex1")
ctxt.DataContext.GetTableNames()
ctxt.DataContext.ExecuteCommand("
    CREATE TABLE page_view_ex1(viewTime INT, userid BIGINT,
                    page_url STRING, referrer_url STRING,
                    ip STRING COMMENT 'IP Address of the User')
    COMMENT 'This is the page view table'
    PARTITIONED BY(dt STRING, country STRING)
    STORED AS SEQUENCEFILE")
ctxt.DataContext.GetTableNames()
// HIVE doesn't like this:
//ctxt.DataContext.ExecuteCommand("""INSERT INTO TABLE page_view (viewTime, userid, page_url, referrer_url, ip, dt, country)
//                                   VALUES (100, 200, 'http://mysite/page1', 'http://bing.com', '192.168.0.1', '2008', 'uk')""")

ctxt.DataContext.ExecuteCommand("DROP TABLE page_view")
ctxt.DataContext.GetTableNames()
ctxt.DataContext.ExecuteCommand("
    CREATE TABLE page_view_ex2(viewTime INT, userid BIGINT,
                    page_url STRING, referrer_url STRING,
                    ip STRING COMMENT 'IP Address of the User')
    COMMENT 'This is the page view table'
    PARTITIONED BY(dt STRING, country STRING)
    ROW FORMAT DELIMITED
            FIELDS TERMINATED BY '1'
    STORED AS SEQUENCEFILE;
  ")
ctxt.DataContext.GetTableNames()
*)

(*
[<Comment("This is the page view table")>]
[<HiveStorage("CLUSTERED BY(userid) SORTED BY(viewTime) INTO 32 BUCKETS
               ROW FORMAT DELIMITED
                        FIELDS TERMINATED BY '1'
                        COLLECTION ITEMS TERMINATED BY '2'
                        MAP KEYS TERMINATED BY '3'
               STORED AS SEQUENCEFILE
            ")>]
type PageView = 
   { viewTime : int
     userid: int64
     page_url : string
     referrer_url : string
     friends : int[]
     properties : IDictionary<string,string>
     [<Comment("IP Address of the User")>]
     ip : string
     [<PartitionKey(0)>]
     dt : string
     [<PartitionKey(1)>]
     country: string }

*)


type PageViewFake =
    { viewTime : int } 
let t = ctxt.DataContext.GetTable<PageViewFake>("page_view")

// TODO: this doesn't return the partition names. Perhaps that's because there is no data
t.GetPartitionNames()

(*
//----------------------------------------------------------------
// Populate a table using direct commands

ctxt.DataContext.ExecuteCommand("
    INSERT OVERWRITE TABLE iris_partitioned PARTITION(loadKey1='2013-02-12', loadKey2='Iris-setosa')
    SELECT i.sepallength, i.sepalwidth, i.petallength, i.petalwidth, i.class     FROM iris i

    WHERE i.class = 'Iris-setosa';
    ",timeout=300<s>)
ctxt.DataContext.ExecuteCommand("
    FROM iris i
    INSERT OVERWRITE TABLE iris_partitioned PARTITION(loadKey1='2013-02-13', loadKey2='Iris-fake1')
    SELECT i.sepallength, i.sepalwidth, i.petallength, i.petalwidth, 'Iris-fake1'
    WHERE i.class = 'Iris-versicolor';
    ",timeout=300<s>)
ctxt.DataContext.ExecuteCommand("
    FROM iris i
    INSERT OVERWRITE TABLE iris_partitioned PARTITION(loadKey1='2013-02-13', loadKey2='Iris-fake2')
    SELECT i.sepallength, i.sepalwidth, i.petallength, i.petalwidth, 'Iris-fake2'
    WHERE i.class = 'Iris-versicolor';
    ",timeout=300<s>)
let tp = ctxt.DataContext.GetTable<HiveDataRow>("iris_partitioned")
tp.GetSchema()

[ for x in tp -> x.GetValue("sepallength") : float ]
[ for x in tp -> x.GetValue("sepalwidth") : float ]
[ for x in tp -> x.GetValue("petallength") : float ]
[ for x in tp -> x.GetValue("petalwidth") : float ]
[ for x in tp -> x.GetValue("class") : string ]
[ for x in tp -> x.GetValue("loadkey1") : string ]
[ for x in tp -> x.GetValue("loadkey2") : string ]
fsi.PrintLength <- 1000

t.GetPartitionNames() 
*)
  //[|"loadkey1=2013-02-12/loadkey2=Iris-setosa";
  //  "loadkey1=2013-02-12/loadkey2=Iris-versicolor";
  //  "loadkey1=2013-02-13/loadkey2=Iris-fake1"|]

(*
async { try 
          let! v1 = asyncOp "A"
          let! v2 = asyncOp "B"
          return (v1+v2) 
        with _ -> 
          return 0 }
-->
    <@ async.Delay(fun () -> async.TryWith(async.Delay(fun () -> async.Bind(asyncOp "A", fun v1 -> async.Bind(asyncOp "B", fun v2 -> async.Return(v1+v2)))))) @>


asyncSeq { for i in asyncSeqA do 
               let! v1 = asyncOp "A"
               let! v2 = asyncOp "B"
               yield (v1+v2) }
-->
    async.Delay(fun () -> async.For... Bind(asyncOp "A", fun v1 -> async.Bind(asyncOp "B", fun v2 -> async.Return(v1+v2))))

seq { for i in 0 .. 100 do 
          yield (i, i*i) } 

[ for i in 0 .. 100 do 
     yield (i, i*i) ]

[| for i in 0 .. 100 do 
     yield (i, i*i) |]
     *)

//------------------

(*

open Microsoft.FSharp.Quotations
type B() = 
    member __.Yield(_x:'T) : 'T = failwith "The 'yield' operator may only be executed on the server"
    member __.For(_xs : HiveQueryable<'T>, _f : 'T -> 'U) : HiveQueryable<'U> = failwith "The 'for' operator may only be executed on the server"
    member __.Return(_xs : 'T) : 'T = failwith "The 'select1' operator may only be executed on the server"
    [<CustomOperation("select")>]
    member __.Select(_xs : HiveQueryable<'T>, [<ProjectionParameter>]_f : 'T -> 'U) : HiveQueryable<'U> = failwith "The 'select' operator may only be executed on the server"
    [<CustomOperation("groupBy",AllowIntoPattern=true)>] 
    member __.GroupBy<'T,'Key when 'Key : equality> (source: HiveQueryable<'T>, [<ProjectionParameter>] keySelector:('T -> 'Key)) : HiveQueryable<HiveGrouping<'Key,'T>>  = ignore (source,keySelector); failwith "The 'groupBy' operator may only be executed on the server"
    member __.Quote() = ()
    member sp.Run(q:Expr<'T>)  =  q



let b = B()

b { for x in ctxt.abalone do
    groupBy x.sex into g
    select g.Key }
b { for x in ctxt.abalone do 
    let a = 1.0 
    select (x.diameter, 1.0) } 

b { for x in ctxt.abalone do
    groupBy x.sex into g
    let avg = 1.0 // hiveQuery { for v in g do averageBy v.height }
    let avg2 = 2.0 // hiveQuery { for v in g do averageBy v.height }
    select (g.Key, avg, avg2) }
DerivedPatterns.
let x = b { return 1.0 }
For(...,Lambda(_arg2,Let(g,_arg2,   Let(avg,Application(Lambda(builder,Call(builder,Run,[Quote(...)])),_), 
                                          Call(_,Yield([NewTuple(g,avg)]))))))

   --> 

let y = b { let avg = hiveQuery { for v in ctxt.abalone do averageBy v.height }
            return avg }

b { for x in ctxt.abalone do
    groupBy x.sex into g
    let avg = g.SumBy(fun v -> v.height) 
    select (g.Key, avg) }

b { for x in ctxt.abalone do
    groupBy x.sex into g
    select (g.Key, avg(height)) }
*)


let f (v:Quotations.Expr<int>) = 
    hiveQuery { for x in ctxt.abalone do
                where (x.sex = "M") 
                select %v }
      |> queryString

f <@ 1 @>

query { for x in ctxt.abalone do
        where (x.sex = "M") 
        select %v }

hiveQuery { for x in ctxt.abalone do
            groupBy x.sex into g
            select g.Key }
  |> queryString

hiveQuery { for x in ctxt.abalone do
            groupBy x.sex into g
            let avg = hiveQuery { for v in g do averageBy v.height }
            select (g.Key, avg) } 
  |> queryString

open System.Linq

type Obj = { X: double; Y: double}

let data = 
    query { for x in [for i in 0 .. 10 -> {X=double (i % 3);Y= double (i*i)}] do
            groupBy x.X  into g
            select (g.Key, g.Sum(fun x -> x.Y)) }
      |> Seq.toList

let mockTable = ctxt.DataContext.GetTable<Obj>("hello")

let data2 = 
    hiveQuery { for x in mockTable do
                groupBy x.X  into g
                select (g.Key, g.Distinct().Sum(fun x -> x.Y)) }
      |> Seq.toList



let data3 = 
    hiveQuery { for x in mockTable do
                groupBy x.X  into g
                let sum = g.SumBy(fun v -> v.Y)
                let avg = g.AverageBy(fun v -> v.Y)
                let cnt = g.Distinct().Count() 
                select (g.Key, sum, avg, cnt)
                writeTable "pv_gender_sum" }
      |> Seq.toList


//let data3 = 
//    hiveQuery { let sum = query { for v in mockTable do sumBy v.Y }
//                let avg = query { for v in mockTable do averageBy v.Y }
//                select (sum, avg)}
      //|> Seq.toList


(*
hiveQuery { for pv_user in pv_users do 
            groupBy pv_users.gender into group
            select (group.Key, count (DISTINCT pv_users.userid)

    INSERT OVERWRITE TABLE pv_gender_sum
    SELECT pv_users.gender, count (DISTINCT pv_users.userid)
    FROM pv_users
    GROUP BY pv_users.gender;
*)
