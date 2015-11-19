using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using Syngenta.AgriCast.Common.View;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using System.Web.SessionState;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Security;
using Syngenta.AgriCast.ExceptionLogger;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.IO;
using System.Net.Mime;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;


namespace Syngenta.AgriCast.Common
{
    public partial class Toolbar : System.Web.UI.UserControl, IToolbar
    {
        ToolBarPresenter objPreTool;
        IToolbar objITool;
        ServiceInfo objSvcInfo;
        LocationInfo objLocInfo;
        Controls common = new Controls();
        string favName;
        string toAddress;
        string sub;
        string body;
        int index = 0;
        string sFile = "Page";
        DataSet dsFav = new DataSet();
        DataTable dtFav = new DataTable();
        DataTable dtCulCode;
        ServicePresenter objSvcPre;
        /*IM01246233 :- New Agricast - missing translation tags - Begin */
        CommonUtil objCommonUtil = new CommonUtil();
        /*IM01246233 :- New Agricast - missing translation tags - End */
        string strTranslatedText;
        DataTable dtUnit = new DataTable();
        public event EventHandler IndexChange;
        string CulCode;
        string strRandomtxt;
        int CaptchaLength;
        double FontSize;
        string FontFamily;
        string BackgroundImagePath = "";
        string TextColor;
        string SuccessMessage;

        string CharacterSet;
        string FeedbackName;
        string FeedbackEmail;
        string FeedbackMessage;
        string Rating;
        ServicePresenter objServicePre = new ServicePresenter();
        UserInfo objUserInfo = null;
        public event EventHandler ClickExport;
        public event EventHandler SaveFavorite;
        public event EventHandler EMail_Page;
        public event EventHandler Print_Page;
        public const string CUSTOM = "Custom...";
        public const string SUCCESS_MESSAGE = "Message sent successfully.";
        public const string FILL_FORM = "Please fill the form completely.";
        public const string EMAIL_ERROR = "Please enter a valid email id";
        public const string EMPTY_CAPTCHA = "Please enter the captcha text.";
        string strPlaceID = string.Empty;
        string strPlaceName = string.Empty;
        SmtpClient smtpClientObj;
        string smtpServer = System.Configuration.ConfigurationManager.AppSettings["MailConfig"];
        string strMailBody = string.Empty;
        string strSubject = string.Empty;
        string strAttachmentpath = string.Empty;
        string strTo = string.Empty;
        string strFrom = string.Empty;
        MailMessage mail = null;
        Attachment attachFile = null;
        string sFileName = string.Empty;
        string strPath = string.Empty;
        bool IsCaptchaReset = false;
        bool IsEmail = false;
        bool IsEmailSection = false;
        bool IsPrint = false;
        bool blGenerateHtml = false;
        bool isFeedback = false;
        bool isFavorite = false;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (hdnGridFavoriteStatus.Value != null && hdnGridFavoriteStatus.Value.Contains("ShowFavorite"))
                isFavorite = true;

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                objPreTool = new ToolBarPresenter(this);
                objSvcInfo = (ServiceInfo)Session["serviceInfo"];
                objLocInfo = (LocationInfo)Session["objLocationInfo"];
                objSvcPre = new ServicePresenter();
                if (!IsPostBack)
                {

                    //Set the UserName
                    objUserInfo = (UserInfo)Session["objuserinfo"];
                    if (objUserInfo != null)
                    {
                        /*IM01246266 - New Agricast - can't save a favourite - Begin */
                        //userName.InnerText = objUserInfo.UserName;
                        if (!string.IsNullOrEmpty(objUserInfo.UserName))
                        {
                            if (objUserInfo.UserName.IndexOf('^') > -1)
                            {
                                userName.InnerText = objUserInfo.UserName.Substring(objUserInfo.UserName.IndexOf('^') + 1);
                            }
                        }
                        /*IM01246266 - New Agricast - can't save a favourite - End */
                    }

                    objSvcPre = new ServicePresenter(this);

                    string strLink = ConfigurationManager.AppSettings["link"];
                    userName.HRef = strLink;
                    //objSvcPre.createServiceSession("Demo");

                    objPreTool.loadPageSettings();

                    if (!showFavorites)
                    {
                        imgFav.Visible = false;
                    }
                    else
                    {
                        //getFavorites();
                        /*IM01246233 :- New Agricast - missing translation tags - Begin */
                        //add the clientclick event for validation only when Email is enabled

                        Fav_AddToFavorites.Attributes.Add("OnClick", "Javascript:return ValidateFavName(" + "'" + objCommonUtil.getTransText(Constants.FAV_NAME_EMPTY_CHECK) + "'" + ");");
                        /*IM01246233 :- New Agricast - missing translation tags - End */
                    }

                    if (!showPrint)
                        imgPrint.Visible = false;

                    if (!showEmail)
                        imgEmail.Visible = false;
                    /*IM01246233 :- New Agricast - missing translation tags - Begin */
                    else
                    {
                        //add the clientclick event for validation only when Email is enabled
                        EMailPage.Attributes.Add("OnClick", "Javascript:return ValidateEMail(" + "'" + objCommonUtil.getTransText(Constants.EMAIL_ID_EMPTY_CHECK) + "'" + ");");
                    }
                    /*IM01246233 :- New Agricast - missing translation tags - End */

                    if (!showExportExcel)
                        btnExcel.Visible = false;

                    if (!showFeedback)
                        imgFB.Visible = false;


                    LoadCulture();


                    objPreTool.getCultureCode();
                    changeLabelText(strCulCode);



                }

                //Load units if not postback or in case culture has changed.
                if ((!IsPostBack) || ddlCulture.SelectedValue != objSvcInfo.Culture)
                    LoadUnits();


                LoadCustomSettings();

