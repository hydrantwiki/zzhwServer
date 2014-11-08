<%@ Page Title="" Language="C#" MasterPageFile="~/App.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="HydrantWiki.Web.Home" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadStyleSheetManager ID="RadStyleSheetManager1" runat="server">
    </telerik:RadStyleSheetManager>
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" 
                Name="Telerik.Web.UI.Common.Core.js">
            </asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" 
                Name="Telerik.Web.UI.Common.jQuery.js">
            </asp:ScriptReference>
            <asp:ScriptReference Assembly="Telerik.Web.UI" 
                Name="Telerik.Web.UI.Common.jQueryInclude.js">
            </asp:ScriptReference>
        </scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    </telerik:RadAjaxManager>
    <div class="homeHeader">
        <img src="http://images.hydrantwiki.com/web/Logo_half.png" />
    </div>
    <div class="homeMenu">
        <telerik:RadPanelBar Runat="server" ID="mnuMenu" Height="100%" Width="100%">
            <Items>
                <telerik:RadPanelItem runat="server" Text="Tagger">
                    <Items>
                        <telerik:RadPanelItem runat="server" NavigateUrl="~/MyTags.aspx" Text="My Tags">
                        </telerik:RadPanelItem>
                        <telerik:RadPanelItem runat="server" NavigateUrl="~/Hydrants.aspx" 
                            Text="Hydrants">
                        </telerik:RadPanelItem>
                    </Items>
                </telerik:RadPanelItem>
                <telerik:RadPanelItem runat="server" Text="Reviewer">
                    <Items>
                        <telerik:RadPanelItem runat="server" Text="Review Tags" 
                            NavigateUrl="~/ReviewTags.aspx">
                        </telerik:RadPanelItem>
                    </Items>
                </telerik:RadPanelItem>
                <telerik:RadPanelItem runat="server" Text="Administrator">
                    <Items>
                        <telerik:RadPanelItem runat="server" Text="User Manager">
                        </telerik:RadPanelItem>
                    </Items>
                </telerik:RadPanelItem>
            </Items>
        </telerik:RadPanelBar>
    </div>
    <div class="homeMain">
        <asp:Label ID="lblUserWelcome" runat="server" CssClass="homeUserWelcome" Text=""></asp:Label>
    </div>
 </asp:Content>
