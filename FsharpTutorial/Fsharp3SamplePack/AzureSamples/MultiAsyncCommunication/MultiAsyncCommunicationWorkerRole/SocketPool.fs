namespace WorkerRole

open System
open System.Text
open System.Net.Sockets
open System.Collections.Generic

    /// Represents a collection of reusable SocketAsyncEventArgs objects. 
    [<Sealed>]
    type internal SocketAsyncEventArgsPool =

        /// Pool of SocketAsyncEventArgs.
        val pool : Stack<SocketAsyncEventArgs> 

        /// <summary>
        /// Initializes the object pool to the specified size.
        /// </summary>
        /// <param name="capacity">Maximum number of SocketAsyncEventArgs objects the pool can hold.</param>
        new (capacity:Int32) =
            { pool = new Stack<SocketAsyncEventArgs>(capacity) }

        /// Removes a SocketAsyncEventArgs instance from the pool.
        member internal this.Pop() : SocketAsyncEventArgs Option  = 

            lock (this.pool) (fun () -> if this.pool.Count > 0 then Some(this.pool.Pop()) else None)         

        /// Add a SocketAsyncEventArg instance to the pool. 
        member internal this.Push(item: SocketAsyncEventArgs) =
        
            if item = null then        
                failwith "Items added to a SocketAsyncEventArgsPool cannot be null"
            
            lock (this.pool) (fun () -> this.pool.Push(item))
        