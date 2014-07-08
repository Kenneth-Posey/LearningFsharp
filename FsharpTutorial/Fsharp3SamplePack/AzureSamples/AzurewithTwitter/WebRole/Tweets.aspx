<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Tweets.aspx.cs" Inherits="WebRole.MyTweets" %>

<%@ MasterType TypeName="WebRole.SiteMaster" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="search-box">
        Tweet(140 charaters)
        <asp:TextBox ID="TextBox1" runat="server" Width="675px" MaxLength="140"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="Tweet" Width="116px" OnClick="Add_Tweet" />
    </div>
    <div class="frontpage-timeline" runat="server" id="divshow" visible="false">
        <div class="tweet">
            <asp:Image ID="Image1" runat="server" CssClass="user-image" />
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="#" Text="" />
            at
            <asp:Literal ID="Literal1" runat="server" Text="" />
            <br />
            <asp:Literal ID="Literal2" runat="server" Text="" />
        </div>
    </div>
</asp:Content>
