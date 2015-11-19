<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Login.aspx.cs" Inherits="Login" %>

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
        <script type="text/javascript">

            function ValidateLogin() {
                var Username = document.getElementById("ctl00$MainContent$txtUsername")
                var Password = document.getElementById("ctl00$MainContent$txtPassword")
                if (Username != null && Password != null) {
                    if (Username.value == "" || Password.value == "") {
                        alert("UserName and Password are mandatory ")
                        var errorMsg = document.getElementById("MainContent_lblLoginMessage")
                        if (errorMsg != null) {
                            errorMsg.value = "UserName and Password are mandatory";
                        }
                        return false;
                    }

                }
                return true;
            }
        </script>
    </head>
    <body>
        <div id="divErrorMessage" runat="server" visible="false">
            <asp:Label ID="lblLoginMessage" runat="server" Style="color: Red; text-align: center;" />
        </div>
        <div id="divLoginLink" runat="server" visible="false" >
         <asp:Label ID="lblClick" runat="server" Text="Click" />
         <a href="#" id="lnkLogin" runat="server">here</a>
          <asp:Label ID="lblLoginAgain" runat="server"  Text="To Login Again"/>
        </div>
        <div id="loginDiv" class="logindiv" runat="server">
            <h1 id="h1Welcome" runat="server" name=" Welcome to AgriCast!">
            </h1>
            <fieldset class="fieldset">
                <legend id="lgndLoginInfo" runat="server" name="Login Information"></legend>
                <div>
                    <asp:Label ID="lblUserName" runat="server" Text="UserName" name="UserName"></asp:Label>
                </div>
                <asp:TextBox ID="txtUsername" runat="server" MaxLength="50" Style="width: 150px;"
                    CssClass="inputbox input-ie7"></asp:TextBox>
                <br />
                <div>
                    <asp:Label ID="lblPassword" runat="server" Text="Password" name="Password"></asp:Label>
                </div>
                <asp:TextBox ID="txtPassword" runat="server" MaxLength="50" CssClass="inputbox input-ie7" Style="width: 150px;"
                    TextMode="Password"></asp:TextBox>
                <div id="formButtons" style="padding-top: 5px;">
                    <asp:Button ID="btnLogin" runat="server" Text="Login" name="Login" CssClass="button"
                        OnClick="btnLogin_Click" OnClientClick="Javascript:return ValidateLogin();" />&nbsp;&nbsp;&nbsp;<a
                            id="aForgotPassword" runat="server" href="ForgotPassword.aspx" name="Forgot Password?">Forgot
                            Password?</a>
                </div>
            </fieldset>
            
        </div>
    </body>
    </html>
</asp:Content>
