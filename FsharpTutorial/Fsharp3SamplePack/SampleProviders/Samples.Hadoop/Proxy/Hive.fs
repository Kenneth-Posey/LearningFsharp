module Hive

open System
open System.Text
open System.Linq
open System.Collections.Concurrent
open System.Net
open System.Net.Sockets
open System.IO
open Microsoft.FSharp.Control.WebExtensions
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols
open System.Threading
open System.Collections.Generic
open System.Diagnostics
open System.ComponentModel 
open SerDes
open System.IO.Pipes
open System.Text.RegularExpressions
open System.Reflection
open System.Data.Odbc
open HiveSchema


type ServerWrap =
    { server : string
      port : int
      msg  : HiveResult}
    

#if POOL_CONNECTIONS
let connectionDict = new ConcurrentDictionary<(string * int),OdbcConnection>()
#endif

let mockServer = "tryfsharp"

/// We have two different data servers - one in-memory/mock and one Odbc
type IHiveDataServer = 
    inherit System.IDisposable
    abstract GetTableNames : unit -> string[]
    abstract GetTablePartitionNames : tableName: string -> string[]
    abstract GetTableSchema : tableName: string * useUnitAnnotations: bool -> HiveTableSchema
    abstract ExecuteCommand : string -> int
    abstract GetDataFrame : HiveQueryData * HiveColumnSchema[] -> TVal[][] 


#if NO_INCLUDE_MOCK_DATA
#else
type MockDataServer() = 
    let tables = Samples.Hadoop.Internals.Data.EmbeddedData.tables
    let GetTableSchema tableName = tables |> Array.find(fun x -> x.HiveTable = tableName)

    interface IDisposable with 
        member x.Dispose() = ()
    interface IHiveDataServer with 
      member x.GetTableNames() = tables |> Array.map (fun x -> x.HiveTable)
      member x.GetTablePartitionNames(_tableName) = [| |]
      member x.GetTableSchema (tableName, useUnitAnnotations) = 
          let dd = GetTableSchema tableName 
          let columns = 
              dd.HiveColumns |> Array.map (fun (a,b,c,d) -> 
                  // Erase units of measure if unit annotations aren't being used
                  let typ = 
                      match c with 
                      | DSingle _ when useUnitAnnotations -> DSingle One 
                      | DDouble _ when useUnitAnnotations -> DDouble One 
                      | DDecimal _ when useUnitAnnotations -> DDecimal One 
                      | x -> x
                  {HiveName=a;Description=b;HiveType=typ;IsRequired=d})
          {Description=dd.HiveInfo;Columns=columns;PartitionKeys=[||]; BucketKeys=[||];SortKeys=[||] }
      member x.ExecuteCommand (_command)  = 0
      member x.GetDataFrame (query, _colData) = 
          let tableName = getTableName query
          let dd = GetTableSchema tableName                                
          let idxTable = dict (dd.HiveColumns |> Array.mapi (fun i (colName, _colDesc, _datatype, _req) -> (colName, i)))
          let TBool b = VBoolean (Nullable(b))
          let addVals v1 v2 = 
              match v1,v2 with 
              | VInt8 (Value n1), VInt8 (Value n2) -> VInt8 (Value (n1 + n2))
              | VInt16 (Value n1), VInt16 (Value n2) -> VInt16 (Value (n1 + n2))
              | VInt32 (Value n1), VInt32 (Value n2) -> VInt32 (Value (n1 + n2))
              | VInt64 (Value n1), VInt64 (Value n2) -> VInt64 (Value (n1 + n2))
              | VSingle (Value n1), VSingle (Value n2) -> VSingle (Value (n1 + n2))
              | VDouble (Value n1), VDouble (Value n2) -> VDouble (Value (n1 + n2))
              | VDecimal (Value n1), VDecimal (Value n2) -> VDecimal (Value (n1 + n2))
              | VString n1, VString n2 -> VString (n1 + n2)
              | _ -> failwith "can't evaluate addition"
          let getZero t = 
              match t with
              | DInt8 -> VInt8(Nullable 0y)
              | DInt16 -> VInt16(Nullable 0s)
              | DInt32 -> VInt32(Nullable 0)
              | DInt64 -> VInt64(Nullable 0L)
              | DSingle _u -> VSingle(Nullable 0.0f)
              | DDouble _u -> VDouble(Nullable 0.0)
              | DDecimal _u -> VDecimal(Nullable 0.0M)
              | DBoolean -> VBoolean(Nullable false)
              | DString -> VString("")
              | DMap(_arg1,_arg2) -> failwith "nyi: map/array/struct"
              | DArray(_arg) -> failwith "nyi: map/array/struct"
              | DStruct(_args) -> failwith "nyi: map/array/struct"
              | DTable(_args) -> failwith "evaluating whole table as part of client-side expression"
              | DAtom -> failwith "unreachable: atom"
          let rec evalExpr (row:TVal[]) e =
              match e with 
              | ETable (_,_ty) -> failwith "evaluating whole table as part of client-side expression"
              | EQuery q -> 
                  match interp q with 
                  | [| [| v |] |] -> v
                  | _ -> failwith "unexpected non-aggregate nested queries over embedded mock data"
              | EVal (v,_ty) -> v
              | EBinOp(e1,op,e2,_ty) ->  
                  let v1 = evalExpr row e1
                  let v2 = evalExpr row e2
                  match op with 
                  | "+" -> addVals v1 v2
                  | "=" -> 
                      let res = 
                          match v1,v2 with 
                          | VInt8 (Value n1), VInt8 (Value n2) -> n1 = n2 
                          | VInt16 (Value n1), VInt16 (Value n2) -> n1 = n2 
                          | VInt32 (Value n1), VInt32 (Value n2) -> n1 = n2 
                          | VInt64 (Value n1), VInt64 (Value n2) -> n1 = n2
                          | VSingle (Value n1), VSingle (Value n2) -> n1 = n2
                          | VDouble (Value n1), VDouble (Value n2) -> n1 = n2
                          | VDecimal (Value n1), VDecimal (Value n2) -> n1 = n2
                          | VBoolean (Value n1), VBoolean (Value  n2) -> n1 = n2
                          | VString n1, VString n2 when n1 <> null && n2 <> null -> n1 = n2
                          | _ -> false
                      TBool res
                  | "!=" -> 
                      let res = 
                          match v1,v2 with 
                          | VInt8 (Value n1), VInt8 (Value n2) -> n1 <> n2 
                          | VInt16 (Value n1), VInt16 (Value n2) -> n1 <> n2 
                          | VInt32 (Value n1), VInt32 (Value n2) -> n1 <> n2 
                          | VInt64 (Value n1), VInt64 (Value n2) -> n1 <> n2
                          | VSingle (Value n1), VSingle (Value n2) -> n1 <> n2
                          | VDouble (Value n1), VDouble (Value n2) -> n1 <> n2
                          | VDecimal (Value n1), VDecimal (Value n2) -> n1 <> n2
                          | VBoolean (Value n1), VBoolean (Value n2) -> n1 <> n2
                          | VString n1, VString n2 when n1 <> null && n2 <> null -> n1 <> n2
                          | _ -> false
                      TBool res
                  | "<" -> 
                      let res = 
                          match v1,v2 with 
                          | VInt8 (Value n1), VInt8 (Value n2) -> n1 < n2
                          | VInt16 (Value n1), VInt16 (Value n2) -> n1 < n2
                          | VInt32 (Value n1), VInt32 (Value n2) -> n1 < n2
                          | VInt64 (Value n1), VInt64 (Value n2) -> n1 < n2
                          | VSingle (Value n1), VSingle (Value n2) -> n1 < n2
                          | VDouble (Value n1), VDouble (Value n2) -> n1 < n2
                          | VDecimal (Value n1), VDecimal (Value n2) -> n1 < n2
                          | _ -> false
                      TBool res
                  | "<=" -> 
                      let res = 
                          match v1,v2 with 
                          | VInt8 (Value n1), VInt8 (Value n2) -> n1 <= n2
                          | VInt16 (Value n1), VInt16 (Value n2) -> n1 <= n2
                          | VInt32 (Value n1), VInt32 (Value n2) -> n1 <= n2
                          | VInt64 (Value n1), VInt64 (Value n2) -> n1 <= n2
                          | VSingle (Value n1), VSingle (Value n2) -> n1 <= n2
                          | VDouble (Value n1), VDouble (Value n2) -> n1 <= n2
                          | VDecimal (Value n1), VDecimal (Value n2) -> n1 <= n2
                          | _ -> false
                      TBool res
                  | ">" -> 
                      let res = 
                          match v1,v2 with 
                          | VInt8 (Value n1), VInt8 (Value n2) -> n1 > n2
                          | VInt16 (Value n1), VInt16 (Value n2) -> n1 > n2
                          | VInt32 (Value n1), VInt32 (Value n2) -> n1 > n2
                          | VInt64 (Value n1), VInt64 (Value n2) -> n1 > n2
                          | VSingle (Value n1), VSingle (Value n2) -> n1 > n2
                          | VDouble (Value n1), VDouble (Value n2) -> n1 > n2
                          | VDecimal (Value n1), VDecimal (Value n2) -> n1 > n2
                          | _ -> false
                      TBool res
                  | ">=" -> 
                      let res = 
                          match v1,v2 with 
                          | VInt8 (Value n1), VInt8 (Value n2) -> n1 >= n2
                          | VInt16 (Value n1), VInt16 (Value n2) -> n1 >= n2
                          | VInt32 (Value n1), VInt32 (Value n2) -> n1 >= n2
                          | VInt64 (Value n1), VInt64 (Value n2) -> n1 >= n2
                          | VSingle (Value n1), VSingle (Value n2) -> n1 >= n2
                          | VDouble (Value n1), VDouble (Value n2) -> n1 >= n2
                          | VDecimal (Value n1), VDecimal (Value n2) -> n1 >= n2
                          | _ -> false
                      TBool res
                  | _ -> failwithf "binary operator '%s' may not yet be used when querying mock data" op
              | EFunc(op,_args,_ty) ->  failwithf "function '%s' may not yet be used when querying mock data" op

              | EColumn (colName,_ty) -> 
                  let idx = idxTable.[colName]
                  row.[idx] 

          and interp q : TVal[][] = 
            match q with 
            | Table(tableName,_) ->
                let dd = GetTableSchema tableName                                
                let data = Assembly.GetExecutingAssembly().GetManifestResourceStream(dd.DataPath)
                let sr = new IO.StreamReader(data)
                let text = sr.ReadToEnd().Replace("\r","")
                let rows = text.Split '\n'
                rows |> Array.map (fun row -> 
                   let cols = row.Split ','
                   (cols,dd.HiveColumns)
                   ||> Array.map2 (fun col (_colName, _colDesc, datatype, _req) ->
                        let tBasic = 
                            match datatype with
                            | DInt8 -> VInt8(match SByte.TryParse(col, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture) with | (false,_) -> Null | (true,x) -> Nullable x)
                            | DInt16 -> VInt16(match Int16.TryParse(col, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture) with | (false,_) -> Null | (true,x) -> Nullable x)
                            | DInt32 -> VInt32(match Int32.TryParse(col, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture) with | (false,_) -> Null | (true,x) -> Nullable x)
                            | DInt64 -> VInt64(match Int64.TryParse(col, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture) with | (false,_) -> Null | (true,x) -> Nullable x)
                            | DSingle u -> VSingle(match Single.TryParse(col, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) with | (false,_) -> Null | (true,x) -> Nullable (unitTransform32 true u x))
                            | DDouble u -> VDouble(match Double.TryParse(col, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) with | (false,_) -> Null | (true,x) -> Nullable (unitTransform64 true u x))
                            | DDecimal u -> VDecimal(match Decimal.TryParse(col, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) with | (false,_) -> Null | (true,x) -> Nullable (unitTransformDecimal true u x))
                            | DBoolean -> VBoolean(match Boolean.TryParse col with | (false,_) -> Null | (true,x) -> Nullable x)
                            | DString -> VString(col)
                            | DMap(_arg1,_arg2) -> failwith "nyi: map/array/struct"
                            | DArray(_arg) -> failwith "nyi: map/array/struct"
                            | DStruct(_args) -> failwith "nyi: map/array/struct"
                            | DTable(_args) -> failwith "evaluating whole table as part of client-side expression"
                            | DAtom -> failwith "unreachable: atom"
                        tBasic))

            | Limit(q2, rowCount) ->
                interp q2
                    |> Seq.truncate rowCount
                    |> Seq.toArray
            | Count(q2) ->
                interp q2
                    |> Seq.length
                    |> fun r -> [| [| VInt64(Nullable<int64>(int64 r)) |] |]

            | Where (q2,e1) -> 
                interp q2 |> Array.filter (fun row -> match evalExpr row e1 with VBoolean (Value true) -> true | _ -> false)

            | Select (q2,colExprs) -> 
                interp q2 |> Array.map (fun row -> colExprs |> Array.map (fun (_,e,_) -> evalExpr row e))
            | AggregateOpBy ("SUM",q2,e,_) -> 
                interp q2 
                    |> Array.map (fun row -> evalExpr row e)
                    |> Array.fold addVals (getZero (typeOfExpr e))
                    |> fun r -> [| [| r |] |]
            | TableSample(q2,n1,n2)  -> 
                interp q2 
                    |> Array.filter (fun row -> uint32 (hash row) % uint32 n2 = uint32 n1)
            | Distinct(q2)  -> 
                interp q2 
                    |> Seq.distinct
                    |> Seq.toArray
            | AggregateOpBy (op,_q2,_e,_isRequired) -> failwith ("operation '" + op + "' not supported on in-memory mock data")
            | GroupBy _ -> failwith ("operation 'groupBy' not supported on in-memory mock data")
                            
          let table = interp query
          table               
