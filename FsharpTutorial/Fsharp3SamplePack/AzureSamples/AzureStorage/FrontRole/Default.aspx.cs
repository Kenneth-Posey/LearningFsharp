using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OndemandTransferCls;
namespace FrontRole
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string result = OndemandTransferCls.OndemandTransferCls.OndemandTransfer("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", "FrontRole");
            if (result == "Success")
            {
                TransferTB.Text = "Transfer successful";
            }
            else if (result == "False")
            {
                TransferTB.Text = "Transfer is in progress , cancell all transfer tasks!";
            }
            else
            {
                TransferTB.Text = "Error occured";
            }
        }
    }
}
