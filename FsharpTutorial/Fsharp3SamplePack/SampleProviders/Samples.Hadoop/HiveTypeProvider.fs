
namespace Samples.Hadoop.Hive

open System
open System.Reflection
open System.Linq
open System.Linq.Expressions
open System.Collections.Generic

open Samples.FSharp.ProvidedTypes
open Samples.Hadoop.Internals
open Samples.FSharp.FrebaseUnitsOfMeasure

open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Reflection
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

open HiveSchema

[<AutoOpen>]
module internal StaticHelpers = 
    let memoize f = 
        let tab = System.Collections.Generic.Dictionary()
        fun x -> 
            if tab.ContainsKey x then tab.[x] 
            else let a = f x in tab.[x] <- a; a


    let rec computeErasedTypeWithoutNullable(colHiveType) = 
        match colHiveType with 
        | DInt8  -> typeof<sbyte>
        | DInt16  -> typeof<int16>
        | DInt32  -> typeof<int32>
        | DInt64  -> typeof<int64>
        | DSingle _ -> typeof<single>
        | DDouble _ -> typeof<double>
        | DDecimal _ -> typeof<decimal>
        | DBoolean -> typeof<bool>
        | DString -> typeof<string>
        | DAtom -> failwith "should not need to convert atoms"
        | DArray(et) -> computeErasedTypeWithoutNullable(et).MakeArrayType()
        | DMap(dt,rt) -> typedefof<IDictionary<_,_>>.MakeGenericType(computeErasedTypeWithoutNullable dt, computeErasedTypeWithoutNullable rt)
        | DStruct(_cols) -> failwith "nyi: struct"
        | DTable(_args) -> failwith "evaluating whole table as part of client-side expression"

    let computeErasedType(useRequiredAnnotations,colRequired,colHiveType) = 
        let ty = computeErasedTypeWithoutNullable colHiveType
        if (colRequired && useRequiredAnnotations) || not ty.IsValueType then ty
        else typedefof<Nullable<_>>.MakeGenericType(ty)

    let rec colDataOfQuery q = 
        let mkColumn (colName:string, typ, isRequired) = 
            {HiveName=colName
             HiveType= typ
             Description="The column '" + colName + "' in a selected table"
             IsRequired=isRequired}
        match q with 
        | Table(tableName, tableColumns) -> tableName, tableColumns
        | Distinct q2 | Where (q2,_) | Limit (q2,_) | TableSample(q2,_,_) -> colDataOfQuery q2
        | Select (_q2,colExprs) -> 
            // Compute the types of the selected expressions
            let colData = colExprs |> Array.map (fun (colName,colExpr,isRequired) -> mkColumn (colName, HiveSchema.typeOfExpr colExpr, isRequired))
            "$select", colData
        | Count _ -> 
            "$count", [| mkColumn ("count", DInt64, true) |]
        | GroupBy(q2,e) -> 
            let _,groupColData = colDataOfQuery q2
            "$grouping", [| mkColumn ("$key", typeOfExpr e, true); mkColumn ("$group", DTable groupColData, true)  |]
        | AggregateOpBy(_,_,e,isRequired) -> 
            "$result", [| mkColumn ("$value", typeOfExpr e, isRequired) |]

module internal RuntimeHelpers = 
        
    let unexpected exp msg = 
        match msg with 
        | Exception(ex)  -> failwith ex
        | _ -> failwithf "unexpected message, expected %s, got %+A" exp msg
#if NO_PROXY_FOR_HIVE
#else
    let formatParams(proxy,server,port,auth,uid,pwd,timeout) = 
        let auth = if auth = -1 then "None" else string auth
        let uid = if String.IsNullOrWhiteSpace uid then "None" else uid
        let pwd = if String.IsNullOrWhiteSpace pwd then "None" else pwd
#if BROWSER
        sprintf "%s?&Server=%s&Port=%d&Auth=%s&Uid=%s&Pwd=%s&Timeout=%d" proxy server port auth uid pwd timeout
#else
        ignore proxy
        [| "hive"; server; string port; auth; string timeout; uid; pwd |]
        |> String.concat " "
#endif
#endif

/// Represents one row retrieved from the Hive data store
type HiveDataRow internal (dict : System.Collections.Generic.IDictionary<string,obj>) = 
    member __.GetValue<'T> (colName:string) = 
        dict.[colName] :?> 'T

    member __.Item with get (colName : string) = dict.[colName]
    new (_tableName, values : (string * obj * bool) []) = new HiveDataRow (dict [| for (key,obj,_) in values -> (key,obj) |])

    static member internal GetValueMethodInfo = 
        match <@@ fun x -> (Unchecked.defaultof<HiveDataRow>).GetValue(x) @@> with 
        | Lambdas(_,Call(_,mi,_)) -> mi.GetGenericMethodDefinition() 
        | _ -> failwith "unexpected"

    static member internal HiveDataRowCtor = 
        match <@@ fun (x: (string * obj * bool)[]) -> HiveDataRow("",x) @@> with 
        | Lambdas(_,NewObject(ci,_)) -> ci
        | _ -> failwith "unexpected"
    static member internal ItemProp = 
        match <@@ fun (x: HiveDataRow) -> x.[""] @@> with 
        | Lambdas(_,PropertyGet(_,pi,_)) -> pi
        | _ -> failwith "unexpected"
        
     override x.ToString() = "{" + ([ for KeyValue(k,v) in dict -> k+"="+string v ] |> String.concat "; ") + "}"

/// The base type for a connection to the Hive data store.
type HiveDataContext (proxy,server,port,auth,uid,pwd,defaultQueryTimeout,defaultMetadataTimeout,useUnitAnnotations) = 
    let executeRequest (requestData, timeout) =
#if NO_PROXY_FOR_HIVE
        ignore proxy
        Hive.HiveRequestInProcess(server, port, auth, uid, pwd, timeout, requestData)
#else
        let param = RuntimeHelpers.formatParams(proxy,server,port,auth,uid,pwd,timeout)
        let fetcher = Helpers.makeFetcher<HiveRequest,HiveResult> param
        fetcher(requestData)
#endif

    let executeQueryRequest (requestData, queryTimeout) = executeRequest (requestData, defaultArg queryTimeout defaultQueryTimeout)
    let executeMetadataRequest (requestData, metadataTimeout) = executeRequest (requestData, defaultArg metadataTimeout defaultMetadataTimeout)


    let rec getValue v  = 
        match v with
        | VInt8 x -> box x
        | VInt16 x -> box x
        | VInt32 x -> box x
        | VInt64 x -> box x
        | VSingle x -> box x
        | VDouble x -> box x
        | VDecimal x -> box x
        | VBoolean x -> box x
        | VString x -> box x
        | VArray x -> box (Array.map getValue x)
        | VStruct _x -> failwith "nyi: struct"
        | VMap x -> dict (x |> Array.map (fun (x,y) -> getValue x, getValue y)) |> box


    member __.Server = server
    member __.Port = port
    member __.UserName = uid
    member internal __.UseUnitAnnotations = useUnitAnnotations

    /// Dynamically read the table names from the Hive data store
    member __.GetTableNames () =
            match executeMetadataRequest (GetTableNames, None) with 
            | TableNames(tableNames) -> tableNames
            | data -> RuntimeHelpers.unexpected "GetTableNames" data

    /// Dynamically read the table names from the Hive data store
    member __.GetTablePartitionNames (tableName) =
            match executeMetadataRequest (GetTablePartitionNames(tableName), None) with 
            | TablePartitionNames(partitionNames) -> partitionNames
            | data -> RuntimeHelpers.unexpected "GetPartitionNames" data

    /// Dynamically read the table names from the Hive data store
    member __.GetTableSchema (tableName) =
        match executeMetadataRequest (GetTableSchema(tableName, useUnitAnnotations), None) with 
        | TableSchema(schema) -> schema
        | data -> RuntimeHelpers.unexpected "GetTableSchema" data

    /// Dynamically read the table description from the Hive data store.
    member __.GetTableDescription (tableName) = 
            match executeMetadataRequest (GetTableDescription(tableName, useUnitAnnotations),None) with
            | TableDescription(tableDesc,_tableHead) -> tableDesc
            | data -> RuntimeHelpers.unexpected "TableDescription" data

    member x.GetTable<'T> (tableName) = HiveTable<'T>(x, tableName)

    member x.GetTableUntyped (tableName) = HiveTable<HiveDataRow>(x, tableName)

    member x.ExecuteCommand (command, ?timeout) = 
        executeQueryRequest (ExecuteCommand(command), timeout) |> function 
            | CommandResult(text) -> text
            | data -> RuntimeHelpers.unexpected "Commmand" data

    /// Execute a single query value
    member internal x.ExecuteQueryValue (queryData, ?timeout) = 
        let _tableName, colData = colDataOfQuery queryData 
        let rows = 
            executeQueryRequest (GetDataFrame(queryData, colData), timeout) |> function 
                | DataFrame(rows) -> rows
                | data -> RuntimeHelpers.unexpected "GetDataFrame" data

        getValue rows.[0].[0] 
        
    /// Dynamically read the rows of a table from the Hive data store.
    member internal __.ExecuteQuery (queryData, rowType, ?timeout) : seq<obj> = 
        seq { 
            let _tableName, colData = colDataOfQuery queryData 
            let rows = 
                executeQueryRequest (GetDataFrame(queryData, colData), timeout) |> function 
                    | DataFrame(rows) -> rows
                    | data -> RuntimeHelpers.unexpected "GetDataFrame" data

            // If making F# record values, precompute a record constructor
            let maker = 
                if rowType = typeof<HiveDataRow> then 
                   None
                elif FSharpType.IsRecord rowType then 
                    // We don't need to permute the record fields here because F# quotations are already permuted.
                   Some (FSharpValue.PreComputeRecordConstructor(rowType,BindingFlags.NonPublic ||| BindingFlags.Public))
                elif FSharpType.IsUnion rowType && FSharpType.GetUnionCases(rowType).Length=1  then 
                   Some (FSharpValue.PreComputeUnionConstructor(FSharpType.GetUnionCases(rowType).[0],BindingFlags.NonPublic ||| BindingFlags.Public))
                elif FSharpType.IsTuple rowType then 
                   Some (FSharpValue.PreComputeTupleConstructor(rowType))
                else 
                   None //failwith (sprintf "unrecognized row type '%A'" rowType)

            for row in rows do
                let values = (colData, row) ||> Array.map2 (fun colData v -> (colData.HiveName,getValue v) )
                match maker with 
                | None -> yield HiveDataRow (dict values) |> box 
                | Some f -> yield f (Array.map snd values)
                    
        }

/// Represents a query of the Hive data store
and internal IHiveQueryable = 
    abstract DataContext : HiveDataContext
    abstract QueryData : HiveQueryData
    abstract TailOps : Expr list
    abstract Timeout : int<s> option

and internal IHiveTable = 
    abstract TableName : string 

and HiveTable<'T> internal (dataCtxt: HiveDataContext, tableName: string, colData: HiveColumnSchema[]) =
    // Performance: it would be nice not to have to make this metadata request when creating the table
    inherit HiveQueryable<'T>(dataCtxt, Table(tableName, colData), [], None)
    new (dataCtxt: HiveDataContext, tableName: string) = 
        let colData = dataCtxt.GetTableSchema(tableName).Columns
        new HiveTable<'T>(dataCtxt, tableName, colData)
    member x.GetPartitionNames() = dataCtxt.GetTablePartitionNames(tableName)
    member x.GetSchema() = dataCtxt.GetTableSchema(tableName)
    interface IHiveTable with 
        member x.TableName = tableName