#endif

#nowarn "40" // recursive objects

[<AutoOpen>]
module DetailedTableDescriptionParser = 
    open System
    open System.Collections.Generic
    type Parser<'T> = P of (list<char> -> ('T * list<char>) option)

    let result v = P(fun c -> Some (v, c) )
    let zero () = P(fun _c -> None)
    let bind (P p) f = P(fun inp ->
      match p inp with 
      | Some (pr, inp') -> 
               let (P pars) = f pr
               pars inp' 
      | None -> None)
    let plus (P p) (P q) = P (fun inp -> match p inp with Some x -> Some x | None -> q inp )

    let (<|>) p1 p2 = plus p1 p2

    type ParserBuilder() =
      member x.Bind(v, f) = bind v f
      member x.Zero() = zero()
      member x.Return(v) = result(v)
      member x.ReturnFrom(p) = p
      member x.Combine(a, b) = plus a b
      member x.Delay(f) = f()

    let parser = new ParserBuilder()

    /// Accept any character
    let anyChar = P(function | [] -> None | c :: r -> Some (c,r))
    /// Accept any character satisfyin the predicate
    let sat p = parser { let! v = anyChar in if (p v) then return v }
    /// Accept precisely the given character
    let char x = sat ((=) x)
    /// Accept any whitespace character
    let whitespace = sat (Char.IsWhiteSpace)

    /// Accept zero or more repetitions of the given parser
    let rec many p = 
      parser { let! it = p in let! res = many p in return it::res }  
      <|> 
      parser { return [] }

    /// Accept zero or one repetitions of the given parser
    let optional p = 
      parser { let! v = p in return Some v }
      <|> 
      parser { return None }             

    /// Run the given parser
    let apply (P p) (str:seq<char>) = 
      let res = str |> List.ofSeq |> p
      match res with 
      | Some (x,[]) -> Some x
      | _ -> None

    /// Accept one or more repetitions of the given parser with the given character separator followed by whitespace
    let oneOrMoreSep f sep =
        parser { let! first = f
                 let! rest = many (parser { let! _ = char sep in let! _ = many whitespace in let! w = f in return w })
                 return (first :: rest) }

    /// Accept zero or more repetitions of the given parser with the given character separator followed by whitespace
    let zeroOrMoreSep f sep =
        oneOrMoreSep f sep <|> parser { return [] }

    /// Results of parsing the detailed table description
    [<RequireQualifiedAccess>]
    type Data = 
        | Node of string * IDictionary<string,Data>
        | List of Data list
        | Value of string 

    let anyNonDelim lhs = 
        if lhs then sat (fun c -> c <> '=' && c <> '{' && c <> '(' && c <> ')' && c <> ',' && c <> ':' && c <> '[' && c <> ']' && c <> '}' && not (Char.IsWhiteSpace c))
        else sat (fun c -> c <> '(' && c <> '{' && c <> '[' && c <> ')' && c <> ']' && c <> '}' && c <> ',' && not (Char.IsWhiteSpace c))

    let word lhs =
        parser { let! ch = anyNonDelim lhs
                 let! chs = many (anyNonDelim lhs)
                 return String(Array.ofSeq (ch::chs)) } 

    /// Parse either
    //      1023
    //      Foo
    //      Foo(a1:b1,...,an:bn)
    //      [a1,...,an]
    let rec data lhs = 
        parser { let! w = word lhs 
                 let! rest = optional (parser { let! _ = char '('
                                                let! rest = oneOrMoreSep dataField ',' 
                                                let! _ = char ')'
                                                return rest })
                 return (match rest with None -> Data.Value w | Some xs -> Data.Node(w,dict xs)) }
        <|>
        parser { let! xs = dataList
                 return Data.List(xs) }
        <|>
        record
    /// Parse left of 'a:int' or 'a=3' labelled data
    and lhsData = data true
    /// Parse right of 'a:int' or 'a=3' labelled data
    and rhsData = data false
    /// Parse a a1:b1 data field, e.g. in FieldSchema(a:int)
    and dataField = 
        parser { let! w = word(true)
                 let! _ = char ':'
                 let! d = if w = "comment" then commentData elif w = "type" then typeData else rhsData
                 return (w,d) }
    /// Parse a {a1=b1,...,an=bn} record
    and record = 
        parser { let! _ = char '{'
                 let! rest = zeroOrMoreSep recordField ',' 
                 let! _ = char '}'
                 return Data.Node("Record",dict rest) }

    /// Parse a {...a=b...} field
    and recordField = 
        parser { let! w = word(true)
                 let! _ = char '='
                 let! d = if w = "comment" then commentData elif w = "type" then typeData else rhsData
                 return (w,d) }

    /// Parse a comment, which can include balanced parens
    and commentData = delimData  ('(', ')', '{', '}', (fun c -> c <> ')' && c <> '}'))

    /// Parse a type, e.g. 'map<string,string>'
    and typeData = delimData ('<','>', '<', '>', (fun c -> c <> ')' && c <> '}' && c <> ','))

    /// Parse free text where 'startParen' and 'endParen' are balanced and we terminate when 'delim' is false
    and delimData (startParen1,endParen1,startParen2,endParen2,delim) = 
        parser { let! chs = delimText (startParen1,endParen1,startParen2,endParen2,delim) 0
                 return Data.Value(String(Array.ofSeq chs)) } 

    /// Parse free text where 'startParen' and 'endParen' are balanced and we terminate when 'delim' is false
    and delimText (startParen1,endParen1,startParen2,endParen2,endText) parenCount = 
        parser { let! w = (char startParen1 <|> char startParen2)
                 let! d = delimText (startParen1,endParen1,startParen2,endParen2,endText) (parenCount+1)
                 return (w::d) }
        <|>
        parser { let! c = sat (fun c -> parenCount > 0 && (c = endParen1 || c = endParen2))
                 let! d = delimText (startParen1,endParen1,startParen2,endParen2,endText) (parenCount-1)
                 return (c::d) }
        <|>
        parser { let! c = sat (fun c -> parenCount > 0 || (endText c))
                 let! d = delimText (startParen1,endParen1,startParen2,endParen2,endText) parenCount
                 return (c::d) }
        <|>
        parser { return [] }

    /// Parse [FieldSchema(...),...,FieldSchema(...)]
    and dataList = 
        parser { let! _ = char '['
                 let! rest = zeroOrMoreSep lhsData ',' 
                 let! _ = char ']'
                 return rest }

    type IDictionary<'Key,'Value> with 
        member x.TryGet(key) = if x.ContainsKey key then Some x.[key] else None


    let (|ConstructedType|_|) (s:string) = 
        match s.Split('<') with 
        | [| nm; rest |] -> 
            match rest.Split('>') with 
            | [| args; "" |] -> 
                match args.Split( [| ',' |], System.StringSplitOptions.RemoveEmptyEntries) with 
                | [| |] -> None
                | args -> Some (nm,List.ofArray args)
            | _ -> None
        | [| nm |] -> Some(nm,[ ])
        | _ -> None
    //(|ConstructedType|_|) "map<int,int>" = Some ("map", [ "int"; "int" ])
    //(|ConstructedType|_|) "array<int>"= Some ("array", [ "int" ])
    //(|ConstructedType|_|) "array<>"= None
    //(|ConstructedType|_|) "array"= Some("array", [])
    
    let rec parseHiveType (s:string) = 
        match s with
        | "float" -> DSingle One
        | "double" -> DDouble One
        | "decimal" -> DDecimal One
        | "string" -> DString
        | "int" -> DInt32 
        | "tinyint" -> DInt8
        | "smallint" -> DInt16
        | "bigint" -> DInt64
        | "bool" -> DBoolean
        | ConstructedType("map", [arg1; arg2]) -> DMap(parseHiveType arg1, parseHiveType arg2)
        | ConstructedType("array", [arg1]) -> DArray(parseHiveType arg1)
        | _ -> failwith (sprintf "unknown type '%s'" s)

    // Extract the unit and if the field is required (not nullable)
    // Acceptible forms are unit only (g), required only (required), unit and required (g, required)
    // required must be at the end and the expression must be the last part of the string
    let parseHiveColumnAnnotation (desc:string) = 
        let m = Regex.Match(desc,"\(required\)$") 
        if m.Success then (None, true) else 
        let m = Regex.Match(desc,"\(unit=([^\(,)]*)\)$") 
        if m.Success then (Some m.Groups.[1].Value, false) else
        let m = Regex.Match(desc,"\(unit=([^\(,)]*), required\)$") 
        if m.Success then (Some m.Groups.[1].Value, true) else
        (None,false)
    type FieldSchema = { Name: string; Type:DBasic; Comment: string }
    type DetailedTableInfo = 
        { PartitionKeys: FieldSchema[]
          BucketKeys: string[]
          SortKeys: HiveTableSortKey[]
          Columns: FieldSchema[]
          Comment: string }

    /// Get the names of the bucket columns out of the parsed detailed table description
    //bucketCols:[userid]
    let getBucketColumns r = 
       [| match r with 
          | Data.Node("Table",d) ->
              match d.TryGet "bucketCols" with 
              | Some (Data.List d2) -> 
                  for v in d2 do 
                      match v with 
                      | Data.Value(colName) -> yield colName
                      | _ -> ()
              | _ ->  ()
          | _ ->  () |]

    /// Get the names of the sort columns out of the parsed detailed table description
    //sortCols:[Order(col:viewtime, order:1)]
    let getSortColumns r = 
       [| match r with 
          | Data.Node("Table",d) ->
              match d.TryGet "sortCols" with 
              | Some (Data.List d2) -> 
                  for v in d2 do 
                      match v with 
                      | Data.Node("Order",d4) -> 
                            match (d4.TryGet "col", defaultArg (d4.TryGet "order") (Data.Value "1")) with 
                            | Some (Data.Value col), Data.Value order -> 
                              yield {Column=col; Order=order}
                            | _ -> ()
                      | _ ->  ()
              | _ ->  ()
          | _ ->  () |]


    /// Get the partition keys out of the parsed detailed table description
    let getPartitionKeys r = 
       [| match r with 
          | Data.Node("Table",d) ->
              match d.TryGet "partitionKeys" with 
              | Some (Data.List d2) -> 
                  for v in d2 do 
                      match v with 
                      | Data.Node("FieldSchema",d4) -> 
                            match (d4.TryGet "name", d4.TryGet "type", defaultArg (d4.TryGet "comment") (Data.Value "")) with 
                            | Some (Data.Value nm), Some (Data.Value typ), Data.Value comment -> 
                              yield {Name=nm; Type=parseHiveType typ; Comment=comment}
                            | _ -> ()
                      | _ ->  ()
              | _ ->  ()
          | _ ->  () |]

    /// Get the getFields out of the parsed detailed table description
    let getFields r = 
       [| match r with 
          | Data.Node("Table",d) ->
              match d.["sd"] with 
              | Data.Node("StorageDescriptor",d2) ->
                 match d2.TryGet "cols" with 
                 | Some (Data.List cols) ->
                   for col in cols do 
                       match col with 
                       | Data.Node("FieldSchema",d4) -> 
                           match (d4.TryGet "name", d4.TryGet "type", defaultArg (d4.TryGet "comment") (Data.Value "")) with 
                           | Some (Data.Value nm), Some (Data.Value typ), Data.Value comment -> 
                              yield {Name=nm; Type=parseHiveType typ; Comment=comment}
                           | _ -> ()
                       | _ ->  ()
                 | _ ->  ()
              | _ ->  ()
          | _ ->  () |]

    /// Get the 'comment' field out of the parsed detailed table description
    let getComment r = 
        match r with 
        | Data.Node("Table",d) ->
            match d.TryGet "parameters" with 
            | Some (Data.Node(_,d2)) ->
                match d2.TryGet "comment" with 
                | Some (Data.Value comment) -> comment
                | _ ->  ""
            | _ ->  ""
        | _ ->  ""

    let getDetailedTableInfo (text:string) = 
        apply lhsData text 
        |> Option.map (fun r -> 
            { PartitionKeys=getPartitionKeys r; 
              Columns = getFields r 
              SortKeys = getSortColumns r 
              BucketKeys = getBucketColumns r 
              Comment=getComment r})

#if TESTS
    let r = apply lhsData "Table(tableName:abalone)"
    let r = apply lhsData "Table(tableName:abalone,dbName:default)"
    let r = apply lhsData "Table(tableName:abalone,dbName:default,owner:hive)"
    let r = apply lhsData "Table(tableName:abalone, dbName:default, owner:hive)"
    let r = apply lhsData "Table(tableName:abalone, dbName:default, owner:hive, createTime:1360614282)"
    let r = apply lhsData "Table(tableName:abalone, dbName:default, owner:hive, createTime:1360614282, lastAccessTime:0)"
    let r = apply lhsData "Table(tableName:abalone, dbName:default, owner:hive, createTime:1360614282, lastAccessTime:0, retention:0)"
    let r = apply lhsData "Table(sd:StorageDescriptor(cols:[]))"
    let r = apply lhsData "StorageDescriptor(cols:[])"
    let r = apply rhsData "StorageDescriptor(cols:[])"
    let r = apply lhsData "StorageDescriptor(cols:a)"
    let r = apply rhsData "StorageDescriptor"
    let r = apply lhsData "[]"
    let r = apply dataList "[]"
    let r = apply dataList "[1]"
    let r = apply (zeroOrMoreSep lhsData ',') "[]"
    let r = apply (oneOrMoreSep lhsData ',') "a"
    let r = apply (zeroOrMoreSep lhsData ',') "a"
    let r = apply (zeroOrMoreSep lhsData ',') ""
    let r = apply dataList "[a]"
    let r = apply (parser { let! _ = char '[' in let! _ = char ']' in return 1 }) "[]"
    let r = apply (parser { let! _ = char '[' in let! _ = lhsData in let! _ = char ']' in return 1 }) "[1]"
    let r = apply lhsData "Table(sd:StorageDescriptor(cols:[FieldSchema(name:sex, type:string)]))"
    let r = apply lhsData "Table(sd:StorageDescriptor(cols:[FieldSchema(name:sex,type:string)]))"
    let r = apply lhsData "Table(sd:StorageDescriptor(cols:[FieldSchema(name:sex, type:string, comment:M)]))"
    let r = apply lhsData "Table(sd:StorageDescriptor(cols:[FieldSchema(name:sex, type:string, comment:M (unit=1))]))"
    let r = apply lhsData "Table(sd:StorageDescriptor(cols:[FieldSchema(name:sex, type:string, comment:M foo bar)]))"
    let r = apply commentData "M"
    let r = apply commentData "M, I"
    let r = apply commentData "M (infant)"
    let r = apply commentData "M (infant)"
    let r = apply commentData "M (infant)"
    let r = apply commentData "M (infant)"
    let r = apply lhsData "Table(sd:StorageDescriptor(cols:[FieldSchema(name:sex, type:string, comment:M)]))"


    let r = apply lhsData "Table(sd:StorageDescriptor(cols:[FieldSchema(name:sex, type:string, comment:M (infant))]))"
    let r = apply rhsData "FieldSchema(name:sex, type:string, comment:M infant)"
    let r = apply rhsData "FieldSchema(type:string, comment:M (infant))"


    let r = apply lhsData "Table(sd:StorageDescriptor(cols:[FieldSchema(name:sex, type:string, comment:M, I (infant))]))"

    let r = apply lhsData "Table(sd:StorageDescriptor(location:hdfs://0.0.0.0:8020/Data/Abalone))"
    let r = apply lhsData "Table(sd:StorageDescriptor(inputFormat:org.apache.hadoop.mapred.TextInputFormat))"
    let r = apply lhsData "Table(sd:StorageDescriptor(numBuckets:-1))"
    let r = apply recordField "serialization=44"
    let r = apply lhsData "{serialization=44}"
    let r = apply lhsData "{serialization.format=44}"
    let r = apply lhsData "{serialization.format=44, field.delim=44}"
    let r = apply lhsData "{}"
    let r = apply commentData "<para>Abalone data</para><para>Number of Instances: 4177</para><para>Predicting the age of abalone from physical measurements.  The age of abalone is determined by cutting the shell through the cone, staining it, and counting the number of rings through a microscope -- a boring and time-consuming task.  Other measurements, which are easier to obtain, are used to predict the age.</para>"
    let r = apply commentData "<para>Abalone data</para><para>Number of Instances: 4177</para><para>Predicting the age of abalone from physical measurements.  The age of abalone is determined by cutting the shell through the cone, staining it, and counting the number of rings through a microscope -- a boring and time-consuming task.  Other measurements, which are easier to obtain, are used to predict the age.</para>"
    let r = apply lhsData "{EXTERNAL=TRUE, transient_lastDdlTime=1360614282}"
    let r = apply lhsData "{EXTERNAL=TRUE, transient_lastDdlTime=1360614282, comment=<para>Abalone data</para><para>Number of Instances: 4177</para><para>Predicting the age of abalone from physical measurements.  The age of abalone is determined by cutting the shell through the cone, staining it, and counting the number of rings through a microscope -- a boring and time-consuming task.  Other measurements, which are easier to obtain, are used to predict the age.</para>}"
    let r = apply lhsData "Table(parameters:{EXTERNAL=TRUE, transient_lastDdlTime=1360614282, comment=<para>Abalone data</para><para>Number of Instances: 4177</para><para>Predicting the age of abalone from physical measurements.  The age of abalone is determined by cutting the shell through the cone, staining it, and counting the number of rings through a microscope -- a boring and time-consuming task.  Other measurements, which are easier to obtain, are used to predict the age.</para>})"
    let r = apply lhsData "Table(sd:StorageDescriptor(serdeInfo:SerDeInfo(name:null, serializationLib:org.apache.hadoop.hive.serde2.lazy.LazySimpleSerDe, parameters:{serialization.format=44, field.delim=44})))"
    let r = apply lhsData "Table(tableName:abalone, 
          dbName:default, 
          owner:hive, 
          createTime:1360614282, 
          tableType:EXTERNAL_TABLE)"
    let r = apply lhsData "Table(tableName:abalone, 
          getPartitionKeys:[FieldSchema(name:dt, type:string, comment:null), 
                         FieldSchema(name:country, type:string, comment:null)],
          tableType:EXTERNAL_TABLE)"

    let r = apply lhsData "Table(tableName:abalone, 
          parameters:{EXTERNAL=TRUE, 
                      transient_lastDdlTime=1360614282, 
                      comment=<para>Abalone data</para><para>Number of Instances: 4177</para><para>Predicting the age of abalone from physical measurements.  The age of abalone is determined by cutting the shell through the cone, staining it, and counting the number of rings through a microscope -- a boring and time-consuming task.  Other measurements, which are easier to obtain, are used to predict the age.</para>}, 
          tableType:EXTERNAL_TABLE)"

    let r = apply lhsData "Table(tableName:abalone, 
          getPartitionKeys:[FieldSchema(name:dt, type:string, comment:null), 
                         FieldSchema(name:country, type:string, comment:null)],
          parameters:{EXTERNAL=TRUE, 
                      transient_lastDdlTime=1360614282, 
                      comment=<para>Abalone data</para><para>Number of Instances: 4177</para><para>Predicting the age of abalone from physical measurements.  The age of abalone is determined by cutting the shell through the cone, staining it, and counting the number of rings through a microscope -- a boring and time-consuming task.  Other measurements, which are easier to obtain, are used to predict the age.</para>}, 
          tableType:EXTERNAL_TABLE)"

    let r = apply lhsData "Table(tableName:abalone, 
          sd:StorageDescriptor(location:hdfs://0.0.0.0:8020/Data/Abalone, 
                                inputFormat:org.apache.hadoop.mapred.TextInputFormat, 
                                outputFormat:org.apache.hadoop.hive.ql.io.HiveIgnoreKeyTextOutputFormat, 
                                compressed:false, 
                                numBuckets:-1, 
                                serdeInfo:SerDeInfo(name:null, 
                                                    serializationLib:org.apache.hadoop.hive.serde2.lazy.LazySimpleSerDe, 
                                                    parameters:{serialization.format=44, field.delim=44}), 
                                bucketCols:[], 
                                sortCols:[], 
                                parameters:{}))"

    let r = apply lhsData "Table(tableName:abalone, 
          sd:StorageDescriptor(cols:[FieldSchema(name:sex, type:string, comment:M, F, and I (infant)  (required)), 
                                     FieldSchema(name:length, type:double, comment:Longest shell measurement (unit=mm, required)), 
                                     FieldSchema(name:diameter, type:double, comment:Diameter perpendicular to length (unit=mm, required)), 
                                     FieldSchema(name:height, type:double, comment:Height with meat in shell (unit=mm, required)), 
                                     FieldSchema(name:wholeweight, type:double, comment:hole abalone (unit=g, required)), 
                                     FieldSchema(name:shuckedweight, type:double, comment:weight of meat (unit=g, required)), 
                                     FieldSchema(name:visceraweight, type:double, comment:gut weight (after bleeding) (unit=g, required)), 
                                     FieldSchema(name:shellweight, type:double, comment: after being dried (unit=g, required)), 
                                     FieldSchema(name:rings, type:int, comment:+1.5 gives the age in years (required))]))"


    let r = getDetailedTableInfo "Table(tableName:abalone, 
          dbName:default, 
          owner:hive, 
          createTime:1360614282, 
          lastAccessTime:0, 
          retention:0, 
          sd:StorageDescriptor(cols:[FieldSchema(name:sex, type:string, comment:M, F, and I (infant)  (required)), 
                                     FieldSchema(name:length, type:double, comment:Longest shell measurement (unit=mm, required)), 
                                     FieldSchema(name:diameter, type:double, comment:Diameter perpendicular to length (unit=mm, required)), 
                                     FieldSchema(name:height, type:double, comment:Height with meat in shell (unit=mm, required)), 
                                     FieldSchema(name:wholeweight, type:double, comment:hole abalone (unit=g, required)), 
                                     FieldSchema(name:shuckedweight, type:double, comment:weight of meat (unit=g, required)), 
                                     FieldSchema(name:visceraweight, type:double, comment:gut weight (after bleeding) (unit=g, required)), 
                                     FieldSchema(name:shellweight, type:double, comment: after being dried (unit=g, required)), 
                                     FieldSchema(name:rings, type:int, comment:+1.5 gives the age in years (required))], 
                                location:hdfs://0.0.0.0:8020/Data/Abalone, 
                                inputFormat:org.apache.hadoop.mapred.TextInputFormat, 
                                outputFormat:org.apache.hadoop.hive.ql.io.HiveIgnoreKeyTextOutputFormat, 
                                compressed:false, 
                                numBuckets:-1, 
                                serdeInfo:SerDeInfo(name:null, 
                                                    serializationLib:org.apache.hadoop.hive.serde2.lazy.LazySimpleSerDe, 
                                                    parameters:{serialization.format=44, field.delim=44}), 
                                bucketCols:[], 
                                sortCols:[], 
                                parameters:{}), 
          getPartitionKeys:[FieldSchema(name:dt, type:string, comment:null), 
                         FieldSchema(name:country, type:string, comment:null)],
          parameters:{EXTERNAL=TRUE, 
                      transient_lastDdlTime=1360614282, 
                      comment=<para>Abalone data</para><para>Number of Instances: 4177</para><para>Predicting the age of abalone from physical measurements.  The age of abalone is determined by cutting the shell through the cone, staining it, and counting the number of rings through a microscope -- a boring and time-consuming task.  Other measurements, which are easier to obtain, are used to predict the age.</para>}, 
          viewOriginalText:null, 
          viewExpandedText:null, 
          tableType:EXTERNAL_TABLE)"

    let r = apply lhsData "Table(tableName:page_view, dbName:default, owner:hive, createTime:1360701065, lastAccessTime:0, retention:0)"
    let r = apply lhsData "Table(tableName:page_view, dbName:default, owner:hive, createTime:1360701065, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:viewtime, type:int, comment:null)]))"
    let r = getDetailedTableInfo "Table(tableName:page_view, dbName:default, owner:hive, createTime:1360701065, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:viewtime, type:int, comment:null)]))"
    let r = getDetailedTableInfo "Table(tableName:page_view, dbName:default, owner:hive, createTime:1360701065, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:viewtime, type:int, comment:null)], location:hdfs://0.0.0.0:8020/user/hive/warehouse/page_view, inputFormat:org.apache.hadoop.mapred.SequenceFileInputFormat, outputFormat:org.apache.hadoop.hive.ql.io.HiveSequenceFileOutputFormat, compressed:false, numBuckets:32))"
    let r = getDetailedTableInfo "Table(tableName:page_view, dbName:default, owner:hive, createTime:1360701065, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:viewtime, type:int, comment:null), FieldSchema(name:userid, type:bigint, comment:null), FieldSchema(name:page_url, type:string, comment:null), FieldSchema(name:referrer_url, type:string, comment:null), FieldSchema(name:friends, type:array<bigint>, comment:null), FieldSchema(name:properties, type:string, comment:null), FieldSchema(name:ip, type:string, comment:IP Address of the User), FieldSchema(name:dt, type:string, comment:null), FieldSchema(name:country, type:string, comment:null)], location:hdfs://0.0.0.0:8020/user/hive/warehouse/page_view, inputFormat:org.apache.hadoop.mapred.SequenceFileInputFormat, outputFormat:org.apache.hadoop.hive.ql.io.HiveSequenceFileOutputFormat, compressed:false, numBuckets:32))"
    let r = getDetailedTableInfo "Table(tableName:page_view, dbName:default, owner:hive, createTime:1360701065, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:viewtime, type:int, comment:null), FieldSchema(name:userid, type:bigint, comment:null), FieldSchema(name:page_url, type:string, comment:null), FieldSchema(name:referrer_url, type:string, comment:null), FieldSchema(name:friends, type:array<bigint>, comment:null), FieldSchema(name:properties, type:map<string,string>, comment:null), FieldSchema(name:ip, type:string, comment:IP Address of the User), FieldSchema(name:dt, type:string, comment:null), FieldSchema(name:country, type:string, comment:null)], location:hdfs://0.0.0.0:8020/user/hive/warehouse/page_view, inputFormat:org.apache.hadoop.mapred.SequenceFileInputFormat, outputFormat:org.apache.hadoop.hive.ql.io.HiveSequenceFileOutputFormat, compressed:false, numBuckets:32))"

    let r = getDetailedTableInfo "Table(tableName:page_view, dbName:default, owner:hive, createTime:1360701065, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:viewtime, type:int, comment:null), FieldSchema(name:userid, type:bigint, comment:null), FieldSchema(name:page_url, type:string, comment:null), FieldSchema(name:referrer_url, type:string, comment:null), FieldSchema(name:friends, type:array<bigint>, comment:null), FieldSchema(name:properties, type:map<string,string>, comment:null), FieldSchema(name:ip, type:string, comment:IP Address of the User), FieldSchema(name:dt, type:string, comment:null), FieldSchema(name:country, type:string, comment:null)], location:hdfs://0.0.0.0:8020/user/hive/warehouse/page_view, inputFormat:org.apache.hadoop.mapred.SequenceFileInputFormat, outputFormat:org.apache.hadoop.hive.ql.io.HiveSequenceFileOutputFormat, compressed:false, numBuckets:32, serdeInfo:SerDeInfo(name:null, serializationLib:org.apache.hadoop.hive.serde2.lazy.LazySimpleSerDe, parameters:{colelction.delim=2, mapkey.delim=3, serialization.format=1, field.delim=1})))"


    let r = getDetailedTableInfo "Table(tableName:page_view, dbName:default, owner:hive, createTime:1360701065, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:viewtime, type:int, comment:null), FieldSchema(name:userid, type:bigint, comment:null), FieldSchema(name:page_url, type:string, comment:null), FieldSchema(name:referrer_url, type:string, comment:null), FieldSchema(name:friends, type:array<bigint>, comment:null), FieldSchema(name:properties, type:map<string,string>, comment:null), FieldSchema(name:ip, type:string, comment:IP Address of the User), FieldSchema(name:dt, type:string, comment:null), FieldSchema(name:country, type:string, comment:null)], location:hdfs://0.0.0.0:8020/user/hive/warehouse/page_view, inputFormat:org.apache.hadoop.mapred.SequenceFileInputFormat, outputFormat:org.apache.hadoop.hive.ql.io.HiveSequenceFileOutputFormat, compressed:false, numBuckets:32, serdeInfo:SerDeInfo(name:null, serializationLib:org.apache.hadoop.hive.serde2.lazy.LazySimpleSerDe, parameters:{colelction.delim=2, mapkey.delim=3, serialization.format=1, field.delim=1}), bucketCols:[userid], sortCols:[Order(col:viewtime, order:1)], parameters:{}))"

    let r = getDetailedTableInfo "Table(tableName:page_view, dbName:default, owner:hive, createTime:1360701065, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:viewtime, type:int, comment:null), FieldSchema(name:userid, type:bigint, comment:null), FieldSchema(name:page_url, type:string, comment:null), FieldSchema(name:referrer_url, type:string, comment:null), FieldSchema(name:friends, type:array<bigint>, comment:null), FieldSchema(name:properties, type:map<string,string>, comment:null), FieldSchema(name:ip, type:string, comment:IP Address of the User), FieldSchema(name:dt, type:string, comment:null), FieldSchema(name:country, type:string, comment:null)], location:hdfs://0.0.0.0:8020/user/hive/warehouse/page_view, inputFormat:org.apache.hadoop.mapred.SequenceFileInputFormat, outputFormat:org.apache.hadoop.hive.ql.io.HiveSequenceFileOutputFormat, compressed:false, numBuckets:32, serdeInfo:SerDeInfo(name:null, serializationLib:org.apache.hadoop.hive.serde2.lazy.LazySimpleSerDe, parameters:{colelction.delim=2, mapkey.delim=3, serialization.format=1, field.delim=1}), bucketCols:[userid], sortCols:[Order(col:viewtime, order:1)], parameters:{}), getPartitionKeys:[FieldSchema(name:dt, type:string, comment:null), FieldSchema(name:country, type:string, comment:null)])"

    let r = getDetailedTableInfo "Table(tableName:page_view, dbName:default, owner:hive, createTime:1360701065, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:viewtime, type:int, comment:null), FieldSchema(name:userid, type:bigint, comment:null), FieldSchema(name:page_url, type:string, comment:null), FieldSchema(name:referrer_url, type:string, comment:null), FieldSchema(name:friends, type:array<bigint>, comment:null), FieldSchema(name:properties, type:map<string,string>, comment:null), FieldSchema(name:ip, type:string, comment:IP Address of the User), FieldSchema(name:dt, type:string, comment:null), FieldSchema(name:country, type:string, comment:null)], location:hdfs://0.0.0.0:8020/user/hive/warehouse/page_view, inputFormat:org.apache.hadoop.mapred.SequenceFileInputFormat, outputFormat:org.apache.hadoop.hive.ql.io.HiveSequenceFileOutputFormat, compressed:false, numBuckets:32, serdeInfo:SerDeInfo(name:null, serializationLib:org.apache.hadoop.hive.serde2.lazy.LazySimpleSerDe, parameters:{colelction.delim=2, mapkey.delim=3, serialization.format=1, field.delim=1}), bucketCols:[userid], sortCols:[Order(col:viewtime, order:1)], parameters:{}), getPartitionKeys:[FieldSchema(name:dt, type:string, comment:null), FieldSchema(name:country, type:string, comment:null)], parameters:{transient_lastDdlTime=1360701065, comment=This is the page view table}, viewOriginalText:null, viewExpandedText:null, tableType:MANAGED_TABLE)"

    let r = getDetailedTableInfo "Table(tableName:pokerhand, dbName:default, owner:hive, createTime:1360614282, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[]))"
    let r = getDetailedTableInfo "Table(tableName:pokerhand, dbName:default, owner:hive, createTime:1360614282, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:s1, type:int, comment:Suit)]))"
    
    // SUCCEEDS:
    let r = getDetailedTableInfo "Table(tableName:pokerhand, dbName:default, owner:hive, createTime:1360614282, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:s1, type:int, comment:Suit of card #1 Ordinal (1-4) representing (required))]))"
    let r = getDetailedTableInfo "Table(tableName:pokerhand, dbName:default, owner:hive, createTime:1360614282, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:s1, type:int, comment:Suit of card #1 Ordinal (1-4) representing {Hearts, Spades, Diamonds, Clubs} (required))]))"
    let r = getDetailedTableInfo "Table(tableName:pokerhand, dbName:default, owner:hive, createTime:1360614282, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:s1, type:int, comment:Suit of card #1 Ordinal (1-4) representing {Hearts, Spades, Diamonds, Clubs} (required)), FieldSchema(name:c1, type:int, comment:Rank of card #1 Numerical (1-13) representing (Ace, 2, 3, ... , Queen, King) (required))]))"
    let r = getDetailedTableInfo "Table(tableName:pokerhand, dbName:default, owner:hive, createTime:1360614282, lastAccessTime:0, retention:0, sd:StorageDescriptor(cols:[FieldSchema(name:s1, type:int, comment:Suit of card #1 Ordinal (1-4) representing {Hearts, Spades, Diamonds, Clubs} (required)), FieldSchema(name:c1, type:int, comment:Rank of card #1 Numerical (1-13) representing (Ace, 2, 3, ... , Queen, King) (required)), FieldSchema(name:s2, type:int, comment:Suit of card #2 Ordinal (1-4) representing {Hearts, Spades, Diamonds, Clubs} (required)), FieldSchema(name:c2, type:int, comment:Rank of card #2 Numerical (1-13) representing (Ace, 2, 3, ... , Queen, King) (required)), FieldSchema(name:s3, type:int, comment:Suit of card #3 Ordinal (1-4) representing {Hearts, Spades, Diamonds, Clubs} (required)), FieldSchema(name:c3, type:int, comment:Rank of card #3 Numerical (1-13) representing (Ace, 2, 3, ... , Queen, King) (required)), FieldSchema(name:s4, type:int, comment:Suit of card #4 Ordinal (1-4) representing {Hearts, Spades, Diamonds, Clubs} (required)), FieldSchema(name:c4, type:int, comment:Rank of card #4 Numerical (1-13) representing (Ace, 2, 3, ... , Queen, King) (required)), FieldSchema(name:s5, type:int, comment:Suit of card #5 Ordinal (1-4) representing {Hearts, Spades, Diamonds, Clubs} (required)), FieldSchema(name:c5, type:int, comment:Rank of card #5 Numerical (1-13) representing (Ace, 2, 3, ... , Queen, King) (required)), FieldSchema(name:class, type:int, comment:0 (low) to 9 (high) (required))], location:hdfs://0.0.0.0:8020/Data/PokerHand, inputFormat:org.apache.hadoop.mapred.TextInputFormat, outputFormat:org.apache.hadoop.hive.ql.io.HiveIgnoreKeyTextOutputFormat, compressed:false, numBuckets:-1, serdeInfo:SerDeInfo(name:null, serializationLib:org.apache.hadoop.hive.serde2.lazy.LazySimpleSerDe, parameters:{serialization.format=44, field.delim=44}), bucketCols:[], sortCols:[], parameters:{}), partitionKeys:[], parameters:{EXTERNAL=TRUE, transient_lastDdlTime=1360614282, comment=<para>Poker Hand Dataset</para><para>Number of Instances: 1,000,000</para><para>Each record is an example of a hand consisting of five playing cards drawn from a standard deck of 52. Each card is described using two attributes (suit and rank), for a total of 10 predictive attributes. There is one Class attribute that describes the Poker Hand. The order of cards is important, which is why there are 480 possible Royal Flush hands as compared to 4 (one for each suit � explained in more detail below).</para>}, viewOriginalText:null, viewExpandedText:null, tableType:EXTERNAL_TABLE)"
 
 #endif


