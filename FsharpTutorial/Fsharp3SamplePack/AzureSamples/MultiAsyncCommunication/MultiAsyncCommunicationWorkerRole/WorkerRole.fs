namespace WorkerRole

open System
open System.Collections.Generic
open System.Diagnostics
open System.Linq
open System.Net
open System.Threading
open Microsoft.WindowsAzure
open Microsoft.WindowsAzure.Diagnostics
open Microsoft.WindowsAzure.ServiceRuntime
open Microsoft.WindowsAzure.StorageClient

type WorkerRole() =
    inherit RoleEntryPoint() 

    // This is a sample worker implementation. Replace with your logic.

    let log message kind = Trace.WriteLine(message, kind)

    override wr.Run() =

        let sl = new SocketListener(6, 512)
        sl.Start(13000)

    override wr.OnStart() = 

        // Set the maximum number of concurrent connections 
        ServicePointManager.DefaultConnectionLimit <- 6
       
        // For information on handling configuration changes
        // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

        base.OnStart()