and HiveGrouping<'Key,'T> internal (key: 'Key, dataCtxt: HiveDataContext, queryData: HiveQueryData, tailOps: Expr list, timeout: int<s> option) = 
    inherit HiveQueryable<'T>(dataCtxt, queryData, tailOps, timeout)
    member x.Key = key

and HiveQueryable<'T> internal (dataCtxt: HiveDataContext, queryData: HiveQueryData, tailOps: Expr list, timeout: int<s> option) = 

    member x.Run(?timeout) = 
        let rowType = 
            match tailOps with 
            | [] -> typeof<'T>
            | tailOp::_ -> 
                assert (tailOp.Type.IsGenericType)  // assert the tailOp is a function
                assert (tailOp.Type.GetGenericTypeDefinition() = typedefof<int -> int>) // assert the tailOp is a function
                let funTy = tailOp.Type.GetGenericArguments()
                let domTy,_ranTy = funTy.[0], funTy.[1]
                domTy
        let objs = dataCtxt.ExecuteQuery(queryData, rowType, ?timeout=timeout)
        (objs,tailOps) 
          ||> List.fold (fun objsInt tailOp ->
            // Compile the tailOp
            assert (tailOp.Type.IsGenericType)  // assert the tailOp is a function
            assert (tailOp.Type.GetGenericTypeDefinition() = typedefof<int -> int>) // assert the tailOp is a function
            let funTy = tailOp.Type.GetGenericArguments()
            let domTy,ranTy = funTy.[0], funTy.[1]
            let delTy = typedefof<Converter<_,_>>.MakeGenericType [| domTy; ranTy |]
            let delVar = Var("x",domTy)
            let tailOpDel = Expr.NewDelegate(delTy,[delVar],Expr.Application(tailOp,Expr.Var(delVar)))
            let tailOpExpression = Linq.RuntimeHelpers.LeafExpressionConverter.QuotationToExpression tailOpDel :?> LambdaExpression
            let del = tailOpExpression.Compile()
            objsInt |> Seq.map (fun obj -> del.DynamicInvoke [| obj |]))
          |> Seq.cast<'T>
          |> Seq.toArray

    interface IHiveQueryable with 
        member x.QueryData = queryData
        member x.DataContext = dataCtxt
        member x.TailOps = tailOps
        member x.Timeout = timeout

    interface seq<'T> with 
          member x.GetEnumerator() = x.Run() :> seq<_> |> fun s -> s.GetEnumerator()
    interface System.Collections.IEnumerable with 
          member x.GetEnumerator() = x.Run():> System.Collections.IEnumerable |> fun s -> s.GetEnumerator() 

    override __.ToString() = sprintf "<Hive Query '%s'>" (HiveSchema.formatQuery HiveQueryUnitSystem.SI queryData)
    member x.GetQueryString() = HiveSchema.formatQuery HiveQueryUnitSystem.SI queryData
    member x.SumBy(keySelector : 'T -> 'TKey) : 'TKey = ignore keySelector; failwith "the SumBy operator can only be executed on the server"
(*
    // TODO: remote implementation of IQueryable
    interface IQueryable<'T> 
    interface IQueryable with 
          member x.Provider = (Seq.readonly x).AsQueryable().Provider
          member x.Expression =  (Seq.readonly x).AsQueryable().Expression
          member x.ElementType = (Seq.readonly x).AsQueryable().ElementType
*)

[<AutoOpen>]
module HiveQuery = 
    let run (hq: HiveQueryable<'T>) = hq.Run()
    let queryString (hq: HiveQueryable<'T>) = hq.GetQueryString()

[<AutoOpen>]
module internal QuotationUtils = 

    let (|Getter|_|) (prop: PropertyInfo) =
        match prop.GetGetMethod(true) with 
        | null -> None
        | v -> Some v

    let (|MacroReduction|_|) (p : Expr) = 
        match p with 
        | Applications(Lambdas(vs, body), args) when vs.Length = args.Length && List.forall2 (fun vs args -> List.length vs = List.length args) vs args -> 
            let tab = Map.ofSeq (List.concat (List.map2 List.zip vs args))
            let body = body.Substitute tab.TryFind 
            Some body

        // Macro
        | PropertyGet(None, Getter(MethodWithReflectedDefinition(body)), []) -> 
            Some body

        // Macro
        | Call(None, MethodWithReflectedDefinition(Lambdas(vs, body)), args) -> 
            let tab = Map.ofSeq (List.concat (List.map2 (fun (vs:Var list) arg -> match vs, arg with [v], arg -> [(v, arg)] | vs, NewTuple(args) -> List.zip vs args | _ -> List.zip vs [arg]) vs args))
            let body = body.Substitute tab.TryFind 
            Some body

        // Macro - eliminate 'let'. 
        //
        // Always eliminate these:
        //    - function definitions 
        //
        // Always eliminate these, which are representations introduced by F# quotations:
        //    - let v1 = v2
        //    - let v1 = tupledArg.Item*
        //    - let copyOfStruct = ...

        | Let(v, e, body) when (match e with 
                                | Lambda _ -> true
                                | Var _ -> true 
                                | TupleGet(Var tv, _) when tv.Name = "tupledArg" -> true 
                                | _ when v.Name = "copyOfStruct" && v.Type.IsValueType -> true
                                | _ -> false) ->
            let body = body.Substitute (fun v2 -> if v = v2 then Some e else None)
            Some body
        
        | _ -> None

    let rec existsSubQuotation f q = 
        f q || 
        match q with 
        | Quotations.ExprShape.ShapeCombination(_op,xs) -> xs |> List.exists (existsSubQuotation f)
        | Quotations.ExprShape.ShapeLambda(_v,b) -> existsSubQuotation f b
        | Quotations.ExprShape.ShapeVar _ -> false

    let (|NestedQuery|_|) q = 
        match q with 
        | Application(Lambda(_builderVar,Call(_builderVar2,mi,[Quote(queryQuotation)])),_builderExpr) when mi.Name = "Run" ->  Some(queryQuotation)
        | _ -> None
    
    let isNullableTy (ty:Type) = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Nullable<_>>
    /// A column in an intermediate table gets labelled as 'required' if it is a value type
    /// and the F# type for the selected expression is not nullable.
    let isRequiredTy (ty:Type) = ty.IsValueType && not (isNullableTy ty)

type OptionBuilder() = 
    member inline __.Bind(x,f) = match x with None -> None | Some v -> f v
    member inline __.Return x = Some x
    member inline __.ReturnFrom x = x

type internal HiveExpressionTranslator(decodeNestedQuery, dataCtxt:HiveDataContext, tableName, tableVar:Var, colData, auxVars, auxExprs, allowEvaluation: bool) = 
    let option = OptionBuilder()
    let (|Member|_|) (s:string) (mi:MemberInfo) = if mi.Name = s then Some() else None
    let (|StringMember|_|) s mi = match mi with Member s when (mi.DeclaringType.FullName = "System.String") -> Some() | _ -> None
    let (|DateTimeMember|_|) s mi = match mi with Member s when (mi.DeclaringType.FullName = "System.DateTime") -> Some() | _ -> None
    let (|DecimalMember|_|) s mi = match mi with Member s when (mi.DeclaringType.FullName = "System.Decimal") -> Some() | _ -> None
    let (|MathMember|_|) s mi = match mi with Member s when (mi.DeclaringType.FullName = "System.Math") -> Some() | _ -> None
    let (|OperatorsMember|_|) s mi = match mi with Member s when (mi.DeclaringType.FullName = "Microsoft.FSharp.Core.Operators") -> Some() | _ -> None
    let (|NullableOperatorsMember|_|) s mi = match mi with Member s when (mi.DeclaringType.FullName = "Microsoft.FSharp.Linq.NullableOperators") -> Some() | _ -> None
    let (|DecimalMathOrOperatorsMember|_|) s mi = 
        match mi with 
        | DecimalMember s -> Some()
        | OperatorsMember s -> Some()
        | MathMember s -> Some()
        |  _ -> None

    let EAtom s = EFunc(s,[], DAtom)
    let EString s = EVal (VString s, DString)
    let EInt32 s = EVal (VInt32 (Value s), DInt32)

    let rec unifyUnits ty1 ty2 = 
        match ty1, ty2 with 
        | AnyUnit.Unknown rv, AnyUnit.Unknown ru when Object.ReferenceEquals (rv,ru) -> ()
        | AnyUnit.Unknown ({contents = None} as rv), _ -> rv := Some ty2
        | _, AnyUnit.Unknown ({contents = None} as rv) -> rv := Some ty1
        | AnyUnit.Unknown {contents = Some tyv}, _ -> unifyUnits tyv ty2
        | _, AnyUnit.Unknown {contents = Some tyv} -> unifyUnits ty1 tyv
        | _, AnyUnit.Unknown ({contents = None} as rv) -> rv := Some ty1
        // TODO: this is approximate and doesn't implement fully normalizing unit inference/unification
        | AnyUnit.Prod (u1,u2), AnyUnit.Prod (v1,v2) -> unifyUnits u1 v1; unifyUnits u2 v2
        | AnyUnit.Inv u, AnyUnit.Inv v -> unifyUnits u v
        | _ -> ()
    let rec unifyTypes ty1 ty2 = 
        match ty1,ty2 with 
        | DSingle u1, DSingle u2 | DDouble u1, DDouble u2 | DDecimal u1, DDecimal u2 -> unifyUnits u1 u2
        | DMap (dty1, rty1), DMap (dty2, rty2) -> unifyTypes dty1 dty2; unifyTypes rty1 rty2
        | DArray ety1, DArray ety2 -> unifyTypes ety1 ety2
        | DStruct s1, DStruct s2 -> if s1.Length = s2.Length then (s1,s2) ||> Array.iter2 (fun (_,x1) (_,x2) -> unifyTypes x1 x2)
        | _ -> ()
        
    let equateTypes e1 e2  = let ty1 = typeOfExpr e1 in let ty2 = typeOfExpr e2 in unifyTypes ty1 ty2; ty1 
    let (+.) e1 e2  = EBinOp(e1,"+",e2, equateTypes e1 e2)
    let ( *. ) e1 e2  = EBinOp(e1,"*",e2, typeOfExpr e1 * typeOfExpr e2)
    let ( %. ) e1 e2  = EBinOp(e1,"%",e2, equateTypes e1 e2)
    let (/.) e1 e2  = EBinOp(e1,"/",e2, typeOfExpr e1 / typeOfExpr e2)
    let (-.) e1 e2  = EBinOp(e1,"-",e2, equateTypes e1 e2)
    let RelOp(e1,op,e2) = unifyTypes (typeOfExpr e1) (typeOfExpr e2); EBinOp(e1,op,e2,DBoolean)
    let concat es = EFunc("CONCAT", es, DString)
    let colTableByName = dict [ for col in colData  -> col.HiveName, col.HiveType ]
    do assert (List.length auxVars = List.length auxExprs)
    let auxMap = dict (List.zip auxVars auxExprs)


    //let evalTable expr = 
    //    let obj = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation expr
    //    match obj with 
    //    | :? IHiveTable as t -> EString t.TableName
    //    | _ -> failwithf "The following expression evaluated to a '%s' instead of a HiveTable: %A" (obj.GetType().ToString()) expr
    let rec tryConvExpr e = 
      option {
        match e with 

        // 'select x' where 'x' is a whole table
        | Var v2 when tableVar = v2 -> return ETable(tableName, colData)

        // 'x' where 'x' is an auxiliary let-bound or other bound variable
        | Var v2 when auxMap.ContainsKey v2 -> return! tryConvExpr auxMap.[v2]

        // Property access when an F# record type is used as the intermediate schema
        | PropertyGet(Some (Var v2), propInfo, []) when tableVar = v2 && Reflection.FSharpType.IsRecord propInfo.DeclaringType && colTableByName.ContainsKey propInfo.Name -> 
            let propName = propInfo.Name
            let hiveType = colTableByName.[propName]
            return EColumn (propName, hiveType) 

        // Property access when an F# record type is used as the intermediate schema
        | TupleGet(Var v2, n) when tableVar = v2 && (colTableByName.ContainsKey ("Item" + string (n+1))) -> 
            let propName = "Item" + string (n+1)
            let hiveType = colTableByName.[propName]
            return EColumn (propName, hiveType) 

        // Property access on an erased type
        | Call(Some (Var v2), Member "GetValue", [String hiveName]) when tableVar = v2 && colTableByName.ContainsKey hiveName -> 
            let hiveType = colTableByName.[hiveName]
            return EColumn (hiveName, hiveType) 

        // Property access g.Key for a group
        | PropertyGet(Some (Var v2), propInfo, []) when tableVar = v2 && propInfo.Name = "Key" && propInfo.DeclaringType.Name = "HiveGrouping`2" -> 
            return EColumn ("*", colTableByName.["$key"]) // SELECT * FROM table GROUP BY keyExpr;  <-- this selects the key 

        // Detect a nested query. The only nested queries we allow are iterating a group 
        // which results from 'GroupBy'
        | NestedQuery(queryQuotation) when colTableByName.ContainsKey "$group" -> 

            let (_hqCtxt:HiveDataContext), hqQueryData, _hqTail, _hqTimeout, _hqAuxExprsF  = 
                let groupColData = 
                    match colTableByName.["$group"] with 
                    | DTable cols -> cols
                    | _ -> failwith "unexpected group type"

                decodeNestedQuery (Some (tableVar,dataCtxt,tableName,groupColData)) false queryQuotation
            // TODO: check the query context and table are the same
            // TODO: check there is no timeout
            return EQuery(hqQueryData)

        | Int32 d  -> return EVal (VInt32 (Value d), DInt32) 
        | Int64 d  -> return EVal (VInt64 (Value d), DInt64) 
        | Single d -> return EVal (VSingle (Value d), DSingle (AnyUnit.NewVar(dataCtxt.UseUnitAnnotations))) 
        | Double d -> return EVal (VDouble (Value d), DDouble (AnyUnit.NewVar(dataCtxt.UseUnitAnnotations))) 
        | String null -> return EAtom "null"
        | String d -> return EString d 
        | Bool d   -> return EVal (VBoolean (Value d), DBoolean) 
        // Detect constant decimal creations, which get encoded in F#
        | Call(None,mi,[Int32 d1; Int32 d2; Int32 d3; Bool d4; Byte d5]) when mi.Name = "MakeDecimal" -> 
             return EVal (VDecimal (Value (Decimal(d1,d2,d3,d4,d5))), DDecimal (AnyUnit.NewVar(dataCtxt.UseUnitAnnotations))) 

        // Nullable() --> null
        | DefaultValue _ -> return EAtom("null")
        // Nullable(x) --> x
        | NewObject(cinfo,[arg]) when isNullableTy cinfo.DeclaringType -> return! tryConvExpr arg
        // x.Value --> x
        | PropertyGet(Some obj,pinfo,_) when pinfo.Name = "Value" && isNullableTy pinfo.DeclaringType -> return! tryConvExpr obj
        // TODO: .GetValueOrDefault()        

(*
        // TODO: these are LINQ methods. Is this the way we want these surfaced?
        // TODO: these are not tested and won't work because we must extract the name of the table from 'obj'
        | Call(None,Member "LongCount",[obj]) -> return! unop obj (fun objH -> EFunc("COUNT_BIG",[objH], DInt64))
        | Call(None,Member "Sum"      ,[obj]) -> return! unop obj (fun objH -> EFunc("SUM",[objH], typeOfExpr objH))
        | Call(None,Member "Average"  ,[obj]) -> return! unop obj (fun objH -> EFunc("AVG",[objH], typeOfExpr objH))
        | Call(None,Member "Max"      ,[obj]) -> return! unop obj (fun objH -> EFunc("MAX",[objH], typeOfExpr objH))
        | Call(None,Member "Min"      ,[obj]) -> return! unop obj (fun objH -> EFunc("MIN",[objH], typeOfExpr objH))
        | Call(None,Member "Count"    ,[obj]) -> return! unop obj (fun objH -> EFunc("LEN",[objH], DInt32))
*)
(*

bigint  count(*), count(expr), count(DISTINCT expr[, expr_.])  count(*) - Returns the total number of retrieved rows, including rows containing NULL values; count(expr) - Returns the number of rows for which the supplied expression is non-NULL; count(DISTINCT expr[, expr]) - Returns the number of rows for which the supplied expression(s) are unique and non-NULL. 
double  sum(col), sum(DISTINCT col)  Returns the sum of the elements in the group or the sum of the distinct values of the column in the group 
double  avg(col), avg(DISTINCT col)  Returns the average of the elements in the group or the average of the distinct values of the column in the group 
double  min(col)  Returns the minimum of the column in the group 
double  max(col)  Returns the maximum value of the column in the group 
double  variance(col), var_pop(col)  Returns the variance of a numeric column in the group 
double  var_samp(col) Returns the unbiased sample variance of a numeric column in the group 
double  stddev_pop(col)  Returns the standard deviation of a numeric column in the group 
double  stddev_samp(col)  Returns the unbiased sample standard deviation of a numeric column in the group 
double  covar_pop(col1, col2)  Returns the population covariance of a pair of numeric columns in the group 
double covar_samp(col1, col2) Returns the sample covariance of a pair of a numeric columns in the group 
double  corr(col1, col2) Returns the Pearson coefficient of correlation of a pair of a numeric columns in the group 
double percentile(BIGINT col, p) Returns the exact pth percentile of a column in the group (does not work with floating point types). p must be between 0 and 1. NOTE: A true percentile can only be computed for integer values. Use PERCENTILE_APPROX if your input is non-integral. 
array<double> percentile(BIGINT col, array(p1 [, p2]...)) Returns the exact percentiles p1, p2, ... of a column in the group (does not work with floating point types). pi must be between 0 and 1. NOTE: A true percentile can only be computed for integer values. Use PERCENTILE_APPROX if your input is non-integral. 
double percentile_approx(DOUBLE col, p [, B]) Returns an approximate pth percentile of a numeric column (including floating point types) in the group. The B parameter controls approximation accuracy at the cost of memory. Higher values yield better approximations, and the default is 10,000. When the number of distinct values in col is smaller than B, this gives an exact percentile value. 
array<double> percentile_approx(DOUBLE col, array(p1 [, p2]...) [, B]) Same as above, but accepts and returns an array of percentile values instead of a single one. 
array<struct {'x','y'}> histogram_numeric(col, b) Computes a histogram of a numeric column in the group using b non-uniformly spaced bins. The output is an array of size b of double-valued (x,y) coordinates that represent the bin centers and heights 
array collect_set(col) Returns a set of objects with duplicate elements eliminated 

Rouch sketch:
module Hive = 
    min: 'T -> 'T 
    minNullable: Nullable<'T> -> Nullable<'T>
    variance: 'T -> 'U when (*) : 'T * 'T -> 'U
    varianceNullable: 'T -> 'U when (*) : 'T * 'T -> 'U
    sqrt: float<u^s> -> float<u>
    sqrtNullable: float<u^s> -> float<u>
    ...




*)

        | PropertyGet(Some obj,StringMember "Length",[])         -> return! unop obj (fun objH -> EFunc("LENGTH",[objH], DInt32))
        | PropertyGet(Some obj,DateTimeMember "Day",[])         -> return! unop obj (fun objH -> EFunc("DAY",[objH], DInt32))
        | PropertyGet(Some obj,DateTimeMember "Month",[])       -> return! unop obj (fun objH -> EFunc("MONTH",[objH], DInt32))
        | PropertyGet(Some obj,DateTimeMember "Year",[])        -> return! unop obj (fun objH -> EFunc("YEAR",[objH], DInt32))
        | PropertyGet(Some obj,DateTimeMember "Hour",[])        -> return! unop obj (fun objH -> EFunc("DATEPART",[EAtom "hour";objH], DInt32))
        | PropertyGet(Some obj,DateTimeMember "Minute",[])      -> return! unop obj (fun objH -> EFunc("DATEPART",[EAtom "minute";objH], DInt32))
        | PropertyGet(Some obj,DateTimeMember "Second",[])      -> return! unop obj (fun objH -> EFunc("DATEPART",[EAtom "second";objH], DInt32))
        | PropertyGet(Some obj,DateTimeMember "Millisecond",[]) -> return! unop obj (fun objH -> EFunc("DATEPART",[EAtom "millisecond";objH], DInt32))
        | PropertyGet(Some obj,DateTimeMember "DayOfWeek",[])   -> return! unop obj (fun objH -> EFunc("DATEPART",[EAtom "weekday";objH], DInt32))
        | PropertyGet(Some obj,DateTimeMember "DayOfYear",[])   -> return! unop obj (fun objH -> EFunc("DATEPART",[EAtom "dayofyear";objH], DInt32))
        // TODO: what if user's input contains '%'
        | Call(Some obj,StringMember "StartsWith",[arg1])       -> return! binop obj arg1 (fun objH arg1H -> RelOp(objH,"LIKE", concat [arg1H; EString "%"]))
        | Call(Some obj,StringMember "EndsWith"  ,[arg1])       -> return! binop obj arg1 (fun objH arg1H -> RelOp(objH,"LIKE", concat [EString "%"; arg1H]))
        | Call(Some obj,StringMember "Contains"  ,[arg1])       -> return! binop obj arg1 (fun objH arg1H -> RelOp(objH,"LIKE",concat [EString "%"; arg1H; EString "%"]) )
        | Call(None,StringMember "IsNullOrEmpty" ,[obj])        -> return! unop obj (fun objH -> RelOp(RelOp(objH,"IS",EAtom("NULL")),"OR",RelOp(objH,"=",EString "")))
        | Call(Some obj,StringMember "ToUpper"   ,[])           -> return! unop obj (fun objH -> EFunc("UPPER",[objH], DString))
        | Call(Some obj,StringMember "ToLower"   ,[])           -> return! unop obj (fun objH -> EFunc("LOWER",[objH], DString))
        //    TODO: obj.Concat(foo1,...,fooN) -.> foo1 + ... + fooN
        | Call(Some obj,StringMember "Replace"   ,[arg1; arg2]) -> return! triop obj arg1 arg2 (fun objH arg1H arg2H -> EFunc("REPLACE",[objH;arg1H;arg2H], DString))
        | Call(Some obj,StringMember "Substring" ,[arg1])       -> return! binop obj arg1 (fun objH arg1H -> EFunc("SUBSTRING",[objH;arg1H +. EInt32 1;EInt32 8000], DString))
        | Call(Some obj,StringMember "Substring" ,[arg1;arg2])  -> return! triop obj arg1 arg2 (fun objH arg1H arg2H -> EFunc("SUBSTRING",[objH;arg1H +. EInt32 1;arg2H], DString))
        | Call(Some obj,StringMember "Remove"    ,[arg1])       -> return! binop obj arg1 (fun objH arg1H -> EFunc("STUFF",[objH;arg1H +. EInt32 1;EInt32 8000], DString))
        | Call(Some obj,StringMember "Remove"    ,[arg1;arg2])  -> return! triop obj arg1 arg2 (fun objH arg1H arg2H -> EFunc("STUFF",[objH;arg1H +. EInt32 1;arg2H], DString))
        | Call(Some obj,StringMember "IndexOf"   ,[arg1])       -> return! binop obj arg1 (fun objH arg1H -> EFunc("CHARINDEX",[arg1H;objH], DInt32) -. EInt32 1)
        | Call(Some obj,StringMember "IndexOf"   ,[arg1; arg2]) -> return! triop obj arg1 arg2 (fun objH arg1H arg2H -> EFunc("CHARINDEX",[arg1H;objH;arg2H +. EInt32 1], DInt32) -. EInt32 1)
        | Call(Some obj,StringMember "Trim"      ,[])           -> return! unop obj (fun objH -> EFunc("RTRIM",[EFunc("LTRIM",[objH],DString)],DString))
        | Call(Some obj,StringMember "TrimEnd"   ,[])           -> return! unop obj (fun objH -> EFunc("RTRIM",[objH],DString))
        | Call(Some obj,StringMember "TrimStart" ,[])           -> return! unop obj (fun objH -> EFunc("LTRIM",[objH],DString))
        | Call(None,DateTimeMember "op_Subtraction",[arg1; arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> EFunc("DATEDIFF",[arg1H;arg2H], DDate))
        | Call(Some obj,DateTimeMember "AddYears"       ,[arg1]) -> return! binop obj arg1 (fun objH arg1H -> EFunc("DATEADD",[EAtom "YYYY";objH;arg1H], DDate))
        | Call(Some obj,DateTimeMember "AddMonths"      ,[arg1]) -> return! binop obj arg1 (fun objH arg1H -> EFunc("DATEADD",[EAtom "MM";objH;arg1H], DDate))
        | Call(Some obj,DateTimeMember "AddDays"        ,[arg1]) -> return! binop obj arg1 (fun objH arg1H -> EFunc("DATEADD",[EAtom "DD";objH;arg1H], DDate))
        | Call(Some obj,DateTimeMember "AddHours"       ,[arg1]) -> return! binop obj arg1 (fun objH arg1H -> EFunc("DATEADD",[EAtom "HH";objH;arg1H], DDate))
        | Call(Some obj,DateTimeMember "AddMinutes"     ,[arg1]) -> return! binop obj arg1 (fun objH arg1H -> EFunc("DATEADD",[EAtom "MI";objH;arg1H], DDate))
        | Call(Some obj,DateTimeMember "AddSeconds"     ,[arg1]) -> return! binop obj arg1 (fun objH arg1H -> EFunc("DATEADD",[EAtom "SS";objH;arg1H], DDate))
        | Call(Some obj,DateTimeMember "AddMilliseconds",[arg1]) -> return! binop obj arg1 (fun objH arg1H -> EFunc("DATEADD",[EAtom "MS";objH;arg1H], DDate))

        | Call(None,Member "op_Addition"     ,[arg1; arg2]) -> return! binop arg1 arg2 (+.)
        | Call(None,Member "op_Subtraction"  ,[arg1; arg2]) -> return! binop arg1 arg2 (-.)
        | Call(None,Member "op_Multiply"     ,[arg1; arg2]) -> return! binop arg1 arg2 ( *. )
        | Call(None,Member "op_Division"     ,[arg1; arg2]) -> return! binop arg1 arg2 (/.)
        | Call(None,Member "op_Modulus"      ,[arg1; arg2]) -> return! binop arg1 arg2 (%.) 
        | Call(None,Member "op_UnaryNegation",[arg1]) -> return! unop arg1 (fun arg1H -> EFunc("-",[arg1H],typeOfExpr arg1H))
        | Call(None,Member "op_Exponentiation",[arg1; arg2])-> return! binop arg1 arg2 (fun arg1H arg2H -> EFunc("POWER",[arg1H; arg2H], typeOfExpr arg1H)) 

        | Call(None,DecimalMember "Add"      ,[arg1; arg2]) -> return! binop arg1 arg2 (+.)
        | Call(None,DecimalMember "Subtract" ,[arg1; arg2]) -> return! binop arg1 arg2 (-.)
        | Call(None,DecimalMember "Multiply" ,[arg1; arg2]) -> return! binop arg1 arg2 ( *. )
        | Call(None,DecimalMember "Divide"   ,[arg1; arg2]) -> return! binop arg1 arg2 (/.) 
        | Call(None,DecimalMember "Remainder",[arg1; arg2]) -> return! binop arg1 arg2 (%.)
        | Call(None,DecimalMember "Negate"   ,[arg1])       -> return! unop arg1 (fun arg1H -> EFunc("-",[arg1H],typeOfExpr arg1H))

        // TODO: use the correct translation of all conversions. For now assuming the identity function.
        
        | Call(None,OperatorsMember "ToDouble",[arg1])  -> return! unop arg1 (fun arg1H -> EFunc("$id",[arg1H], DDouble AnyUnit.One))
        | Call(None,OperatorsMember "ToDecimal",[arg1])  -> return! unop arg1 (fun arg1H -> EFunc("$id",[arg1H], DDecimal AnyUnit.One))
        | Call(None,OperatorsMember "ToSingle",[arg1])  -> return! unop arg1 (fun arg1H -> EFunc("$id",[arg1H], DSingle AnyUnit.One))
        | Call(None,OperatorsMember "ToSByte",[arg1])  -> return! unop arg1 (fun arg1H -> EFunc("$id",[arg1H], DInt8))
        | Call(None,OperatorsMember "ToInt16",[arg1])  -> return! unop arg1 (fun arg1H -> EFunc("$id",[arg1H], DInt16))
        | Call(None,OperatorsMember "ToInt32",[arg1])  -> return! unop arg1 (fun arg1H -> EFunc("$id",[arg1H], DInt32))
        | Call(None,OperatorsMember "ToInt64",[arg1])  -> return! unop arg1 (fun arg1H -> EFunc("$id",[arg1H], DInt64))

        | Call(None,DecimalMathOrOperatorsMember "Ceiling",[arg1])  -> return! unop arg1 (fun arg1H -> EFunc("CEILING",[arg1H],typeOfExpr arg1H))
        | Call(None,DecimalMathOrOperatorsMember "Floor"  ,[arg1])  -> return! unop arg1 (fun arg1H -> EFunc("FLOOR",[arg1H], typeOfExpr arg1H))
        | Call(None,DecimalMathOrOperatorsMember "Round"  ,[arg1])  -> return! unop arg1 (fun arg1H -> EFunc("ROUND",[arg1H], typeOfExpr arg1H))
        | Call(None,DecimalMathOrOperatorsMember "Round"  ,[arg1; arg2]) when arg2.Type = typeof<int> -> 
                                                                       return! binop arg1 arg2 (fun arg1H arg2H -> EFunc("ROUND",[arg1H; arg2H], typeOfExpr arg1H))
        | Call(None,DecimalMathOrOperatorsMember "Truncate",[arg1]) -> return! unop arg1 (fun arg1H -> EFunc("ROUND",[arg1H; EInt32 0; EInt32 1], typeOfExpr arg1H) )
        // TODO: correct units for these operations, especially 'sqrt' and friends
        | Call(None,((DecimalMathOrOperatorsMember "Abs"
                     |DecimalMathOrOperatorsMember "Acos"
                     |DecimalMathOrOperatorsMember "Asin"
                     |DecimalMathOrOperatorsMember "Atan"
                     |DecimalMathOrOperatorsMember "Cos"
                     |DecimalMathOrOperatorsMember "Exp"
                     |DecimalMathOrOperatorsMember "Log10"
                     |DecimalMathOrOperatorsMember "Sin"
                     |DecimalMathOrOperatorsMember "Tan"
                     |DecimalMathOrOperatorsMember "Sqrt"
                     |DecimalMathOrOperatorsMember "Sign"
                     |DecimalMathOrOperatorsMember "Ceiling"
                     |DecimalMathOrOperatorsMember "Floor") as m) ,[arg1]) -> 
            return! unop arg1 (fun arg1H -> EFunc(m.Name.ToUpper(),[arg1H], typeOfExpr arg1H))
        | Call(None,DecimalMathOrOperatorsMember "Atan2",[arg1; arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> EFunc("ATAN2",[arg1H; arg2H], typeOfExpr arg1H))
        | Call(None,DecimalMathOrOperatorsMember "Log"  ,[arg1])       -> return! unop arg1 (fun arg1H -> EFunc("LOG10",[arg1H], typeOfExpr arg1H))
        | Call(None,DecimalMathOrOperatorsMember "Pow"  ,[arg1;arg2])       -> return! binop arg1 arg2 (fun arg1H arg2H -> EFunc("POWER",[arg1H; arg2H], typeOfExpr arg1H)) 
        | NewObject(ci, [arg1;arg2;arg3]) when ci.DeclaringType.FullName = "System.DateTime" ->
                    // TODO: check this one
              return! triop arg1 arg2 arg3 (fun arg1H arg2H arg3H -> 
                  EFunc("Convert", [ EAtom "DateTime"; 
                                  concat [EFunc("Convert",[EAtom "nvarchar"; arg1H], DString); EString "/"
                                          EFunc("Convert",[EAtom "nvarchar"; arg2H], DString); EString "/"
                                          EFunc("Convert",[EAtom "nvarchar"; arg3H], DString)]], DDate))
        | NewObject(ci, [arg1;arg2;arg3;arg4;arg5;arg6]) when ci.DeclaringType.FullName = "System.DateTime" ->
              return! sixop arg1 arg2 arg3 arg4 arg5 arg6 (fun arg1H arg2H arg3H arg4H arg5H arg6H -> 
                    EFunc("Convert", [ EAtom "DateTime"; 
                                       concat [EFunc("Convert",[EAtom "nvarchar"; arg1H], DString); EString "/"
                                               EFunc("Convert",[EAtom "nvarchar"; arg2H], DString); EString "/"
                                               EFunc("Convert",[EAtom "nvarchar"; arg3H], DString); EString " "
                                               EFunc("Convert",[EAtom "nvarchar"; arg4H], DString); EString ":"
                                               EFunc("Convert",[EAtom "nvarchar"; arg5H], DString); EString ":"
                                               EFunc("Convert",[EAtom "nvarchar"; arg6H], DString)]], DDate))

        | Call(None,OperatorsMember "op_Equality"          ,[arg1;arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> RelOp(arg1H, "=", arg2H))
        | Call(None,OperatorsMember "op_LessThan"          ,[arg1;arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> RelOp(arg1H, "<", arg2H))
        | Call(None,OperatorsMember "op_GreaterThan"       ,[arg1;arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> RelOp(arg1H, ">", arg2H))
        | Call(None,OperatorsMember "op_LessThanOrEqual"   ,[arg1;arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> RelOp(arg1H, "<=", arg2H))
        | Call(None,OperatorsMember "op_GreaterThanOrEqual",[arg1;arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> RelOp(arg1H, ">=", arg2H))
        | Call(None,OperatorsMember "op_Inequality"        ,[arg1;arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> RelOp(arg1H, "!=", arg2H))


        // Nullable operators x ?= y etc.
        | Call(None,NullableOperatorsMember "op_QmarkEquals",[arg1;arg2]) 
        | Call(None,NullableOperatorsMember "op_QmarkEqualsQmark",[arg1;arg2]) 
        | Call(None,NullableOperatorsMember "op_EqualsQmark",[arg1;arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> RelOp(arg1H, "=", arg2H))

        | Call(None,NullableOperatorsMember "op_QmarkLess",[arg1;arg2]) 
        | Call(None,NullableOperatorsMember "op_QmarkLessQmark",[arg1;arg2]) 
        | Call(None,NullableOperatorsMember "op_LessQmark",[arg1;arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> RelOp(arg1H, "<", arg2H))

        | Call(None,NullableOperatorsMember "op_QmarkGreater",[arg1;arg2]) 
        | Call(None,NullableOperatorsMember "op_QmarkGreaterQmark",[arg1;arg2]) 
        | Call(None,NullableOperatorsMember "op_GreaterQmark",[arg1;arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> RelOp(arg1H, ">", arg2H))

        | Call(None,NullableOperatorsMember "op_QmarkLessEquals",[arg1;arg2]) 
        | Call(None,NullableOperatorsMember "op_QmarkLessEqualsQmark",[arg1;arg2]) 
        | Call(None,NullableOperatorsMember "op_LessEqualsQmark",[arg1;arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> RelOp(arg1H, "<=", arg2H))

        | Call(None,NullableOperatorsMember "op_QmarkGreaterEquals",[arg1;arg2]) 
        | Call(None,NullableOperatorsMember "op_QmarkGreaterEqualsQmark",[arg1;arg2]) 
        | Call(None,NullableOperatorsMember "op_GreaterEqualsQmark",[arg1;arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> RelOp(arg1H, ">=", arg2H))

        | Call(None,NullableOperatorsMember "op_QmarkLessGreater",[arg1;arg2]) 
        | Call(None,NullableOperatorsMember "op_QmarkLessGreaterQmark",[arg1;arg2]) 
        | Call(None,NullableOperatorsMember "op_LessGreaterQmark",[arg1;arg2]) -> return! binop arg1 arg2 (fun arg1H arg2H -> RelOp(arg1H, "!=", arg2H))

        // Nullable operators x ?+ y etc.
        | Call(None,NullableOperatorsMember "op_QmarkPlus",[arg1; arg2]) 
        | Call(None,NullableOperatorsMember "op_QmarkPlusQmark",[arg1; arg2]) 
        | Call(None,NullableOperatorsMember "op_PlusQmark",[arg1; arg2]) -> return! binop arg1 arg2 (+.)

        | Call(None,NullableOperatorsMember "op_QmarkMinus",[arg1; arg2]) 
        | Call(None,NullableOperatorsMember "op_QmarkMinusQmark",[arg1; arg2]) 
        | Call(None,NullableOperatorsMember "op_MinusQmark",[arg1; arg2]) -> return! binop arg1 arg2 (-.)

        | Call(None,NullableOperatorsMember "op_QmarkMultiply",[arg1; arg2]) 
        | Call(None,NullableOperatorsMember "op_QmarkMultiplyQmark",[arg1; arg2]) 
        | Call(None,NullableOperatorsMember "op_MultiplyQmark",[arg1; arg2]) -> return! binop arg1 arg2 ( *. )

        | Call(None,NullableOperatorsMember "op_QmarkDivide",[arg1; arg2]) 
        | Call(None,NullableOperatorsMember "op_QmarkDivideQmark",[arg1; arg2]) 
        | Call(None,NullableOperatorsMember "op_DivideQmark",[arg1; arg2]) -> return! binop arg1 arg2 (/.)

        | Call(None,NullableOperatorsMember "op_QmarkPercent",[arg1; arg2]) 
        | Call(None,NullableOperatorsMember "op_QmarkPercentQmark",[arg1; arg2]) 
        | Call(None,NullableOperatorsMember "op_PercentQmark",[arg1; arg2]) -> return! binop arg1 arg2 (%.)


        | QuotationUtils.MacroReduction reduced -> return! tryConvExpr reduced
        // TODO:
        //    obj.ToString() -.> CONVERT(NVARCHAR, obj)
        //    i.CompareTo(j) -.> (CASE WHEN i = j THEN 0 WHEN i < j THEN -1 ELSE 1 END)
        //    System.Int32.Compare(i,j) -.> (CASE WHEN i = j THEN 0 WHEN i < j THEN -1 ELSE 1 END)
        //    a ** b -.> POWER(a,b)
        //    pown a b -.> POWER(a,b)
        //    ?????????????? -.> COALESCE(a,b)
        //    a <<< b -.> (a * POWER(2,b))
        //    a >>> b -.> (a / POWER(2,b))
        //    b -.> (CASE WHEN (b) THEN 1 ELSE 0 END)
        | _ -> 
            let n = if allowEvaluation then try Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation e with _ -> null else null
            match n with 
            | :? Int32 as d -> return EInt32 d 
            | :? Int64  as d -> return EVal (VInt64 (Value d), DInt64) 
            | :? Single  as d -> return EVal (VSingle (Value d), DSingle (AnyUnit.NewVar(dataCtxt.UseUnitAnnotations))) 
            | :? Double  as d -> return EVal (VDouble (Value d), DDouble (AnyUnit.NewVar(dataCtxt.UseUnitAnnotations))) 
            | :? Decimal as d -> return EVal (VDecimal (Value d), DDecimal (AnyUnit.NewVar(dataCtxt.UseUnitAnnotations))) 
            | :? String  as s -> return EString s  
            | :? Boolean as b -> return EVal (VBoolean (Value  b), DBoolean) 
            | _obj -> return! None
        }
    and unop arg1 f = 
        option { 
            let! arg1H = tryConvExpr arg1 
            return f arg1H 
        }
    and binop arg1 arg2 f = 
        option { 
            let! arg1H = tryConvExpr arg1 
            let! arg2H = tryConvExpr arg2 
            return f arg1H arg2H
        }
    and triop arg1 arg2 arg3 f = 
        option { 
            let! arg1H = tryConvExpr arg1 
            let! arg2H = tryConvExpr arg2 
            let! arg3H = tryConvExpr arg3
            return f arg1H arg2H arg3H
        }
    and sixop arg1 arg2 arg3 arg4 arg5 arg6 f = 
        option { 
            let! arg1H = tryConvExpr arg1 
            let! arg2H = tryConvExpr arg2 
            let! arg3H = tryConvExpr arg3
            let! arg4H = tryConvExpr arg4
            let! arg5H = tryConvExpr arg5 
            let! arg6H = tryConvExpr arg6
            return f arg1H arg2H arg3H arg4H arg5H arg6H
        }
    let convExpr x = 
        match tryConvExpr x with 
        | Some res -> res
        | None -> failwithf "Unrecognized expression: %A" x

    // Loop through conjuncts
    let rec foldPredicates queryData pq = 
        match pq with 
        | AndAlso(cq1,cq2) -> foldPredicates (foldPredicates queryData cq1) cq2
        | _ -> HiveQueryData.Where(queryData, convExpr pq)

    member x.FoldPredicates queryData pq = foldPredicates queryData pq
    member x.ConvertExpression c = convExpr c
    member x.RecognizeExpression c = 
        let res = tryConvExpr c  |> Option.isSome
        //if res then printfn "recognized %A" c
        //else printfn "did not recognize %A" c
        res

type HiveQueryBuilder() = 

    /// Detect 'select' queries where we have not had a 'where', 'count' or any other server-side operation (apart from 'limit').
    /// This causes the query translation to prefer a pure 'select *' plus a client-side tail operation, which is much faster
    /// because no map/reduce job is scheduled.
    let rec isSimpleSelect q = 
        match q with 
        | Table _ -> true
        | Distinct _ | GroupBy _ | AggregateOpBy _ | Count _ | Where _ -> false
        | Limit(hq,_) | Select(hq,_) | TableSample(hq,_,_) -> isSimpleSelect hq 

    let normalize auxVars auxExprs (e:Expr) = 
        assert (List.length auxVars = List.length auxExprs)
        let auxMap = dict (List.zip auxVars auxExprs)
        e.Substitute(fun v -> if auxMap.ContainsKey v then Some auxMap.[v] else None)

    // canTail - can we accept a 'select' with tail ops
    let rec decodeQuery nestedTableOpt canTail q = 
        match q with 
        | Call(_,mi,[hq;Lambdas([(tableVar::auxVars)],selectorBody)]) when mi.Name = "Select" -> 
            decodeSelect nestedTableOpt canTail (hq,tableVar,auxVars,selectorBody)
(*
CREATE [EXTERNAL] TABLE [IF NOT EXISTS] [db_name.]table_name
  [(col_name data_type [COMMENT col_comment], ...)]
  [COMMENT table_comment]
  [PARTITIONED BY (col_name data_type [COMMENT col_comment], ...)]
  [CLUSTERED BY (col_name, col_name, ...) [SORTED BY (col_name [ASC|DESC], ...)] INTO num_buckets BUCKETS]
  [SKEWED BY (col_name, col_name, ...) ON ([(col_value, col_value, ...), ...|col_value, col_value, ...]) (Note: only available starting with 0.10.0)]
  [
   [ROW FORMAT row_format] [STORED AS file_format]
   | STORED BY 'storage.handler.class.name' [WITH SERDEPROPERTIES (...)]  (Note: only available starting with 0.6.0)
  ]
  [LOCATION hdfs_path]
  [TBLPROPERTIES (property_name=property_value, ...)]  (Note: only available starting with 0.6.0)
  [AS select_statement]  (Note: this feature is only available starting with 0.5.0, and is not supported when creating external tables.)

*)
        // newTable tableName expr
        // writeDistributedFile tableName expr
        //
        // Note: writeLocalFile is disabled, see note below
        | Call(_,mi,[hq;targetTableNameExpr;Lambdas([tableVar::auxVars],selectorBody)]) when mi.Name = "NewTable" || mi.Name = "WriteLocalFile" || mi.Name = "WriteDistributedFile" -> 
            let (hqCtxt:HiveDataContext), hqQueryData, hqTail, hqTimeout, hqAuxExprsF  = decodeSelect nestedTableOpt false (hq,tableVar,auxVars,selectorBody)
            // Evaluate the name of the table
            let targetTableName = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation targetTableNameExpr :?> string
            let _tableName,colData = colDataOfQuery hqQueryData
            if mi.Name = "NewTable" then 
                hqCtxt.ExecuteCommand("DROP TABLE " + targetTableName) |> ignore
                let columns = [ for c in colData -> c.HiveName + " " + HiveSchema.formatHiveType c.HiveType + " COMMENT 'The column " + c.HiveName + " in the table " + targetTableName + " " + HiveSchema.formatHiveComment (c.HiveType, c.IsRequired) + "'" ] |> String.concat ","
                hqCtxt.ExecuteCommand("CREATE TABLE " + targetTableName + "(" + columns + ")", ?timeout=hqTimeout) |> ignore
                // We leave the query results as non-SI units because we are writing the results directly
                // to a non-SI distributed table annotated with non-SI metadata.
                let queryText = HiveSchema.formatQuery HiveQueryUnitSystem.NonSI hqQueryData
                hqCtxt.ExecuteCommand("INSERT OVERWRITE TABLE " + targetTableName + " " + queryText, ?timeout=hqTimeout) |> ignore
                let _tableName, colData = colDataOfQuery hqQueryData
                hqCtxt, Table(targetTableName, colData), hqTail, None, hqAuxExprsF 
            else 
                // We write the results using SI units because those are the types in the F# code
                // and how they would appear as F# values, and the results are no longer annotated with 
                // non-SI metadata.
                let queryText = HiveSchema.formatQuery HiveQueryUnitSystem.SI hqQueryData
                hqCtxt.ExecuteCommand("INSERT OVERWRITE " + (if mi.Name = "WriteDistributedFile" then "" else "LOCAL ") + "DIRECTORY '" + targetTableName + "' " + queryText, ?timeout=hqTimeout) |> ignore
                hqCtxt, hqQueryData, hqTail, hqTimeout, hqAuxExprsF 
            

        // writeRows (table.NewRow(....))
        // insertRows (table.NewRow(....))
        | Call(_,mi,[hq;Lambdas([tableVar::auxVars],rowDescription)]) when mi.Name = "WriteRows" || mi.Name = "InsertRows" || mi.Name = "WritePartition" || mi.Name = "InsertPartition" -> 
            let hqCtxt, hqQueryData, (hqTail:Expr list), hqTimeout, hqAuxExprsF  = decodeQuery nestedTableOpt false hq
            let hqAuxExprs = hqAuxExprsF tableVar
            let targetTable, colArgs =
                let rec look t = 
                    match t with 
                    | NewObject(_ci,[String targetTable; NewArray(_,elems)]) -> 
                        let colArgs = elems |> List.map (function NewTuple [String colName; Coerce(colArg,_); Bool colIsPartitionKey] -> (colName, colArg, colIsPartitionKey)
                                                                | x -> failwithf "unexpected form for row contents in 'WriteRows/InsertRows': %A " x)
                        targetTable, colArgs
                    | Coerce(t2,_) -> look t2
                    | Let(v,e,b) -> look (b.Substitute(fun v2 -> if v = v2 then Some e else None))
                    | _ -> failwithf "unexpected form for row description in 'WriteRows/InsertRows' :\n%A" t
                look rowDescription
            if not hqTail.IsEmpty then failwith "unexpected selection before 'writeRows'"
            
            let tableName, colData = colDataOfQuery hqQueryData
 
            let converter = HiveExpressionTranslator(decodeQuery,hqCtxt, tableName, tableVar, colData, auxVars, hqAuxExprs, allowEvaluation=true)
            let partitionKeys, trueColArgs = 
                colArgs |> List.partition (fun (_colName, _colExpr, colIsPartitionKey) -> colIsPartitionKey)
            
            if mi.Name = "WriteRows" && not partitionKeys.IsEmpty then failwith "use 'writePartition' to write to a partition of a partitioned table"
            if mi.Name = "WritePartition" && partitionKeys.IsEmpty then failwith "use 'writeRows' to write to a non-partitioned table"
            // e.g. PARTITION(loadKey1='2013-02-12', loadKey2='Iris-setosa')
            let partitionKeysText = 
                match partitionKeys with 
                | [] -> ""
                | _ ->
                   "PARTITION(" + 
                     (partitionKeys
                        |> List.map (fun (colName, colExpr, _colIsPartitionKey) -> 
                            let colExprConv = converter.ConvertExpression colExpr
                            // We write using non-SI units since we're writing to a table stored as non-SI units
                            let exprText = HiveSchema.formatExpr HiveQueryUnitSystem.NonSI colExprConv
                            colName + "=" + exprText)
                        |> String.concat ",") + 
                     ") "
            let colExprs = 
                trueColArgs |> List.map (fun (colName, colExpr, _colIsPartitionKey) -> (colName, converter.ConvertExpression colExpr, isRequiredTy colExpr.Type)) 
                            |> List.toArray
            let hqQueryDataWithSelect = Select(hqQueryData, colExprs)
            let command = if mi.Name = "InsertRows" then "INSERT" else "INSERT OVERWRITE" 

            // We format the query using non-SI units since the table is stored using non-SI units
            let queryText = HiveSchema.formatQuery HiveQueryUnitSystem.NonSI hqQueryDataWithSelect 
            hqCtxt.ExecuteCommand(command + " TABLE " + targetTable + " " + partitionKeysText + queryText, ?timeout=hqTimeout) |> ignore
            hqCtxt, hqQueryData, hqTail, None, hqAuxExprsF 

        // where expr
        | Call(_,mi,[hq;Lambdas([tableVar::auxVars],pred)]) when mi.Name = "Where" -> 
            let hqCtxt, hqQueryData, hqTail, hqTimeout, hqAuxExprsF  = decodeQuery nestedTableOpt false hq
            let hqAuxExprs = hqAuxExprsF tableVar
            let tableName, colData = colDataOfQuery hqQueryData
            let converter = HiveExpressionTranslator(decodeQuery,hqCtxt, tableName, tableVar, colData, auxVars, hqAuxExprs, allowEvaluation=true)
            let queryData = converter.FoldPredicates hqQueryData pred
            hqCtxt, queryData, hqTail, hqTimeout, hqAuxExprsF

        // This recognizes for ... yield which may include additional 'let'. This arises whenever
        // internal 'let' expressions are used, e.g.
        //     
        //  hiveQuery { for x in ctxt.abalone do
        //              let avg = 1.0 
        //              let avg2 = 2.0 
        //              select (x.Name, avg + avg2) }
        //
        // Somewhat unexpectedly, the 'let' bind towards the 'for', in the sense that the construct becomes
        // this:
        //
        //  Select("for x in ctxt.abalone do
        //          let avg = 1.0 
        //          let avg2 = 2.0 
        //          yield (x,avg,avg2)"
        //         (fun x,avg,avg2 -> select (x.Name, avg + avg2)))
        //
        // The general pattern is For(..., Lambdas(Let..., Let(newVar,newExpr, ... Yield(...newVar)))
        | Call(_,mi1,[hq;Lambdas([tableVar::auxVars],forLoopBody)]) when mi1.Name = "For"  -> 
            let hqCtxt, hqQueryData, hqTail, hqTimeout, hqAuxExprsF  = decodeQuery nestedTableOpt canTail hq
            let hqAuxExprs = hqAuxExprsF tableVar
            let forLoopBodyNorm = normalize auxVars hqAuxExprs forLoopBody
            let rec loop e = 
                match e with 
                | Let(newAuxVar, newAuxExpr, e2) -> 
                    loop (e2.Substitute (fun v2 -> if newAuxVar = v2 then Some newAuxExpr else None))
                | Call(_,mi2,[NewTuple (Var tableVar2::finalAuxExprs)]) when tableVar = tableVar2 && mi2.Name = "Yield" -> 
                     (fun tableVar3 -> finalAuxExprs |> List.map (normalize [tableVar2] [Expr.Var tableVar3]))
                | Call(_,mi2,[Var tableVar2]) when tableVar = tableVar2 && mi2.Name = "Yield" -> 
                     (fun _ -> [])
                | e -> failwithf "unrecognized construct in 'for' expression: %A" e
            let newAuxExprs = loop forLoopBodyNorm
            hqCtxt, hqQueryData, hqTail, hqTimeout, newAuxExprs

        // This recognizes the 'groupBy ... into g' pattern
        | Call(_,mi2,[hq;Lambdas([tableVar::auxVars],keySelector)]) when mi2.Name = "GroupBy" -> 
            let hqCtxt, hqQueryData, hqTail, hqTimeout, hqAuxExprsF  = decodeQuery nestedTableOpt false hq
            let hqAuxExprs = hqAuxExprsF tableVar
            let tableName, colData = colDataOfQuery hqQueryData
            let converter = HiveExpressionTranslator(decodeQuery,hqCtxt, tableName, tableVar, colData, auxVars, hqAuxExprs, allowEvaluation=true)
            let groupKeyExpr = converter.ConvertExpression keySelector
            hqCtxt, HiveQueryData.GroupBy(hqQueryData, groupKeyExpr), hqTail, hqTimeout, (fun _ -> [])

        // take numElements
        | Call(_,mi,[hq;nExpr]) when mi.Name = "Take" -> 
            let n = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation nExpr :?> int
            let hqCtxt, hqQueryData, hqTail, hqTimeout, hqAuxExprsF  = decodeQuery nestedTableOpt canTail hq
            hqCtxt, HiveQueryData.Limit(hqQueryData, n), hqTail , hqTimeout, hqAuxExprsF 

        // distinct
        | Call(_,mi,[hq]) when mi.Name = "Distinct" -> 
            let hqCtxt, hqQueryData, hqTail, hqTimeout, hqAuxExprsF  = decodeQuery nestedTableOpt false hq
            hqCtxt, HiveQueryData.Distinct(hqQueryData), hqTail , hqTimeout, hqAuxExprsF 

        // count
        | Call(_,mi,[hq]) when mi.Name = "Count" -> 
            let hqCtxt, hqQueryData, _hqTail, hqTimeout, hqAuxExprsF  = decodeQuery nestedTableOpt false hq
            // Throw away _hqTail, e.g. select-then-count
            hqCtxt, HiveQueryData.Count(hqQueryData), [], hqTimeout, hqAuxExprsF 

        // sumBy expr
        // sumByNullable expr
        // ...
        | Call(_,mi,[hq;Lambdas([tableVar::auxVars],selector)]) 
                                                    when mi.Name = "SumBy" || mi.Name = "SumByNullable" || 
                                                         mi.Name = "MaxBy" || mi.Name = "MaxByNullable" || 
                                                         mi.Name = "MinBy" || mi.Name = "MinByNullable" || 
                                                         mi.Name = "AverageBy" || mi.Name = "AverageByNullable"  -> 
            let hqCtxt, hqQueryData, hqTail, hqTimeout, hqAuxExprsF  = decodeQuery nestedTableOpt false hq
            let hqAuxExprs = hqAuxExprsF tableVar
            let tableName, colData = colDataOfQuery hqQueryData
            let converter = HiveExpressionTranslator(decodeQuery,hqCtxt, tableName, tableVar, colData, auxVars, hqAuxExprs, allowEvaluation=true)
            let selectorExpr = converter.ConvertExpression selector
            if not hqTail.IsEmpty then failwith ("unexpected selection before '" + mi.Name + "'")
            let op = 
                match mi.Name with 
                | "SumBy" | "SumByNullable" -> "SUM"
                | "MaxBy" | "MaxByNullable" -> "MAX"
                | "MinBy" | "MinByNullable" -> "MIN"
                | "AverageBy" | "AverageByNullable" -> "AVG"
                | _ -> failwith "unreachable"
            hqCtxt, HiveQueryData.AggregateOpBy(op,hqQueryData, selectorExpr, isRequiredTy selector.Type), [], hqTimeout, hqAuxExprsF 

        // timeout timeoutInSeconds
        | Call(_,mi,[hq;nExpr]) when mi.Name = "Timeout" -> 
            let n = (Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation nExpr :?> int) * 1<s>
            let hqCtxt, hqQueryData, hqTail, _hqTimeout, hqAuxExprsF  = decodeQuery nestedTableOpt canTail hq
            hqCtxt, hqQueryData, hqTail, Some n, hqAuxExprsF 

        // sampleBucket bucketNumber numberOfBuckets
        | Call(_,mi,[hq;nExpr1;nExpr2]) when mi.Name = "SampleBucket" -> 
            let n1 = (Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation nExpr1 :?> int) 
            let n2 = (Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation nExpr2 :?> int) 
            let hqCtxt, hqQueryData, hqTail, hqTimeout, hqAuxExprsF  = decodeQuery nestedTableOpt canTail hq
            hqCtxt, HiveQueryData.TableSample(hqQueryData,n1,n2), hqTail, hqTimeout, hqAuxExprsF

        // Match a table variable for a group, used in nested queries, e.g. 'g' in 'for x in g' in this:
        //
        //   hiveQuery { for x in ctxt.abalone do 
        //               groupBy x.sex into g
        //               select (g.Key,hiveQuery { for x in g do averageBy x.height }) } 
        //
        | Coerce(hq,_) -> decodeQuery nestedTableOpt canTail hq
        | Var v when (match nestedTableOpt with Some (nestedTableVar,_,_,_) -> v = nestedTableVar | None -> false) ->
            match nestedTableOpt with 
            | Some (_nestedTableVar,nestedTableCtxt,nestedTableName,nestedTableColData) -> 
                nestedTableCtxt, HiveQueryData.Table(nestedTableName,nestedTableColData), [], None, (fun _ -> [])
            | _ -> failwith "unreachable"
            
        // The 'hq' in ' hiveQuery { for v in hq do ... } gets evaluated here....
        | _ -> 
            let obj = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation q 
            match obj with 
            | :? IHiveQueryable as hq -> hq.DataContext, hq.QueryData, hq.TailOps, hq.Timeout, (fun _ -> [])
            | _ -> failwithf "The following quotation evaluated to a '%s' instead of a HiveQueryable: %A" (obj.GetType().ToString()) q

    and decodeSelect nestedTableOpt canTail (hq,tableVar,auxVars,selectorBody) =
        let (hqCtxt:HiveDataContext), hqQueryData, hqTail, hqTimeout, hqAuxExprsF  = decodeQuery nestedTableOpt false hq

        // Substitute the aux expressions through the selection
        let hqAuxExprs = hqAuxExprsF tableVar
        let selectorBody = normalize auxVars hqAuxExprs selectorBody
        // when 'canTail' is true, 'select' can involve any expression at all, e.g. tuples. At the fringes we find expressions, e.g. "tableVar.column1 + tableVar.column2"
        let qQueryData, qTail = 
          match selectorBody with 
          | Var v when tableVar = v -> 
              hqQueryData, hqTail

          // Simple selections like 'select x.height' become 'SELECT *' followed by a tail op.
          | _ when 
                 // Only apply this optimiation for selections in 'tail' position which are actually
                 // about to produce in-memory objects.
                 canTail && 
                 // Don't apply this optimiation when using unit annotations as transformations from Non-SI to SI may be needed.
                 not hqCtxt.UseUnitAnnotations && 
                 // Check this is a simple selection.
                 isSimpleSelect hqQueryData && 
                 // We don't apply this optimization when there are nested queries in the result since they do not
                 // execute on the client side.
                 not (selectorBody |> existsSubQuotation (function NestedQuery(_) -> true | _ -> false)) -> 
              hqQueryData, hqTail @ [ Expr.Lambda(tableVar, selectorBody) ] 
          | _ -> 
            let tableName, colData = colDataOfQuery hqQueryData
            // First see if we are in the final ('tail') position where we can produce in-memory objects
            // using a tail expression.
            if canTail then 
                // If in tail position, extract the parts of the query that can be run on the server
                // side and replace them by variables. Then use the resulting expression as the tail expression.
                let recognizer = HiveExpressionTranslator(decodeQuery, hqCtxt, tableName, tableVar, colData, auxVars, hqAuxExprs, allowEvaluation=false)
                let next = let c = ref 0 in fun () -> incr c; string !c
                let newExtractor(t:Expr) = 
                    let extractorVar = new Var("Item"+next(),t.Type)
                    [(extractorVar,t)], Expr.Var(extractorVar)
                let rec extract t = 
                    if recognizer.RecognizeExpression t then 
                        newExtractor  t
                    else
                        match t with 
                        | Quotations.ExprShape.ShapeCombination(op,xs) -> 
                            let subProjs,subResidues = List.map extract xs  |> List.unzip
                            List.concat subProjs, ExprShape.RebuildShapeCombination(op,subResidues)
                        | Quotations.ExprShape.ShapeLambda(v,body) -> 
                            let subProjs,subResidue = extract body
                            subProjs, Expr.Lambda(v, subResidue)
                        | Quotations.ExprShape.ShapeVar _ -> [], t 
                let projs, residue = extract selectorBody
                let newTableVar = new Var("x",typeof<HiveDataRow>)
                
                // Replace all the extractor variables in the residue with lookups on the newTableVar
                let newProjection = 
                    (projs, residue) ||> List.foldBack (fun (extractorVar,_e) t -> 
                        let replacementForExtractionVar = 
                            let meth = HiveDataRow.GetValueMethodInfo.MakeGenericMethod(extractorVar.Type)
                            Expr.Call(Expr.Var(newTableVar),meth,[Expr.Value extractorVar.Name])
                        t.Substitute(fun v -> 
                            if v = extractorVar then Some replacementForExtractionVar
                            else None))  
                let newProjectionLambda = Expr.Lambda(newTableVar, newProjection)
                let converter = HiveExpressionTranslator(decodeQuery,hqCtxt, tableName, tableVar, colData, auxVars, hqAuxExprs, allowEvaluation=true)
                let colExprs = projs |> List.map (fun (v,e) -> (v.Name, converter.ConvertExpression e, isRequiredTy e.Type)) |> List.toArray
                let qQueryData = Select(hqQueryData, colExprs)
                qQueryData, hqTail @ [ newProjectionLambda ] 
            else
                // When a 'select' is not executable in-memory, it must be more restricted. For example
                //     ... select x.gender; ...
                //     ... select (x.gender, x.height) ...
                //     ... select {Gender=x.gender;Height=x.height} ....
                //     ... select (Data(Gender=x.gender,Height=x.height)) ...
                // In these cases, we can't execute the select as in-memory tail operations.
                let converter = HiveExpressionTranslator(decodeQuery,hqCtxt, tableName, tableVar, colData, auxVars, hqAuxExprs, allowEvaluation=true)
                let colExprs = 
                    match selectorBody with 
                    | Patterns.NewTuple(exprs) -> exprs |> List.mapi (fun i x -> "Item" + string (i+1), x) |> List.toArray
                    | Patterns.NewUnionCase(uc,exprs) -> (uc.GetFields(), exprs) ||> Seq.map2 (fun p x -> p.Name, x) |> Seq.toArray 
                    | Patterns.NewRecord(r,exprs) -> (FSharpType.GetRecordFields r, exprs) ||> Seq.map2 (fun p x -> p.Name, x) |> Seq.toArray 
                    | t -> [| ("Item1",t) |]
                let colExprs = colExprs |> Array.map (fun (nm,e) -> (nm,converter.ConvertExpression e, isRequiredTy e.Type))
                let qQueryData = Select(hqQueryData, colExprs)
                qQueryData, hqTail 


        hqCtxt, qQueryData, qTail, hqTimeout, (fun _ -> [])

    member __.For(source : HiveQueryable<'T>, selector : 'T -> 'U) : HiveQueryable<'U> = ignore (source,selector); failwith "The 'for' operator may only be executed on the server"

    [<CustomOperation("timeout", MaintainsVariableSpace = true)>]
    /// Specify a timeout for the query
    member __.Timeout(source : HiveQueryable<'T>, timeoutInSeconds : int) : HiveQueryable<'T> = ignore (source,timeoutInSeconds); failwith "The 'timeout' operator may only be executed on the server"

    [<CustomOperation("sampleBucket", MaintainsVariableSpace = true)>]
    /// Specify a sample bucket to use from a clsutered distributed table
    member __.SampleBucket(source : HiveQueryable<'T>, bucketNumber : int, numBuckets : int) : HiveQueryable<'T> = ignore (source,bucketNumber,numBuckets); failwith "The 'timeout' operator may only be executed on the server"

    [<CustomOperation("take", MaintainsVariableSpace = true)>]
    /// Specify the maximum number of query element results to return
    member __.Take(source : HiveQueryable<'T>, n : int) : HiveQueryable<'T> = ignore (source,n); failwith "The 'take' operator may only be executed on the server"

    [<CustomOperation("count")>]
    /// Count the number of selected items
    member __.Count(source : HiveQueryable<'T>) : int64 = ignore (source); failwith "The 'count' operator may only be executed on the server"

    [<CustomOperation("where", MaintainsVariableSpace = true)>]
    /// Restrict the selected items to those satisfying the given predicate
    member __.Where(source : HiveQueryable<'T>, [<ProjectionParameter>]selector : 'T -> bool) : HiveQueryable<'T> = ignore (source,selector); failwith "The 'where' operator may only be executed on the server"

    [<CustomOperation("select")>]
    /// Select the given items as the result of the query
    member __.Select(source : HiveQueryable<'T>, [<ProjectionParameter>]selector : 'T -> 'U) : HiveQueryable<'U> = ignore (source,selector); failwith "The 'select' operator may only be executed on the server"

    //[<CustomOperation("select1")>]
    //member __.Select1(source : HiveQueryable<'T>, [<ProjectionParameter>]selector : 'T -> 'U) : 'U = ignore (source); failwith "The 'select1' operator may only be executed on the server"

    [<CustomOperation("averageBy")>]
    /// Evaluate the average of the given selection
    member inline __.AverageBy< 'T, ^Value when ^Value : (static member ( + ) : ^Value * ^Value -> ^Value) and  ^Value : (static member DivideByInt : ^Value * int -> ^Value) and  ^Value : (static member Zero : ^Value)>(source: HiveQueryable<'T>, [<ProjectionParameter>]_selector: 'T -> ^Value) : ^Value =  ignore (source); failwith "The 'averageBy' operator may only be executed on the server"

    [<CustomOperation("minBy")>]
    /// Evaluate the minimum of the given selection
    member __.MinBy<'T,'Key when 'Key : equality and 'Key : comparison> (source:HiveQueryable<'T>, [<ProjectionParameter>] valueSelector : 'T -> 'Key) : 'Key = ignore (source,valueSelector); failwith "The 'minBy' operator may only be executed on the server"

    [<CustomOperation("maxBy")>]
    /// Evaluate the maximum of the given selection
    member __.MaxBy<'T,'Key when 'Key : equality and 'Key : comparison> (source:HiveQueryable<'T>, [<ProjectionParameter>] valueSelector : 'T -> 'Key) : 'Key = ignore (source,valueSelector); failwith "The 'maxBy' operator may only be executed on the server"

    [<CustomOperation("sumBy")>]
    /// Evaluate the sum of the given selection
    member inline __.SumBy< 'T, ^Value when ^Value : (static member ( + ) : ^Value * ^Value -> ^Value) and  ^Value : (static member Zero : ^Value)>(source:HiveQueryable<'T>, [<ProjectionParameter>] valueSelector : ('T -> ^Value)) : ^Value =  ignore (source,valueSelector); failwith "The 'averageBy' operator may only be executed on the server"

    [<CustomOperation("averageByNullable")>]
    /// Evaluate the average of the given selection
    member inline __.AverageByNullable< 'T, ^Value when ^Value :> ValueType and ^Value : struct and ^Value : (new : unit -> ^Value) and ^Value : (static member ( + ) : ^Value * ^Value -> ^Value) and  ^Value : (static member DivideByInt : ^Value * int -> ^Value) and  ^Value : (static member Zero : ^Value)  >(source: HiveQueryable<'T>, [<ProjectionParameter>]_selector: 'T -> Nullable< ^Value >) : Nullable< ^Value >   =  ignore (source); failwith "The 'averageByNullable' operator may only be executed on the server"

    [<CustomOperation("minByNullable")>]
    /// Evaluate the minimum of the given selection
    member __.MinByNullable<'T,'Key when 'Key : equality and 'Key : comparison and 'Key : (new : unit -> 'Key) and 'Key : struct and 'Key :> ValueType> (source:HiveQueryable<'T>, [<ProjectionParameter>]valueSelector : 'T -> Nullable<'Key>) : Nullable<'Key> =  ignore (source,valueSelector); failwith "The 'maxBy' operator may only be executed on the server"

    [<CustomOperation("maxByNullable")>]
    /// Evaluate the maximum of the given selection
    member __.MaxByNullable<'T,'Key when 'Key : equality and 'Key : comparison and 'Key : (new : unit -> 'Key) and 'Key : struct and 'Key :> ValueType> (source:HiveQueryable<'T>, [<ProjectionParameter>]valueSelector : 'T -> Nullable<'Key>) : Nullable<'Key> = ignore (source,valueSelector); failwith "The 'maxBy' operator may only be executed on the server"

    [<CustomOperation("sumByNullable")>]
    /// Evaluate the sum of the given selection
    member inline __.SumByNullable<'T, ^Value when ^Value :> ValueType and ^Value : struct  and ^Value : (new : unit -> ^Value)  and ^Value : (static member ( + ) : ^Value * ^Value -> ^Value)  and  ^Value : (static member Zero : ^Value)>(source: HiveQueryable<'T>, [<ProjectionParameter>]valueSelector : 'T -> Nullable< ^Value >) : Nullable< ^Value >  = ignore (source,valueSelector); failwith "The 'sumByNullable' operator may only be executed on the server"

    [<CustomOperation("groupBy",AllowIntoPattern=true)>] 
    /// Group the selection by the given key
    member __.GroupBy<'T,'Key when 'Key : equality> (source: HiveQueryable<'T>, [<ProjectionParameter>] keySelector:('T -> 'Key)) : HiveQueryable<HiveGrouping<'Key,'T>>  = ignore (source,keySelector); failwith "The 'groupBy' operator may only be executed on the server"

    [<CustomOperation("distinct",MaintainsVariableSpace=true,AllowIntoPattern=true)>] 
    /// Ensure the elements of the selection are distinct
    member __.Distinct<'T when 'T : equality>(source:HiveQueryable<'T>) : HiveQueryable<'T>  = ignore (source); failwith "The 'distinct' operator may only be executed on the server"
    //member __.GroupValBy<'T,'Key,'Result, 'Q when 'Key : equality > (source:HiveQueryable<'T>, resultSelector: 'T -> 'Result, keySelector: 'T -> 'Key) : HiveQueryable<System.Linq.IGrouping<'Key,'Result>,'Q>   = 

    //member __.SortBy (source: HiveQueryable<'T>, keySelector : 'T -> 'Key) : HiveQueryable<'T> when 'Key : equality and 'Key : comparison  = 
    //member __.SortByDescending (source: HiveQueryable<'T>, keySelector : 'T -> 'Key) : HiveQueryable<'T> when 'Key : equality and 'Key : comparison  = 
    //member __.ThenBy (source: HiveQueryable<'T>, keySelector : 'T -> 'Key) : HiveQueryable<'T> when 'Key : equality and 'Key : comparison = 
    //member __.ThenByDescending (source: HiveQueryable<'T>, keySelector : 'T -> 'Key) : HiveQueryable<'T> when 'Key : equality and 'Key : comparison  = 
    //member __.SortByNullable (source: HiveQueryable<'T>, keySelector : 'T -> Nullable<'Key>) : HiveQueryable<'T> when 'Key : equality and 'Key : comparison  = 
    //member __.SortByNullableDescending (source: HiveQueryable<'T>, keySelector : 'T -> Nullable<'Key>) : HiveQueryable<'T> when 'Key : equality and 'Key : comparison  = 
    //member __.ThenByNullable (source: HiveQueryable<'T>, keySelector : 'T -> Nullable<'Key>) : HiveQueryable<'T> when 'Key : equality and 'Key : comparison = 
    //member __.ThenByNullableDescending (source:  HiveQueryable<'T>, keySelector : 'T -> Nullable<'Key>) : HiveQueryable<'T> when 'Key : equality and 'Key : comparison  = 
    //member __.Join  (outerSource: HiveQueryable<_,'Q>, innerSource: HiveQueryable<_,'Q>, outerKeySelector, innerKeySelector, elementSelector) : HiveQueryable<_,'Q> = 
    //member __.GroupJoin (outerSource: HiveQueryable<_,'Q>, innerSource: HiveQueryable<_,'Q>, outerKeySelector, innerKeySelector, elementSelector: _ ->  seq<_> -> _) : HiveQueryable<_,'Q> = 
    //member __.LeftOuterJoin (outerSource:HiveQueryable<_,'Q>, innerSource: HiveQueryable<_,'Q>, outerKeySelector, innerKeySelector, elementSelector: _ ->  seq<_> -> _) : HiveQueryable<_,'Q> = 

    [<CustomOperation("writeRows", MaintainsVariableSpace = true)>]
    /// Write to an existing table, overwriting any previous rows in the table 
    member __.WriteRows(source : HiveQueryable<'T>, [<ProjectionParameter>]selector : 'T -> 'U) : HiveQueryable<'T> = ignore (source,selector); failwith "The 'writeRows' operator may only be executed on the server"


    [<CustomOperation("writePartition", MaintainsVariableSpace = true)>]
    /// Write to an existing partition, overwriting any previous rows in the partition
    member __.WritePartition(source : HiveQueryable<'T>, [<ProjectionParameter>]selector : 'T -> 'U) : HiveQueryable<'T> = ignore (source,selector); failwith "The 'writeRows' operator may only be executed on the server"

    [<CustomOperation("insertRows", MaintainsVariableSpace = true)>]
    /// Insert new rows into an existing table
    member __.InsertRows(source : HiveQueryable<'T>, [<ProjectionParameter>]selector : 'T -> 'U) : HiveQueryable<'T> = ignore (source,selector); failwith "The 'insertRows' operator may only be executed on the server"

    [<CustomOperation("newTable")>]
    /// Write new rows to a new intermediate table. Any previous table with this name is dropped and overwritten.
    member __.NewTable(source : HiveQueryable<'T>, tableName: string, [<ProjectionParameter>]selector : 'T -> 'U) : HiveTable<'U> = ignore (source, tableName, selector); failwith "The 'newTable' operator may only be executed on the server"

    // Taken this out for now, it is not 'local' - it is on the head node - and the file doesn't
    // actually seem to get written correctly.
    //
    // [<CustomOperation("writeLocalFile")>]
    // /// Write the new rows to a local file
    // member __.WriteLocalFile(source : HiveQueryable<'T>, fileName: string, [<ProjectionParameter>]selector : 'T -> 'U) : HiveQueryable<'U> = ignore (source); failwith "The 'writeLocalFile' operator may only be executed on the server"

    [<CustomOperation("writeDistributedFile", MaintainsVariableSpace = true)>]
    /// Write the selection to a distributed file
    member __.WriteDistributedFile(source : HiveQueryable<'T>, fileName: string, [<ProjectionParameter>]selector : 'T -> 'U) : HiveQueryable<'T> = ignore (source,fileName,selector); failwith "The 'writeLocalFile' operator may only be executed on the server"

    member __.Yield(_x:'T) : 'T = failwith "The 'yield' operator may only be executed on the server"
    member __.Quote() = ()
 
    // This is the primary entry point to the query implementation. The value 'q' is the desugared
    // quotation for the contents of hiveQuery { ... }. We must use Hive to 'eval' this quotation.
    member sp.Run(q:Expr<'T>) : 'T =  
        
        // First, analyze the quotation and extract the query.
        let qCtxt, qQueryData, qTailOps, qTimeout, _qAuxExprs = decodeQuery None true q

        let resTy = typeof<'T> 
        // Is the result of the query a HiveTable<_>? If so, create the table object. We have
        // to use reflection to do this to set the right value for the type parameter.
        if resTy.IsGenericType && resTy.GetGenericTypeDefinition() = typedefof<HiveTable<_>> && (match qQueryData with Table _ -> true | _ -> false ) then 
            // wrap it up - 'T is HiveTable<_>
            let tableName, colData = colDataOfQuery qQueryData
            resTy.GetConstructors(BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Instance).[0].Invoke([|qCtxt; tableName; colData|]) |> box |> unbox 

        // Is the result of the query a HiveQueryable<_>? If so, create the queryable object. 
        elif resTy.IsGenericType && resTy.GetGenericTypeDefinition() = typedefof<HiveQueryable<_>> then 
            // wrap it up - 'T is HiveQueryable<_>
            resTy.GetConstructors(BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Instance).[0].Invoke([|qCtxt; qQueryData; qTailOps; (None: int option)|]) |> box |> unbox 

        // Otherwise, this is th result of an aggregation operator and we execute the query immediately
        else 
            qCtxt.ExecuteQueryValue(qQueryData, ?timeout=qTimeout) |> unbox


/// For unitized properties we must provide a method call GetColumnValueFromRow<float<kg>>(obj,string)
/// However, units of measure can'T be used in MakeGenericMethod on the generic method info. Hence we 
/// build our own provided method info that returns the right information. This could be generalized and placed
/// in the ProvidedTypes api. The current implementation is approximate because it only works for the case where
/// the return type is T and T is not used anywhere in the parameter types.
//
// CLEANUP: fold this into ProvidedTypes API
type internal ProvidedApproxSymbolMethod(gmd: MethodInfo, returnType) =
    inherit MethodInfo()

    // Implement overloads
    override this.GetParameters() = gmd.GetParameters()
    override this.Attributes = gmd.Attributes
    override this.Name = gmd.Name
    override this.DeclaringType = gmd.DeclaringType
    override this.IsDefined(attributeType, ``inherit``) = gmd.IsDefined(attributeType, ``inherit``)
    override this.MemberType = gmd.MemberType
    override this.CallingConvention = gmd.CallingConvention
    override this.ReturnType = returnType
    override this.ReturnParameter = null 
    override this.ToString() = gmd.ToString()

    override this.GetGenericMethodDefinition() = gmd
    override this.GetGenericArguments() = [| returnType |]
    override this.IsGenericMethod = true
    override this.IsGenericMethodDefinition = false
    override this.ReturnTypeCustomAttributes                           = gmd.ReturnTypeCustomAttributes
    override this.GetBaseDefinition()                                  = gmd.GetBaseDefinition()
    override this.GetMethodImplementationFlags()                       = gmd.GetMethodImplementationFlags()
    override this.MethodHandle                                         = gmd.MethodHandle
    override this.MetadataToken                                        = gmd.MetadataToken
    override this.Invoke(obj,invokeAttr,binder,parameters,culture)     = gmd.Invoke(obj,invokeAttr,binder,parameters,culture)
    override this.ReflectedType                                        = gmd.ReflectedType
    override this.GetCustomAttributes(``inherit``)                     = gmd.GetCustomAttributes(``inherit``)
    override this.GetCustomAttributes(attributeType, ``inherit``)      = gmd.GetCustomAttributes(attributeType, ``inherit``)
                                                                        
[<TypeProvider>]
type HiveTypeProviderImplementation(_config: TypeProviderConfig) as this = 
    inherit TypeProviderForNamespaces()

    let ns = "Samples.Hadoop"

    let asm = Assembly.GetExecutingAssembly()

    /// Translate information about a unit of measure into F# unit type annotations
    let rec translateUnit u =
        match u with 
        | Unit.SI s -> ProvidedMeasureBuilder.Default.SI s
        | Unit.Prod(u1,u2) -> ProvidedMeasureBuilder.Default.Product (translateUnit u1, translateUnit u2)
        | Unit.Div(u1,u2) -> ProvidedMeasureBuilder.Default.Ratio (translateUnit u1, translateUnit u2)
        | Unit.One -> ProvidedMeasureBuilder.Default.One

    /// Convert an AnyUnit into an SI Unit
    let rec anyUnitToSI unit =
        match unit with
        | AnyUnit.Unknown r -> 
            match r.Value with 
            | None -> failwith "unknown unit of measure in Hive type (anyUnitToSI)"
            | Some soln -> anyUnitToSI soln 
        | AnyUnit.Unit unitString ->
            let (measureAnnotation,_multiplier,_offset) = UnitsOfMeasure.unitSearch(unitString)
            measureAnnotation
        | AnyUnit.One -> Unit.One
        | AnyUnit.Prod (u1,u2) -> Unit.Prod(anyUnitToSI u1, anyUnitToSI u2)
        | AnyUnit.Inv u -> Unit.Div(Unit.One, anyUnitToSI u)

    /// Add a unit of measure annotation to a type as an SI unit of measure
    let unitize unit ty =
        match anyUnitToSI unit with
        | Unit.One -> ty
        | u -> ProvidedMeasureBuilder.Default.AnnotateType(ty,[translateUnit u])


#if NO_PROXY_FOR_HIVE
    let memoizedFetcher (server, port, auth, uid, pwd, metadataTimeout) req = 
        Hive.HiveRequestInProcess(server, port, auth, uid, pwd, metadataTimeout, req)
#else
    let memoizedFetcher = memoize (fun param -> makeFetcher<HiveRequest,HiveResult> param)
#endif
    let memoizedGetTables = memoize (fun param -> memoizedFetcher(param)(GetTableNames))
    let memoizedGetTableDescription = memoize (fun (param,tableName) -> memoizedFetcher(param)(GetTableDescription(tableName)))
    let memoizedGetTableSchema = memoize (fun (param,tableName) -> memoizedFetcher(param)(GetTableSchema(tableName)))

    let hiveTy = 
        let hiveTyped = ProvidedTypeDefinition(asm, ns, "HiveTypeProvider", Some(typeof<obj>), HideObjectMethods = true)
        let serverArg = ProvidedStaticParameter("Server", typeof<string>)
        let portArg = ProvidedStaticParameter("Port", typeof<int>,10000)
        let authArg = ProvidedStaticParameter("Authentication", typeof<int>,-1)
        let uidArg  = ProvidedStaticParameter("UserName", typeof<string>,"")
        let pwdArg  = ProvidedStaticParameter("Password", typeof<string>,"")
        let unitsArg = ProvidedStaticParameter("UseUnitAnnotations", typeof<bool>,false)
        let reqArg   = ProvidedStaticParameter("UseRequiredAnnotations", typeof<bool>,true)
        let queryTimeoutArg  = ProvidedStaticParameter("DefaultQueryTimeout", typeof<int>, 60)
        let metadataTimeoutArg  = ProvidedStaticParameter("DefaultMetadataTimeout", typeof<int>, 5)
        let helpText = "<summary>Typed representation of the Hive Tables</summary>
                        <param name='Server'>The Hive connection address</param>
                        <param name='Port'>The Hive connection port</param>
                        <param name='UserName'>The Hive connection username</param>
                        <param name='Password'>The Hive connection password</param>
                        <param name='Authentication'>The kind of Hive ODBC authentication to use (3 = integrated)</param>
                        <param name='UseUnitAnnotations'>Use units-of-measure annotations in Hive table descriptions to give annotated F# types (default: false). The annotation is, for example, '(unit=kg)' and must be at the end of the table description.</param>
                        <param name='UseRequiredAnnotations'>Use 'required' annotations in Hive table descriptions to give annotated F# types (default: true). The annotation '(required)' must be at the end of the table description. If a unit annotation is also given, use '(unit=kg, required)'.</param>
                        <param name='DefaultQueryTimeout'>The timeout in seconds used when fetching metadata from the Hive ODBC service at compile-time, and the default timeout for accesses to the Hive ODBC service at runtime (default: 60 seconds).</param>
                        <param name='DefaultMetadataTimeout'>The timeout in seconds used when fetching metadata from the Hive ODBC service at compile-time, and the default timeout for accesses to the Hive ODBC service at runtime (default: 5 seconds).</param>"


#if BROWSER
        let defaultProxyUri = AsyncUtilities.RunOnMainThread(fun () -> System.Windows.Application.Current.Server.Source)
        let defaultProxyPrefix = System.Uri(defaultProxyUri,"/Proxy/hadoophive").ToString()
        let proxy = ProvidedStaticParameter("Proxy", typeof<string>, defaultProxyPrefix) 
        let extraHelpText = "<param name='ProxyUrl'>The proxy for the Hive service (default: " + defaultProxyPrefix + ")</param>"
        do hiveTyped.AddXmlDoc(helpText + extraHelpText)
        do hiveTyped.DefineStaticParameters([serverArg;portArg;authArg;uidArg;pwdArg;unitsArg;reqArg;queryTimeoutArg;metadataTimeoutArg;proxyArg], fun typeName providerArgs -> 
                let server, port, auth, uid, pwd, useUnitAnnotations, useRequiredAnnotations, queryTimeout, metadataTimeout, proxy = 
                    match providerArgs with 
                    | [|  :? string as server; :? int as port; :? int as auth; :? string as uid; :? string as pwd; :? bool as useUnitAnnotations; :? bool as useRequiredAnnotations; :? int as queryTimeout; :? int as metadataTimeout; :? string as proxy |] ->
                       (server, port, auth, uid, pwd, useUnitAnnotations, useRequiredAnnotations, timeout, proxy)
                    | args -> failwithf "unexpected arguments to type provider, got %A" args
#else            
        let extraHelpText = "<param name='CredentialsFile'>A file containing two lines - the Hive connection user name and the Hive connection password</param>"
        let credFileParamArg = ProvidedStaticParameter("CredentialsFile", typeof<string>, "")
        do hiveTyped.AddXmlDoc(helpText + extraHelpText)
        do hiveTyped.DefineStaticParameters([serverArg;portArg;authArg;uidArg;pwdArg;unitsArg;reqArg;queryTimeoutArg;metadataTimeoutArg;credFileParamArg], fun typeName providerArgs ->
                let server,port,auth,uid,pwd,useUnitAnnotations,useRequiredAnnotations,queryTimeout,metadataTimeout,credFile = 
                    match providerArgs with 
                    | [| :? string as server; 
                         :? int as port; 
                         :? int as auth; 
                         :? string as uid; 
                         :? string as pwd; 
                         :? bool as useUnitAnnotations; 
                         :? bool as useRequiredAnnotations; 
                         :? int as queryTimeout; 
                         :? int as metadataTimeout; 
                         :? string as credFile |]  -> 
                        (server,port,auth,uid,pwd,useUnitAnnotations,useRequiredAnnotations,queryTimeout,metadataTimeout,credFile)
                    | args -> 
                        failwithf "unexpected arguments to type provider, got %A" args
                let queryTimeout = queryTimeout * 1<s>
                let metadataTimeout = metadataTimeout * 1<s>
                let proxy = 0
                let uid,pwd = 
                    if String.IsNullOrWhiteSpace credFile then 
                        uid,pwd 
                    else
                        if not (String.IsNullOrWhiteSpace uid) || not (String.IsNullOrWhiteSpace pwd) then 
                            failwith "either one of CredentialFile or a UserName/Password pair may be specified"
                        let lines = System.IO.File.ReadAllLines credFile |> Array.filter (fun s -> not (String.IsNullOrWhiteSpace s))
                        if lines.Length < 2 then failwith "the credentials file '%s' did not contain two lines" credFile
                        lines.[0], lines.[1]

#endif          
#if NO_PROXY_FOR_HIVE
                let param = (server, port, auth, uid, pwd, metadataTimeout)
#else
                let param = RuntimeHelpers.formatParams(proxy,server,port,auth,uid,pwd,metadataTimeout)
#endif


                let root = ProvidedTypeDefinition(asm, ns, typeName, baseType = Some typeof<obj>, HideObjectMethods = true) 
                let containerForRowTypes = ProvidedTypeDefinition("DataTypes", baseType = Some typeof<obj>, HideObjectMethods = true) 
                root.AddMember(containerForRowTypes)

                let dataServiceType = ProvidedTypeDefinition("DataService", baseType = Some typeof<obj>, HideObjectMethods = true) 
                containerForRowTypes.AddMember(dataServiceType)
                dataServiceType.AddXmlDoc "Represents the connection configuration for accessinng a Hadoop/Hive data store"
                dataServiceType.AddMembersDelayed (fun () -> 
                    let tableNames = 
                        memoizedGetTables param |> function 
                            | TableNames(names) -> names
                            | data -> RuntimeHelpers.unexpected "TableNames" data
                    let rowTypes, tableTypes = 
                        [ for tableName in tableNames do
                            let rowType = ProvidedTypeDefinition(tableName, Some typeof<HiveDataRow>, HideObjectMethods = true) 
                            let tableColumnInfoLazy = 
                              lazy 
                                let tableInfo = 
                                    match memoizedGetTableSchema(param,(tableName,useUnitAnnotations)) with
                                    | TableSchema(tableInfo) -> tableInfo
                                    | data -> RuntimeHelpers.unexpected "TableDescription" data
                                let exactColumnTypes = 
                                    [ for col in tableInfo.Columns do 
                                        let erasedColumnTypeWithoutNullable = computeErasedTypeWithoutNullable(col.HiveType)
                                        let exactColumnTypeWithoutNullable = 
                                            if useUnitAnnotations then 
                                                match col.HiveType with 
                                                | DSingle u | DDouble u | DDecimal u -> unitize u erasedColumnTypeWithoutNullable
                                                | _ -> erasedColumnTypeWithoutNullable // no units on integers, booleans or strings
                                            else
                                                erasedColumnTypeWithoutNullable
                                        let exactColumnType = 
                                            // 'string' and other reference types are already nullable in .NET land
                                            if (useRequiredAnnotations && col.IsRequired) || not exactColumnTypeWithoutNullable.IsValueType then 
                                                exactColumnTypeWithoutNullable
                                            else
                                                ProvidedTypeBuilder.MakeGenericType(typedefof<Nullable<_>>, [ exactColumnTypeWithoutNullable ])
                                        yield  exactColumnType ]
                                (tableInfo, exactColumnTypes)

                            rowType.AddMembersDelayed (fun () -> 
                                let props = 
                                    let tableInfo, exactColumnTypes = tableColumnInfoLazy.Force()
                                    [ for col, exactColumnType in Seq.zip tableInfo.Columns exactColumnTypes do 
                                        let p = 
                                            ProvidedProperty
                                                (propertyName = col.HiveName, propertyType = exactColumnType, IsStatic=false, 
                                                 GetterCode= (fun args -> 
                                                    let meth = ProvidedApproxSymbolMethod(HiveDataRow.GetValueMethodInfo, exactColumnType)
                                                    let hqQueryData = Expr.Call(args.[0], meth, [Expr.Value(col.HiveName)])
                                                    hqQueryData))
                                        let colDesc = 
                                            if String.IsNullOrWhiteSpace col.Description || col.Description = "null"  then 
                                                "The column " + col.HiveName + " in the table " + tableName + " in the Hive metastore." 
                                            else col.Description + "."
                                        let isPartitionKey = tableInfo.PartitionKeys |> Array.exists (fun x -> x = col.HiveName)
                                        let isBucketKey = tableInfo.BucketKeys |> Array.exists (fun x -> x = col.HiveName)
                                        let isSortKey = tableInfo.SortKeys |> Array.exists (fun x -> x.Column = col.HiveName)
                                        let colDesc = colDesc + (if isPartitionKey then " The column is a partition key." else "")
                                        let colDesc = colDesc + (if isBucketKey then " The column is a bucket key." else "")
                                        let colDesc = colDesc + (if isSortKey then " The column is a sort key." else "")
                                        p.AddXmlDoc colDesc
                                        yield p  ]
                                props)
                            let tableBaseType = typedefof<HiveTable<_>>.MakeGenericType [| (rowType :> System.Type) |]
                            let tableType = ProvidedTypeDefinition(tableName+"Table", Some tableBaseType, HideObjectMethods = true) 
                            tableType.AddMembersDelayed (fun () -> 
                                let tableInfo, exactColumnTypes = tableColumnInfoLazy.Force()
                                let ctorArgs = 
                                    [ for col, exactColumnType in Seq.zip tableInfo.Columns exactColumnTypes do 
                                       yield ProvidedParameter(parameterName = col.HiveName, parameterType = exactColumnType,?optionalValue= (if col.IsRequired then None else Some null)) ]
                                let ctor = 
                                    ProvidedMethod("NewRow",
                                                   ctorArgs,
                                                   rowType,
                                                   IsStaticMethod=false,
                                                   InvokeCode= (fun args -> 
                                                        let hqQueryData = Expr.NewObject(HiveDataRow.HiveDataRowCtor, 
                                                                                         [Expr.Value tableName;
                                                                                          Expr.NewArray(typeof<string * obj * bool>,
                                                                                                       [ for x,(arg,col) in (Seq.zip ctorArgs (Seq.zip (List.tail args) tableInfo.Columns)) -> 
                                                                                                           let isPartitionKey = tableInfo.PartitionKeys |> Array.exists (fun x -> x = col.HiveName)
                                                                                                           Expr.NewTuple [Expr.Value x.Name; Expr.Coerce(arg,typeof<obj>); Expr.Value isPartitionKey]])])
                                                        // TODO: bind the XML doc for the various parameters
                                                        hqQueryData))
                                [ctor])
                            yield rowType,(tableType,tableColumnInfoLazy) ]
                      |> List.unzip

                    containerForRowTypes.AddMembers(List.map fst tableTypes)
                    containerForRowTypes.AddMembers(rowTypes)

                    let dcProp = 
                        ProvidedProperty(propertyName = "DataContext", propertyType = typeof<HiveDataContext>, 
                                         GetterCode= (fun args -> Expr.Coerce(args.[0], typeof<HiveDataContext>)))
                    dcProp.AddXmlDocDelayed(fun () -> "The underlying data context for dynamic operations")
                    
                    let tableProps = 
                        [ for (tableName,(tableType,tableColumnInfoLazy)) in Seq.zip tableNames tableTypes do
                                let p = ProvidedProperty(propertyName = tableName, propertyType = tableType, 
                                                        GetterCode= (fun args -> <@@ (%%(Expr.Coerce(args.[0], typeof<HiveDataContext>)) : HiveDataContext).GetTable<HiveDataRow>(tableName) @@>))
                                p.AddXmlDocDelayed(fun () -> 
                                    let tableInfo, _ = tableColumnInfoLazy.Force()
                                    let tableDesc,tableHead = 
                                        match memoizedGetTableDescription(param,(tableName, useUnitAnnotations)) with
                                        | TableDescription(tableDesc,tableHead) -> tableDesc,tableHead
                                        | Exception ex -> sprintf "table '%s'" tableName, sprintf "error %s" ex
                                        | data -> sprintf "table '%s'" tableName, sprintf "unexpected result %A" data

                                    let tablePartitionKeys = 
                                        match tableInfo.PartitionKeys with 
                                        | [| |] -> "No partition keys."
                                        | keys -> "Partition keys: " + String.concat ", " keys + "."
                                    let tableBucketKeys = 
                                        match tableInfo.BucketKeys with 
                                        | [| |] -> "No bucket keys."
                                        | keys -> "Bucket keys: " + String.concat ", " keys + "."
                                    let tableSortKeys = 
                                        match tableInfo.SortKeys with 
                                        | [| |] -> "No sort keys."
                                        | keys -> "Sort keys: " + String.concat ", " (keys |> Array.map (fun x -> x.Column)) + "."
                                    if tableDesc = "" then tableName else (sprintf "<summary>%s<para>%s %s %s</para>%s</summary>" tableDesc tablePartitionKeys tableBucketKeys tableSortKeys tableHead)) 
                                yield p ]
                    dcProp :: tableProps)

                let getDataContextMeth = 
                    ProvidedMethod("GetDataContext",
                                   [ ProvidedParameter("server",typeof<string>,optionalValue=server);
                                     ProvidedParameter("port",typeof<int>,optionalValue=port);
                                     ProvidedParameter("auth",typeof<int>,optionalValue=auth);
                                     ProvidedParameter("uid",typeof<string>,optionalValue=uid);
                                     ProvidedParameter("password",typeof<string>,optionalValue=pwd);
                                     ProvidedParameter("queryTimeout",typeof<int>,optionalValue=queryTimeout);
                                     ProvidedParameter("metadataTimeout",typeof<int>,optionalValue=metadataTimeout)    ],
                                    dataServiceType,
                                    InvokeCode= (fun args -> <@@ new HiveDataContext(proxy,(%%args.[0] : string),(%%args.[1] : int),(%%args.[2] : int),(%%args.[3] : string),(%%args.[4] : string),(%%args.[5] : int<s>),(%%args.[6] : int<s>),useUnitAnnotations) @@>),
                                    IsStaticMethod=true)
                root.AddMembers [getDataContextMeth ]
                        
                root)
        hiveTyped
    do this.AddNamespace(ns, [hiveTy])

[<assembly:TypeProviderAssembly>] 
do()

[<AutoOpen>]
module  HiveQueryBuilderStatics = 
    let hiveQuery = HiveQueryBuilder()

