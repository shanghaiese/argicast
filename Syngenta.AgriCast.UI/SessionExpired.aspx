<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SessionExpired.aspx.cs" Inherits="SessionExpired" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../../Scripts/jquery-1.8.2.min.js"></script>
    <script type="text/javascript">
        //window.history.forward(1);
        function DisplaySessionTimeout() {
            //we are in an iframe 
            document.getElementById("message").style.display = "none"
            if (self != top) {
                if (document.location.href.indexOf("syngenta.com") != -1) {
                    document.domain = "syngenta.com";
                }
                localdomain = self.document.domain
                localHref = self.document.location.href
                var par = self.parent
                var pardomain = ""
                try {
                    pardomain = par.document.domain
                }
                catch (err) {
                    pardomain = ""
                }
                //we are in the same domain
                if (pardomain == localdomain) {
                    window.top.location.reload();
                }
            }
            document.getElementById("message").style.display = "block"
        }

    </script>
</head>
<body onload="DisplaySessionTimeout()">
    <form id="form1" runat="server">
        <%-- /* Agricast CR - 3.5	R5 - External user management changes. - Begin */ --%>
        <div id="message" runat="server" style="display: none">
            <%-- /* Agricast CR - 3.5	R5 - External user management changes. - End */ --%>
    Session expired.
            <br />
            Click <a href="Default.aspx" id="hdefault" runat="server">here</a> to start again.
        </div>
    </form>
</body>
</html>
