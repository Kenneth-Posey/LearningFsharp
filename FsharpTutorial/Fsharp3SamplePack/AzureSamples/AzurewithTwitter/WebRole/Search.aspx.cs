using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebRole
{
    public partial class Search : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack && !string.IsNullOrWhiteSpace(Request.QueryString["text"]))
            {
                this.SearchTextBox.Text = Request.QueryString["text"];
                PerformSearch();
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            var ResponseObject = Globals.GetAProxy().TwitterSearch(this.SearchTextBox.Text);
            this.SearchResultsListView.DataSource = ResponseObject;
            this.SearchResultsListView.DataBind();
        }
    }
}