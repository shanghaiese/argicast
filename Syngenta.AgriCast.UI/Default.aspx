<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   <script type="text/javascript" src="../../Scripts/jquery-1.8.2.min.js"></script>
   <script type="text/javascript" src="../../Scripts/jquery.blockUI.js" ></script> 
   <script type="text/javascript" src="../../Scripts/spin.min.js"></script>
   <script type="text/javascript">
       $(document).ready(function () {
           $.blockUI.defaults.message = $('#blockui-animated-content');
           $.blockUI.defaults.css.width = '250px';
           $.blockUI.defaults.css.top = '30%';
           
           var opts =
               {
               lines: 13, // The number of lines to draw
               length: 15, // The length of each line
               width: 10, // The line thickness
               radius: 20, // The radius of the inner circle
               corners: 1, // Corner roundness (0..1)
               rotate: 50, // The rotation offset
               direction: 1, // 1: clockwise, -1: counterclockwise
               color: '#000', // #rgb or #rrggbb or array of colors
               speed: 0.8, // Rounds per second
               trail: 42, // Afterglow percentage
               shadow: false, // Whether to render a shadow
               hwaccel: false, // Whether to use hardware acceleration
               className: 'spinner', // The CSS class to assign to the spinner
               zIndex: 2e9, // The z-index (defaults to 2000000000)
               top: 'auto', // Top position relative to parent in px
               left: 'auto' // Left position relative to parent in px
              };
           var target = document.getElementById('preview');
           var spinner = new Spinner(opts).spin(target);

           $.blockUI();
       });
    </script>
    <link type="text/css" rel="stylesheet" href="Styles/Site.css" />
    <link href="Styles/Common.css" rel="Stylesheet" type="text/css" />
</head>
<body>
        <div id="blockui-animated-content" style="display: none; padding: 65px">
            <div id="preview" style="margin-right:7px; vertical-align: middle; display: inline-block">
               <%-- <img src="../../Images/ajax-loader.GIF" alt="Processing..."/>--%>
            </div>            
        </div>
</body>
</html>

<%--IM01865638:New Agricast - default.aspx not stylable - start--%>
<%--<script type="text/javascript">
    var switchInt = setInterval(switchPage, 1000);
    var count = -1;
    function switchPage() {
        if (count > 0) {
            clearInterval(switchInt);
            window.location.href = "<%= switchUrl%>";
        }
        else {
            count++;
        }
    }
</script>--%>
<%--IM01865638:New Agricast - default.aspx not stylable - start--%> 
