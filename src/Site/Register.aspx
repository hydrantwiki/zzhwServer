<%@ Page Title="" Language="C#" MasterPageFile="~/App.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="HydrantWiki.Web.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   
    <div class="masterHeader" class="masterHeader">
        <img src="http://images.hydrantwiki.com/web/Logo_half.png" />
    </div>
      <div id="masterTitle" class="masterTitle">
        <asp:Label ID="lblTitle" runat="server" Text="Register" CssClass="lblMasterTitle"></asp:Label>
    </div> 

    <div id="registerBody" class="registerBody">
        <asp:Panel runat="server" ID="pnlRegister" Height="100%" Width="100%" >
            <asp:Label runat="server" Text="UserName" ID="lblUserName" ClientIDMode="Static" ></asp:Label>
            <asp:TextBox ID="txtUserName" runat="server" ontextchanged="txtUserName_TextChanged" ClientIDMode="Static" ></asp:TextBox>
            <asp:Label runat="server" Text="" ID="lblUserNameAvailable" ClientIDMode="Static" ></asp:Label>
            <br />
            <asp:Label runat="server" Text="Password" ID="lblPasswordFirst" ClientIDMode="Static" ></asp:Label>
            <asp:TextBox ID="txtPasswordFirst" runat="server" ClientIDMode="Static" TabIndex="2" TextMode="Password"></asp:TextBox>
            <br />
            <asp:Label runat="server" Text="Reenter" ID="lblPasswordReenter" ClientIDMode="Static" ></asp:Label>
            <asp:TextBox ID="txtPasswordReenter" runat="server" ClientIDMode="Static" TabIndex="3" TextMode="Password"></asp:TextBox>
            <br />
            <asp:Label runat="server" Text="Email Address" ID="lblEmailAddress" ClientIDMode="Static" ></asp:Label>
            <asp:TextBox ID="txtEmailAddress" runat="server" ClientIDMode="Static" TabIndex="4" ></asp:TextBox>
            <asp:Label runat="server" Text="" ID="lblEmailAvailable" ClientIDMode="Static"></asp:Label>
            <br />
            <asp:Button ID="btnCreateAccount" runat="server" Text="Create Account" onclick="btnRegister_Click" />
        </asp:Panel>
    </div>
</asp:Content>
