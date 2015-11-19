<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AgriInfo.ascx.cs" Inherits="Syngenta.AgriCast.AgriInfo.AgriInfo" %>
<!-- 3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start -->
<script type="text/javascript">
    var updateTotalFieldCapacity = function () {
        var soilTypeFactors = {
            'Sd': 0.7,
            'Ls': 1.1,
            'Sl': 1.4,
            'Cy': 1.8,
            'Lm': 1.5,
            'So': 1.55
        };
        var $topSoilType = $('#ddlTopSoilType');
        var $subSoilType = $('#ddlSubSoilType');
        var $topSoilDepth = $('#txtTopSoilDepth');
        var $subSoilDepth = $('#txtSubSoilDepth');
        var $totolCapacity = $('#txtTotalFieldCapacity');
        var result = Math.round(soilTypeFactors[$topSoilType.val()] * (parseInt($topSoilDepth.val() || 0)))
                    + Math.round(soilTypeFactors[$subSoilType.val()] * (parseInt($subSoilDepth.val() || 0)));
        $totolCapacity.val(result);
    }
    updateTotalFieldCapacity();
</script>
<!-- 3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End -->
<div id="WeatherMenu" class="treeContainer">
    <div id="divMenu" class="divShow" runat="server">
<!--Modify for HU - 20140606 - Start-->
        <!--<div>-->
        <div style="float: left; width: 100%;">
