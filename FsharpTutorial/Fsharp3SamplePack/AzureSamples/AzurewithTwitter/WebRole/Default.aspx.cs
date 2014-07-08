namespace WebRole
{
    using System;
    using TwitterContracts;
    using System.ServiceModel;
    using System.Configuration;
    using System.Collections.Generic;

    public partial class _Default : System.Web.UI.Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Authorized users see this.
            List<TwitterStatus> homeTimelineResponse = Globals.GetAProxy().GetMyHomeTimeLine();

            if (homeTimelineResponse != null)
            {
                this.TimelineList.DataSource = homeTimelineResponse;
                this.TimelineList.DataBind();
            }
            else
            {
                this.Master.ErrorMessage = "Could not retrieve the home timeline. 404 Not Authorized!";
            }

            this.headerText.Text = "Your home timeline";

        }
    }

    public static class Globals
    {
        private static string serviceUrl = ConfigurationManager.AppSettings["serviceurl"];

        public static ITwitterStatusContracts GetAProxy()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            EndpointAddress endpointAddress = new EndpointAddress(Globals.serviceUrl);

            return new ChannelFactory<ITwitterStatusContracts>
                (binding, endpointAddress).CreateChannel();
        }
    }
}
