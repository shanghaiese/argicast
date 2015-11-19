#region Comment Header
/**************************************************************************************************
**		File Name		: AgriCastHandler.cs
**		Functionality	: This Class is code behind for AgriCastHandler.ashx which will be used
**                        for generating agricast modules from javscript using Agricast.js
**      Namespace       : 
**      Class			: AgriCastHandler
**	    Author			: Infosys Technologies Limited
**		Date			: 15-Jan-2012
***************************************************************************************************
**		Change History
***************************************************************************************************
**		Date:	    	Author:				  Description:
**		--------	    --------			  -----------------
**      15-Jan-2012      Infosys               
******************************************************************************************************/
#endregion

#region Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Globalization; 
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Threading;
using System.Data;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Web.SessionState;
using System.Web.Script.Serialization;
using System.Reflection;

using Syngenta.AgriCast;
using Syngenta.AgriCast.Common;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common.View;
using Syngenta.AgriCast.LocationSearch.Presenter;
using Syngenta.AgriCast.LocationSearch.Service;
using Syngenta.AgriCast.LocationSearch.LocWebService;
using Syngenta.AgriCast.AgriInfo;
using Syngenta.AgriCast.Icon;
using Syngenta.AgriCast.Tables;
using Syngenta.AgriCast.Charting.Presenter;
using Syngenta.AgriCast.Charting.View;
using ChartFX.WebForms;
using Syngenta.AgriCast.Charting;
using System.Text.RegularExpressions;
using System.Configuration;
using Syngenta.AgriWeb.LocationSearch.DataObjects;

#endregion

#region Class
public class AgriCastHandler : IHttpHandler, IRequiresSessionState
{

    #region Declaration
    string method;
    string culture;
    string pub;
    string key;
    string country;
    string place;
    string lat;
    string lng;
    string dist;
    string alt;
    string module;
    string stnname;
    //default to true
    bool isLatLongValid = true;

    ServiceInfo objSvcInfo;
    ServicePresenter objSvcPre;
    service svc;
    ServiceHandler sh;
    LocationInfo objLocInfo;
    LocSearchWebService objLocWebSer;
    locSearchService locSvc;
    DataPointInfo objDataPointInfo;
    nearbyPointPresenter objNearByPre;
    nearbyPointService nearBySvc;
    CommonUtil objComUtil;
    const string BOXMINUS = "~/Images/boxminus.gif";
    string LABEL250 = "label250";
    #endregion

    #region Constants
    private const string DEFAULT_PUB = "Demo";
    private const string DEFAULT_CULTURE = "en-GB";
    private const string DEFAULT_KEY = "";
    private const string DEFAULT_COUNTRY = "";
    private const string DEFAULT_PLACE = "";
    private const string DEFAULT_LAT = "0.0";
    private const string DEFAULT_LNG = "0.0";
    private const string DEFAULT_STNNAME = "";
    private const string DEFAULT_DIST = "0";
    private const string DEFAULT_ALT = "0";
    private const string DEFAULT_MODULE = "Weathercast";
    private const string CONTROLS_ID_PREFIX = "ag_";
    private string APPLICATION_URL = "http://localhost/";


    #region CSS Constants

    #region LocationSearch Constants
    private const string CSS_CLASS_CONTAINER = "container";
    private const string CSS_CLASS_DROPDOWN = "dropdown";
    private const string CSS_CLASS_SEARCHBOX = "searchbox";
    private const string CSS_CLASS_SEARCHINPUT = "seachInput";
    private const string CSS_CLASS_LABEL100 = "label100";
    private const string CSS_CLASS_LABEL50 = "label50";
    private const string CSS_CLASS_SPLIT = "split";
    private const string CSS_CLASS_HIDE = "hide";
    private const string CSS_CLASS_SHOW = "show";
    #endregion

    #endregion

    #endregion 

