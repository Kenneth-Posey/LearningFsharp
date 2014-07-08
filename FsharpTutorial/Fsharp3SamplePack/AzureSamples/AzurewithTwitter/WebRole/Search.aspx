<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Search.aspx.cs" Inherits="WebRole.Search" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="search-box">
What's your query, caller?
<asp:TextBox ID="SearchTextBox" runat="server" /> <asp:Button runat="server" ID="SearchButton" Text="Search" OnClick="SearchButton_Click" />
</div>
    <div class="frontpage-timeline">
        <asp:ListView ID="SearchResultsListView" runat="server" EnableViewState="false">
            <ItemTemplate>
                <div class="tweet">
                    <asp:Image runat="server" ImageUrl='<%# Eval("ProfileImageLocation") %>'
                        CssClass="user-image" />
                    <asp:HyperLink runat="server" NavigateUrl='<%# "~/user.aspx?screenname=" + Eval("FromUserScreenName") %>'
                        Text='<%# "@" + Eval("FromUserScreenName") %>' />
                    at
                    <asp:Literal runat="server" Text='<%# Eval("CreatedDate") %>' />
                    <br />
                    <asp:Literal runat="server" Text='<%# Eval("Text") %>' />
                </div>
            </ItemTemplate>
        </asp:ListView>
    </div>
</asp:Content>
