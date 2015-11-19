<%@ Page Title="" Language="C#" MasterPageFile="~/Mobile/MobileMaster.master" AutoEventWireup="true" CodeFile="MobileForecast.aspx.cs" Inherits="Mobile_MobileForecast" %>
<%@ Register TagPrefix="uc" TagName="DayForecast" Src="~/Mobile/UserControls/DayForeCast.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<form id="form1" action="MobileForecast.aspx" runat="server" data-ajax="false">
   <uc:DayForecast ID="ucForeCast" runat="server" />
   </form>
</asp:Content>

  