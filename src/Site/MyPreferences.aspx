<%@ Page Title="" Language="C#" MasterPageFile="~/App.Master" AutoEventWireup="true" CodeBehind="MyPreferences.aspx.cs" Inherits="HydrantWiki.MyPreferences" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="MyTagsHeader">
        <asp:HyperLink class="HomeLink" ID="hypHome" runat="server" NavigateUrl="~/Home.aspx">Home</asp:HyperLink>
        <asp:Label ID="lblTitle" runat="server" Text="My Preferences" CssClass="lblMyTagsHeader"></asp:Label>
    </div> 
</asp:Content>
