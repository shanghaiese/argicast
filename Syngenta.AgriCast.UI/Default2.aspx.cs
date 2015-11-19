using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.Serialization;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Syngenta.AgriCast.LocationSearch.LocWebService;
using Syngenta.AgriCast.LocationSearch;
using System.Collections;
using Syngenta.AgriCast.Charting;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common;
using Syngenta.AgriCast.Common.View;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.LocationSearch.Service;
using Syngenta.AgriCast.AgriInfo;
using System.IO;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using Syngenta.AgriCast.Icon;
using Syngenta.AgriCast.Tables;
using Syngenta.AgriCast.Tables.Presenter;
using System.Reflection;
using Syngenta.AgriCast.AgriInfo.Presenter;
using Syngenta.AgriCast.Charting.Presenter;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.Agricast.UserManagement;
using System.Web.Services;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using ExpertPdf.HtmlToPdf;
using ExpertPdf.HtmlToPdf.PdfDocument;
using System.Diagnostics;
using System.Web.Security;
using System.Web.Profile;
using System.Collections.Specialized;
using Syngenta.AgriWeb.NearByStation.DataObjects;



public partial class _Default2 : BasePage, IDefault
{

    # region Declarations
    LocSearchWebService objWebService = new LocSearchWebService();
    Controls common = new Controls();
    Toolbar tb;
    WebLocationSearch wls;
    //AgriInfo ai;
    //Icon ic;
    //WebChart wc;
    //Tables tbl;
    ServiceInfo objSvcInfo;
    ServicePresenter objSvcPre;
    string pubname = null;
    string ccode = null;
    LocationInfo objLocInfo;
    DataPointInfo objDataPointInfo;
    DataTable dtPages;
    TablePresenter objTblPre;
    bool hasLoadedControls = false;
    string url = string.Empty;
    string subStrURL = string.Empty;
    IDictionary dictAudit = new Hashtable();
    ToolBarPresenter objToolBarPre = null;
    string pdf_License = ConfigurationManager.AppSettings["pdfLicence"];
    string sFile = "";
    DataTable dtFavorites = null;
    UserInfo objUserInfo = null;
    Dictionary<string, Syngenta.Agricast.UserManagement.Favorite> dictFavList = null;
    Syngenta.Agricast.UserManagement.Favorite objFav = null;
    CommonUtil objCommonUtil = new CommonUtil();
    ServiceHandler sh = new ServiceHandler();
    bool isEmail = false;
    bool isPrint = false;
    bool blGenerateHtml = false;
    bool blIsFavLoaded = false;
    SmtpClient smtpClientObj;
    string smtpServer = System.Configuration.ConfigurationManager.AppSettings["MailConfig"];
    string strMailBody = string.Empty;
    string strSubject = string.Empty;
    string strAttachmentpath = string.Empty;
    string strTo = string.Empty;
    string strFrom = string.Empty;
    MailMessage mail = null;
    Attachment attachFile = null;
    bool ispubchange = false;
    string filename = string.Empty;
    string tempFolderPath = ConfigurationManager.AppSettings["tempfolder"];
    string rootPath = string.Empty;//Application root Path.
    string strFullPath_Nofile = string.Empty;
    string strHTMLFilePath = string.Empty; //Holds the path where HTMl is Stored
    //Constants
    string DEFAULTMODULE = "Weathercast";
    string LABEL250 = "label250";
    const string BOXMINUS = "~/Images/boxminus.gif";
    MembershipUser memUser = null;
    bool isAuthorised = false;
    //created for location search logic-start
    string strOverridecookie = string.Empty;
    string strLocation = string.Empty;
    string strLat = string.Empty;
    string strLong = string.Empty;
    string strClientid = string.Empty;
    HttpCookie locCookie;
    HttpCookie LocationCookie;

    //created for location search logic-end

    #endregion

