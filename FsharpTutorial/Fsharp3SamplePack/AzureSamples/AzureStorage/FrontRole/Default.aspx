<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="FrontRole._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <p>
    Hello Windows Azure logs .</p>
<p>
    &nbsp;</p>
<asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
    Text="Transfer Logs" />
&nbsp; Click this buttone to transfer the logs to Windows Azure Storage ,in 
on-demand way.<br />
    <asp:Label ID="TransferTB" runat="server" Text="Ready to transfer logs"></asp:Label>
</asp:Content>
