<%@ Page Language="C#" MasterPageFile="~/Mobile/MobileMaster.master" AutoEventWireup="true" CodeFile="Disclaimer.aspx.cs" Inherits="Mobile_Disclaimer" ClientIDMode="Static" %>
 

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <form id="form1" runat="server">
    <div>
      <h3><asp:Label ID="lblTitle" runat="server"></asp:Label></h3>
      <br />
      <asp:Label ID="lblDisclaimer" runat="server"></asp:Label>
    </div>
    </form>
</asp:Content> 
