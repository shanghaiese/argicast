<%@ Page Title="" Language="C#" MasterPageFile="~/Mobile/MobileMaster.master" AutoEventWireup="true"
    CodeFile="ChartView.aspx.cs" Inherits="Mobile_ChartView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">    
     
    <br />
    <div style="font-weight: bold; text-align: center;">
        <asp:Label ID="lblLocation" runat="server"></asp:Label>
    </div>
    <br />
    <div id="chartView" style="text-align: center;">
    <img id="chartImg" src="" alt="chart" style="border: solid 1px black;" runat="server" />
    </div>
</asp:Content>
   