<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebLocationSearch.ascx.cs"
    Inherits="Syngenta.AgriCast.LocationSearch.WebLocationSearch" %>
<div id="divSearch" class="container" runat="server">
    <table>
        <tr style="background-color:rgb(239, 237, 234)">
            <td >
                <div class="label100" Style="width:auto;margin-right:10px;">
                    <asp:Button ID="btnDefault" runat="server" ClientIDMode="Static" Style="display: none"
                        OnClientClick="return false" />
                    <asp:Label ID="searchIn" runat="server" Style="width: 50%; white-space:nowrap;" Text="Search in"></asp:Label>
                </div>
            </td><td>
                <div class="dropDown" style="width:auto;">
                    <asp:DropDownList ID="ddlCountry" runat="server" ClientIDMode="Static" />
                </div>
            </td><td>
                <div class="label50" Style="width:auto;margin-right:10px;">
                    <asp:Label ID="for" runat="server" Style="white-space:nowrap;"> for</asp:Label>
                </div>
            </td><td Style="width:100%; padding-right:10px;">
                <div class="searchbox" Style="width:100%">
                    <asp:Panel ID="pnSearch" runat="server" DefaultButton="btnSearch">
                        <input id="searchbox" class="seachInput" type="text" runat="server" autofocus="autofocus"
                            autocomplete="off" style="width: 100%;"/>
                            <asp:HiddenField ID="searchboxLatLng" runat="server" ClientIDMode="Static" />
                    </asp:Panel>
                </div>
            </td><td>
  <%--  <asp:ImageButton ID="btnSearch" ClientIDMode="Static" runat="server" ImageUrl="~/Images/Search.png"
        AlternateText="Search" OnClick="btnSearch_Click" CausesValidation="False" Style="margin-top: 3px;
        float: left;" />--%>
        <asp:Button id="btnSearch" ClientIDMode="Static" runat="server"  CausesValidation="False" class="button" Text="Search" onclick="btnSearch_Click" 
            Style="margin-top: 3px;margin-right:10px;float: right;white-space:nowrap;cursor:pointer"></asp:Button>
            </td><td>
         </tr>
    </table>
    <div id="divStnName">
                    <div id="StnName" class="split" runat="server">
                        <asp:Label ID="lblPointText" runat="server" Visible="false">
                            Forecast for</asp:Label>
                        <label id="lblStnName" runat="server" visible="false">
                            $StnName</label>
                        <label id="lblDistText" runat="server" visible="false">
                            $Distance</label>
                    </div>
                    <div class="split">
                        <asp:HyperLink ID="hNearByPoint" Text="NearByStations" Style="cursor: pointer; text-decoration: underline;
                            color: #0093cf;" runat="server"></asp:HyperLink>
                    </div>
                </div>
</div>
<div id="list">
</div>
<div id="divLocation" class="hide" runat="server">
    <div class="tab">
        <ul id="tabs">
            <li id="liList">
                <img id="imglist" src="../../Images/list.jpg" alt="Places" data-controls="List" data-activedescendant="mapImage"
                    title="List View" />
                <span id="textlist" runat="server" title="List View"></span></li>
            <li id="liMap" runat="server">
                <img id="imgmap" src="../../Images/map.jpg" alt="Map" data-controls="mapImage" data-activedescendant="List"
                    title="Map View" />
                <span id="textmap" runat="server" title="Map View"></span></li>
        </ul>
    </div>
    <div id="LocClose" class="close" role="button" data-controls="divLocation">
        X</div>
    <div id="List">
        <div id="LocList" class="avoidscroll">
            <label id="Loc_NoMatchFor" runat="server" visible="false" class="label100">
                No data found</label>
            <asp:GridView ID="gvLocation" runat="server" AutoGenerateColumns="False" ClientIDMode="Static"
                OnRowDataBound="gvLocation_RowDataBound" OnRowCommand="gvLocation_RowCommand"
                class="tablesorter">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name">
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="250px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Latitude" HeaderText="Latitude" SortExpression="lat">
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="100px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Longitude" HeaderText="Longitude" SortExpression="lng">
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="100px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="AdminName1" HeaderText="Admin Name 1" SortExpression="adminName1">
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="200px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="AdminName2" HeaderText="Admin Name 2" SortExpression="adminName2">
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="200px" />
                    </asp:BoundField>
                    <asp:ButtonField CommandName="Select" Text="Select" Visible="false" />
                    <asp:BoundField DataField="PlaceID" HeaderText="PlaceID" HeaderStyle-CssClass="hide"
                        ItemStyle-CssClass="hide">
                        <HeaderStyle CssClass="hide"></HeaderStyle>
                        <ItemStyle CssClass="hide"></ItemStyle>
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
        </div>
        <div id="stnList" class="avoidscroll">
            <asp:GridView ID="gvStations" runat="server" ClientIDMode="Static" 
                AutoGenerateColumns="False" OnRowDataBound="gvStations_RowDataBound" OnRowCommand="gvStations_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Station Name" ItemStyle-Width="200px"
                        SortExpression="Name" />
                    <asp:BoundField DataField="DistanceKm" HeaderText="Actual Distance" ItemStyle-CssClass="hide"
                        HeaderStyle-CssClass="hide" />
                    <asp:BoundField DataField="Altitude" HeaderText="Elevation" ItemStyle-CssClass="hide"
                        HeaderStyle-CssClass="hide">
                        <HeaderStyle CssClass="hide"></HeaderStyle>
                        <ItemStyle CssClass="hide"></ItemStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="Longitude" HeaderText="Longitude" ItemStyle-CssClass="hide"
                        HeaderStyle-CssClass="hide">
                        <HeaderStyle CssClass="hide"></HeaderStyle>
                        <ItemStyle CssClass="hide"></ItemStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="Latitude" HeaderText="Latitude" ItemStyle-CssClass="hide"
                        HeaderStyle-CssClass="hide">
                        <HeaderStyle CssClass="hide"></HeaderStyle>
                        <ItemStyle CssClass="hide"></ItemStyle>
                    </asp:BoundField>
                    <asp:BoundField HeaderText="Distance" ItemStyle-Width="550px" SortExpression="DistanceKm" />
                    <asp:ButtonField CommandName="Select" Text="Select" Visible="false" />
                    <asp:BoundField DataField="DataPointID" HeaderText="Station ID" ItemStyle-CssClass="hide"
                        HeaderStyle-CssClass="hide" />
                    <asp:BoundField DataField="BearingDegrees" HeaderText="BearingDegrees" ItemStyle-CssClass="hide"
                        HeaderStyle-CssClass="hide" />
                    <asp:BoundField DataField="TimezoneOffset" HeaderText="TimezoneOffset" ItemStyle-CssClass="hide"
                        HeaderStyle-CssClass="hide" />
                    <asp:BoundField DataField="DstOn" HeaderText="DstOn" ItemStyle-CssClass="hide" HeaderStyle-CssClass="hide" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div id="mapImage" class="hide" runat="server">
        Map View under construction.
    </div>
    <div id="hdnStation">
        <asp:HiddenField ID="hdnLatLng" runat="server" OnValueChanged="hdnLatLng_ValueChanged" />
        <asp:HiddenField ID="hdnGridStatus" runat="server" />
        <asp:HiddenField ID="hdnLocSelected" runat="server" />
    </div>
</div>
