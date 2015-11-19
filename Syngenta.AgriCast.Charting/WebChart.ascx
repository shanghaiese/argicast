<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebChart.ascx.cs" Inherits="Syngenta.AgriCast.Charting.WebChart" %>
<%-- /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - Begin */ 
    /* 3.3.1	Charting component should have zooming enabled. */
--%>
<script type="text/javascript">
    /*IM01166162 - AgriInfo UI Issues - Begin*/
    //Added aditonal param "isAgriInfo"

    function showBigImage(url, isAgriInfo) {
       
        var features = 'height=600px,width=1050,toolbar=0,status=0,resizable=1,location=0,menuBar=0';
        //var features = 'height=600px,width=1050,toolbar=0,scrollbars=0,status=0,resizable=1,location=0,menuBar=0';
        if (isAgriInfo.toLowerCase() == "true")
            features = features + ",scrollbars=1"; // add scroll bars for agriInfo
        else
            features = features + ",scrollbars=0";
        /*IM01166162 - AgriInfo UI Issues - End*/

        window.open(url, 'name', features).focus();
    }
</script>
<%-- /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - End */ --%>
<div id="divFcChart" runat="server">
    <div id="chartFeedback" class="Right" runat="server">
        <label id="lblRating" runat="server" class="label110" name="Rate this site">
            Rate this chart</label>
        <div id="rating_Chart" class="RateIt" data-accesskey="rate">
        </div>
    </div>
</div>
<div class="chartDiv" id="chrtDiv" runat="server" style="width: auto; *width: auto">
    <%-- /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - Begin */ 
    /* 3.3.1	Charting component should have zooming enabled. */
    /*  
        style="cursor:pointer;" 
        title="Click to view bigger chart image."
    */
    --%>
    <img id="imgChart" runat="server" src="" alt="ChartImage" class="imgArrow" style="cursor: pointer;"
        title="Click to view bigger chart image." />
    <%-- /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - End */ --%>
    <%--/* Agricast CR - R6 - Add wind icons and legend for Humidity - Begin */--%>
    <asp:PlaceHolder ID="windIcons" runat="server"></asp:PlaceHolder>
    <%--/* Agricast CR - R6 - Add wind icons and legend for Humidity - End */--%>
</div>
