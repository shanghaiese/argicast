#region Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.LocationSearch.LocWebService;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common;
using Syngenta.AgriCast.LocationSearch.View;
using Syngenta.AgriCast.LocationSearch.Presenter;
using System.Globalization;
using Syngenta.AgriCast.Icon;
using Syngenta.AgriCast.LocationSearch.Service;
using Syngenta.AgriCast.Tables.Presenter;
using Syngenta.AgriCast.Tables.View;
using System.Collections;
using Syngenta.AgriCast.Mobile.Presenter;
using System.Text;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Text.RegularExpressions;
using Syngenta.AgriCast.ExceptionLogger;
using System.Web.Security;
using Syngenta.AgriWeb.LocationSearch.DataObjects;
#endregion

#region Class
public partial class Mobile_Default : System.Web.UI.Page, ILocSearch
{
    # region Declarations
    MobilePresenter objMobPresenter;
    // ServiceHandler objServiceHandler = null;
    ArrayList alIocnList;

    LocSearchWebService objWebService;
    locSearchPresenter objLocPresenter;
    ServiceInfo objSvcInfo;
    ServicePresenter objSvcPre;
    ServiceHandler ServiceObj;
    nearbyPointPresenter objNearByPre;
    Controls common = new Controls();
    nearbyPointService nearBySvc;
    locSearchService locSvc;
    CommonUtil objComUtil;
    string pubname = null;
    string ccode = null;
    LocationInfo objLocInfo;
    DataPointInfo objDataPointInfo = new DataPointInfo();
    ModuleInfo objModInfo;
    DataTable dtPages;
    //string strCulCode = "";
    string strText;
    LocationSearchSource eProvider;
    int NumOfLocs;
    bool isSelectingSingleLoc = false;



    //Constants
    string DEFAULTMODULE = "MobileForecast";
    const string SERVICEPAGENAME = "servicePage";
    const string SERVICENAME = "service";
    const string DATE = "date";
    const string WIND_DIRECTION = "winddir";
    const string WIND_DIRECTION_TEXT = "winddirtext";
    const string WIND_SPEED = "windspeed";
    const string PRECIP = "precip";
    const string TEMPMAX = "maxTemp";
    const string IMAGEURL = "imageurl";
    const string TEMPMIN = "minTemp";
    string AllignDetails = string.Empty;
    /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
    /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */
    const string WIND_DIR_TEXT = "winddirectionText";
    /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */

    IDictionary dictAudit = new Hashtable();

    DataSet dsTableSeries;
    PaletteMap objPm;
    DataTable dtSeriesDetails;
    DataTable dtPageNodes;
    List<string[]> al;
    DataTable dtTransposed;
    string CultureCode;

    int step = 1;
    int NumOfDays;
    GridView gvDynamic;
    CultureInfo VariantCulture;
    DataTable dtModalInput = new DataTable();
    DataTable dtOutput;
    //DataPointInfo objDataPointInfo;
    DataTable dtByDaysLoc = null;
    DataTable dtSeriesnames = null;


    string strWindDir = string.Empty;
    string strWindSpeed = string.Empty;
    string strPrecip = string.Empty;
    string strTempMax = string.Empty;
    string strTempMin = string.Empty;
    string strImageURl = string.Empty;

    public string WUnit = "mph";
    public string TUnit = "C";
    public string PUnit = "mm";

    const string TRNSTAG_TEMPMAX = "temperature_max";
    const string TRNSTAG_TEMPMIN = "temperature_min";
    const string TRNSTAG_WIND = "we_wind_kmh";
    const string TRNSTAG_RAIN = "precipitation";

    //bool flagQS = false;
    #endregion

    /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
    /* 2.1	If mobile site is enabled or restricted service then login page should appear. */
    MembershipUser memUser = null;
    bool isAuthorised = false;
    bool isSecure = false;

    /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */

    #region Events

    #region Page_Init
    protected void Page_Init(object sender, EventArgs e)
    {
        //Initialize the location and service objects
        if (Session == null || Session["serviceInfo"] == null)
        {
            objSvcInfo = new ServiceInfo();
        }
        else
        {
            objSvcInfo = (ServiceInfo)Session["serviceInfo"];
        }
        ServiceObj = new ServiceHandler();
        objDataPointInfo = DataPointInfo.getDataPointObject;
        objLocInfo = new LocationInfo();
        objLocPresenter = new locSearchPresenter(this);
        objNearByPre = new nearbyPointPresenter(this);
        objWebService = new LocSearchWebService();
        objSvcPre = new ServicePresenter();
        locSvc = new locSearchService();
        nearBySvc = new nearbyPointService();
        objComUtil = new CommonUtil();
        objMobPresenter = new MobilePresenter();

    }
    #endregion

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            //Get last searched location from cookie if present

            HttpCookie locCookie = Request.Cookies["ais_LocationInfo"];
            lblErrorMessage.Visible = false;
            if (locCookie != null)
            {
                LocationInfo newInfo = objLocInfo.DeserializeCookieString(locCookie.Value);
                if (newInfo != null)
                {
                    Session["objLocationInfo"] = newInfo;
                    objLocInfo = newInfo;
                    if (Session["AuditData"] != null)
                    {
                        IDictionary dict = (IDictionary)Session["AuditData"];
                        dict["locSearchType"] = "By Cookie";
                        Session["AuditData"] = dict;
                    }
                }

            }

            if (!Page.IsPostBack)
            {
                readPubnameQueryStrings();
            }
            if (objSvcInfo.Culture == null || objSvcInfo.Culture == "")
            {
                objSvcInfo.Culture = objSvcPre.getCultureCode();
            }
            //createMenu();
            objSvcInfo.IsMobile = true;
            if ((objLocInfo.placeName == "" || (objLocInfo.latitude == 0d && objLocInfo.longitude == 0d)) && objLocInfo.CountryCode == "")
            {
                setDefaultLoc();
            }

            AddAuditdata();

            /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
            /* 2.1	If mobile site is enabled or restricted service then login page should appear. */

