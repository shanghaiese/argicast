<%@ Page Language="C#"  AutoEventWireup="true" CodeBehind="Popup.aspx.cs" Inherits="Syngenta.AgriCast.AgriInfo.Popup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
   
    <script src="../../Scripts/jquery-1.7.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui.min.js"  type="text/javascript"></script>
        <script type="text/javascript">

            function closePopup() {
                parent.$("#divPopup", window.parent.document).dialog('close');
                return false;
            }
             
</script>
    <style>
        
.label140 {
	width: 140px; 
	margin-top:5px; 
	font-weight: bold;
	font-size:14px;
	padding-left: 45px; 
	padding-right:30px;
	padding-top: 5px;
	float: left;
	font-size: 68%;
    font-family:Arial, Helvetica, Verdana, sans-serif;
     color: #666;	 
}
.label50 { 
	width:50px; 
	margin-top:5px; 
	font-weight: bold;
	font-size:14px;
	padding-left: 5px; 
	padding-right:5px;
	padding-top: 5px;
	float: left;
	font-size: 68%;
    font-family:Arial, Helvetica, Verdana, sans-serif;
     color: #666;	 	 
}
.dropDown {
	width: 110px;
	float: left;
	margin-right: 10px;
	margin-top:5px; 
	font-size: 68%;
    font-family:Arial, Helvetica, Verdana, sans-serif;
     color: #666;	
	 
}
.textBoxTree {	 
	width: 100px;	 
	float: left;
	padding-right: 0px;
	margin-top: 5px;
	margin-right: 0px;
	font-size: 68%;
    font-family:Arial, Helvetica, Verdana, sans-serif;
     color: #666;	
}
.checkBox
{
  padding-right: 90px; 
  float:left;
  margin-top:5px;  
}
.button
{
    margin-left: 90px; 
    width: 150px;
    height: 21px;
    margin-top:25px; 
    text-align:center;
    color: #fff !important; 
	font-family: Helvetica !important;
	font-size: 10px !important;
	font-weight: bold !important;
	vertical-align: middle !important;
	border: 0px !important;
	background-image: url("../../images/button.gif") !important;
	background-repeat: repeat-x !important;
	background-color: transparent !important;
}
.hide {
	display: none;
}
</style>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div>
            <label id="lblAggregationFunction" runat="server" class="label140">
                Aggregation Function:</label>
            <asp:DropDownList ID="ddlAggregationFunction" runat="server" CssClass="dropDown">
            </asp:DropDownList>
        </div>
        <div>
            <label id="lblAccumulate" runat="server" class="label140">
                Accumulate:</label>
             <asp:CheckBox ID="cbAccumulate" runat="server" class="checkBox"/>
        </div>
        <div>
            <label id="lblYearCompare" runat="server" class="label140">
                Compare with Year:</label>              
            <asp:DropDownList ID="ddlYear" runat="server" CssClass="dropDown">
            </asp:DropDownList>
        </div>
        <div>
            <label id="lblTrends" runat="server" class="label140">
                Add Trends:</label>
          <asp:DropDownList ID="ddlTrends" runat="server" CssClass="dropDown"> </asp:DropDownList>     
        </div>
     
        <div>
            <label id="lblAltitude" runat="server" class="label140">
                Adjust Altitude to (masl):</label>
                 <asp:CheckBox ID="cbAltitude" runat="server" class="checkBox"/>
       
        </div>
        <div>
            <label id="lblMethod" runat="server" class="label140">
                Method</label>
          <asp:DropDownList ID="ddlMethod" runat="server" CssClass="dropDown"> </asp:DropDownList>     
        </div>
         <div>
            <label id="lblCap" runat="server" class="label140">
                Cap</label>
          <asp:TextBox ID="txtCap" runat="server" CssClass="textBoxTree"> </asp:TextBox>     
        </div>
         <div>
            <label id="lblBase" runat="server" class="label140">
                Base</label>
          <asp:TextBox ID="txtBase" runat="server" CssClass="textBoxTree"> </asp:TextBox>     
        </div>
    </div>
    <div>
    <asp:Button ID="btnsave" runat="server" Text="Save"  ClientIDMode="Static" 
            class="button" onclick="btnsave_Click" CssClass="button"/>
            
    </div>
    </form>
  
    
   
</body>
</html>
