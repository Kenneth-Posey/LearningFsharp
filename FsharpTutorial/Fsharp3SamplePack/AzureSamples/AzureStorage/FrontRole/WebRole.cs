using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Threading;
namespace FrontRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            //get the configuration 
            var config = DiagnosticMonitor.GetDefaultInitialConfiguration();
            
            //Enable to collect crush dump
            Microsoft.WindowsAzure.Diagnostics.CrashDumps.EnableCollection(true);
            // Start Diagnostic monitor using the configuration above, the first parm specify the Azure storage account. If the app runs locally, use UseDevelopmentStorage
            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", config);

            return base.OnStart();
        }

        public override void Run()
        {

            while (true)
            {
                System.Diagnostics.Trace.TraceInformation("Web role is running");
                Thread.Sleep(5000);
            }
        
        }
    }
}
