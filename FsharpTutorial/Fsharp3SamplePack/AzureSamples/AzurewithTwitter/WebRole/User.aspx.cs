using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TwitterContracts;

namespace WebRole
{
    public partial class User : System.Web.UI.Page
    {
        public TwitterStatus TwitterUser { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
                return;

            string screenname = Request.QueryString["screenname"];
            if (string.IsNullOrWhiteSpace(screenname))
                Response.Redirect("~/Default.aspx", true);

            List<TwitterStatus> responseObject = Globals.GetAProxy().GetUserHomeTimeLine(screenname);

            if (responseObject == null)
            {
                Response.Redirect("~/Default.aspx?error=Could not retrieve the home timeline");
            }

            this.TwitterUser = responseObject.First();
            this.TimelineList.DataSource = responseObject;

            this.DataBind();
        }
    }

  
}