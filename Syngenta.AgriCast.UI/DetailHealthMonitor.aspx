<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DetailHealthMonitor.aspx.cs" Inherits="DetailHealthMonitor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="test" runat="server"></asp:Label>
        </div>
        <p>
            <asp:Button ID="Button1" runat="server" Height="34px" Text="Agricast DB connection" Width="178px" OnClick="Button1_Click" />
            <asp:TextBox ID="TextBox1" runat="server" Height="64px" Style="margin-left: 100px" Width="361px" TextMode="MultiLine"></asp:TextBox>
        </p>

        <p>
            <asp:Button ID="Button2" runat="server" Height="34px" Text="Weather Service connection" Width="178px" OnClick="Button2_Click" />
            <asp:TextBox ID="TextBox2" runat="server" Height="53px" Style="margin-left: 100px" Width="359px" TextMode="MultiLine"></asp:TextBox>
        </p>

        <p>
            <asp:Button ID="Button3" runat="server" Height="34px" Text="Location Search" Width="178px" OnClick="Button3_Click" />
            <asp:TextBox ID="TextBox3" runat="server" Height="65px" Style="margin-left: 100px" Width="356px" TextMode="MultiLine"></asp:TextBox>
        </p>
    </form>
</body>
</html>
