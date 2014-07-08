namespace Microsoft.Samples.ServiceHosting.HelloFabric

open System
open System.Collections.Generic
open System.Text
open System.Net
open System.Net.Sockets
open System.IO
open System.Diagnostics
open Microsoft.WindowsAzure.ServiceRuntime

type public DayTimeTcpServer =
    val private _tcpListener : TcpListener
    new(ep : IPEndPoint) = { _tcpListener = new TcpListener(ep)}

    member private self.HandleClientConnect( tcp : TcpClient ) =
        Trace.TraceInformation("Accepting client from: %A", tcp.Client.RemoteEndPoint) 
        use strm = tcp.GetStream()
        let wr = new StreamWriter(strm)
        wr.WriteLine(DateTime.UtcNow.ToString("r"))
        wr.Close()

    member self.OnAcceptTcpClient( x : IAsyncResult ) =
        let thread = new System.Threading.Thread(fun () -> 
                                                            use tcpClient = self._tcpListener.EndAcceptTcpClient(x)
                                                            self.HandleClientConnect(tcpClient))                                                
        thread.Start()

        try            
            self._tcpListener.BeginAcceptSocket(new AsyncCallback(self.OnAcceptTcpClient), null)  |> ignore
        with
            | :? SocketException -> printfn "An error occurred while attempting to access the socket."
            | :? ObjectDisposedException -> printfn "The Socket has been closed. "
        
    member public self.Start() =        
        try
            self._tcpListener.Start()
            self._tcpListener.BeginAcceptSocket(new AsyncCallback(self.OnAcceptTcpClient), null)  |> ignore
        with
            | :? SocketException -> printfn "An error occurred while attempting to access the socket."
            | :? ObjectDisposedException -> printfn "The Socket has been closed. "
        
    member public self.Stop() =
        Trace.TraceInformation("Shutting off tcp service on %A\n",self._tcpListener.Server.LocalEndPoint);
        self._tcpListener.Stop()

