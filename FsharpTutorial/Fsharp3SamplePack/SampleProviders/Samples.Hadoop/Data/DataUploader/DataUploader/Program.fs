module DataUploader 
open System
open System.IO


open System.Data.Odbc

type DBasic =
    | DInt 
    | DInt64
    | DFloat 
    | DBoolean 
    | DString 

let tables = 
        [
        (@"Abalone/abalone.data","Abalone", "<para>Abalone data</para><para>Number of Instances: 4177</para><para>Predicting the age of abalone from physical measurements.  The age of abalone is determined by cutting the shell through the cone, staining it, and counting the number of rings through a microscope -- a boring and time-consuming task.  Other measurements, which are easier to obtain, are used to predict the age.</para>", 
            [
                ("Sex","M, F, and I (infant)  (required)",DString)
                ("Length","Longest shell measurement (unit=mm, required)",DFloat)
                ("Diameter","Diameter perpendicular to length (unit=mm, required)",DFloat)
                ("Height","Height with meat in shell (unit=mm, required)",DFloat)
                ("WholeWeight","hole abalone (unit=g, required)",DFloat)
                ("ShuckedWeight","weight of meat (unit=g, required)", DFloat)
                ("VisceraWeight","gut weight (after bleeding) (unit=g, required)", DFloat)
                ("ShellWeight"," after being dried (unit=g, required)", DFloat)
                ("Rings","+1.5 gives the age in years (required)", DInt)
            ])
        (@"BankMarketing/bank.data", "BankMarketing", "<para>Bank Marketing</para><para>Number of Instances: 45211</para><para>The data is related with direct marketing campaigns of a Portuguese banking institution. The marketing campaigns were based on phone calls. Often, more than one contact to the same client was required, in order to access if the product (bank term deposit) would be (or not) subscribed. The classification goal is to predict if the client will subscribe a term deposit (variable y).</para>", 
            [
            //   # bank client data:
                ("age","age (numeric) (required)",DInt)
                ("job","type of job (categorical: admin.,unknown,unemployed,management,housemaid,entrepreneur,student,blue-collar,self-employed,retired,technician,services) (required)", DString)
                ("marital","marital status (categorical: married,divorced,single; note: divorced means divorced or widowed) (required)",DString)
                ("education", "(categorical: unknown,secondary,primary,tertiary) (required)", DString)
                ("default", "has credit in default? (binary: yes,no) (required)", DString)
                ("balance", "balance: average yearly balance, in euros (numeric) (required)", DFloat)
                ("housing", "has housing loan? (binary: yes,no) (required)", DString)
                ("loan", "has personal loan? (binary: yes,no) (required)", DString)
            //   # related with the last contact of the current campaign:
                ("contact", "communication type (categorical: unknown,telephone,cellular) (required)", DString)
                ("day", "last contact day of the month (numeric) (required)", DInt)
                ("month", "last contact month of year (categorical: jan, feb, mar, ..., nov, dec) (required)", DString)
                ("duration", "last contact duration, in seconds (numeric) (required)", DInt)
            /// # other attributes
                ("campaign", "number of contacts performed during this campaign and for this client (numeric, includes last contact) (required)", DInt)
                ("pdays", "number of days that passed by after the client was last contacted from a previous campaign (numeric, -1 means client was not previously contacted) (required)", DInt)
                ("previous", "number of contacts performed before this campaign and for this client (numeric) (required)", DInt)
                ("poutcome","outcome of the previous marketing campaign (categorical: unknown,other,failure,success) (required)", DString)
                ("y", "has the client subscribed a term deposit? (binary: yes,no) (required)", DString)
            ])
        (@"BreastCancer/wdbc.data", "BreastCancer", "<para>Wisconsin Diagnostic Breast Cancer (WDBC)</para><para>Number of instances: 569 (357 benign, 212 malignant)</para><para>Features are computed from a digitized image of a fine needle aspirate (FNA) of a breast mass.  They describe characteristics of the cell nuclei present in the image.</para>(required)", 
            //   # bank client data:
                ([("ID","ID number (required)",DInt);("Diagnosis","(M = malignant, B = benign) (required)", DString)],[3..32] |> List.map (fun x -> ("Col" + x.ToString(),"(required)",DFloat)))
                ||> List.append)
        (@"CarEvaluation/car.data","CarEvaluation", "<para>Car Evaluation Database</para><para>Number of Instances: 1728</para><para>Car Evaluation Database was derived from a simple hierarchical decision model. Because of known underlying concept structure, this database may be particularly useful for testing constructive induction and structure discovery methods.</para>(required)",
            [
                ("buying","v-high, high, med, low (required)",DString)
                ("maint","v-high, high, med, low (required)", DString)
                ("doors", "2, 3, 4, 5-more (required)", DInt)
                ("persons","2, 4, more (required)", DInt)
                ("log_boot","small, med, big (required)", DString)
                ("safety","low, med, high (required)",DString)
            ])
        (@"Iris/iris.data","Iris", "<para>Iris Plants</para><para>Number of Instances: 150 (50 in each of three classes)</para><para>The data set contains 3 classes of 50 instances each, where each class refers to a type of iris plant.  One class is linearly separable from the other 2; the latter are NOT linearly  separable from each other.</para>",
            [
                ("sepalLength", "sepal length in cm (unit=cm, required)", DFloat)
                ("sepalWidth", "sepal width in cm (unit=cm, required)", DFloat)
                ("petalLength", "petal length in cm (unit=cm, required)", DFloat)
                ("petalWidth", "petal width in cm (unit=cm, required)", DFloat)
                ("class", "Iris Setosa, Iris Versicolour, Iris Virginica (required)", DString)
            ])
        (@"PokerHand/poker-hand.data","PokerHand", "<para>Poker Hand Dataset</para><para>Number of Instances: 1,000,000</para><para>Each record is an example of a hand consisting of five playing cards drawn from a standard deck of 52. Each card is described using two attributes (suit and rank), for a total of 10 predictive attributes. There is one Class attribute that describes the Poker Hand. The order of cards is important, which is why there are 480 possible Royal Flush hands as compared to 4 (one for each suit – explained in more detail below).</para>",
            [
                ("S1", "Suit of card #1 Ordinal (1-4) representing {Hearts, Spades, Diamonds, Clubs} (required)", DInt)
                ("C1", "Rank of card #1 Numerical (1-13) representing (Ace, 2, 3, ... , Queen, King) (required)", DInt)
                ("S2", "Suit of card #2 Ordinal (1-4) representing {Hearts, Spades, Diamonds, Clubs} (required)", DInt)
                ("C2", "Rank of card #2 Numerical (1-13) representing (Ace, 2, 3, ... , Queen, King) (required)", DInt)
                ("S3", "Suit of card #3 Ordinal (1-4) representing {Hearts, Spades, Diamonds, Clubs} (required)", DInt)
                ("C3", "Rank of card #3 Numerical (1-13) representing (Ace, 2, 3, ... , Queen, King) (required)", DInt)
                ("S4", "Suit of card #4 Ordinal (1-4) representing {Hearts, Spades, Diamonds, Clubs} (required)", DInt)
                ("C4", "Rank of card #4 Numerical (1-13) representing (Ace, 2, 3, ... , Queen, King) (required)", DInt)
                ("S5", "Suit of card #5 Ordinal (1-4) representing {Hearts, Spades, Diamonds, Clubs} (required)", DInt)
                ("C5", "Rank of card #5 Numerical (1-13) representing (Ace, 2, 3, ... , Queen, King) (required)", DInt)
                ("Class", "0 (low) to 9 (high) (required)", DInt)
            ])
        (@"WineQualityWhite/winequality-white.data", "WineQualityWhite",  "<para>Wine Quality</para><para>Number of Instances: white wine - 4898.</para><para>Dataset rleatest to the white variants of the Portuguese \"Vinho Verde\" wine. Due to privacy and logistic issues, only physicochemical (inputs) and sensory (the output) variables are available (e.g. there is no data about grape types, wine brand, wine selling price, etc.).These datasets can be viewed as classification or regression tasks. The classes are ordered and not balanced (e.g. there are munch more normal wines than excellent or poor ones). Outlier detection algorithms could be used to detect the few excellent or poor wines. Also, we are not sure if all input variables are relevant. So it could be interesting to test feature selection methods.</para>",
            [
                ("fixedAcidity","(unit=mg/l, required)",DFloat)
                ("volatileAcidity","(unit=mg/l, required)",DFloat)
                ("citricAcid","(unit=mg/l, required)",DFloat)
                ("residualSugar","(unit=g/l, required)",DFloat)
                ("chlorides","(unit=mg/l, required)",DFloat)
                ("freeSulfurDioxide","(unit=mg/l, required)",DFloat)
                ("totalSulfurDioxide","(unit=mg/l, required)",DFloat)
                ("density","wine desnisty devide (unit=g/cm^3, required)",DFloat)
                ("pH","pH (required)",DFloat)
                ("sulphates","(unit=mg/l, required)",DFloat)
                ("alcohol","percentage by volume (required)",DFloat)
                ("quality","(required)",DInt)
            ])
        ]