type OdbcDataServer(cns:string,timeout:int<s>) = 
    let conn = 
#if POOL_CONNECTIONS
        match connectionDict.TryGetValue((cns,timeout)) with
        | (true,conn) when conn.State &&& Data.ConnectionState.Open <> enum 0 -> conn
        | _ ->
#endif
            let conn = new OdbcConnection (cns,ConnectionTimeout=int timeout*1000)
            
            
            conn.Open()
#if POOL_CONNECTIONS
            connectionDict.[(cns,timeout)] <- conn
#endif
            conn

    let runReaderComand (cmd:string) = 
        printfn "executing query command \"%s\"" cmd
        let command = conn.CreateCommand(CommandTimeout=int timeout*1000, CommandText=cmd)
        command.ExecuteReader()

    let runNonQueryComand (cmd:string) = 
        printfn "executing non-query command \"%s\"" cmd
        let command = conn.CreateCommand(CommandTimeout=int timeout*1000, CommandText=cmd)
        command.ExecuteNonQuery()
    let GetTableSchema (tableName, useUnitAnnotations) = 
        let xs = 
            [| use reader = runReaderComand (sprintf "DESCRIBE extended %s;" tableName) 
                       
               while reader.Read() do 
                    let colName = reader.GetString(0)
                    match colName with
                    | "Detailed Table Information" -> 
                         let detailedInfoText = reader.GetString(1)
                         match getDetailedTableInfo detailedInfoText with 
                         | Some detailedInfo -> yield (None, None, Some detailedInfo)
                         | None -> 
                            // The comment will be empty if there is no match
                            let comment = System.Text.RegularExpressions.Regex.Match(detailedInfoText,"parameters:{[^}]*comment=([^}]*)}").Groups.[1].Value
                            yield (Some(comment),None,None)
                         
                    | s when String.IsNullOrWhiteSpace s -> ()
                    | x -> 
                        // These are the 'simple' description of the table columns. If anything goes wrong
                        // with parsing the detailed description then we assume there are no partition
                        // keys and use the information extracted from the simple description instead.
                        let datatype = parseHiveType (reader.GetString(1).ToLower())
                        let desc = (if reader.IsDBNull(2) then "" else reader.GetString(2))
                        yield (None,Some((x, desc, datatype)),None)
            |] 
        let tableDetailsOpt = xs |> Array.map (fun (_,_,c) -> c) |> Array.tryPick id
        let tableDetails = 
            match tableDetailsOpt with 
            | Some tableDetails -> tableDetails
            | None -> 
                let tableCommentBackup = xs |> Array.map (fun (a,_,_) -> a) |> Array.tryPick id
                let tableColumnsBackup = xs |> Array.map (fun (_,b,_) -> b) |> Array.choose id
                { Columns = tableColumnsBackup |> Array.map (fun (a,b,c) -> { Name=a; Type=c; Comment=b })
                  SortKeys=[| |]
                  BucketKeys=[| |]
                  PartitionKeys=[| |]
                  Comment=defaultArg tableCommentBackup ""}
        let columns = 
                tableDetails.Columns
                    //|> Array.map snd 
                    //|> Array.choose (fun x -> x)
                    |> Array.map (fun column -> 
                        let (unit,colRequired) = parseHiveColumnAnnotation column.Comment
                        let colTypeWithUnit = 
                            match column.Type, unit with 
                            | DSingle _, Some u when useUnitAnnotations -> DSingle (AnyUnit.Unit u)
                            | DDouble _, Some u when useUnitAnnotations -> DDouble (AnyUnit.Unit u)
                            | DDecimal _, Some u when useUnitAnnotations -> DDecimal (AnyUnit.Unit u)
                            | dt,_ -> dt
                        {HiveName=column.Name;Description=column.Comment;HiveType=colTypeWithUnit;IsRequired=colRequired})
        let partitionKeys = tableDetails.PartitionKeys |> Array.map (fun x -> x.Name)
        {Columns=columns;Description=tableDetails.Comment;PartitionKeys=partitionKeys;BucketKeys=tableDetails.BucketKeys;SortKeys=tableDetails.SortKeys}

    // TODO: we are getting long pauses on exit of fsc.exe because the ODBC connection pool is 
    // being finalized. This is really annoying. I don't know what to do about this. Explicitly
    // closing doesn't help.

    interface IDisposable with 
        member x.Dispose() = 
            //printfn "closing..."
            //System.Data.Odbc.OdbcConnection.ReleaseObjectPool()
            conn.Dispose()
            //printfn "%A" [ for f in conn.GetType().GetFields(System.Reflection.BindingFlags.Public ||| System.Reflection.BindingFlags.Instance ||| System.Reflection.BindingFlags.NonPublic) -> f.Name, f.GetValue(conn) ]
            //System.AppDomain.CurrentDomain.DomainUnload.Add(fun _ -> 
            //    printfn "exiting..."
            //    System.Environment.Exit(0))
            //printfn "closed!"

    interface IHiveDataServer with 

        member x.GetTablePartitionNames(tableName) = 
            [| use reader = runReaderComand ("SHOW PARTITIONS " + tableName + ";") ; 
               while reader.Read() do yield reader.GetString(0)  |]

        member x.GetTableNames() = 
            [| use reader = runReaderComand "SHOW TABLES;" ; 
               while reader.Read() do yield reader.GetString(0)  |]

        member x.GetTableSchema (tableName, useUnitAnnotations) = GetTableSchema (tableName, useUnitAnnotations)

        member x.ExecuteCommand (commandText) = runNonQueryComand commandText

        member x.GetDataFrame (query, colData) =
          // Convert the results to SI units
          let queryText = formatQuery HiveQueryUnitSystem.SI query + ";"
                    
          let table = 
              [| use reader = runReaderComand queryText 
                 while reader.Read() do 
                    let row = [|for i in 0..colData.Length-1 -> reader.GetValue(i)|]
                    yield [|
                        for i in 0..colData.Length-1 do
                          let colData = 
                              match colData.[i].HiveType with
                              | DInt8 -> VInt8(match row.[i] with | :? DBNull -> Null | o -> Value(Convert.ToSByte o))
                              | DInt16 -> VInt16(match row.[i] with | :? DBNull -> Null | o -> Value(Convert.ToInt16 o))
                              | DInt32 -> VInt32(match row.[i] with | :? DBNull -> Null | o -> Value(Convert.ToInt32 o))
                              | DInt64 -> VInt64(match row.[i] with | :? DBNull -> Null | o -> Value(Convert.ToInt64 o))
                              | DSingle _ -> VSingle(match row.[i] with | :? DBNull -> Null | o -> Value(Convert.ToSingle o))
                              | DDouble _ -> VDouble(match row.[i] with | :? DBNull -> Null | o -> Value(Convert.ToDouble o))
                              | DDecimal _ -> VDecimal(match row.[i] with | :? DBNull -> Null | o -> Value(Convert.ToDecimal o))
                              | DBoolean -> VBoolean(match row.[i] with | :? DBNull -> Null | o -> Value(Convert.ToBoolean o))
                              | DString -> VString(match row.[i] with | :? DBNull -> null | o -> (o :?> string))
                              | DArray _ -> failwith "nyi map/array/struct"
                              | DMap _ -> failwith "nyi map/array/struct"
                              | DStruct _ -> failwith "nyi map/array/struct"
                              | DAtom -> failwith "unreachable: atom"
                              | DTable(_args) -> failwith "evaluating whole table as part of client-side expression"
                          yield colData 
                      |]
              |]
          table

