<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChartZoomedImage.aspx.cs"
    Inherits="ChartZoomedImage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Show Big Chart</title>
    <style type="text/css">
.iconDivClass
{
	position: relative;	
    top:70px;
}
        .iconHeaderTableClass
        {
            margin-left: 53px;
            *margin-left: 0px; /* for IE 7 */
            width: 916px;
        }
   
.float-icon-legend {
    color: #333;
    margin-bottom: -382px;
    margin-left: 16px;
	overflow: hidden;
    position: relative;
    top: -382px;
    width: 954px;
}
.float-icon-legend table tr:hover {
	background: none;
}
.float-icon-legend table tr td {
    text-align: center;
}

.float-icon-legend .chart-float-icons {
    float: left;
    font-size:10px;
    margin-left: 37px; 
    *margin-left: 7px; /* for IE 7 */
    /*margin-top: 232px; */
    margin-top: 225px; /* for published */
    line-height:1em;
}
.float-icon-legend .chart-float-icons table.incomplete-data {
	position: relative;
	top: -13px;
}

.float-icon-legend .chart-float-legend {
    display: none;
}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align: center; width: 1032px;">
        <asp:PlaceHolder ID="weatherIcons" runat="server"></asp:PlaceHolder>
        <asp:Image ID="imgBigger" runat="server" />
        <asp:PlaceHolder ID="windIcons" runat="server"></asp:PlaceHolder>
    </div>
    </form>
</body>
</html>
