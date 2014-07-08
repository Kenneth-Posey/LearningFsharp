
#r @"..\Debug\net40\Samples.Hadoop.TypeProviders.dll"

#load "credentials.fsx"

open Samples.Hadoop.Hive
open Microsoft.FSharp.Linq.NullableOperators

let inline (=~=) (x:float<'u>) y = abs (x - y)  < 0.0001<_>

module WithoutUnits = 
    type Data = Samples.Hadoop.HiveTypeProvider<Server=server,Port=port,UserName=user,Password=pwd>

    let ctxt = Data.GetDataContext(queryTimeout=100)

    // Copy the 'abalone' table to the 'abc' table
    let abc = 
        hiveQuery { for x in ctxt.bankmarketing do 
                    newTable "abc" x } 

    let elems1 = 
        hiveQuery { for x in ctxt.bankmarketing do 
                    sampleBucket 3 16
                    select x  } 
          |> Seq.toArray
                    
    let elems2 = 
        hiveQuery { for x in ctxt.abalone do 
                    select x.height  } 
          |> Seq.toArray

    let abc2 =                         
        hiveQuery { for x in ctxt.abalone do 
                    newTable "abc" (x.length,x.height) } 

    let elems3 = 
        hiveQuery { // Note, you can't do 'for (length,height) in abc2' as yet 
                    for x in abc2 do 
                    let (length,height) = x
                    select height } 
          |> Seq.toArray

    elems2 = elems3

    let res1 =
        hiveQuery { for x in ctxt.abalone do 
                    sumBy x.length } 
    let res2 =
        hiveQuery { for x in ctxt.abalone do 
                    take 10 // NOTE: this 'take' has no effect - it gets translated as LIMIT 10 but is ignored by Hive....
                    sumBy x.length } 


    type Row  = { Length : float; Height : float }
    let abc3 =                         
        hiveQuery { for x in ctxt.abalone do 
                    newTable "abc3" {Length=x.length; Height=x.height } } 

    abc3.GetSchema()

    let elems3 = 
        hiveQuery { for x in abc3 do 
                    select x.Length } 
          |> Seq.toArray

    let dat3 = 
        hiveQuery { for x in abc3 do 
                    averageBy x.Length } 

    // Lower case column names
    type Row4  = { len: float; hght: float }
    let abc4 =                         
        hiveQuery { for x in ctxt.abalone do 
                    newTable "abc4" {len=x.length; hght=x.height } } 

    let elems4 = 
        hiveQuery { for x in abc4 do 
                    select x.len } 
          |> Seq.toArray


    let dat4 = 
        hiveQuery { for x in abc4 do 
                    averageBy x.len } 

    //query { for x in [1..100] do
    //        select (x+x) } 

    hiveQuery { for x in ctxt.abalone do 
                where (x.height > 0.1)
                select x.height }  


    //hiveQuery { for x in ctxt.abalone do select (hiveQuery { for x in ctxt.abalone do sumBy x.height }) }  |> queryString
    //hiveQuery { for x in ctxt.abalone do where (x.height > 0.2);  select (hiveQuery { for x in ctxt.abalone do sumBy x.height }) }  |> queryString

    // b { for x in ctxt.abalone do let a = hiveQuery { for x in ctxt.abalone do sumBy x.height } in select (x.diameter,a) } 

    //hiveQuery { for x in ctxt.abalone do let a = hiveQuery { for x in ctxt.abalone do sumBy x.height } in select (x.diameter,a) } |> queryString
    //hiveQuery { for x in ctxt.abalone do let a = hiveQuery { for x in ctxt.abalone do sumBy x.height } in select a } |> queryString
    //hiveQuery { for x in ctxt.abalone do select (hiveQuery { for x in ctxt.abalone do sumBy x.height }) } |> queryString
    //hiveQuery { for x in ctxt.abalone do select (hiveQuery { for x in ctxt.abalone do sumBy x.height }) } |> run

    hiveQuery { for x in ctxt.abalone do let a = 1.0 in select (x.diameter,a) } |> run
    hiveQuery { for x in ctxt.abalone do let a = 1.0 in let b = 2.0 in select (x.diameter,a, b) } |> run
    hiveQuery { for x in ctxt.abalone do let a = x.diameter in select a } |> run
    hiveQuery { for x in ctxt.abalone do let a = x.diameter in let b = x.height in select (a,b) } |> run
    hiveQuery { for x in ctxt.abalone do let a = x.diameter in let b = x.height + x.shellweight in select (a,b) } |> run
    hiveQuery { for x in ctxt.abalone do let a = 1.0 in let b = 2.0 in select (x.diameter,a,b) } |> run
    hiveQuery { for x in ctxt.abalone do groupBy x.sex into g; select g.Key } |> run
    hiveQuery { for x in ctxt.abalone do groupBy x.sex into g; let a = 1.0 in select (g.Key,a) } |> run
    hiveQuery { for x in ctxt.abalone do groupBy x.sex into g; let a = 1.0 in let b = 2.0 in select (g.Key,a,b) } |> run

    hiveQuery { for x in ctxt.abalone do 
                groupBy x.sex into g
                let a = hiveQuery { for x in g do averageBy x.height } 
                let b = hiveQuery { for x in g do averageBy x.shellweight } 
                select (g.Key,a,b) } 
        |> run

    // Test nested aggregates
    hiveQuery { for x in ctxt.abalone do 
                groupBy x.sex into g
                let a = hiveQuery { for x in g do averageBy (x.height + x.diameter) } 
                let b = hiveQuery { for x in g do averageBy (x.shellweight + x.shuckedweight) } 
                select (g.Key,a,b) } 
        |> queryString
      =  "SELECT *,AVG(height + diameter),AVG(shellweight + shuckedweight) FROM abalone GROUP BY sex"

    // This correctly raises an exception because nested queries can't use 'where' yet
    hiveQuery { for x in ctxt.abalone do 
                groupBy (x.rings % 3) into g
                let a = hiveQuery { for x in g do where (x.length > 0.2); averageBy (x.height + x.diameter) } 
                let b = hiveQuery { for x in g do averageBy (x.shellweight + x.shuckedweight) } 
                select (g.Key,a,b) } 
        |> queryString
      =  "SELECT *,AVG(height + diameter),AVG(shellweight + shuckedweight) FROM abalone GROUP BY rings % 3"

    hiveQuery { for x in ctxt.pokerhand do 
                groupBy x.``class`` into g
                let a = hiveQuery { for x in g do sumBy (x.c1 + x.c2 + x.c3 + x.c4 + x.c5) } 
                select (g.Key,a) } 
        |> queryString
      =  "SELECT *,SUM(c1 + c2 + c3 + c4 + c5) FROM pokerhand GROUP BY class"

    hiveQuery { for x in ctxt.pokerhand do 
                select x }
        |> run

    hiveQuery { for x in ctxt.pokerhand do 
                groupBy x.``class`` into g
                let a = hiveQuery { for x in g do sumBy (x.c1 + x.c2 + x.c3 + x.c4 + x.c5) } 
                select (g.Key,a) } 
        |> run

    
    hiveQuery { for x in ctxt.abalone do select x } |> Seq.length
    hiveQuery { for x in ctxt.abalone do select x.diameter } |> Seq.length
    hiveQuery { for x in ctxt.abc do select x } |> Seq.length
    hiveQuery { for x in ctxt.abc do select x.rings } |> Seq.length
    hiveQuery { for x in ctxt.abc3 do select x } |> Seq.length
    hiveQuery { for x in ctxt.abc3 do select x.petallength } |> Seq.length
    hiveQuery { for x in ctxt.abcd do select x } |> Seq.length
    hiveQuery { for x in ctxt.abcd do select x.length } |> Seq.length
    hiveQuery { for x in ctxt.abcd2 do select x } |> Seq.length
    hiveQuery { for x in ctxt.abcd2 do select x.diameter } |> Seq.length
    hiveQuery { for x in ctxt.bankmarketing do select x } |> Seq.length
    hiveQuery { for x in ctxt.bankmarketing do select x.balance } |> Seq.toList
    hiveQuery { for x in ctxt.bankmarketing do select x.campaign } |> Seq.toList
    hiveQuery { for x in ctxt.bankmarketing do select x.contact } |> Seq.toList
    hiveQuery { for x in ctxt.bankmarketing do select x.day } |> Seq.toList
    hiveQuery { for x in ctxt.bankmarketing do select x.``default`` } |> Seq.toList
    hiveQuery { for x in ctxt.bankmarketing do select x.education } |> Seq.toList
    hiveQuery { for x in ctxt.bankmarketing do select x.housing } |> Seq.toList
    hiveQuery { for x in ctxt.bankmarketing do select x.job } |> Seq.toList
    hiveQuery { for x in ctxt.bankmarketing do select x.loan } |> Seq.toList
    hiveQuery { for x in ctxt.breastcancer do select x } |> Seq.length
    hiveQuery { for x in ctxt.breastcancer do select x.col18 } |> Seq.toList
    hiveQuery { for x in ctxt.carevaluation do select x } |> Seq.length
    hiveQuery { for x in ctxt.carevaluation do select x.persons } |> Seq.length
    hiveQuery { for x in ctxt.iris do select x } |> Seq.length
    hiveQuery { for x in ctxt.iris do select x.sepallength } |> Seq.length
    hiveQuery { for x in ctxt.iris2 do select x } |> Seq.length
    hiveQuery { for x in ctxt.iris2 do select x.sepalwidth } |> Seq.length
    hiveQuery { for x in ctxt.iris_partitioned do select x } |> Seq.length
    hiveQuery { for x in ctxt.iris_partitioned do select x.loadkey2 } |> Seq.length
    hiveQuery { for x in ctxt.page_view do select x } |> Seq.length
    hiveQuery { for x in ctxt.page_view do select x.page_url } |> Seq.length
    hiveQuery { for x in ctxt.pokerhand do select x } |> Seq.length
    hiveQuery { for x in ctxt.pokerhand do select x.c1 } |> Seq.toList
    hiveQuery { for x in ctxt.pokerhand do select x.c2 } |> Seq.toList
    hiveQuery { for x in ctxt.pokerhand do select x.c3 } |> Seq.toList
    hiveQuery { for x in ctxt.pokerhand do select x.c4 } |> Seq.toList
    hiveQuery { for x in ctxt.pokerhand do select x.``class`` } |> Seq.toList
    hiveQuery { for x in ctxt.pokerhand do select x.s1 } |> Seq.toList
    hiveQuery { for x in ctxt.pokerhand do select x.s2 } |> Seq.toList
    hiveQuery { for x in ctxt.pokerhand do select x.s3 } |> Seq.toList
    hiveQuery { for x in ctxt.pokerhand do select x.s4 } |> Seq.toList
    hiveQuery { for x in ctxt.pokerhand do select x.s5 } |> Seq.toList
    hiveQuery { for x in ctxt.sample_07 do select x } |> Seq.length
    hiveQuery { for x in ctxt.sample_07 do select x } |> Seq.length
    hiveQuery { for x in ctxt.sample_08 do select x } |> Seq.length
    hiveQuery { for x in ctxt.sample_08 do select x } |> Seq.length
    hiveQuery { for x in ctxt.winequalitywhite do select x } |> Seq.length
    hiveQuery { for x in ctxt.winequalitywhite do select x.alcohol } |> Seq.length
    hiveQuery { for x in ctxt.winequalitywhite do select x.chlorides } |> Seq.length
    hiveQuery { for x in ctxt.winequalitywhite do select x.citricacid } |> Seq.length
    hiveQuery { for x in ctxt.winequalitywhite do select x.density } |> Seq.length
    hiveQuery { for x in ctxt.winequalitywhite do select x.fixedacidity } |> Seq.length
    hiveQuery { for x in ctxt.winequalitywhite do select x.freesulfurdioxide } |> Seq.length
    hiveQuery { for x in ctxt.winequalitywhite do select x.residualsugar } |> Seq.length
    hiveQuery { for x in ctxt.winequalitywhite do select x.sulphates } |> Seq.length
    hiveQuery { for x in ctxt.winequalitywhite do select x.volatileacidity } |> Seq.length
    hiveQuery { for x in ctxt.winequalitywhite do select x.totalsulfurdioxide } |> Seq.length

    hiveQuery { for x in ctxt.abalone do select x.length } |> Seq.length
    hiveQuery { for x in ctxt.abalone do minBy x.length }  =~= 0.075
    hiveQuery { for x in ctxt.abalone do maxBy x.length } =~= 0.815
    hiveQuery { for x in ctxt.abalone do averageBy x.length } =~= 0.5239920996
    hiveQuery { for x in ctxt.abalone do sumBy x.length }  =~= 2188.715

    hiveQuery { for x in ctxt.abalone do select (id x.length) } |> Seq.length
    hiveQuery { for x in ctxt.abalone do select (id x.length) } |> Seq.sum =~= 2188.715
    hiveQuery { for x in ctxt.abalone do select (id x.length + id x.length) } |> Seq.sum =~= 4377.43
    hiveQuery { for x in ctxt.abalone do select (x.length + x.length) } |> Seq.sum =~= 4377.43
    hiveQuery { for x in ctxt.abalone do select x } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do select x.length } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do select (x.length, x.rings) } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do where (x.length < 0.6); select (x.length, x.rings) } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do where (x.length < 0.6); timeout 1; select (x.length, x.rings) } |> Seq.toList


    let x0 = hiveQuery { for x in ctxt.abalone do select x } 
    let x1 = hiveQuery { for x in ctxt.abalone do count } = 4177L
    hiveQuery { for x in ctxt.abalone do where (x.length < 0.6); count } = 2874L

    hiveQuery { for x in ctxt.abalone do select (id x) } |> Seq.sumBy (fun x -> x.height) =~= 582.76

    // This correctly gives 'error: evaluating whole table as part of client-side expression'
    // because we are using the expression 'x' in a tail operation on the client-side of a non-trivial select
    hiveQuery { for x in ctxt.abalone do where (x.length < 0.6); select (id x) } |> Seq.length
    hiveQuery { for x in ctxt.abalone do where (x.length < 0.6); select (id x, x.length + 1.0) } |> Seq.length

    ctxt.page_view.GetSchema()


    ctxt.DataContext.ExecuteCommand("DESCRIBE abalone")

    let abc2 = 
        hiveQuery { for x in ctxt.abalone do 
                    where (x.length < 0.6); 
                    newTable "abc2" } 


    (hiveQuery { for x in abc do select x }  |> Seq.length) = 4177

    abc.GetSchema()
    
