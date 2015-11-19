<%@ Page Title="" Language="C#" MasterPageFile="~/Mobile/MobileMaster.master" AutoEventWireup="true"
    CodeFile="SingleDayForecast.aspx.cs" Inherits="Mobile_SingleDayForecast" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <ul data-role="listview">
    
        <li><a href="#">
            <p>
            </p>
            <placeholder id="phWind" runat="server"></placeholder>
            <div class="ui-grid-c" style="font-size: large;">
                <div class="ui-block-a">
                    <span style="color: Red;"></span><span style="color: Blue;">
                        <placeholder id="phWindDirection" runat="server"></placeholder>
                      
                    </span><span style="color: Gray">
                        <placeholder id="phWindSpeed" runat="server"></placeholder>
                        <small><small>mph</small></small></span><br />
                </div>
                <div class="ui-block-a">
                    <img src="../../Images/rain1.jpg" alt="No Image of Precipitation" />
                    <span style="color: gray;">
                        <placeholder id="phPrecip" runat="server"></placeholder>
                    </span>
                </div>
                <div class="ui-block-c">
                    <span style="color: Red;"><big><big>
                        <placeholder id="phTempMax" runat="server"></placeholder>
                        &deg;</big></big></span> <span style="color: Blue;"><big><big>
                            <placeholder id="phTempMin" runat="server"></placeholder>
                            &deg;</big></big> </span>
                </div>
                <div class="ui-block-c" style="text-align: center">
                    <placeholder id="phImageUrl" runat="server"></placeholder>
                </div>
            </div>
            <asp:Literal ID="litSprayTab" Text="" runat="server" />
        </a></li>
    </ul>
</asp:Content>
