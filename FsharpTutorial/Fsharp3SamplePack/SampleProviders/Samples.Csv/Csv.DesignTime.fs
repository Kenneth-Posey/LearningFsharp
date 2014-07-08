// Learn more about F# at http://fsharp.net

namespace Samples.Csv.DesignTime

open System
open System.Reflection
open System.IO
open Samples.FSharp.ProvidedTypes
open Microsoft.FSharp.Core.CompilerServices
open System.Text.RegularExpressions

type private Top = Top

[<TypeProvider>]
type public CsvFileProvider(cfg:TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces()

    let memo f =
        let d = System.Collections.Generic.Dictionary(HashIdentity.Structural)
        fun x y ->
            if not <| d.ContainsKey(x,y) then
                d.[(x,y)] <- f x y
            d.[(x,y)]

#if FX_NO_ASSEMBLY_LOAD_FROM
    let onUiThread f = 
        if System.Windows.Deployment.Current.Dispatcher.CheckAccess() then 
            f() 
        else
            let resultTask = System.Threading.Tasks.TaskCompletionSource<'T>()
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(Action(fun () -> try resultTask.SetResult (f()) with err -> resultTask.SetException err)) |> ignore
            resultTask.Task.Result

    let runtimeAssembly = 
        onUiThread (fun () ->
            let assemblyPart = System.Windows.AssemblyPart()
            let FileStreamReadShim(fileName) = 
                match System.Windows.Application.GetResourceStream(System.Uri(fileName,System.UriKind.Relative)) with 
                | null -> System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication().OpenFile(fileName, System.IO.FileMode.Open) :> System.IO.Stream 
                | resStream -> resStream.Stream
            let assemblyStream = FileStreamReadShim cfg.RuntimeAssembly
            
            assemblyPart.Load(assemblyStream))
#else
    #if FX_NO_REFLECTION_ONLY_LOAD
    let runtimeAssembly = ignore cfg; System.Reflection.Assembly.Load("Samples.Csv")
    #else
    // doesn't handle Silverlight/Portable assembly loading correctly: FSharp.Core, Version=2.3.5.0
    do System.AppDomain.CurrentDomain.add_ReflectionOnlyAssemblyResolve(fun _ resEvArgs -> System.Reflection.Assembly.ReflectionOnlyLoadFrom(System.Reflection.Assembly.Load(resEvArgs.Name).Location))
    // The types are associated with the hetereogeneous runtime assembly (e.g. .NET 2.0-3.5)
    let runtimeAssembly = System.Reflection.Assembly.ReflectionOnlyLoadFrom(cfg.RuntimeAssembly)     
    #endif
#endif

    let helperTy = runtimeAssembly.GetType("Samples.Csv.Runtime.Helpers")
    let makeOptMeth = helperTy.GetMethod("makeOpt")
    
    let strTy = makeOptMeth.GetParameters().[1].ParameterType
    let mscorlib = strTy.Assembly
    let intTy = mscorlib.GetType("System.Int32")
    let int64Ty = mscorlib.GetType("System.Int64")
    let boolTy = mscorlib.GetType("System.Boolean")
    let floatTy = mscorlib.GetType("System.Double")
    let dateTy = mscorlib.GetType("System.DateTime")
    let strTy = mscorlib.GetType("System.String")
    let charTy = mscorlib.GetType("System.Char")
    let numStyTy = mscorlib.GetType("System.Globalization.NumberStyles")
    let dateStyTy = mscorlib.GetType("System.Globalization.DateTimeStyles")
    let cultureInfoTy = mscorlib.GetType("System.Globalization.CultureInfo")
    let iFmtProTy = mscorlib.GetType("System.IFormatProvider")
    let objTy = mscorlib.GetType("System.Object")
    let funcTyDef = mscorlib.GetType("System.Func`2")
    let optionTyDef = makeOptMeth.ReturnType.GetGenericTypeDefinition()
    let intOptTy = optionTyDef.MakeGenericType(intTy) 
    let int64OptTy = optionTyDef.MakeGenericType(int64Ty) 
    let boolOptTy = optionTyDef.MakeGenericType(boolTy)
    let floatOptTy = optionTyDef.MakeGenericType(floatTy)
    let dateOptTy = optionTyDef.MakeGenericType(dateTy)
    let topTy = typeof<Top>
    let topOptTy = optionTyDef.MakeGenericType(topTy)

    let cultureInfoCtor = cultureInfoTy.GetConstructor([| strTy |])

    let (|IntTy|Int64Ty|BoolTy|FloatTy|DateTy|) ty =
        if ty = intTy then IntTy
        elif ty = int64Ty then Int64Ty
        elif ty = boolTy then BoolTy
        elif ty = floatTy then FloatTy
        elif ty = dateTy then DateTy
        else
            failwith (sprintf "Unexpected type: %A" ty)

    let generalizeBaseTypes t1 t2 =
        if t1 = t2 then t1
        elif t1 = topTy then t2
        elif t2 = topTy then t1
        elif t1 = strTy || t2 = strTy then strTy
        else
            match t1,t2 with
            | IntTy,Int64Ty
            | Int64Ty,IntTy -> int64Ty
            | (IntTy|Int64Ty),FloatTy
            | FloatTy,(IntTy|Int64Ty) -> floatTy
            | _,_ -> strTy

    let rec generalize (t1:Type) (t2:Type) =
        let baseType1 = if t1.IsGenericType then t1.GetGenericArguments().[0] else t1
        let baseType2 = if t2.IsGenericType then t2.GetGenericArguments().[0] else t2
        let ty = generalizeBaseTypes baseType1 baseType2
        let makeOptional = t1.IsGenericType || t2.IsGenericType
        if ty = strTy || not makeOptional then ty 
        else optionTyDef.MakeGenericType(ty)

    let typeFromStr (cul:Globalization.CultureInfo) str =
        let test f = match f str with | true,_ -> true | _ -> false
        if System.String.IsNullOrEmpty str then topOptTy
        elif test (fun s -> System.Int32.TryParse(s, Globalization.NumberStyles.Integer, cul.NumberFormat)) then intTy
        elif test (fun s -> System.Int64.TryParse(s, Globalization.NumberStyles.Integer, cul.NumberFormat)) then int64Ty
        elif test (fun s -> System.Double.TryParse(s, Globalization.NumberStyles.Float, cul.NumberFormat)) then floatTy
        elif test System.Boolean.TryParse then boolTy
        elif test (fun s -> System.DateTime.TryParse(s, cul.DateTimeFormat, Globalization.DateTimeStyles.None)) then dateTy
        else strTy
    
    //let arrGetMeth = fscoreAsm.GetType("Microsoft.FSharp.Core.LanguagePrimitives+IntrinsicFunctions").GetMethod("GetArray").MakeGenericMethod(strTy)
    let (Quotations.Patterns.Call(_,arrGetMeth,_)) = <@ [|"test"|].[0] @>

    let dateStyNone = Quotations.Expr.Value(dateStyTy.GetField("None").GetRawConstantValue(), dateStyTy)
    let numStyInt = Quotations.Expr.Value(numStyTy.GetField("Integer").GetRawConstantValue(), numStyTy)
    let numStyFlt = Quotations.Expr.Value(numStyTy.GetField("Float").GetRawConstantValue(), numStyTy)

    let mToF (m:MethodInfo) args = 
        let x = Quotations.Var("x", m.GetParameters().[0].ParameterType)
        Quotations.Expr.NewDelegate(funcTyDef.MakeGenericType(strTy, m.ReturnType), [x], Quotations.Expr.Call(m, (Quotations.Expr.Var x)::args))
    
    let parseIntMeth = intTy.GetMethod("Parse", [|strTy; numStyTy; iFmtProTy|])
    let parseInt64Meth = int64Ty.GetMethod("Parse", [|strTy; numStyTy; iFmtProTy|])
    let parseBoolMeth = boolTy.GetMethod("Parse", [|strTy|])
    let parseFloatMeth = floatTy.GetMethod("Parse", [|strTy; numStyTy; iFmtProTy|])
    let parseDateMeth = dateTy.GetMethod("Parse", [|strTy; iFmtProTy; dateStyTy|])
    
    let csvFileImplTyDefn = runtimeAssembly.GetType("Samples.Csv.Runtime.CsvFileImpl`1")

    let uriTy = csvFileImplTyDefn.GetConstructors().[0].GetParameters().[0].ParameterType

    let ns = "Samples.Csv"
    let csvTy = ProvidedTypeDefinition(runtimeAssembly, ns, "CsvFile", Some(objTy))
    let filename = ProvidedStaticParameter("File", strTy)
    let delim = ProvidedStaticParameter("Delimiter", charTy, ',')
    let quote = ProvidedStaticParameter("Quote", charTy, '"')
    let culture = ProvidedStaticParameter("Culture", strTy, "")
    let schema = ProvidedStaticParameter("Schema", strTy, "")
    let inferTypes = ProvidedStaticParameter("InferTypes", boolTy, false)
    let inferRows = ProvidedStaticParameter("InferRows", intTy, 0)
    let ignoreErrors = ProvidedStaticParameter("IgnoreErrors", boolTy, false)

    let helpText = """<summary>Typed representation of a CSV file</summary>
                      <param name='File'>CSV file location</param>
                      <param name='Delimiter'>Column delimiter</param>
                      <param name='Quote'>TheQuotation mark (for surrounding values containing the delimiter)</param>
                      <param name='Culture'>The culture used for parsing numbers and dates.</param>
                      <param name='Schema'>Optional column types, in a comma separated list.  Valid types are "string", "int", "int64", "float", "bool", "date", "int?", "int64?", "float?", "bool?", and "date?".</param>
                      <param name='InferTypes'>Whether to infer types from actual data.  This option is incompatible with an explicit schema.</param>
                      <param name='InferRows'>Number of rows to use for inference (if inferTypes is true).  If this is zero (the default), all rows are used.</param>
                      <param name='IgnoreErrors'>Whether to ignore rows that have the wrong number of columns or which can't be parsed using the inferred or specified schema.  Otherwise an exception is thrown when these rows are encountered.</param>"""

    do csvTy.AddXmlDoc helpText
    do csvTy.DefineStaticParameters([filename; delim; quote; culture; schema; inferTypes; inferRows; ignoreErrors], memo (fun tyName [| :? string as filename; :? char as delim; :? char as quote; :? string as culture; :? string as schema; :? bool as inferTypes; :? int as inferRows; :? bool as ignoreErrors |] ->
        let culture = System.Globalization.CultureInfo(culture)
        let uri =
#if SILVERLIGHT
#else
            if System.Uri.IsWellFormedUriString(filename, System.UriKind.Relative) then
                System.Uri(System.IO.Path.Combine(Path.Combine(cfg.ResolutionFolder, filename)))
            else // note: this just works for full paths even without leading "file:///"
#endif
                System.Uri(filename)

        let headerLine, otherLines = 
#if FX_NO_LOCAL_FILESYSTEM
#else
            if uri.IsFile then
                let lines = File.ReadLines(uri.LocalPath)
                let en = lines.GetEnumerator()
                en.MoveNext() |> ignore
                en.Current, seq {
                    while en.MoveNext() do
                        yield en.Current
                    en.Dispose()
                } |> Seq.cache
            else
#endif
                let client = new System.Net.WebClient()
                client.AsyncDownloadString(uri) 
                |> Async.RunSynchronously
                |> fun s -> 
                    let arr = s.Split([|'\n';'\r'|], System.StringSplitOptions.RemoveEmptyEntries)
                    arr.[0], arr.[1..] :> seq<_>

        // extract header names from the file, splitting on delimiters
        // we use Regex matching so that we can get the position in the row at which the field occurs
        // TODO: handle escaped delimiters
        let splitReg = Regex(sprintf "^((^|(?<=%c))((%c(?<data>[^%c]*)%c)|(?<data>[^%c]*))(%c|$))*$" delim quote quote quote delim delim, RegexOptions.ExplicitCapture)
        let components line = [| for field in splitReg.Match(line).Groups.["data"].Captures -> field.Value |]
        let headers = splitReg.Match(headerLine).Groups.["data"].Captures

        let schemaFields = 
            if String.IsNullOrEmpty schema then
                null
            else
                let components = schema.Split(delim) 
                if components.Length <> headers.Count then
                    failwith (sprintf "Schema length was %i, but %i headers were found" components.Length headers.Count)
                components       

        let tyToParser = 
            [intTy,         fun e c -> Quotations.Expr.Call(parseIntMeth, [e; numStyInt; c])
             int64Ty,       fun e c -> Quotations.Expr.Call(parseInt64Meth, [e; numStyInt; c])
             boolTy,        fun e _ -> Quotations.Expr.Call(parseBoolMeth, [e])
             floatTy,       fun e c -> Quotations.Expr.Call(parseFloatMeth, [e; numStyFlt; c])
             dateTy,        fun e c -> Quotations.Expr.Call(parseDateMeth, [e; c; dateStyNone])
             strTy,         fun e _ -> e // identity conversion
             intOptTy,      fun e c -> Quotations.Expr.Call(makeOptMeth.MakeGenericMethod(intTy), [mToF parseIntMeth [numStyInt; c]; e])
             int64OptTy,    fun e c -> Quotations.Expr.Call(makeOptMeth.MakeGenericMethod(int64Ty), [mToF parseInt64Meth [numStyInt; c]; e])
             boolOptTy,     fun e _ -> Quotations.Expr.Call(makeOptMeth.MakeGenericMethod(boolTy), [mToF parseBoolMeth []; e])
             floatOptTy,    fun e c -> Quotations.Expr.Call(makeOptMeth.MakeGenericMethod(floatTy), [mToF parseFloatMeth [numStyFlt; c]; e])
             dateOptTy,     fun e c -> Quotations.Expr.Call(makeOptMeth.MakeGenericMethod(dateTy), [mToF parseDateMeth [c; dateStyNone]; e])]
            |> dict

        let nameToTy =
            ["int" ,    intTy  
             "int64",   int64Ty
             "bool",    boolTy 
             "float",   floatTy
             "date",    dateTy
             "string",  strTy 
             "int?",    intOptTy   
             "int64?",  int64OptTy
             "bool?",   boolOptTy
             "float?",  floatOptTy
             "date?",   dateOptTy]
            |> dict

        // get the index, name, type, conversion, and location of each field
        let fieldInfos = 
            [for i in 0 .. headers.Count - 1 do
                let headerText = headers.[i].Value
                let name, ty =
                    if inferTypes then
                        let ty =
                            let rows = 
                                otherLines 
                                |> Seq.filter (fun row -> not <| String.IsNullOrEmpty row)
                                |> Seq.map (fun row -> components row)

                            if inferRows > 0 then
                                Seq.truncate inferRows rows
                            else rows
                            |> Seq.filter (fun row -> row.Length = headers.Count || not ignoreErrors)
                            |> Seq.map (fun row -> row.[i])
                            |> Seq.fold (fun ty str -> generalize ty (typeFromStr culture str)) topTy
                        headerText, if ty = topOptTy then strTy else ty
                    elif String.IsNullOrEmpty schema then
                        // try to decompose this header into a name and type
                        let m = Regex.Match(headerText, @"^(?<name>.+)( \((?<type>.+)\))?$")
                        let fieldTy = 
                            if m.Groups.["type"].Success then
                                match nameToTy.TryGetValue(m.Groups.["type"].Value) with
                                | true, ty -> ty
                                | _ -> strTy
                            else strTy
                        m.Groups.["name"].Value, fieldTy                        
                    else
                        headerText, nameToTy.[schemaFields.[i]]
                yield i, name, ty, headers.[i].Index + 1]
        // get the tuple type that the rows will erase to
        let tupleTy = 
            Reflection.FSharpType.MakeTupleType([| for (_,_,ty,_) in fieldInfos -> ty |])
      
        // define a provided type for the row, erasing to that tuple type
        let rowTy = ProvidedTypeDefinition("Row", Some(tupleTy))

        // add one property per CSV field
        for (i,fieldName,fieldTy,fieldLocation) in fieldInfos do

            let prop = ProvidedProperty(fieldName, fieldTy, GetterCode = fun [row] -> Quotations.Expr.TupleGet(row, i))

            // Add metadata defining the property's location in the referenced file so that we can "Go to definition"
            prop.AddDefinitionLocation(1, fieldLocation, filename)

            rowTy.AddMember(prop)
        
        // based on the set of fields, create a function that converts a string[] to the tuple type
        let convertStrings = 
            let strs = Quotations.Var("strs", strTy.MakeArrayType())
            let cul = Quotations.Var("cul", cultureInfoTy)

            // convert each element of strs using the appropriate conversion
            let convertedItems =
                [for (i,_,ty,_) in fieldInfos do
                    let conv = tyToParser.[ty]
                    yield conv (Quotations.Expr.Call(arrGetMeth, [Quotations.Expr.Var strs; Quotations.Expr.Value i])) (Quotations.Expr.Var cul)]
                    
            // then build a tuple out of them
            let tup =
                let culObj = Quotations.Expr.NewObject(cultureInfoCtor, [Quotations.Expr.Value culture.Name])
                Quotations.Expr.Let(cul, culObj, Quotations.Expr.NewTuple(convertedItems))

            Quotations.Expr.NewDelegate(funcTyDef.MakeGenericType(strTy.MakeArrayType(), tupleTy), [strs], tup)

        let csvFileImpl = csvFileImplTyDefn.MakeGenericType(rowTy)
        let csvFileImplErased = csvFileImplTyDefn.MakeGenericType(tupleTy)
                
        let csvFileCtor = csvFileImplErased.GetConstructors().[0]

        // define the provided type, erasing to CsvFileImpl<rowTy>
        let ty = ProvidedTypeDefinition(runtimeAssembly, ns, tyName, Some(csvFileImpl))

        // have to be careful to call GetConstructors on csvErasedTy since it will throw an exception if called on csvTy
        let callCtor e = Quotations.Expr.NewObject(csvFileCtor, [e; Quotations.Expr.Value delim; Quotations.Expr.Value quote; Quotations.Expr.Value ignoreErrors; convertStrings])

        // Add a parameterless constructor which loads the file that was used to define the schema
        ty.AddMember(ProvidedConstructor([], InvokeCode = fun [] -> callCtor (Quotations.Expr.NewObject(uriTy.GetConstructor([|strTy|]), [Quotations.Expr.Value (string uri)]))))

        // Add another constructor which loads a specific file assuming it has the same schema
        ty.AddMember(ProvidedConstructor([ProvidedParameter("uri", uriTy)], InvokeCode = fun [uri] -> callCtor uri))

        ty.AddMember(rowTy)
        ty))
    do this.AddNamespace(ns, [csvTy])