module WithoutUnitsOrRequired = 
    open Microsoft.FSharp.Linq.NullableOperators
    type Data = Samples.Hadoop.HiveTypeProvider<Server=server,Port=port,UserName=user,Password=pwd,UseRequiredAnnotations=false>

    let ctxt = Data.GetDataContext(queryTimeout=100)

    hiveQuery { for x in ctxt.abalone do select x } |> Seq.length = 4177
    hiveQuery { for x in ctxt.abalone do timeout 1; select x } |> Seq.length = 4177
    hiveQuery { for x in ctxt.abalone do select x } |> Seq.toList
    (hiveQuery { for x in ctxt.abalone do minByNullable x.length }).Value =~= 0.075 
    (hiveQuery { for x in ctxt.abalone do timeout 1; minByNullable x.length }).Value =~= 0.075 
    (hiveQuery { for x in ctxt.abalone do maxByNullable x.length }).Value =~= 0.815
    (hiveQuery { for x in ctxt.abalone do averageByNullable x.length }).Value     =~= 0.5239920996
    (hiveQuery { for x in ctxt.abalone do sumByNullable x.length }).Value         =~= 2188.715


    hiveQuery { for x in ctxt.abalone do select (id x.length) } |> Seq.length
    hiveQuery { for x in ctxt.abalone do select x } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do select x.length } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do select (x.length, x.rings) } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do where (x.length ?< 0.6); select (x.length, x.rings) } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do where (x.length ?<= 0.6); select (x.length, x.rings) } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do where (x.length ?> 0.6); select (x.length, x.rings) } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do where (x.length ?= 0.6); select (x.length, x.rings) } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do where (x.length ?<> 0.6); select (x.length, x.rings) } |> Seq.toList

    let x0 = hiveQuery { for x in ctxt.abalone do select x } 
    let x1 = hiveQuery { for x in ctxt.abalone do count } = 4177L
    hiveQuery { for x in ctxt.abalone do where (x.length ?< 0.6); count } = 2874L


    // This correctly gives 'error: evaluating whole table as part of client-side expression'
    // because we are using the expression 'x' in a tail operation on the client-side of a non-trivial select
    hiveQuery { for x in ctxt.abalone do where (x.length ?< 0.6); select (id x) } |> Seq.length


    let abc = hiveQuery { for x in ctxt.abalone do newTable "abcd" } 
    let abc2 = hiveQuery { for x in ctxt.abalone do where (x.length ?< 0.6); newTable "abcd2" } 
    hiveQuery { for x in abc do select x }  |> Seq.length = 4177
    abc.GetSchema()
    