                getFavorites();
                if (!IsPostBack)
                {
                    LoadCaptcha();
                    //set the first value in Dropdown as default unit and add it o session
                    DefaultUnitSettings = dtUnits.Rows[0][0].ToString();
                    if (objSvcInfo.Unit.Trim() != "")
                    {int count=0;
                       for(int i=0; i< dtUnits.Rows.Count; i++)
                       {
                           if(objSvcInfo.Unit.Trim().ToLower().ToString() == dtUnits.Rows[i][0].ToString().ToLower().Trim())
                           {
                               ddlUnits.SelectedIndex = ddlUnits.Items.IndexOf(ddlUnits.Items.FindByValue(objSvcInfo.Unit.Trim()));
                               count++;
                               break;
                           }
                       }
                        if (count==0)
                            objSvcInfo.Unit = DefaultUnitSettings;    
                    }
                    else
                        objSvcInfo.Unit = DefaultUnitSettings;
                    
                }
                setEmailValues();




            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.TOOLBAR_LOADFAILURE, strCulCode) + ex.Message.ToString();
                return;
            }
        }

        protected void Page_prerender(object sender, EventArgs e)
        {

            if (this.gvFavorites.Rows.Count > 0)
            {
                gvFavorites.UseAccessibleHeader = true;
                gvFavorites.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            if (IsCaptchaReset || isFeedback)
            {
                feedback.Attributes.Add("class", "show");
                IsCaptchaReset = false;
            }
            else
            {
                feedback.Attributes.Add("class", "hide");
            }


            if (hdnGridFavoriteStatus.Value != null && hdnGridFavoriteStatus.Value.Equals("ShowFavorite"))
            {
                favorites.Attributes.Add("class", "show");
                gvFavorites.CssClass = "show";
            }
            else if (hdnGridFavoriteStatus.Value.Equals("HideFavorite"))
            {
                // favorites.Attributes.Remove("class");
                favorites.Attributes.Add("class", "hide");
                gvFavorites.CssClass = "hide";
            }

            if (email != null)
            {

                if (IsEmailSection)
                {

                    email.Attributes.Add("class", "show");

                }
                else
                {
                    email.Attributes.Add("class", "hide");
                }
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {

            if (IsEmail == true)
            {
                //    //HtmlInputControl Tomail = (HtmlInputControl)common.FindControlRecursive(this, "Tomail");
                Tomail.Value = string.Empty;
                //    //HtmlInputControl Subject = (HtmlInputControl)common.FindControlRecursive(this, "Subject");
                Subject.Value = string.Empty;
                //    //HtmlTextArea txtBody = (HtmlTextArea)common.FindControlRecursive(this, "txtBody");
                txtBody.Value = string.Empty;
                //Resetting isEmail back to false
                IsEmail = false;
            }
            base.Render(writer);
        }

        public void Fav_AddToFavorites_click(object sender, EventArgs e)
        {
            objPreTool.getCultureCode();



            if (!string.IsNullOrEmpty(Favorite))
            {

                //Call the Presenter method
                objPreTool.AddToFavorites();

                //Trigger the save event only if Fav Name is unique 
                if (IsFavAdded)
                {

                    //Trigger the Event to Add the Favorite Dictionary and save in database
                    onSaveFavorite(e);
                    //HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FAVORITE_ADD_SUCCESS, strCulCode); ;
                    HttpContext.Current.Session["SuccessMessage"] = objPreTool.getTranslatedText(Constants.FAVORITE_ADD_SUCCESS, strCulCode);
                }
                //ReLoad GetFavorites
                getFavorites();

            }
            else
            {
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FAVORITE_ADD_FAILURE, strCulCode);
            }

            //Clear the Input TextBox once the Favorite is added
            Favorite = string.Empty;
        }

        public void btnExcel_Click(object sender, ImageClickEventArgs e)
        {

            try
            {
                //Response.Clear();
                //Response.ContentType = "application/ms-excel";
                //Response.Charset = "";
                //Page.EnableViewState = false;
                //Response.AddHeader("Content-Disposition", "inline;filename=report.xls");

                //System.IO.StringWriter sw = new System.IO.StringWriter();
                //System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw);

                //gvFavorites.RenderControl(hw);

                //Response.Write(sw.ToString());

                //Response.End();
                onClickExport(e);
                // objPreTool.SaveExcelData();
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.EXCEL_FAILURE, strCulCode) + ex.Message.ToString();
            }

        }


        protected void onClickExport(EventArgs e)
        {
            if (ClickExport != null)
            {
                ClickExport(this, e);
            }
        }

        //Method to Handle Favorites modifcation
        protected void onSaveFavorite(EventArgs e)
        {
            if (SaveFavorite != null)
            {
                SaveFavorite(this, e);
            }
        }

        protected void EMailPage_Click(object sender, EventArgs e)
        {
            /*IM01246233 :- New Agricast - missing translation tags - Begin */
            //object already created on page load
            //objCommonUtil = new CommonUtil();
            /*IM01246233 :- New Agricast - missing translation tags - Begin */
            //Validate the Email format
            string pattern = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|" +
               @"0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z]" +
               @"[a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";
            Match match = Regex.Match(Tomail.Value.Trim(), pattern, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.EMAIL_INVALIDADDRESS);
                IsEmailSection = true;//show the email section
            }
            else
            {
                Attachment attachFile = null;
                //Bubble the Event to create Pdf in Default Page
                onEMailPage(e);

                blGenerateHtml = true;//Generate the HTML for E Mail
                IsEmail = true;// Generate PDF



                ////Set the Path where the File i stored
                //strAttachmentpath = "";

                //strPath = ConfigurationManager.AppSettings["tempfolder"] + Constants.EMAILPDFFOLDER;

                ////Pick the Stored PDF and send an email
                //try
                //{

                //    objPreTool = new ToolBarPresenter(this);


                //    //From Field - Array List Property
                objPreTool.getDefaultEmailSettings();

                //Add values to session
                Session["EMailTo"] = Tomail.Value.Trim();
                Session["EMailFrom"] = alEmailSettings[1].ToString();
                Session["EMailSubject"] = Subject.Value.Trim();
                Session["EMailBody"] = txtBody.Value;
            }
            //    strFrom = alEmailSettings[1].ToString();

            //    //To Field
            //    strTo = Tomail.Value.Trim();

            //    //Subject Field
            //    strSubject = Subject.Value.Trim();

            //    //Mail Body
            //    strMailBody = txtBody.Value; ;


            //    //Fetch the Filename from the Session
            //    sFileName = Session["EMailFileName"].ToString();

            //    //Stream pdf = (Stream)Session["streamPDF"];

            //    //Important to reset back to the begining of the stream!!!
            //    // pdf.Position = 0; 



            //    //Form the Stored File Path Location
            //    strAttachmentpath = HttpRuntime.AppDomainAppPath + strPath + "\\" + sFileName;

            //    mail = new MailMessage(strFrom.ToString(), strTo.ToString());
            //    if (strAttachmentpath != "" && strAttachmentpath != null)
            //    {
            //        attachFile = new Attachment(strAttachmentpath);

            //        mail.Attachments.Add(attachFile);

            //        //Send the PDF inline
            //        //attachFile.ContentDisposition.Inline = true;
            //        attachFile.ContentDisposition.DispositionType = DispositionTypeNames.Attachment;
            //        //attachFile.ContentId = "PDF"; 
            //        attachFile.ContentType.MediaType = "application/pdf";


            //    }


            //    //if (pdf !=null)
            //    //{
            //    //    attachFile = new Attachment(pdf, "MyPDF.pdf", "application/pdf");

            //    //    mail.Attachments.Add(attachFile);
            //    //}
            //    StringBuilder sb = new StringBuilder(strMailBody);
            //    string filename = "Test.html";
            //    string tempFolderPath = ConfigurationManager.AppSettings["tempfolder"];
            //    string rootPath = HttpRuntime.AppDomainAppPath.ToString() + tempFolderPath;

            //    string newPath = System.IO.Path.Combine(rootPath, Constants.EMAILPDFFOLDER);
            //    string strHTMLPATH = newPath + "/" + filename;
            //    StreamReader sr = new StreamReader(strHTMLPATH);
            //    StringWriter sw = new StringWriter();

            //    HtmlTextWriter writer = new HtmlTextWriter(sw);
            //    writer.Write(sr.ReadToEnd());

            //    sb.Append(writer.InnerWriter.ToString());
            //    mail.Subject = strSubject;
            //    mail.Body = sb.ToString();
            //    mail.IsBodyHtml = true;
            //    smtpClientObj = new SmtpClient(smtpServer);
            //    //smtpClientObj.Host = "localhost";
            //    smtpClientObj.Port = 25;
            //    smtpClientObj.Send(mail);

            //}
            //catch (SmtpException smtpex)
            //{
            //    HttpContext.Current.Session["ErrorMessage"] = "The following error has occured on the page : " + smtpex.Message.ToString();
            //}
            //catch (Exception ex)
            //{
            //    HttpContext.Current.Session["ErrorMessage"] = "The following error has occured on the page : " + ex.Message.ToString();
            //}
        }


        protected void onEMailPage(EventArgs e)
        {
            if (EMail_Page != null)
            {
                EMail_Page(this, e);
            }

        }
        protected void onPrintPage(EventArgs e)
        {
            if (Print_Page != null)
            {
                Print_Page(this, e);
            }
        }
        //Method to populate the language dropdwon. Fetches the values from Pub.config file
        protected void LoadCulture()
        {
            try
            {
                objSvcPre = new ServicePresenter(this);
                objSvcPre.loadCulture();
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.CULTURE_LOADFAILURE, strCulCode) + ex.Message.ToString();
            }
        }

        protected void LoadUnits()
        {
            try
            {
                objSvcPre = new ServicePresenter(this);
                objSvcPre.loadUnits();
                // DefaultUnitSettings = dtUnits.Rows[0][0].ToString();
                //objSvcInfo.Unit = DefaultUnitSettings;
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.UNITS_LOADFAILURE, strCulCode) + ex.Message.ToString();
            }

        }

        //Method to fetch the default email setting from the config file
        protected void setEmailValues()
        {
            objPreTool.getDefaultEmailSettings();
            //Values for page email 
            //Tomail.Value = alEmailSettings[1].ToString();
            Subject.Value = alEmailSettings[2].ToString();
            txtBody.Value = alEmailSettings[3].ToString();

            //Vaues for feedback
            txtName.Value = alEmailSettings[0].ToString();
            //txtEmail.Value = alEmailSettings[1].ToString();

        }

        protected void LoadCustomSettings()
        {
            string strCustom = string.Empty;
            try
            {
                objSvcPre = new ServicePresenter(this);
                objSvcPre.LoadCustomSettings();
                //  strCustom = "Wind-m/sec,knots#Rain-inches,cm#Temperature-Fahrenheit,Celsius";
                string[] settings = strCustomUnits.Split('#');

                for (int i = 0; i < settings.Count(); i++)
                {
                    string Parameter = settings[i].Remove(settings[i].IndexOf("-"));
                    string[] ParameterSettings = settings[i].Substring(settings[i].IndexOf("-") + 1).Split(',');

                    switch (Parameter.ToLower())
                    {

                        case "wind": ddlWind.Items.Clear();
                            foreach (string param in ParameterSettings)
                            {

                                ddlWind.Items.Add(new ListItem(objPreTool.getTranslatedText(param, objSvcInfo.Culture), param));
                            }
                            ddlWind.DataBind();
                            Wind.Attributes.Add("class", "show");
                            break;
                        case "rain": foreach (string param in ParameterSettings)
                            {
                                ddlRain.Items.Add(param);

                            }
                            ddlRain.DataBind();
                            Rain.Attributes.Add("class", "show");
                            break;
                        case "temperature": foreach (string param in ParameterSettings)
                            {
                                ddlTemp.Items.Add(param);

                            }
                            ddlTemp.DataBind();
                            Temp.Attributes.Add("class", "show");
                            break;
                        default: break;


                    }

                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.UNITS_LOADFAILURE, strCulCode) + ex.Message.ToString();
            }

        }



        //Method to fetch the favourites from DB for a particualr user
        protected void getFavorites()
        {
            string strUserName = string.Empty;
            try
            {

                objPreTool = new ToolBarPresenter(this);
                objPreTool.getCultureCode();
                objUserInfo = (UserInfo)Session["objUserInfo"];

                if (objUserInfo != null)
                {
                    objPreTool.getFavorites();

                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FAVORITE_GENERIC_FAILURE, strCulCode) + ex.Message.ToString();
            }

        }

        public void changeLabelText(string strCultureCode)
        {
            try
            {
                /*IM01289946-New Agricast - Add a * on mandatory fields-BEGIN*/
                string strSpanMandatory = "<span class=\"mandatory\"> * </span>";
                /*IM01289946-New Agricast - Add a * on mandatory fields-END*/
                objPreTool = new ToolBarPresenter(this);

                /* UAT Tracker 519	Email - translation of the default values of Email fields - Begin*/
                Tomail.Attributes.Add("placeholder", objPreTool.getTranslatedText("Enter the email addresses ; seperated", strCultureCode));
                Subject.Attributes.Add("placeholder", objPreTool.getTranslatedText("Enter the email subject.", strCultureCode));
                txtBody.Attributes.Add("placeholder", objPreTool.getTranslatedText("Enter the email subject.", strCultureCode));
                /* UAT Tracker 519	Email - translation of the default values of Email fields - End*/

                //Set the ToolTip Text of Toolbar Images 
                imgPrint.ToolTip = objPreTool.getTranslatedText(Constants.PRINT_PAGE, strCultureCode);
                imgEmail.Attributes["title"] = objPreTool.getTranslatedText(Constants.EMAIL, strCultureCode);
                btnExcel.ToolTip = objPreTool.getTranslatedText(Constants.EXCEL, strCultureCode);
                imgFav.Attributes["title"] = objPreTool.getTranslatedText(Constants.FAVORITES, strCultureCode);
                imgFB.Attributes["title"] = objPreTool.getTranslatedText(Constants.FEEDBACK, strCultureCode);

                //Set the Label within Custom Div
                lblWind.InnerText = objPreTool.getTranslatedText(Constants.WIND, strCultureCode);
                btnOK.Text = objPreTool.getTranslatedText(Constants.OK, strCultureCode);
                btnCancel.Text = objPreTool.getTranslatedText(Constants.CANCEL, strCultureCode);
                /*IM01289946-New Agricast - Add a * on mandatory fields-BEGIN*/
                //Fee_EmailAddress.InnerText = objPreTool.getTranslatedText(Constants.FEE_TO_EMAIL_ADDRESS, strCultureCode);
                Fee_EmailAddress.InnerHtml = objPreTool.getTranslatedText(Constants.FEE_TO_EMAIL_ADDRESS, strCultureCode) + strSpanMandatory;
                /*IM01289946-New Agricast - Add a * on mandatory fields-END*/
                tb_subject.InnerText = objPreTool.getTranslatedText(Constants.SUBJECT, strCultureCode);
                mailbody.InnerText = objPreTool.getTranslatedText(Constants.MAILBODY, strCultureCode);
                EMailPage.Text = objPreTool.getTranslatedText(Constants.EMAIL_PAGE, strCultureCode);
                Fav_AddToFavorites.Text = objPreTool.getTranslatedText(Constants.FAV_ADD_TO_FAVOURITES, strCultureCode);
                if (gvFavorites.Rows.Count != 0)
                {


                    (gvFavorites.HeaderRow.Cells[0]).Text = objPreTool.getTranslatedText(Constants.NAME, strCultureCode);
                    (gvFavorites.HeaderRow.Cells[1]).Text = objPreTool.getTranslatedText(Constants.SERVICE, strCultureCode);
                    (gvFavorites.HeaderRow.Cells[2]).Text = objPreTool.getTranslatedText(Constants.LOCATION_NAME, strCultureCode);
                    for (int i = 0; i < gvFavorites.Rows.Count; i++)
                    {
                        /* IM01365225 - New Agricast - favorite - Service Name - Jerrey - Begin */
                        // Commented by Jerrey - IM01365225
                        //gvFavorites.Rows[i].Cells[1].Text = objPreTool.getTranslatedText(gvFavorites.Rows[i].Cells[1].Text, strCultureCode);
                        gvFavorites.Rows[i].Cells[1].Text = objPreTool.getTranslatedText((gvFavorites.Rows[i].Cells[1].Controls[1] as Label).Text, strCultureCode);
                        /* IM01365225 - New Agricast - favorite - Service Name - Jerrey - End */
                    }

                }
                Welcome.InnerText = objPreTool.getTranslatedText(Constants.WELCOME, strCultureCode);
                /*IM01289946-New Agricast - Add a * on mandatory fields-BEGIN*/

                lblFavName.InnerHtml = objPreTool.getTranslatedText(Constants.NAME, strCultureCode) + strSpanMandatory;


                //lblName.InnerText = objPreTool.getTranslatedText(Constants.NAME, strCultureCode);

                /*IM01294326 -New Agricast - Feedback section - Begin*/
                //Create a new Transtag for Feedback name 
                lblName.InnerHtml = objPreTool.getTranslatedText(Constants.FEEDBACK_SUBJECT, strCultureCode) + strSpanMandatory;
                /*IM01294326 -New Agricast - Feedback section - End*/

                //lblEmail.InnerText = objPreTool.getTranslatedText(Constants.EMAIL, strCultureCode);
                lblEmail.InnerHtml = objPreTool.getTranslatedText(Constants.EMAIL, strCultureCode) + strSpanMandatory;

                //lblMessage.InnerText = objPreTool.getTranslatedText(Constants.YOUR_COMMENTS, strCultureCode);
                lblMessage.InnerHtml = objPreTool.getTranslatedText(Constants.YOUR_COMMENTS, strCultureCode) + strSpanMandatory;

                lblRating.InnerText = objPreTool.getTranslatedText(Constants.RATE_THIS_SITE, strCultureCode);

                //lblCaptcha.InnerText = objPreTool.getTranslatedText(Constants.CAPTCHA_EXPLANATION, strCultureCode);
                lblCaptcha.InnerHtml = objPreTool.getTranslatedText(Constants.CAPTCHA_EXPLANATION, strCultureCode) + strSpanMandatory;
                /*IM01289946-New Agricast - Add a * on mandatory fields-END*/

                btnSendMsg.Text = objPreTool.getTranslatedText(Constants.SEND, strCultureCode);
                /*IM01246263 - New Agricast - Add a new translation tag  - BEGIN*/
                lblFeedBackHeader.InnerText = objPreTool.getTranslatedText(Constants.FEEDBACK_HEADER, strCultureCode);
                /*IM01246263 - New Agricast - Add a new translation tag  - END*/

                /* July 23, Spritzwetter - Added by Jerrey - Begin */

                BindRangeDropDownListItems(strCulCode);

                var hintsMsg = string.Format(@"<div style=""font-size:10px; padding-left:20px;""><br />{0}<br /><br /></div>",
                        objPreTool.getTranslatedText(Constants.HINTS_MESSAGE, strCulCode));
                txtHintsMessageOfFeedback.Text = hintsMsg;
                txtHintsMessageOfEmail.Text = hintsMsg;
                /* July 23, Spritzwetter - Added by Jerrey - End */
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.TRANSLATION_FAILURE, strCulCode) + ex.Message.ToString();
            }
        }

        /* July 23, Spritzwetter - Added by Jerrey - Begin */
        private void BindRangeDropDownListItems(string strCulCode)
        {
            if (!IsPostBack)
            {
                range.Items.Clear();
                range.Items.Add(new ListItem("---", ""));
                range.Items.Add(new ListItem(objPreTool.getTranslatedText(Constants.OPTION_INSUFFICIENTLY, strCulCode), "0"));
                range.Items.Add(new ListItem(objPreTool.getTranslatedText(Constants.OPTION_DEFICIENT, strCulCode), "1"));
                range.Items.Add(new ListItem(objPreTool.getTranslatedText(Constants.OPTION_SUFFICIENTLY, strCulCode), "2"));
                range.Items.Add(new ListItem(objPreTool.getTranslatedText(Constants.OPTION_SATISFACTORY, strCulCode), "3"));
                range.Items.Add(new ListItem(objPreTool.getTranslatedText(Constants.OPTION_GOOD, strCulCode), "4"));
                range.Items.Add(new ListItem(objPreTool.getTranslatedText(Constants.OPTION_VERY_WELL, strCulCode), "5"));
            }
        }
        /* July 23, Spritzwetter - Added by Jerrey - End */

        protected void ddlCulture_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Session["sessionCultruCode"] = ddlCulture.SelectedValue; 
                // objPreTool = new ToolBarPresenter(this);
                //// ((Label)((UserControl)((Content)ddlCulture.NamingContainer.NamingContainer.FindControl("BodyContent")).FindControl("body")).FindControl("searchIn")).Text = "Hello";

                // Fee_EmailAddress.InnerText = objPreTool.getTranslatedText(Fee_EmailAddress.ID, ddlCulture.SelectedValue);
                // mailbody.InnerText = objPreTool.getTranslatedText(mailbody.ID, ddlCulture.SelectedValue);
                // EMailPage.Value = objPreTool.getTranslatedText(EMailPage.ID, ddlCulture.SelectedValue);
                // Fav_AddToFavorites.Value = objPreTool.getTranslatedText(Fav_AddToFavorites.ID, ddlCulture.SelectedValue);
                // gvFavorites.HeaderRow.Cells[0].Text = objPreTool.getTranslatedText("name", ddlCulture.SelectedValue);
                // gvFavorites.HeaderRow.Cells[1].Text = objPreTool.getTranslatedText("service", ddlCulture.SelectedValue);
                // gvFavorites.HeaderRow.Cells[2].Text = objPreTool.getTranslatedText("LocationName", ddlCulture.SelectedValue);
                // for (int i = 0; i < gvFavorites.Rows.Count; i++)
                // {
                //     gvFavorites.Rows[i].Cells[1].Text = objPreTool.getTranslatedText("weathercast", ddlCulture.SelectedValue);
                // }
                onIndexChange(e);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                throw ex;
            }

        }

        //protected void gvFavorites_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //    Response.Write(gvFavorites.SelectedRow.Cells[1].Text);
        //}

        protected void onIndexChange(EventArgs e)
        {
            if (IndexChange != null)
            {
                IndexChange(this, e);
            }
        }

        protected void gv_Favorites_RowDataBound(object obj, GridViewRowEventArgs e)
        {

            try
            {
                /* IM01365142 - New Agricast - Favorite table - translation for "Edit" and "Delete" - Jerrey - Begin */
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[3].Text = string.Format("{0} / {1}",
                                                    objPreTool.getTranslatedText(Constants.FAV_EDIT, strCulCode),
                                                    objPreTool.getTranslatedText(Constants.FAV_DELETE, strCulCode));
                }
                /* IM01365142 - New Agricast - Favorite table - translation for "Edit" and "Delete" - Jerrey - End */

                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    LinkButton lbButton = new LinkButton();
                    lbButton.Text = e.Row.Cells[0].Text;
                    e.Row.Cells[0].Controls.Add(lbButton);
                    lbButton.CommandName = "Select";
                    lbButton.CommandArgument = index.ToString();
                    index++;

                    /* IM01365225 - New Agricast - favorite - Service Name - Jerrey - Begin */
                    objSvcPre = new ServicePresenter();
                    Label lblModuleName = new Label();
                    lblModuleName.Text = e.Row.Cells[1].Text;
                    lblModuleName.Visible = false;
                    e.Row.Cells[1].Controls.Add(lblModuleName);

                    Label lblModuleTransTag = new Label();
                    lblModuleTransTag.Text = objSvcPre.GetServicePageTransTag(lblModuleName.Text);
                    e.Row.Cells[1].Controls.Add(lblModuleTransTag);
                    /* IM01365225 - New Agricast - favorite - Service Name - Jerrey - End */
                }




                //hide the key field in the GRID
                e.Row.Cells[4].Visible = false;


            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.GENERIC_ERRORONPAGE, strCulCode) + ex.Message.ToString();

            }


        }


        protected void gvFavorites_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                favorites.Attributes.Add("class", "show");
                gvFavorites.EditIndex = e.NewEditIndex;
                getFavorites();
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                throw ex;
            }
        }

        protected void gvFavorites_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {

                string strFavKey = gvFavorites.DataKeys[e.RowIndex].Value.ToString();
                objPreTool.DeleteFavorite(strFavKey);
                objPreTool.getCultureCode();
                //Trigger the Event to Update the Favorite Dictionary and save in database
                onSaveFavorite(e);

                getFavorites();
                //HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FAVORITE_DELETE_SUCCESS, strCulCode);
                HttpContext.Current.Session["SuccessMessage"] = objPreTool.getTranslatedText(Constants.FAVORITE_DELETE_SUCCESS, strCulCode);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FAVORITE_DELETE_FAILURE, strCulCode); ;
            }
        }

        protected void gvFavorites_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                TextBox favoriteName = gvFavorites.Rows[e.RowIndex].Cells[0].Controls[0] as TextBox;
                TextBox favoriteId = gvFavorites.Rows[e.RowIndex].Cells[4].Controls[0] as TextBox;
                objPreTool = new ToolBarPresenter(this);
                objPreTool.getCultureCode();
                //objPreTool.updateFavorite(favoriteName.Text, Convert.ToInt32(favoriteId.Text));
                objPreTool.UpdateFavorite(favoriteName.Text, favoriteId.Text);
                gvFavorites.EditIndex = -1;

                if (IsFavUpdated)
                {
                    //Trigger the Event to Update the Favorite Dictionary and save in database
                    onSaveFavorite(e);

                    getFavorites();
                    //HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FAVORITE_UPDATE_SUCCESS, strCulCode); ;
                    HttpContext.Current.Session["SuccessMessage"] = objPreTool.getTranslatedText(Constants.FAVORITE_UPDATE_SUCCESS, strCulCode);
                    favorites.Attributes.Add("class", "hide");
                }
                else
                {
                    HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FAVORITE_UPDATE_DUPLICATE, strCulCode) + favoriteName.Text + objPreTool.getTranslatedText(Constants.GENERIC_ALREADYEXISTS, strCulCode);
                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FAVORITE_UPDATE_FAILURE, strCulCode);
            }
        }

        protected void gvFavorites_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            try
            {
                double d = 0.0;
                string pGetFavPlaceDetails = string.Empty;
                GridView _gridView = (GridView)sender;
                bool serviceExists = false;
                objPreTool = new ToolBarPresenter(this);
                objPreTool.getCultureCode();

                if (e.CommandName.Equals("Select", StringComparison.CurrentCultureIgnoreCase))
                {
                    int _selectedIndex = int.Parse(e.CommandArgument.ToString());
                    string _commandName = e.CommandName;

                    _gridView.SelectedIndex = _selectedIndex;

                    //Code to check existance of the service in the current pub
                    //Has to be fetched only if exists
                    /* IM01365225 - New Agricast - favorite - Service Name - Jerrey - Begin */
                    // commented by Jerrey - IM01365225
                    //string strService = gvFavorites.SelectedRow.Cells[1].Text;
                    string strService = (gvFavorites.SelectedRow.Cells[1].Controls[0] as Label).Text;
                    /* IM01365225 - New Agricast - favorite - Service Name - Jerrey - End */
                    objPreTool.getNodeList("service");
                    foreach (string[] service in alNodeList)
                    {
                        if (service[1].ToString().ToLower() == strService.ToLower())
                        {
                            serviceExists = true;
                            break;
                        }
                    }

                    if (serviceExists)
                    {
                        /*objLocInfo.placeName */
                        strPlaceName = HttpUtility.HtmlDecode(gvFavorites.SelectedRow.Cells[2].Text);
                        /*objLocInfo.CountryCode */

                        //split the String and take the first part - Place Name
                        if (strPlaceName.IndexOf(',') != -1)
                        {
                            strPlaceName = strPlaceName.Split(',')[0];
                        }

                        //get the Value of Place Id from selected row
                        //string str1=_gridView.Rows[0].Cells[6].Text;
                        strPlaceID = gvFavorites.SelectedRow.Cells[6].Text;



                        //check if the Place Id is Null or empty or Zero
                        //In such cases, Pass the Place Name instead of PlaceID
                        if (string.IsNullOrEmpty(strPlaceID) || strPlaceID.Equals("0"))
                        {
                            pGetFavPlaceDetails = strPlaceName;// set the PlaceName as parameter 
                        }
                        else
                        {
                            pGetFavPlaceDetails = strPlaceID;// set the PlaceID as parameter 
                        }

                        // //Call the Presenter method to get Location Details
                        objPreTool.GetFavPlaceDetails(pGetFavPlaceDetails);


                        /*remove the concept of place id and place name.
                         *always retrive the favorite based on the latitude and longitude of the location*/

                        //string strFavLatitude = gvFavorites.SelectedRow.Cells[8].Text ;
                        //string strFavLongitude = gvFavorites.SelectedRow.Cells[9].Text;

                        //if (!(string.IsNullOrEmpty(strFavLatitude) || string.IsNullOrEmpty(strFavLongitude)))
                        //{
                        //    //Call the Presenter method to get Location Details
                        //    objPreTool.GetFavPlaceDetails(strFavLatitude, strFavLongitude);

                        //}


                        if (dtPlaceDetails.Rows.Count > 0)
                        {
                            objLocInfo.placeName = strPlaceName;//PlaceName
                            objLocInfo.latitude = double.TryParse(dtPlaceDetails.Rows[0][Constants.FAV_LATITUDE].ToString(), out d) ? d : 0.0;//Latitude
                            objLocInfo.longitude = double.TryParse(dtPlaceDetails.Rows[0][Constants.FAV_LONGITUDE].ToString(), out d) ? d : 0.0;//Longitude
                            objLocInfo.CountryCode = dtPlaceDetails.Rows[0][Constants.COUNTRY_CODE].ToString();//CountryCode
                        }
                        else
                        {
                            HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.GENERIC_DETAILSNOTFOUND, strCulCode);
                        }
                        /* IM01365225 - New Agricast - favorite - Service Name - Jerrey - Begin */
                        // commented by Jerrey - IM01365225
                        //objLocInfo.ServiceInfo.Module = gvFavorites.SelectedRow.Cells[1].Text;
                        objLocInfo.ServiceInfo.Module = (gvFavorites.SelectedRow.Cells[1].Controls[0] as Label).Text;
                        /* IM01365225 - New Agricast - favorite - Service Name - Jerrey - End */

                        //Remove the Old Module Name from the URL
                        string url = HttpContext.Current.Request.Url.ToString();
                        url = url.Split('?')[0];


                        System.Collections.Specialized.NameValueCollection t = HttpUtility.ParseQueryString(Request.Url.Query);
                        t.Set("module", objSvcInfo.Module);

                        /* IM01680394 - meinSyngenta; Spraweather; Bug in favorites function - Feb 28th, 2014 - Begin */
                        //if (!string.IsNullOrWhiteSpace(t.Get("securitykey")))
                        //    t.Remove("securitykey"); // because security key will expired after 5 mins
                        if (!string.IsNullOrWhiteSpace(t.Get("location")))
                            t.Set("location", objLocInfo.placeName);
                        if (!string.IsNullOrWhiteSpace(t.Get("lat")))
                            t.Set("lat", objLocInfo.latitude.ToString());
                        if (!string.IsNullOrWhiteSpace(t.Get("long")))
                            t.Set("long", objLocInfo.longitude.ToString());
                        /* IM01680394 - meinSyngenta; Spraweather; Bug in favorites function - Feb 28th, 2014 - End */

                        if (t.Count == 1)
                        {
                            url = url + "?" + t.Keys[0] + "=" + t[0];
                        }
                        else
                        {
                            for (int i = 0; i < t.Count; i++)
                            {
                                if (i == 0)
                                {
                                    url = url + "?" + t.Keys[i] + "=" + t[i];
                                }
                                else
                                {
                                    url = url + "&" + t.Keys[i] + "=" + t[i];
                                }

                            }
                        }


                        Response.Redirect(url, false);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();

                    }
                    else
                    {
                        HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.GENERIC_NOSERVICE, strCulCode);
                        isFavorite = false;
                    }


                }

                //if (e.CommandName.Equals("delete", StringComparison.CurrentCultureIgnoreCase))
                //{
                //    int i = int.Parse(e.CommandArgument.ToString());
                //    ImageButton btn = gvFavorites.Rows[i].Cells[3].Controls[0] as ImageButton;

                //    //theDeleteButton.Attributes.Add("onClick", "return confirm('Are you sure you want to delete?');");

                //    btn.OnClientClick = "return confirm('Are you sure you want to delete?')";

                //}

            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.GENERIC_ERRORONPAGE, strCulCode) + ex.Message.ToString();
            }

        }
        protected void gvFavorites_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                favorites.Attributes.Add("class", "hide");
                gvFavorites.EditIndex = -1;
                getFavorites();
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                throw ex;
            }
        }

        protected void gvFavorites_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                string sortExpression = e.SortExpression;

                if (GridViewSortDirection == SortDirection.Ascending)
                {

                    GridViewSortDirection = SortDirection.Descending;

                    SortGridView(sortExpression, "DESC");

                }

                else
                {

                    GridViewSortDirection = SortDirection.Ascending;

                    SortGridView(sortExpression, "ASC");

                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FAVORITE_SORTING_ERROR, strCulCode) + ex.Message.ToString();
            }
        }

        public SortDirection GridViewSortDirection
        {

            get
            {

                if (ViewState["sortDirection"] == null)

                    ViewState["sortDirection"] = SortDirection.Ascending;

                return (SortDirection)ViewState["sortDirection"];

            }

            set { ViewState["sortDirection"] = value; }

        }

        private void SortGridView(string sortExpression, string direction)
        {
            objPreTool = new ToolBarPresenter(this);


            //Get the Favorites from session 
            objUserInfo = (UserInfo)Session["objUserInfo"];
            //objPreTool.getFavorites(objUserInfo.FavList);
            objPreTool.getFavorites();

            DataView dv = new DataView(dtFavorites);
            dv.Sort = sortExpression + " " + direction;

            gvFavorites.DataSource = dv;
            gvFavorites.DataBind();

            DataTable dt = dv.ToTable();

            if (objUserInfo != null)
            {
                objUserInfo.DtFavorites = dt;
                Session["objUserInfo"] = objUserInfo;
            }


            //favorites.Attributes["class"] = "show";   
            if (hdnGridImages_Status != null)
            {
                favorites.Attributes.Add("class", hdnGridImages_Status.Value);

            }

        }

        public string Favorite
        {
            get
            {
                return txtFavName.Text.ToString();
            }
            set
            {
                txtFavName.Text = value;
            }
        }

        public string MailIds
        {
            get
            {
                return Tomail.Value.ToString();
            }
        }
        public string Sub
        {
            get
            {
                return Subject.Value.ToString();
            }
            set
            {
                Subject.Value = value;
            }
        }
        //public string MailBody
        //{
        //    get {
        //        return bodyText.Value.ToString();
        //    }
        //    set {
        //        bodyText.Value = value;
        //    }
        //}
        public DataTable Favorites
        {
            set
            {
                gvFavorites.DataSource = value;
                gvFavorites.DataBind();
            }
        }



        public string selLang
        {
            get
            {

                return ddlCulture != null ? ddlCulture.SelectedValue : "";
            }
        }

        public string selUnit
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string setUserLink
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool showEmail
        {
            get;
            set;
        }

        public bool showExportExcel
        {
            get;
            set;
        }

        public bool showFavorites
        {
            get;
            set;
        }

        public bool showFeedback
        {
            get;
            set;
        }

        public bool showCulture
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool showPrint
        {
            get;
            set;
        }

        public bool showSavedata
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool showUnits
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public List<string[]> alNodeList
        {
            get;
            set;
        }
        public DataTable dtCultureCode
        {
            get
            {
                return dtCulCode;
            }
            set
            {
                dtCulCode = value;
                ddlCulture.DataSource = dtCulCode;
                ddlCulture.DataTextField = "Value";
                ddlCulture.DataValueField = "code";
                ddlCulture.DataBind();
                objPreTool = new ToolBarPresenter(this);
                objPreTool.getCultureCode();
                ddlCulture.SelectedValue = strCulCode;
            }
        }

        public DataTable dtUnits
        {
            get
            {
                return dtUnit;
            }
            set
            {
                dtUnit = value;
                if (objSvcInfo.ServiceName != "geotroll1")
                {
                    DataRow drCust = dtUnit.NewRow();
                    drCust[0] = CUSTOM;
                    dtUnit.Rows.Add(drCust);
                }
                //Get the Translated text and add it as a new Column .
                //assign this new column as TextField
                DataColumn dcTransText = new DataColumn("transtext");
                dtUnit.Columns.Add(dcTransText);

                //get transtext for the Units column
                foreach (DataRow dr in dtUnits.Rows)
                {
                    dr["transtext"] = objPreTool.getTranslatedText(dr["units"].ToString(), objSvcInfo.Culture);
                }
                ddlUnits.DataSource = dtUnit;
                ddlUnits.DataValueField = "units";
                ddlUnits.DataTextField = "transtext";
                ddlUnits.DataBind();
            }
        }

        public bool IsFavAdded
        {
            get;
            set;
        }


        public bool IsFavUpdated
        {
            get;
            set;
        }


        public DataTable dtFavorites
        {
            get
            {
                return dtFav;
            }
            set
            {
                dtFav = value;
                gvFavorites.DataSource = dtFav;
                gvFavorites.DataBind();

                //Set the Visibility of Favorites Grid to True
                // hdnGridFavorite_Status.Value = "show";
            }
        }




        public string strText
        {
            get
            {
                return strTranslatedText;
            }
            set
            {
                strTranslatedText = value;
            }
        }

        public string getCultureCode
        {
            get
            {
                return ddlCulture.SelectedValue;
            }
        }

        public string DefaultUnitSettings
        {
            get;
            set;
        }

        public void LoadAnother()
        {
            LoadCaptcha();
        }

        /// <summary>
        /// Set captcha
        /// </summary>
        public void LoadCaptcha()
        {

            try
            {
                objPreTool.getCultureCode();
                SetValues();
                objPreTool.GetRandomText(characterSet, captchaLength);
                Session.Add("captcha", strRandomText);
                //Cancel for IM01848043 : new Agricast - catpcha image not displayed from time to time - Start
                //string ImagePath = GetImage();
                //Im1.ImageUrl = "~/temp/" + ImagePath.Substring(ImagePath.LastIndexOf("\\") + 1);
                //Cancel for IM01848043 : new Agricast - catpcha image not displayed from time to time - end
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FEEDBACK_CAPTCHA_LOADFAILURE, strCulCode) + ex.Message.ToString();
            }

        }
        public void SetValues()
        {
            backgroundImagePath = HttpRuntime.AppDomainAppPath + @"\Images\captcha2.png";
            captchaLength = Constants.CAPTCHA_LENTGH;
            fontSize = Constants.FONT_SIZE;
            fontFamily = Constants.FONT_FAMILY;
            textColor = Constants.TEXT_COLOR;
            // errorMessage = Constants.FEEDBACK_INCORRECT_CAPTCHA;
            characterSet = Constants.CHARACTER_SET;
        }

        public bool ValidateCaptcha(string text)
        {
            objPreTool = new ToolBarPresenter(this);
            objPreTool.getCultureCode();
            try
            {
                HttpSessionState session = HttpContext.Current.Session;
                if (text.ToLower() == ((string)session["captcha"]).ToLower())
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FEEDBACK_CAPTCHA_VALIDATION, strCulCode) + ex.Message.ToString();
                return false;
            }

        }



        public string GetImage()
        {

            string text = string.Empty;
            if (this.Session["captcha"] != null)
                text = (string)this.Session["captcha"];

            int imageWidth = Convert.ToInt32(((fontSize + 8) * text.Length));
            int imageHeight = Convert.ToInt32(fontSize * 2.5);


            Bitmap bmp = new Bitmap(imageWidth, imageHeight);
            var Grph = Graphics.FromImage(bmp);
            Grph.FillRectangle(new SolidBrush(Color.Lavender), 0, 0, bmp.Width, bmp.Height);

            var grp = Graphics.FromImage(bmp);
            System.Drawing.Image background = System.Drawing.Image.FromFile(backgroundImagePath);
            grp.DrawImage(background, new Rectangle(0, 0, bmp.Width, bmp.Height));


            Grph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            Grph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            int xPos = 10;

            Font f = GetFont();

            char[] textArray = text.ToCharArray();
            int yPosition = 0;
            Random r = new Random();

            for (int i = 0; i < textArray.Length; i++)
            {
                if (i % 2 == 0)
                    Grph.RotateTransform(5);
                else
                    Grph.RotateTransform(-5);

                yPosition = (int)(r.NextDouble() * 10);

                Grph.DrawString(textArray[i].ToString(), f, new SolidBrush(Color.FromName(textColor)), xPos, yPosition);
                xPos += 20;

            }
            string imgpath = HttpRuntime.AppDomainAppPath + @"\temp\Captcha" + System.Diagnostics.Stopwatch.GetTimestamp() + ".jpeg";
            bmp.Save(imgpath, System.Drawing.Imaging.ImageFormat.Jpeg);


            return imgpath;

        }
        public System.Drawing.Font GetFont()
        {
            return new System.Drawing.Font(fontFamily, (float)fontSize);
        }

        public void btnSendMsg_Click(object sender, EventArgs e)
        {
            try
            {
                string text = txtCaptcha.Text;
                //LStatus.Text = ErrorMessage;
                // LStatus.Text = SuccessMessage;
                isFeedback = true;
                objPreTool = new ToolBarPresenter(this);
                objPreTool.getCultureCode();
                if ((feedbackName != null && feedbackEmail != null && feedbackMessage != null) && (feedbackName != "" && feedbackEmail != "" && feedbackMessage != ""))
                {

                    string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                                 + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                                 + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                                 + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

                    bool validateEmail = Regex.IsMatch(feedbackEmail, MatchEmailPattern);
                    if (validateEmail)
                    {

                        lblEmailError.Text = "";
                        if (text == "")
                        {
                            //LStatus.Text = objPreTool.getTranslatedText(EMPTY_CAPTCHA, strCulCode);
                            HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FEEDBACK_EMPTY_CAPTCHA, strCulCode);
                        }
                        else
                        {
                            if (ValidateCaptcha(text))
                            {
                                string Rating = hdnFeedbackRating.Value;
                                /*IM01294335	New Agricast - rating not sent - Begin*/
                                //string mailBody = objPreTool.getTranslatedText(Constants.USER_RATING, strCulCode) + " : " + Rating + " <br /> " + feedbackMessage;
                                string mailBody = objPreTool.getTranslatedText(Constants.USER_RATING, strCulCode) + " : " + Rating + " <br /> " +
                                    objPreTool.getTranslatedText(Constants.USER_COMMENT, strCulCode) + " : " + feedbackMessage;
                                /*IM01294335	New Agricast - rating not sent - End*/
                                objPreTool.SendMail(feedbackName, alEmailSettings[1].ToString(), feedbackEmail, mailBody);
                                //LStatus.Text = SUCCESS_MESSAGE;
                                //HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FEEDBACK_SUCCESS, strCulCode);
                                HttpContext.Current.Session["SuccessMessage"] = objPreTool.getTranslatedText(Constants.FEEDBACK_SUCCESS, strCulCode);
                                isFeedback = true;//show feedback grid after sending msg
                                //clear Captcha and Comments
                                txtCaptcha.Text = string.Empty;
                                txtName.Value = string.Empty;
                                TextareaMessage.InnerText = string.Empty;
                                txtEmail.Value = string.Empty;
                                LoadAnother();
                            }
                            else
                            {
                                //LStatus.Text = objPreTool.getTranslatedText(ErrorMessage, strCulCode);
                                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FEEDBACK_INCORRECT_CAPTCHA, strCulCode);
                                LoadAnother();
                                txtCaptcha.Text = "";
                            }
                        }
                    }
                    else
                    {
                        //lblEmailError.Text = objPreTool.getTranslatedText(EMAIL_ERROR, strCulCode);
                        HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FEEDBACK_EMAIL_VALIDATION, strCulCode);
                        //LStatus.Text = "";
                        LoadAnother();
                        txtCaptcha.Text = "";
                    }

                }
                else
                {
                    //LStatus.Text = objPreTool.getTranslatedText(FILL_FORM, strCulCode);
                    HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FEEDBACK_FILLFORM, strCulCode);
                    LoadAnother();
                    txtCaptcha.Text = "";
                }

            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objServicePre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objPreTool.getTranslatedText(Constants.FEEDBACK_GENERROR, strCulCode) + ex.Message.ToString();
            }
        }

        public void btnRefresh_Click(object sender, ImageClickEventArgs e)
        {

            LoadAnother();
            txtCaptcha.Text = "";
            feedback.Attributes.Add("class", "show");
            IsCaptchaReset = true;
        }


        public int captchaLength
        {
            get { return CaptchaLength; }
            set
            {
                try
                {
                    int k = Convert.ToInt32(value);
                    if (k < 5 || k > 10)
                        CaptchaLength = 6;
                    else
                        CaptchaLength = k;
                    if (value == 0)
                    {
                        CaptchaLength = 6;
                    }
                }
                catch (Exception ex)
                {
                    CaptchaLength = 6;
                }
            }
        }

        public double fontSize
        {
            get { return FontSize; }
            set
            {
                try
                {
                    FontSize = Convert.ToInt32(value);
                    if (FontSize <= 10 && FontSize >= 24)
                        FontSize = 16;
                    if (value == 0)
                    {
                        FontSize = 16;
                    }
                }
                catch (Exception ex)
                {
                    FontSize = 16;
                }
            }
        }

        public string fontFamily
        {

            get { return FontFamily; }
            set
            {
                if (value != string.Empty && value != null)
                    FontFamily = value;
                else
                    FontFamily = "Arial";
            }
        }

        public string backgroundImagePath
        {
            get { return BackgroundImagePath; }
            set
            {

                BackgroundImagePath = value;
            }
        }

        public string textColor
        {

            get { return TextColor; }
            set
            {
                if (value == string.Empty || value == null)
                    TextColor = "Gray";
                else
                    TextColor = value;
            }
        }

        public string successMessage
        {
            get { return SuccessMessage; }
            set { SuccessMessage = value; }
        }

        public string characterSet
        {
            get { return CharacterSet; }
            set
            {
                if (value == "" || value == null)
                    CharacterSet = "123456789";
                else
                    CharacterSet = value;
            }
        }

        public DataTable dtPlaceDetails
        {
            get;
            set;
        }
        public string feedbackName
        {
            get
            {
                return txtName.Value;
            }
            set
            {
                FeedbackName = txtName.Value;
            }
        }

        public string feedbackEmail
        {
            get
            {
                return txtEmail.Value;
            }
            set
            {
                FeedbackEmail = txtEmail.Value;
            }
        }

        public string feedbackMessage
        {
            get
            {
                return TextareaMessage.Value;
            }
            set
            {
                FeedbackMessage = TextareaMessage.Value;
            }
        }

        protected void hdnFeedbackRating_ValueChanged(object sender, EventArgs e)
        {
            Rating = hdnFeedbackRating.Value.ToString();
        }


        public string strCulCode
        {
            get
            {
                return CulCode;
            }
            set
            {
                CulCode = value;
            }
        }


        public string strRandomText
        {
            get
            {
                return strRandomtxt;
            }
            set
            {
                strRandomtxt = value;
            }
        }

        public string strCustomUnits
        {
            get;
            set;
        }

        public ArrayList alEmailSettings
        {
            //get
            //{
            //    alEmailSettings.Add(Tomail.Value.Trim());
            //    alEmailSettings.Add(Subject.Value);
            //    alEmailSettings.Add(body);

            //    return alEmailSettings;
            //}
            get;
            set;
        }
        protected void imgPrint_Click(object sender, ImageClickEventArgs e)
        {
            //Trigger the Print Page Event (common for Print and E mail)
            onPrintPage(e);

            blGenerateHtml = true;//create HTML for printing
            IsPrint = true;// Process the workflow only till printing the html

        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            // can be used later to save custom settings in case multiple custom controls are present.

            //objSvcInfo.WUnit =  "#Wind-" + ddlWind.SelectedIndex.ToString() != "-1" ? ddlWind.SelectedItem.ToString() : "" +
            //    "#Rain-" + ddlRain.SelectedIndex.ToString() != "-1" ? ddlRain.SelectedItem.ToString() : "" +
            //    "#Temperature-" + ddlTemp.SelectedIndex.ToString() != "-1" ? ddlTemp.SelectedItem.ToString() : "";
            //objSvcInfo = (ServiceInfo)Session["serviceInfo"];
            //  ddlUnits.SelectedValue = objSvcInfo.Unit.ToLower();
            objSvcInfo.WUnit = ddlWind.SelectedValue.ToString();

        }

        protected void ddlUnits_SelectedIndexChanged(object sender, EventArgs e)
        {


            if (ddlUnits.SelectedValue.ToString() != CUSTOM)
            {
                objSvcInfo.Unit = ddlUnits.SelectedValue.ToString();
            }
            else if (ddlUnits.SelectedValue.ToString() == CUSTOM && objSvcInfo.Unit == string.Empty)
            {

                objSvcInfo.Unit = DefaultUnitSettings;
            }
            Session["serviceInfo"] = objSvcInfo;
        }

        public string errorMessage
        {
            get;
            set;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ddlUnits.SelectedValue = objSvcInfo.Unit.ToLower();
        }
    }

}
