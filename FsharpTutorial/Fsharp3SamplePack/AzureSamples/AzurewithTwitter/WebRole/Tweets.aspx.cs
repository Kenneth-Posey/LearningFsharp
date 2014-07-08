using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TwitterContracts;

namespace WebRole
{
    public partial class MyTweets : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
                       
        }

        protected void Add_Tweet(object sender, EventArgs e)
        {

            TwitterStatus response = Globals.GetAProxy().TweetUpdate(TextBox1.Text);
            if (response != null)
            {
                divshow.Visible = true;
                Image1.ImageUrl = response.ProfileImageLocation;
                HyperLink1.NavigateUrl = "~/user.aspx?screenname=" + response.Name;
                HyperLink1.Text = response.Name;
                Literal1.Text = response.CreatedDate.ToString();
                Literal2.Text = response.LinkifiedText;
            }
            
        }

    }
}