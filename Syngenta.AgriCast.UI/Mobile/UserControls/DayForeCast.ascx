<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DayForeCast.ascx.cs" Inherits="Mobile_DayForeCast" %>
<%--<%@ Register src="SprayWindowImg.ascx" tagname="SprayWindowImg" tagprefix="uc1" %>--%>
<div id="forecast">      
    <ul data-role="listview">
        <asp:Repeater ID="Repeater1" runat="server" EnableViewState="false" 
            onitemdatabound="Repeater1_ItemDataBound">
            <ItemTemplate>
                <li data-role="divider" data-theme="b"><strong>
                    <%# Eval("NewDate")%></strong></li>                    
                <li><%--<a data-url='?WindDir=<%# Eval("winddirtext")%>&Precip=<%#Eval("precip")%>&WindSpeed=<%#Eval("windspeed")%>&TempMax=<%#Eval("maxTemp")%>&TempMin=<%#Eval("minTemp")%>&ImageURL=<%# Eval("imageurl")%>' href='javascript:void(0)' />--%>
                   <div class="ui-grid-a" style="font-size: medium;">
                        <div class="ui-block-a" style="text-align: center;">
                            <img src="<%#Eval("imageurl")%>" alt="No Image" />
                            <br />
                            <span style="color: Gray;">
<%-- /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */ 
        /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */
    --%>
    <img src="../../Images/<%#Eval("winddirectionText")%>.gif" alt="<%#Eval("winddirectionText")%>" />
<%--<img src="../../Images/<%#Eval("winddirtext")%>.gif" alt="<%#Eval("winddiretext")%>" />--%>
<%-- /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */ --%>
                            </span>
                        </div>
                        <div class="ui-block-b" style="text-align: left;font-weight:normal">
<%-- /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */ 
        /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */
    --%>
                            <asp:Literal ID="ltlSeries" runat="server"></asp:Literal>
<%-- // commented for new implementation
                        <label id="lblMax" style="color: Gray;" runat="server"><%=Max%>:&nbsp;</label> 
                            <span style="color: Gray;"><%#Eval("maxTemp")%>&deg;
                            <label id="Label1" style="font-size: smaller;" runat="server"></label></span>
                            <br />
                              <label id="lblMin" style="color: Gray;" runat="server"><%=Min%>:&nbsp;</label>
                            <span style="color: Gray;"><%#Eval("minTemp")%>&deg;
                            <label id="Label2" style="font-size: smaller;"
                                runat="server"></label></span>
                            <br />
                             <label id="lblWind" style="color: Gray;" runat="server"><%=Wind%>:&nbsp;</label>
                            <span style="color: Gray;"></span>
                            <span style="color: Gray"> <%#Eval("windspeed")%>
                                <label id="Label3" style="font-size: smaller;" runat="server">
                                    </label></small></span>
                                    <%//this is commented originally--<span style="color: Gray;">
                                <img src="../../Images/<%#Eval("winddirtext")%>.gif" alt="<%#Eval("winddirtext")%>"/>
                            </span>--//%>
                                    <br />
                           <!-- <span style="color: Gray;">Rain:&nbsp;</span>-->
                            <label id="lblRain" style="color: Gray;" runat="server"><%=Rain%>:&nbsp;</label>
                            <span style="color: gray;">
                                <%#Eval("precip")%><label id="Label4" style="font-size: smaller;" runat="server"></label></span> 
--%>
<%-- /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */ --%>
                        </div>
                    </div>                  
                    </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</div>