            isSecure = ServiceObj.GetSecuritySetting();

            /* UAT issue - mobile login - Begin */
            objSvcInfo.AllowedRoles = ServiceObj.GetRoles();
            /* UAT issue - mobile login - End */

            if (isSecure)
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
            //redirect to login page only if security setting in config is true
            if (isSecure && (Session["objUserInfo"] == null || !((UserInfo)Session["objUserInfo"]).IsAuthenticated))
            {
                var url = HttpContext.Current.Request.Url.ToString();
                var subStrURL = url.Substring(0, url.LastIndexOf('/'));

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
            /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */

            if (!IsPostBack)
            {
                getCountryListFromPub();
                prefillpage();
                if (!string.IsNullOrWhiteSpace(objLocInfo.CountryCode))
                {
                    divSearchLoc.Attributes.Add("data-collapsed", "true");
                }
                else
                {
                    divSearchLoc.Attributes.Add("data-collapsed", "false");
                }
            }
            else
            {
                divSearchLoc.Attributes.Remove("data-collapsed");
                divSearchLoc.Attributes.Add("data-collapsed", "true");
            }


            ChangeLabelText();
            HtmlAnchor btnBack = (HtmlAnchor)this.Master.FindControl("btnBack");
            btnBack.Visible = false;


        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.MOB_LOAD_FAILURE) + " : " + ex.Message.ToString();
            return;
        }


    }

    /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
    /* 2.1	If mobile site is enabled or restricted service then login page should appear. */
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
    /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */

    #endregion

    #region Page_prerender
    private void Page_prerender(object sender, EventArgs e)
    {
        try
        {

            CreateCookies();

            loadModules();

        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.MOB_LOAD_FAILURE) + " : " + ex.Message.ToString();
            return;
        }
        finally
        {
            if (Session["AuditData"] != null)
            {
                objSvcPre.SaveAuditData();

            }
        }
    }
    #endregion

    #region btnSearch_Click
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            isSelectingSingleLoc = false;
            loadLocationInfo();
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.MOB_SEARCHLOCATION_ERROR) + " : " + ex.Message.ToString();
            return;
        }
        finally
        {
            if (Session["AuditData"] != null)
            {
                IDictionary dict = (IDictionary)Session["AuditData"];
                dict["locSearchType"] = "By Search";
                dict["locSearchString"] = objLocInfo.searchLocation;
                dict["locSearchDatasource"] = objLocInfo.Provider;
                dict["numOfLocs"] = NumOfLocs;
                dict["searchLat"] = objLocInfo.latitude;
                dict["searchLong"] = objLocInfo.longitude;
                dict["countryName"] = (objLocInfo.CountryCode == null) ? "" : objLocInfo.CountryCode;
                dict["locName"] = objLocInfo.placeName;
                dict["weatherDatasource"] = objDataPointInfo.NearbyPointSettings.DataSource;
                dictAudit["weatherLat"] = objDataPointInfo.stationLatitude;
                dictAudit["weatherLong"] = objDataPointInfo.stationLongitude;
                Session["AuditData"] = dict;
            }

            //objSvcPre.SaveAuditData();
        }
    }
    #endregion
    #region btnSearchSingle_Click
    protected void btnSearchSingle_Click(object sender, EventArgs e)
    {
        try
        {
            isSelectingSingleLoc = true;
            loadLocationInfo();
            ddlMultiplePlaces.Items.Clear();
            divSelectSingleLoc.Visible = false;
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.MOB_SEARCHLOCATION_ERROR) + " : " + ex.Message.ToString();
            return;
        }
        finally
        {
            if (Session["AuditData"] != null)
            {
                IDictionary dict = (IDictionary)Session["AuditData"];
                dict["locSearchType"] = "By Search";
                dict["locSearchString"] = objLocInfo.searchLocation;
                dict["locSearchDatasource"] = objLocInfo.Provider;
                dict["numOfLocs"] = NumOfLocs;
                dict["searchLat"] = objLocInfo.latitude;
                dict["searchLong"] = objLocInfo.longitude;
                dict["countryName"] = (objLocInfo.CountryCode == null) ? "" : objLocInfo.CountryCode;
                dict["locName"] = objLocInfo.placeName;
                dict["weatherDatasource"] = objDataPointInfo.NearbyPointSettings.DataSource;
                dictAudit["weatherLat"] = objDataPointInfo.stationLatitude;
                dictAudit["weatherLong"] = objDataPointInfo.stationLongitude;
                Session["AuditData"] = dict;
            }

            //objSvcPre.SaveAuditData();
        }
    }
    #endregion

    #endregion

    #region Methods
    void loadLocationInfo()
    {
        getLocation();

        if (!string.IsNullOrWhiteSpace(hdnLocSelected.Value) && !string.IsNullOrWhiteSpace(hdnLatLng.Value))
        {
            objLocInfo = LocationInfo.getLocationInfoObj;
            objLocInfo.placeName = hdnLocSelected.Value.Split('|')[0].ToString();
            objLocInfo.latitude = Convert.ToDouble(hdnLatLng.Value.Split('|')[0].ToString());
            objLocInfo.longitude = Convert.ToDouble(hdnLatLng.Value.Split('|')[1].ToString());
            objLocInfo.AdminName = hdnLocSelected.Value.Split('|')[1].ToString();
            objLocInfo.CountryCode = ddlCountry.SelectedValue.ToString();
            divLocation.Attributes.Add("class", "show");
            //lblNoMatch.Visible = false;                
        }
        else
        {
            divLocation.Attributes.Add("class", "hide");
            //lblNoMatch.Visible = true;
        }
        LoadStation();

    }
    void ChangeColNames(DataTable dtByDays)
    {
        foreach (DataColumn col in dtByDays.Columns)
        {
            string colname = col.ColumnName.ToLower();
            if (colname.Contains("temp"))
            {
                if (colname.Contains("daytime") || colname.Contains("maxt"))
                {
                    dtByDays.Columns[colname].ColumnName = TEMPMAX;
                }
                else
                {
                    dtByDays.Columns[colname].ColumnName = TEMPMIN;
                }
            }
            if (colname.Contains("windspeed"))
                dtByDays.Columns[colname].ColumnName = WIND_SPEED;
            if (colname.Contains("direction"))
            {
                if (colname.Contains("directiontext"))
                    dtByDays.Columns[colname].ColumnName = WIND_DIRECTION_TEXT;
                else
                    dtByDays.Columns[colname].ColumnName = WIND_DIRECTION;
            }
            if (colname.Contains("precip"))
                dtByDays.Columns[colname].ColumnName = PRECIP;
        }
    }

    /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
    /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */
    /* label: 
     * value: 
     * unit: "&deg;"
     * */
    private void GenerateHtmlForSeries(StringBuilder strBuilder, string label, string value, string unit = "")
    {
        strBuilder.Append("<div>");
        strBuilder.AppendFormat(@"<label style=""color: Gray;"">{0}:&nbsp;</label>", label);
        strBuilder.AppendFormat(@"<span style=""color: Gray;"">{0}{1}<label style=""font-size: smaller;""></label></span>",
                                value, unit);
        strBuilder.Append("</div>");
    }
    /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */

    #region Create Html
    public void CreateHTML()
    {
        try
        {
            double iMax = 0.0;
            double iMin = 0.0;
            StringBuilder sb = new StringBuilder();

            //Store first day's date
            //double dAvgMaxMin = ((double.TryParse(dtByDaysLoc.Rows[0][TEMPMAX].ToString(), out iMax) ? iMax : 0.0) + (double.TryParse(dtByDaysLoc.Rows[0][TEMPMIN].ToString(), out iMin) ? iMin : 0.0)) / 2.0;
            //Add the   First Image from GetIconData
            string strPath = alIocnList != null ? alIocnList[0].ToString() : "";
            //lblTempText.Text = Math.Round(dAvgMaxMin).ToString();
            if (dtByDaysLoc != null && dtByDaysLoc.Rows.Count > 0)
            {
                imgTemp.ImageUrl = "../Images/Icons/" + Path.GetFileName(strPath);
                /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
                /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */
                
                imgWindDir.ImageUrl = "../Images/" + dtByDaysLoc.Rows[0][WIND_DIR_TEXT].ToString() + ".gif";
                //imgWindDir.ImageUrl = "../Images/" + dtByDaysLoc.Rows[0][WIND_DIRECTION_TEXT].ToString() + ".gif";
                foreach (DataRow row in dtSeriesnames.Rows)
                {
                    var tagName = row["trnsTag"].ToString();
                    var colName = row["name"].ToString();

                    GenerateHtmlForSeries(sb, getTranslatedText(tagName), dtByDaysLoc.Rows[0][colName].ToString());
                }
                ltlSeries.Text = sb.ToString();

                /* commented for new implementation 
                lblTempMax.Text = Math.Round(double.TryParse(dtByDaysLoc.Rows[0][TEMPMAX].ToString(), out iMax) ? iMax : 0.0).ToString();
                lblTempMin.Text = Math.Round(double.TryParse(dtByDaysLoc.Rows[0][TEMPMIN].ToString(), out iMin) ? iMin : 0.0).ToString();
                lblWindSpeed.Text = Math.Round(double.TryParse(dtByDaysLoc.Rows[0][WIND_SPEED].ToString(), out iMax) ? iMax : 0.0).ToString();
                //imgPrecip.ImageUrl = "../Images/Rain1.jpg";
                lblPrecip.Text = (double.TryParse(dtByDaysLoc.Rows[0][PRECIP].ToString(), out iMax) ? iMax : 0.0).ToString();
                foreach (DataRow dr in dtSeriesnames.Rows)
                {
                    if (dr["trnsTag"].ToString().ToLower().Contains("temp"))
                    {
                        if (dr["trnsTag"].ToString().ToLower().Contains("max"))
                            lblMax.Text = getTranslatedText(dr["trnsTag"].ToString()) + ": ";
                        else
                            lblMin.Text = getTranslatedText(dr["trnsTag"].ToString()) + ": ";
                    }
                    if (dr["trnsTag"].ToString().Contains("_wind_"))
                        lblWind.Text = getTranslatedText(dr["trnsTag"].ToString()) + ": ";
                    if (dr["trnsTag"].ToString().Contains(TRNSTAG_RAIN))
                        lblRain.Text = getTranslatedText(dr["trnsTag"].ToString()) + ": ";
                }
                */
                /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */
            }
            else if (IsPostBack) //Added by Rahul
            {
                divLocation.Attributes.Add("class", "hide");
                lblNoMatch.Visible = true;
            }

            //if (objSvcInfo.Unit.ToLower() == "imperial")
            //{
            //    TUnit = "F";
            //    PUnit = "in";
            //}
            //switch (objSvcInfo.WUnit.ToLower())
            //{
            //    case "beaufort":
            //        WUnit = "bft";
            //        break;
            //    case "mph":
            //        WUnit = "mph";
            //        break;
            //    case "kmh":
            //        WUnit = "kmh";
            //        break;
            //}
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.MOB_CREATEHTML_ERROR) + " : " + ex.Message.ToString();
            return;
        }
    }
    #endregion

    #region getCountryListFromPub
    /// <summary>
    /// Method to populate the country dropdown.
    /// </summary>
    protected void getCountryListFromPub()
    {
        try
        {
            objLocPresenter.getCountry();
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.MOB_GETCOUNTRY_ERROR) + " : " + ex.Message.ToString();
            return;
        }
    }
    #endregion

    #region getLocation
    /// <summary>
    /// Method to fetch the place/location entered by the user
    /// </summary>
    protected void getLocation()
    {
        try
        {
            objLocPresenter.getProviderName();
            LocationSearchSource strProvider = eProviderName;
            Session["Provider"] = strProvider;
            //If the user enter the coordinates
            var lat = 0.00;
            var lang = 0.00;
            string searchFor = string.Empty;
            if (isSelectingSingleLoc)
            {
                searchFor = ddlMultiplePlaces.SelectedValue;
            }
            else
            {
                searchFor = searchbox.Value;
            }
            objLocInfo.searchLocation = searchFor;
            if (searchFor.IndexOf(',') > 0) //&& searchbox.Value.Split(',')[0] != "")
            {
                double.TryParse(searchFor.ToString().Split(',')[0], out lat);
                double.TryParse(searchFor.ToString().Split(',')[1], out lang);

                //string MatchLatPattern = @"^-?([1-8]?[1-9]|[1-9]0)\.\d{0,6}";
                //string MatchLongPattern = @"^-?([1]?[1-7][1-9]|[1]?[1-8][0]|[1-9]?[0-9])\.\d{0,6}";
                string MatchLatPattern = @"^-?([1-8]?[1-9]|[1-9]0)?\.?[0-9]{0,4}";
                string MatchLongPattern = @"^-?([1]?[1-7][1-9]|[1]?[1-8][0]|[1-9]?[0-9])?\.?[0-9]{0,4}";

                if (Regex.IsMatch(lat.ToString(), MatchLatPattern) && Regex.IsMatch(lang.ToString(), MatchLongPattern))
                {

                    objLocInfo.latitude = lat;
                    objLocInfo.longitude = lang;
                    objLocInfo.placeName = "Lat: " + lat + " Long: " + lang;
                    objLocInfo.CountryCode = ddlCountry.SelectedValue.ToString();


                    if (objLocInfo.longitude != 0.00 && objLocInfo.latitude != 0.00)
                    {
                        //check if the Lat long specified belongs to the selected country code
                        //if found, method returns country code , if not, some junk html
                        string strCountryCode = objComUtil.CheckLatLongForCountry(double.Parse(lat.ToString()), double.Parse(lang.ToString()));

                        //takte the first two characters
                        if (!string.IsNullOrEmpty(strCountryCode))
                            strCountryCode = strCountryCode.Substring(0, 2);
                        if (strCountryCode == objLocInfo.CountryCode)
                        {

                            LoadStation();
                            hdnLatLng.Value = objLocInfo.latitude.ToString() + '|' + objLocInfo.longitude.ToString();
                            hdnLocSelected.Value = objLocInfo.placeName.ToString() + "|" + "";
                            lblNoMatch.Visible = false;
                            if (isSelectingSingleLoc)
                            {
                                lblLocationName.Text = ddlMultiplePlaces.SelectedItem.Text.ToString();
                            }
                            else
                            {
                                lblLocationName.Text = objLocInfo.placeName.ToString();
                            }


                            if (Session["AuditData"] != null)
                            {
                                IDictionary dict = (IDictionary)Session["AuditData"];
                                dict["locSearchStringType"] = "Lat/Long";
                                dict["locSearchString"] = objLocInfo.searchLocation;
                                dict["locSearchDatasource"] = objLocInfo.Provider;
                                dict["numOfLocs"] = 1;
                                dict["searchLat"] = objLocInfo.latitude;
                                dict["searchLong"] = objLocInfo.longitude;
                                Session["AuditData"] = dict;
                            }

                        }
                        else
                        {
                            hdnLatLng.Value = string.Empty;
                            hdnLocSelected.Value = string.Empty;
                            lblErrorMessage.Text = getTranslatedText("Lat & Long entered do not belong to the country selected.");
                            lblErrorMessage.Visible = true;
                        }
                    }
                }
                else
                {
                    objLocPresenter.getLocationDetails(searchFor.Split(',')[0].ToString(), ddlCountry.SelectedValue.ToString(), strProvider, 0.00, 0.00, objSvcInfo.Culture);
                }
            }
            else
            {
                objLocPresenter.getLocationDetails(searchFor.ToString().Split('(')[0], ddlCountry.SelectedValue.ToString(), strProvider, 0.00, 0.00, objSvcInfo.Culture);
                if (Session["AuditData"] != null)
                {
                    IDictionary dict = (IDictionary)Session["AuditData"];
                    dict["locSearchStringType"] = "Placename";
                    dict["locSearchString"] = objLocInfo.searchLocation;
                    dict["locSearchDatasource"] = objLocInfo.Provider;
                    dict["numOfLocs"] = NumOfLocs;
                    dict["searchLat"] = objLocInfo.latitude;
                    dict["searchLong"] = objLocInfo.longitude;
                    Session["AuditData"] = dict;
                }
            }
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.MOB_GETLOCATION_ERROR) + " : " + ex.Message.ToString();
            return;
        }
    }
    #endregion

    #region CreateCookies
    private void CreateCookies()
    {

        HttpCookie LocationCookie = new HttpCookie("ais_LocationInfo", objLocInfo.SerializeCookieString());
        LocationCookie.Expires = DateTime.Now.AddDays(365);
        Response.Cookies.Add(LocationCookie);
    }
    #endregion

    #region setDefaultLoc
    private void setDefaultLoc()
    {
        objSvcPre.setDefaultLoc();
    }
    #endregion

    #region readPubnameQueryStrings
    private void readPubnameQueryStrings()
    {
        ServiceInfo.ServiceConfig = objSvcInfo;
        objLocInfo = LocationInfo.getLocationInfoObj;
        string RedirectedUrl = null;
        try
        {
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

            //Checking previous servicename
            string prePubname = objSvcInfo.ServiceName;
            //if null or different, assigning current pubname to session variable
            if ((prePubname == "") || (prePubname != pubname))
            {
                objSvcInfo.ServiceName = pubname;
                ServicePresenter objSvcPre = new ServicePresenter();
                Session["service"] = null;
                //objSvcPre.createServiceSession();
                //svc = (service)Session["service"];
                service svc = objSvcPre.readConfig();
                objSvcPre.createServiceSession();

            }

            //Reading query strings(if any) and adding values to the session 
            if (Request.QueryString["module"] != null)
            {
                objSvcInfo.Module = Request.QueryString["module"];
            }
            else
            {
                objSvcInfo = (ServiceInfo)Session["serviceInfo"];
                if (objSvcInfo.Module == null || objSvcInfo.Module == "")
                {
                    objSvcInfo.Module = DEFAULTMODULE;
                }
            }

            if (Request.QueryString["Culture"] != null)
            {
                objSvcInfo.Culture = Request.QueryString["Culture"];
            }
            if (Request.QueryString["Country"] != null)
            {
                objSvcInfo.Country = Request.QueryString["Country"];
            }
            if (Request.QueryString["Unit"] != null)
            {
                objSvcInfo.ServiceName = Request.QueryString["Unit"];
            }
            if (Request.QueryString["Placename"] != null)
            {
                objLocInfo.placeName = Request.QueryString["Placename"];
            }
            if (Request.QueryString["Latitude"] != null)
            {
                objLocInfo.latitude = Convert.ToDouble(Request.QueryString["Latitude"]);
            }
            if (Request.QueryString["Longitude"] != null)
            {
                objLocInfo.longitude = Convert.ToDouble(Request.QueryString["Longitude"]);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    #endregion

    #region prefillpage
    protected void prefillpage()
    {
        try
        {
            objLocInfo = LocationInfo.getLocationInfoObj;

            if (!string.IsNullOrWhiteSpace(objLocInfo.CountryCode))
            {
                if (ddlCountry.Items.FindByValue(objLocInfo.CountryCode) != null)
                {
                    ddlCountry.SelectedValue = objLocInfo.CountryCode;
                    //if (objLocInfo.searchLocation != searchbox.Value)
                    //{
                    searchbox.Value = objLocInfo.searchLocation == "" ? objLocInfo.placeName : objLocInfo.searchLocation;
                    if (!string.IsNullOrWhiteSpace(searchbox.Value))
                    {
                        loadLocationInfo();
                    }
                    //}
                }

            }
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.MOB_PREFILL_ERROR) + " : " + ex.Message.ToString();
            return;
        }
    }
    #endregion

    #region createMenu
    private void createMenu()
    {
        objSvcInfo.IsMobile = true;
        DataTable dtPages = ServiceObj.loadMenuTabs();

        int num = dtPages.Rows.Count;
        HtmlGenericControl ul = new HtmlGenericControl();
        ul.TagName = "ul";
        for (int i = 0; i < num; i++)
        {
            HtmlGenericControl lt = new HtmlGenericControl();
            string liid = "tab_mresult_" + dtPages.Rows[i][0].ToString();
            lt.ID = liid;
            lt.TagName = "li";
            if (dtPages.Rows[i][0].ToString() == objSvcInfo.Module.ToString())
                lt.Attributes.Add("class", "active");
            lt.InnerHtml = "<a href=\"ChartView.aspx?module=" + dtPages.Rows[i][0].ToString() + "\">"
                + dtPages.Rows[i][0].ToString()
                + "</a>";

            ul.Controls.Add(lt);
        }
        // MenuTabs.Controls.Add(ul);
    }
    #endregion

    #region LoadStation
    public void LoadStation()
    {
        try
        {
            //Load Stations
            objDataPointInfo = DataPointInfo.getDataPointObject;

            NearByPointSettings arNearBydata = objNearByPre.setNearbyStationAttributes();
            if (arNearBydata != null )
            {
                objDataPointInfo.NearbyPointSettings = arNearBydata;
            }
            objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            objDataPointInfo = DataPointInfo.getDataPointObject;
            string strStation = objDataPointInfo.stationName;

            DataTable dtNearByStations = nearBySvc.getNearbydata(objLocInfo.latitude, objLocInfo.longitude);
            dtNearByStations.TableName = "Stations";

            if (dtNearByStations.Rows.Count != 0)
            {
                dtNearByStations.Columns.Add("DistText", typeof(string));
                dtNearByStations.Columns["DistText"].DefaultValue = string.Empty;

                for (int i = 0; i < dtNearByStations.Rows.Count; i++)
                {
                    string strText = locSvc.getTranslatedText("ResDistanceFrom", objSvcInfo.Culture);
                    strText = strText.Replace("{Dist}", Convert.ToString(Math.Round(Convert.ToDouble(dtNearByStations.Rows[i]["DistanceKm"].ToString()))));
                    strText = strText.Replace("{Dir}", objComUtil.getTextDirection(Convert.ToInt32(dtNearByStations.Rows[i]["BearingDegrees"].ToString())));
                    strText = strText.Replace("{CityName}", objLocInfo.placeName);
                    strText = strText.Replace("{Elevation}", dtNearByStations.Rows[i]["Altitude"].ToString());
                    dtNearByStations.Rows[i].BeginEdit();
                    dtNearByStations.Rows[i]["DistText"] = strText;
                    dtNearByStations.Rows[i].EndEdit();
                    dtNearByStations.AcceptChanges();
                }

                Array ar = null;
                if (strStation != null && strStation != "")
                    ar = dtNearByStations.Select("Name = '" + strStation + "'");
                if (ar != null && ar.Length > 0)
                {
                    DataRow dr = (DataRow)ar.GetValue(0);
                    objDataPointInfo = DataPointInfo.getDataPointObject;

                    objDataPointInfo.stationName = dr["Name"].ToString();
                    objDataPointInfo.stationLatitude = Convert.ToDouble(dr["Latitude"].ToString());
                    objDataPointInfo.stationLongitude = Convert.ToDouble(dr["Longitude"].ToString());
                    objDataPointInfo.SunRise = objComUtil.getSunrise(DateTime.Now.Date, Convert.ToDouble(dr["Latitude"].ToString()), Convert.ToDouble(dr["Longitude"].ToString()));
                    objDataPointInfo.SunSet = objComUtil.getSunset(DateTime.Now.Date, Convert.ToDouble(dr["Latitude"].ToString()), Convert.ToDouble(dr["Longitude"].ToString()));
                    objDataPointInfo.directionLetter = objComUtil.getTextDirection(Convert.ToInt32(dr["BearingDegrees"].ToString()));
                    //objDataPointInfo.DayOffset = locSvc.getTimeZoneOffset(Convert.ToInt32(dr[0].ToString()));
                    objDataPointInfo.altitude = Convert.ToInt32(dr["Altitude"].ToString() == "" ? "0" : dr["Altitude"].ToString());
                    objDataPointInfo.DayOffset = Convert.ToInt32(dr["TimezoneOffset"].ToString() == "" ? "0" : dr["TimezoneOffset"].ToString());
                    objDataPointInfo.iDstOn = Convert.ToInt32(dr["DstOn"].ToString() == "" ? "0" : dr["DstOn"].ToString());
                }
                else
                {

                    objDataPointInfo.stationName = dtNearByStations.Rows[0]["Name"].ToString();
                    objDataPointInfo.stationLatitude = Convert.ToDouble(dtNearByStations.Rows[0]["Latitude"].ToString());
                    objDataPointInfo.stationLongitude = Convert.ToDouble(dtNearByStations.Rows[0]["Longitude"].ToString());
                    objDataPointInfo.SunRise = objComUtil.getSunrise(DateTime.Now.Date, Convert.ToDouble(dtNearByStations.Rows[0]["Latitude"].ToString()), Convert.ToDouble(dtNearByStations.Rows[0]["Longitude"].ToString()));
                    objDataPointInfo.SunSet = objComUtil.getSunset(DateTime.Now.Date, Convert.ToDouble(dtNearByStations.Rows[0]["Latitude"].ToString()), Convert.ToDouble(dtNearByStations.Rows[0]["Longitude"].ToString()));
                    objDataPointInfo.directionLetter = objComUtil.getTextDirection(Convert.ToInt32(dtNearByStations.Rows[0]["BearingDegrees"].ToString()));
                    //objDataPointInfo.DayOffset = locSvc.getTimeZoneOffset(Convert.ToInt32(dtNearByStations.Rows[0][0].ToString()));
                    objDataPointInfo.altitude = Convert.ToInt32(dtNearByStations.Rows[0]["Altitude"].ToString() == "" ? "0" : dtNearByStations.Rows[0]["Altitude"].ToString());
                    objDataPointInfo.DayOffset = Convert.ToInt32(dtNearByStations.Rows[0]["TimezoneOffset"].ToString() == "" ? "0" : dtNearByStations.Rows[0]["TimezoneOffset"].ToString());
                    objDataPointInfo.iDstOn = Convert.ToInt32(dtNearByStations.Rows[0]["DstOn"].ToString() == "" ? "0" : dtNearByStations.Rows[0]["DstOn"].ToString());
                }
                // NumOfLocs = dtNearByStations.Rows.Count;
            }
            else
            {
                objDataPointInfo.stationName = "";
                objDataPointInfo.stationLatitude = 0.0;
                objDataPointInfo.stationLongitude = 0.0;
            }
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.MOB_LOADSTATION_ERROR) + " : " + ex.Message.ToString();
            return;
        }
        finally
        {
            if (Session["AuditData"] != null)
            {
                IDictionary dict = (IDictionary)Session["AuditData"];
                dict["weatherDatasource"] = objDataPointInfo.NearbyPointSettings.DataSource;
                dictAudit["weatherLat"] = objDataPointInfo.stationLatitude;
                dictAudit["weatherLong"] = objDataPointInfo.stationLongitude;
                Session["AuditData"] = dict;
            }
        }
    }
    #endregion

    #region IsEmpty<T>
    /// <summary>
    /// For checking if object is empty. Checks for any kind of object
    /// </summary>
    /// <typeparam name="T">Datatype</typeparam>
    /// <param name="obj">object of type T</param>
    /// <returns>Boolean</returns>
    private bool IsEmpty<T>(dynamic obj)
    {
        object property_value = null;

        bool flag = false;

        System.Reflection.PropertyInfo[] properties_info = typeof(T).GetProperties();

        foreach (System.Reflection.PropertyInfo prop in properties_info)
            if (prop != null)
            {
                property_value = prop.GetValue(obj, null);
                switch (prop.PropertyType.Name.ToString().ToLower())
                {
                    case "int32":
                        flag = Int32.Parse(property_value.ToString()) == 0;
                        break;
                    case "double":
                        flag = double.Parse(property_value.ToString()) == 0.0;
                        break;
                    case "string":
                        flag = property_value.ToString() == "";
                        break;
                    case "datetime":
                        flag = DateTime.Parse(property_value.ToString()) == DateTime.MinValue || DateTime.Parse(property_value.ToString()) == DateTime.MaxValue;
                        break;
                }
            }

        return flag;
    }
    #endregion

    #region LoadModules
    void loadModules()
    {
        try
        {
            //Load the Icon and Table Data
            //Get the Module first. If you are redirected from the webpage you will not have the mobile module yet.
            List<string[]> list = ServiceObj.getNodeList(SERVICENAME);
            if (list.Count > 0)
                objSvcInfo.Module = list[0][1].ToString();

            if (objDataPointInfo != null && objDataPointInfo.stationName != "" && objDataPointInfo.stationLatitude != 0.0 && objDataPointInfo.stationLongitude != 0.0)
            {
                //Get the NodeList
                List<string[]> objList = ServiceObj.getNodeList(SERVICEPAGENAME);
                string Node = string.Empty;
                string Name = string.Empty;

                for (int i = 0; i < objList.Count; i++)
                {
                    if (objList[i].Contains("icon") && objList[i][1].ToString().ToLower().Contains("summary"))
                    {
                        //Name = objList[i][1].ToString();
                        //Node = objList[i][0].ToString();
                        // objIconPre = new IconPresenter(this, Name);

                        //Get The Icon data
                        //objIconPre.getIconData();
                        alIocnList = objMobPresenter.GetIconData(objList[i][1].ToString());
                    }
                    //if (objList[i].Contains("tblDaysRows"))
                    //{
                    //    Name = objList[i][1].ToString();
                    //    Node = objList[i][0].ToString();
                    //    objTablePre = new TablePresenter(this, Name);

                    //    DisplayTableData();
                    //}
                    if (objList[i].Contains("tblSeriesRows") && objList[i][1].ToString().ToLower().Contains("summary"))
                    {
                        Name = objList[i][1].ToString();
                        Node = objList[i][0].ToString();
                        //objTablePre = new TablePresenter(this, Name);
                        objMobPresenter = new MobilePresenter();
                        DataSet ds = new DataSet();
                        ds = objMobPresenter.GetTableData(alIocnList, Node, Name);
                        dtByDaysLoc = ds.Tables[1];
                        dtSeriesnames = ds.Tables[0];

                        /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
                        /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */
                        //ChangeColNames(dtByDaysLoc);
                        /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */

                        //Create the HTML Code for single display
                        //CreateHTML();

                    }
                    //This is for spray window on the main page.
                    //if (objList[i].Contains("tblDaysRows"))
                    //{
                    //    //Load spray window user control
                    //    PropertyInfo propertyInfoName = ucSprayWindowImg.GetType().GetProperty("Name");
                    //    propertyInfoName.SetValue(ucSprayWindowImg, Convert.ChangeType(objList[i][1].ToString(), propertyInfoName.PropertyType), null);

                    //    PropertyInfo propertyInfoNode = ucSprayWindowImg.GetType().GetProperty("Node");
                    //    propertyInfoNode.SetValue(ucSprayWindowImg, Convert.ChangeType(objList[i][0].ToString(), propertyInfoNode.PropertyType), null);

                    //    PropertyInfo propertyInfoDate = ucSprayWindowImg.GetType().GetProperty("Date");
                    //    propertyInfoDate.SetValue(ucSprayWindowImg, Convert.ChangeType(DateTime.Now, propertyInfoDate.PropertyType), null);
                    //}
                }
                CreateHTML();
            }
            else
            {
                divLocation.Attributes.Add("class", "hide");
                lblNoMatch.Visible = true;
            }

        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.MOB_LOADMODULE_ERROR) + " : " + ex.Message.ToString();
            return;
        }

    }
    #endregion

    public string GetFirstDateTime()
    {
        loadModules();

        string firstDateTime = string.Empty;
        if (dtByDaysLoc != null)
        {
            if (dtByDaysLoc.Rows.Count > 0)
            {
                firstDateTime = dtByDaysLoc.Rows[0]["NewDate"].ToString();
            }
        }

        return firstDateTime;
    }

    string getTranslatedText(string text)
    {
        if (Session["serviceInfo"] != null)
        {
            objSvcInfo = (ServiceInfo)Session["serviceInfo"];
        }
        else
        {
            objSvcInfo = ServiceInfo.ServiceConfig;
        }
        try
        {
            string strCul = objSvcInfo.Culture;
            objSvcPre = new ServicePresenter();
            text = objSvcPre.getTranslatedText(text, strCul);
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.MOB_TRANS_ERROR) + " : " + ex.Message.ToString();

        }
        return text;
    }

    void ChangeLabelText()
    {
        lblCountry.InnerText = getTranslatedText("SearchIn");
        lblSearchbox.InnerText = getTranslatedText("for");
        lblNoMatch.Text = getTranslatedText("No Results Found");
        /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
        /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */
        //lblMax.Text = getTranslatedText("Max: ");
        //lblMin.Text = getTranslatedText("Min: ");
        //lblWind.Text = getTranslatedText("Wind: ");
        //lblRain.Text = getTranslatedText("Rain: ");
        /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */
        Chart.InnerText = getTranslatedText("Forecast Chart");
        SprayWindow.InnerText = getTranslatedText("Spray Window");
        DayForecast.InnerText = getTranslatedText("5 Day Forecast");
        units.InnerText = getTranslatedText("Settings");
        hSearchLoc.InnerText = getTranslatedText("Search Location");
        btnSearch.Text = getTranslatedText("Search");



    }

    private void AddAuditdata()
    {
        if (Session["AuditData"] == null)
        {

            dictAudit["userIP"] = HttpContext.Current.Request.UserHostAddress;
            dictAudit["userID"] = "";
            dictAudit["token"] = "";
            dictAudit["referrer"] = (HttpContext.Current.Request.UrlReferrer == null) ? "none" : HttpContext.Current.Request.UrlReferrer.ToString();
            dictAudit["entrancePath"] = "Mobile";
            dictAudit["culture"] = objSvcInfo.Culture;
            dictAudit["sessionID"] = HttpContext.Current.Session.SessionID;
            dictAudit["service"] = objSvcInfo.ServiceName;
            dictAudit["module"] = objSvcInfo.Module;
            dictAudit["locSearchType"] = "By Cookie";
            dictAudit["locSearchStringType"] = "";
            dictAudit["locSearchString"] = objLocInfo.searchLocation;
            dictAudit["locSearchDatasource"] = objLocInfo.Provider;
            dictAudit["numOfLocs"] = "";
            dictAudit["searchLat"] = objLocInfo.latitude;
            dictAudit["searchLong"] = objLocInfo.longitude;
            dictAudit["countryName"] = (objLocInfo.CountryCode == null) ? "" : objLocInfo.CountryCode;
            dictAudit["locName"] = objLocInfo.placeName;
            dictAudit["weatherDatasource"] = objDataPointInfo.NearbyPointSettings.DataSource;
            dictAudit["weatherLat"] = objDataPointInfo.stationLatitude;
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
            dict["sessionID"] = HttpContext.Current.Session.SessionID;
            dict["locSearchDatasource"] = objLocInfo.Provider;
            dict["numOfLocs"] = "";
            dict["searchLat"] = objLocInfo.latitude;
            dict["searchLong"] = objLocInfo.longitude;
            dict["countryName"] = (objLocInfo.CountryCode == null) ? "" : objLocInfo.CountryCode;
            dict["locName"] = objLocInfo.placeName;
            dict["weatherDatasource"] = objDataPointInfo.NearbyPointSettings.DataSource;
            dict["weatherLat"] = objDataPointInfo.stationLatitude;
            dict["weatherLong"] = objDataPointInfo.stationLongitude;
            Session["AuditData"] = dict;
        }

    }
    #endregion

    #region ILocSearch Members

    //public string defaultLocation
    //{
    //    get
    //    {
    //        throw new NotImplementedException();
    //    }
    //    set
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public string selCountry
    //{
    //    get
    //    {
    //        throw new NotImplementedException();
    //    }
    //    set
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public string searchString
    //{
    //    get
    //    {
    //        throw new NotImplementedException();
    //    }
    //    set
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public string setCntry
    //{
    //    get
    //    {
    //        throw new NotImplementedException();
    //    }
    //    set
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    public bool showMap
    {
        get;
        set;
    }
    public DataTable dtNearByPoints
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

    //public string nearbyName
    //{
    //    get
    //    {
    //        throw new NotImplementedException();
    //    }
    //    set
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public string nearbyText
    //{
    //    get
    //    {
    //        throw new NotImplementedException();
    //    }
    //    set
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public string getLatLong
    //{
    //    get
    //    {
    //        throw new NotImplementedException();
    //    }
    //    set
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public DataTable dtCountry
    {
        get
        {
            throw new NotImplementedException();
        }
        set
        {
            ddlCountry.DataSource = value;
            ddlCountry.DataTextField = "Value";
            ddlCountry.DataValueField = "code";
            ddlCountry.DataBind();
            Session["CountryCode"] = ddlCountry.SelectedValue;
        }
    }

    public string strTranslatedText
    {
        get
        {
            return strText;
        }

        set
        {
            strText = value;
        }
    }

    public string strCultureCode
    {
        get;
        set;
    }

    public List<Location> LocationList
    {
        get
        {
            throw new NotImplementedException();
        }
        set
        {
            LocationSearchSource eProvider = (LocationSearchSource)Session["Provider"];
            objLocInfo.Provider = eProvider;
            if (eProvider == LocationSearchSource.Geonames )
            {
                if (value.Count > 1)
                {
                    //take only the first value from the table
                    lblLocationName.Text = value[0].Name;
                    hdnLatLng.Value = value[0].Latitude.ToString() + "|" + value[0].Longitude.ToString();
                    hdnLocSelected.Value = value[0].Name + "|" + value[0].AdminName1;
                    CultureInfo currCulture = new CultureInfo(objSvcInfo.Culture);
                    //lblDateTime.Text = DateTime.Now.ToString("ddddd dd/MM", currCulture);
                    lblDateTime.Text = GetFirstDateTime();
                    NumOfLocs = value.Count;
                }
                else
                {
                    hdnLatLng.Value = string.Empty;
                    hdnLocSelected.Value = string.Empty;
                }

            }
            else
            {
                if (value != null && value.Count > 0)
                {
                    if (value.Count == 1)
                    {
                        //take only the first value from the table
                        lblLocationName.Text = value[0].Name;
                        hdnLatLng.Value = value[0].Latitude.ToString() + "|" + value[0].Longitude.ToString();
                        hdnLocSelected.Value = value[0].Name + "|" + value[0].AreaId.ToString();
                        CultureInfo currCulture = new CultureInfo(objSvcInfo.Culture);
                        //lblDateTime.Text = DateTime.Now.ToString("ddddd dd/MM", currCulture);
                        lblDateTime.Text = GetFirstDateTime();
                        lblNoMatch.Visible = false;
                        divSelectSingleLoc.Visible = false;
                    }
                    else if (value.Count > 1)
                    {
                        CultureInfo currCulture = new CultureInfo(objSvcInfo.Culture);
                        //lblDateTime.Text = DateTime.Now.ToString("ddddd dd/MM", currCulture);
                        lblDateTime.Text = GetFirstDateTime();
                        lblNoMatch.Visible = false;

                        DataTable dtMultipleLocations = new DataTable();
                        dtMultipleLocations.Columns.Add("value");
                        dtMultipleLocations.Columns.Add("text");
                        DataRow dr;

                        for (int i = 0; i < value.Count; i++)
                        {
                            dr = dtMultipleLocations.NewRow();
                            dr["value"] = value[i].Latitude.ToString() + "," + value[i].Longitude.ToString();
                            dr["text"] = value[i].Name + value[i].AdminName1 +value[i].AdminName2;
                            dtMultipleLocations.Rows.Add(dr);
                        }

                        ddlMultiplePlaces.DataValueField = "value";
                        ddlMultiplePlaces.DataTextField = "text";

                        ddlMultiplePlaces.DataSource = dtMultipleLocations;
                        ddlMultiplePlaces.DataBind();

                        divSelectSingleLoc.Visible = true;
                        divSearchLoc.Attributes.Add("data-collapsed", "false");
                    }
                    else
                    {
                        CultureInfo currCulture = new CultureInfo(objSvcInfo.Culture);
                        //lblDateTime.Text = DateTime.Now.ToString("ddddd dd/MM", currCulture);
                        lblDateTime.Text = GetFirstDateTime();
                        lblNoMatch.Visible = true;
                        divSelectSingleLoc.Visible = false;
                    }
                }
            }
        }
    }

    public DataTable dtFavorites
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

    public LocationSearchSource eProviderName
    {
        get
        {
            return eProvider;
        }
        set
        {
            eProvider = value;
            Session["Provider"] = eProvider;
        }
    }

    public DataTable getCountry()
    {
        throw new NotImplementedException();
    }

    public string getTranslatedText(string strLabelName, string strLangID)
    {
        throw new NotImplementedException();
    }

    public List<Location> getLocationDetails(string strPlaceName, string strCountry, string strCultureCode, LocationSearchSource eProvider)
    {
        throw new NotImplementedException();
    }

    public DataSet getNearByStations(double dlat, double dlong, int intMaxAllowedDist, int intMaxAllowedAlt, int intResultCount)
    {
        throw new NotImplementedException();
    }

    public DateTime sunRiseTime
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

    public DateTime sunSetTime
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


    public int iTimeZoneOffset
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

    public int iDstOn
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
    public string Version()
    {
        throw new NotImplementedException();
    }
    #endregion

}
#endregion