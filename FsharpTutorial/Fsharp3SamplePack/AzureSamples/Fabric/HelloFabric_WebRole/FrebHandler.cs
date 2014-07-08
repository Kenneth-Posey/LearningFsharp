using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Microsoft.Samples.ServiceHosting.HelloFabric
{
    /// <summary>
    /// This handler serves the XML stylesheet referred to by the Failed Request Tracing log files. 
    /// </summary>
    public class FrebHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var logPath =
                Path.Combine(RoleEnvironment.GetLocalResource("DiagnosticStore").RootPath, @"FailedReqLogFiles\W3SVC1");

            var stylesheetPath = Path.Combine(logPath, "freb.xsl");
            if (!File.Exists(stylesheetPath)) throw new HttpException(404, "File Not Found");

            context.Response.WriteFile(stylesheetPath);
            context.Response.End();
        }
    }


}
