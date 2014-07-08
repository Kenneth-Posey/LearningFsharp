namespace Microsoft.Samples.ServiceHosting.HelloFabric

open System
open System.Diagnostics
open System.Threading
open Microsoft.Samples.ServiceHosting.HelloFabric
open Microsoft.WindowsAzure.Diagnostics
open Microsoft.WindowsAzure.ServiceRuntime
type WorkerRole() =
    inherit RoleEntryPoint() 
 
    override wr.OnStart() = 
        DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString") |> ignore
        base.OnStart()

    override wr.Run() =
        let dayTimerServer = new DayTimeTcpServer(RoleEnvironment.CurrentRoleInstance.InstanceEndpoints.["DayTime"].IPEndpoint)
        dayTimerServer.Start()
        
        let mutable count = 0

        while(true) do 
            count <- count + 1
            Trace.WriteLine(String.Format("Message %d",count))
            Thread.Sleep(TimeSpan.FromSeconds(10.0))

