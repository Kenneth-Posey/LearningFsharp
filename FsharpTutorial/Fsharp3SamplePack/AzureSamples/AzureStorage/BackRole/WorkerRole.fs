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

type WorkerRole() =
    inherit RoleEntryPoint() 

    override wr.Run() =
          
        while(true) do 
            Thread.Sleep(30000)
            //WaWorkerHost.exe will output this message every half minute.
            Trace.TraceError("Should contain 2 messages every minute")
            
    override wr.OnStart() = 
        
        let config = Microsoft.WindowsAzure.Diagnostics.DiagnosticMonitor.GetDefaultInitialConfiguration()

//==========Collect windwos event logs=============//        
        // collect windows event logs
        config.WindowsEventLog.DataSources.Add("System!*")

        //Filter the logs, just transfer logs above information. 
        //The hierarchy of main logs are : critical > error > warning > information > verbose
        config.WindowsEventLog.ScheduledTransferLogLevelFilter <- LogLevel.Information

        //Transfer windows Event Log to Azure Storage every 1 minute , there is also a on-demand way to transfer log to storage , see the ondemandTransfer method in library OndemandCollectLgs
        config.WindowsEventLog.ScheduledTransferPeriod <- TimeSpan.FromMinutes(1.0)

//==========Collect Crash Dumps=============// 
        //Enable Diagnostics monitor to collect crash dump
        Diagnostics.CrashDumps.EnableCollection(true)

//==========Collect performance counter logs============// 
        //Collect performance counter logs
        let perfCounterConfig = new PerformanceCounterConfiguration()
        perfCounterConfig.CounterSpecifier <- @"\Processor(_Total)\% Processor Time"
        perfCounterConfig.SampleRate <- TimeSpan.FromSeconds(5.0)
        config.PerformanceCounters.DataSources.Add(perfCounterConfig)

//==========Collect Diagnostic infrastructure logs============// 
        // Since Diagnostic infrastructure logs are collect by default, can transfer these log to Azure storage by schedule, and just transfer the logs above error.
        //There are 2 other kinds of logs are collected by default:  Windows Azure logs, IIS logs,
        config.DiagnosticInfrastructureLogs.ScheduledTransferLogLevelFilter <- LogLevel.Error
        config.DiagnosticInfrastructureLogs.ScheduledTransferPeriod <- TimeSpan.FromMinutes(5.0)


        ServicePointManager.DefaultConnectionLimit <- 12

//===========Start the confiuration==============//        
        DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString",config) |> ignore

        base.OnStart()
