// This sample demonstrates the use of the F# Hadoop/Hive provider from Visual Studio.
//    - The identical programming experience is available in the browser


#r @"..\Debug\net40\Samples.Hadoop.TypeProviders.dll"

#load "credentials.fsx"

open Samples.Hadoop
open Samples.Hadoop.Hive

type ctxt = Samples.Hadoop.HiveTypeProvider<Server=server,Port=port,UserName=user,Password=pwd>

let ctxt = ctxt.GetDataContext(queryTimeout=100)

// Press '.'
hiveQuery { for x in ctxt do
            select x }


let testQuery1 = 
    hiveQuery { for x in ctxt.abalone do
                select x }



module AbaloneCatchAnalysis = 

    /// What's the average shell weight of the Abalone in our ctxt set?
    let averageShellWeightOfAbalone = 
        hiveQuery { for x in ctxt.abalone do
                    averageBy x.shellweight }

module MarketingCampaignAnalysis = 

        /// What's the average duration of a marketing call?
        let averageDuration = 
            hiveQuery { for x in ctxt.bankmarketing  do 
                        averageBy (float x.duration) }

        /// What's the average age for a successful marketing call?
        let averageAgeForPeopleWhoDidBuy = 
            hiveQuery { for a in ctxt.bankmarketing  do 
                        where (a.y = "yes") 
                        averageBy (float a.age)  }
 
        /// What's the average age for an unsuccessful marketing call?
        let averageAgeForPeopleWhoDidNotBuy = 
            hiveQuery { for a in ctxt.bankmarketing  do 
                        where (a.y = "no") 
                        averageBy (float a.age)  }

        /// What's the average duration for an unsuccessful marketing call?
        let averageDurationForPeopleWhoDidNotBuy = 
            hiveQuery { for a in ctxt.bankmarketing  do 
                        where (a.y = "no")
                        averageBy (float a.duration) }

        /// What's the average duration for an unsuccessful marketing call?
        let averageDurationForPeopleWhoDidBuy = 
            hiveQuery { for a in ctxt.bankmarketing  do 
                        where (a.y = "yes")
                        averageBy (float a.duration) }

        /// What's the average age for all categories of marketing call?
        let marketingAnalysis2 = 
            hiveQuery { for a in ctxt.bankmarketing  do 
                        groupBy a.y into group
                        let avAge = hiveQuery { for a in group do averageBy (float a.age) }
                        select (group.Key, avAge) }
            |> Seq.toList
 
        /// What's the average (duration,age,balance,marital) for all categories of marketing call?
        let marketingAnalysis3 = 
            hiveQuery { for a in ctxt.bankmarketing  do 
                        groupBy a.y into group
                        let avDuration = hiveQuery { for a in group do averageBy (float a.duration) }
                        let avAge = hiveQuery { for a in group do averageBy (float a.age) }
                        let avBalance = hiveQuery { for a in group do averageBy a.balance }
                        let avSingle = hiveQuery { for a in group do averageBy (if a.marital = "single" then 1.0 else 0.0) }
                        let count = hiveQuery { for a in group do count }
                        select (group.Key, (avDuration,avAge,avBalance,avSingle,count)) }
            |> Seq.toList
 

//type Hdfs =  Samples.Hadoop.HdfsTypeProvider<Host=server,User=user>
//
//let hdfs = Hdfs.ctxt.abalone.ctxt.Head 10


