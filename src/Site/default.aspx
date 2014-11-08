<%@ Page Title="HydrantWiki" Language="C#" MasterPageFile="Base.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="HydrantWiki.Web._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="fb-root"></div>
    
    <asp:PlaceHolder ID="phFBScript" runat="server"></asp:PlaceHolder>
    
    <div class="frame">
        <div class="Logo"><img src="http://images.hydrantwiki.com/web/Logo_half.png" /></div>
        <div class="summary">
            <span class="hydrant_title">Hydrants: 
                <asp:Label ID="lblHydrantCount" runat="server" Text=""></asp:Label>
            </span>
            <span class="tag_title">Tags: 
                <asp:Label ID="lblTagCount" runat="server" Text=""></asp:Label>
            </span>
            <span class="user_title">Contributors: 
                <asp:Label ID="lblUserCount" runat="server" Text=""></asp:Label>
            </span>
        </div>
        <div class="login" id="login">   
            <asp:Panel runat="server" ID="pnlLogin" Height="100%" Width="100%">
                <asp:Label runat="server" Text="UserName" ID="lblUserName" 
                    ClientIDMode="Static"></asp:Label>
                <asp:TextBox ID="txtUserName" runat="server" ClientIDMode="Static"></asp:TextBox>
                
                <br />
                <asp:Label runat="server" Text="Password" ID="lblPassword" 
                    ClientIDMode="Static"></asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" ClientIDMode="Static" TextMode="Password"></asp:TextBox>
                <br />
                <div class="chkRemember">
                    <asp:CheckBox runat="server" ID="chkRemember" Text="Remember Me" 
                        ClientIDMode="Predictable"></asp:CheckBox> 
                </div>
                <asp:Button ID="btnRegister" runat="server" Text="Create Account" ClientIDMode="Static" onclick="btnRegister_Click" />
                <asp:Button ID="btnLogin" runat="server" Text="Login" ClientIDMode="Static" onclick="btnLogin_Click" />
                <asp:Label runat="server" ID="lblStatus" Width="100%"  ClientIDMode="Static"></asp:Label>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlEnterApp" Width="100%">
                <asp:Button ID="btnEnterApp" runat="server" Text="Enter App" ClientIDMode="Static" onclick="btnEnterApp_Click" />
                <asp:Button ID="btnLogOut" runat="server" Text="Logout" ClientIDMode="Static" onclick="btnLogOut_Click" />
            </asp:Panel>
        </div> 
        <div class="license">
            HydrantWiki's Data is made available under Open Database License whose full text can be found at <a href="http://opendatacommons.org/licenses/odbl/1-0/" target="_blank">ODbL Full License</a>. A Human readable summary can be found at <a href="http://opendatacommons.org/licenses/odbl/summary/" target="_blank">ODbL Summary</a>    
        </div>
        <div class="side">
            <img src="http://images.hydrantwiki.com/web/hydrant_sidebar.png" alt="Hydrant" width="200" height="500"/>
        </div>
    </div>
</asp:Content>
