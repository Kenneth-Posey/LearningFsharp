using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
namespace FrontRole
{
    public partial class About : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
                      
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
                        
            System.Threading.Thread.Sleep(20000);// Arbitrary delay - to trigger failure log   '
            System.Diagnostics.Trace.TraceInformation("Generate failed IIS request logs");
            RequestTB.Text = "Ok , finished";
        }
    }
}