(*

 //---------------------------------------------------------------------------


 /// What's the biggest Abalone in our ctxt set?
let biggestAbalone = 
    HadoopData.abalone |> hiveQuery.maxBy (fun x -> x.height)


let averageHeight = 
    hiveQuery { for x in HadoopData.abalone  do 
            averageBy x.height }

let males = 
   let maleCount = 
       hiveQuery { for x in HadoopData.abalone  do 
               where (x.sex = "M") 
               count } 

   let totalCount =  
       hiveQuery { for x in HadoopData.abalone  do 
               count } 

   float maleCount / float totalCount




let weightInfo = 
   let maleAvWeight = HadoopData.abalone |> hiveQuery.where (fun x -> x.sex = "M") |> hiveQuery.averageBy (fun x -> x.wholeweight)
   let femaleAvWeight = HadoopData.abalone |> hiveQuery.where (fun x -> x.sex = "F") |> hiveQuery.averageBy (fun x -> x.wholeweight)
   maleAvWeight, femaleAvWeight


let weightInfo2 = 
   hiveQuery { for x in HadoopData.abalone do
           groupBy x.sex into group
           let av = hiveQuery { for g in group do averageBy g.wholeweight }
           select (group.Key, av) }
   |> hiveQuery.toList

let education = 
   let smartLadiesCount = 
       hiveQuery { for x in HadoopData.bankmarketing do 
               where (x.marital = "single") 
               where (x.education = "tertiary")
               count } 

   let smartLadiesCount = 
       hiveQuery { for x in HadoopData.bankmarketing do 
               where (x.marital = "single") 
               where (x.loan = "yes")
               count } 

   let ladiesCount = 
       hiveQuery { for x in HadoopData.bankmarketing do 
               where (x.marital = "single") 
               where (x.loan = "yes")
               count } 

   let totalCount =  
       hiveQuery { for x in HadoopData.abalone  do count } 

   float ladiesCount / float totalCount


let averageHeightOfAbalone = 
    HadoopData.abalone 
        |> hiveQuery.averageBy (fun x -> x.height)

let smallestAbaloneByLength = 
    HadoopData.abalone |> hiveQuery.sortBy (fun x -> x.length) |> hiveQuery.head

let biggestAbsloneByLength = 
    HadoopData.abalone |> hiveQuery.sortBy (fun x -> x.length) |> hiveQuery.head

let lengthOfBiggestAbslone2 = 
    HadoopData.abalone |> hiveQuery.maxBy (fun x -> x.length) 

*)

//T2.
    
//for x in T2.abalone do x.
//for x in T2.bankmarketing do x.a


//T2.abalone |> Seq.head |> fun y -> y.
//hiveQuery { for i in T2.abalone do         
//        select (i.diameter, i.rings) }   

//T2.carevaluation |> Seq.head |> fun y -> y.maint











//T2.bankmarketing |> Seq.head |> fun z -> z.

        

//type T9 = HiveProvider.HiveTyped<"openfsharp">

//
//type T = HiveProvider.HiveTyped<>

