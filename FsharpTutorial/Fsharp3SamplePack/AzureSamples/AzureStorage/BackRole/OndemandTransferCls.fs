namespace BackRole

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
open Microsoft.WindowsAzure.Diagnostics.Management

type OndemandTransferCls() = 

     static member OndemandTransfer(diagnosticConnectiontStr : string, roleName : string) =        
        let diagManager = new DeploymentDiagnosticManager(RoleEnvironment.GetConfigurationSettingValue(diagnosticConnectiontStr),RoleEnvironment.DeploymentId)
        let roleInstDiagMgr = diagManager.GetRoleInstanceDiagnosticManager(roleName, RoleEnvironment.CurrentRoleInstance.Id)

        // you can specify the type of logs which will be force transfered storage
        //By switch the field of  DataBufferName to get this
        // At this case, we force the basic windows azure logs to transfer to Azure Storage
        let dataBuffersToTransfer = DataBufferName.Logs
        let transferOptions = new OnDemandTransferOptions()

        transferOptions.From <- DateTime.MinValue
        transferOptions.To <- DateTime.UtcNow

        let mutable requestID = Guid.Empty

        try
            requestID <- roleInstDiagMgr.BeginOnDemandTransfer(dataBuffersToTransfer,transferOptions)

        with
            | :? System.InvalidOperationException ->    
                        // if the transfer operation is in progress , cancel the all logs transfers
                        roleInstDiagMgr.CancelOnDemandTransfers(DataBufferName.Logs) |> ignore
