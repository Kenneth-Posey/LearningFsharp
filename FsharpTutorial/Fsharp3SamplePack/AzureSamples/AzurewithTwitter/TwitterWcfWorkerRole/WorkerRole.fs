namespace TwitterWcfWorkerRole

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
open System.ServiceModel
open System.Runtime.Serialization
open TwitterContracts

type WorkerRole() =
    inherit RoleEntryPoint() 

    // This is a sample worker implementation. Replace with your logic.
    let log message kind = Trace.WriteLine(message, kind)

    [<DefaultValue>]
    val mutable serviceHost : ServiceHost

    member private this.CreateServiceHost() =      
        
        this.serviceHost <- new ServiceHost(typeof<TwitterStatusImplementation>) 
        let binding = new NetTcpBinding(SecurityMode.None)
        let externalEndPoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints.["WCFEndpoint"]
        let endpoint = String.Format("net.tcp://{0}/TwitterContracts", externalEndPoint.IPEndpoint)       
        this.serviceHost.AddServiceEndpoint(typeof<ITwitterStatusContracts>, binding, endpoint) |> ignore
        this.serviceHost.Open()

    override wr.Run() =

        log "TwitterWcfWorkerRole entry point called" "Information"
        while(true) do 
            Thread.Sleep(10000)
            log "Working" "Information"

    override wr.OnStart() = 

        // Set the maximum number of concurrent connections 
        ServicePointManager.DefaultConnectionLimit <- 12
       
        // For information on handling configuration changes
        // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
        wr.CreateServiceHost()
        base.OnStart()
