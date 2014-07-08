namespace OndemandTransferCls

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
            if OndemandTransferCls.IsTransferInProgress(diagnosticConnectiontStr) then                
                "False"
            else                
                requestID <- roleInstDiagMgr.BeginOnDemandTransfer(dataBuffersToTransfer,transferOptions)
                
                "Success"
        with
            | :? System.InvalidOperationException ->
                        let out = roleInstDiagMgr.CancelOnDemandTransfers(DataBufferName.Logs) 
                        "Error"     
     
     static member IsTransferInProgress(diagnosticConnectiontStr : string) =
      
        let manager = new DeploymentDiagnosticManager(RoleEnvironment.GetConfigurationSettingValue(diagnosticConnectiontStr),RoleEnvironment.DeploymentId)
        let account = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(diagnosticConnectiontStr))
        let queueClient = account.CreateCloudQueueClient()

        let result = ref false
        let activeTransfers = new Dictionary<string,List<OnDemandTransferInfo>>()

        manager.GetRoleNames()  |> Seq.iter( fun role ->
            manager.GetRoleInstanceDiagnosticManagersForRole(role) |> Seq.iter( fun instanceManager ->
                let transfers = instanceManager.GetActiveTransfers()
                transfers |> Seq.iter( fun transfer ->

                        try 
                            transfer.Value.NotificationQueueName |> ignore
                            if activeTransfers.ContainsKey(transfer.Value.NotificationQueueName) then
                                activeTransfers.[transfer.Value.NotificationQueueName].Add(transfer.Value)
                            else
                                try 
                                    activeTransfers.Add(transfer.Value.NotificationQueueName, new List<OnDemandTransferInfo>())
                                with
                                    | :? ArgumentNullException ->  printfn "Can't add new transfer to dictionary" 
                        with
                            | :? ArgumentNullException -> printfn "there is no active transfer" 
                                
                            )))
                                    
        activeTransfers 
            |> Seq.iter(fun queueTransfersPair ->
                                let queue = queueClient.GetQueueReference(queueTransfersPair.Key)
                                if (queue.Exists()) then
                                    queue.GetMessages(queueTransfersPair.Value.Count) 
                                        |> Seq.iter( fun msg ->
                                            let info = OnDemandTransferInfo.FromQueueMessage(msg)
                                            let instanceManager = manager.GetRoleInstanceDiagnosticManager(info.RoleName, info.RoleInstanceId)
                    
                                            let res = instanceManager.EndOnDemandTransfer(info.RequestId)
                                            let traceStr = "data transfer complete for role instance " + info.RoleInstanceId
                                            System.Diagnostics.Trace.WriteLine(traceStr)
                    
                                            try 
                                                let pairInfo = queueTransfersPair.Value |> Seq.find(fun value -> value.RequestId = info.RequestId )
                                                queueTransfersPair.Value.Remove(pairInfo) |> ignore

                                            with
                                                 | :? KeyNotFoundException -> printfn "key was not found"

                                            queue.DeleteMessage(msg) )

                                if (queueTransfersPair.Value.Count <> 0) then
                                    result := true
                                else if queue.Exists() then
                                    queue.Delete()
                                )
        !result                                                        