<!--Modify for HU - 20140606 - End-->
            <div style="float: left; width: 50%;">
                <asp:Button CssClass="button" ID="hApply" runat="server" Text="Apply To Chart"
                    ClientIDMode="Static" style="cursor:pointer" OnClick="hApply_Click"></asp:Button></div>
            <div style="float: right; width: 50%;">
                <asp:Button id="hCancel" CssClass="button" runat="server" style="cursor:pointer" Text="Clear Selection" ClientIDMode="Static"></asp:Button>
            </div>
        </div>
        <div>
            <asp:PlaceHolder ID="phTree" runat="server"></asp:PlaceHolder>
            <div class="tree" id="dp">
                <label id="lblDateScale" runat="server" class="labelTreeH1">
                    Date Range</label></div>
            <div class="tree">
                <label id="lblStartDate" runat="server" class="labelPlain">
                    StartDate</label>
                <div>
                    <input type="text" id="txtstartDate" runat="server" class="textBoxTree" />
                </div>
            </div>
            <div class="tree">
                <label id="lblDuration" runat="server" class="labelPlain">
                    Duration</label>
                <asp:DropDownList ID="ddlDuration" runat="server" class="dropDownTree">
                </asp:DropDownList>
            </div>
            <div class="hide tree" id="divEnd" runat="server">
                <label id="lblEndDate" runat="server" class="labelPlain">
                    End Date</label>
                <input type="text" id="txtEndDate" runat="server" class="textBoxTree" />
            </div>
            <div class="tree">
                <label id="lblAggregate" runat="server" class="labelPlain" style="padding-left:0px;font-weight:bold;">
                    Aggregation</label>
                <asp:DropDownList ID="ddlAggregate" runat="server" class="dropDownTree" style="margin-left:4px;">
                </asp:DropDownList>
            </div>
            <div class="DivAltitude">
                <label id="lblAltitude" runat="server" class="labelPlain" style="padding-left:0px;font-weight:bold;">
                    Altitude</label>
                <input type="text" id="txtAltitude" runat="server" Title="Values adjusted to this altitude" class="textBoxTree" style="margin-left:4px;"/>
                <label id="lblMasl" runat="server">
                </label>
            </div>
        </div>
        <div id="divCropInfo" runat="server">
            <div class="tree">
                <img id="imgSeriesCrop" runat="server" src="~/Images/boxplus.gif" class="buttonTree"
                    alt="+" />
                <label id="lblCropInformation" runat="server" class="labelTreeH1">
                    Crop Information</label>
            </div>
            <div id="divCrop" runat="server" class="hide">
                <div class="tree" id="d1">
                    <label id="lblPlantingdate" runat="server" class="labelPlain">
                        Planting Date</label>
                    <input type="text" id="txtPlantingDate" runat="server" class="textBoxTree" />
                </div>
            </div>
        </div>
        <!-- 3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start -->
        <div id="divSoilInfo" runat="server">
            <div class="tree">
                <img id="imgSeriesSoil" runat="server" src="~/Images/boxplus.gif" class="buttonTree"
                    alt="+" />
                <label id="lblSoilInformation" runat="server" class="labelTreeH1">
                    Soil Information</label>
            </div>
            <div id="divSoil" runat="server" class="hide">
                <div class="tree">
                    <label id="lblTopSoilDepth" runat="server" class="labelPlain">
                        Top Soil Depth</label>
                    <input type="text" id="txtTopSoilDepth" runat="server" class="textBoxTree" onkeyup="updateTotalFieldCapacity();" />cm
                </div>
                <div class="tree">
                    <label id="lblTopSoilType" runat="server" class="labelPlain">
                        Top Soil Type</label>
                    <asp:DropDownList ID="ddlTopSoilType" runat="server" class="dropDownTree" onChange="updateTotalFieldCapacity();">
                    </asp:DropDownList>
                </div>
                <div class="tree">
                    <label id="lblSubSoilDepth" runat="server" class="labelPlain">
                        Sub Soil Depth</label>
                    <input type="text" id="txtSubSoilDepth" runat="server" class="textBoxTree" onkeyup="updateTotalFieldCapacity();" />cm
                </div>
                <div class="tree">
                    <label id="lblSubSoilType" runat="server" class="labelPlain">
                        Sub Soil Type</label>
                    <asp:DropDownList ID="ddlSubSoilType" runat="server" class="dropDownTree" onChange="updateTotalFieldCapacity();">
                    </asp:DropDownList>
                </div>
                <div class="tree">
                    <label id="lblTotalFieldCapacity" runat="server" class="labelPlain">
                        Total Field Capacity</label>
                    <input type="text" id="txtTotalFieldCapacity" runat="server" class="textBoxTree"
                        disabled="disabled" />mm
                </div>
            </div>
        </div>
        <!-- 3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End -->
        <div>
            <div id="divweather" runat="server" class="tree">
                <img id="imgSeriesExpand" runat="server" src="~/Images/boxminus.gif" class="buttonTree"
                    alt="+" />
                <label id="lblSeries" runat="server" class="labelTreeH1">
                    Weather</label>
                <div id="divSeries" runat="server" class="tree">
                    <asp:PlaceHolder ID="ph0" runat="server" ClientIDMode="Static"></asp:PlaceHolder>
                    <div id="Div0" class="tree" runat="server">
                        <ul id="UL0" runat="server">
                        </ul>
                    </div>
                    <asp:PlaceHolder ID="ph1" runat="server" ClientIDMode="Static"></asp:PlaceHolder>
                    <div id="Div1" class="tree" runat="server">
                        <ul id="UL1" runat="server">
                        </ul>
                    </div>
                    <asp:PlaceHolder ID="ph2" runat="server" ClientIDMode="Static"></asp:PlaceHolder>
                    <div id="Div2" class="tree" runat="server">
                        <ul id="UL2" runat="server">
                        </ul>
                    </div>
                    <asp:PlaceHolder ID="ph3" runat="server" ClientIDMode="Static"></asp:PlaceHolder>
                    <div id="Div3" class="tree; hide" runat="server">
                        <ul id="UL3" runat="server">
                        </ul>
                    </div>
                    <asp:PlaceHolder ID="ph4" runat="server" ClientIDMode="Static"></asp:PlaceHolder>
                    <div id="Div4" class="tree; hide" runat="server">
                        <ul id="UL4" runat="server">
                        </ul>
                    </div>
                    <asp:PlaceHolder ID="ph5" runat="server" ClientIDMode="Static"></asp:PlaceHolder>
                    <div id="Div5" class="tree; hide" runat="server">
                        <ul id="UL5" runat="server">
                        </ul>
                    </div>
                    <asp:PlaceHolder ID="ph6" runat="server" ClientIDMode="Static"></asp:PlaceHolder>
                    <div id="Div6" class="tree; hide" runat="server">
                        <ul id="UL6" runat="server">
                        </ul>
                    </div>
                    <asp:PlaceHolder ID="ph7" runat="server" ClientIDMode="Static"></asp:PlaceHolder>
                    <div id="Div7" class="tree; hide" runat="server">
                        <ul id="UL7" runat="server">
                        </ul>
                    </div>
                </div>
            </div>
            <div id="divGDD" runat="server" class="tree">
                <img id="imgSeriesGDDExpand" runat="server" src="~/Images/boxminus.gif" class="buttonTree"
                    alt="+" />
                <label id="lblGdd" runat="server" class="labelTreeH1">
                    GDD</label>
                <asp:PlaceHolder ID="phGdd" runat="server" ClientIDMode="Static"></asp:PlaceHolder>
                <div id="divGddSeries" runat="server" class="tree">
                    <ul id="GddUl" runat="server">
                    </ul>
                </div>
            </div>
        </div>
    </div>
</div>
<div id="divAgICharts">
    <asp:HiddenField ID="hdnSeries" runat="server" />
    <asp:HiddenField ID="hdnExpandCollapse" runat="server" />
    <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
</div>
<div id="divPopup" class="hide">
    <iframe src="" id="iframePop" style="width: 100%; height: 100%"></iframe>
</div>

<%--IM01258137 - New Agricast - Translation - can't translate "{.More}" - BEGIN --%>
<%--this Span tag will hve translation for Calender tool tip which is used in common.js--%>
<span id="spnClickCalenderToolTip" runat="server" style="display:none"  />
<%--IM01258137 - New Agricast - Translation - can't translate "{.More}" - END --%>
