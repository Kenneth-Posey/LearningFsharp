namespace WorkerRole

open System
open System.Text
open System.Net.Sockets
open System.Globalization

    /// Token for use with SocketAsyncEventArgs.
    [<Sealed>]
    type internal Token =
        interface IDisposable with
            member this.Dispose() = 
                try
                    this.connection.Shutdown(SocketShutdown.Send)
                finally
                    this.connection.Close()

        val private  connection : Socket
        val mutable private  sb : StringBuilder
        val mutable private currentIndex : Int32

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="connection">Socket to accept incoming data.</param>
        /// <param name="bufferSize">Buffer size for accepted data.</param>
        new(connections : Socket, bufferSize : Int32) =
            {
                connection = connections
                sb = new StringBuilder(bufferSize);
                currentIndex = 0
            }         
        /// to Display the implemented Dispose method
        member internal this.Dispose() = (this :> IDisposable).Dispose()

        /// Accept socket.
        member internal this.Connection with get() = this.connection

        /// Process data received from the client.
        member internal this.ProcessData(args:SocketAsyncEventArgs) =
            // Get the message received from the client.
            let received = this.sb.ToString()

            // Use message received to perform a specific operation.
            let receivedProcess = received + " from server"

            let sendBuffer = Encoding.ASCII.GetBytes(receivedProcess)
            args.SetBuffer(sendBuffer, 0, sendBuffer.Length)

            // Clear StringBuffer, so it can receive more data from a keep-alive connection client.
            this.sb.Length <- 0
            this.currentIndex <- 0
        

        /// Set data received from the client.
        member internal this.SetData(args : SocketAsyncEventArgs) =
        
            let count = args.BytesTransferred
            if ((this.currentIndex + count) > this.sb.Capacity) then
                failwith (String.Format(CultureInfo.CurrentCulture,
                                        "Adding {0} bytes on buffer which has {1} bytes, the listener buffer will overflow.",
                                        count, this.currentIndex))

            this.sb.Append(Encoding.ASCII.GetString(args.Buffer, args.Offset, count))
            this.currentIndex <- this.currentIndex + count