    #region Events
    protected void Page_Load(object sender, EventArgs e)
    {

        lblError.Text = " ";

        if (Session == null || Session["serviceInfo"] == null)
            objSvcInfo = new ServiceInfo();
        else
            objSvcInfo = (ServiceInfo)Session["serviceInfo"];

        objDataPointInfo = DataPointInfo.getDataPointObject;
        objLocInfo = new LocationInfo();

        try
        {
            HttpContext.Current.Session["ErrorMessage"] = "";
            HttpContext.Current.Session["ErrorMessage"] = null;
            HttpContext.Current.Session["SuccessMessage"] = "";
            HttpContext.Current.Session["SuccessMessage"] = null;
            if (!string.IsNullOrEmpty(Request.QueryString["Clientid"]))
            {
                strClientid = Request.QueryString["Clientid"].ToString();
                locCookie = Request.Cookies["ais_" + strClientid];
                Session["Clientid"] = strClientid;
            }
            else if (Session["Clientid"] != null)
            {
                strClientid = Session["Clientid"].ToString();
                locCookie = Request.Cookies["ais_" + Session["Clientid"].ToString()];
            }
            else
                locCookie = Request.Cookies["ais_LocationInfo"];


            if (locCookie != null)
            {
                LocationInfo newInfo = objLocInfo.DeserializeCookieString(locCookie.Value);
                if (newInfo != null)
                {
                    Session["objLocationInfo"] = newInfo;
                    objLocInfo = newInfo;
                }
                if (Session["AuditData"] != null)
                {
                    IDictionary dict = (IDictionary)Session["AuditData"];
                    dict["locSearchType"] = "By Cookie";
                    Session["AuditData"] = dict;
                }
            }
            else
            {
                objLocInfo.latitude = 0.0;
                objLocInfo.longitude = 0.0;
                objLocInfo.placeName = "";
                //Clearing off datapoint object in case search crtieria is given in query string
                objDataPointInfo = null;
                objLocInfo.DataPointInfo = null;

                Session["objDataPointInfo"] = null;
                Session["objLocationInfo"] = null;
            }


            if (objSvcInfo.IsMobile)
            {
                objSvcInfo.IsMobile = false;
                objSvcInfo.Module = "";
            }

            objSvcPre = new ServicePresenter(this);

            #region Agricast CR 3.5
            /* Agricast CR - 3.5	R5 - External user management changes. - Begin */
            if (!string.IsNullOrEmpty(Request.QueryString["securitykey"]))// && !string.IsNullOrEmpty(Request.QueryString["clientid"]))
            {
                #region Syncrptography

                var encryptKey = ConfigurationManager.AppSettings["EncryptKey"];
                var defaultPassword = ConfigurationManager.AppSettings["DefaultPassword"];

                /* expired time check - begin - added by Jerrey */
                int iExpiredTime = 0;
                var expiredTime = ConfigurationManager.AppSettings["SecurityKeyExpiredTime"];
                if (string.IsNullOrWhiteSpace(expiredTime))
                    int.TryParse(expiredTime, out iExpiredTime);
                if (iExpiredTime == 0)
                    iExpiredTime = 30;
                /* expired time check - end - added by Jerrey */

                var securityKey = Request.QueryString["securitykey"];
                var clientID = Request.QueryString["clientid"];
                //After HtmlDecode, %2b will be " ", replace it with +
                if (securityKey.Contains(" "))
                    securityKey = securityKey.Replace(" ", "+");
                
              
                
                // decrypt security key                
               
                //currently key used is null
                var decryptedNode = Syngenta.Agriweb.SynCryptography.SynCryptographyConsumer.Decrypt(securityKey, null, "AgriCast", "Direct");
               

                // get token & userID & datetime
                var info = decryptedNode.Split(new char[] { '#' });
                var token = info[0];
                var userID = info[1].Trim() != "" ? info[1] : ConfigurationManager.AppSettings["DefaultUser"];
                var dateStr = info[2];
                var dateTime = DateTime.Parse(dateStr);

                #endregion

                string uid = string.Format("{0}^{1}", token, userID);

                // check date and time Difference < 5 min
                if (DateTime.UtcNow.Subtract(dateTime).Minutes > iExpiredTime) // 5) -- modified by Jerrey
                {
                    Response.Redirect("SessionExpired.aspx?ExtUsrAD=1", false);
                    return;
                }
                url = HttpContext.Current.Request.Url.ToString();
                subStrURL = url.Substring(0, url.LastIndexOf(".aspx"));
                subStrURL = subStrURL.Substring(0, subStrURL.LastIndexOf("/"));
                //Get the Pub Name 
                int lastindex = subStrURL.LastIndexOf("Pub/", StringComparison.InvariantCultureIgnoreCase);
                string strNewPubName = subStrURL.Substring(lastindex + 4);

                // check pub
                var isValidRole = Roles.RoleExists(strNewPubName);
                if (!isValidRole)
                {
                    Response.Redirect("SessionExpired.aspx?ExtUsrAD=3", false);
                    return;
                }

                // check if the token is valid 
                var validToken = Roles.FindUsersInRole(strNewPubName, token);
                if (validToken.Length == 0)
                {
                    Response.Redirect("SessionExpired.aspx?ExtUsrAD=2", false);
                    return;
                }

                var isValidUser = Membership.ValidateUser(uid, defaultPassword);
                if (!isValidUser)
                {
                    Membership.CreateUser(uid, defaultPassword);
                }

                // allow access
                UserInfo objUserInfo = (UserInfo)Session["objUserInfo"];
                if (objUserInfo == null) objUserInfo = new UserInfo();
                if (objUserInfo != null)
                {
                    objUserInfo.IsAuthenticated = true;
                    isAuthorised = true;
                    Session["authority"] = "true";
                    objUserInfo.UserName = uid;
                    Session["objUserInfo"] = objUserInfo;
                }
                /*IM01246266 - New Agricast - can't save a favourite - Begin */
                //External user should be able to add favorite - based on the tokenID^username passed as security key
                ProfileCommon objProf = Profile.GetProfile(uid);
                if (objProf != null)
                {
                    Session["UserFavorites"] = objProf.MyCustomProfile.FavoriteList;
                }
                /*IM01246266 - New Agricast - can't save a favourite - End */
                if (info.Length > 3)
                {
                    var QS = (NameValueCollection)Request.GetType().GetField("_queryString", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Request);
                    PropertyInfo readOnlyInfo = QS.GetType().GetProperty("IsReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
                    readOnlyInfo.SetValue(QS, false, null);
                    var parameters = info[3].Split(new char[] { '&' });
                    var result = parameters.Where(g => g.Contains("Location="));
                    if (parameters.Where(g => g.Contains("Location=")).FirstOrDefault() != null)
                    {
                        var data = parameters.Where(g => g.Contains("Location=")).FirstOrDefault().Split(new char[] { '=' })[1];
                        if (!string.IsNullOrEmpty(data))
                        {
                            if (Request.QueryString["Location"] != null)
                            {
                                QS["Location"] = data;
                            }
                            else
                            {
                                QS.Add("Location", data);
                            }
                        }
                    }

                    if (parameters.Where(g => g.Contains("Lat=")).FirstOrDefault() != null)
                    {
                        var data = parameters.Where(g => g.Contains("Lat=")).FirstOrDefault().Split(new char[] { '=' })[1];
                        if (!string.IsNullOrEmpty(data))
                        {
                            if (Request.QueryString["Lat"] != null)
                            {
                                QS["Lat"] = data;
                            }
                            else
                            {
                                QS.Add("Lat", data);
                            }
                        }
                    }

                    if (parameters.Where(g => g.Contains("Long=")).FirstOrDefault() != null)
                    {
                        var data = parameters.Where(g => g.Contains("Long=")).FirstOrDefault().Split(new char[] { '=' })[1];
                        if (!string.IsNullOrEmpty(data))
                        {
                            if (Request.QueryString["Long"] != null)
                            {
                                QS["Long"] = data;
                            }
                            else
                            {
                                QS.Add("Long", data);
                            }
                        }
                    }

                    readOnlyInfo.SetValue(QS, true, null);
                }
            }
            /* Agricast CR - 3.5	R5 - External user management changes. - End */
            #endregion

            //Checking previous servicename
            string prePubname = objSvcInfo.ServiceName;

            AddAuditdata();

            checkPub();
            if (String.IsNullOrEmpty(objSvcInfo.Module))
            {

                //set the default page if module is empty
                objSvcPre.getDefaultPage();
                objSvcInfo.Module = (DefaultTab != null || DefaultTab == "") ? DefaultTab : DEFAULTMODULE;

            }

            if (!Page.IsPostBack)
            {
                readPubnameQueryStrings();
                //Use this Method to Convert FavoriteList from Session
                GetFavorites();
            }

            //set the Cutlure code from ObjLocation object to Service info
            //this is used in Login page for tranlsation
            if (objLocInfo != null && objLocInfo.ServiceInfo != null)
                objSvcInfo.Culture = objLocInfo.ServiceInfo.Culture;

            //method to read whether the service is secured and requiers login
            objSvcPre.GetSecuritySetting();

            //method to read the allowed roles from the config
            objSvcPre.GetRoles();
            objSvcInfo.AllowedRoles = AllowedRoles;
            if (IsSecure)
            {
                CheckAuthority();
                UserInfo objUserInfo = (UserInfo)Session["objUserInfo"];
                if (objUserInfo != null)
                {
                    if (!(Session["authority"] != null && Session["authority"].ToString() == "true"))
                    {
                        if (objUserInfo != null)
                        {
                            objUserInfo.IsAuthenticated = false;
                            Session.Add("objUserInfo", objUserInfo);
                        }
                        Response.Redirect("Login.aspx", false);
                        Session.Add("LoginError", Constants.LOGIN_NOPUBACCESS);
                    }
                }
            }

            if ((objLocInfo.placeName == "" || (objLocInfo.latitude == 0d && objLocInfo.longitude == 0d)) && objLocInfo.CountryCode == "")
            {
                setDefault();
            }
            objSvcPre.getNodeList("servicePage");

            if (!IsAsync)
            {

                wls = (WebLocationSearch)common.LoadCtrl(this.Page, WebLocSearchHolder, "WebLocationSearch");
                wls.ClientIDMode = ClientIDMode.Static;
                tb = (Toolbar)common.LoadCtrl(this.Page, ToolbarHolder, "Toolbar");
                tb.ClientIDMode = ClientIDMode.Static;
                tb.ClickExport += new EventHandler(ClickExport);
                tb.SaveFavorite += new EventHandler(SaveFavorite);
                tb.EMail_Page += new EventHandler(EmailPage);
                tb.Print_Page += new EventHandler(PrintPage);

                LoadModules();
            }

            checkLanguage();


            #region Commented for /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
            //
            //if (!IsPostBack)
            //{
            //    if (Request.QueryString["Moss"] != null && Request.QueryString["Moss"].ToString() == "true")
            //    {
            //        objSvcInfo.Moss = "true";
            //    }

            //    /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
            //    //Get the Culture COde for moss from url
            //    if (!string.IsNullOrEmpty(Request.QueryString["MossCultureCode"]))
            //    {
            //        objSvcInfo.MossCultureCode = Request.QueryString["MossCultureCode"].ToString();
            //    }
            //    else
            //    {
            //        //if the querystirng is not passed , then set en-US as default code
            //        objSvcInfo.MossCultureCode = "en-US";
            //    }

            //    /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
            //}
            #endregion
            objSvcPre.GetExtNavigation();

            /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
            // get the MossReturnUrl from service config .this is an optional parameter 
            // When the session expires, Agricast Website will be redirected to this url when access from moss site i.e when moss=true and mossculturecode="any code" is passed in url
            objSvcPre.GetMossReturnUrl();

            if (!string.IsNullOrEmpty(MossReturnUrl))
                objSvcInfo.MossRedirectionUrl = MossReturnUrl;
            /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - END*/

            if (objSvcInfo.Moss == "true" && ExtNavigation != string.Empty)
            {

                createMossMenu();
                //if (dsMossMenu != null && objSvcInfo.Moss != "false")
                if (objSvcInfo.Moss != "false")
                {
                    createMenu();
                    Control ctrl = Master.FindControl("PageContent");
                    HtmlContainerControl mainDiv = (HtmlContainerControl)this.Master.FindControl("PageContent");
                    mainDiv.Attributes.Add("class", "mainMoss");
                    HtmlContainerControl navDiv = (HtmlContainerControl)this.Master.FindControl("PageMenu");
                    navDiv.Attributes.Add("class", "menuMoss");

                }
                else
                {
                    CreateRegularMenu();
                }
            }
            else
            {
                CreateRegularMenu();
            }

            ////Get the User Favorites
            //Dictionary<string, Favorite> dictFavList = ((Dictionary<string,Favorite>)Session["UserFavorites"]);

            //Redirect to Login Page only if userInfo Object is null or if the Publication is changed
            // if (Session["objUserInfo"] == null || !((UserInfo)Session["objUserInfo"]).IsAuthenticated)

            //redirect to login page only if security setting in config is true
            if (IsSecure && (Session["objUserInfo"] == null || !((UserInfo)Session["objUserInfo"]).IsAuthenticated))
            {
                url = HttpContext.Current.Request.Url.ToString();
                /* Agricast CR - 3.5	R5 - External user management changes. - Begin */
                subStrURL = url.Substring(0, url.LastIndexOf(".aspx"));
                subStrURL = subStrURL.Substring(0, subStrURL.LastIndexOf("/"));

                //subStrURL = url.Substring(0, url.LastIndexOf('/'));
                /* Agricast CR - 3.5	R5 - External user management changes. - End */
                //Get the Pub Name 
                int lastindex = subStrURL.LastIndexOf("Pub/", StringComparison.InvariantCultureIgnoreCase);
                string strNewPubName = subStrURL.Substring(lastindex + 4);

                //Get the String until pub and form the url
                subStrURL = subStrURL.Substring(0, subStrURL.LastIndexOf('/'));

                subStrURL = subStrURL + "/" + strNewPubName;
                Session.Add("RedirectURL", subStrURL);

                Response.Redirect("" + subStrURL + "/Login.aspx", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            else if (IsSecure == false)
            {
                //Session["objUserInfo"] = null;
            }

            setFooterText();


            ///* fix the issue of security key timeout - modified by Jerrey - Nov 27th, 2013 - begin */
            //if (objSvcInfo.Moss == "true" && !string.IsNullOrEmpty(Request.QueryString["securitykey"]))
            //{
            //    Response.Redirect(Request.Url.AbsolutePath, false);
            //}
            ///* fix the issue of security key timeout - modified by Jerrey - Nov 27th, 2013 - End */
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.DEF_LOAD_FAILURE) + " : " + ex.Message.ToString();
            return;


        }
        finally
        {
            //if (Session["AuditData"] != null)
            //{
            //    objSvcPre.SaveAuditData();
            //    //AgriCastException currEx = new AgriCastException(null,(IDictionary)Session["AuditData"]);
            //    //AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error, "Audit");
            //}
        }

    }

    public override void VerifyRenderingInServerForm(Control control)
    {

    }
    void CreateRegularMenu()
    {
        createMenu();
        Control ctrl = Master.FindControl("PageContent");
        HtmlContainerControl mainDiv = (HtmlContainerControl)this.Master.FindControl("PageContent");
        /* IM01174742 - [DD 15 Nov]New Agricast - appending the servicepage and the component name in the classes - Jerrey - Begin */
        mainDiv.Attributes.Add("class", "main " + objSvcInfo.Module);
        /* IM01174742 - [DD 15 Nov]New Agricast - appending the servicepage and the component name in the classes - Jerrey - End */
        HtmlContainerControl navDiv = (HtmlContainerControl)this.Master.FindControl("PageMenu");
        navDiv.Attributes.Add("class", "menu");
        slidemenu.Attributes.Add("class", "hide");
    }
    void CheckAuthority()
    {

        UserInfo objUserInfo = (UserInfo)Session["objUserInfo"];
        if (objUserInfo != null)
        {
            memUser = Membership.GetUser(objUserInfo.UserName);
            //Get all the roles for the signed in user
            string[] roles = Roles.GetRolesForUser(objUserInfo.UserName);
            //Get the roles allowed in the service
            string[] AllowedRoles = objSvcInfo.AllowedRoles.Split(',');
            if (AllowedRoles[0] != string.Empty)
            {
                foreach (string strAllowedRole in AllowedRoles)
                {

                    var srchString = strAllowedRole;
                    var strFound = Array.FindAll(roles, str => str.ToLower().Trim().Equals(srchString.ToLower().Trim()));
                    if (strFound.ToList().Count > 0)
                    {
                        //if role exists in the roles for the particular user, set flag allow to true
                        isAuthorised = true;
                        Session["authority"] = "true";
                        break;
                    }


                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
                objUserInfo.IsAuthenticated = true;
                //Add the User Object to Session
                Session.Add("objUserInfo", objUserInfo);

            }
        }
        else
        {
            Response.Redirect("Login.aspx", false);
        }

    }

    void LoadModules()
    {
        try
        {
            if (objLocInfo.DataPointInfo != null && !IsEmpty<DataPointInfo>(objLocInfo.DataPointInfo))
            {

                if (!hasLoadedControls)
                {
                    hasLoadedControls = true;
                    if (alNodeList == null)
                        objSvcPre.getNodeList("servicePage");

                    if (alNodeList != null)
                    {
                        foreach (string[] node in alNodeList)
                        {
                            switch (node[0].ToString().ToLower())
                            {
                                case "chart":
                                    common.LoadCtrl<WebChart>(this.Page, CentrePlaceHolder, "WebChart", node[1].ToString(), "");
                                    /* Agricast CR - R6 - Add wind icons and legend for Humidity - Begin */
                                    //common.LoadCtrl(this.Page, CentrePlaceHolder, "WebChartIcons");
                                    /* Agricast CR - R6 - Add wind icons and legend for Humidity - End */
                                    break;
                                case "icon":
                                    common.LoadCtrl<Icon>(this.Page, CentrePlaceHolder, "Icon", node[1].ToString(), "");
                                    break;
                                case "tbldaysrows":
                                    common.LoadCtrl<Tables>(this.Page, CentrePlaceHolder, "Tables", node[1].ToString(), node[0].ToString());
                                    break;
                                case "tblseriesrows":
                                    common.LoadCtrl<Tables>(this.Page, CentrePlaceHolder, "Tables", node[1].ToString(), node[0].ToString());
                                    break;
                                case "agriinfo":
                                    common.LoadCtrl<AgriInfo>(this.Page, CentrePlaceHolder, "AgriInfo", node[1].ToString(), node[0].ToString());
                                    break;
                                case "legend":
                                    DisplayLegend(node[0].ToString(), node[1].ToString());
                                    break;

                                /*Wind Icon as a Sepearate component -- BEGIN*/
                                case "windicon": common.LoadCtrl<WebChartIcons>(this.Page, CentrePlaceHolder, "WebChartIcons", node[1].ToString(), node[0].ToString());
                                    break;
                                /*Wind Icon as a Sepearate component -- END*/
                                default:
                                    break;
                            }
                        }

                        //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - start
                        foreach (Control ctl in CentrePlaceHolder.Controls)
                        {
                            if (ctl is Tables)
                            {
                                Tables t = (Tables)ctl;

                                t.SeriesGridViewDataBind += t_SeriesGridViewDataBind;
                            }
                        }
                        //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - end
                    }                    
                }
            }
            else
            {
                DropDownList dropdown = (DropDownList)common.FindControlRecursive(this, "ddlCountry");
                if (objLocInfo.CountryCode != null && objLocInfo.CountryCode != "")
                {
                    if (dropdown != null && dropdown.SelectedValue.ToString() != "")
                    {
                        if (Session["CountryNotPresent"] != null)
                        {
                            if (Session["CountryNotPresent"].ToString().ToLower() != "true")
                                dropdown.SelectedValue = (objLocInfo.CountryCode.ToUpper() != null && objLocInfo.CountryCode.ToUpper() != "") ? objLocInfo.CountryCode.ToUpper() : dropdown.SelectedValue.ToString();
                            else
                                objLocInfo.CountryCode = dropdown.SelectedValue;
                        }
                    }
                }
                HtmlInputControl TextBoxSearch = (HtmlInputControl)common.FindControlRecursive(this, "searchbox");
                if (TextBoxSearch != null)
                    TextBoxSearch.Value = objLocInfo.searchLocation;
            }

        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.DEF_LOADMODULES_FAILURE) + " : " + ex.Message.ToString();
            return;


        }
    }

    //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - start
    void t_SeriesGridViewDataBind(object sender, EventArgs e)
    {
        GridView gv = sender as GridView;
        //var svc = (service)Session["service"];
        List<string[]> objList = sh.getNodeList("servicePage");

         for (int i = 0; i < objList.Count; i++)
         {
             if (objList[i].Contains("tblSeriesRows"))
             {
                 if (gv != null && gv.Rows.Count > 0)
                 {
                     DataSet dsTblSeries = sh.GetTableSeries("tblSeriesRows", objList[i][1].ToString());
                     if (dsTblSeries.Tables[0].Columns.Contains("isInvisible"))
                     {
                         //if (dsTblSeries.Tables["tblSeriesRows"].Columns.Contains("isInvisible"))
                         for (int j = 0; j < dsTblSeries.Tables[0].Rows.Count; j++)
                         {
                             if (bool.Parse(dsTblSeries.Tables[0].Rows[j]["isInvisible"].ToString()))
                             {
                                 gv.Rows[j].Style.Add(HtmlTextWriterStyle.Display, "none");
                             }
                         }
                     }
                 }
                 break;
             }
         }       
    }
    //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - end

    public void DisplayLegend(string node, string name)
    {
        try
        {
            objSvcPre = new ServicePresenter(this);
            objSvcPre.DisplayLegend(node, name);
            //load only if station is selected
            if (dtLegenddetails != null && objLocInfo.DataPointInfo != null && (objLocInfo.DataPointInfo.stationLatitude != 0.0 || objLocInfo.DataPointInfo.stationLongitude != 0.0))
            {
                Label lblLegend = new Label();
                if (dtLegenddetails.Rows[0][1].ToString() != "")
                    objSvcPre.getTransText(dtLegenddetails.Rows[0][1].ToString());
                lblLegend.Text = TransString ?? "";
                lblLegend.CssClass = LABEL250;
                lblLegend.ID = "lbl_" + name;
                StringBuilder sbBody = new StringBuilder();
                StringBuilder sb1;
                StringBuilder sb;
                string legendPath = string.Empty;

                /*IM01813796 - New Agricast - location of legend files - Begin */
                //if (name.Equals("Banner"))
                //{
                //    legendPath = "Pub\\" + objSvcInfo.ServiceName + "\\" + dtLegenddetails.Rows[0][2].ToString().ToString();
                //}
                //else
                //{
                //    legendPath = dtLegenddetails.Rows[0][2].ToString().ToString();
                //}
                legendPath = "Pub\\" + objSvcInfo.ServiceName + "\\" + dtLegenddetails.Rows[0][2].ToString().ToString();
                if (!File.Exists(HttpRuntime.AppDomainAppPath + legendPath))
                    legendPath = dtLegenddetails.Rows[0][2].ToString().ToString();

                //if (!File.Exists(HttpRuntime.AppDomainAppPath + legendPath))
                //    throw new Exception("Legend file not found.");
                /*IM01813796 - New Agricast - location of legend files - End */

                using (StreamReader sr = new StreamReader(HttpRuntime.AppDomainAppPath + legendPath))
                {
                    String line;
                    // Read and display lines from the file until the end of  the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {

                        sbBody.Append(line);

                    }
                    sb1 = new StringBuilder(sbBody.ToString().Substring(sbBody.ToString().IndexOf("<div"), (sbBody.ToString().IndexOf("</div>") - sbBody.ToString().IndexOf("<div") + 6)));

                    string strLegend = sb1.ToString();
                    //get delimiters from web.config
                    string startDelimiter = ConfigurationManager.AppSettings["startDelimiter"] != null && ConfigurationManager.AppSettings["startDelimiter"] != string.Empty ? ConfigurationManager.AppSettings["startDelimiter"] : "{";
                    string endDelimiter = ConfigurationManager.AppSettings["endDelimiter"] != null && ConfigurationManager.AppSettings["endDelimiter"] != string.Empty ? ConfigurationManager.AppSettings["endDelimiter"] : "}";
                    while (strLegend.IndexOf(endDelimiter) != -1)
                    {
                        int start = strLegend.IndexOf(startDelimiter);
                        int end = strLegend.IndexOf(endDelimiter);

                        string text = strLegend.Substring(start, end - start + startDelimiter.Count());
                        if (text != string.Empty)
                        {
                            objSvcPre.getTransText(text.Substring(startDelimiter.Count(), text.Length - (endDelimiter.Count() + startDelimiter.Count())));
                            strLegend = strLegend.Remove(start) + TransString + strLegend.Substring(end + endDelimiter.Count());
                        }

                    }
                    sb = new StringBuilder(strLegend);
                }
                HtmlGenericControl divAdditional = new HtmlGenericControl("div");


                HtmlGenericControl div1 = new HtmlGenericControl("div");
                div1.ID = "divLegend_" + name;
                LiteralControl literal = new LiteralControl(sb.ToString());
                literal.ID = "legend_" + name;
                if (bool.Parse(dtLegenddetails.Rows[0]["collapsible"].ToString()))
                {
                    System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                    img.ImageUrl = BOXMINUS;
                    img.ID = "img1_" + name;
                    img.ClientIDMode = ClientIDMode.Static;
                    divAdditional.Controls.Add(img);
                }

                divAdditional.Controls.Add(lblLegend);
                div1.Controls.Add(literal);
                divAdditional.Controls.Add(div1);
                divAdditional.Attributes.Add("class", "Legend");
                CentrePlaceHolder.Controls.Add(divAdditional);
            }
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.DEF_DISPLAYLEGENDS_FAILURE) + " : " + ex.Message.ToString();
            return;


        }
    }

