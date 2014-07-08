<%@ Page Title="Generate Failed IIS request log" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="FrontRole.About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Failed Request
    </h2>
    <p>
        The PageLoad of this page has a hard-coded 18 second sleep which given the 
        parameters in web.config will generate a failed request log.
    </p>
    <p>
        Click the button to get the failed request</p>
    <p>
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
            Text="Failed Request" />
    </p>
    <p dir="ltr">
        <asp:Label ID="RequestTB" runat="server" 
            Text="Wait 18 secs to generate the failed request."></asp:Label>
    </p>
</asp:Content>
