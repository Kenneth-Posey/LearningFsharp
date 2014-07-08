<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="User.aspx.cs" Inherits="WebRole.User" EnableViewState="false" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="single-user">
        <h2>
        <asp:Literal runat="server" Text='<%# TwitterUser.ScreenName %>' /></h2>
        <asp:Image runat="server" CssClass="profile-image" ImageUrl='<%# TwitterUser.ProfileImageLocation %>' />
        <asp:Literal runat="server" Text='<%# TwitterUser.Name %>' />
        <br />
        <asp:Literal runat="server" Text='<%# TwitterUser.Location %>' />
        <br />
        <asp:Literal runat="server" Text='<%# TwitterUser.Description %>' />
    </div>
    <asp:ListView ID="TimelineList" runat="server">
        <ItemTemplate>
            <div class="tweet">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# "~/user.aspx?screenname=" + Eval("ScreenName") %>'
                    Text='<%# "@" + Eval("ScreenName") %>' />
                at
                <asp:Literal ID="Literal1" runat="server" Text='<%# Eval("CreatedDate") %>' />
                <br />
                <asp:Literal ID="Literal2" runat="server" Text='<%# Eval("LinkifiedText") %>' />
            </div>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>
