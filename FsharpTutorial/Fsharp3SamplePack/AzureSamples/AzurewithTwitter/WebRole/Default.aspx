<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebRole._Default" %>

<%@ MasterType TypeName="WebRole.SiteMaster" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        <asp:Literal ID="headerText" runat="server" />
    </h2>


    <div class="frontpage-timeline">
        <asp:ListView ID="TimelineList" runat="server" EnableViewState="false">
            <ItemTemplate>
                <div class="tweet">
                    <asp:Image runat="server" ImageUrl='<%# Eval("ProfileImageLocation") %>' CssClass="user-image" />
                    <asp:HyperLink runat="server" NavigateUrl='<%# "~/user.aspx?screenname=" + Eval("ScreenName") %>'
                        Text='<%# "@" + Eval("ScreenName") %>' />
                    at
                    <asp:Literal runat="server" Text='<%# Eval("CreatedDate") %>' />
                    <br />
                    <asp:Literal runat="server" Text='<%# Eval("LinkifiedText") %>' />
                </div>
            </ItemTemplate>
        </asp:ListView>
    </div>
</asp:Content>
