//
// <copyright file="WebRole.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
using System;
using System.Threading;
using Microsoft.Samples.ServiceHosting.HelloFabric;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Diagnostics;
using System.Diagnostics;

namespace Microsoft.Samples.ServiceHosting.HelloFabric
{
    public class MyWebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // This role is starting with a custom configuration for the purpose of the sample.
            var config = DiagnosticMonitor.GetDefaultInitialConfiguration();
            
            // Adding performance counters to the default diagnostic configuration
            config.PerformanceCounters.DataSources.Add(
                new PerformanceCounterConfiguration()
                {
                    CounterSpecifier = @"\Processor(_Total)\% Processor Time",
                    SampleRate = TimeSpan.FromSeconds(5)                    
                });

            // Schedule transfer of performance counter data
            config.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);

            // Schedule transfer of trace log data
            config.Logs.ScheduledTransferLogLevelFilter = LogLevel.Undefined;
            config.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);

            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", config);

            return base.OnStart();
        }
        // Generate a tick in the log every 10 seconds.
        public override void Run()
        {
            int count = 0;

            for ( ; ; )
            {
                count ++;
                Trace.WriteLine(String.Format("Message {0}", count));
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }
    }
}
