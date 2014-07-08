
namespace Samples.Hadoop.Internals

open System
open System.Reflection
open Samples.FSharp.ProvidedTypes
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open System.Net
open System.Net.Sockets
open System.Diagnostics
open System.IO

#if BROWSER
module AsyncUtilities = 
    let SwitchToDispatcher() : Async<unit> = 
        if System.Windows.Deployment.Current.Dispatcher.CheckAccess() then async.Return()
        else Async.FromContinuations(fun (scont,_,_) -> do System.Windows.Deployment.Current.Dispatcher.BeginInvoke(System.Action< >(fun () -> scont())) |> ignore)

    let RunOnMainThread(f)= 
        async { do! SwitchToDispatcher()
                return f() }
         |> Async.RunSynchronously
#endif


[<AutoOpen>]
module Helpers = 
#if BROWSER
    
    let makeFetcher<'a,'b> (proxyUrl:string) = fun (send:'a) ->
             async {
                 try 
                        let req = System.Net.WebRequest.CreateHttp(Uri(proxyUrl,UriKind.Absolute), Method="POST")
                        let outArray = SerDes.toBytes send
                        req.ContentType <- "application/x-www-form-urlencoded"
                        req.ContentLength <- int64 outArray.Length
                        let! output = Async.FromBeginEnd(req.BeginGetRequestStream, req.EndGetRequestStream)
                        do! output.AsyncWrite(outArray,0,outArray.Length)
                        output.Flush()
                        output.Close()
                        let! resp = req.AsyncGetResponse()
                        let length = int32 resp.ContentLength
                        let input = resp.GetResponseStream()
                        let! inArray = input.AsyncRead(length)
                        input.Close()
                        return SerDes.fromBytes<'b> inArray
                 with :? System.Net.WebException as e -> return! failwith (sprintf "communicating with the proxy '%s' failed: %s" proxyUrl e.Message)
                        } |>Async.RunSynchronously
#else
   open System.IO.Pipes
   type ServerWrap =
        {
            args : string
            data : byte[]
        }
  
   let startProxyProcessAgent() = 
        let process' = new Process()
        let startInfo = process'.StartInfo
        startInfo.UseShellExecute <- false
        startInfo.CreateNoWindow <- true
        startInfo.FileName <-  System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) +  "/HadoopProxy.exe"
        let outStream = new AnonymousPipeServerStream(PipeDirection.Out,HandleInheritability.Inheritable)
        let inStream = new AnonymousPipeServerStream(PipeDirection.In,HandleInheritability.Inheritable)
        startInfo.Arguments <- [| "inprocess"; outStream.GetClientHandleAsString(); inStream.GetClientHandleAsString(); |]  |> Array.reduce (fun x y -> x + " " + y)
        startInfo.RedirectStandardError <- true
        startInfo.RedirectStandardInput <- true
        startInfo.RedirectStandardOutput <- true
        if not (process'.Start()) then failwith "failed to start process"

        let agent = MailboxProcessor.Start(fun (inbox:MailboxProcessor<string*byte[]*AsyncReplyChannel<byte[]>>) ->
            async {
                while true do
                    let! (args,msg,replyChanel) = inbox.Receive()
                    do! SerDes.sendMsg outStream (SerDes.toBytes({args = args; data = msg}))
                    outStream.Flush()
                    let! reply = SerDes.readMsg inStream
                    replyChanel.Reply(reply)
            })
        fun (args,msg) -> agent.PostAndReply(fun x -> args,msg,x)

   let theProxyProcessAgent = startProxyProcessAgent()

   let makeFetcher<'a,'b> (args:string) = fun (x:'a) -> SerDes.fromBytes (theProxyProcessAgent (args,SerDes.toBytes x)) : 'b
         
#endif