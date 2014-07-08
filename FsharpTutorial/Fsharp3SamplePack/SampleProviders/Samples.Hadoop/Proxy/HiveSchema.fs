namespace global

module HiveSchema =

    let inline (|Value|Null|) (x:System.Nullable<_>) = if x.HasValue then Value x.Value else Null 
    let inline Value x = System.Nullable x
    let inline Null<'T when 'T : (new : unit -> 'T) and 'T : struct and 'T :> System.ValueType> = System.Nullable<'T>()

    // Algebra of units, may currently include non-SI units
    // CLEANUP: make this into normalized SI units that carry their conversion factors, as this is simpler than continually
    // re-consulting the units table
    type AnyUnit = 
        /// Used to infer units in cases where they are not recorded statically
        | Unknown of AnyUnit option ref 
        | One
        | Unit of string
        | Prod of AnyUnit * AnyUnit
        | Inv of AnyUnit
        static member NewVar(useUnitAnnotations) = if useUnitAnnotations then AnyUnit.Unknown (ref None) else AnyUnit.One

    /// Transform the value 'x' into the corresponding value in SI units. 'dir' indicates the direction of travel.
    let inline unitTransformGeneric ofFloat dir u x =
        let rec conv dir u x = 
            match u with
            | AnyUnit.Unknown r -> 
                match r.Value with 
                // Unsolved, treat as 'One'
                | None -> x 
                | Some soln -> conv dir soln x
            | AnyUnit.One -> x
            | AnyUnit.Prod (u1,u2) -> conv dir u1 (conv dir u2 x)
            | AnyUnit.Inv u -> conv (not dir) u x
            | AnyUnit.Unit unitString  ->
                let (_measureAnnotation,multiplier,offset) = Samples.FSharp.FrebaseUnitsOfMeasure.UnitsOfMeasure.unitSearch unitString 
                let shift = (match offset with | Some(y) -> ofFloat y | None -> LanguagePrimitives.GenericZero)
                if dir then (x + shift) * ofFloat multiplier
                else (x / ofFloat multiplier) - shift
        conv dir u x

    let unitTransform32 dir u (x:single) = unitTransformGeneric float32 dir u x
    let unitTransform64 dir u (x:double) = unitTransformGeneric float dir u x
    let unitTransformDecimal dir u (x:decimal) = unitTransformGeneric decimal dir u x

    /// Represents a type of a column in a hive table
    type DBasic =
        | DInt8
        | DInt16
        | DInt32 
        | DInt64
        | DSingle of AnyUnit 
        | DDouble of AnyUnit 
        | DDecimal of AnyUnit 
        | DBoolean 
        | DAtom 
        | DMap of DBasic * DBasic
        | DArray of DBasic
        | DStruct of (string * DBasic) []
        | DTable of HiveColumnSchema[] // result of 'select *'
        | DString 
        // Compute unit propagation for multiplication
        static member (*) (t1:DBasic,t2:DBasic) = 
            match t1,t2 with 
            | DSingle u1, DSingle u2 -> DSingle(Prod(u1,u2)) 
            | DDouble u1, DDouble u2 -> DDouble(Prod(u1,u2)) 
            | DDecimal u1, DDecimal u2 -> DDecimal(Prod(u1,u2)) 
            | _ -> t1
        // Compute unit propagation for division
        static member (/) (t1:DBasic,t2:DBasic) = 
            match t1,t2 with 
            | DSingle u1, DSingle u2 -> DSingle(Prod(u1,Inv u2)) 
            | DDouble u1, DDouble u2 -> DDouble(Prod(u1,Inv u2)) 
            | DDecimal u1, DDecimal u2 -> DDecimal(Prod(u1,Inv u2)) 
            | _ -> t1

    and HiveColumnSchema = 
        { HiveName: string
          Description: string
          HiveType: DBasic
          IsRequired: bool }

    and HiveTableSortKey = 
        { Column: string; Order: string }

    and HiveTableSchema = 
        { Description: string
          Columns: HiveColumnSchema[]
          PartitionKeys: string[]
          BucketKeys: string[]
          SortKeys: HiveTableSortKey[] }

    let DDate = DString // TODO: check me

    let rec formatHiveType ty = 
        match ty with
        | DSingle _ -> "float" 
        | DDouble _ -> "double" 
        | DDecimal _ -> "decimal" 
        | DString -> "string" 
        | DInt32 -> "int" 
        | DInt8 -> "tinyint" 
        | DInt16 -> "smallint" 
        | DInt64 -> "bigint" 
        | DBoolean -> "bool" 
        | DMap(ty1,ty2) -> "map<" + formatHiveType ty1 + "," + formatHiveType ty2 + ">"
        | DArray(ty1) -> "array<" + formatHiveType ty1 + ">"
        | DStruct _cols -> failwith "struct types: nyi"
        | _ -> failwith (sprintf "unexpected type '%A'" ty)

    let (|Unitized|_|) x = match x with (DSingle u | DDouble u | DDecimal u) -> Some u | _ -> None

    let rec formatHiveUnit u = 
        match u with 
        | AnyUnit.Unknown r -> 
            match r.Value with 
            | None -> failwith "unknown unit-of-measure in emitted Hive type" 
            | Some soln -> formatHiveUnit soln
        | One -> ""
        | Unit s -> s
        | Prod (u1,Inv u2) -> formatHiveUnit u1 + "/" + formatHiveUnit u2
        | Prod (u1,u2) -> formatHiveUnit u1 + "*" + formatHiveUnit u2
        | Inv u -> "1/" + formatHiveUnit u

    let rec formatHiveComment (ty, req) = 
        match ty, req with
        | Unitized One, true -> "(required)"
        | Unitized One, false -> ""
        | Unitized u, true -> "(unit="+formatHiveUnit u+", required)"
        | Unitized u, false -> "(unit="+formatHiveUnit u+")"
        | _, true -> "(required)"
        | _ -> "(no comment)"

    /// Represents a value in one column of one row in a hive table
    type TVal =
        | VInt8 of System.Nullable<sbyte>
        | VInt16 of System.Nullable<int16>
        | VInt32 of System.Nullable<int32>
        | VInt64 of System.Nullable<int64>
        | VSingle of System.Nullable<single>
        | VDecimal of System.Nullable<decimal>
        | VDouble of System.Nullable<double>
        | VBoolean of System.Nullable<bool>
        | VString of string  // may be null
        | VMap of (TVal * TVal) [] // may be null
        | VArray of TVal[] // may be null
        | VStruct of (string * TVal)[] // may be null

    /// Represents an expression in a hive 'where' expression
    type TExpr =
        | EVal of TVal * DBasic
        | ETable of string * HiveColumnSchema[]
        | EColumn of string * DBasic
        | EQuery of HiveQueryData // e.g. 'AVG(height)'
        | EBinOp of TExpr * string * TExpr * DBasic
        | EFunc of string * TExpr list * DBasic

    and TReln = 
        | RGeq
        | RLt
        | RLeq
        | RGt
        | REq
        | RNeq


    /// Represents a hive query
    and HiveQueryData = 
        | Table of string * HiveColumnSchema[] 
        | GroupBy of HiveQueryData  * TExpr
        | Distinct of HiveQueryData 
        | Where of HiveQueryData * TExpr 
        | Limit of HiveQueryData * int
        | Count of HiveQueryData 
        // A 'sumBy' or other aggregation operation.
        // The 'bool' indicates whether the result would get labelled as 'required' if we write the table.
        | AggregateOpBy of string * HiveQueryData * TExpr * bool
        | TableSample of HiveQueryData * int * int
        // A 'select' operation.
        // The 'bool' indicates whether the field would get labelled as 'required' if we write the table.
        | Select of HiveQueryData * (string * TExpr * bool)[] 

    let rec typeOfExpr e = 
        match e with 
        | EVal (_,ty) -> ty
        | ETable (_,ty) -> DTable ty
        | EColumn (_,ty) -> ty
        | EQuery q -> 
            match q with 
            | AggregateOpBy(_,_,e,_) -> typeOfExpr e 
            | Count(_) -> DInt64
            | q -> failwith (sprintf "unsupported query embedded in expression: %A" q)
        | EBinOp (_,_,_,ty) -> ty
        | EFunc (_,_,ty) -> ty
    
    type HiveRequest =
        | GetTableNames
        | GetTablePartitionNames of string
        | GetTableDescription of string * bool
        | GetTableSchema of string * bool
        | GetDataFrame of HiveQueryData * HiveColumnSchema[]
        | ExecuteCommand of string
  
    type HiveResult =
        | TableNames of string[]
        // tableDescription, first 10 rows
        | TableDescription of string*string
        | TablePartitionNames of string[]
        | TableSchema of HiveTableSchema
        // data[][]
        | DataFrame of TVal[][]
        | Exception of string
        | CommandResult of int

    type internal HiveQueryUnitSystem = 
       /// The results are converted to SI, constants deriving from F# are left as SI
       | SI 
       /// The results are converted left as non-SI, constants deriving from F# are converted to non-SI
       | NonSI
    let rec internal formatExpr useSI expr =
        match expr with 
        | ETable (_v,_ty) -> "*"
        | EVal (v,ty) -> 
            match v with 
            | VInt8 (Value n) -> string n
            | VInt16 (Value n) -> string n
            | VInt32 (Value n) -> string n
            | VInt64 (Value n)  -> string n
            // TODO check formatting of floating point corner cases
            | VSingle (Value n) -> 
                 match ty, useSI with 
                 | DSingle u, NonSI -> string (unitTransform32 false u n)  
                 | _ -> string n 
            | VDouble (Value n) -> 
                 match ty, useSI with 
                 | DSingle u, NonSI -> string (unitTransform64 false u n)  
                 | _ -> string n 
            | VDecimal (Value n) -> 
                 match ty, useSI with 
                 | DSingle u, NonSI -> string (unitTransformDecimal false u n)  
                 | _ -> string n 
            | VBoolean (Value b) -> string b
            | VString null -> "null"
            | VString s -> 
                // "String literals can be expressed with either single quotes (') or double quotes ("). Hive uses C-style escaping within the strings."
                //  (from https://cwiki.apache.org/confluence/display/Hive/Literals)
                sprintf "'%s'" (s.Replace("'", @"\'"))
            | VInt8 Null -> "null"
            | VInt16 Null -> "null"
            | VInt32 Null -> "null"
            | VInt64 Null -> "null"
            | VSingle Null -> "null"
            | VDouble Null -> "null"
            | VDecimal Null -> "null"
            | VBoolean Null -> "null"
            | VMap _ | VArray _ | VStruct _ -> "<nyi: map/array/struct>"
        | EColumn (nm,ty) -> 
            // Convert the column from non-SI units into SI units
            match ty, useSI with
            | DSingle u, SI-> 
                let convF = unitTransform32 true u 1.0f
                if convF = 1.0f then nm
                else sprintf "%s * %f" nm convF
            | DDouble u, SI -> 
                let convF = unitTransform64 true u 1.0
                if convF = 1.0 then nm
                else sprintf "%s * %f" nm convF
            | DDecimal u, SI -> 
                let convF = unitTransformDecimal true u 1.0M
                if convF = 1.0M then nm
                else sprintf "%s * %f" nm convF
            | _ -> nm
        | EBinOp(arg1,op,arg2,_) -> formatExpr useSI arg1 + " " + op  + " " + formatExpr useSI arg2
        | EQuery(q) -> 
            if formatQueryAux useSI q <> "" then failwith (sprintf "unsupported nested query: %A" q)
            formatQuerySelect useSI q
        | EFunc("$id",[e],_) -> formatExpr useSI e // used for changing types
        | EFunc(op,[],_) -> op 
        | EFunc(op,args,_) -> op + "(" + (List.map (formatExpr useSI) args |> String.concat ",") + ")"

    and formatReln = function
        | REq  -> "="
        | RGt  -> ">"
        | RGeq  -> ">="
        | RLeq  -> "<="
        | RLt  -> "<"
        | RNeq  -> "!="

    and getTableName q = 
        match q with 
        | Table(tableName,_) -> tableName
        | Distinct q2 | GroupBy(q2,_) | AggregateOpBy(_,q2,_,_) | Select(q2,_) | Count(q2) | Limit(q2, _) | Where(q2,_) | TableSample(q2,_,_) -> getTableName q2

    // https://cwiki.apache.org/Hive/languagemanual.html
    and internal formatQuerySelect useSI q = 
        match q with 
        | Table(_tableName, _) -> "*"
        | Select (_q2,colExprs) -> colExprs |> Array.map (fun (_,e,_) -> formatExpr useSI e) |> String.concat ","
        | AggregateOpBy (op, _q2,selector,_) -> op + "(" + formatExpr useSI selector + ")"
        | TableSample (q2,_n1,_n2) -> formatQuerySelect useSI q2 
        | GroupBy (q2,_) -> formatQuerySelect useSI q2 
        | Limit (q2,_rowCount) -> formatQuerySelect useSI q2
        | Count (q2) -> "COUNT("+formatQuerySelect useSI q2+")"
        | Distinct (q2) -> "DISTINCT("+formatQuerySelect useSI q2+")"
        | Where (q2,_pred) -> formatQuerySelect useSI q2 

    and internal formatQueryAux useSI q = 
        match q with 
        | Table(_tableName,_) -> ""
        | Select (q2,_colExprs) -> formatQueryAux useSI q2
        | GroupBy (q2,keyExpr) -> formatQueryAux useSI q2 + " GROUP BY " + formatExpr useSI keyExpr
        | AggregateOpBy (_op, q2,_selector,_) -> formatQueryAux useSI q2
        | TableSample (q2,n1,n2) -> formatQueryAux useSI q2 + sprintf " TABLESAMPLE(BUCKET %d OUT OF %d)" n1 n2
        | Limit (q2,rowCount) -> formatQueryAux useSI q2 + sprintf " LIMIT %i" rowCount
        | Count (q2) -> formatQueryAux useSI q2
        | Distinct (q2) -> formatQueryAux useSI q2
        | Where (q2,pred) -> formatQueryAux useSI q2 + " WHERE " + formatExpr useSI pred

    let internal formatQuery useSI q =  
        "SELECT " + formatQuerySelect useSI q + " FROM " + getTableName q + formatQueryAux useSI q