    #region Main-ProcessRequest
    public void ProcessRequest(HttpContext context) 
    {        
        
        #region GetQuerystringValues
        method = context.Request.QueryString["method"].ToString();
        culture = (context.Request.QueryString["culture"] ?? DEFAULT_CULTURE).ToString();
        pub = (context.Request.QueryString["pub"] ?? DEFAULT_PUB).ToString();
        key = (context.Request.QueryString["key"] ?? DEFAULT_KEY).ToString();
        country = (context.Request.QueryString["country"] ?? DEFAULT_COUNTRY).ToString().ToUpper();
        place = (HttpUtility.HtmlDecode(context.Request.QueryString["place"]) ?? DEFAULT_PLACE).ToString();
        lat = (context.Request.QueryString["lat"] ?? DEFAULT_LAT).ToString();
        lng = (context.Request.QueryString["lng"] ?? DEFAULT_LNG).ToString();
        dist = (context.Request.QueryString["dist"] ?? DEFAULT_DIST).ToString();
        alt = (context.Request.QueryString["alt"] ?? DEFAULT_ALT).ToString();
        module = (context.Request.QueryString["module"] ?? "").ToString();
        stnname = (context.Request.QueryString["stnname"] ?? DEFAULT_STNNAME).ToString();

        StringBuilder appurl = new StringBuilder("");
        appurl.Append(context.Request.Url.ToString().Split(new string[] { context.Request.Url.LocalPath }, StringSplitOptions.None)[0].ToString());
        appurl.Append(context.Request.Url.LocalPath.Split(new string[] { "AgriCastHandler.ashx" }, StringSplitOptions.None)[0].ToString());
        APPLICATION_URL = appurl.ToString();

        
        #endregion

        #region Initialize Agricast Objects
        if (HttpContext.Current.Session == null || HttpContext.Current.Session["serviceInfo"] == null)
        {
            objSvcInfo = new ServiceInfo();
        }
        else
        {
            objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
        }
        objLocInfo = new LocationInfo();
        objLocWebSer = new LocSearchWebService();
        objNearByPre = new nearbyPointPresenter(null);
        objSvcPre = new ServicePresenter();
        locSvc = new locSearchService();
        nearBySvc = new nearbyPointService();        
        objComUtil = new CommonUtil();

        HttpCookie locCookie = HttpContext.Current.Request.Cookies["ais_LocationInfo"];

        if (locCookie != null)
        {
            LocationInfo newInfo = objLocInfo.DeserializeCookieString(locCookie.Value);
            if (newInfo != null)
            {
                HttpContext.Current.Session["objLocationInfo"] = newInfo;
                objLocInfo = newInfo;
            }
        }

        objLocInfo.placeName = place;
        objLocInfo.CountryCode = country;
        objLocInfo.latitude = double.Parse(lat);
        objLocInfo.longitude = double.Parse(lng);

        if ((objLocInfo.placeName == "" || (objLocInfo.latitude == 0d && objLocInfo.longitude == 0d)) && objLocInfo.CountryCode == "")
        {
            objSvcPre.setDefaultLoc();
        }
        sh = new ServiceHandler();
        objSvcInfo = new ServiceInfo(); 

        ServiceInfo.ServiceConfig = objSvcInfo;
        objSvcInfo.ServiceName = pub;
 
        svc = objSvcPre.readConfig();
        objSvcPre.createServiceSession();


        if (module == "")
        {
            module = sh.getDefaultPage();
        }

        if (string.IsNullOrWhiteSpace(objSvcInfo.Module))
        {
            objSvcInfo.Module = module;
        }
        if (string.IsNullOrWhiteSpace(objSvcInfo.Culture))
        {
            objSvcInfo.Culture = culture;
        }
        #endregion

       

        context.Response.ContentType = "application/json";
        context.Response.ContentEncoding = Encoding.UTF8;

        switch (method.ToLower())
        {
            //for getting location search UI and populating the country dropdown
            case "locationsearch":
                //context.Response.Write(GetLocationSearch(culture, pub));
                context.Response.Write(string.Format("{0}({1});", context.Request["callback"], GetLocationSearch(culture, pub)));
                break;
            //for getting Autocomplete values for search textbox
            case "locdetauto":
                //context.Response.Write(GetLocationDetails(place,country,culture));
                context.Response.Write(string.Format("{0}({1});", context.Request["callback"], GetLocDetailsAuto(place, country, culture)));
                break;
            //for getting location and station information to populate tables.
            case "locdetstn":
                //context.Response.Write(GetLocationDetails(place,country,culture));
                context.Response.Write(string.Format("{0}({1});", context.Request["callback"], GetLocDetStations(place, country, culture)));
                break;
            //for getting all Modules requested from pub
            case "getmodules":
                //context.Response.Write(GetLocationDetails(place,country,culture));
                context.Response.Write(string.Format("{0}({1});", context.Request["callback"], GetModules(pub, module)));
                break;
        }

        #region Write Cookie to Response
        objLocInfo = LocationInfo.getLocationInfoObj;
        objLocInfo.placeName = place;
        objLocInfo.searchLocation = place;
        double latVal;
        double.TryParse(objLocInfo.latitude.ToString() == "0.0" || objLocInfo.latitude.ToString() == "0" ? lat.ToString() : objLocInfo.latitude.ToString(), out latVal);
        objLocInfo.latitude = latVal;
        double lngVal;
        double.TryParse(objLocInfo.longitude.ToString() == "0.0" || objLocInfo.longitude.ToString() == "0" ? lng.ToString() : objLocInfo.longitude.ToString(), out lngVal);
        objLocInfo.longitude = lngVal;
        objLocInfo.AdminName = "";

        if (!string.IsNullOrWhiteSpace(country))
        {
            objLocInfo.CountryCode = country;
        }

        HttpCookie LocationCookie = new HttpCookie("ais_LocationInfo", objLocInfo.SerializeCookieString());
        LocationCookie.Expires = DateTime.Now.AddDays(365); 
        
        if (locCookie != null)
        {
            LocationCookie.Path = locCookie.Path;
        }
        HttpContext.Current.Response.Cookies.Add(LocationCookie);         

        #endregion

    }
    #endregion

    #region Methods

