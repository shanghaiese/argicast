<%--/* Agricast CR - R6 - Add wind icons and legend for Humidity - Begin */ --%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebChartIcons.ascx.cs"
    Inherits="Syngenta.AgriCast.Charting.WebChartIcons" %>
<div id="float-icon-legend" class="float-icon-legend">
    <div class="chart-float-legend">
        <table id="tblwinddirectionlegend">
            <tbody>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="windIconsLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        0
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        15
                    </td>
                    <td style="text-align: left">
                        <asp:Image ID="img15" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        40
                    </td>
                    <td style="text-align: left">
                        <asp:Image ID="img40" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        75
                    </td>
                    <td style="text-align: left">
                        <asp:Image ID="img75" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        120
                    </td>
                    <td style="text-align: left">
                        <asp:Image ID="img120" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="weatherLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <table id="tblwinddirectionlegend1">
                            <tbody>
                                <tr>
                                    <td>
                                    </td>
                                    <td align="center">
                                        <asp:Label ID="northDir" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="middle">
                                        <asp:Label ID="westDir" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Image ID="imgDirection" runat="server" />
                                    </td>
                                    <td valign="middle">
                                        <asp:Label ID="eastDir" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td align="center">
                                        <asp:Label ID="southDir" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                    
                    <table id="tblhumidlegend">
                        <tbody>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <strong>XX%</strong>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="humidityLabel" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div id="iconsContainer" class="chart-float-icons" runat="server">
    </div>
</div>
<%--/* Agricast CR - R6 - Add wind icons and legend for Humidity - End */--%>