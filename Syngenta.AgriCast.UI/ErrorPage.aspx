<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ErrorPage.aspx.cs" Inherits="ErrorPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div><h3><span style="color:Red;">
    A critical application error has occured.</h3></span>
    Click <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click"> here </asp:LinkButton>to start again.
    </div>
    </form>
    
</body>
</html>
