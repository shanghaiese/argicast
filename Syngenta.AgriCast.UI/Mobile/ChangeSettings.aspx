<%@ Page Title="" Language="C#" MasterPageFile="~/Mobile/MobileMaster.master" AutoEventWireup="true"
    CodeFile="ChangeSettings.aspx.cs" Inherits="Mobile_ChangeSettings" %>


 <asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">      
 </asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <form id="ChangeSettings" action="ChangeSettings.aspx" runat="server" data-ajax="false">
    
    <div class="ui-grid-a">
    <div class="ui-block-a" style="width: 20%;padding-top:1em;">
        <label id="lblUnit" for="ddlUnits" class="select" runat="server"> Units</label>
       </div>
       <br />
        <div class="ui-block-b" style="width: 80%">
            <asp:DropDownList name="ddlUnits" id="ddlUnits" data-native-menu="false" runat="server">
           </asp:DropDownList></div>
   </div>
      
    <div class="ui-grid-a">
    <div class="ui-block-a" style="width: 20%;padding-top:1em;">
        <label id="lblWind" for="ddlWind" class="select" runat="server">Wind</label></div>
        <br />
        <div class="ui-block-b" style="width: 80%">
            <asp:DropDownList name="ddlWind" id="ddlWind"  data-native-menu="false" runat="server" >           
            </asp:DropDownList>
            </div>
       </div>
          <div class="ui-grid-a">
          <div class="ui-block-a" style="width: 20%;padding-top:1em;">
              <label id="lblCulture" for="ddlCulture" class="select" runat="server">Language</label></div>
              <br />
            <div class="ui-block-b" style="width: 80%">
            <asp:DropDownList name="ddlLanguage" id="ddlCulture" data-native-menu="false" runat="server" > 
            </asp:DropDownList>
            </div>
       </div>
     <asp:Button ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click"/>    
    </form>
</asp:Content>
