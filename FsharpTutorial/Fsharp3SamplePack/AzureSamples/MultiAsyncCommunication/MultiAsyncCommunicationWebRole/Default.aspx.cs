using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Net;
using System.Net.Sockets;
using MultiAsyncCommunication.SocketClientHelper;

namespace DouChannelsCommunication
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            using (SocketClient sa = new SocketClient("127.0.0.1", 13000))
            {
                sa.Connect();

                for (Int32 i = 0; i < 5; i++)
                {
                    TextBox1.Text += sa.SendReceive(" Message #" + i.ToString());
                }
                sa.Disconnect();
            }
        }
    }
}