//
//
//
//let ctxt = T.iris
//T.iris.First()
//
//
//
//hiveQuery { for i in T.iris do 
//        where (i.sepal_width ?>= 3.0)
//        select (i.petal_length, i.sepal_length) }
//
//// Example - average of two columns
//let data2 = 
//    hiveQuery { let g = hiveQuery { for i in T.iris do 
//                            where (i.sepal_width ?>= 3.0) }
//            let avg1 = g.Average(fun (i: T.DataTypes.iris) -> i.petal_length)
//            let avg2 = g.Average(fun (i: T.DataTypes.iris) -> i.sepal_length)
//            select (avg1, avg2) }
//
//// Example - average of two columns, using no LINQ operators
//let data3 = 
//    hiveQuery { let g = hiveQuery { for i in T.iris do 
//                            where (i.sepal_width ?>= 3.0) }
//            let avg1 = hiveQuery { for i in g do averageByNullable i.petal_length }
//            let avg2 = hiveQuery { for i in g do averageByNullable i.sepal_length }
//            select (avg1, avg2) }
//
//
//
//
//
//
//
//
//
//
//
//// HiveQuery("select petal_length,petal_count from iris where sepal_width >= 3 limit 200")
//
////hiveQuery { for i in T.iris do 
////        where (i.sepal_width ?>= 3.0)
////        select (i.petal_length, i.sepal_length) }
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//T.iris.Where(fun (i:T.DataTypes.iris) -> i.petal_length ?>= 3.0 )
//
//
//
//
//
//
//
//
//
//
//
//
//T.iris
//T.iris.Select(fun i -> 3.0 )
//T.iris.Select(fun (i:T.DataTypes.iris) -> i.petal_width) |> Seq.toList
//T.iris.Where(fun (i:T.DataTypes.iris) -> i.petal_length ?>= 3.0 )
//T.iris.Where(fun (i:T.DataTypes.iris) -> i.petal_width ?>= 1.4 )
//
//
//// Connectivity option 1: assume an ODBC ctxt Source has been registered for 
//// the hive connection????
//// type T = HiveTyped<OdbcDataSourceName="LocalHiveServer">
//
//// Connectivity option 2: give a full ODBC connection string
//// type T = HiveTyped<OdbcConnectionString="Provider=MSDASQL.1;Persist Security Info=False;DSN=LocalHiveServer">
//
//// Connectivity option 2: give a host/port (except we don't know how to map to a ODBC connection string)
////   + later options - "UseFramedPacketCommunication=true"
////   + later options - "UserName=foo"
////   + later options - "Password=true"
//// type T = HiveTyped<Host="myserver",Port=10000>
//
//
//
//
//
//
//(*
//    hiveQuery { for i in T_iris do 
//            where (i.sepal_width ?>= 3.0) 
//            select (i.petal_length, i.sepal_length) }
//
//*)
//(*
//
//
//type IrisElement = 
//  { sepal_length : System.Nullable<float>
//    sepal_width : System.Nullable<float>
//    petal_length : System.Nullable<float>
//    petal_width : System.Nullable<float> 
//    ``class`` : string }
//
//let T_iris : System.Linq.IQueryable<IrisElement> = failwith ""
//
//// Example - simple filter/select
//let data1 = 
//    hiveQuery { for i in T_iris do 
//            where (i.sepal_width ?>= 3.0) 
//            select (i.petal_length, i.sepal_length) }
//
//// Example - average of two columns
//let data2 = 
//    hiveQuery { let g = hiveQuery { for i in T_iris do 
//                            where (i.sepal_width ?>= 3.0) }
//            let avg1 = g.Average(fun i -> i.petal_length)
//            let avg2 = g.Average(fun i -> i.sepal_length)
//            select (avg1, avg2) }
//
//// Example - average of two columns, using no LINQ operators
//let data3 = 
//    hiveQuery { let g = hiveQuery { for i in T_iris do 
//                            where (i.sepal_width ?>= 3.0) }
//            let avg1 = hiveQuery { for i in g do averageByNullable i.petal_length }
//            let avg2 = hiveQuery { for i in g do averageByNullable i.sepal_length }
//            select (avg1, avg2) }
//
//
////-----
//
//(*
//each : Column<T> -> T
//avg : Column<T> -> T
//select : T -> unit
//sortBy : T -> unit
//
//let data4 = 
//    query2 { from T.iris
//             where (each T.iris.sepal_width ?>= 3.0) 
//             select (T.iris.petal_length, T.iris.sepal_length) }
//
//let data4b = 
//    query2 { from T.iris
//             where (each T.iris.sepal_width ?>= 3.0) 
//             sortBy (each T.iris.sepal_width) 
//             select (avg T.iris.petal_length, avg T.iris.sepal_length) }
//*)
//
//
//              //where (i.sepal_length = 3) 
//              //sortByNullable i.petal_length
//              //select (g.Count())// note, huge list of functions here
//              //take 200 
//
//let result1 = T_iris.Where(fun i -> (i.sepal_length ?= 3.0)).Take(200).Average(fun i -> i.petal_length)
//let result2 = T_iris.Where(fun i -> (i.sepal_length ?= 3.0)).Take(200).Average(fun i -> i.petal_length)
//
//
//
//
//T_iris.Where(fun i -> 
//
//count: string -> string
//select: string -> string
// 
//select (count(T.petal_length) (T.sepal_length === Q "3")
//
//
//
// //) .Take(200).Average(fun i -> i.petal_length)
//
//
// //--> select avg(petal_length),avg(sepal_length) from iris where sepal_width >= 3 order by sepal_width, sepal_length limit 200
//*)
