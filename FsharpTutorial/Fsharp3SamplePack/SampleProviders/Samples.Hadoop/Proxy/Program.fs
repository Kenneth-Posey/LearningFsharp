namespace Samples.Hadoop.TypeProvider.HadoopProxy
#if LIBRARY
#else
open Logging
#endif
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Control.WebExtensions
#if NO_HDFS
#else
open Microsoft.Hdfs
#endif
open SerDes
open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.ComponentModel 
open System.Configuration.Install 
open System.Diagnostics
open System.IO
open System.IO.Pipes
open System.Linq
open System.Net
open System.Net.Sockets
open System.ServiceProcess
open System.Text
open System.Threading

#if LIBRARY
#else
module ServerSideOfProxy =

    let access_policy = 
         @"<?xml version=""1.0"" encoding=""utf-8""?>
    <access-policy>
      <cross-domain-access>
        <policy>
          <allow-from http-request-headers=""*"">
            <domain uri=""*"" />
          </allow-from>
          <grant-to>
             <socket-resource port=""4502"" protocol=""http"" />
             <socket-resource port=""4503"" protocol=""http"" />
             <resource path=""/"" include-subpaths=""true""/>
          </grant-to>
        </policy>
      </cross-domain-access>
    </access-policy>" |> Encoding.UTF8.GetBytes

    let webServer (name:string) (prefixs:string[])   =  
        let cts = new CancellationTokenSource()
        let token = cts.Token
        sprintf "%s starting up on %A..." name prefixs |> Log |> log
        let httpListener = new HttpListener()
        for prefix in prefixs do httpListener.Prefixes.Add(prefix)
    
        let main = async {
            try
                httpListener.Start()          
                while not cts.IsCancellationRequested do
                    try
                        let! client = Async.FromBeginEnd(httpListener.BeginGetContext, httpListener.EndGetContext)
                        (name + " " + client.Request.HttpMethod + " request from " + client.Request.RemoteEndPoint.ToString()) |> Log |> log
                        let req = client.Request
                        let resp = client.Response
                        let input = client.Request.InputStream
                        let! inBytes = client.Request.InputStream.AsyncRead(int32 client.Request.ContentLength64)
                        input.Close()
                        let output = resp.OutputStream
                        if req.HttpMethod = "GET" then
                            resp.ContentLength64 <- int64 access_policy.Length
                            resp.ContentType <- "text/xml"
                            output.Write(access_policy,0,access_policy.Length)
#if NO_HIVE
#else
                        elif req.Url.LocalPath = "/Hive" && (client.Request.QueryString.AllKeys.Contains("Server")) 
                                                            && (client.Request.QueryString.AllKeys.Contains("Port")) then
                            let timeout = int32 client.Request.QueryString.["Timeout"]
                            let host = client.Request.QueryString.["Server"]
                            let port = int32 client.Request.QueryString.["Port"]
                            
                            let outBytes =
                                // The default case
                                let req = sprintf "Driver=HIVE;Host=%s;Port=%d" host port 
                                let req = 
                                    if client.Request.QueryString.AllKeys.Contains("Uid") && client.Request.QueryString.AllKeys.Contains("Pwd") then 
                                        match client.Request.QueryString.["Uid"], client.Request.QueryString.["Pwd"] with
                                        | "None",_ 
                                        | _,"None" -> req
                                        | uid, pwd -> sprintf "%s;UID=%s;PWD=%s" req uid pwd
                                    else
                                        req
                                let req = 
                                    if client.Request.QueryString.AllKeys.Contains("Auth") then 
                                        match client.Request.QueryString.["Auth"] with 
                                        | "None" -> req
                                        | auth -> sprintf "%s;AUTHENTICATION=%s" req auth
                                    else
                                        req
                                Hive.getBytes req timeout inBytes
                            resp.ContentType <- "application/x-www-form-urlencoded"                        
                            resp.ContentLength64 <- int64 outBytes.Length
                            output.Write(outBytes,0,outBytes.Length)
#endif
                        elif req.Url.LocalPath = "/HDFS" && (client.Request.QueryString.AllKeys.Contains("Address")) 
                                                            && (client.Request.QueryString.AllKeys.Contains("Port")) then
                            let address = client.Request.QueryString.["Address"]
                            let port = Int32.Parse(client.Request.QueryString.["Port"])
                            let outBytes = 
                                if client.Request.QueryString.AllKeys.Contains("User") 
                                then HDFS.getBytes address port (Some(client.Request.QueryString.["User"])) inBytes
                                else HDFS.getBytes address port None inBytes
                            resp.ContentType <- "application/x-www-form-urlencoded"                        
                            resp.ContentLength64 <- int64 outBytes.Length
                            output.Write(outBytes,0,outBytes.Length)
                        else
                            ()
                        output.Flush()
                        output.Close()   
                    with
                    | ex ->  Error("Unknown Error", ex.Message, ex.StackTrace) |> log
            finally
                httpListener.Stop()
        }
        Async.Start(main,token)
        { new IDisposable with 
            member x.Dispose() = 
                cts.Cancel()
        }

    let mutable server = None

    let run() =

        let prefix (ip:IPAddress) = sprintf "http://%s:%d/" (ip.ToString()) 8082 
        let prefixes = 
            [| for ip in Dns.GetHostAddresses(Dns.GetHostName())  do 
                   if ip.AddressFamily = Sockets.AddressFamily.InterNetwork then 
                       yield prefix ip 
               yield prefix IPAddress.Loopback |]

        let proxy = webServer "Hadoop Proxy" prefixes
        server <- Some({new IDisposable with member x.Dispose() = proxy.Dispose() })

    let stop() = match server with | Some(x) -> x.Dispose() | None -> ()
    open System.Security.Principal
    let start(args) =
        match args with
        | [||] -> 
            // By default, publish the service using the first IP address listed in DNS host addresses in this machine
            if not (WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)) then
                printfn "----------------------------------"
                printfn "WARNING: Do you need to run this program as administrator. Yes!!!"
                printfn "----------------------------------"
                printfn "Press any key to continue...."
                System.Console.ReadLine() |> ignore
            else
                run() 

        | _ -> failwith (sprintf "Incorrect arguments: %s\n\n " (args |> String.concat " "))

