namespace WorkerRole

    open System
    open System.IO
    open System.Net.Sockets
    open System.Threading
    open System.Net
    open System.Text

    /// Based on example from http://msdn2.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.aspx
    /// Implements the connection logic for the socket server.  
    /// After accepting a connection, all data read from the client is sent back. 
    /// The read and echo back to the client pattern is continued until the client disconnects.
    [<Sealed>]
    type internal SocketListener =

        /// The socket used to listen for incoming connection requests.
        [<DefaultValue>]
        val mutable listenSocket : Socket

        /// Buffer size to use for each socket I/O operation.
        val private bufferSize : Int32

        /// The total number of clients connected to the server.
        val private numConnectedSockets : Int32

        /// the maximum number of connections the sample is designed to handle simultaneously.
        val private numConnections : Int32

        /// Pool of reusable SocketAsyncEventArgs objects for write, read and accept socket operations.
        val private readWritePool : SocketAsyncEventArgsPool

        /// Controls the total number of clients connected to the server.
        val private  semaphoreAcceptedClients : Semaphore

        /// <summary>
        /// Create an uninitialized server instance.  
        /// To start the server listening for connection requests
        /// call the Init method followed by Start method.
        /// </summary>
        /// <param name="numConnections">Maximum number of connections to be handled simultaneously.</param>
        /// <param name="bufferSize">Buffer size to use for each socket I/O operation.</param>
        new (numConnections : Int32, bufferSize:Int32) as this =
            {
                numConnectedSockets = 0;
                numConnections = numConnections;
                bufferSize = bufferSize;

                readWritePool = new SocketAsyncEventArgsPool(numConnections);
                semaphoreAcceptedClients = new Semaphore(numConnections, numConnections);
            } then
                // Preallocate pool of SocketAsyncEventArgs objects.
                for i = 0 to this.numConnections - 1 do
                    let readWriteEventArg = new SocketAsyncEventArgs()
                    readWriteEventArg.Completed.AddHandler (new EventHandler<SocketAsyncEventArgs>(this.OnIOCompleted))
                    let buffer = Array.zeroCreate bufferSize
                    readWriteEventArg.SetBuffer(buffer, 0, this.bufferSize)

                    // Add SocketAsyncEventArg to the pool.
                    this.readWritePool.Push(readWriteEventArg);
        /// Mutex to synchronize server execution.
        static member mutex = new Mutex()
           
        /// Close the socket associated with the client.
        /// SocketAsyncEventArg associated with the completed send/receive operation.
        member private this.CloseClientSocket(e:SocketAsyncEventArgs) =       
            let token = e.UserToken :?> Token
            this.CloseClientSocket(token, e)

        member private this.CloseClientSocket(token:Token, e:SocketAsyncEventArgs) =
            token.Dispose()

            // Decrement the counter keeping track of the total number of clients connected to the server.
            let _ = this.semaphoreAcceptedClients.Release()
            let _ = Interlocked.Decrement(ref this.numConnectedSockets)
            //Console.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", this.numConnectedSockets);

            // Free the SocketAsyncEventArg so they can be reused by another client.
            this.readWritePool.Push(e);  

        /// Callback method associated with Socket.AcceptAsync 
        /// operations and is invoked when an accept operation is complete.
        member private this.OnAcceptCompleted (sender: obj) (e: SocketAsyncEventArgs ) =     
            this.ProcessAccept(e)
        
        /// Callback called whenever a receive or send operation is completed on a socket.
        member private this.OnIOCompleted (sender:obj) (e: SocketAsyncEventArgs) =
            // Determine which type of operation just completed and call the associated handler.
            match e.LastOperation with
            | SocketAsyncOperation.Receive -> this.ProcessReceive(e)
            | SocketAsyncOperation.Send -> this.ProcessSend(e)
            | _ -> failwith "The last operation completed on the socket was not a receive or send"
            
        /// Process the accept for the socket listener.
        member private this.ProcessAccept(e:SocketAsyncEventArgs) =
            let s = e.AcceptSocket
            if s.Connected then
                try
                    let readEventArgs = this.readWritePool.Pop();
                    match readEventArgs with
                    | Some(readEventArgs) ->
                        // Get the socket for the accepted client connection and put it into the 
                        // ReadEventArg object user token.
                        readEventArgs.UserToken <- new Token(s, this.bufferSize)

                        Interlocked.Increment(ref this.numConnectedSockets) |> ignore
                        //Console.WriteLine("Client connection accepted. There are {0} clients connected to the server", this.numConnectedSockets);

                        if s.ReceiveAsync(readEventArgs) = false then            
                            this.ProcessReceive(readEventArgs)
                    | None -> ()

                with
                | :? SocketException as ex ->
                    let token = e.UserToken :?> Token
                    failwith (String.Format("Error when processing data received from {0}:\r\n{1}", token.Connection.RemoteEndPoint, ex.ToString()))
                | _ ->  failwith "Error happened, please see the inner exception."

                // Accept the next connection request.
                this.StartAccept(e)


        member private this.ProcessError(e:SocketAsyncEventArgs) =     
            let token = e.UserToken :?> Token;
            let localEp = token.Connection.LocalEndPoint :?> IPEndPoint

            this.CloseClientSocket(token, e);

            //Console.WriteLine("Socket error {0} on endpoint {1} during {2}.", (Int32)e.SocketError, localEp, e.LastOperation);
        
        /// This method is invoked when an asynchronous receive operation completes. 
        /// If the remote host closed the connection, then the socket is closed.  
        /// If data was received then the data is echoed back to the client.
        member private this.ProcessReceive(e:SocketAsyncEventArgs) =    
            // Check if the remote host closed the connection.
            if e.BytesTransferred > 0 then
                if e.SocketError = SocketError.Success then

                    let token = e.UserToken :?> Token
                    token.SetData(e)

                    let s = token.Connection
                    if s.Available = 0 then
                        // Set return buffer.
                        token.ProcessData(e)
                        if s.SendAsync(e) = false then
                            // Set the buffer to send back to the client.
                            this.ProcessSend(e)

                    else if s.ReceiveAsync(e) = false then                 
                        // Read the next block of data sent by client.
                        this.ProcessReceive(e);
                else
                    this.ProcessError(e);                
            else
                this.CloseClientSocket(e);

        /// This method is invoked when an asynchronous send operation completes.  
        /// The method issues another receive on the socket to read any additional 
        /// data sent from the client.
        member private this.ProcessSend(e:SocketAsyncEventArgs) =
            if e.SocketError = SocketError.Success then
                // Done echoing data back to the client.
                let token = e.UserToken :?> Token

                if token.Connection.ReceiveAsync(e) = false then
                    // Read the next block of data send from the client.
                    this.ProcessReceive(e);
                       
            else       
                this.ProcessError(e)

        /// Starts the server listening for incoming connection requests.
        member internal this.Start(port : Int32) =
            // Get host related information.
            let addressList = Dns.GetHostEntry(Environment.MachineName).AddressList

            // Get endpoint for the listener. 
            let localEndPoint = new IPEndPoint(addressList.[addressList.Length - 1], port);

            // Create the socket which listens for incoming connections.
            this.listenSocket <- new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.listenSocket.ReceiveBufferSize <- this.bufferSize
            this.listenSocket.SendBufferSize <- this.bufferSize

            if localEndPoint.AddressFamily = AddressFamily.InterNetworkV6 then 
                // Set dual-mode (IPv4 & IPv6) for the socket listener.
                // 27 is equivalent to IPV6_V6ONLY socket option in the winsock snippet below
                this.listenSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false)
                this.listenSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, localEndPoint.Port))           
            else
                // Associate the socket with the local endpoint.
                this.listenSocket.Bind(localEndPoint)

            // Start the server.
            this.listenSocket.Listen(this.numConnections);

            // Post accepts on the listening socket.
            this.StartAccept(null);

            // Blocks the current thread to receive incoming messages.
            SocketListener.mutex.WaitOne() |> ignore
            ()

        /// Begins an operation to accept a connection request from the client.
        member private this.StartAccept(acceptEventArg : SocketAsyncEventArgs) =
            let mutable acceptEventArgc = acceptEventArg
            if acceptEventArgc = null then
                acceptEventArgc <- new SocketAsyncEventArgs()
                acceptEventArgc.Completed.AddHandler(new EventHandler<SocketAsyncEventArgs>(this.OnAcceptCompleted))
            else
                // Socket must be cleared since the context object is being reused.
                acceptEventArgc.AcceptSocket <- null

            this.semaphoreAcceptedClients.WaitOne() |> ignore
            if this.listenSocket.AcceptAsync(acceptEventArgc) = false then
                this.ProcessAccept(acceptEventArgc)

//
//        /// <summary>
//        /// Stop the server.
//        /// </summary>
//        internal void Stop()
//        {
//            this.listenSocket.Close();
//            mutex.ReleaseMutex();
//        }
//    }

