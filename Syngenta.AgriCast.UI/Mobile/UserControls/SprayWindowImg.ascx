<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SprayWindowImg.ascx.cs" Inherits="Mobile_UserControls_SprayWindowImg" %>

<ul data-role="listview">
        <asp:Repeater ID="Repeater1" runat="server" EnableViewState="false">
            <ItemTemplate>
                <li data-role="divider" data-theme="b"><strong>
                    <%#Eval("date")%></strong></li>
                <li>
                    <div class="ui-grid-a" style="font-size: medium;text-align:center;width:100%;">
                         <img src="<%# Eval("imageurl") %>" alt="No Image" />                       
                    </div>                    
               </li>
            </ItemTemplate>
        </asp:Repeater>
         <li data-role="divider" data-theme="b" id="liSprayLegend" runat="server"><strong>Spray Legend</strong>
         </li>
         <li style="text-align:center;">
           <asp:PlaceHolder ID="CentrePlaceHolder" runat="server"></asp:PlaceHolder>
         </li>
    </ul> 
  

      