module HDFS

open System
open System.Text
open System.Linq
open System.Collections.Concurrent
open System.Net
open System.Net.Sockets
open System.IO
open Microsoft.FSharp.Control.WebExtensions
open Microsoft.FSharp.Collections
open System.Threading
open System.Collections.Generic
open System.Diagnostics
open System.ComponentModel 
open System.Configuration.Install 
open System.ServiceProcess
open SerDes
open Microsoft.Hdfs
open HdfsSchema
open System.IO.Pipes
open System.Reflection

let connectionDict = new ConcurrentDictionary<(string*int*string option),HdfsFileSystem>()
let mockServer = "tryfsharp"
let hdfsHandler (address:string) (port:int) (user:string option) (msg:HdfsRecv) =
    try
        async {
#if INCLUDE_MOCK_DATA
            open Samples.Hadoop.Internals.Data
            if (address = mockServer) 
            then
                let getText name = (new System.IO.StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(name))).ReadToEnd()

                let depth (x:string) = x.ToCharArray() |> Array.sumBy (fun x -> if x = '\\' then 1 else 0)

                let directories = 
                    [|  yield ("\\",None)
                        yield ("\\Data\\",None)
                        for table in EmbeddedData.tables -> ("\\Data\\" + table.HiveTable + "\\", Some(table.HdfsInfo))
                        yield ("\\example\\", None)
                        yield ("\\example\\apps\\", None)
                        yield ("\\example\\data\\", None)
                        yield ("\\hdfs\\", None)
                        yield ("\\hive\\", None)
                        yield ("\\hive\\warehouse\\", None)
                        yield ("\\tmp\\", None)
                        yield ("\\uploads\\", None)
                        yield ("\\user\\",None)
                        yield ("\\user\\SYSTEM\\",None) |] 
                let directoriesTab = directories |> Map.ofArray

                let files = 
                    [|  let basePath = "\\"
                        for table in EmbeddedData.tables do
                            let tableBase = basePath + "Data\\" + table.HiveTable
                            yield (tableBase + "\\data", getText table.DataPath)
                            yield (tableBase + "\\.info", table.HdfsInfo)
                            yield (tableBase + "\\.names",table.Names)
                            yield (tableBase + "\\.source", table.Source) |] 

                let filesTab = files |> Map.ofArray

                let getFiles (path:string) =
                    let pathDepth = path |> depth
                    files 
                    |> Array.choose (fun x -> 
                        let name,data = x; 
                        // check if it is the correct level
                        if name.StartsWith(path) && (pathDepth = depth name)
                        then Some(name,data)
                        else None)
                      
                let getDirectores (path:string) =
                    let pathDepth = path |> depth
                    directories 
                    |> Array.choose (fun (name,info) -> 
                        // check if it is the correct level
                        if name.StartsWith(path) && (pathDepth + 1 = depth name)
                        then Some(name,info)
                        else None)
                
                return 
                  match msg with
                    | GetDirectoryContents(path) -> 
                        let strip (x : string) = x.Split([|'\\'|],System.StringSplitOptions.RemoveEmptyEntries) |> Array.rev |> Seq.head
                        let dirs = getDirectores path |> Array.map (fst >> strip)
                        let files = getFiles path |> Array.map (fst >> strip) 
                        (dirs,files) |> DirectoryContents

                    | GetFileInfo(path) ->
                        {
                            BlockSize = 0L
                            Group = "none"
                            IsDirectory =  directoriesTab.ContainsKey(path) 
                            LastAccessed = DateTime.Now
                            LastModifies = DateTime.Now
                            Name = "none"
                            Owner = "none"
                            Permissions = 0s
                            Replication = 0s
                            Size = 0L
                        }|> Some |> FileInfo
                    | Copy(from,to') -> CopyResult false
                    | Move(from,to') -> MoveResult false                        
                    | GetFileHead(path,length) -> 
                        
                        let path' =  if path.EndsWith(".info") then path else path.Substring(0,path.Length - 1)
                        match filesTab.TryFind(path') with 
                        | Some(x) ->
                            let rows = x.Split([|'\n'|])
                            let l = Math.Min(rows.Length-1,length)
                            rows |> Seq.take l |> Array.ofSeq |> FileHead
                        | None -> FileHead([||])
                        
                    | GetFileTail(path, length) -> FileTail([||])    
                    | GetFileData(path) -> 
                        let path' = if path.EndsWith(".info") then path else path.Substring(0,path.Length - 1)

                        match filesTab.TryFind(path') with     
                        | Some(x) -> System.Text.UTF8Encoding.UTF8.GetBytes(x) |> FileData
                        | None -> FileData([||])
                    | DeleteFile(path) -> DeleteResult false
                    | WriteFile(path,data) -> WriteFileResult false
            else
#endif
                let hdfs = 
                        match connectionDict.TryGetValue((address,port,user)) with
                        | (true,conn) -> conn
                        | (false,_) ->
                            let conn =
                                match user with
                                | Some(user) -> HdfsFileSystem.Connect(address,uint16 port, user)
                                | None -> HdfsFileSystem.Connect(address,uint16 port)
                            connectionDict.TryAdd((address,port,user),conn) |> ignore
                            conn
    
                let hdfs = HdfsFileSystem.Connect(address,uint16 port) 

                return 
                  match msg with
                    | GetDirectoryContents(path) -> 
                        match hdfs.ListDirectory(path).Entries with
                            | null -> ([||],[||]) |> DirectoryContents
                            | es -> es |> Seq.toArray |> Array.partition (fun e -> e.Kind = HdfsFileInfoEntryKind.Directory)
                                       |> fun (dirs,files) -> (dirs |> Array.map (fun e -> System.IO.Path.GetFileName(e.Name)), files |> Array.map (fun e -> System.IO.Path.GetFileName(e.Name)))
                                       |> DirectoryContents                
                    | GetFileInfo(path) ->
                        let info = hdfs.GetPathInfo(path)
                        {
                            BlockSize = info.BlockSize
                            Group = info.Group
                            IsDirectory =  (info.Kind = HdfsFileInfoEntryKind.Directory)
                            LastAccessed = info.LastAccessed
                            LastModifies = info.LastModified
                            Name = info.Name
                            Owner = info.Owner
                            Permissions = info.Permissions
                            Replication = info.Replication
                            Size = info.Size
                        }|> Some |> FileInfo
                    | Copy(from,to') -> CopyResult false
                    | Move(from,to') -> MoveResult false                        
                    | GetFileHead(path,length) -> 
                        let file = hdfs.OpenFileForRead(path)
                        try
                            try
                                [|for _ in 0..length -> file.ReadLine()|] |> FileHead
                            finally 
                                file.Dispose()
                        with
                        | _ -> FileHead([||])
                    | GetFileTail(path, length) -> FileTail([||])
                    | GetFileData(path) -> 
                        let file = hdfs.OpenFileForRead(path)
                        let ms = new MemoryStream()
                        let buf = Array.zeroCreate 8192
                        let rec impl () = 
                            let read = file.ReadBytes(buf, 0, buf.Length) 
                            if read > 0 then 
                                ms.Write(buf, 0, read)
                                impl ()
                        impl ()                        
                        let x = ms.ToArray() |> FileData
                        file.Dispose()
                        x
                    | DeleteFile(path) -> DeleteResult false
                    | WriteFile(path,data) -> WriteFileResult false
                } 
            |> fun (x:Async<HdfsSend>) -> Async.RunSynchronously(x,1000)
        with
        | ex -> Exception(ex.Message)

let getBytes (address:string) (port:int) (user:string option) (input:byte[])   =
    SerDes.fromBytes<HdfsRecv> input
    |> hdfsHandler address port user
    |> SerDes.toBytes

 
let HdfsRequest  (url:string, inBytes:byte[]) = 
    let req = System.Web.HttpUtility.ParseQueryString(url) 
    let address = req.["Address"]
    let port = Int32.Parse(req.["Port"])
    let outBytes = 
        if req.AllKeys.Contains("User") 
        then getBytes address port (Some(req.["User"])) inBytes
        else getBytes address port None inBytes
    outBytes
