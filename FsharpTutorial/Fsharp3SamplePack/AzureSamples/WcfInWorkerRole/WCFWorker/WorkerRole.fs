namespace WCFWorker

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
open LoanCalculatorContracts

type WorkerRole() as this =
    inherit RoleEntryPoint() 

    [<DefaultValue>]
    val mutable serviceHost : ServiceHost
    //let serviceHost = new ServiceHost(typeof<LoanCalculatorImplementation>) 

    member private this.CreateServiceHost() =      
        
        this.serviceHost <- new ServiceHost(typeof<LoanCalculatorImplementation>) 
        let binding = new NetTcpBinding(SecurityMode.None)
        let externalEndPoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints.["WCFEndpoint"]
        let endpoint = String.Format("net.tcp://{0}/LoanCalculator", externalEndPoint.IPEndpoint)       
        this.serviceHost.AddServiceEndpoint(typeof<ILoanCadulator>, binding, endpoint) |> ignore
        this.serviceHost.Open()

    override wr.Run() =
        while (true) do

                Thread.Sleep(10000);
                Trace.WriteLine("Working", "Information");
            


    override wr.OnStart() = 

        // Set the maximum number of concurrent connections 
        ServicePointManager.DefaultConnectionLimit <- 12
       
        // For information on handling configuration changes
        // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
        this.CreateServiceHost()
        base.OnStart()