module WithUnits = 
    open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames
    open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

    type Data = Samples.Hadoop.HiveTypeProvider<Server=server,Port=port,UserName=user,Password=pwd,UseUnitAnnotations=true>

    let ctxt = Data.GetDataContext(queryTimeout=100)

    // Copy the 'abalone' table to the 'abc' table
    let abc = 
        hiveQuery { for x in ctxt.abalone do 
                    newTable "abc" } 

    let elems1 = 
        hiveQuery { for x in abc do 
                    where (x.sex = "M")
                    select x.height  } 
          |> Seq.toArray
                    
    let elems2Text = 
        hiveQuery { for x in ctxt.abalone do 
                    select x.height  } 
          |> queryString

    let elems2 = 
        hiveQuery { for x in ctxt.abalone do 
                    select x.height  } 
          |> Seq.toArray

    elems1 = elems2

    // TODO: this does 'select 0.001000 * length, 0.001000 * height (SI units) but the unit annotations are non-SI
    let abc2 =                         
        hiveQuery { for x in ctxt.abalone do 
                    select (x.length,x.height)
                    newTable "abc" } 

    let elems3 = 
        hiveQuery { // Note, you can't do 'for (length,height) in abc2 do 
                    for x in abc2 do 
                    let (length,height) = x
                    select height  } 
          |> Seq.toArray

    elems2 = elems3


    hiveQuery { for x in ctxt.abalone do select x } |> Seq.toList 

    // Check unitized addition in-memory
    hiveQuery { for x in ctxt.abalone do select x } |> Seq.sumBy (fun x -> x.length + x.length) =~= 4.37743<m>
    // Check unitized addition in-query
    hiveQuery { for x in ctxt.abalone do select (x.length + x.length) } |> Seq.sum =~= 4.37743<m>

    // Check unitized multiplication in-memory
    hiveQuery { for x in ctxt.abalone do select x } |> Seq.sumBy (fun x -> x.length * x.length) =~= 0.001207096925<m^2> 

    // Check unitized multiplication in-query
    hiveQuery { for x in ctxt.abalone do select (x.length * x.length) } |> Seq.sum =~= 0.001207096925<m^2>

    hiveQuery { for x in ctxt.abalone do select x } |> Seq.length= 4177

    // Check unitized comparison in-memory
    hiveQuery { for x in ctxt.abalone do select x } |> Seq.filter (fun x -> x.length < 0.0006<m>) |> Seq.length = 2874
    
    // Check unitized comparison in-query
    hiveQuery { for x in ctxt.abalone do where (x.length < 0.0006<m>); count } = 2874L

    hiveQuery { for x in ctxt.abalone do select x } |> Seq.length
    hiveQuery { for x in ctxt.abalone do minBy x.length }  =~= 0.000075<m>
    hiveQuery { for x in ctxt.abalone do maxBy x.length } =~= 0.000815<m>
    hiveQuery { for x in ctxt.abalone do averageBy x.length } =~= 0.0005239920996<m>
    hiveQuery { for x in ctxt.abalone do sumBy x.length }  =~= 2.188715<m>
    // Misc
    hiveQuery { for x in ctxt.abalone do select x.length } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do select x.length } |> Seq.filter (fun x -> x < 0.0006<m>) |> Seq.length
    hiveQuery { for x in ctxt.abalone do select (x.length, x.rings) } |> Seq.toList
    hiveQuery { for x in ctxt.abalone do where (x.length < 0.0006<m>); select (x.length, x.rings) } |> Seq.toList

    let x0 = hiveQuery { for x in ctxt.abalone do select x } 
    let x1 = hiveQuery { for x in ctxt.abalone do count } 
    //let x1 = hiveQuery { for x in ctxt.abalone do sumBy x.length } 
    //let x1 = hiveQuery { for x in ctxt.abalone do select sum(x.length) } 
    let x2 = hiveQuery { for x in ctxt.abalone do where (x.length < 0.0006<m>); count } 

    let abc = hiveQuery { for x in ctxt.abalone do newTable "abcd" } 

    // TODO: the unit of measure metadata annotations are not propagated/written to the new table. This means
    // the unit adjustments will be wrong, and also that when we re-open the script and 
    // access 'ctxt.abc2', the table will be weakly typed, and that the units reported
    // by abc2.GetSchema() are not accurate.
    let abc2 = hiveQuery { for x in ctxt.abalone do where (x.length < 0.0006<m>); newTable "abcd2" } 


    hiveQuery { for x in abc2 do select x.length }  |> Seq.sum = 4177.0<m>
    abc2.GetSchema()
