namespace MultiAsyncCommunication.SocketClientHelper

open System;
open System.IO;
open System.Text;
open System.Net;
open System.Net.Sockets;
open Microsoft.WindowsAzure.ServiceRuntime;
open System.Diagnostics;
open System.Collections.Generic;
open System.Threading;

    /// Implements the connection logic for the socket client.
    [<Sealed>]
    type public SocketClient =
        interface IDisposable with
            /// Disposes the instance of SocketClient.
            member this.Dispose() =     
                SocketClient.autoConnectEvent.Close()
                SocketClient.autoSendReceiveEvents.[this.SendOperation].Close()
                SocketClient.autoSendReceiveEvents.[this.ReceiveOperation].Close()
                if this.clientSocket.Connected then
                    this.clientSocket.Close()

        /// The socket used to send/receive messages.
        [<DefaultValue>]
        val mutable private clientSocket : Socket

        /// Flag for connected socket.
        val mutable private connected : Boolean

        /// Listener endpoint.
        val private hostEndPoint : IPEndPoint

        /// Create an uninitialized client instance.  
        /// To start the send/receive processing
        /// call the Connect method followed by SendReceive method.
        public new(hostName: string, port:Int32) as this= 
            // Get host related information.
            let host = Dns.GetHostEntry(hostName)

            // Addres of the host.
            let addressList = host.AddressList
            {

                connected = false
                // Instantiates the endpoint and socket.
                hostEndPoint = new IPEndPoint(addressList.[addressList.Length - 1], port)
                
            } then
                this.clientSocket <- new Socket(this.hostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)

        /// Show Dispose on current type
        member public this.Dispose() = (this :> IDisposable).Dispose()

        /// Signals a connection.
        static member private autoConnectEvent = new AutoResetEvent(false)

        /// Signals the send/receive operation.
        static member private autoSendReceiveEvents = [|new AutoResetEvent(false)
                                                        new AutoResetEvent(false)|]
        /// Constants for socket operations.
        member public this.ReceiveOperation = 1
        member public this.SendOperation = 0


        /// Connect to the host.
        member public this.Connect() =
        
            let connectArgs = new SocketAsyncEventArgs()

            connectArgs.UserToken <- this.clientSocket
            connectArgs.RemoteEndPoint <- this.hostEndPoint
            connectArgs.Completed.AddHandler (new EventHandler<SocketAsyncEventArgs>(this.OnConnect))

            this.clientSocket.ConnectAsync(connectArgs) |> ignore
            SocketClient.autoConnectEvent.WaitOne() |> ignore

            let errorCode = connectArgs.SocketError
            if errorCode <> SocketError.Success then
                   raise <| new SocketException((int)errorCode)
            
        /// Disconnect from the host.
        member public this.Disconnect() = 
            this.clientSocket.Disconnect(false);
        

        member private this.OnConnect(sender : obj) (e : SocketAsyncEventArgs) =
            // Signals the end of connection.
            SocketClient.autoConnectEvent.Set() |> ignore

            // Set the flag for socket connected.
            this.connected = (e.SocketError = SocketError.Success) |> ignore
            ()

        member private this.OnReceive(sender:obj) (e:SocketAsyncEventArgs) = 
            // Signals the end of receive.
            SocketClient.autoSendReceiveEvents.[this.SendOperation].Set() |> ignore           

        member private this.OnSend(sender : obj) (e:SocketAsyncEventArgs) =
            // Signals the end of send.
            SocketClient.autoSendReceiveEvents.[this.ReceiveOperation].Set() |> ignore

            if e.SocketError = SocketError.Success then           
                if e.LastOperation = SocketAsyncOperation.Send then 
                    // Prepare receiving.
                    let s = e.UserToken :?> Socket

                    let receiveBuffer = Array.zeroCreate 255
                    e.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
                    e.Completed.AddHandler(new EventHandler<SocketAsyncEventArgs>(this.OnReceive))
                    s.ReceiveAsync(e)   
                    ()                    
            else           
                this.ProcessError(e)
           
            
        /// Close socket in case of failure and throws a SockeException according to the SocketError.
        member private this.ProcessError(e:SocketAsyncEventArgs) =
            let s = e.UserToken :?> Socket
            if s.Connected then
            
                // close the socket associated with the client
                try              
                    s.Shutdown(SocketShutdown.Both);              
                finally
                    if s.Connected then
                       s.Close();    

            // Throw the SocketException
            raise <| new SocketException((int)e.SocketError)
        
        /// Exchange a message with the host.
        member public this.SendReceive(message : String) =
        
            if this.connected then
            
                // Create a buffer to send.
                let sendBuffer = Encoding.ASCII.GetBytes(message)

                // Prepare arguments for send/receive operation.
                let completeArgs = new SocketAsyncEventArgs()
                completeArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length)
                completeArgs.UserToken <- this.clientSocket
                completeArgs.RemoteEndPoint <- this.hostEndPoint
                completeArgs.Completed.AddHandler( new EventHandler<SocketAsyncEventArgs>(this.OnSend))

                // Start sending asyncronally.
                this.clientSocket.SendAsync(completeArgs) |> ignore

                let wh = [|SocketClient.autoSendReceiveEvents.[0] :> WaitHandle
                           SocketClient.autoSendReceiveEvents.[1] :> WaitHandle|]
                // Wait for the send/receive completed.
                AutoResetEvent.WaitAll(wh) |> ignore

                // Return data from SocketAsyncEventArgs buffer.
                Encoding.ASCII.GetString(completeArgs.Buffer, completeArgs.Offset, completeArgs.BytesTransferred)
            
            else
            
                raise <| new SocketException((int)SocketError.NotConnected)
            