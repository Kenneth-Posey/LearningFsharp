namespace Portable.Samples.Spreadsheet

open System
open System.Collections.Generic

[<AutoOpen>]
module Extensions = 
    type HashSet<'T> with
        member this.AddUnit(v) = ignore( this.Add(v) )

type internal Reference = string

/// Result of formula evaluation
[<RequireQualifiedAccess>]
type internal EvalResult = 
    | Success of obj
    | Error of string

/// Function that resolves reference to value.
/// if formula that computes value fails - this function should also return failure
type internal ResolutionContext = Reference -> EvalResult

/// Parsed expression
[<RequireQualifiedAccess>]
type internal Expression = 
    | Val of obj
    | Ref of Reference
    | Op of (ResolutionContext -> list<Expression> -> EvalResult) * list<Expression>
    with 
    member this.GetReferences() = 
        match this with
        | Expression.Ref r -> Set.singleton r
        | Expression.Val _ -> Set.empty
        | Expression.Op (_, args) -> (Set.empty, args) ||> List.fold (fun acc arg -> acc + arg.GetReferences())

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal Operations = 
    
    let eval (ctx : ResolutionContext) = 
        function
        | Expression.Val v -> EvalResult.Success v
        | Expression.Ref r -> ctx r
        | Expression.Op (f, args) -> try f ctx args with e -> EvalResult.Error e.Message
    
    type private Eval = Do 
        with 
        member this.Return(v) = EvalResult.Success v
        member this.ReturnFrom(v) = v
        member this.Bind(r, f) = 
            match r with
            | EvalResult.Success v -> f v
            | EvalResult.Error _-> r

    let private mkBinaryOperation<'A, 'R> (op : 'A -> 'A -> 'R) ctx =
        function
        | [a; b] -> 
            Eval.Do {
                let! ra = eval ctx a
                let! rb = eval ctx b
                match ra, rb with
                | (:? 'A as ra), (:? 'A as rb) -> return op ra rb
                | _ -> return! EvalResult.Error "Unexpected type of argument"
            }
        | _ -> EvalResult.Error "invalid number of arguments"

    let add = mkBinaryOperation<float, float> (+)
    let sub = mkBinaryOperation<float, float> (-)
    let mul = mkBinaryOperation<float, float> (*)
    let div = mkBinaryOperation<float, float> (/)

    let ge = mkBinaryOperation<float, bool> (>=)
    let gt = mkBinaryOperation<float, bool> (>)

    let le = mkBinaryOperation<float, bool> (<=)
    let lt = mkBinaryOperation<float, bool> (<)

    let eq = mkBinaryOperation<IComparable, bool> (=)
    let neq = mkBinaryOperation<IComparable, bool> (<>)

    let mmax = mkBinaryOperation<float, float> max
    let mmin = mkBinaryOperation<float, float> min

    let iif ctx = 
        function
        | [cond; ifTrue; ifFalse] -> 
            Eval.Do {
                let! condValue = eval ctx cond
                match condValue with
                | :? bool as condValue-> 
                    let e = if condValue then ifTrue else ifFalse
                    return! eval ctx e
                | _ -> return! EvalResult.Error "Condition should be evaluated to bool"
            }
        | _ -> EvalResult.Error "invalid number of arguments"
    
    let get (name : string) = 
        match name.ToUpper() with
        | "MAX" -> mmax
        | "MIN" -> mmin
        | "IF" -> iif
        | x -> failwithf "unknown operation %s" x

module internal Parser =
    let private some v (rest : string) = Some(v, rest)
    let private capture pattern text =
        let m = System.Text.RegularExpressions.Regex.Match(text, "^(" + pattern + ")(.*)")
        if m.Success then
            some m.Groups.[1].Value m.Groups.[2].Value
        else None
    let private matchValue pattern = (capture @"\s*") >> (Option.bind (snd >> capture pattern))

    let private matchSymbol pattern = (matchValue pattern) >> (Option.bind (snd >> Some))
    let private (|NUMBER|_|) = matchValue @"-?\d+\.?\d*"
    let private (|IDENTIFIER|_|) = matchValue @"[A-Za-z]\w*"
    let private (|LPAREN|_|) = matchSymbol @"\("
    let private (|RPAREN|_|) = matchSymbol @"\)"
    let private (|PLUS|_|) = matchSymbol @"\+"
    let private (|MINUS|_|) = matchSymbol @"-"
    let private (|GT|_|) = matchSymbol @">"
    let private (|GE|_|) = matchSymbol @">="
    let private (|LT|_|) = matchSymbol @"<"
    let private (|LE|_|) = matchSymbol @"<="
    let private (|EQ|_|) = matchSymbol @"="
    let private (|NEQ|_|) = matchSymbol @"<>"
    let private (|MUL|_|) = matchSymbol @"\*"
    let private (|DIV|_|) = matchSymbol @"/"
    let private (|COMMA|_|) = matchSymbol @","
    let private operation op args rest = some (Expression.Op(op, args)) rest
    let rec private (|Factor|_|) = function
        | IDENTIFIER(id, r) ->
            match r with
            | LPAREN (ArgList (args, RPAREN r)) -> operation (Operations.get id) args r
            | _ -> some(Expression.Ref id) r
        | NUMBER (v, r) -> some (Expression.Val (float v)) r
        | LPAREN(Logical (e, RPAREN r)) -> some e r
        | _ -> None

    and private (|ArgList|_|) = function
        | Logical(e, r) ->
            match r with
            | COMMA (ArgList(t, r1)) -> some (e::t) r1
            | _ -> some [e] r
        | rest -> some [] rest

    and private (|Term|_|) = function
        | Factor(e, r) ->
            match r with
            | MUL (Term(r, rest)) -> operation Operations.mul [e; r] rest
            | DIV (Term(r, rest)) -> operation Operations.div [e; r] rest
            | _ -> some e r
        | _ -> None

    and private (|Expr|_|) = function
        | Term(e, r) ->
            match r with
            | PLUS (Expr(r, rest)) -> operation Operations.add [e; r] rest
            | MINUS (Expr(r, rest)) -> operation Operations.sub [e; r] rest
            | _ -> some e r
        | _ -> None

    and private (|Logical|_|) = function
        | Expr(l, r) ->
            match r with
            | GE (Logical(r, rest)) -> operation Operations.ge [l; r] rest
            | GT (Logical(r, rest)) -> operation Operations.gt [l; r] rest
            | LE (Logical(r, rest)) -> operation Operations.le [l; r] rest
            | LT (Logical(r, rest)) -> operation Operations.lt [l; r] rest
            | EQ (Logical(r, rest)) -> operation Operations.eq [l; r] rest
            | NEQ (Logical(r, rest)) -> operation Operations.neq [l; r] rest
            | _ -> some l r
        | _ -> None

    and private (|Formula|_|) (s : string) =
        if s.StartsWith("=") then
            match s.Substring(1) with
            | Logical(l, t) when System.String.IsNullOrEmpty(t) -> Some l
            | _ -> None
        else None

    let parse text = 
        match text with
        | Formula f -> Some f
        | _ -> None

type internal CellReference = string

module internal Dependencies = 

    type Graph() = 
        let map = new Dictionary<CellReference, HashSet<CellReference>>()

        let ensureGraphHasNoCycles(cellRef) =
            let visited = HashSet()
            let rec go cycles s =
                if Set.contains s cycles then failwith ("Cycle detected:" + (String.concat "," cycles))
                if visited.Contains s then cycles
                else
                visited.AddUnit s
                if map.ContainsKey s then
                    let children = map.[s]
                    ((Set.add s cycles), children)
                        ||> Seq.fold go
                        |> (fun cycle -> Set.remove s cycles)
                else
                    cycles

            ignore (go Set.empty cellRef)

        member this.Insert(cell, parentCells) = 
            for p in parentCells do
                let parentSet = 
                    match map.TryGetValue p with
                    | true, set -> set
                    | false, _ ->
                        let set = HashSet()
                        map.Add(p, set)
                        set
                parentSet.AddUnit cell
            try 
                ensureGraphHasNoCycles cell
            with
                _ -> 
                this.Delete(cell, parentCells)
                reraise()
                             
        member this.GetDependents(cell) = 
            let visited = HashSet()
            let order = Queue()
            let rec visit curr = 
                if not (visited.Contains curr) then 
                    visited.AddUnit curr
                    order.Enqueue(curr)
                    match map.TryGetValue curr with
                    | true, children -> 
                        for ch in children do
                            visit ch
                    | _ -> ()

                    
            visit cell
            order :> seq<_>

        member this.Delete(cell, parentCells) = 
            for p in parentCells do
                map.[p].Remove(cell)
                |> ignore

type Cell = 
    {
        Reference : CellReference
        Value : string
        RawValue : string
        HasError : bool
    }

type RowReferences = 
    {
        Name : string
        Cells : string[]
    }

type Spreadsheet(height : int, width : int) = 
    
    do 
        if height <=0 then failwith "Height should be greater than zero"
        if width <=0 || width > 26 then failwith "Width should be greater than zero and lesser than 26"

    let rowNames = [| for i = 0 to height - 1 do yield string (i + 1)|]
    let colNames = [| for i = 0 to (width - 1) do yield string (char (int 'A' + i)) |]

    let isValidReference (s : string) = 
        if s.Length < 2 then false
        else
        let c = s.[0..0]
        let r = s.[1..]
        (Array.exists ((=)c) colNames) && (Array.exists ((=)r) rowNames)

    let dependencies = Dependencies.Graph()
    let formulas = Dictionary<_, Expression>()

    let values = Dictionary()
    let rawValues = Dictionary()

    let setError cell text = 
        values.[cell] <- EvalResult.Error text

    let getValue reference = 
        match values.TryGetValue reference with
        | true, v -> v
        | _ -> EvalResult.Success 0.0
    
    let deleteValue reference = 
        values.Remove(reference)
        |> ignore

    let deleteFormula cell = 
        match formulas.TryGetValue cell with
        | true, expr ->
            dependencies.Delete(cell, expr.GetReferences())
            formulas.Remove(cell) 
            |> ignore
        | _ -> ()

    let evaluate cell = 
        let deps = dependencies.GetDependents cell
        for d in deps do
            match formulas.TryGetValue d with
            | true, e -> 
                let r = Operations.eval getValue e
                values.[d] <- r
            | _ -> ()
        deps

    let setFormula cell text = 
        let setError msg = 
            setError cell msg
            [cell] :> seq<_>
        
        try 
            match Parser.parse text with
            | Some expr ->
                let references = expr.GetReferences()
                let invalidReferences = [for r in references do if not (isValidReference r) then yield r]
                if not (List.isEmpty invalidReferences) then
                    let msg = sprintf "Formula contains invalid references:%s" (String.concat ", " invalidReferences)
                    setError msg
                else
                try
                    dependencies.Insert(cell, references)
                    formulas.Add(cell, expr)
                    |> ignore
                    evaluate cell
                with
                    e -> setError e.Message
            | _ -> setError "Invalid formula text"
        with e -> setError e.Message

    member this.Headers = colNames
    member this.Rows = rowNames
    member this.GetRowReferences() = 
        seq { for r in rowNames do
              let cells = [| for c in colNames do yield c + r |]
              yield { Name = r; Cells = cells } }

    member this.SetValue(cellRef : Reference, value : string) : Cell[] = 
        rawValues.Remove(cellRef)
        |> ignore

        if not (String.IsNullOrEmpty value) then
            rawValues.[cellRef] <- value

        deleteFormula cellRef
        
        let affectedCells = 
            if (value <> null && value.StartsWith "=") then
                setFormula cellRef value
            elif String.IsNullOrEmpty value then
                deleteValue cellRef
                evaluate cellRef
            else
                match Double.TryParse value with
                | true, value -> 
                    values.[cellRef] <- EvalResult.Success value
                    evaluate cellRef
                | _ -> 
                    values.[cellRef] <- EvalResult.Error "Number expected"
                    [cellRef] :> _
        [| for r in affectedCells do 
            let rawValue = 
                match rawValues.TryGetValue r with
                | true, v -> v
                | false, _ -> ""

            let valueStr, hasErr = 
                match values.TryGetValue r with
                | true, (EvalResult.Success v) -> (string v), false
                | true, (EvalResult.Error msg) -> msg, true
                | false, _ -> "", false
            let c = {Reference = r; Value = valueStr; RawValue = rawValue; HasError = hasErr}
            yield c |]