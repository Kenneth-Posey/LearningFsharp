<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Microsoft.Samples.ServiceHosting.HelloFabric._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Hello Fabric</title>
</head>

<body style="font-family: Arial, Helvetica, sans-serif;">

    <h1>Hello Fabric</h1>

    <h3>General Information</h3>

    <p>By using the IsEmulated property of the RoleEnvironment class, you can discover whether an application is running in the Windows Azure Compute Emulator. 
    The current status is:</p>

    <form id="form1" runat="server" style="font-size: medium">
    <asp:ScriptManager runat="server" />
    <div>
        <asp:Label ID="Label3" runat="server" Text="Runing as ??"></asp:Label><br />
    </div>    
    
    <hr />

    <h3>Configuration Settings</h3>
    
    <p>There is also an easy way for getting configuration settings that have been set in the .cscfg file. The setting 
    called BannerText is currently configured as the following string:</p>
    
    <div>
        <asp:Label ID="Label1" runat="server" Text="Setting" ToolTip="This setting can be changed by using CSRun if running locally, or in the Windows Azure Platform Management Portal if running in Windows Azure."></asp:Label>
    </div>
    
    <hr />
    
    <h3>Diagnostics</h3>
    
    <p>Application events can be traced and transferred to storage.</p>
    <p>Enter a message in the textbox and then click the button to submit an event to be traced.</p>
    
    <asp:Panel ID="Panel1" runat="server" DefaultButton="Button1">
        <asp:DropDownList ID="DropDownList1" runat="server" Width="20%" ToolTip="Set the level of the message to log.">
            <asp:ListItem>Error</asp:ListItem>
            <asp:ListItem>Warning</asp:ListItem>
            <asp:ListItem>Information</asp:ListItem>
        </asp:DropDownList>
        <asp:TextBox ID="TextBox1" runat="server" Width="70%" ToolTip="Message to Log" 
            ontextchanged="TextBox1_TextChanged"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="Log Event" OnClick="Button1_Click"
            ToolTip="Log an event at a known level." />
    </asp:Panel>

    <p>Windows Azure Diagnostics enables diagnostic data that is collected to be transferred to storage for later viewing. The transfer of data can be completed on a scheduled interval or can be done on-demand.</p>
        
    <asp:UpdatePanel runat="server" ID="LogTransferPanel">
        <ContentTemplate>
        <asp:Label ID="TransferErrorMessage" runat="server" ForeColor="Red" Visible="false" />
        <div id="TransferSetupPanel" runat="server">
            <p>Click the button to transfer all Windows Azure logs from the last N minutes.</p>        
            <p>
                <asp:Button ID="PushAzure" runat="server" Text="Transfer Logs" onclick="PushAzure_Click" />
                in the last
                <asp:TextBox ID="PushAge" runat="server" Width="30%" Text="1" />
                minutes at level
                <asp:DropDownList ID="PushLevel" runat="server" Width="20%" ToolTip="Set the minimum level of event to push.">
                    <asp:ListItem>Critical</asp:ListItem>
                    <asp:ListItem>Error</asp:ListItem>
                    <asp:ListItem>Warning</asp:ListItem>
                    <asp:ListItem>Information</asp:ListItem>
                    <asp:ListItem Selected="True">Verbose</asp:ListItem>
                </asp:DropDownList>
                or above.
            </p>
            
           
            <p>In this sample the performance counter being collected is ‘\Proccesor(_Total)\% Processor Time’.</p> 
            <p>Click the button to transfer all of the data from the performance counters.</p>    
            <p>
                <asp:Button ID="PushPerf" runat="server" Text="Transfer Performance Counters" onclick="PushPerf_Click" />
                in the last
                <asp:TextBox ID="PushPerfAge" runat="server" Width="30%" Text="1" />
                minutes.
            </p>
            
            <p>Click the button transfer all IIS logs from the last N minutes.</p>
            <p>
                <asp:Button ID="PushIIS" runat="server" Text="Transfer IIS Logs" onclick="PushIIS_Click" />
                in the last
                <asp:TextBox ID="PushIISAge" runat="server" Width="30%" Text="1" />
                minutes.
            </p>
        </div>
        <div id="TransferProgressPanel" runat="Server" visible="false">
            <p><i>Transferring...</i></p>
            <asp:Timer ID="TransferProgressTimer" runat="server" Interval="1000" OnTick="TransferProgressTimer_Tick" />
        </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <hr />
    
    <h3>External Connections</h3>
    
    <p>Both web roles and worker roles in Windows Azure are allowed to make connections to external sites.</p>
    <p>Click the button to perform a test connection via HTTP.</p>
     <asp:Panel ID="DNSResolutionErrorPanel" runat="server" Visible="false">
        <asp:Label ID="DNSResolutionErrorLabel" runat="server" ForeColor="Red" />
    </asp:Panel>
    <p>Use the following form to submit an <a href="http://www.w3.org/Protocols/rfc2616/rfc2616.txt">HTTP</a> GET request to the specified URL.</p>
    <p>
        Address:
        <asp:TextBox ID="HttpAddress" runat="server" Width="70%">http://www.example.com/</asp:TextBox>
        <asp:Button ID="HttpButton" runat="server" OnClick="HttpButton_Click" Text="Make HTTP Request" />
    </p>
    <asp:Panel ID="HttpResponsePanel" runat="server" Visible="false">
        <p>Response from server:</p>
        <pre><asp:Label ID="HttpResponseLabel" runat="server" /></pre>
    </asp:Panel>
    
    <hr />
    
    <h3>Local Storage</h3>    
    
    <p>Windows Azure also provides local storage for each role that can be accessed by using standard file I/O operations.
    This storage is persisted for the lifetime of the role. You can use the following textbox to store a message in local storage.
    This message in local storage one will be persisted until the role gets recycled, while the message in local storage two
    will persist across role recycles.</p>
    
    <asp:Panel ID="StoredMessagePanel1" runat="server">
        Message stored in local storage one: <pre><asp:Label ID="StoredMessageLabel1" runat="server"></asp:Label></pre>
    </asp:Panel>
    <asp:Panel ID="StoredMessageErrorPanel1" runat="server">
        <asp:Label ID="StoredMessageErrorLabel1" runat="server" ForeColor="Red"></asp:Label>
    </asp:Panel>
    
    <div>
        <asp:TextBox ID="InputMessageTextBox1" runat="server" Width="70%" ToolTip="Message to Log"></asp:TextBox>
        <asp:Button ID="StoreMessage1" runat="server" Text="Store Message" OnClick="StoreMessage1_Click"
            ToolTip="Store this message in the first local storage" />
    </div>

    <asp:Panel ID="StoredMessagePanel2" runat="server">
        Message stored in local storage two: <pre><asp:Label ID="StoredMessageLabel2" runat="server"></asp:Label></pre>
    </asp:Panel>
    <asp:Panel ID="StoredMessageErrorPanel2" runat="server">
        <asp:Label ID="StoredMessageErrorLabel2" runat="server" ForeColor="Red"></asp:Label>
    </asp:Panel>
    
    <div>
        <asp:TextBox ID="InputMessageTextBox2" runat="server" Width="70%" ToolTip="Message to Log"></asp:TextBox>
        <asp:Button ID="StoreMessage2" runat="server" Text="Store Message" OnClick="StoreMessage2_Click"
            ToolTip="Store this message in the second local storage" />
    </div>

    <hr />

    <h3>Internal Endpoints</h3>
    
    <p>Windows Azure roles can expose internal endpoints for inter-role communication. The worker role is exposing a
    simple TCP server that returns the current time:</p>
    
    <div>
        Time reported by worker role: <asp:Label ID="WorkerTime" runat="server" />
    </div>
    
    </form>
</body>
</html>