[<EntryPoint>]
let main args =
    let uploadData(address:string,
                    hivePort:int, 
                    hiveCredentials:(string*string) option) =
//    let uploadData(address:string,hdfsPort:int,
//                        hivePort:int,hdfsUserName:string option, 
//                        hiveCredentials:(string*string) option, 
//                        localFileLocation:string, hdfsFileLocation:string) =
        
//        let hdfs =
//            match hdfsUserName with
//            | Some(userName) -> HdfsFileSystem.Connect(address, hdfsPort |> uint16, userName)
//            | None -> HdfsFileSystem.Connect(address, hdfsPort |> uint16)

        

//        let transfer (fromLocalPath:string) (toHDFSPath:string) =              
//            use fromFile = File.OpenRead(fromLocalPath)
//            use toFile = hdfs.OpenFileStream(toHDFSPath, HdfsFileAccess.Write)
//            printfn "From: %s To: %s" fromLocalPath toHDFSPath
//            fromFile.CopyTo(toFile)  

//        let listFiles (path:string) = 
//                let info = DirectoryInfo(path)
//                (info.FullName.Length, info.GetFiles("*",SearchOption.AllDirectories) |> Array.map (fun x -> x.FullName))
//
//        let listTransfers1 (fromPath:string) (toPath:string) = 
//            let (pathLength,list) = listFiles fromPath 
//            list |> Array.map  (fun x -> (x,toPath + x.Substring(pathLength)))
//
//        let transfers =  listTransfers1 localFileLocation hdfsFileLocation
//
//        transfers
//            |> Array.iteri (fun i (fromPath,toPath) -> 
//                printfn "%d of %d" i (transfers.Length - 1)
//                transfer fromPath toPath)

        let createHiveTable (basePath:string) (tablePath : string ,tableName : string , comment:string, columnDefs : (string * string * DBasic) list) =
            [   yield sprintf "CREATE EXTERNAL TABLE IF NOT EXISTS %s (" tableName
                yield columnDefs |> List.map (fun (colName, colDesc, colType) ->
                    let hiveType =
                        match colType with
                        | DInt -> "INT"
                        | DInt64 -> "BIGINT"
                        | DFloat -> "DOUBLE"
                        | DBoolean -> "BOOLEAN"
                        | DString -> "STRING"
                    sprintf "\t%s %s COMMENT '%s'" colName hiveType colDesc)
                    |> function
                    | [x] -> x
                    | xs -> xs |> List.reduce (fun x y -> x + ",\n" + y)
                yield ")"
                yield sprintf "COMMENT '%s'" comment
                yield "ROW FORMAT DELIMITED FIELDS TERMINATED BY '44'"
                yield "STORED AS TEXTFILE"
                yield sprintf "LOCATION '%s';" (System.IO.Path.GetDirectoryName(basePath + "/" + tablePath).Replace("\\","/") + "/")
            ] |> List.reduce (fun x y -> x + "\n" + y)

        let loadHiveTable (basePath:string) (tablePath,tableName : string , _, _) = sprintf "LOAD DATA INPATH '%s/%s' INTO TABLE %s;" basePath tablePath tableName
        let dropHiveTable (_,tableName : string , _, _) = sprintf "DROP TABLE %s;" tableName

        //let cns = 
        let conn =
            let cns = 
                match hiveCredentials with
                | Some((username,password)) -> sprintf "Driver=HIVE;Host=%s;AUTHENTICATION=3;Port=%d;UID=%s;PWD=%s" address hivePort username password
                | None -> sprintf "Driver=HIVE;Host=%s;Port=%d;" address hivePort
            new OdbcConnection(cns)

        conn.Open()

        let runCommand (cmd:string) = 
                let command = conn.CreateCommand()
                command.CommandText <- cmd
                command.ExecuteReader()

        (* Drop HIVE tables *)
        printfn "Drop HIVE tables "

        tables |> List.map (dropHiveTable) |> List.map runCommand |> ignore

        (* Load HIVE tables *)
        printfn "Load HIVE tables"

        tables |> List.map (createHiveTable "/Data") |> List.map runCommand |> ignore

    if args.Length % 2 <> 0  || args 
            |> Seq.mapi (fun i x -> (i,x)) 
            |> Seq.exists (fun (i,x) -> i % 2 = 0  && not (x.StartsWith("--")))
    then failwith "arguments are mallformed - must be of form key value list where keys are prepended with \"--\""
    
    let argMap = 
        args 
            |> Seq.pairwise
            |> Seq.mapi (fun i x -> (i,x))
            |> Seq.filter (fun (i,x) -> i % 2 = 0)
            |> Seq.map snd
            |> Seq.fold (fun (map:Map<string,string>) x -> map.Add(fst x, snd x)) Map.empty

    
    let parameters =
        [|
            ("host","server",Some("127.0.0.1"))
            //("hdfsPort","hdfs port",Some("9000"))
            ("hivePort","hive port",Some("10000"))
            //("localFileSystemPath","path to local data directory",Some("..\..\..\..\FSharpData"))
            //("hdfsFileSystemPath","path to data directory on hdfs",Some("/data/"))
            //("hdfsUserName","hdfs user name",None)
            ("hiveUserName","hive user name",None)
            ("hivePassword","hive password",None)
        |] 
    
    let extractedParameters =  
        parameters
            |> Array.map (fun (x,_,y) ->
                match argMap.TryFind("--" + x) with
                | Some(x) -> Some(x)
                | None -> y)

    match extractedParameters with
    | [|Some(host);(*Some(hdfsPort);*)Some(hivePort);(*Some(localFileSystem);Some(hdfsFileSystem);hdfsUserName;*)hiveUserName;hivePassword|] 
        when (hiveUserName.IsSome && hivePassword.IsSome) || (hiveUserName.IsNone && hivePassword.IsNone) -> 
        
        let hiveCredentials =
            match hiveUserName,hivePassword with
            | Some(username),Some(password) -> Some(username,password)
            | None,None -> None
            | _ -> failwith "should not happen"
        //uploadData(host,hdfsPort |> int,hivePort |> int,hdfsUserName, hiveCredentials, localFileSystem, hdfsFileSystem)
        uploadData(host,hivePort |> int, hiveCredentials)
    | _ -> 
        
        printfn "given arguments: %A" args
        printfn "argument list:"
        parameters 
            |> Seq.map (fun (x,y,z) -> 
                match z with 
                | Some(a) -> sprintf "    --%s //%s e.g. %s" x y a
                | None -> sprintf "    --%s //%s" x y)
            |> String.concat "\n"
            |> printfn "%s"
    
    Console.WriteLine("Finished")
    Console.ReadLine() |> ignore    
    0