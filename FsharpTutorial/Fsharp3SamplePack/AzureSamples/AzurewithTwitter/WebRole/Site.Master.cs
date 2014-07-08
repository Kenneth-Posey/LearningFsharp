using System;
using System.Web.UI.WebControls;

namespace WebRole
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        public string ErrorMessage { get; set; }

        protected bool HasErrorMessage
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.ErrorMessage);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
           
            this.DataBind();
        }
    }
}
