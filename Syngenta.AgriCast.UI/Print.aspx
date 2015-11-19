<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Print.aspx.cs" Inherits="Print" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="../../Scripts/Common.js"></script>
    <script type="text/javascript" language="javascript">

        function fnPrintWindow() {

            window.print();
        }

        function fnCloseWindow() {
            window.close();
        }

    </script>
</head>
<body onload="self.focus();">
    <form id="form1" runat="server">
    <div>
        <div id="divTopLeft" style="position: absolute" class="DoNotPrint">
            <table>
                <tr>
                    <td>
                        <asp:Button ID="btnPrint" runat="server" Text="Print" CssClass="buttonCommon" Style="z-index: 2" />
                    </td>
                    <td>
                        <asp:Button ID="btnClose" runat="server" Text="Close" CssClass="buttonCommon" Style="z-index: 2" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="divSectionButton" style="width: 100%">
            <img id="imgPrint" runat="server" src="" alt="No Print Image" />
        </div>
    </div>
    </form>
</body>
</html>
