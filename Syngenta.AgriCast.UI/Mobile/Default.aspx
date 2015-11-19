<%@ Page Title="" Language="C#" MasterPageFile="~/Mobile/MobileMaster.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Mobile_Default" ClientIDMode="Static" %>


<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<form id="form1" action="Default.aspx" runat="server" data-ajax="false" data-fetch="always">
   

            <div id="divSearchLoc" runat="server" data-role="collapsible" data-content-theme="c">
                <h3 id="hSearchLoc" runat="server">
                    Search Location</h3>
              
                <div data-role="fieldcontain" class="changeLocation" id="divChangeLoc"> 
                <br />
                    <label id="lblCountry" for="ddlCountry" class="select" runat="server">
                        Search in:</label>
                    <asp:DropDownList ID="ddlCountry" runat="server" />
                    <label id="lblSearchbox" for="searchbox" runat="server">
                        For:</label>
                    <input id="searchbox" type="text" runat="server" autofocus="autofocus" placeholder="Enter the search criteria" />                        
                    <asp:Button ID="btnSearch" runat="server" Text="Search" onclick="btnSearch_Click" OnClientClick="collapseSearch();" />
            <div id="divSelectSingleLoc" runat="server" visible="false">
                <br />
                <label id="lblSelectSingle" for="ddlCountry" class="select" runat="server">
                    Select Location:</label>
                <asp:DropDownList ID="ddlMultiplePlaces" runat="server">
                </asp:DropDownList>
                <asp:Button ID="btnSearchSingle" runat="server" Text="Continue" OnClick="btnSearchSingle_Click"
                    OnClientClick="collapseSearch();" />
                </div>
            </div>
    </div>
            <asp:Label ID="lblNoMatch" runat="server" Text="No Results Found" Visible="false" style="color:Red;"
                CssClass="Errorhide"></asp:Label> 
                  <asp:Label ID="lblErrorMessage" runat="server" Text="No Results Found" Visible="false" style="color:Red;"
                CssClass="Errorhide"></asp:Label> 
            <div id="divLocation" class="hide" runat="server"> 
                <h4>
                    <asp:Label ID="lblLocationName" runat="server" Text="" Style="font-weight: bold;"></asp:Label>
                    <asp:Label ID="lblDateTime" runat="server" Text="" Style="color: Gray;"></asp:Label>
                </h4>
                <div class="ui-grid-a">                   
                   <div class="ui-block-a" style="text-align: center;">
                     <asp:Image ID="imgTemp" ImageUrl="" AlternateText="No Image" runat="server" />
                    <br />
                      <asp:Image ID="imgWindDir" ImageUrl="" AlternateText="Wind Direction" runat="server" />
                </div>             
                <div class="ui-block-b" style="text-align:left;">
<%-- /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */ 
    /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */
--%>
    <asp:Literal ID="ltlSeries" runat="server"></asp:Literal>
<%-- // commented for new implementation
                    <asp:Label ID="lblMax" Text="Max:&nbsp;" Style="font-size: medium;color:Gray;" runat="server" />
                    <asp:Label ID="lblTempMax" Text="" style="font-size:medium; color:Gray;" runat="server" />
                    <label id="lblMaxUnit" style="font-size: medium; color: Gray;" runat="server">&deg;</label>
                      <br />
                    <asp:Label ID="lblMin" Text="Min:&nbsp;" Style="font-size: medium;color:Gray;" runat="server" />
                    <asp:Label ID="lblTempMin" Text="" style="font-size:medium; color:Gray;" runat="server" />                     
                    <label id="lblMinUnit" style="font-size: medium; color: Gray;" runat="server">&deg;</label>
                    <br />
                    <asp:Label ID="lblWind" Text="Wind:&nbsp;" Style="color: Gray;" runat="server" />                   
                    <asp:Label ID="lblWindSpeed" Text="" style="color:Gray;" runat="server"/>                  
                    <label id="lblSpeedUnit" style="font-size: medium; color: Gray;" runat="server"></label>
                    
                    <br />
                    <asp:Label ID="lblRain" Text="Rain:" Style="color: Gray;" runat="server" />
                    <asp:Label ID="lblPrecip" Text="" style="color:Gray;" runat="server"/>                   
                    <label id="lblPrecipUnit" style="font-size: medium; color: Gray;" runat="server">&nbsp;</label>
--%>
<%-- /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */ --%>
                </div>
                </div>
                <br />                      
                <br />
                <ul data-role="listview">
                     <li data-theme="c"><a href="ChartView.aspx?module=MobileForecast" id="Chart" runat="server">Forecast Chart</a></li>
                     <li data-theme="c"><a href="MobileForecast.aspx" id="DayForecast" runat="server">5 Day Forecast</a> </li>
                     <li data-theme="c"><a href="Spray.aspx" id="SprayWindow" runat="server">Spray Window</a></li>
                     <li data-theme="c"><a href="ChangeSettings.aspx" id="units" runat="server" >Settings</a></li>                     
                </ul>
                <asp:HiddenField ID="hdnLatLng" runat="server" />
                <asp:HiddenField ID="hdnLocSelected" runat="server" />
                <input type="hidden" id="refreshed" value="no" />
            </div>      
    </form>
</asp:Content>
