//
// <copyright file="Default.aspx.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics.Management;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Samples.ServiceHosting.HelloFabric
{
    public partial class _Default : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {           
            // Check if the application is running in the Windows Azure Compute Emulator
            if (RoleEnvironment.IsAvailable)
            {
                if (RoleEnvironment.IsEmulated)
                {
                    Label3.Text = "Running in the Windows Azure Compute Emulator.";
                    Label1.Text = RoleEnvironment.GetConfigurationSettingValue("BannerText");
                }
                else
                {
                    Label3.Text = "Running in Windows Azure.";
                    Label1.Text = WebConfigurationManager.AppSettings["BannerText"];
                }
            }
            loadMessageFromLocalStorage(StoredMessagePanel1,
                                        StoredMessageErrorPanel1,
                                        StoredMessageLabel1,
                                        StoredMessageErrorLabel1,
                                        "localStoreOne");

            loadMessageFromLocalStorage(StoredMessagePanel2,
                                        StoredMessageErrorPanel2,
                                        StoredMessageLabel2,
                                        StoredMessageErrorLabel2,
                                        "localStoreTwo");

            getCurrentTime();

            TransferProgressTimer_Tick(null, EventArgs.Empty);
        }

        private void getCurrentTime()
        {
            IPEndPoint endpoint = null;

            try
            {
                endpoint = RoleEnvironment.Roles["FsHelloFabric_WorkerRole"].Instances[0].InstanceEndpoints["DayTime"].IPEndpoint;
            }
            catch (Exception)
            {
                WorkerTime.Text = "An error occurred retrieving the worker role endpoint. Make sure that at least one instance of the worker role is running.";

                return;
            }

            using (var client = new TcpClient())
            {
                if (client.TryConnect(endpoint, 10, 5000))
                {
                    WorkerTime.Text = client.ReadAllData();
                    client.Close();
                }
                else
                {
                    WorkerTime.Text = "An error occurred connecting to the worker role endpoint. Make sure that the worker role is running and is not busy.";
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            String msg = TextBox1.Text;
            switch (DropDownList1.Text)
            {
                case "Error":
                    System.Diagnostics.Trace.TraceError(msg);
                    break;
                case "Warning":
                    System.Diagnostics.Trace.TraceWarning(msg);
                    break;
                case "Information":
                    System.Diagnostics.Trace.TraceInformation(msg);
                    break;
                default:
                    System.Diagnostics.Trace.TraceError("Unknown log type");
                    return;
            }
        }

        bool CanResolveAddress(string address)
        {
            DNSResolutionErrorPanel.Visible = false;
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(address);
                return true;
            }
            catch (SocketException  e)
            {
                if (e.SocketErrorCode == SocketError.HostNotFound)
                {
                    DNSResolutionErrorPanel.Visible = true;
                    DNSResolutionErrorLabel.Text = 
                        ResolveErrorMessage(address);
                    return false;
                }
                throw;
            }
        }
        string ResolveErrorMessage(string address)
        {
            string msg =
                 String.Format("Unable to resovle {0}. The host name is either incorrect or "+
                               "you are running this sample in the Development Fabric "+
                               "and are behind a firewall. Please check your proxy "+
                               "configuration on the machine runing this sample.",
                 address);
            return HttpUtility.HtmlEncode(msg);
        }
        protected void HttpButton_Click(object sender, EventArgs e)
        {
             HttpResponsePanel.Visible = false;
             Uri url = new Uri(HttpAddress.Text);
             if (CanResolveAddress(url.Host))
             {
                 WebClient webClient = new System.Net.WebClient();
                 HttpResponsePanel.Visible = true;
                 HttpResponseLabel.Text = HttpUtility.HtmlEncode(webClient.DownloadString(HttpAddress.Text));
             }
        }

        private string GetLocalStore(string name)
        {
            if (RoleEnvironment.IsAvailable)
            {
                return RoleEnvironment.GetLocalResource(name).RootPath;
            }
            else
            {
                var storesDir = Path.Combine(Path.Combine(Path.GetTempPath(), "localstores"), Path.GetRandomFileName());

                string dirPath = Path.Combine(storesDir, name);
                Directory.CreateDirectory(dirPath);
                return dirPath;
            }
        }

        private void loadMessageFromLocalStorage(Panel panel, Panel errorPanel, Label label, Label errorLabel, string storeName)
        {
            try
            {
                var path = GetLocalStore(storeName);
                var message = File.ReadAllText(Path.Combine(path, "message.txt"));

                panel.Visible = true;
                errorPanel.Visible = false;

                label.Text = message;
            }
            catch (FileNotFoundException)
            {
                panel.Visible = false;
                errorPanel.Visible = true;

                errorLabel.Text = "No message has been set.";
            }
            catch
            {
                panel.Visible = false;
                errorPanel.Visible = true;

                errorLabel.Text = "There was an error accessing local storage.";
            }
        }

        private void storeMessage(Panel panel, Panel errorPanel, Label label, Label errorLabel, TextBox textBox, string storeName)
        {
            var path = GetLocalStore(storeName);

            using (var write = File.CreateText(Path.Combine(path, "message.txt")))
                write.Write(textBox.Text);

            loadMessageFromLocalStorage(panel, errorPanel, label, errorLabel, storeName);
        }

        protected void StoreMessage1_Click(object sender, EventArgs e)
        {
            storeMessage(StoredMessagePanel1,
                         StoredMessageErrorPanel1,
                         StoredMessageLabel1,
                         StoredMessageErrorLabel1,
                         InputMessageTextBox1,
                         "localStoreOne");
        }

        protected void StoreMessage2_Click(object sender, EventArgs e)
        {
            storeMessage(StoredMessagePanel2,
                         StoredMessageErrorPanel2,
                         StoredMessageLabel2,
                         StoredMessageErrorLabel2,
                         InputMessageTextBox2,
                         "localStoreTwo");
        }
        
        protected void PushAzure_Click(object sender, EventArgs e)
        {
            pushLogs(DataBufferName.Logs, PushAge.Text);
        }

        protected void PushPerf_Click(object sender, EventArgs e)
        {
            pushLogs(DataBufferName.PerformanceCounters, PushPerfAge.Text);
        }

        protected void PushIIS_Click(object sender, EventArgs e)
        {
            pushLogs(DataBufferName.Directories, PushIISAge.Text);
        }

        private OnDemandTransferOptions getTransferParameters(string age, string notificationQueueName)
        {
            return new OnDemandTransferOptions()
            {
                LogLevelFilter = (LogLevel)Enum.Parse(typeof(LogLevel), PushLevel.SelectedValue),
                From = DateTime.Now.Subtract(TimeSpan.FromMinutes(Int32.Parse(age))),
                To = DateTime.Now,
                NotificationQueueName = notificationQueueName
            };
        }

        private void pushLogs(DataBufferName bufferName, string age)
        {
            if (IsTransferInProgress())
            {
                TransferErrorMessage.Text = "Another transfer is in progress. Please wait for this transfer to complete before requesting another.";
                TransferErrorMessage.Visible = true;

                return;
            }

            var queueName = "hellofabric-" + Guid.NewGuid().ToString().ToLowerInvariant();

            var manager = new DeploymentDiagnosticManager(RoleEnvironment.GetConfigurationSettingValue("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString"),
                                                           RoleEnvironment.DeploymentId);
            foreach (var role in manager.GetRoleNames())
            {
                foreach (var instanceManager in manager.GetRoleInstanceDiagnosticManagersForRole(role))
                {
                    var guid = instanceManager.BeginOnDemandTransfer(bufferName, getTransferParameters(age, queueName));
                    System.Diagnostics.Trace.WriteLine(string.Format("data transfer started for role {0}...", role));
                }
            }

            TransferProgressPanel.Visible = true;
            TransferSetupPanel.Visible = false;
        }

        public void TransferProgressTimer_Tick(object sender, EventArgs e)
        {
            // determine whether any jobs are still running
            if (IsTransferInProgress())
            {
                TransferProgressPanel.Visible = true;
                TransferSetupPanel.Visible = false;
            }
            else
            {
                TransferProgressPanel.Visible = false;
                TransferSetupPanel.Visible = true;
                TransferErrorMessage.Visible = false;
            }
        }

        private bool IsTransferInProgress()
        {
            var manager = new DeploymentDiagnosticManager(RoleEnvironment.GetConfigurationSettingValue("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString"),
                RoleEnvironment.DeploymentId);
            var account = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString"));
            var queueClient = account.CreateCloudQueueClient();

            var result = false;
            var activeTransfers = new Dictionary<string, List<OnDemandTransferInfo>>();

            foreach (var role in manager.GetRoleNames())
            {
                foreach (var instanceManager in manager.GetRoleInstanceDiagnosticManagersForRole(role))
                {
                    var transfers = instanceManager.GetActiveTransfers();

                    foreach (var transfer in transfers)
                    {
                        if (!activeTransfers.ContainsKey(transfer.Value.NotificationQueueName))
                            activeTransfers.Add(transfer.Value.NotificationQueueName, new List<OnDemandTransferInfo>());

                        activeTransfers[transfer.Value.NotificationQueueName].Add(transfer.Value);
                    }
                }
            }

            foreach (var queueTransfersPair in activeTransfers)
            {
                var queue = queueClient.GetQueueReference(queueTransfersPair.Key);

                if (queue.Exists())
                {
                    foreach (var msg in queue.GetMessages(queueTransfersPair.Value.Count))
                    {
                        var info = OnDemandTransferInfo.FromQueueMessage(msg);
                        var instanceManager = manager.GetRoleInstanceDiagnosticManager(info.RoleName, info.RoleInstanceId);
                        var res = instanceManager.EndOnDemandTransfer(info.RequestId);
                        System.Diagnostics.Trace.WriteLine(string.Format("data transfer complete for role instance {0}.", info.RoleInstanceId));

                        var pairInfo = queueTransfersPair.Value.Find((value) => value.RequestId == info.RequestId);

                        if (pairInfo != null)
                            queueTransfersPair.Value.Remove(pairInfo);

                        queue.DeleteMessage(msg);
                    }
                }

                if (queueTransfersPair.Value.Count != 0)
                    result = true;
                else if (queue.Exists())
                    queue.Delete();
            }

            return result;
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
