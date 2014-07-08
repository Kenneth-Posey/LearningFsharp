// HDFS Type provider implementation
//
//    - HDFS is not supported by Azure/Hadoop so this bit is of limited use for that scenario.
//
//    - We could consider using FTP as the protocol to implement this provider on Azure/Hadoop but 
//      when we looked into this we couldn't make much headway - e.g. we couldn't connect to the FTP service. It might
//      might have been something simple but we don't know.  We have asked Matt Winkler about this.
// 
//    - The programming model for File and Directory objects is valuable but not yet fully designed.  We would add
//         - dir.GetMatchingFiles("...") (getting the file names based on regular expression matching)
//         - dir.CopyLocal ??
//         - file.CopyLocal ??
//         - also look at all HDFS commands that do not change the schema
//         - also consider HDFS commands that do change the schema, with the caveat that we would have to restart and 
//           it is not replicable w.r.t. strong typing.

namespace Samples.Hadoop.Internals.HdfsProvider

open System
open System.Reflection
open Samples.FSharp.ProvidedTypes
open Samples.Hadoop.Internals
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open HiveSchema    
open HdfsSchema    
    
[<AutoOpen>]
module internal Helpers = 
    let unexpected exp msg = 
        match msg with 
        | Exception(ex)  -> failwith ex
        | _ -> failwithf "unexpected message, expected %s, got %+A" exp msg

    let encode s = 
#if BROWSER
        s
#else
        System.Web.HttpUtility.HtmlEncode(s)
#endif

/// TODO: add helpful info here
type File(path:string, param) =
    let fetch = makeFetcher param
    member this.Path = path
    member inthis.FileInfo() = fetch(GetFileInfo(path)) |> function | FileInfo(Some(fie)) -> fie | data -> unexpected "FileInfo" data
    
    member this.ReadAllBytes() = fetch(GetFileData(path)) |> function | FileData(data) -> data | data -> unexpected "FileData" data
    member this.ReadAllLines() = fetch(GetFileData(path)) |> function | FileData(data) -> System.Text.UTF8Encoding.UTF8.GetString(data,0,data.Length).Split([|'\n'|]) | data -> unexpected "FileData" data

    member this.Head(length:int) = fetch(GetFileHead(path,length)) |> function | FileHead(ss) -> ss | data -> unexpected "FileHead" data
    member this.Head() = this.Head(10)
    member this.Tail(length:int) = fetch(GetFileTail(path,length)) |> function | FileTail(ss) -> ss | data -> unexpected "FileTail" data
    member this.Tail() = this.Tail(10)

/// TODO: add helpful info here
type Directory(path:string, param) =
    let fetch = makeFetcher param
    member this.Path = path
    member this.WriteFile(fileName:string,data:byte[]) = fetch(WriteFile(path + "\\" + fileName,data)) |> function | WriteFileResult(r) -> r | data -> unexpected "WriteFileResult" data


[<TypeProvider>]
type public SampleTypeProvider(_config: TypeProviderConfig) as this = 
    inherit TypeProviderForNamespaces()

    let ns = "Samples.Hadoop"

    let asm = Assembly.GetExecutingAssembly()

  

    let hdfsTy = 
        let hdfsTyped = ProvidedTypeDefinition(asm, ns, "HdfsTypeProvider", Some(typeof<obj>), HideObjectMethods = true)
        let helpText = "<summary>Typed representation of HDFS</summary>
                        <param name='Host'>The HDFS connection host</param>
                        <param name='Port'>The HDFS connection port</param>
                        <param name='User'>The HDFS connection user</param>"
        let address = ProvidedStaticParameter("Host",typeof<string>)
        let port = ProvidedStaticParameter("Port",typeof<int>,9000)
        let usr = ProvidedStaticParameter("User",typeof<string>,"None")
            
            
