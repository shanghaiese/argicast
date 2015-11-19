<%--/* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
/* 2.1	If mobile site is enabled or restricted service then login page should appear. */--%>

<%@ Page Language="C#" Title="Login Page" AutoEventWireup="true" MasterPageFile="~/Mobile/MobileMaster.master"
    CodeFile="Login.aspx.cs" Inherits="Mobile_Login" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h1 id="h1Welcome" runat="server" name=" Welcome to AgriCast!"></h1>
<form id="form1" runat="server">
    <asp:Label runat="server" ID="lblUserName" for="txtUsername" Text="&nbsp;" name="UserName" />
    <asp:TextBox runat="server" ID="txtUsername" ClientIDMode="Static" />

    <asp:Label runat="server" ID="lblPassword" for="txtPassword" Text="&nbsp;" name="Password" />
    <asp:TextBox runat="server" ID="txtPassword" ClientIDMode="Static" TextMode="Password" ></asp:TextBox>

    <asp:Button ID="btnLogin" runat="server" name="Login" data-icon="arrow-r" data-iconpos="right" Text="Login" OnClick="btnLogin_Click" />
            
    <asp:Label ID="lblLoginMessage" runat="server" Style="color: Red; text-align: center;" />
</form>
</asp:Content>

<%--/* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */--%>
