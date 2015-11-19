<%@ Page Title="Agricast-Weather Services" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeFile="Default2.aspx.cs" Inherits="_Default2" EnableEventValidation="false"
    ValidateRequest="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
   <%-- <div style="z-index: 10000; border: medium none; margin: 0pt; padding: 0pt; width: 100%; height: 100%; top: 0pt; left: 0pt; background-color:transparent; filter:alpha(opacity=60);opacity:0.1; cursor: wait; position: fixed;" class="blockUI blockOverlay"></div>--%>

    <link href="../../Styles/jqueryslidemenu.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="../../Styles/bootstrap.css"/>
    <script type="text/javascript" src="../../Scripts/jquery-1.8.2.min.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery-ui.min.js"></script>
    <script type="text/javascript" src="../../Scripts/Common.js"></script>
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?v=3.6&amp;sensor=false"></script>
    <script type="text/javascript" src="../../Scripts/GoogleMaps.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.blockUI.js" ></script>
    <script type="text/javascript" src="../../Scripts/jquery.autocomplete.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.datepick.lang.js"></script>
    <script type="text/javascript" src="../../Scripts/spin.min.js"></script>
    <script src="../../Scripts/jqueryslidemenu.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.tablesorter.js" type="text/javascript"></script> 
    <script type="text/javascript" src="../../Scripts/bootstrap.js"></script>    
    <style type="text/css">
        DYNAMICCSS</style>
    <script type="text/javascript">
        $(document).ready(function () {
            $.blockUI.defaults.message = $('#blockui-animated-content');
            $.blockUI.defaults.css.width = '250px';
            $.blockUI.defaults.css.top = '30%';
            $('#btnSearch').click(function () {
                $.blockUI();
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

            });
            $('#pageMenu ul li a').click(function () {
                $.blockUI();
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

            }); 
        }); 
       

        var locs;
        function pageLoad() {
            common();
        }

        function fnPrintPage() {

            printpage = window.open('print.aspx', 'printpage', 'resizable=1,scrollbars=1,height=700,width=950'); //'location=0,status=0,scrollbars=1'
            printpage.focus();
            printpage.moveTo(0, 0);
            return false;
        }


        function fnTrigHdnBtnAdvancedOption(path) {
            link = path;

            $("#btnHiddenAdvOptions").click();
        }

        function openMore() {
            var url = link;
            var windowName = "Advanced_Options";
            var windowSize = "top=300,left=500,height=250,width=400,status=yes,toolbar=no,menubar=no,location=no,resize=yes"
            window.open(url, windowName, windowSize);

        }


        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

        ga('create', 'UA-190910-32', 'auto');
        ga('send', 'pageview');

        
        $(function () {
            // click to display zoomed chart image
            $('#float-icon-legend').click(function () {
                var $chart = $('#imgChart');
                if ($chart.length > 0)
                    $chart.trigger('click');
            });
        });
    </script>
</asp:Content>
<asp:Content ID="MossMenu" runat="server" ContentPlaceHolderID="ph_MenuTabsTop">
     <div id="blockui-animated-content" style="display: none;  padding: 65px">
            <div id="preview" style="margin-right: 7px; vertical-align: middle; display: inline-block">
               <%-- <img src="../../Images/ajax-loader.GIF" alt="Processing..."/>--%>
            </div>            
        </div>
    <div>
        <div id="slidemenu" runat="server">
            <div id="slide">
            </div>
            <div id="slidemenu1" class="jqueryslidemenu">
                <asp:PlaceHolder ID="MossMenuTab" runat="server"></asp:PlaceHolder>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Menu" runat="server" ContentPlaceHolderID="ph_MenuTabs">
    <asp:PlaceHolder ID="MenuTabs" runat="server"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div id="divPrint" class="hide">
        <%--<img id="agricastLogo" src="../../Images/agricast_rgb_big.png" alt="AgriCast Logo" />--%>
        <asp:Label ID="lblPrintHeader" runat="server" CssClass="hide" ClientIDMode="Static">PRINTHEADER</asp:Label>
    </div>
    <asp:Label ID="lblError" runat="server" ClientIDMode="Static" CssClass="Error" data-key="test"
        Text="~Text~"></asp:Label>
    <asp:PlaceHolder ID="ToolbarHolder" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="WebLocSearchHolder" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="CentrePlaceHolder" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="bottomHolder" runat="server"></asp:PlaceHolder>
    <asp:HiddenField ID="hdnRate" runat="server" ClientIDMode="Static" ViewStateMode="Enabled" />
    <asp:Button ID="btnHiddenPrint" runat="server" ClientIDMode="Static" CssClass="hide" />
    <asp:Button ID="btnHiddenAdvOptions" runat="server" ClientIDMode="Static" CssClass="hide"
        OnClientClick="openMore();" />
    <script type="text/javascript" src="../../Scripts/jquery.rateit.js"></script>
    <div id="divPrintPopup" class="hide">
        <iframe src="" id="iframePrintPopup" style="width: 100%; height: 100%"></iframe>
    </div>
</asp:Content>
<asp:Content ID="Footer" runat="server" ContentPlaceHolderID="PageFooter">
    <ul id="footNav" runat="server" style="list-style-type: none; text-align: center;">
    </ul>
</asp:Content>
