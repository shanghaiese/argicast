<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LocationSearch.ascx.cs" Inherits="LocationSearch" %>

<div class="search">
<div>
<label id="searchIn" runat="server">Search in</label>
<asp:DropDownList ID="ddlCountry" runat="server"></asp:DropDownList> 
<label id="for" runat="server">for</label>
<input id="Search1" type="search" runat="server" />
</div>
<div>
<label id="lblPointText" runat="server">Forecast for</label><label id="lblStnName" runat="server">$StnName</label>
<label id="lblDistText" runat="server">$Distance</label>
<asp:HyperLink ID="hNearbyPoint" Text="NearByStations" runat="server" class="stations"></asp:HyperLink>
</div>
</div>