let hiveHandler (cns:string) (timeout:int<s>) (msg:HiveRequest) : HiveResult = 
    //printfn "request to '%s'" cns
    try
        async {
          try
            use dataServer = 
#if NO_INCLUDE_MOCK_DATA
#else
                if cns.StartsWith("Driver=HIVE;Host=" + mockServer + ";") then 
                    new MockDataServer() :> IHiveDataServer
                else 
#endif
                    new OdbcDataServer(cns,timeout) :> IHiveDataServer

            let result = 
                match msg with
                | ExecuteCommand(commandText) -> dataServer.ExecuteCommand(commandText) |> CommandResult
                | GetTableNames -> dataServer.GetTableNames()|> TableNames
                | GetTablePartitionNames(tableName) -> dataServer.GetTablePartitionNames(tableName)|> TablePartitionNames
                | GetTableDescription(tableName,useUnitAnnotations) ->
                        let table = dataServer.GetTableSchema (tableName,useUnitAnnotations)
                        let rows = dataServer.GetDataFrame (Limit(Table(tableName,table.Columns),10), table.Columns)
                        // let maxWidth = 15
                        let tableHead  =
                            let inline nullToString x = match x with | null -> "NA" | x -> string x
                            let inline nullableToString (x:Nullable<'T>) = if x.HasValue then string x.Value else "NA"
                            (*
                            let resize (center:bool) (length:int) (str:string) = 
                                if str.Length < length 
                                then 
                                    if center then 
                                        let diff = length - str.Length
                                        let leftPad = (String.replicate ((diff / 2) + (diff % 2)) " ")
                                        let rightPad = (String.replicate (diff / 2) " ")
                                        leftPad + str + rightPad
                                    else str.PadLeft(length)
                                elif str.Length = length then str
                                else str.Substring(0,Math.Max(0,length - 2)) + ".."
                            *)
                            [ for row in rows do
                                let text =
                                    (table.Columns,row) 
                                    ||> Array.map2 (fun col v ->
                                        let v' =
                                            match v with
                                            | VInt8 x -> x |> nullableToString
                                            | VInt16 x -> x |> nullableToString
                                            | VInt32 x -> x |> nullableToString
                                            | VInt64 x -> x |> nullableToString
                                            | VSingle x -> x |> nullableToString
                                            | VDouble x -> x |> nullableToString
                                            | VDecimal x -> x |> nullableToString
                                            | VBoolean x -> x |> nullableToString
                                            | VString x -> x |> nullToString
                                            | VArray x -> x |> nullToString
                                            | VStruct x -> x |> nullToString
                                            | VMap x -> x |> nullToString
                                        col.HiveName + "=" + v') 
                                    |> String.concat ", "
                                yield text ] 
                            |> List.map (fun x -> "(" + (if x.Length > 80 then x.Substring(0,78) + ".." else x) + ")")
                            |> List.map (fun x -> sprintf "<para>%s</para>" x)
                            |> String.concat ""
                            |> fun x -> "<para> </para>" + x
                        TableDescription(table.Description,tableHead)
                | GetTableSchema(tableName,useUnitAnnotations) ->
                    TableSchema (dataServer.GetTableSchema (tableName, useUnitAnnotations))
                | GetDataFrame (query, colData) ->
                    DataFrame (dataServer.GetDataFrame (query, colData))
            return result                                
          with
          | ex -> return Exception("Error: " + ex.Message + "\n" (* + sprintf "Connection='%s'\n" cns *) + sprintf "Timeout='%d'\n" (int timeout) + sprintf "Message='%A'\n" msg + ex.ToString())
        } |> fun x -> Async.RunSynchronously(x,int timeout*1000)
    with
    | ex -> Exception("Error: " + ex.Message + "\n" (* + sprintf "Connection='%s'\n" cns *)  + sprintf "Message='%A'\n" msg + ex.ToString())


let HiveRequestInProcess (server, port, auth, uid, pwd, timeout, req:HiveRequest) = 
    let conn = sprintf "Driver=HIVE;Host=%s;Port=%d;FRAMED=0" server port
    let conn = if uid = "None" then conn else sprintf "%s;UID=%s" conn uid
    let conn = if pwd = "None" then conn else sprintf "%s;PWD=%s" conn pwd
    let conn = if auth = -1 then conn else sprintf "%s;AUTHENTICATION=%d" conn auth
    hiveHandler conn timeout req

let getBytes cns timeout (input:byte[]) =
    SerDes.fromBytes<HiveRequest> input
    |> hiveHandler cns timeout
    |> SerDes.toBytes

let HiveRequestInProcessSerialized  (url:string, inBytes:byte[]) = 
    let req = System.Web.HttpUtility.ParseQueryString(url) 
    let timeout = int32 req.["Timeout"] * 1<s>
    let server = req.["Server"]
    let port = int32 req.["Port"]

    let uid = if req.AllKeys.Contains("Uid") then Some req.["Uid"] else None
    // A uid of "None" means none
    let uid = match uid with Some "None" -> None | _ -> uid
    // If the uid wasn't specified and we're using the default host, then use DefaultUid

    let pwd = if req.AllKeys.Contains("Pwd") then Some req.["Pwd"] else None
    // A pwd of "None" means none
    let pwd = match pwd with Some "None" -> None | _ -> pwd
    // If the pwd wasn't specified and we're using the default server, then use DefaultPwd

    let outBytes =
        match uid, pwd with 
        | Some uid, Some pwd -> 
            getBytes (sprintf "Driver=HIVE;Host=%s;AUTHENTICATION=3;FRAMED=0;Port=%d;UID=%s;PWD=%s" server port uid pwd) timeout inBytes
        | _ -> 
            getBytes (sprintf "Driver=HIVE;Host=%s;FRAMED=0;Port=%d" server port) timeout inBytes
    outBytes