#if BROWSER
        let defaultProxyUri = AsyncUtilities.RunOnMainThread(fun () -> System.Windows.Application.Current.Host.Source)
        let defaultProxyPrefix = System.Uri(defaultProxyUri,"/Proxy/hadoophdfs").ToString()
        let proxy = ProvidedStaticParameter("Proxy", typeof<string>, defaultProxyPrefix)
        let extraHelpText = "<param name='ProxyUrl'>The proxy for the HDFS service (default: " + defaultProxyPrefix + ")</param>"
        do hdfsTyped.AddXmlDoc(helpText + extraHelpText)
        do  hdfsTyped.DefineStaticParameters([proxy;address;port;usr], fun typeName [| :? string as proxy; :? string as address; :? int as port ;:? string as usr  |] ->
                let param = 
                    match usr with
                    | "None" -> sprintf "%s?&Address=%s&Port=%d" proxy address port
                    | x -> sprintf "%s?&Address=%s&Port=%d&User=%s" proxy address port x
#else
        do hdfsTyped.AddXmlDoc(helpText)
        do  hdfsTyped.DefineStaticParameters([address;port;usr], fun typeName providerArgs ->
                let (address,port,_usr) = 
                    match providerArgs with 
                    | [| :? string as address; :? int as port; :? string as usr  |] -> (address,port,usr) 
                    | args -> failwithf "unexpected arguments to type provider, got %A" args
                        
                let param = [|"HDFS";address;string port|] |> Array.reduce (fun x y -> x + " " + y)
#endif
                let fetch = makeFetcher<HdfsRecv,HdfsSend> param

                let getDirInfoDoc(path:string)() = 
                        fetch(HdfsSchema.GetDirectoryContents(path)) 
                        |> function
                        | HdfsSchema.DirectoryContents(_,files) ->
                            if files |> Array.exists (fun x -> x = ".info") 
                            then
                                fetch(HdfsRecv.GetFileData(path + ".info")) |> function 
                                    | FileData(data) -> System.Text.Encoding.UTF8.GetString(data,0,data.Length)
                                    | data -> unexpected "FileInfo" data
                            else 
                                sprintf "<summary>Directory: %s</summary>" (encode path)
                        | data -> unexpected "FileInfo" data
                                                        
                let getFile (path:string list) =
                    let path' = "\\" + (path |> List.fold (fun acc y -> y + "\\" + acc) "")
                    let p = ProvidedProperty(propertyName = path.Head, propertyType = typeof<File>, IsStatic=true, 
                                                GetterCode= (fun _ -> <@@ File(path',param) @@>))
                        
                    p.AddXmlDocDelayed(fun () -> 
                        let head = 
                            fetch(HdfsRecv.GetFileHead(path',10)) |> function 
                                | FileHead(xs) -> xs |> String.concat "\n"
                                | data -> unexpected "FileInfo" data
                        sprintf "<summary><para>File: %s</para><para>%s</para></summary>" (encode path') (encode head))
                    p

                let rec getDirectoryContents (path:string list) =
                    let dir = "\\" + (path |> List.fold (fun acc y -> y + "\\" + acc) "")
                    fetch(HdfsRecv.GetDirectoryContents(dir)) 
                    |> function
                    | DirectoryContents(dirs,files) -> 
                        [ for dir in dirs -> getDirectory (dir::path) :> MemberInfo
                          for file in files -> getFile (file::path) :> MemberInfo
                          let p = ProvidedProperty(propertyName = "DirectoryInfo", propertyType = typeof<Directory>, IsStatic=true, 
                                                    GetterCode = (fun _ -> <@@ Directory(dir,param) @@> )) 
                          p.AddXmlDoc("<summary>Runtime Helper Object</summary>")

                          yield (p :> MemberInfo)
                        ] 
                    | data -> unexpected "FileInfo" data
                    
                and getDirectory (path:string list) =
                    let t = ProvidedTypeDefinition(path.Head, baseType = Some typeof<obj>, HideObjectMethods = true)
                    t.AddMembersDelayed(fun () -> getDirectoryContents (path))
                    t.AddXmlDocDelayed(getDirInfoDoc("\\" + (path |> List.fold (fun acc y -> y + "\\" + acc) "")))
                    t                    
                
                let root = ProvidedTypeDefinition(asm, ns, typeName, baseType = Some typeof<obj>, HideObjectMethods = true)
                root.AddXmlDoc(getDirInfoDoc("\\")())
                root.AddMembers(getDirectoryContents [])
                root)
        hdfsTyped

    do this.AddNamespace(ns, [hdfsTy])