<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Toolbar.ascx.cs" Inherits="Syngenta.AgriCast.Common.Toolbar" %>
<script type="text/javascript">
<!--

    /*IM01246233 :- New Agricast - missing translation tags - Begin */
    function ValidateEMail(errorMsg) {

        var obj = document.getElementById("Tomail");
        if (obj != null) {
            if (obj.value == "") {
                //alert("Please enter an E-Mail ID");
                alert(errorMsg);
                return false;
            }
        }
        return true;
    }

    function ValidateFavName(ErrorMsg) {
        if ($('#txtFavName').val().trim() == "") {
            alert(ErrorMsg);
            return false;
        }
    }
    /*IM01246233 :- New Agricast - missing translation tags - End */
    /* IM01294335 - New Agricast - rating not sent - Jerrey - Begin */
    $(function () {
        var $hdnFeedbackRating = $('input[type=hidden][id$="hdnFeedbackRating"]');

        $('#range').change(function () {

            //$hdnFeedbackRating.val($(this).val());
            $hdnFeedbackRating.val($(this).val() + " - " + $("select[id=range] option:selected").text());
        });
    });
    /* IM01294335 - New Agricast - rating not sent - Jerrey - End */
// -->
</script>
<div id="ToolBar" style="padding-top: 0px; width: 100%">
    <div id="greeting" class="split">
        <label id="Welcome" runat="server">
            Welcome</label>
        <a id="userName" runat="server" href="#"></a>
    </div>
    <div id="tools" class="split">
        <asp:DropDownList ID="ddlCulture" runat="server" CssClass="dropDown" AutoPostBack="true"
            OnSelectedIndexChanged="ddlCulture_SelectedIndexChanged" Style="width: 160pt" />
        <asp:DropDownList ID="ddlUnits" runat="server" CssClass="dropDown" OnSelectedIndexChanged="ddlUnits_SelectedIndexChanged">
        </asp:DropDownList>
        <div id="divCustom" class="hide" runat="server">
            <div class="box">
                <div id="Wind" class="hide" runat="server">
                    <label id="lblWind" class="labelCustom" runat="server">
                        Wind</label>
                    <asp:DropDownList ID="ddlWind" class="DropdownCustom" runat="server">
                    </asp:DropDownList>
                </div>
                <div id="Rain" class="hide" runat="server">
                    <label id="lblRain" class="labelCustom" runat="server">
                        Rain</label>
                    <asp:DropDownList ID="ddlRain" class="DropdownCustom" runat="server">
                    </asp:DropDownList>
                </div>
                <div id="Temp" class="hide" runat="server">
                    <label id="lblTemp" class="labelCustom" runat="server">
                        Temperature</label>
                    <asp:DropDownList ID="ddlTemp" class="DropdownCustom" runat="server">
                    </asp:DropDownList>
                </div>
                <div>
                    <asp:Button ID="btnOK" runat="server" OnClick="btnOK_Click" Text="ok" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
                </div>
            </div>
        </div>
        <asp:ImageButton ID="imgPrint" runat="server" ImageUrl="~/Images/print.gif" AlternateText="Print Page"
            ToolTip="" ClientIDMode="Static" CausesValidation="False" OnClick="imgPrint_Click" />
        <img id="imgEmail" src="~/Images/email.gif" alt="email" data-controls="email" data-activedescendant="favorites;feedback"
            runat="server" />
        <asp:ImageButton ID="btnExcel" runat="server" ImageUrl="~/Images/excel.png" AlternateText="Excel"
            ToolTip="Export To Excel" OnClick="btnExcel_Click" CausesValidation="False" />
        <img id="imgFav" src="~/Images/FilledStar.png" alt="Favorites" data-controls="favorites"
            data-activedescendant="email;feedback" runat="server" />
        <img id="imgFB" src="~/Images/fback.png" alt="FeedBack" runat="server" data-controls="feedback"
            data-activedescendant="favorites;email" />
    </div>
    <div id="email" class="hide" runat="server">
        <table id="tblEmail" runat="server" style="width: 100%">
            <tr>
                <td class="NewtdLeft">
                </td>
                <td class="Newtd">
                </td>
                <td>
                    <div id="divClose" class="close" data-controls="email">
                        X
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <label id="Fee_EmailAddress" runat="server" class="label150" style="white-space: ">
                        To Address :
                    </label>
                </td>
                <td>
                    <%--/* UAT Tracker 519	Email - translation of the default values of Email fields */  placeholder="Enter the email addresses ; seperated"--%>
                    <input id="Tomail" type="text" class="textBox" runat="server" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <label id="tb_subject" runat="server" class="label150">
                        Subject :</label>
                </td>
                <td>
                    <%--/* UAT Tracker 519	Email - translation of the default values of Email fields */  placeholder="Enter the email subject."--%>
                    <input id="Subject" type="text" class="textBox" runat="server" data-controls="subject" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <label id="mailbody" runat="server" class="label150">
                        Mail Body :</label>
                </td>
                <td>
                    <%--/* UAT Tracker 519	Email - translation of the default values of Email fields */ placeholder="Enter the email subject."--%>
                    <textarea id="txtBody" rows="4" cols="50" runat="server" class="comments" onclick="return body_onclick()" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="EMailPage" Text="Email this Page" runat="server" ClientIDMode="Static"
                        class="button" OnClick="EMailPage_Click" />
                    <%--OnClientClick="Javascript:return ValidateEMail();"--%>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
        </table>
        <asp:Literal ID="txtHintsMessageOfEmail" runat="server"></asp:Literal>
    </div>
    <div id="favorites" class="hide" runat="server">
        <table id="tblFavorites" runat="server" style="width: 100%">
            <tr>
                <td>
                </td>
                <td class="Newtd">
                </td>
                <td>
                    <div id="FavClose" class="close" data-controls="favorites">
                        X</div>
                </td>
            </tr>
            <tr>
                <td>
                    <%--IM01322917 - New Agricast - favorites table to be 100% wide - begin--%>
                    <div>
                        <span>
                            <label id="lblFavName" runat="server">
                                Name
                            </label>
                        </span><span>
                            <asp:TextBox ID="txtFavName" runat="server"></asp:TextBox>
                            <asp:Button ID="Fav_AddToFavorites" runat="server" Text="Add to Favorites" class="button"
                                OnClick="Fav_AddToFavorites_click" />
                        </span>
                    </div>
                    <%--IM01322917 - New Agricast - favorites table to be 100% wide - end--%>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="gvFavorites" runat="server" AutoGenerateColumns="False" OnRowEditing="gvFavorites_RowEditing"
                        OnRowCancelingEdit="gvFavorites_RowCancelingEdit" OnRowDeleting="gvFavorites_RowDeleting"
                        OnRowUpdating="gvFavorites_RowUpdating" OnRowDataBound="gv_Favorites_RowDataBound"
                        DataKeyNames="Key" OnRowCommand="gvFavorites_RowCommand" ClientIDMode="Static"
                        EnableViewState="false" style="width:100%; padding-left:3px;">
                        <Columns>
                            <asp:BoundField DataField="FavoriteName" HeaderText="Favorite Name" SortExpression="FavoriteName"
                                ItemStyle-Width="200px" />
                            <asp:BoundField DataField="ModuleName" HeaderText="Service" ReadOnly="True" ItemStyle-Width="150px"
                                SortExpression="ModuleName">
                                <ItemStyle Width="150px"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="PlaceName" HeaderText="Location Name" ReadOnly="True"
                                ItemStyle-Width="300px" SortExpression="PlaceName">
                                <ItemStyle Width="300px"></ItemStyle>
                            </asp:BoundField>
                            <asp:CommandField  ItemStyle-Width="280px" ButtonType="Image" CancelImageUrl="~/Images/button_smallicon_cancel.gif"
                                DeleteImageUrl="~/Images/delete.gif" EditImageUrl="~/Images/edit.gif" HeaderText="Edit / Delete"
                                ShowDeleteButton="True" ShowEditButton="True" UpdateImageUrl="~/Images/button_smallicon_save.gif" />
                            <asp:BoundField DataField="Key" HeaderText="Favorite ID" />
                            <asp:ButtonField CommandName="Select" Text="Select" Visible="false" />
                            <asp:BoundField DataField="PlaceID" HeaderText="PlaceID" HeaderStyle-CssClass="hide"
                                ItemStyle-CssClass="hide">
                                <HeaderStyle CssClass="hide"></HeaderStyle>
                                <ItemStyle CssClass="hide"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="PlaceName" HeaderText="PlaceName" HeaderStyle-CssClass="hide"
                                ItemStyle-CssClass="hide">
                                <HeaderStyle CssClass="hide"></HeaderStyle>
                                <ItemStyle CssClass="hide"></ItemStyle>
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
        </table>
    </div>
    <div id="feedback" class="hide" runat="server">
        <table id="tblFeedback" runat="server" style="width: 100%">
            <%--/*IM01246263 - New Agricast - Add a new translation tag  - BEGIN*/--%>
            <tr>
                <td class="NewtdLeft" colspan="2">
                    <label id="lblFeedBackHeader" runat="server" class="FeedBackHeader">
                    </label>
                </td>
                <td class="Newtd">
                </td>
                <td style="margin-left: 100px; margin-right: 100px;">
                    <div id="feedbackClose" class="close" data-controls="feedback">
                        X</div>
                </td>
            </tr>
            <%--  /*IM01246263 - New Agricast - Add a new translation tag  - END*/--%>
            <tr>
                <td>
                    <label id="lblName" runat="server" class="label150">
                        Name
                    </label>
                </td>
                <td>
                    <input id="txtName" type="text" class="textBox" runat="server" />
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <label id="lblEmail" runat="server" class="label150">
                        Email
                    </label>
                </td>
                <td>
                    <input id="txtEmail" type="text" class="textBox" runat="server" />
                    <asp:Label ID="lblEmailError" runat="server" ClientIDMode="Static"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <label id="lblMessage" runat="server" class="label150">
                        Comments
                    </label>
                </td>
                <td>
                    <textarea id="TextareaMessage" rows="4" cols="50" runat="server" class="comments" />
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <label id="lblRating" runat="server" class="label150">
                        Rate this site <span class="mandatory">* </span>
                    </label>
                </td>
                <td>
                    <asp:DropDownList ID="range" runat="server"></asp:DropDownList>
                    <%--<select class="test" id="range">
                        <option value="">---</option>
                        <option id="txtOptionInsufficiently" runat="server" value="0">0</option>
                        <option id="txtOptionDeficient" runat="server" value="1">Bad</option>
                        <option id="txtOptionSufficiently" runat="server" value="2">Poor</option>
                        <option id="txtOptionSatisfactory" runat="server" value="3">Fair</option>
                        <option id="txtOptionGood" runat="server" value="4">Good</option>
                        <option id="txtOptionVeryWell" runat="server" value="5">Excellent</option>
                    </select>--%>
                    <div id="rating" data-rateit-backingfld="select#range" style="padding-left: 15px">
                    </div>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:Image ID="Im1" runat="server" ClientIDMode="Static" Style="padding-left: 15px"
                        alt="Captcha Text" ImageUrl="Captcha.aspx"/>
                    <asp:ImageButton ID="btnRefresh" ClientIDMode="Static" runat="server" ImageUrl="~/Images/refresh_icon.png"
                        OnClick="btnRefresh_Click" alt="Refresh" />
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <label id="lblCaptcha" runat="server" class="label150">
                        Enter the text shown in the picture
                    </label>
                </td>
                <td>
                    <asp:TextBox ID="txtCaptcha" runat="server" ClientIDMode="Static" class="textBox"></asp:TextBox>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnSendMsg" runat="server" Text="Send Message" OnClick="btnSendMsg_Click"
                        ClientIDMode="Static" class="button" Style="margin-top: 10px" />
                </td>
                <td>
                    <asp:Label ID="LStatus" runat="server" ClientIDMode="Static"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
        </table>
        <asp:Literal ID="txtHintsMessageOfFeedback" runat="server"></asp:Literal>
    </div>
    <asp:HiddenField ID="hdnFeedbackRating" runat="server" ClientIDMode="Static" OnValueChanged="hdnFeedbackRating_ValueChanged" />
    <asp:HiddenField ID="hdnGridFavoriteStatus" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnGridImages_Status" runat="server" ClientIDMode="Static" />
</div>
