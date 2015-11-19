<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="ForgotPassword.aspx.cs" Inherits="ForgotPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ph_MenuTabsTop" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ph_MenuTabs" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="Server">
    <html>
    <head>
        <title>
            <asp:Literal ID="lb_Title" runat="server"></asp:Literal></title>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <asp:Literal ID="includesCode" runat="server"></asp:Literal>
        <script type="text/javascript" language="javascript">

            function ValidateLogin() {

                var Username = document.getElementById("ctl00$MainContent$txtUsername")

                if (Username != null) {
                    if (Username.value == "") {
                        alert("Please Enter the UserName")

                        return false;
                    }

                }
                return true;
            }
        </script>
    </head>
    <body>
        <div id="loginDiv">
            <div>
                <asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label></div>
            <div>
                <label for="txtUserName" id="lblUserName" runat="server" name="UserName">
                </label>
                <asp:TextBox ID="txtUserName" runat="server" CssClass="inputbox" MaxLength="100"></asp:TextBox>
                <asp:Button ID="btnSendToMail" name="EMail My Password" runat="server" Text="EMail My Password"
                    ForeColor="White" CssClass="button" Style="font-size: 12px; width: 150px;" OnClick="btnSendToMail_Click"
                    OnClientClick="javascript:return ValidateLogin();" />
            </div>
        </div>
    </body>
    </html>
</asp:Content>