    private void Page_prerender(object sender, EventArgs e)
    {
        try
        {
            setFooter();
            if (!hasLoadedControls)
            {
                objLocInfo.DataPointInfo = DataPointInfo.getDataPointObject;
                LoadModules();
            }

            if (wls != null && tb != null)
            {
                wls.getCultureCode(objSvcInfo.Culture);
                tb.changeLabelText(objSvcInfo.Culture);
            }
            CreateCookies();
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(objSvcInfo.Culture);

        }
        catch (Exception ex)
        {
            objSvcPre = new ServicePresenter();
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.GENERIC_ERRORONPAGE) + ":" + ex.Message.ToString();
        }
        finally
        {
            if (Session["AuditData"] != null)
            {
                objSvcPre.SaveAuditData();
                //AgriCastException currEx = new AgriCastException(null,(IDictionary)Session["AuditData"]);
                //AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error, "Audit");
            }
        }
    }
    protected override void Render(HtmlTextWriter writer)
    {

        if (HttpContext.Current.Session["ErrorMessage"] != null)
        {
            lblError.Text = HttpContext.Current.Session["ErrorMessage"].ToString();
            if (HttpContext.Current.Session["ErrorMessage"].ToString().ToLower().Contains("success"))
            {
                lblError.CssClass = "Success";
            }



        }
        else
        {
            if (HttpContext.Current.Session["SuccessMessage"] != null)
            {
                lblError.Text = HttpContext.Current.Session["SuccessMessage"].ToString();
                lblError.CssClass = "Success";
            }
        }

        //if no stations are found, clear all controls from the page.
        objDataPointInfo = DataPointInfo.getDataPointObject;
        if (objDataPointInfo == null || (objDataPointInfo.stationLatitude == 0.0 && objDataPointInfo.stationLongitude == 0.0))
        {
            CentrePlaceHolder.Controls.Clear();
        }


        //Either Print or Email is Clicked
        if (blGenerateHtml)
        {
            String html = string.Empty;
            string strNewOutput = string.Empty;
            StringWriter output = new StringWriter();
            base.Render(new HtmlTextWriter(output));
            html = output.ToString();

            html = html.Replace("<div id=\"divPrintPopup\" class=\"hide\"><iframe src=\"\" id=\"iframePrintPopup\" style=\"width:100%;height:100%\" ></iframe></div>", " ");


            strNewOutput = output.ToString();

            //Remove Header and Footer in case of Print
            string strSub = html.Substring(html.IndexOf("data-key=\"test\">"));

            strSub = strSub.Substring(0, strSub.IndexOf("</span>"));

            //Create the html file
            CreateHtmlForPrintandEMail(html);

            if (isEmail)
            {
                GetPDF();
            }


            if (HttpContext.Current.Session["ErrorMessage"] != null)
            {
                //strSub = "key=\"test\">" + HttpContext.Current.Session["ErrorMessage"].ToString();
                //Replace the Place holder text within error label sapn tag
                //strNewOutput = output.ToString().Replace("key=\"test\">",strSub );
                strNewOutput = output.ToString().Replace(strSub, "data-key=\"test\">" + HttpContext.Current.Session["ErrorMessage"].ToString());

                lblError.Text = HttpContext.Current.Session["ErrorMessage"].ToString();
                if (HttpContext.Current.Session["ErrorMessage"].ToString().Contains("successfully"))
                {
                    //lblError.CssClass = "Success";
                    strNewOutput = strNewOutput.Replace("<span id=\"lblError\" class=\"Error\"", "<span id=\"lblError\" class=\"Success\"");
                }

                //fix email text display issue.
                output.Write(strNewOutput);
            }
            else
            {
                if (HttpContext.Current.Session["SuccessMessage"] != null)
                {
                    strNewOutput = output.ToString().Replace(strSub, "data-key=\"test\">" + HttpContext.Current.Session["SuccessMessage"].ToString());

                    lblError.Text = HttpContext.Current.Session["SuccessMessage"].ToString();
                    strNewOutput = strNewOutput.Replace("<span id=\"lblError\" class=\"Error\"", "<span id=\"lblError\" class=\"Success\"");
                }
            }
            //Finally ,write the contents to the Page
            writer.Write(strNewOutput);


        }
        else
        {

            base.Render(writer);

        }

        HttpContext.Current.Session["ErrorMessage"] = "";
        HttpContext.Current.Session["ErrorMessage"] = null;
        HttpContext.Current.Session["SuccessMessage"] = "";
        HttpContext.Current.Session["SuccessMessage"] = null;




    }

    #endregion

    #region Methods

    //Method to check the Language
    protected void checkLanguage()
    {
        if (objSvcInfo.Culture == null || objSvcInfo.Culture == "")
        {
            objSvcInfo.Culture = objSvcPre.getCultureCode();
            //Added by Rahul
            if (!IsPostBack)
            {
                Session["PreviousCulture"] = objSvcInfo.Culture;
                Session["CurrentCulture"] = objSvcInfo.Culture;
            }
        }
        else
        {
            string LangId = common.FindControlRecursive(this, "ddlCulture").UniqueID;
            if (LangId != "")
            {
                ccode = Page.Request.Form[LangId];
                if (ccode != null && objSvcInfo.Culture != ccode)
                {
                    //Addded by Rahul 
                    Session["PreviousCulture"] = objSvcInfo.Culture;
                    objSvcInfo.Culture = ccode;
                    //Added by Rahul
                    Session["CurrentCulture"] = objSvcInfo.Culture;
                }

                //if (((service)(Session["Service"])).cultureSettings.allowedCultureCodes.ToList().Count > 0)
                //{
                //    int j = 0;
                //    for (int i = 0; i < ((service)(Session["Service"])).cultureSettings.allowedCultureCodes.ToList().Count; i++)
                //    {
                //        if (objSvcInfo.Culture.Trim().ToString() == ((service)(Session["Service"])).cultureSettings.allowedCultureCodes.ToList()[i].code.Trim().ToString())
                //        {
                //            j++;
                //            break;
                //        }

                //    }
                //    if (j != 1)
                //    {
                //        Session["PreviousCulture"] = objSvcInfo.Culture;
                //        objSvcInfo.Culture = ((service)(Session["Service"])).cultureSettings.allowedCultureCodes.ToList()[0].code.ToString();
                //        Session["CurrentCulture"] = objSvcInfo.Culture;

                //    }
                //}
            }
        }


        /*First Time Page load Takes a long time issue - BEGIN*/
        try
        {
            if (!Page.IsPostBack || ((Session["PreviousCulture"] != null && Session["CurrentCulture"] != null) && Session["PreviousCulture"] != Session["CurrentCulture"]))
                objCommonUtil.getTransTextForFirstTimeLoad(objSvcInfo.Culture);
        }
        catch (Exception ex)
        {

            HttpContext.Current.Session["ErrorMessage"] = "Error in First Time page load method : '" + ex.Message + "'";
        }
        /*First Time Page load Takes a long time issue - END*/

        if (Session["AuditData"] != null)
        {
            IDictionary dict = (IDictionary)Session["AuditData"];
            dict["culture"] = objSvcInfo.Culture;
            Session["AuditData"] = dict;
        }
    }

    public void SaveRatings()
    {
        List<string[]> lstRatings = new List<string[]>();
        lstRatings = (List<string[]>)HttpContext.Current.Session["Rating"];


        //List<string[]> al = new List<string[]>();
        //// ",rating_Relative humidity (%)#4,rating_Relative humidity (%)#3,rating_Relative humidity (%)#5,rating_Precipitation (mm)#5";
        //string[] Rate = rating.Split(',');
        //for (int i = 1; i < Rate.Count(); i++)
        //{
        //    string[] ratings = Rate[i].Split('#');
        //    al.Add(new string[] { ratings[0].ToString(), ratings[1].ToString(), objSvcInfo.Module.ToString() });
        //}

        objSvcPre.SaveRatings(lstRatings);
        HttpContext.Current.Session["Rating"] = null;

    }

    private void createMenu()
    {
        objSvcPre.loadMenuTabs();

        int num = dtPages.Rows.Count;
        for (int i = 0; i < num; i++)
        {
            HtmlGenericControl lt = new HtmlGenericControl();
            string liid = "tab_mresult_" + dtPages.Rows[i][0].ToString();
            lt.ID = liid;
            lt.TagName = "li";
            if (dtPages.Rows[i][0].ToString() == objSvcInfo.Module.ToString())
                lt.Attributes.Add("class", "active");
            lt.InnerHtml = "<a href=\"default2.aspx?module=" + dtPages.Rows[i][0].ToString() + "\">"
                + dtPages.Rows[i][1].ToString()
                + "</a>";

            MenuTabs.Controls.Add(lt);


        }
    }

    private void createMossMenu()
    {
        /*IM01184669 - New Agricast - redirection to a login page if the publication is protected - BEGIN*/
        //Pass the Moss Redirection Url
        //objSvcPre.GetMossMenuItems(ExtNavigation);
        objSvcPre.GetMossMenuItems(ExtNavigation, objSvcInfo.MossCultureCode);
        /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - END*/

        //intra.dev.syngenta.TopNavigation topNav = new intra.dev.syngenta.TopNavigation();
        //topNav.Url = "http://www-fba.syngentacom.dev.intra/_vti_bin/topnavigation.asmx";
        //dsMossMenu.ReadXml(topNav.ExposeNavigationInXML("syngenta.com", "france", "fr", "fr").ToString());
        if (dsMossMenu != null)
        {
            DataTable dtNode = dsMossMenu.Tables[0];
            DataTable dtChildNode = dsMossMenu.Tables[1];
            for (int i = 0; i < dtNode.Rows.Count; i++)
            {
                HtmlGenericControl ult = new HtmlGenericControl();
                ult.ID = "ul" + i;
                ult.TagName = "ul";

                HtmlGenericControl lt = new HtmlGenericControl();
                string liid = "tab_mresult_" + dtNode.Rows[i][2].ToString();
                lt.ID = liid;
                lt.TagName = "li";
                lt.InnerText = dtNode.Rows[i][2].ToString();
                lt.InnerHtml = "<a href=\"" + dtNode.Rows[i][1].ToString() + "\">"
                    + dtNode.Rows[i][2].ToString()
                    + "</a>";
                ult.Controls.Add(lt);
                if ((dtChildNode.Select("SiteMapNode_Id = " + i)).ToList().Count > 0)
                {
                    DataTable dtChild = dtChildNode.Select("SiteMapNode_Id = " + i).CopyToDataTable();
                    HtmlGenericControl ulChild = new HtmlGenericControl();
                    ulChild.ID = "ulChild" + i + "_" + dtChild.Rows[0][1];
                    ulChild.TagName = "ul";

                    for (int j = 0; j < dtChild.Rows.Count; j++)
                    {

                        HtmlGenericControl ltChild = new HtmlGenericControl();
                        string lid = "tab_moss_" + dtChild.Rows[j][1].ToString();
                        ltChild.ID = lid;
                        ltChild.TagName = "li";
                        ltChild.InnerText = dtChild.Rows[j][1].ToString();
                        ltChild.InnerHtml = "<a href=\"" + dtChild.Rows[j][0].ToString() + "\">"
                            + dtChild.Rows[j][1].ToString()
                            + "</a>";
                        ulChild.Controls.Add(ltChild);

                    }
                    lt.Controls.Add(ulChild);
                }

                MossMenuTab.Controls.Add(ult);
            }
        }

    }


    protected void GetPDF()
    {
        try
        {
            StringBuilder sbInputHtml = new StringBuilder(); ;
            StreamReader sr = null;
            StringWriter strWriter = null;
            string strPDFPath = string.Empty;
            StringBuilder sbMailBody = new StringBuilder();
            PdfConverter pdfConverter = null;
            Document document = null;

            //Generate File Name :- ServiceName- LocationName - DateTime

            LocationInfo objLocInfo = (LocationInfo)Session["objLocationInfo"];

            if (objLocInfo != null)
            {
                if (objLocInfo.ServiceInfo.ServiceName != "geotroll1")
                {
                    //Service Name
                    sFile = sFile + objLocInfo.ServiceInfo.ServiceName + "_";

                    //Location Name
                    sFile = sFile + objLocInfo.DataPointInfo.stationName.Replace(' ', '_');

                    //DateTime stamp
                    sFile = sFile + objCommonUtil.strReplace(DateTime.Now.ToString());

                    sFile = objCommonUtil.strReplace(sFile) + ".pdf";
                }
                else
                {
                    sFile = sFile + "HistoricWeather-" + objCommonUtil.strReplace(objLocInfo.DataPointInfo.stationName) + "-" + DateTime.Now.ToString("yyyy-MM-dd");
                    sFile = sFile + ".pdf";
                }
            }


            //form the PDF Path
            strPDFPath = Path.Combine(strFullPath_Nofile + "\\" + sFile);


            //Get the HTML created by CreatHtml Method
            sr = new StreamReader(strHTMLFilePath);
            strWriter = new StringWriter(sbInputHtml);
            strWriter.Write(sr.ReadToEnd());

            //create an instance of pdfconverter
            pdfConverter = new PdfConverter();

            if (ConfigurationManager.AppSettings.AllKeys.Contains("UserName") && ConfigurationManager.AppSettings.AllKeys.Contains("Password"))
            {
                //use the Account details which has fuill access on the site(all folders)
                AuthenticationOptions authOptions = new AuthenticationOptions();
                authOptions.Username = ConfigurationManager.AppSettings["Domain"].ToString() + "\\" + ConfigurationManager.AppSettings["UserName"].ToString();
                authOptions.Password = ConfigurationManager.AppSettings["Password"].ToString();

                //add the authentication options to PDF Converter
                pdfConverter.AuthenticationOptions.Username = authOptions.Username;
                pdfConverter.AuthenticationOptions.Password = authOptions.Password;
            }

            //add the License key
            pdfConverter.LicenseKey = ConfigurationManager.AppSettings["pdfLicence"].ToString();
            pdfConverter.ConversionDelay = 2;
            document = pdfConverter.GetPdfDocumentObjectFromHtmlString(sbInputHtml.ToString());
            pdfConverter.PdfDocumentOptions.FitWidth = true;
            pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true;


            // HttpResponse Response = Page.Response;

            //document.Save(Response, false, "TradeImportRegularization.pdf");

            document.Save(strPDFPath);

            strAttachmentpath = "";

            //Pick the Stored PDF and send an email

            //To Field
            strTo = Session["EMailTo"].ToString();

            //From field
            strFrom = Session["EMailFrom"].ToString();

            //Subject Field
            strSubject = Session["EMailSubject"].ToString();

            //Mail Body
            strMailBody = Session["EMailBody"].ToString();


            //Form the Stored PDF File Path Location
            strAttachmentpath = strPDFPath;

            mail = new MailMessage(strFrom.ToString(), strTo.ToString());
            if (strAttachmentpath != "" && strAttachmentpath != null)
            {
                attachFile = new Attachment(strAttachmentpath);

                mail.Attachments.Add(attachFile);

                //Send the PDF inline
                //attachFile.ContentDisposition.Inline = true;
                attachFile.ContentDisposition.DispositionType = DispositionTypeNames.Attachment;
                //attachFile.ContentId = "PDF"; 
                attachFile.ContentType.MediaType = "application/pdf";


            }

            sbMailBody = new StringBuilder(strMailBody);

            sbMailBody.Append(sbInputHtml.ToString());
            mail.Subject = strSubject;
            mail.Body = strMailBody;
            //sbMailBody.ToString();
            mail.IsBodyHtml = true;
            smtpClientObj = new SmtpClient(smtpServer);
            //smtpClientObj.Host = "localhost";
            //smtpClientObj.Port = 25;
            smtpClientObj.Send(mail);

            //If mail sent, show a message to user.
            objSvcPre.getTransText(Constants.EMAIL_SUCCESS);
            //HttpContext.Current.Session["ErrorMessage"] = TransString;
            HttpContext.Current.Session["SuccessMessage"] = TransString;
            //HtmlInputControl Tomail = (HtmlInputControl)common.FindControlRecursive(this, "Tomail");
            //Tomail.Value = string.Empty;
            //HtmlInputControl Subject = (HtmlInputControl)common.FindControlRecursive(this, "Subject");
            //Subject.Value = string.Empty;
            //HtmlTextArea txtBody = (HtmlTextArea)common.FindControlRecursive(this, "txtBody");
            //txtBody.Value = string.Empty;

            //Reset the Is Email Flag
            //isEmail = false; //email flag reset in toolbar render once controls are cleared
        }
        //catch (SmtpException)
        //{
        //    objSvcPre.getTransText("emailsuccess");
        //    HttpContext.Current.Session["ErrorMessage"] =TransString ;
        //}
        catch (Exception)
        {
            objSvcPre.getTransText(Constants.EMAIL_FAILURE);
            if (!string.IsNullOrEmpty(TransString))
                HttpContext.Current.Session["ErrorMessage"] = TransString;
            else
                HttpContext.Current.Session["ErrorMessage"] = "An Error Occured in sending mail";
        }
    }


    protected void CreateHtmlForPrintandEMail(string pHTML)
    {
        FileStream fsHtml = null;
        string strHTML = string.Empty;
        StreamWriter strWriter = null;
        string strPathToReplace = string.Empty;
        StreamReader srReadCSSFiles = null;
        string strCommonCSS = "~\\styles\\Common.css";
        string strJqueryCSS = "~\\styles\\jqueryslidemenu.css";
        string strRateIt = "~\\styles\\RateIt.css";
        string strSite = "~\\styles\\Site.css";
        string strUI = "~\\styles\\ui-lightness\\jquery-ui-1.8.17.custom.css";
        string strPrint = "~\\styles\\Print.css";

        filename = "EmailPage" + objCommonUtil.strReplace(DateTime.Now.ToString()) + ".html";
        tempFolderPath = ConfigurationManager.AppSettings["tempfolder"];
        rootPath = HttpRuntime.AppDomainAppPath.ToString() + tempFolderPath;

        strFullPath_Nofile = System.IO.Path.Combine(rootPath, Constants.EMAILPDFFOLDER);
        strHTMLFilePath = strFullPath_Nofile + "/" + filename;

        //Add this to Session - use it in Print Page
        Session["htmlpagepath"] = strHTMLFilePath;

        if (!Directory.Exists(strFullPath_Nofile))
        {
            Directory.CreateDirectory(strFullPath_Nofile);
        }

        ////Save the HTML
        fsHtml = new FileStream(strHTMLFilePath, FileMode.Create, FileAccess.ReadWrite);

        //assign the Input HTML
        strHTML = pHTML;

        strWriter = new StreamWriter(fsHtml, Encoding.UTF8);

        strPathToReplace = "http://" + HttpContext.Current.Request.Url.Authority + (HttpRuntime.AppDomainAppVirtualPath.ToString() != "" ? (HttpRuntime.AppDomainAppVirtualPath.ToString() + "/") : "");

        //Replace the relative path to absolute
        strHTML = strHTML.Replace("../../", strPathToReplace);

        //add the header html with details
        string distText = objCommonUtil.getTransText("ResDistanceFrom");

        distText = distText.Replace("{Dir}", objDataPointInfo.directionLetter + " ");
        distText = distText.Replace("{CityName}", HttpUtility.HtmlDecode(objLocInfo.placeName) + " ");
        //convert Distance and Elevation to Miles and Feet respectively for 
        //culture en-Us and sl-SI
        if (objSvcInfo.Culture.ToLower().Equals("en-us", StringComparison.CurrentCultureIgnoreCase) || objSvcInfo.Culture.ToLower().Equals("sl-SI", StringComparison.CurrentCultureIgnoreCase))
        {
            distText = distText.Replace("{Dist}", objCommonUtil.ConvertValueMetrics(objDataPointInfo.distance.ToString(), "km-mi"));
            distText = distText.Replace("{Elevation}", objCommonUtil.ConvertValueMetrics(objDataPointInfo.altitude.ToString(), "m-ft"));
        }
        else
        {
            distText = distText.Replace("{Dist}", objDataPointInfo.distance.ToString() + " ");
            distText = distText.Replace("{Elevation}", objDataPointInfo.altitude.ToString() + " ");
        }
        string strHeader = objCommonUtil.getTransText("Weather for") + " " + Server.HtmlDecode(objDataPointInfo.stationName) + " " + distText;

        /*IM01294340-New Agricast - printing on AgriInfo - remove the selected conditions - Begin */
        //code commented for IM01294340
        //if the module is agriinfo
        /*  if (objLocInfo.ServiceInfo != null && objLocInfo.ServiceInfo.Module.ToLower().Contains("history"))
          {

              if (Session["IAgriInfo"] != null)
              {
                  Dictionary<string, string> objAgriInfo = (Dictionary<string, string>)Session["IAgriInfo"];
                  //add the start date,end date and aggragation to the header



                  strHeader = strHeader + "</br>" + objCommonUtil.getTransText("Rep_From") + " : " + objAgriInfo["startDate"] + " " + objCommonUtil.getTransText("Rep_To") + " : " + (DateTime.Parse(objAgriInfo["endDate"])).ToShortDateString() + ", " + objCommonUtil.getTransText("Aggregation") + " : " + objAgriInfo["aggregation"];
              }

          }*/
        /*IM01294340-New Agricast - printing on AgriInfo - remove the selected conditions - End */

        //Replace the label text with the new text
        strHTML = strHTML.Replace("PRINTHEADER", strHeader);

        //Add the CSS Content to the HTMl file
        StringBuilder sbHtml = new StringBuilder(strHTML);

        //Read the Common . css

        StringBuilder sbCss = new StringBuilder();
        srReadCSSFiles = new StreamReader(Server.MapPath(strCommonCSS));
        sbCss.Append(srReadCSSFiles.ReadToEnd());


        //Read the Jquery CSS
        srReadCSSFiles = new StreamReader(Server.MapPath(strJqueryCSS));
        sbCss.Append(srReadCSSFiles.ReadToEnd());

        //Read the Rate it CSS
        srReadCSSFiles = new StreamReader(Server.MapPath(strRateIt));
        sbCss.Append(srReadCSSFiles.ReadToEnd());

        //Read the Site CSS
        srReadCSSFiles = new StreamReader(Server.MapPath(strSite));
        sbCss.Append(srReadCSSFiles.ReadToEnd());

        //Read the Jquery Folder CSS
        srReadCSSFiles = new StreamReader(Server.MapPath(strUI));
        sbCss.Append(srReadCSSFiles.ReadToEnd());

        //Read the print CSS
        srReadCSSFiles = new StreamReader(Server.MapPath(strPrint));
        sbCss.Append(srReadCSSFiles.ReadToEnd());


        /* for IM01246228 - New Agricast - Print is not working - Jerrey - Start*/
        string cssForPub = string.Format("~\\pub\\{0}\\", pubname);
        foreach (var cssFile in Directory.GetFiles(Server.MapPath(cssForPub), "*.css"))
        {
            srReadCSSFiles = new StreamReader(cssFile);
            sbCss.Append(srReadCSSFiles.ReadToEnd());
        }
        /* for IM01246228 - New Agricast - Print is not working - Jerrey - End*/

        sbHtml.Replace("DYNAMICCSS", sbCss.ToString());

        //Assign it to string
        strHTML = sbHtml.ToString();

        //write the contents with CSS to HTML File
        strWriter.Write(strHTML);

        strWriter.Flush();
        strWriter.Close();

        //close the streamreaders.
        srReadCSSFiles.Close();
        srReadCSSFiles.Dispose();
        //Reset the Generate HTMl flag
        blGenerateHtml = false;

    }

    private void checkPub()
    {
        ServiceInfo.ServiceConfig = objSvcInfo;
        objLocInfo = LocationInfo.getLocationInfoObj;
        string RedirectedUrl = null;
        string url = string.Empty;
        if (HttpContext.Current.Items.Contains("RedirectedUrl"))
        {
            RedirectedUrl = HttpContext.Current.Items["RedirectedUrl"].ToString();
        }

        if (RedirectedUrl.Contains("?pub="))
        {
            int startPosition = RedirectedUrl.IndexOf("=") + 1;
            int endPosition;
            if (RedirectedUrl.Contains("&"))
            {
                endPosition = RedirectedUrl.IndexOf("&");
                pubname = RedirectedUrl.Remove(endPosition).Substring(startPosition);
            }
            else
            {
                pubname = RedirectedUrl.Substring(startPosition);

            }
        }

        /*MOSS SITE MYSYN01FAR ISSUE- begin*/
        string prePubname = string.Empty;
        string sPubName = string.Empty;
        //Checking previous servicename 
        if (Session["service"] != null)
            sPubName = ((service)(Session["service"])).name;
        if (objSvcInfo.ServiceName.Equals((sPubName)))
            prePubname = objSvcInfo.ServiceName;

        else
            prePubname = sPubName;


        /*MOSS SITE MYSYN01FAR ISSUE- End*/


        //Checking previous servicename
        //if (objSvcInfo.ServiceName.ToUpper() == ((service)(Session["service"])).name.ToUpper())
        //    prePubname = objSvcInfo.ServiceName;
        //else
        //    prePubname = ((service)(Session["service"])).name;
        //if null or different, assigning current pubname to session variable
        if ((prePubname == "") || (prePubname != pubname))
        {
            ispubchange = true;
            if (HttpContext.Current.Session["Rating"] != null)
            {
                SaveRatings();

            }

            objSvcInfo.ServiceName = pubname;
            service svc;// = (service)Session["service"];
            //if (svc == null)
            //{

            //set service to null and read config again
            svc = null;
            Session["service"] = null;
            try
            {
                objSvcPre.createServiceSession();
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                Response.Write("<html><head><body style=\"font-weight:100; color:Red;font-size:large\" >");
                Response.Write(Session["ErrorMessage"] + ex.Message.ToString());
                Response.Write("</body></head></html>");
                Response.End();

            }
            svc = (service)Session["service"];
            //}

            //set the default page if the pub is changed or the pub is null
            objSvcPre.getDefaultPage();
            objSvcInfo.Module = (DefaultTab != null || DefaultTab == "") ? DefaultTab : DEFAULTMODULE;

            /* IM01704703 - New Agricast - taking previous language even if incompatible with service.config - Added by Jerrey - Begin */
            objSvcInfo.Culture = objSvcPre.getCultureCode();
            /* IM01704703 - New Agricast - taking previous language even if incompatible with service.config - Added by Jerrey - End */

            if (Request.QueryString["module"] != null)
            {
                url = HttpContext.Current.Request.Url.ToString();
                url = url.Split('?')[0];
                System.Collections.Specialized.NameValueCollection t = HttpUtility.ParseQueryString(Request.Url.Query);
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                if (string.IsNullOrWhiteSpace(t["module"]))
                    t.Set("module", objSvcInfo.Module);
                else
                {
                    if (objSvcPre.checkModule(Request.QueryString["module"].ToString()))
                        objSvcInfo.Module = Request.QueryString["module"];
                    else
                        objSvcInfo.Module = (DefaultTab != null || DefaultTab == "") ? DefaultTab : DEFAULTMODULE;
                }
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
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

            //Get the UserInfo Object

            UserInfo objUserInfo = (UserInfo)Session["objUserInfo"];
            if (objUserInfo != null)
            {
                //objUserInfo.IsAuthenticated = false;
                objUserInfo.PrePubName = prePubname;
                Session["objUserInfo"] = objUserInfo;
            }

        }
    }
    private void readPubnameQueryStrings()
    {
        try
        {

            objSvcPre = new ServicePresenter(this);
            //Reading query strings(if any) and adding values to the session 
            if (Request.QueryString["module"] != null)
            {
                if (objSvcInfo.Module != Request.QueryString["module"])
                {
                    if (HttpContext.Current.Session["Rating"] != null)
                    {
                        SaveRatings();

                    }
                }
                if (!ispubchange)
                {
                    if (objSvcPre.checkModule(Request.QueryString["module"].ToString()))
                        objSvcInfo.Module = Request.QueryString["module"];
                    else
                    {
                        objSvcPre.getDefaultPage();
                        objSvcInfo.Module = (DefaultTab != null || DefaultTab == "") ? DefaultTab : DEFAULTMODULE;

                    }
                }
            }


            if (Request.QueryString["Culture"] != null)
            {
                objSvcInfo.Culture = Request.QueryString["Culture"].Trim();
            }

            if (Request.QueryString["Unit"] != null)
            {
                objSvcInfo.Unit = Request.QueryString["Unit"];
            }

            //////location search logic/////
            if (Request.QueryString["Overridecookie"] != null)
                strOverridecookie = Request.QueryString["Overridecookie"].ToLower().ToString();

            if (Request.QueryString["Location"] != null)
                /* IM01377132 - New Agricast - Default location : should keep the capital letters - Jerrey - Begin */
                strLocation = Request.QueryString["Location"].ToString(); //.ToLower().ToString();
            /* IM01377132 - New Agricast - Default location : should keep the capital letters - Jerrey - End */

            if (Request.QueryString["Lat"] != null)
                strLat = Request.QueryString["Lat"].ToString();

            if (Request.QueryString["Long"] != null)
                strLong = Request.QueryString["Long"].ToString();


            if (string.IsNullOrEmpty(strOverridecookie) || strOverridecookie == "true")
            {
                if (!string.IsNullOrEmpty(strLat) && !string.IsNullOrEmpty(strLong))
                {
                    double lat = 0.0;
                    double lang = 0.0;
                    lat = double.TryParse(Request.QueryString["Lat"].ToString(), out lat) ? lat : 0.0;
                    lang = double.TryParse(Request.QueryString["Long"].ToString(), out lang) ? lang : 0.0;
                    string strCountryCode = objCommonUtil.CheckLatLongForCountry(double.Parse(lat.ToString()), double.Parse(lang.ToString()));
                    if (checkCountryValidity(lat, lang))
                    {
                        //take the first two characters
                        if (!string.IsNullOrEmpty(strCountryCode))
                            strCountryCode = strCountryCode.Substring(0, 2).ToUpper();

                        objSvcInfo.Country = strCountryCode;
                        objLocInfo.CountryCode = strCountryCode;

                        objLocInfo.latitude = lat;
                        objLocInfo.longitude = lang;

                        if (string.IsNullOrEmpty(strLocation))
                            objLocInfo.placeName = "Lat: " + objLocInfo.latitude + " Long: " + objLocInfo.longitude;
                        else
                            objLocInfo.placeName = strLocation;

                        //Clearing off datapoint object in case search crtieria is given in query string
                        objDataPointInfo = null;
                        objLocInfo.DataPointInfo = null;

                        Session["objDataPointInfo"] = null;

                    }
                    else if (objLocInfo != null)
                    {
                        if (!checkCountryValidity(objLocInfo.latitude, objLocInfo.longitude))
                        {
                            objLocInfo.latitude = 0.0;
                            objLocInfo.longitude = 0.0;
                            objLocInfo.placeName = "";
                            //Clearing off datapoint object in case search crtieria is given in query string
                            objDataPointInfo = null;
                            objLocInfo.DataPointInfo = null;

                            Session["objDataPointInfo"] = null;
                        }
                    }
                }
                else if (objLocInfo != null)
                {
                    if (!checkCountryValidity(objLocInfo.latitude, objLocInfo.longitude))
                    {
                        objLocInfo.latitude = 0.0;
                        objLocInfo.longitude = 0.0;
                        objLocInfo.placeName = "";
                        //Clearing off datapoint object in case search crtieria is given in query string
                        objDataPointInfo = null;
                        objLocInfo.DataPointInfo = null;

                        Session["objDataPointInfo"] = null;
                    }

                }

            }

            else
            {
                if (objLocInfo != null)
                {
                    if (!string.IsNullOrEmpty(strLat) && !string.IsNullOrEmpty(strLong) && !checkCountryValidity(objLocInfo.latitude, objLocInfo.longitude))
                    {
                        double lat = 0.0;
                        double lang = 0.0;
                        lat = double.TryParse(Request.QueryString["Lat"].ToString(), out lat) ? lat : 0.0;
                        lang = double.TryParse(Request.QueryString["Long"].ToString(), out lang) ? lang : 0.0;
                        string strCountryCode = objCommonUtil.CheckLatLongForCountry(double.Parse(lat.ToString()), double.Parse(lang.ToString()));
                        if (checkCountryValidity(lat, lang))
                        {
                            //take the first two characters
                            if (!string.IsNullOrEmpty(strCountryCode))
                                strCountryCode = strCountryCode.Substring(0, 2).ToUpper();

                            objSvcInfo.Country = strCountryCode;
                            objLocInfo.CountryCode = strCountryCode;

                            objLocInfo.latitude = lat;
                            objLocInfo.longitude = lang;

                            if (string.IsNullOrEmpty(strLocation))
                                objLocInfo.placeName = "Lat: " + objLocInfo.latitude + " Long: " + objLocInfo.longitude;
                            else
                                objLocInfo.placeName = strLocation;

                            //Clearing off datapoint object in case search crtieria is given in query string
                            objDataPointInfo = null;
                            objLocInfo.DataPointInfo = null;

                            Session["objDataPointInfo"] = null;

                        }

                    }
                }
            }



            //Setting pub name and module name for audit data logging
            if (Session["AuditData"] != null)
            {
                IDictionary dict = (IDictionary)Session["AuditData"];
                dict["service"] = objSvcInfo.ServiceName;
                dict["module"] = objSvcInfo.Module;
                Session["AuditData"] = dict;
            }
            //changes for user management
            if (Request.QueryString["username"] != null && Request.QueryString["token"] != null)
            {
                if (Request.QueryString["token"].ToLower() == "agricast")
                {           
                    objSvcPre.GetEncryptionKey();
                    if (!string.IsNullOrEmpty(encryptKey))
                    {
                        try
                        {
                            string strUser =  Syngenta.Agriweb.SynCryptography.SynCryptographyConsumer.Decrypt((HttpUtility.UrlDecode(Request.QueryString["username"])), encryptKey, "Agricast");
                          
                            bool existsUser = Membership.FindUsersByName(strUser).Count > 0 ? true : false;
                            if (!existsUser)
                            {
                                Membership.CreateUser(strUser, "password");
                            }
                            UserInfo objUserInfo = (UserInfo)Session["objUserInfo"];
                            if (objUserInfo == null) objUserInfo = new UserInfo();
                            if (objUserInfo != null)
                            {
                                objUserInfo.IsAuthenticated = true;
                                isAuthorised = true;
                                Session["authority"] = "true";
                                objUserInfo.UserName = strUser;
                                Session["objUserInfo"] = objUserInfo;
                            }
                        }
                        catch (Exception ex)
                        {
                            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.GEN_USERNAME_DECRYPT);
                        }
                    }
                    else
                    {
                        HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.GEN_USERNAME_DECRYPT);
                    }
                }
                else
                {
                    Session["authority"] = "false";
                }
            }


            /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
            if (Request.QueryString["Moss"] != null && Request.QueryString["Moss"].ToString() == "true")
            {
                objSvcInfo.Moss = "true";
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                UpdateValueToIAgriInfo("altitude", "0");
                UpdateValueToIAgriInfo("aggregation", "daily");
                if (Request.QueryString["Lat"] != null && Request.QueryString["Long"] != null)
                {
                    //Clearing off datapoint object in case search crtieria is given in query string
                    objDataPointInfo = null;
                    objLocInfo.DataPointInfo = null;
                    Session["objDataPointInfo"] = null;
                    double lat = 0.0;
                    double lang = 0.0;
                    lat = double.TryParse(Request.QueryString["Lat"].ToString(), out lat) ? lat : 0.0;
                    lang = double.TryParse(Request.QueryString["Long"].ToString(), out lang) ? lang : 0.0;

                    objLocInfo.latitude = Convert.ToDouble(Request.QueryString["Lat"]);
                    objLocInfo.longitude = Convert.ToDouble(Request.QueryString["Long"]);
                    if (Request.QueryString["Location"] == null)
                        objLocInfo.placeName = "Lat: " + objLocInfo.latitude + " Long: " + objLocInfo.longitude;

                    else
                        objLocInfo.placeName = Request.QueryString["Location"].ToString();
                }

                if (!string.IsNullOrEmpty(Request.QueryString["startDate"])
                    && !string.IsNullOrEmpty(Request.QueryString["endDate"]))
                {
                    string startDateString = Request.QueryString["startDate"];
                    string endDateString = Request.QueryString["endDate"];
                    string[] startDateArr = startDateString.Split(new char[] { '-' });
                    string[] endDateArr = endDateString.Split(new char[] { '-' });
                    if (startDateString.IndexOf('-') > 0)
                        startDateString = string.Format("{0}/{1}/{2}", startDateArr[1], startDateArr[2], startDateArr[0]);
                    if (endDateString.IndexOf('-') > 0)
                        endDateString = string.Format("{0}/{1}/{2}", endDateArr[1], endDateArr[2], endDateArr[0]);

                    UpdateValueToIAgriInfo("startDate", startDateString);
                    UpdateValueToIAgriInfo("endDate", endDateString);
                }

                if (!string.IsNullOrEmpty(Request["TopSoilWHC"])
                   && !string.IsNullOrEmpty(Request["TopSoilDepth"])
                   && !string.IsNullOrEmpty(Request["SubSoilWHC"])
                   && !string.IsNullOrEmpty(Request["SubSoilDepth"]))
                {
                    UpdateValueToIAgriInfo("TopSoilWHC");
                    UpdateValueToIAgriInfo("TopSoilDepth");
                    UpdateValueToIAgriInfo("SubSoilWHC");
                    UpdateValueToIAgriInfo("SubSoilDepth");
                }

                if (!string.IsNullOrEmpty(Request["Location"]))
                {
                    service svc = null;
                    if (Session["service"] == null)
                    {
                        try
                        {
                            objSvcPre.createServiceSession();
                        }
                        catch (Exception ex)
                        {
                            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                            Response.Write("<html><head><body style=\"font-weight:100; color:Red;font-size:large\" >");
                            Response.Write(Session["ErrorMessage"] + ex.Message.ToString());
                            Response.Write("</body></head></html>");
                            Response.End();

                        }
                        svc = (service)Session["service"];
                    }
                    else
                        svc = (service)Session["service"];

                    var locStr = HttpUtility.HtmlEncode(Request["Location"]);
                    svc.locationSearch.defaultLocation = locStr;
                    Session["service"] = svc;
                }
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/

            }

            //Get the Culture COde for moss from url
            if (!string.IsNullOrEmpty(Request.QueryString["MossCultureCode"]))
            {
                objSvcInfo.MossCultureCode = Request.QueryString["MossCultureCode"].ToString();
            }

            /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/

            /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
            Session["serviceInfo"] = objSvcInfo;
            /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/

            //if (Request.QueryString.Count > 1)
            //{
            //    CreateCookies();
            //    Response.Redirect("Default.aspx?module=" + objSvcInfo.Module, false);
            //    HttpContext.Current.ApplicationInstance.CompleteRequest();
            //}

        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            throw ex;

        }

    }

    private bool checkCountryValidity(double lat, double lng)
    {
        bool isLatLongValid = false;
        string strCountryCode = objCommonUtil.CheckLatLongForCountry(double.Parse(lat.ToString()), double.Parse(lng.ToString()));

        if (!string.IsNullOrEmpty(strCountryCode))
            strCountryCode = strCountryCode.Substring(0, 2);

        DataTable dtCountries = sh.loadCountries();
        if (dtCountries != null && dtCountries.Rows.Count > 0)
        {
            // look for the country code obtained from CheckLatLongForCountry() in the datatable
            if (dtCountries.Select(string.Format("code= '{0}'", strCountryCode.ToUpper())).Distinct().Count() > 0)
            {
                DataTable dt = dtCountries.Select(string.Format("code= '{0}'", strCountryCode.ToUpper())).Distinct().CopyToDataTable();
                if (dt != null && dt.Rows.Count > 0)
                    isLatLongValid = true;

                else
                    isLatLongValid = false;
            }
            else
                isLatLongValid = false;
        }
        //}
        return isLatLongValid;
        //return true;
        /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  END*/
    }
    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
    private void UpdateValueToIAgriInfo(string key, string value)
    {
        Dictionary<string, string> objAgriInfo = new Dictionary<string, string>();
        if (Session["IAgriInfo"] != null)
        {
            objAgriInfo = (Dictionary<string, string>)Session["IAgriInfo"];
        }
        if (objAgriInfo.ContainsKey(key))
            objAgriInfo[key] = value;
        else
            objAgriInfo.Add(key, value);

        Session["IAgriInfo"] = objAgriInfo;
    }
    private void UpdateValueToIAgriInfo(string key)
    {
        if (key == "TopSoilWHC" || key == "SubSoilWHC")
            UpdateValueToIAgriInfo(key, Convert.ToString((Convert.ToDouble(Request[key]) / 10)));
        else
            UpdateValueToIAgriInfo(key, Request[key]);
    }
    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/

    private void CreateCookies()
    {

        if (!string.IsNullOrEmpty(strClientid))
            LocationCookie = new HttpCookie("ais_" + strClientid, objLocInfo.SerializeCookieString());
        else
            LocationCookie = new HttpCookie("ais_LocationInfo", objLocInfo.SerializeCookieString());

        LocationCookie.Expires = DateTime.Now.AddDays(365);
        Response.Cookies.Add(LocationCookie);
    }
    private void setDefault()
    {
        objSvcPre.setDefaultLoc();
    }
    private void setLocationFromIP()
    {
    }
    protected void GetFavorites()
    {
        //Get the User Favorites from Session
        if (Session["UserFavorites"] != null)
        {
            Dictionary<string, Syngenta.Agricast.UserManagement.Favorite> dictFavList = (Dictionary<string, Syngenta.Agricast.UserManagement.Favorite>)Session["UserFavorites"];
            dtFavorites = new DataTable();
            DataRow drFav = null;
            dtFavorites.Columns.Add(Constants.FAV_Key);
            dtFavorites.Columns.Add(Constants.FAV_LATITUDE);
            dtFavorites.Columns.Add(Constants.FAV_FAVORITENAME);
            dtFavorites.Columns.Add(Constants.FAV_SERVICENAME);
            dtFavorites.Columns.Add(Constants.FAV_MODULENAME);
            dtFavorites.Columns.Add(Constants.FAV_PLACENAME);
            dtFavorites.Columns.Add(Constants.FAV_ALTITUDE);
            dtFavorites.Columns.Add(Constants.FAV_ENDDATE);
            dtFavorites.Columns.Add(Constants.FAV_ENDOFFSET);
            dtFavorites.Columns.Add(Constants.FAV_FARMID);
            dtFavorites.Columns.Add(Constants.FAV_FIELDID);
            dtFavorites.Columns.Add(Constants.FAV_LONGITUDE);
            dtFavorites.Columns.Add(Constants.FAV_PLACEID);
            dtFavorites.Columns.Add(Constants.FAV_STARTDATE);
            dtFavorites.Columns.Add(Constants.FAV_STARTOFFSET);

            //Loop through Each Favorite
            foreach (KeyValuePair<string, Syngenta.Agricast.UserManagement.Favorite> pair in dictFavList)
            {
                //Create a New Row
                drFav = dtFavorites.NewRow();

                //Add the Requiqred columns
                drFav[Constants.FAV_Key] = pair.Key;
                drFav[Constants.FAV_FAVORITENAME] = pair.Key;
                drFav[Constants.FAV_MODULENAME] = pair.Value.ModuleName;
                drFav[Constants.FAV_SERVICENAME] = pair.Value.ServiceName;
                drFav[Constants.FAV_PLACENAME] = HttpUtility.HtmlDecode(pair.Value.PlaceName);
                drFav[Constants.FAV_ALTITUDE] = pair.Value.Altitude;
                drFav[Constants.FAV_ENDDATE] = pair.Value.EndDate;
                drFav[Constants.FAV_ENDOFFSET] = pair.Value.EndOffset;
                drFav[Constants.FAV_FARMID] = pair.Value.FarmID;
                drFav[Constants.FAV_FIELDID] = pair.Value.FieldID;
                drFav[Constants.FAV_LATITUDE] = pair.Value.Latitude;
                drFav[Constants.FAV_LONGITUDE] = pair.Value.Longitude;
                drFav[Constants.FAV_PLACEID] = pair.Value.PlaceID;
                drFav[Constants.FAV_STARTDATE] = pair.Value.StartDate;
                drFav[Constants.FAV_STARTOFFSET] = pair.Value.StartOffset;

                //Add the Row to Datatable
                dtFavorites.Rows.Add(drFav);

                //clear the DataRow object
                drFav = null;
            }


            //Add the Favorites Datatable to Session
            UserInfo ObjUserInfo = (UserInfo)Session["objUserInfo"];
            if (ObjUserInfo != null)
            {
                ObjUserInfo.DtFavorites = dtFavorites;
                Session["ObjUserInfo"] = ObjUserInfo;
            }
        }


    }
    #endregion
    public string TransString
    {
        get;
        set;
    }
    public DataTable dtMenuData
    {
        get
        {
            return dtPages;
        }
        set
        {
            dtPages = value;
        }
    }
    public string DefaultTab
    {
        get;
        set;
    }
    public List<string[]> alNodeList
    {
        get;
        set;
    }
    public DataSet dsMossMenu
    {
        get;
        set;
    }
    public string ExtNavigation
    {
        get;
        set;
    }
    /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
    public string MossReturnUrl
    {
        get;
        set;
    }
    /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - END*/
    public bool IsSecure
    {
        get;
        set;
    }
    public string AllowedRoles
    {
        get;
        set;
    }
    public DataTable dtLegenddetails
    {
        get;
        set;
    }
    public string encryptKey
    {
        get;
        set;
    }
    private bool IsEmpty<T>(dynamic obj)
    {
        object property_value = null;

        bool flag = false;

        System.Reflection.PropertyInfo[] properties_info = typeof(T).GetProperties();
        //System.Reflection.PropertyInfo property_info = default(System.Reflection.PropertyInfo); 

        foreach (System.Reflection.PropertyInfo prop in properties_info)
            if (prop != null)
            {
                property_value = prop.GetValue(obj, null);
                switch (prop.PropertyType.Name.ToString().ToLower())
                {
                    case "int32": if (property_value != null)
                            flag = Int32.Parse(property_value.ToString()) == 0;
                        break;
                    case "double": if (property_value != null)
                            flag = double.Parse(property_value.ToString()) == 0.0;
                        break;
                    case "string": if (property_value != null)
                            flag = property_value.ToString() == "";
                        break;
                    case "datetime": if (property_value != null)
                            flag = DateTime.Parse(property_value.ToString()) == DateTime.MinValue || DateTime.Parse(property_value.ToString()) == DateTime.MaxValue;
                        break;
                }
            }

        return flag;
    }
    private void ClickExport(object sender, EventArgs e)
    {
        SaveExcelData();

    }

    private void SaveFavorite(object sender, EventArgs e)
    {
        DataTable dtTemp = null;
        int i;
        double d;
        DateTime date;
        //Get the UserInfo From Sesison
        objUserInfo = (UserInfo)Session["objUserInfo"];

        if (objUserInfo != null)
        {
            //Convert the Datatable back to Dictionary
            dtTemp = objUserInfo.DtFavorites;

            dictFavList = (Dictionary<string, Syngenta.Agricast.UserManagement.Favorite>)Session["UserFavorites"];

            //Execute Update(count of dictionary and Datatable must be equal)
            if (dictFavList.Count == dtTemp.Rows.Count)
            {
                foreach (DataRow dr in dtTemp.Rows)
                {
                    objFav = new Syngenta.Agricast.UserManagement.Favorite();

                    //Check if the Old Key Exists
                    if (dictFavList.ContainsKey(dr[Constants.FAV_Key].ToString()))
                    {
                        //Delete the old one and add the values with new key
                        dictFavList.Remove(dr[Constants.FAV_Key].ToString());

                        objFav.Altitude = int.TryParse(dr[Constants.FAV_ALTITUDE].ToString(), out i) ? i : 0;
                        objFav.EndDate = DateTime.TryParse(dr[Constants.FAV_ENDDATE].ToString(), out date) ? date : DateTime.Parse(DateTime.Now.ToString());
                        //objFav.EndDate = (string.IsNullOrEmpty(dr[Constants.FAV_ENDDATE].ToString())) ? "" : dr[Constants.FAV_ENDDATE].ToString();
                        objFav.EndOffset = int.TryParse(dr[Constants.FAV_ENDOFFSET].ToString(), out i) ? i : 0;
                        objFav.FarmID = int.TryParse(dr[Constants.FAV_FARMID].ToString(), out i) ? i : 0;
                        objFav.FieldID = int.TryParse(dr[Constants.FAV_FIELDID].ToString(), out i) ? i : 0;
                        objFav.Latitude = double.TryParse(dr[Constants.FAV_LATITUDE].ToString(), out d) ? d : 0;
                        objFav.Longitude = double.TryParse(dr[Constants.FAV_LONGITUDE].ToString(), out d) ? d : 0;
                        objFav.ModuleName = dr[Constants.FAV_MODULENAME].ToString();
                        objFav.PlaceID = int.TryParse(dr[Constants.FAV_PLACEID].ToString(), out i) ? i : 0;
                        objFav.ServiceName = dr[Constants.FAV_SERVICENAME].ToString();
                        objFav.StartDate = DateTime.TryParse(dr[Constants.FAV_STARTDATE].ToString(), out date) ? date : DateTime.Parse(DateTime.Now.ToString());
                        objFav.StartOffset = int.TryParse(dr[Constants.FAV_STARTOFFSET].ToString(), out i) ? i : 0;
                        objFav.PlaceName = dr[Constants.FAV_PLACENAME].ToString();

                        //Add the New Values to Dictionary
                        dictFavList.Add(dr[Constants.FAV_FAVORITENAME].ToString(), objFav);

                        //Reset the Fav Object
                        objFav = null;

                    }


                }//End of For each
            }//end of if

            //Execute Delete if the Count of Data is more in Dictionary
            else if (dictFavList.Count > dtTemp.Rows.Count)
            {
                //loop through each entry in dictionary
                foreach (KeyValuePair<string, Syngenta.Agricast.UserManagement.Favorite> pair in dictFavList)
                {
                    //check whether the the Dictionary key exists in Datatable
                    var res = from p in dtTemp.AsEnumerable()
                              where p[Constants.FAV_Key].Equals(pair.Key)
                              select p[Constants.FAV_Key];

                    //when no matach found
                    if (res.ToList().Count == 0)
                    {
                        //Remove the data from dictionary
                        dictFavList.Remove(pair.Key);
                        break;
                    }
                }

            }
            //Execute Add Operation if the Count of Data is more in Datatable
            else
            {
                foreach (DataRow dr in dtTemp.Rows)
                {
                    objFav = new Syngenta.Agricast.UserManagement.Favorite();

                    //Check if the Old Key Exists
                    if (dictFavList.ContainsKey(dr[Constants.FAV_Key].ToString()))
                    {
                        //Delete the old one and add the values with new key
                        dictFavList.Remove(dr[Constants.FAV_Key].ToString());


                    }
                    objFav.Altitude = int.TryParse(dr[Constants.FAV_ALTITUDE].ToString(), out i) ? i : 0;
                    //objFav.EndDate = DateTime.Parse(dr[Constants.FAV_ENDDATE].ToString());
                    //objFav.EndOffset = int.TryParse(dr[Constants.FAV_ENDOFFSET].ToString(), out i) ? i : 0;
                    //objFav.FarmID = int.TryParse(dr[Constants.FAV_FARMID].ToString(), out i) ? i : 0;
                    // objFav.FieldID = int.TryParse(dr[Constants.FAV_FIELDID].ToString(), out i) ? i : 0;
                    objFav.Latitude = double.TryParse(dr[Constants.FAV_LATITUDE].ToString(), out d) ? d : 0;
                    objFav.Longitude = double.TryParse(dr[Constants.FAV_LONGITUDE].ToString(), out d) ? d : 0;
                    objFav.ModuleName = dr[Constants.FAV_MODULENAME].ToString();
                    objFav.PlaceID = int.TryParse(dr[Constants.FAV_PLACEID].ToString(), out i) ? i : 0;
                    objFav.ServiceName = dr[Constants.FAV_SERVICENAME].ToString();
                    //objFav.StartDate = DateTime.Parse(dr[Constants.FAV_STARTDATE].ToString());
                    //objFav.StartOffset = int.TryParse(dr[Constants.FAV_STARTOFFSET].ToString(), out i) ? i : 0;
                    objFav.PlaceName = HttpUtility.HtmlDecode(dr[Constants.FAV_PLACENAME].ToString());

                    //Add the New Values to Dictionary
                    dictFavList.Add(dr[Constants.FAV_FAVORITENAME].ToString(), objFav);

                    //Reset the Fav Object
                    objFav = null;

                }
            }


            //Use the Favorite  Name as the Key in Dictionary
            Session["UserFavorites"] = dictFavList;

            //Get the Current userProfile and update
            ProfileCommon prof = Profile.GetProfile(objUserInfo.UserName);

            //assign the modified FavoriteList
            prof.MyCustomProfile.FavoriteList = dictFavList;

            //Save the Profile
            prof.Save();

            //Get the Latest Saved Favorities from database
            GetFavorites();

        }


    }

    private void EmailPage(object sender, EventArgs e)
    {

        try
        {

            //set Email to true
            isEmail = true;
            Session.Add("isPrintorPDF", "true");
            //set HTML needed = true
            blGenerateHtml = true;

        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.DEF_PDF_FAIL);
        }

    }


    private void PrintPage(object sender, EventArgs e)
    {
        try
        {
            //Set Print = true
            isPrint = true;
            /* IM01246228 - New Agricast - Print is not working -Begin*/
            //Session.Add("isPrintorPDF", "true");
            /* IM01246228 - New Agricast - Print is not working  -End*/
            //set HTML needed = true
            blGenerateHtml = true;

            ClientScript.RegisterStartupScript(Page.GetType(), "onclickLoad", "fnPrintPage();", true);
            //ClientScript.RegisterStartupScript(Page.GetType(), "onclickLoad", "fnTrigHdnBtnPrint();", true);


        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.DEF_PDF_FAIL);
        }

    }
    string getHtml(string url)
    {

        HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);

        myWebRequest.Method = "GET";

        // make request for web page

        HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();

        StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream());

        string myPageSource = string.Empty;

        myPageSource = myWebSource.ReadToEnd();

        myWebResponse.Close();

        return myPageSource;

    }


    private void AddAuditdata()
    {
        if (Session["AuditData"] == null)
        {

            dictAudit["userIP"] = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            dictAudit["userID"] = objUserInfo != null ? objUserInfo.UserName : "";
            dictAudit["token"] = "";
            dictAudit["referrer"] = (HttpContext.Current.Request.UrlReferrer == null) ? "none" : HttpContext.Current.Request.UrlReferrer.ToString();
            dictAudit["entrancePath"] = objSvcInfo.Moss.ToLower() == "true" ? "Moss" : "External";
            dictAudit["culture"] = objSvcInfo.Culture;
            dictAudit["sessionID"] = HttpContext.Current.Session.SessionID;
            dictAudit["service"] = objSvcInfo.ServiceName;
            dictAudit["module"] = objSvcInfo.Module;
            dictAudit["locSearchType"] = "By Cookie";
            dictAudit["locSearchStringType"] = "";
            dictAudit["locSearchString"] = objLocInfo.searchLocation;
            dictAudit["locSearchDatasource"] = objLocInfo.Provider;
            dictAudit["numOfLocs"] = "";
            if (objLocInfo.latitude == 0)
                dictAudit["searchLat"] = DBNull.Value;
            else
                dictAudit["searchLat"] = objLocInfo.latitude;
            if (objLocInfo.longitude == 0)
                dictAudit["searchLong"] = DBNull.Value;
            else
                dictAudit["searchLong"] = objLocInfo.longitude;
            dictAudit["countryName"] = (objLocInfo.CountryCode == null) ? "" : objLocInfo.CountryCode;
            dictAudit["locName"] = objLocInfo.placeName;

            dictAudit["weatherDatasource"] = (objDataPointInfo == null) ? enumDataSources.glbStnFcst : objDataPointInfo.NearbyPointSettings.DataSource;
            if (objDataPointInfo == null || objDataPointInfo.stationLatitude == 0)
                dictAudit["weatherLat"] = DBNull.Value;
            else
                dictAudit["weatherLat"] = objDataPointInfo.stationLatitude;
            if (objDataPointInfo == null || objDataPointInfo.stationLongitude == 0)
                dictAudit["weatherLong"] = DBNull.Value;
            else
                dictAudit["weatherLong"] = objDataPointInfo.stationLongitude;
            dictAudit["weatherDateFrom"] = "";
            dictAudit["weatherDateTo"] = "";
            dictAudit["weatherSeries"] = "";
            dictAudit["weatherParams"] = DBNull.Value;


            Session.Add("AuditData", dictAudit);
        }
        else if (Session["AuditData"] != null)
        {
            IDictionary dict = (IDictionary)Session["AuditData"];
            dict["locSearchDatasource"] = objLocInfo.Provider;
            dict["numOfLocs"] = "";
            if (objLocInfo.latitude == 0)
                dict["searchLat"] = DBNull.Value;
            else
                dict["searchLat"] = objLocInfo.latitude;
            if (objLocInfo.longitude == 0)
                dict["searchLong"] = DBNull.Value;
            else
                dict["searchLong"] = objLocInfo.longitude;
            dict["countryName"] = (objLocInfo.CountryCode == null) ? "" : objLocInfo.CountryCode;
            dict["locName"] = objLocInfo.placeName;
            dictAudit["weatherDatasource"] = (objDataPointInfo == null) ? enumDataSources.glbStnFcst : objDataPointInfo.NearbyPointSettings.DataSource;
            if (objDataPointInfo == null || objDataPointInfo.stationLatitude == 0)
                dict["weatherLat"] = DBNull.Value;
            else
                dict["weatherLat"] = objDataPointInfo.stationLatitude;
            if (objDataPointInfo == null || objDataPointInfo.stationLongitude == 0)
                dict["weatherLong"] = DBNull.Value;
            else
                dict["weatherLong"] = objDataPointInfo.stationLongitude;
            Session["AuditData"] = dict;
        }

    }
    public void SaveExcelData()
    {
        objSvcPre = new ServicePresenter(this);
        objTblPre = new TablePresenter();
        List<string[]> alNodeList = objSvcPre.getModuleNodeList("servicePage");
        DataSet ds = new DataSet();
        List<string[]> alList = new List<string[]>();
        objDataPointInfo = DataPointInfo.getDataPointObject;
        if (objDataPointInfo != null && objDataPointInfo.stationLatitude != 0.0 || objDataPointInfo.stationLongitude != 0.0)
        {
            foreach (string[] node in alNodeList)
            {
                switch (node[0].ToString().ToLower())
                {
                    case "tbldaysrows": alList.Add(new string[] { "tbldaysrows", node[1].ToString() });
                        break;

                    case "tblseriesrows": alList.Add(new string[] { "tblseriesrows", node[1].ToString() });
                        break;

                    case "agriinfo":
                        Control ctrl = common.FindControlRecursive(this, "hdnSeries");
                        if (ctrl != null)
                        {

                            HiddenField hf = (HiddenField)ctrl;
                            string strcheckedSeries = Request.Form.Get(hf.UniqueID) != null ? Request.Form.Get(hf.UniqueID) : "";
                            string checkedSeries = hf.Value;

                            if (checkedSeries != null && checkedSeries != string.Empty)
                            {
                                //Code to check whether start date is empty
                                Control ctrlDate = common.FindControlRecursive(this, "txtstartDate");
                                if (ctrlDate != null)
                                {

                                    HtmlInputText inStartDate = (HtmlInputText)ctrlDate;
                                    if (inStartDate.Value != "")
                                    {
                                        HttpContext.Current.Session["ErrorMessage"] = "";
                                        common.LoadCtrl<AgriInfo>(this.Page, CentrePlaceHolder, "AgriInfo", node[1].ToString(), node[0].ToString(), true, checkedSeries);
                                    }
                                    else
                                    {
                                        objSvcPre.getTransText(Constants.DEF_SAVEEXCEL_STARTDATE);
                                        HttpContext.Current.Session["ErrorMessage"] = TransString;
                                    }

                                }
                            }
                            else
                            {
                                objSvcPre.getTransText(Constants.DEF_SAVEEXCEL_NODATA);
                                HttpContext.Current.Session["ErrorMessage"] = TransString;
                            }

                        }
                        else
                        {
                            objSvcPre.getTransText(Constants.DEF_SAVEEXCEL_NODATA);
                            HttpContext.Current.Session["ErrorMessage"] = TransString;
                        }
                        break;

                    default:
                        break;
                }



            }
            if (!objSvcInfo.Module.ToLower().Contains("history") && alList.Count > 0)
                common.LoadCtrl<Tables>(this.Page, CentrePlaceHolder, "Tables", "excel", alList);

        }
        else
        {
            objSvcPre.getTransText(Constants.DEF_SAVEEXCEL_NOSTATIONS);
            HttpContext.Current.Session["ErrorMessage"] = TransString;
        }

    }

    void setFooter()
    {
        string opentag = "<li>";
        string closeTag = "</li>";
        StringBuilder sb = new StringBuilder();
        sb.Append(opentag);
        //objSvcPre.getTransText("All_Copyright");
        //sb.Append(TransString + "&nbsp;");
        // sb.Append(opentag);

        if ((Session["ForecastProvider"]) != null && (Session["ForecastProvider"]).ToString() != "")
        {
            if (objSvcInfo.Module != "HistoryData_WMO")
            {
                objSvcPre.getTransText("All_DisclaimerLink");
                sb.Append(TransString + "&nbsp;");
                //  sb.Append(opentag);
                objSvcPre.getTransText(Session["ForecastProvider"].ToString());
                sb.Append(TransString + closeTag);
            }
            else
            {
                sb.Append(closeTag);
            }

        }
        else
        {
            sb.Append(closeTag);
        }

        footNav.InnerHtml = sb.ToString().Replace("{year}", DateTime.Now.Year.ToString());

    }

    public void setFooterText()
    {
        //set the master page controls

        HtmlAnchor hplTerms = (HtmlAnchor)common.FindControlRecursive(this, "lnkTnC");
        if (hplTerms != null)
            hplTerms.InnerText = objCommonUtil.getTransText(Constants.TERMS_AND_CONDITIONS);

        HtmlAnchor hplPrivPolicy = (HtmlAnchor)common.FindControlRecursive(this, "lnkPrivPolicy");
        if (hplPrivPolicy != null)
            hplPrivPolicy.InnerText = objCommonUtil.getTransText(Constants.PRIVACY_POLICY);

        HtmlAnchor hplContacts = (HtmlAnchor)common.FindControlRecursive(this, "lnkContacts");
        if (hplContacts != null)
            hplContacts.InnerText = objCommonUtil.getTransText(Constants.CONTACTS);

        HtmlAnchor hplMobile = (HtmlAnchor)common.FindControlRecursive(this, "lnkMobile");
        if (hplMobile != null)
            hplMobile.InnerText = objCommonUtil.getTransText(Constants.MOBILE_SITE);
    }

    protected void hdnRate_ValueChanged(object sender, EventArgs e)
    {
        string strRatings = hdnRate.Value;
        Session["Ratings"] = (Session["Ratings"] != null) ? (Session["Ratings"].ToString() + strRatings) : strRatings;
    }

    //public void  hdnRate_ValueChanged(string Ratings)
    //{
    //    string strRatings = hdnRate.Value;
    //    Session["Ratings"] = (Session["Ratings"] != null) ? (Session["Ratings"].ToString() + strRatings) : strRatings;
    //}




}