    #region GetLocationSearch
    /// <summary>
    /// Get location search UI with country dropdown populated
    /// </summary>
    /// <param name="culture">current culture eg: en-GB</param>
    /// <param name="pub">publication to use</param>
    /// <returns>Json string</returns>
    protected string GetLocationSearch(string culture, string pub)
    {
        /////////////////////////////////////////////////////////////
        //create panel add controls to it and then render the panel//
        ///////////////////////////////////////////////////////////// 
        //Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        //double d = 100.75;
        //Label l = new Label();
        //l.Text = d.ToString("C");

        #region Generate HTML Controls

        #region First Controls
        Panel chgLoc = new Panel();
        chgLoc.ID = CONTROLS_ID_PREFIX + "ChangeLoc";
        chgLoc.CssClass = "hide";
        //chgLoc.Style.Add("display", "none");

        HtmlAnchor cngloc = new HtmlAnchor();
        cngloc.ID = "cngLoc";
        cngloc.HRef = "#";
        cngloc.InnerText = locSvc.getTranslatedText("ChangeLocation", objSvcInfo.Culture);

        HtmlAnchor cngstn = new HtmlAnchor();
        cngstn.ID = "cngStat";
        cngstn.HRef = "#";
        cngstn.InnerHtml = locSvc.getTranslatedText("ResNearbyStations", objSvcInfo.Culture);
        

        chgLoc.Controls.Add(cngloc);
        chgLoc.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;"));
        chgLoc.Controls.Add(cngstn);

        Panel p = new Panel();
        p.ID = CONTROLS_ID_PREFIX + "divLocSearch";
        p.CssClass = CSS_CLASS_CONTAINER;

        Label lblCountry = new Label();
        lblCountry.ID = CONTROLS_ID_PREFIX + "lblCountry";
        //lblCountry.Style.Add("width",Unit.Percentage(50).ToString());
        lblCountry.Text = locSvc.getTranslatedText("searchin", objSvcInfo.Culture)+":";

        DropDownList ddlCountry = new DropDownList();
        ddlCountry.ID = CONTROLS_ID_PREFIX + "ddlCountry";
        //ddlCountry.Style.Add("width", Unit.Percentage(100).ToString());
        ddlCountry.DataSource = GetCountries(pub);
        ddlCountry.DataTextField = "Value";
        ddlCountry.DataValueField = "code";
        ddlCountry.DataBind();
        country = objLocInfo.CountryCode;
        place = objLocInfo.placeName;
        if (!string.IsNullOrWhiteSpace(country) && ddlCountry.Items.FindByValue(country.ToUpper()) != null)
        {
            ddlCountry.SelectedValue = country.ToUpper();
        }


        Label lblFor = new Label();
        lblFor.ID = CONTROLS_ID_PREFIX + "lblFor";
        lblFor.Text = locSvc.getTranslatedText("for", objSvcInfo.Culture);

        TextBox txtPlace = new TextBox();
        //txtPlace.CssClass = CSS_CLASS_SEARCHINPUT;
        txtPlace.ID = CONTROLS_ID_PREFIX + "txtPlace";
        //txtPlace.Style.Add("width", Unit.Percentage(100).ToString());
        txtPlace.Attributes.Add("placeholder", "Enter the search criteria");
        txtPlace.Attributes.Add("autofocus", "autofocus");
        txtPlace.AutoCompleteType = AutoCompleteType.Disabled;

        if (!string.IsNullOrWhiteSpace(place))
        {
            txtPlace.Text = place;
        }

        HtmlImage imgSearch = new HtmlImage();
        imgSearch.ID = CONTROLS_ID_PREFIX + "btnSearch";
        imgSearch.Src = APPLICATION_URL + "Images/Search.png";
        imgSearch.Alt = locSvc.getTranslatedText("Search", objSvcInfo.Culture);

        Panel pnlStnName = new Panel();
        pnlStnName.ID = "ag_stnName";
        Label lblPointText = new Label();
        lblPointText.ID = CONTROLS_ID_PREFIX + "lblPointText";
        lblPointText.CssClass = "lblDataPt";
        lblPointText.Text = objSvcPre.getTranslatedText("Forecast for", objSvcInfo.Culture); 
        lblPointText.Visible = true;        
        Label lblStnName = new Label();
        lblStnName.ID = CONTROLS_ID_PREFIX + "lblStnName";
        lblStnName.CssClass = "lblDataPt";
        lblStnName.Text = "";
        lblStnName.Visible = true;
        Label lblDistText = new Label();
        lblDistText.ID = CONTROLS_ID_PREFIX + "lblDistText";
        lblDistText.CssClass = "split";
        lblDistText.Text = "";
        lblDistText.Visible = true;

        //LinkButton lnkNearBy = new LinkButton();
        //lnkNearBy.ID = CONTROLS_ID_PREFIX + "hNearByPoint";
        //lnkNearBy.Text = "NearByStations";


        pnlStnName.Controls.Add(lblPointText);
        pnlStnName.Controls.Add(lblStnName);
        pnlStnName.Controls.Add(lblDistText);
        //pnlStnName.Controls.Add(lnkNearBy);


        p.Controls.Add(lblCountry);
        p.Controls.Add(ddlCountry);
        p.Controls.Add(lblFor);
        p.Controls.Add(txtPlace);
        p.Controls.Add(imgSearch);
        p.Controls.Add(pnlStnName);
        #endregion

        #region Search Results
        Panel pnlSearchRes = new Panel();
        pnlSearchRes.ID = CONTROLS_ID_PREFIX + "divLocation";
        pnlSearchRes.CssClass = CSS_CLASS_HIDE;

        Label lblNoRes = new Label();
        lblNoRes.ID = CONTROLS_ID_PREFIX + "Loc_NoMatchFor";
        lblNoRes.Text = locSvc.getTranslatedText("Loc_NoMatchFor", objSvcInfo.Culture).Replace("{PlaceName}", objLocInfo.placeName);
        lblNoRes.CssClass = "hide";

        Table gvLocation = new Table();
        gvLocation.ID = CONTROLS_ID_PREFIX + "gvLocation";
        gvLocation.Attributes.Add("border", "1");
        gvLocation.BorderWidth = 1;        
        //gvLocation.CellSpacing = 0;        
        
        TableRow tr = new TableRow();
        TableHeaderCell th = new TableHeaderCell();
        th.Text = locSvc.getTranslatedText("name", objSvcInfo.Culture);
        tr.Controls.Add(th);
        gvLocation.Controls.Add(tr);
        
        pnlSearchRes.Controls.Add(lblNoRes);
        pnlSearchRes.Controls.Add(gvLocation);
        #endregion

        #region Stations
        Panel pStations = new Panel();
        pStations.ID = CONTROLS_ID_PREFIX + "divStations";
        pStations.CssClass = CSS_CLASS_HIDE;

        Label lblNoStat = new Label();
        lblNoStat.ID = CONTROLS_ID_PREFIX + "Loc_NoStation";
        lblNoStat.Text = locSvc.getTranslatedText("No stations found", objSvcInfo.Culture);
        lblNoStat.CssClass = "hide";

        Table gvStations = new Table();
        gvStations.ID = CONTROLS_ID_PREFIX + "gvStations";
        gvStations.Attributes.Add("border", "1");
        gvStations.BorderWidth = 1;
        //gvStations.CellSpacing = 0;

        TableRow stntr = new TableRow();
        TableHeaderCell stnname = new TableHeaderCell();
        stnname.Text = locSvc.getTranslatedText("NearStationName", objSvcInfo.Culture);
        stntr.Controls.Add(stnname);
        TableHeaderCell stnDist = new TableHeaderCell();
        stnDist.Text = locSvc.getTranslatedText("NearDistance", objSvcInfo.Culture);
        stntr.Controls.Add(stnDist);
        gvStations.Controls.Add(stntr);

        pStations.Controls.Add(lblNoStat);
        pStations.Controls.Add(gvStations);
        #endregion

        #region Modules
        Panel pModMaster = new Panel();
        pModMaster.ID = CONTROLS_ID_PREFIX + "divModMaster";
        //pModMaster.CssClass = CSS_CLASS_CONTAINER;
        #endregion

        #endregion

        StringBuilder sbPage = new StringBuilder();
        //Change Location div
        StringBuilder sb = new StringBuilder();
        StringWriter tw = new StringWriter(sb);
        HtmlTextWriter hw = new HtmlTextWriter(tw);
        chgLoc.RenderControl(hw);
        sbPage.Append(sb.ToString());

        //Location Search div
          sb = new StringBuilder();
          tw = new StringWriter(sb);
          hw = new HtmlTextWriter(tw);
        p.RenderControl(hw);
        sbPage.Append(sb.ToString());

        //Search Results div
        sb = new StringBuilder();
        tw = new StringWriter(sb);
        hw = new HtmlTextWriter(tw);
        pnlSearchRes.RenderControl(hw);
        sbPage.Append(sb.ToString());

        //Stations div
        sb = new StringBuilder();
        tw = new StringWriter(sb);
        hw = new HtmlTextWriter(tw);
        pStations.RenderControl(hw);
        sbPage.Append(sb.ToString());

        //Modules div
        sb = new StringBuilder();
        tw = new StringWriter(sb);
        hw = new HtmlTextWriter(tw);
        pModMaster.RenderControl(hw);
        sbPage.Append(sb.ToString());

        StringBuilder sbJson = new StringBuilder();
        sbJson.Append(@"{""PgHTML"":""");
        sbJson.Append(GetCleanHtmlforJson(sbPage.ToString()));
        sbJson.Append(@""" }");

        return (sbJson.ToString());
    }
    #endregion

    #region GetCountries
    /// <summary>
    /// Method for populating country dropdown
    /// </summary>
    /// <param name="PubName">Publication name</param>
    /// <returns>Country Datatable</returns>
    public DataTable GetCountries(string PubName)
    {
        return sh.loadCountries();
    }
    #endregion

    #region GetLocDetailsAuto
    /// <summary> 
    /// Used for getting autocomplete results when searched in place textbox.
    /// </summary>
    /// <param name="strPlaceName">Place to search</param>
    /// <param name="strCountry">Country to search in</param>
    /// <param name="strCultureCode">Current culture</param>
    /// <returns>Json string</returns>
    public string GetLocDetailsAuto(string strPlaceName, string strCountry, string strCultureCode)
    {
        //DataSet ds = objLocWebSer.getLocationDetails(strPlaceName, strCountry, strCultureCode, svc.locationSearch.provider);
        List<Location> lLoc = locSvc.getLocationDetails(HttpUtility.HtmlDecode(strPlaceName), HttpUtility.HtmlDecode(strCountry), svc.locationSearch.provider, 0.0d, 0.0d, HttpUtility.HtmlDecode(strCultureCode));
        return ConvertLocationNearbyToJson(lLoc, null);
    }
    #endregion

    #region GetLocDetStations
    /// <summary>
    /// Used for getting location and station information when clicked on search button.
    /// </summary>
    /// <param name="strPlaceName">Place to search</param>
    /// <param name="strCountry">Country to search in</param>
    /// <param name="strCultureCode">Current culture</param>
    /// <returns>Json string</returns>
    public string GetLocDetStations(string strPlaceName, string strCountry, string strCultureCode)
    {
       
        double plat = 0.00;
        double plng = 0.00;
        objLocInfo.latitude = plat;
        objLocInfo.longitude = plng;
        List<Location> locationList = new List<Location>();
        //If the user enter the coordinates
        if (strPlaceName.IndexOf(',') > 0 && strPlaceName.Split(',')[0] != null)
        {
            double.TryParse(strPlaceName.Split(',')[0], out plat);
            double.TryParse(strPlaceName.Split(',')[1], out plng);

            string MatchLatPattern = @"^-?([1-8]?[1-9]|[1-9]0)?\.?[0-9]{0,4}";
            string MatchLongPattern = @"^-?([1]?[1-7][1-9]|[1]?[1-8][0]|[1-9]?[0-9])?\.?[0-9]{0,4}";
            string sNewPlaceName = string.Empty;
            if (Regex.IsMatch(plat.ToString(), MatchLatPattern) && Regex.IsMatch(plng.ToString(), MatchLongPattern))
            {
                objLocInfo.latitude = plat;
                objLocInfo.longitude = plng;
                if (objLocInfo.longitude != 0.00 && objLocInfo.latitude != 0.00)
                {
                    checkCountryValidity(plat, plng, strCountry);
                }

                sNewPlaceName = "Lat: " + plat + " Long: " + plng;
                objLocInfo.placeName = sNewPlaceName;
                
                Location l = new Location();
                l.Name = sNewPlaceName;
                l.Latitude = (decimal)plat;
                l.Longitude = (decimal)plng;
                locationList.Add(l);
            }
        }
        else
        {
            //The user has passed a location name
            string place = "";
            if (strPlaceName.IndexOf('(') > 0)
                place = strPlaceName.Split('(')[0];
            else
                place = strPlaceName;
  
                locationList = locSvc.getLocationDetails(HttpUtility.HtmlDecode(strPlaceName), HttpUtility.HtmlDecode(strCountry), svc.locationSearch.provider, 0.0d, 0.0d, HttpUtility.HtmlDecode(strCultureCode));
                

            if ((!String.IsNullOrEmpty(lat) || lat != "0.0") && (!string.IsNullOrEmpty(lng) || lng != "0.0") && !string.IsNullOrEmpty(strPlaceName))
            {
                double.TryParse(lat, out plat);
                double.TryParse(lng, out plng);
                objLocInfo.latitude = plat;
                objLocInfo.longitude = plng;
                objLocInfo.placeName = strPlaceName;
                objLocInfo.searchLocation = strPlaceName;
            }
            else
            {
                //if only one location, we use it
                if (locationList.Count == 1 || string.IsNullOrWhiteSpace(stnname))
                {
                    objLocInfo.placeName = locationList[0].Name;
                    objLocInfo.searchLocation = locationList[0].Name;
                    objLocInfo.latitude = (double)locationList[0].Latitude;
                    objLocInfo.longitude = (double)locationList[0].Longitude;
                    objLocInfo.AdminName = locationList[0].AdminName1;
                }
            }
        }

        DataSet ds = new DataSet();
        //load the stations only if lat long is valid or the place is used for searching
        if (isLatLongValid)
        {
            DataTable dtNearByStations = LoadStations();

            DataTable dtTotalStations = new DataTable("totalStations");
            dtTotalStations.Columns.Add("Total", typeof(int));
            dtTotalStations.Rows.Add(dtNearByStations.Rows.Count);

            ds.Tables.Add(dtTotalStations);
            ds.Tables.Add(dtNearByStations);
        }

        return ConvertLocationNearbyToJson(locationList, ds);
    }
    #endregion

    #region GetModules
    /// <summary>
    /// Load all modules from pub. Chart,Tables and Icons
    /// </summary>
    /// <param name="PubName">Publication</param>
    /// <param name="module">Module</param>
    /// <returns>Json String</returns>
    public string GetModules(string PubName, string module)
    {
        DataTable dt = LoadStations();
        Panel p = new Panel();
        p.ID = CONTROLS_ID_PREFIX + "divModules";
        //p.CssClass = CSS_CLASS_CONTAINER;

        foreach (string[] node in sh.getNodeList("servicePage"))
        {
            Page pgMod = new Page();
            pgMod.EnableViewState = false;

            switch (node[0].ToString().ToLower())
            {
                case "chart":
                    LoadUserControl<WebChart>(pgMod, p, @"UserControls\WebChart.ascx", node[1].ToString(), "");
                    break;
                case "icon":
                    LoadUserControl<Icon>(pgMod, p, @"UserControls\Icon.ascx", node[1].ToString(), "");
                    break;
                case "tbldaysrows":
                    LoadUserControl<Tables>(pgMod, p, @"UserControls\Tables.ascx", node[1].ToString(), node[0].ToString());
                    break;
                case "tblseriesrows":
                    LoadUserControl<Tables>(pgMod, p, @"UserControls\Tables.ascx", node[1].ToString(), node[0].ToString());
                    break;
                case "AgriInfo":
                    LoadUserControl<AgriInfo>(pgMod, p, @"UserControls\AgriInfo.ascx", node[1].ToString(), "");
                    break;
                case "legend":
                     HtmlGenericControl div1 =  DisplayLegend(node[0].ToString(), node[1].ToString());
                     p.Controls.Add(div1);
                    break;
                default:
                    break;
            }
        }

        StringBuilder sbModule = new StringBuilder();
        StringBuilder sb = new StringBuilder();
        StringWriter tw = new StringWriter(sb);
        HtmlTextWriter hw = new HtmlTextWriter(tw);
        p.RenderControl(hw);
        sbModule.Append(sb.ToString());

        StringBuilder sbJson = new StringBuilder();
        sbJson.Append(@"{""ModHTML"":""");
        sbJson.Append(GetCleanHtmlforJson(sbModule.ToString()));
        sbJson.Append(@""" }");

        return (sbJson.ToString());
    }
    #endregion

    #endregion

    #region Helper Methods

    #region GetCleanHtmlforJson
    /// <summary>
    /// Replaces double quotes with \" and new line \r\n with html break br
    /// </summary>
    /// <param name="htmltext">HTML markup to format</param>
    /// <returns>string</returns>
    public string GetCleanHtmlforJson(string htmltext)
    {
        //replace " with \",  \r\n with "" and \n \r \t individually with "" 
        return htmltext.Replace("\"", "\\\"").Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
    }

    void checkCountryValidity(double lat,double lng,string strCountry)
    {
        //check if the Lat long specified belongs to the selected country code
        //if found, method returns country code , if not, some junk html
        string strCountryCode = objComUtil.CheckLatLongForCountry(double.Parse(lat.ToString()), double.Parse(lng.ToString()));

        //takte the first two characters
        if (!string.IsNullOrEmpty(strCountryCode))
            strCountryCode = strCountryCode.Substring(0, 2);

        if (strCountryCode.Equals(strCountry))
        {
            isLatLongValid = true;
        }
        else
        {
            isLatLongValid = false;
            // HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.GEN_LATLONGINVALID);
        }
        
    }
    #endregion

    #region ConvertDataTableToJson
    /// <summary>
    /// Converts a Datatable to Json string.
    /// </summary>
    /// <param name="dt"></param>
    /// <returns>Json string</returns>
    public string ConvertDataTableToJson(DataTable dt)
    {
        Dictionary<string, object> resultMain = new Dictionary<string, object>();
        int index = 0;
        foreach (DataRow dr in dt.Rows)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (DataColumn dc in dt.Columns)
            {
                result.Add(dc.ColumnName, dr[dc].ToString());
            }
            resultMain.Add(index.ToString(), result);
            index++;
        }

        JavaScriptSerializer objSer = new JavaScriptSerializer();

        return objSer.Serialize(resultMain);
    }
    #endregion

    #region ConvertDataSetToJson
    /// <summary>
    /// Converts Dataset to Json string with datatable names as 1st level of hierarchy
    /// </summary>
    /// <param name="ds">Dataset to convert</param>
    /// <returns>Json string</returns>
    public string ConvertDataSetToJson(DataSet ds)
    {
        Dictionary<string, object> resultMain = new Dictionary<string, object>();
         
        foreach (DataTable dt in ds.Tables)
        {
            int index = 0;
            Dictionary<string, object> resultdt = new Dictionary<string, object>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                resultdt.Add(index.ToString(), result);
                index++;
            }
            resultMain.Add(dt.TableName, resultdt);
        }

        JavaScriptSerializer objSer = new JavaScriptSerializer();

        return objSer.Serialize(resultMain);
    }
    #endregion

    #region ConvertLocationNearbyToJson
    /// <summary>
    /// Converts Dataset to Json string with datatable names as 1st level of hierarchy
    /// </summary>
    /// <param name="locationList">List of locations to convert</param>
    /// <param name="ds">List of NearbyStations to convert</param>

    /// <returns>Json string</returns>
    public string ConvertLocationNearbyToJson(List<Location> locationList, DataSet ds)
    {
        Dictionary<string, object> resultMain = new Dictionary<string, object>();
        //locations
        Dictionary<string, object> resultdtLocations = new Dictionary<string, object>();
        int index = 0;
        foreach (Location l in locationList)
        {
            Dictionary<string, object> resultdtLocationsRow = new Dictionary<string, object>();
            resultdtLocationsRow.Add("name", l.Name);
            resultdtLocationsRow.Add("lat", l.Latitude);
            resultdtLocationsRow.Add("lng", l.Longitude);
            resultdtLocationsRow.Add("AreaId", l.AreaId);
            resultdtLocationsRow.Add("PlaceId", l.PlaceId);
            resultdtLocationsRow.Add("adminName1", l.AdminName1);
            resultdtLocationsRow.Add("adminName2", l.AdminName2);
            resultdtLocations.Add(index.ToString(), resultdtLocationsRow);
            index++;
        }
        resultMain.Add("geoname", resultdtLocations);

        //number of locations
        Dictionary<string, object> resultdtNumLocations = new Dictionary<string, object>();
            Dictionary<string, object> resultdtNumLocationsRow = new Dictionary<string, object>();
                resultdtNumLocationsRow.Add("totalResultsCount", locationList.Count);
            resultdtNumLocations.Add("0", resultdtNumLocationsRow);
        resultMain.Add("geonames", resultdtNumLocations);

        //nearbystations
        if (ds != null)
        foreach (DataTable dt in ds.Tables)
        {
                index = 0;
            Dictionary<string, object> resultdt = new Dictionary<string, object>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                resultdt.Add(index.ToString(), result);
                index++;
            }
            resultMain.Add(dt.TableName, resultdt);
        }

        JavaScriptSerializer objSer = new JavaScriptSerializer();

        return objSer.Serialize(resultMain);
    }
    #endregion

    #region LoadUserControl
    /// <summary>
    /// Loads user controls and adds the html output to the container object.
    /// </summary>
    /// <typeparam name="T">Type of UserControl</typeparam>
    /// <param name="pg">Page Object</param>
    /// <param name="Container">Page container</param>
    /// <param name="Filename">UserControl File Location</param>
    /// <param name="Name">UserControl Parameter Name</param>
    /// <param name="Node">UserControl Parameter Node</param>
    public void LoadUserControl<T>(Page pg, Panel Container, string Filename, string Name, string Node) where T : UserControl
    {
        const string STR_BeginRenderControlBlock = "<!-- BEGIN RENDERCONTROL BLOCK -->";
        const string STR_EndRenderControlBlock = "<!-- End RENDERCONTROL BLOCK -->";

        T ctr = (T)pg.LoadControl(Filename);
        ctr.ClientIDMode = ClientIDMode.Static;
        PropertyInfo propertyInfo = ctr.GetType().GetProperty("Name");
        propertyInfo.SetValue(ctr, Convert.ChangeType(Name, propertyInfo.PropertyType), null);
        PropertyInfo propertyInfoNode = ctr.GetType().GetProperty("Node");
        if (propertyInfoNode != null)
        {
            propertyInfoNode.SetValue(ctr, Convert.ChangeType(Node, propertyInfoNode.PropertyType), null);
        }
        StringWriter tw = new StringWriter();
        HtmlForm form = new HtmlForm(); form.ID = "_frmtemp";
        pg.Controls.Add(form);
        form.Controls.Add(new LiteralControl(STR_BeginRenderControlBlock));
        form.Controls.Add(ctr);
        form.Controls.Add(new LiteralControl(STR_EndRenderControlBlock));
        HttpContext.Current.Server.Execute(pg, tw, false);
        string Html = tw.ToString();
        // Strip out form and ViewState, Event Validation etc.  
        int startIdx = Html.IndexOf(STR_BeginRenderControlBlock);
        int endIdx = Html.IndexOf(STR_EndRenderControlBlock);
        if (startIdx > -1 && endIdx > startIdx)
        {
            Html = Html.Substring(startIdx + STR_BeginRenderControlBlock.Length);
            Html = Html.Substring(0, endIdx - startIdx - STR_BeginRenderControlBlock.Length);
        }

        LiteralControl litContent = new LiteralControl(Html);
        Panel pnlSeriesRows = new Panel();
        pnlSeriesRows.Controls.Add(litContent);

        Container.Controls.Add(pnlSeriesRows);
    }
    #endregion

    #region LoadStations
    /// <summary>
    /// Loads near by stations based on the location passed.
    /// </summary>
    /// <returns>Station DataTable</returns>
    public DataTable LoadStations()
    {
        //Load Stations
        objDataPointInfo = DataPointInfo.getDataPointObject;
        if (!string.IsNullOrWhiteSpace(stnname))
        {
            objDataPointInfo.stationName = stnname;
        }
        NearByPointSettings arNearBydata = objNearByPre.setNearbyStationAttributes();
        objDataPointInfo.NearbyPointSettings = arNearBydata;

        objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
        objDataPointInfo = DataPointInfo.getDataPointObject;
        string strStation = objDataPointInfo.stationName;
        if (string.IsNullOrEmpty(objLocInfo.latitude == 0.0 ? "" : objLocInfo.latitude.ToString()) || string.IsNullOrEmpty(objLocInfo.longitude==0.0?"":objLocInfo.longitude.ToString()))
        {
            objLocInfo.latitude = double.Parse(lat);
            objLocInfo.longitude = double.Parse(lng);
        }
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
                objDataPointInfo.DayOffset = Convert.ToInt32(dr["TimezoneOffset"].ToString() == "" ? "0" : dr["TimezoneOffset"].ToString());
                objDataPointInfo.iDstOn = Convert.ToInt32(dr["DstOn"].ToString() == "" ? "0" : dr["DstOn"].ToString());

                objDataPointInfo.SunRise = objComUtil.getSunrise(DateTime.Today.Date, Convert.ToDouble(dr["Latitude"].ToString()), Convert.ToDouble(dr["Longitude"].ToString()));
                objDataPointInfo.SunSet = objComUtil.getSunset(DateTime.Today.Date, Convert.ToDouble(dr["Latitude"].ToString()), Convert.ToDouble(dr["Longitude"].ToString()));
                objDataPointInfo.directionLetter = objComUtil.getTextDirection(Convert.ToInt32(dr["BearingDegrees"].ToString()));
                objDataPointInfo.altitude = Convert.ToInt32(dr["Altitude"].ToString() != "" ? dr["Altitude"].ToString() : "0");
                //objDataPointInfo.DayOffset = locSvc.getTimeZoneOffset(Convert.ToInt32(dr[0].ToString()));

                //add the offset according to the location
                objDataPointInfo.SunRise = objDataPointInfo.SunRise.AddHours(Convert.ToDouble(objDataPointInfo.DayOffset + objDataPointInfo.iDstOn));
                objDataPointInfo.SunSet = objDataPointInfo.SunSet.AddHours(Convert.ToDouble(objDataPointInfo.DayOffset + objDataPointInfo.iDstOn));

            }
            else
            {

                objDataPointInfo.stationName = dtNearByStations.Rows[0]["Name"].ToString();
                objDataPointInfo.stationLatitude = Convert.ToDouble(dtNearByStations.Rows[0]["Latitude"].ToString());
                objDataPointInfo.stationLongitude = Convert.ToDouble(dtNearByStations.Rows[0]["Longitude"].ToString());
                objDataPointInfo.SunRise = objComUtil.getSunrise(DateTime.Today.Date, Convert.ToDouble(dtNearByStations.Rows[0]["Latitude"].ToString()), Convert.ToDouble(dtNearByStations.Rows[0]["Longitude"].ToString()));
                objDataPointInfo.SunSet = objComUtil.getSunset(DateTime.Today.Date, Convert.ToDouble(dtNearByStations.Rows[0]["Latitude"].ToString()), Convert.ToDouble(dtNearByStations.Rows[0]["Longitude"].ToString()));
                objDataPointInfo.directionLetter = objComUtil.getTextDirection(Convert.ToInt32(dtNearByStations.Rows[0]["BearingDegrees"].ToString()));
                objDataPointInfo.altitude = Convert.ToInt32(dtNearByStations.Rows[0]["Altitude"].ToString() != "" ? dtNearByStations.Rows[0]["Altitude"].ToString() : "0");
                //objDataPointInfo.DayOffset = locSvc.getTimeZoneOffset(Convert.ToInt32(dtNearByStations.Rows[0][0].ToString()));
                objDataPointInfo.DayOffset = Convert.ToInt32(dtNearByStations.Rows[0]["TimezoneOffset"].ToString() == "" ? "0" : dtNearByStations.Rows[0]["TimezoneOffset"].ToString());
                objDataPointInfo.iDstOn = Convert.ToInt32(dtNearByStations.Rows[0]["DstOn"].ToString() == "" ? "0" : dtNearByStations.Rows[0]["DstOn"].ToString());

                //add the offset according to the location
                objDataPointInfo.SunRise = objDataPointInfo.SunRise.AddHours(Convert.ToDouble(objDataPointInfo.DayOffset + objDataPointInfo.iDstOn));
                objDataPointInfo.SunSet = objDataPointInfo.SunSet.AddHours(Convert.ToDouble(objDataPointInfo.DayOffset + objDataPointInfo.iDstOn));
            }
        }
        else
        {
            objDataPointInfo = new DataPointInfo();
            HttpContext.Current.Session["objDataPointInfo"] = objDataPointInfo;
        }

        return dtNearByStations;
    }
    #endregion

    public HtmlGenericControl DisplayLegend(string node, string name)
    {
      //  try
        {
            objSvcPre = new ServicePresenter();
           DataTable dtLegenddetails = objSvcPre.DisplayLegends(node, name);
            //load only if station is selected
            if (dtLegenddetails != null && objDataPointInfo != null && (objDataPointInfo.stationLatitude != 0.0 || objDataPointInfo.stationLongitude != 0.0))
            {
                Label lblLegend = new Label();
                if (dtLegenddetails.Rows[0][1].ToString() != "")
                    //objSvcPre.getTransText(dtLegenddetails.Rows[0][1].ToString());
                    lblLegend.Text = objSvcPre.getTranslatedText(dtLegenddetails.Rows[0][1].ToString(), objSvcInfo.Culture) ?? "";
               // lblLegend.Text = TransString ?? "";
                lblLegend.CssClass = LABEL250;
                lblLegend.ID = "lbl_" + name;
                StringBuilder sbBody = new StringBuilder();
                StringBuilder sb1;
                StringBuilder sb;
                string legendPath = dtLegenddetails.Rows[0][2].ToString().ToString();

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
                           string TransString = objSvcPre.getTranslatedText(text.Substring(startDelimiter.Count(), text.Length - (endDelimiter.Count() + startDelimiter.Count())),objSvcInfo.Culture);
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
              //  pgMod.Controls.Add(divAdditional);
                return divAdditional;
            }
            HtmlGenericControl divEmpty = new HtmlGenericControl("div");
            return divEmpty;
        }
      
    }
    #endregion

    #region Interface Members

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    #endregion

    public object Location { get; set; }
}
#endregion