// This code allows the proxy to be installed as a Windows Service. It is not used if the proxy component is started manually.
type HDFSWindowsService() as this = 
    inherit ServiceBase()
    do  
        this.ServiceName <- "HadoopProxy" 
        this.EventLog.Log <- "Application"

    override this.OnStart(args:string[]) = 
        ServerSideOfProxy.start(args)
        base.OnStart(args)

    override this.OnStop() = 
        ServerSideOfProxy.stop()
        base.OnStop()

    member this.InteractiveStart(args:string[]) =
        this.OnStart(args)

    member this.InteractiveStop() =
        this.OnStop()

[<RunInstaller(true)>] 
type HDFSProxyInstaller() as this = 
    inherit Installer() 
    do 
        let spi = new ServiceProcessInstaller() 
        let si = new ServiceInstaller() 
        spi.Account <- ServiceAccount.NetworkService 

        si.DisplayName <- "HadoopProxy" 
        si.StartType <- ServiceStartMode.Automatic 
        si.ServiceName <- "HadoopProxy"
        
        this.Installers.Add(spi) |> ignore 
        this.Installers.Add(si) |> ignore


module Entry = 
    type ServerWrap =
        {
            args : string
            data : byte[]
        }

    [<EntryPoint>] 
    let Main(args) = 
        let host = new HDFSWindowsService()
        match args with
        | [|"inprocess";pipeIn;pipeOut;|] -> 
          try 
            use pipeClientIn = new AnonymousPipeClientStream(PipeDirection.In, pipeIn)
            use pipeClientOut = new AnonymousPipeClientStream(PipeDirection.Out, pipeOut)
            while true do
                let msg : ServerWrap = SerDes.readTypedMsg pipeClientIn |> Async.RunSynchronously
                match msg.args.Split([|' '|]) with
#if NO_HIVE
#else
                | [|"hive";server;port;auth;timeoutArg;uid;pwd|] -> 
                    let timeout = int32 timeoutArg
                    let m : HiveSchema.HiveRecv = SerDes.fromBytes msg.data                
                    let conn = sprintf "Driver=HIVE;Host=%s;Port=%d" server (int port) 
                    let conn = if uid = "None" then conn else sprintf "%s;UID=%s" conn uid
                    let conn = if pwd = "None" then conn else sprintf "%s;PWD=%s" conn pwd
                    let conn = if auth = "None" then conn else sprintf "%s;AUTHENTICATION=%s" conn auth
                    SerDes.sendTypedMsg pipeClientOut (Hive.hiveHandler conn timeout m) |> Async.RunSynchronously
#endif
                | [|"HDFS";server;port|] -> 
                    let m : HdfsSchema.HdfsRecv = SerDes.fromBytes msg.data
                    SerDes.sendTypedMsg pipeClientOut (HDFS.hdfsHandler server (int port) None m) |> Async.RunSynchronously
                | [|"HDFS";server;port;usr|] -> 
                    let m : HdfsSchema.HdfsRecv = SerDes.fromBytes msg.data
                    SerDes.sendTypedMsg pipeClientOut (HDFS.hdfsHandler server (int port) (Some(usr)) m) |> Async.RunSynchronously
                | _ -> 
                    failwith (sprintf "unknown request arguments: '%s" msg.args)
          // REVIEW: ideally we would detect the closure of the input pipe
          with :? System.IO.EndOfStreamException          -> ()
          0
        | _ ->
            if Environment.UserInteractive then 
                host.InteractiveStart(args)
                System.Console.ReadLine() |> ignore
                host.InteractiveStop()
            else 
                ServiceBase.Run(host)
            0

#endif
