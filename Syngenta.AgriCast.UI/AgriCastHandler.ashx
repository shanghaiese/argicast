<%@ WebHandler Language="C#" Class="AgriCastHandler" %>

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

    //New Widget Request
    bool isWidget = false;

    //Multi Cookie Implementation -Begin
    //ClientID refers to cookie name against which the details are stored
    string clientID;
    //Multi Cookie Implementation -End

    //Units Implementation for Embed Js - Begin
    string strUnit = string.Empty;
    //Units Implementation for Embed Js - End

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
    //Multi Cookie Implementation -Begin
    private const string DEFAULT_COOKIE = "EmbedJsDefCookie";
    //Multi Cookie Implementation -End

    //Units Implementation for Embed Js - Begin
    private const string DEFAULT_UNIT = "metric";
    //Units Implementation for Embed Js - End


    //translate Weather forecastText
    private const string strWeatherForecastKey = "Weather Forecast";
    //Show invalid lat long search error message - begin
    DataTable dtErrorMessages = null;
    //Show invalid lat long search error message - end

    /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  Begin*/
    /*IM01176629- New_Agricast_Embedded_JS_failed_to_get_the_country_from_cookie - BEGIN*/
    //change overide cookie to false by default
    private const bool DEFAULT_OVERRIDECOOKIE = false;
    //bool blOverrideCookie = false;
    /*IM01176629- New_Agricast_Embedded_JS_failed_to_get_the_country_from_cookie - END*/
    /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  End*/

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
        try
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
            isWidget = bool.Parse(context.Request.QueryString["iswidget"] ?? "false");
            //Multi Cookie Implementation -Begin
            clientID = (context.Request.QueryString["clientID"] ?? DEFAULT_COOKIE).ToString();
            //Multi Cookie Implementation -End

            //Units Implementation for Embed Js - Begin
            strUnit = (context.Request.QueryString["unit"] ?? DEFAULT_UNIT).ToString();
            //Units Implementation for Embed Js - End

            /*IM01173263 - New Agricast - EmbeddedJS- location search querystring - BEGIN */
            //blOverrideCookie = bool.Parse(context.Request.QueryString["overrideCookie"] ?? DEFAULT_OVERRIDECOOKIE.ToString());
            /*IM01173263 - New Agricast - EmbeddedJS- location search querystring - END */
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

            //Multi Cookie Implementation -Begin
            // HttpCookie locCookie = HttpContext.Current.Request.Cookies["LocationInfo"];
            //HttpCookie locCookie = HttpContext.Current.Request.Cookies[clientID];
            //Multi Cookie Implementation -End

            //if (locCookie != null)
            //{
            //    LocationInfo newInfo = objLocInfo.DeserializeCookieString(locCookie.Value);
            //    if (newInfo != null)
            //    {
            //Multi Cookie Implementation -Begin
            // HttpContext.Current.Session["objLocationInfo"] = newInfo;
            //        HttpContext.Current.Session[clientID] = newInfo;
            //Multi Cookie Implementation -End
            //        objLocInfo = newInfo;
            //    }
            // }

            /*BEGIN - Jan 14th 2013 - AIS Web Part Issue - Special charaters issue - BEGIN*/
            //place = HttpUtility.UrlDecode(place);
            /*END - Jan 14th 2013 - AIS Web Part Issue - - Special charaters issue - END*/


            /*IM01173263 - New Agricast - EmbeddedJS- location search querystring - BEGIN */
            //On LoadSearch , if override cookie is true, replace all the values in cookie

            ////if (blOverrideCookie)
            ////{
            ////    double latlng = 0.0;
            ////    objLocInfo.CountryCode = country.ToUpper() == "UNDEFINED" ? "" : country;
            ////    objLocInfo.placeName = place;

            ////    objLocInfo.latitude = (Double.TryParse(lat, out latlng) ? latlng : 0.0);
            ////    objLocInfo.longitude = (Double.TryParse(lng, out latlng) ? latlng : 0.0);
            ////    //if (!HttpUtility.UrlDecode(objLocInfo.placeName).Equals(place))//  /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  /
            ////    //{
            ////    //    objLocInfo.placeName = place;//set the value passed from querystring to location info object
            ////    //    //clear the lat and long
            ////    //    lat = ""; lng = "";
            ////    //}
            ////    //else
            ////    //{
            ////    //    objLocInfo.placeName = place;

            ////    //    objLocInfo.latitude = (Double.TryParse(lat, out latlng) ? latlng : 0.0);
            ////    //    objLocInfo.longitude = (Double.TryParse(lng, out latlng) ? latlng : 0.0);
            ////    //}

            ////}
            /*IM01173263 - New Agricast - EmbeddedJS- location search querystring - END */

            //New Widget Request
            //cookie Issue
            //if (!string.IsNullOrEmpty(place))
            //{
            //    if (!objLocInfo.placeName.Equals(place))//  /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  /
            //    {
            //        objLocInfo.placeName = place;//set the value passed from querystring to location info object
            //        //clear the lat and long
            //        lat = ""; lng = "";
            //    }

            //}
            //else
            //{
            //    place = objLocInfo.placeName;//Retrive from location info object
            //}




            //Page Refresh cookie issue
            /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  Begin*/
            //if (!string.IsNullOrEmpty(country) && country.ToUpper() != "UNDEFINED")

            //    objLocInfo.CountryCode = country;
            //else
            //    country = objLocInfo.CountryCode;
            /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  End*/

            //Place name search wihtout lat long - avoid parisng of empty string
            //NewWidget Request
            ////cookie issue
            //if (!string.IsNullOrEmpty(lat))
            //    objLocInfo.latitude = double.Parse(lat);


            //if (!string.IsNullOrEmpty(lng))
            //    objLocInfo.longitude = double.Parse(lng);

            //End of Change
            //commented for Page Refresh cookie issue
            //if ((objLocInfo.placeName == "" || (objLocInfo.latitude == 0d && objLocInfo.longitude == 0d)) && objLocInfo.CountryCode == "")
            //{
            //    objSvcPre.setDefaultLoc();
            //}
            sh = new ServiceHandler();
            objSvcInfo = new ServiceInfo();

            ServiceInfo.ServiceConfig = objSvcInfo;
            objSvcInfo.ServiceName = pub;

            svc = objSvcPre.readConfig();
            objSvcPre.createServiceSession();
            //new implementation for multicookie//
            double coord = 0d;
            objLocInfo.searchLocation = place.ToString();
            objLocInfo.placeID = 0;
            objLocInfo.placeName = place.ToString();
            objLocInfo.longitude = double.TryParse(lng, out coord) ? coord : 0d;
            objLocInfo.latitude = double.TryParse(lat, out coord) ? coord : 0d;
            objLocInfo.CountryCode = country;
            objLocInfo.Provider = LocationSearchSource.defaultService;
            objLocInfo.AdminName = "";
            objSvcInfo.Country = country;
            objSvcInfo.Culture = culture;
            objSvcInfo.Module = module;
            objSvcInfo.ServiceName = pub;
            objSvcInfo.Unit = strUnit;
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


            //Units Implementation for Embed Js - Begin
            objSvcInfo.Unit = strUnit;
            //Units Implementation for Embed Js - End


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
            /*BEGIN - Jan 14th 2013 - AIS Web Part Issue - Special charaters issue - BEGIN*/
            place = HttpUtility.UrlEncode(place);

            /*END - Jan 14th 2013 - AIS Web Part Issue - - Special charaters issue - END*/

            objLocInfo.placeName = place;
            objLocInfo.searchLocation = place;
            /*IM01173263 - New Agricast - EmbeddedJS- location search querystring - BEGIN */
            //double latVal;
            //double.TryParse(objLocInfo.latitude.ToString() == "0.0" || objLocInfo.latitude.ToString() == "0" ? lat.ToString() : objLocInfo.latitude.ToString(), out latVal);
            //objLocInfo.latitude = latVal;
            //double lngVal;
            //double.TryParse(objLocInfo.longitude.ToString() == "0.0" || objLocInfo.longitude.ToString() == "0" ? lng.ToString() : objLocInfo.longitude.ToString(), out lngVal);
            //objLocInfo.longitude = lngVal;

            double latVal;
            double.TryParse(lat == "" ? "0.0" : lat.ToString(), out latVal);
            objLocInfo.latitude = latVal;
            double lngVal;
            double.TryParse(lng == "" ? "0.0" : lng.ToString(), out lngVal);
            objLocInfo.longitude = lngVal;

            /*IM01173263 - New Agricast - EmbeddedJS- location search querystring - END */
            objLocInfo.AdminName = "";

            if (!string.IsNullOrWhiteSpace(country))
            {
                objLocInfo.CountryCode = country;
            }

            //Multi Cookie Implementation -Begin
            // HttpCookie LocationCookie = new HttpCookie("LocationInfo", objLocInfo.SerializeCookieString());
            HttpCookie LocationCookie = new HttpCookie("ais_"+ clientID, objLocInfo.SerializeCookieString());
            //Multi Cookie Implementation -End

            LocationCookie.Expires = DateTime.Now.AddDays(365);
            //Dont remove this line as it is required for the embedjs to work properly in IE8 & IE7
            HttpContext.Current.Response.AddHeader("p3p", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");
            HttpContext.Current.Response.AddHeader("Cache-Control","private");
            HttpContext.Current.Response.AddHeader("max-age", "7200");

            //if (locCookie != null)
            //{
            //    LocationCookie.Path = locCookie.Path;
            //}
            HttpContext.Current.Response.Cookies.Add(LocationCookie);

            #endregion

        }

        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.Write(string.Format("Process Request Failed= ({0});", ex.Message.ToString()));
        }
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
        lblCountry.Text = locSvc.getTranslatedText("SearchIn", objSvcInfo.Culture) + ":";

        DropDownList ddlCountry = new DropDownList();
        ddlCountry.ID = CONTROLS_ID_PREFIX + "ddlCountry";
        //ddlCountry.Style.Add("width", Unit.Percentage(100).ToString());

        /*IM01277740 - New Agricast - add "Select" to country dropdown in both application and embed js - begin */
        //ddlCountry.DataSource = GetCountries(pub);
        DataTable dtCon = GetCountries(pub);

        // add the "select" value only if more than 1 country is specifed
        if (dtCon.Rows.Count != 1)
        {
            DataRow drCon = dtCon.NewRow();
            drCon["Value"] = locSvc.getTranslatedText("Select country", objSvcInfo.Culture);
            drCon["code"] = "XX";
            dtCon.Rows.InsertAt(drCon, 0);

        }
        ddlCountry.DataSource = dtCon;
        /*IM01277740 - New Agricast - add "Select" to country dropdown in both application and embed js - end */
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
        //new widget Request
        //show country dropdown - nov 8 th 2012
        if (isWidget)
        {
            lblFor.Text = locSvc.getTranslatedText("Search for", objSvcInfo.Culture);
        }
        else
        {
            lblFor.Text = locSvc.getTranslatedText("for", objSvcInfo.Culture);
        }

        TextBox txtPlace = new TextBox();
        //txtPlace.CssClass = CSS_CLASS_SEARCHINPUT;
        txtPlace.ID = CONTROLS_ID_PREFIX + "txtPlace";
        //txtPlace.Style.Add("width", Unit.Percentage(100).ToString());
        txtPlace.Attributes.Add("placeholder", "Enter the search criteria");
        txtPlace.Attributes.Add("autofocus", "autofocus");
        txtPlace.AutoCompleteType = AutoCompleteType.Disabled;

        if (!string.IsNullOrWhiteSpace(place))
        {
            txtPlace.Text = HttpUtility.UrlDecode(place);
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
        gvLocation.Style.Add("border", "1px solid grey");
        gvLocation.Style.Add("width", "97%");
        gvLocation.CellPadding = 2;
        //gvLocation.Style.Add("border-bottom", "1px solid black");
        //gvLocation.Style.Add("border-top", "1px solid black");
        //gvLocation.Style.Add("border-left", "1px solid black");
        //gvLocation.Style.Add("border-right", "1px solid black");
        //gvLocation.Attributes.Add("border", "1");
        //gvLocation.BorderWidth = 1;
        //gvLocation.CellSpacing = 0;        

        TableRow tr = new TableRow();
        TableHeaderCell th = new TableHeaderCell();
        th.Style.Add("border", "1px solid grey");
        th.Text = locSvc.getTranslatedText(Constants.NAME, objSvcInfo.Culture);
        tr.Controls.Add(th);
        if (!isWidget)
        {
            TableHeaderCell th1 = new TableHeaderCell();
            th1.Style.Add("border", "1px solid grey");
            th1.Text = locSvc.getTranslatedText(Constants.LATITUDE, objSvcInfo.Culture);
            tr.Controls.Add(th1);
            TableHeaderCell th2 = new TableHeaderCell();
            th2.Style.Add("border", "1px solid grey");
            th2.Text = locSvc.getTranslatedText(Constants.LONGITUDE, objSvcInfo.Culture);
            tr.Controls.Add(th2);
            TableHeaderCell th3 = new TableHeaderCell();
            th3.Style.Add("border", "1px solid grey");
            th3.Text = locSvc.getTranslatedText(Constants.ADMIN1, objSvcInfo.Culture);
            tr.Controls.Add(th3);
            TableHeaderCell th4 = new TableHeaderCell();
            th4.Style.Add("border", "1px solid grey");
            th4.Text = locSvc.getTranslatedText(Constants.ADMIN2, objSvcInfo.Culture);
            tr.Controls.Add(th4);
        }
        
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
        gvStations.Style.Add("border", "1px solid grey");
        gvStations.Style.Add("width", "97%");

        gvLocation.CellPadding = 2;

        TableRow stntr = new TableRow();
        TableHeaderCell stnname = new TableHeaderCell();
        stnname.Style.Add("border", "1px solid grey");
        stnname.Text = locSvc.getTranslatedText("NearStationName", objSvcInfo.Culture);
        stntr.Controls.Add(stnname);
        TableHeaderCell stnDist = new TableHeaderCell();
        stnDist.Style.Add("border", "1px solid grey");
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

        // //Show invalid lat long search error message - begin
        Label lblGenError = new Label();
        lblGenError.ID = CONTROLS_ID_PREFIX + "lblGenericError";
        lblGenError.CssClass = "hide";

        /*BEGIN - Jan 14th 2013 - AIS Web Part Issue - show error message when nolocation is defined on first time load of widget - BEGIN*/
        if (string.IsNullOrEmpty(place))
            lblGenError.Text = objComUtil.getTransText("Please set your location");
        /*END - Jan 14th 2013 - AIS Web Part Issue - show error message when nolocation is defined on first time load of widget - END*/
        sb = new StringBuilder();
        tw = new StringWriter(sb);
        hw = new HtmlTextWriter(tw);
        lblGenError.RenderControl(hw);
        sbPage.Append(sb.ToString());

        // //Show invalid lat long search error message - end

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
        List<Location> locationList = new List<Location>();
        double plat = 0.00;
        double plng = 0.00;
        //Mulitple Locations issue
        //objLocInfo.latitude = plat;
        //objLocInfo.longitude = plng;
        //end of change
        /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  Begin*/
        string strCountryCode = string.Empty;
        /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  End*/
        DataSet ds = new DataSet();
        /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  Begin*/
        //if the lat and lng is passed in the querstring 
        if (!(string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lng)))
        {
            /*IM01176629- New_Agricast_Embedded_JS_failed_to_get_the_country_from_cookie - BEGIN*/
            //if (lat != "0.0" && lng != "0.0")
            if (double.Parse(lat) != 0.0 && double.Parse(lng) != 0.0)
            /*IM01176629- New_Agricast_Embedded_JS_failed_to_get_the_country_from_cookie - END*/
            {
                if (string.IsNullOrEmpty(strPlaceName))
                    strPlaceName = string.Format("{0},{1}", lat, lng);
                else
                    strPlaceName = string.Format("{0},{1},{2}", place, lat, lng);
            }
        }
        /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  End*/

        //If the user enter the coordinates
        if (strPlaceName.IndexOf(',') > 0 && strPlaceName.Split(',')[0] != null)
        {
            //Lat Long Format Change - implementation - begin
            string sNewPlaceName = string.Empty;
            string[] strArray = strPlaceName.Split(',');
            //double.TryParse(strPlaceName.Split(',')[0], out plat);
            //double.TryParse(strPlaceName.Split(',')[1], out plng);
            //basel , 47.5,7.4
            if (strArray.Length == 3 || strArray.Length == 5)
            {
                sNewPlaceName = strArray[0];
                double.TryParse(strArray[1], out plat);
                double.TryParse(strArray[2], out plng);
            }
            // 47.5,7.4
            else
            {

                double.TryParse(strArray[0], out plat);
                double.TryParse(strArray[1], out plng);
                sNewPlaceName = "Lat: " + plat + " Long: " + plng;
            }
            //Lat Long Format Change - implementation - end


            string MatchLatPattern = @"^-?([1-8]?[1-9]|[1-9]0)?\.?[0-9]{0,4}";
            string MatchLongPattern = @"^-?([1]?[1-7][1-9]|[1]?[1-8][0]|[1-9]?[0-9])?\.?[0-9]{0,4}";

            if (Regex.IsMatch(plat.ToString(), MatchLatPattern) && Regex.IsMatch(plng.ToString(), MatchLongPattern))
            {
                objLocInfo.latitude = plat;
                objLocInfo.longitude = plng;
                if (objLocInfo.longitude != 0.00 && objLocInfo.latitude != 0.00)
                {
                    /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  End*/
                    strCountryCode = checkCountryValidity(plat, plng, strCountry);
                    /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  End*/
                }

                //Lat Long Format Change - implementation - begin
                //sNewPlaceName = "Lat: " + plat + " Long: " + plng;

                //Lat Long Format Change - implementation - begin
                objLocInfo.placeName = sNewPlaceName;

                /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  Begin*/
                country = strCountryCode;
                /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  End*/
                //if (!ds.Tables.Contains("geoname"))
                //{
                //Create a new Datatable with name "Geoname" and strucutre same as the output of getLocationDetails()
                /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  BEGIN*/
                Location l = new Location();
                l.Name = sNewPlaceName;
                l.Latitude = (decimal)plat;
                l.Longitude = (decimal)plng;
                l.CountryCode = strCountryCode;
                locationList.Add(l);
            }
        }
        else
        {
            string place = "";
            if (strPlaceName.IndexOf('(') > 0)
                place = strPlaceName.Split('(')[0];
            else
                place = strPlaceName;
            //ds = objLocWebSer.getLocationDetails(place, strCountry, strCultureCode, svc.locationSearch.provider);
            locationList = locSvc.getLocationDetails(HttpUtility.HtmlDecode(strPlaceName), HttpUtility.HtmlDecode(strCountry), svc.locationSearch.provider, 0.0d, 0.0d, HttpUtility.HtmlDecode(strCultureCode));
            //Multiple Locations issue
            //filter the Dataset based onlocation info object 
            // loc info object will have lat long only when clicked on location grid
            if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
            {
                /*IM01176629- New_Agricast_Embedded_JS_failed_to_get_the_country_from_cookie - BEGIN*/
                if (double.Parse(lat) != 0.0 && double.Parse(lng) != 0.0)
                {
                    locationList = locationList.Where(y => y.Latitude == decimal.Parse(lat) && y.Longitude == decimal.Parse(lng)).Distinct().ToList();
                }
            }

            //end of changes
            //new widget request
            //cookie issue
            //location search fetches only one results
            if (locationList.Count == 1)
            {
                lat = locationList[0].Latitude.ToString();
                lng = locationList[0].Longitude.ToString();
            }
            //multiple location issue
            else // return to the calling method when the location is not found 
            {
                lat = "0.0";
                lng = "0.0";
                //Set the Text of No Loc
            }



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

        ds = new DataSet();
        //load the stations only if lat long is valid or the place is used for searching
        if (isLatLongValid)
        {
            DataTable dtNearByStations = LoadStations();

            /*May 13th - changes for No stations found issue - begin*/
            //if no stations are found, set no station found error messgae and use it in JS
            if (dtNearByStations != null && dtNearByStations.Rows.Count == 0)
            {
                SetErrorMessage(objComUtil.getTransText(Constants.LS_NOSTATIONSFOUND));
                ds.Tables.Add(dtErrorMessages);
            }
            /*May 13th - changes for No stations found issue - end*/
            
            DataTable dtTotalStations = new DataTable("totalStations");
            dtTotalStations.Columns.Add("Total", typeof(int));
            dtTotalStations.Rows.Add(dtNearByStations.Rows.Count);

            ds.Tables.Add(dtTotalStations);
            ds.Tables.Add(dtNearByStations);
        }
        //Show invalid lat long search error message - begin
        else
        {
            SetErrorMessage(objComUtil.getTransText(Constants.GEN_LATLONGINVALID));
            ds.Tables.Add(dtErrorMessages);
        }
        //Show invalid lat long search error message - end
        return ConvertLocationNearbyToJson(locationList, ds);
    }
    #endregion
    //Show invalid lat long search error message - begin
    /// <summary>
    /// 
    /// </summary>
    void SetErrorMessage(string strText)
    {
        if (dtErrorMessages == null)
        {
            dtErrorMessages = new DataTable("ErrorMessages");
            dtErrorMessages.Columns.Add("Message");
        }
        DataRow dr = dtErrorMessages.NewRow();
        dr["Message"] = strText;
        dtErrorMessages.Rows.Add(dr);
    }
    //Show invalid lat long search error message - end

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
                    //LoadUserControl<WebChartIcons>(pgMod, p, @"UserControls\WebChartIcons.ascx", "", "");
                    break;

                /*Wind Icons Implementation for EmbedJS - BEGIN */
                case "windicon":
                    LoadUserControl<WebChartIcons>(pgMod, p, @"UserControls\WebChartIcons.ascx", node[1].ToString(), node[0].ToString());
                    //LoadUserControl<WebChartIcons>(pgMod, p, @"UserControls\WebChartIcons.ascx", "", "");
                    break;
                /*Wind Icons Implementation for EmbedJS - END */
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
                    HtmlGenericControl div1 = DisplayLegend(node[0].ToString(), node[1].ToString());
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
        //New Widget Request
        if (isWidget)
        {

            //Get the clean html without special characters
            string strCleanHTML = GetCleanHtmlforJson(sbModule.ToString());

            //DataTable to convert to html
            DataTable dtWidgetData = new DataTable();

            DataTable dtImages = new DataTable();
            dtImages.Columns.Add("url");

            DataTable dtTemp = new DataTable();

            //string strIconsPattern = "<table id=\"weathericontable\".*</table>";
            string strIconsPattern = "<img.*?>";
            string strSRC = "src=\\\\\".*?\\\\\"";
            string strSrcMatch = string.Empty;
            DataRow drwidget = null;
            DataRow drImages = null;
            //Fetch all the <TD> from the tables
            string strOuterDiv = "<table class=.*?</table>";
            string strAllTDs = "<td.*?>.*?</td>";
            string strTDforSeriesCount = "<td>.*?</td>";
            int iSeriesCount = 0;
            int iStep = 0;
            MatchCollection matches = null, matchSRC = null, matchesTable = null, matchesTDs = null, matchesSeriesCountTDs = null;
            //Regex reg = new Regex(strIconsPattern, RegexOptions.IgnoreCase);
            //MatchCollection matches = reg.Matches(strCleanHTML);
            matches = GetRegexData(strIconsPattern, strCleanHTML);

            foreach (Match mch in matches)
            {
                //reg = new Regex(strSRC, RegexOptions.IgnoreCase);
                matchSRC = GetRegexData(strSRC, mch.Value);

                if (matchSRC != null && matchSRC[0].Success)
                {
                    //if (String.IsNullOrEmpty(strSrcMatch))
                    //    strSrcMatch = matchSRC[0].Value;
                    //else

                    //    strSrcMatch = strSrcMatch + '#' + matchSRC[0].Value;
                    drImages = dtImages.NewRow();
                    strSrcMatch = matchSRC[0].Value;
                    strSrcMatch = strSrcMatch.Replace("src=", "").Replace("\\", "").Replace("\"", "");

                    drImages["url"] = strSrcMatch;
                    dtImages.Rows.Add(drImages);
                }

            }


            // Regex regTable = new Regex(strOuterDiv, RegexOptions.IgnoreCase);
            //Filter the contents within <table></table>
            matchesTable = GetRegexData(strOuterDiv, strCleanHTML);

            //Get all the TDs from the table
            if (matchesTable.Count > 0)
            {

                //Regex regTDs = new Regex(strTDs, RegexOptions.IgnoreCase);
                //Get all the TDs from this table.
                //This will filter all the td from the source table
                matchesTDs = GetRegexData(strAllTDs, matchesTable[0].Value);

                //get the TDs containing the series names for table
                // these <td> will have no style associated with it.for eg <td>Temperature min C</td>
                //this is used to count the number of series configured for Tableseriesrows
                matchesSeriesCountTDs = GetRegexData(strTDforSeriesCount, matchesTable[0].Value);

                if (matchesSeriesCountTDs != null)
                {
                    iSeriesCount = matchesSeriesCountTDs.Count;

                }
                //check the count of TDs 
                //Replace "3" with series count + 1  for future request


                //Indicates the number of days
                iStep = dtImages.Rows.Count;

                //dynamically create the Widget Table Table
                dtWidgetData.Columns.Add("Series");
                for (int i = 0; i < iStep; i++)
                {
                    dtWidgetData.Columns.Add(string.Format("Day{0}", i + 1));

                }

                if (matchesTDs.Count > 0)
                {
                    //Indicates the number of rows 
                    int iCount = matchesTDs.Count / (iStep + 1);


                    for (int i = 0; i < iCount; i++)
                    {
                        drwidget = dtWidgetData.NewRow();
                        for (int j = 0; j < (iStep + 1); j++)
                        {

                            if (j == 0)
                                drwidget["Series"] = getSubstringForTD(matchesTDs[i * (iStep + 1) + j].Value);
                            else
                                drwidget[string.Format("Day{0}", j)] = getSubstringForTD(matchesTDs[i * (iStep + 1) + j].Value);

                            //drwidget["Metric"] = getSubstringForTD(matchesTDs[iMod * i].Value);
                            //drwidget["TempMin"] = getSubstringForTD(matchesTDs[(iMod * i) + 1].Value);
                            //drwidget["TempMax"] = getSubstringForTD(matchesTDs[(iMod * i) + 2].Value);


                        }
                        dtWidgetData.Rows.Add(drwidget);
                    }

                }
            }


            //New Widget Request
            //Cookie and invalid location issue
            //Transpose the Widget Data
            if (dtWidgetData != null && dtWidgetData.Rows.Count > 0)
            {
                DataTable dtOuputTable = GetTransposeTableForWidget(dtWidgetData);

                //Add Images Url to the output table
                //This is to ensure that the daily images and the temperature configuration is same.
                if (dtImages.Rows.Count == dtOuputTable.Rows.Count)
                {
                    dtOuputTable.Columns.Add("ImageURL");

                    for (int iRow = 0; iRow < dtImages.Rows.Count; iRow++)
                    {
                        dtOuputTable.Rows[iRow]["ImageURL"] = dtImages.Rows[iRow][0].ToString();
                    }
                }

                string strJason = GetHtmlForWidget(dtOuputTable);



                sbJson.Append(@"{""ModHTML"":""");
                sbJson.Append(GetCleanHtmlforJson(strJason));
                sbJson.Append(@""" }");
            }
        }
        else
        {
            // StringBuilder sbJson = new StringBuilder();
            sbJson.Append(@"{""ModHTML"":""");
            sbJson.Append(GetCleanHtmlforJson(sbModule.ToString()));
            sbJson.Append(@""" }");
        }
        return (sbJson.ToString());
    }
    #endregion

    #endregion

    #region Helper Methods

    #region Widget Methods
    /// <summary>
    /// Get the filtered data based the regex provided
    /// </summary>
    /// <param name="regexfilter"></param>
    /// <param name="strinput"></param>
    /// <returns></returns>
    public MatchCollection GetRegexData(string regexfilter, string strinput)
    {

        Regex reg = new Regex(regexfilter, RegexOptions.IgnoreCase);
        return reg.Matches(strinput);
    }

    /// <summary>
    /// this method rerurns the data between the <td></td>tags
    /// </summary>
    /// <param name="strInput"></param>
    public string getSubstringForTD(string strInput)
    {
        int strIndex = strInput.IndexOf(">") + 1;
        int endIndex = strInput.LastIndexOf("<");
        return strInput.Substring(strIndex, (endIndex - strIndex));

    }
    public DataTable GetTransposeTableForWidget(DataTable dtInputTable)
    {
        DataTable dtOutputTable = new DataTable();

        //The first row and first column is the same for transposed table
        dtOutputTable.Columns.Add(dtInputTable.Columns[0].ColumnName.ToString());

        string strNewColName = string.Empty;
        //Convert rows of input table as columns in output table
        foreach (DataRow dr in dtInputTable.Rows)
        {
            strNewColName = dr[0].ToString();
            if (string.IsNullOrEmpty(strNewColName))
                strNewColName = "DaysOfWeek";

            dtOutputTable.Columns.Add(strNewColName);
        }

        DataRow drOutput = null;
        for (int iCol = 1; iCol <= dtInputTable.Columns.Count - 1; iCol++)
        {
            drOutput = dtOutputTable.NewRow();
            drOutput[0] = dtInputTable.Columns[iCol].ColumnName.ToString();
            for (int iRow = 0; iRow <= dtInputTable.Rows.Count - 1; iRow++)
            {
                string colValue = dtInputTable.Rows[iRow][iCol].ToString();
                drOutput[iRow + 1] = colValue;
            }
            dtOutputTable.Rows.Add(drOutput);
        }
        return dtOutputTable;
    }


    #region Widget in Table Format
    //public string GetHtmlForWidget(DataTable dtInput)
    //{
    //    CommonUtil objCommonUtil = new CommonUtil();

    //    StringBuilder sbHTML = new StringBuilder();
    //    //Lat Long Format Change - implementation - begin
    //    string strPlace = place;
    //    //Lat Long Format Change - implementation - end

    //    int iRowcount = dtInput.Rows.Count;
    //    int width = iRowcount * 15;
    //    sbHTML.Append("<div id=\"ag_divModules\"><div>");
    //    sbHTML.Append("<table id=\"tblwidget\" class=\"widgetTblwidget\"");
    //    sbHTML.Append("<tr>");
    //    /*Split the location and Forecast Text to two spans - Begin*/
    //    // sbHTML.AppendFormat("<td colspan={0}  class=\"widgetForecastText\">", iRowcount);
    //    sbHTML.AppendFormat("<td colspan={0} id=\"tdLocText\">", iRowcount);
    //    //Lat Long Format Change - implementation - begin
    //    if (place.IndexOf(',') != -1)
    //    {
    //        string[] strArray = place.Split(',');
    //        if (strArray.Length == 3)
    //        {
    //            strPlace = strArray[0];
    //        }
    //        else
    //        {
    //            strPlace = string.Format("{0} : {1}", strArray[0], strArray[1]);
    //        }
    //    }

    //    //Translate this text
    //    //sbHTML.AppendFormat("{0} : Weather Forecast", place.ToUpper());

    //    sbHTML.Append("<span class=\"widgetForecastLocation\">");
    //    sbHTML.Append(strPlace.ToUpper());
    //    sbHTML.Append("</span>");
    //    sbHTML.Append("&nbsp;&nbsp;");
    //    sbHTML.Append(":&nbsp;&nbsp;");
    //    sbHTML.Append("<span class=\"widgetForecastText\">");
    //    sbHTML.Append(objCommonUtil.getTransText(strWeatherForecastKey));
    //    sbHTML.Append("</span>");
    //    // sbHTML.AppendFormat("{0} : {1}", strPlace, objCommonUtil.getTransText(strWeatherForecastKey));
    //    /*Split the location and Forecast Text to two spans - End*/

    //    //Lat Long Format Change - implementation - end
    //    sbHTML.Append("</td>");
    //    sbHTML.Append("</tr>");
    //    sbHTML.Append("<tr>");

    //    //Create a <TD> tag for each Day data :- iStep
    //    string strTempMax = string.Empty;
    //    string strTempMin = string.Empty;
    //    string strUnit = string.Empty;

    //    //Unit Implementation for Embed JS -BEGIN
    //    //if (dtInput.Columns[2].ColumnName.IndexOf("(") != -1 && dtInput.Columns[2].ColumnName.Contains("C"))
    //    //    strUnit = "°C";
    //    //For units, Take the substring with parathesis in tempaerature series
    //    string strTempColName = dtInput.Columns[2].ColumnName;
    //    int idxLeftParan = -1;//Left ( index
    //    int idxRightParan = -1; //Right ) index
    //    if (!string.IsNullOrEmpty(strTempColName))
    //    {
    //        //GEt the indices of left and right paranthensis
    //        idxLeftParan = strTempColName.IndexOf("(");
    //        idxRightParan = strTempColName.IndexOf(")");

    //        if (idxLeftParan != -1 && idxRightParan != -1)
    //        {

    //            strUnit = strTempColName.Substring(idxLeftParan + 1, (idxRightParan - idxLeftParan - 1));
    //        }
    //    }
    //    //Unit Implementation for Embed JS - END

    //    foreach (DataRow drInput in dtInput.Rows)
    //    {
    //        sbHTML.Append("<td>");
    //        sbHTML.Append("<table border=0>");
    //        sbHTML.Append("<tr>");
    //        sbHTML.Append("<td  class=\"wid_tdDaysofWeek\" align=\"center\">");
    //        /*DSTP CSS Changes - BEGIN*/
    //        //added clasess to all the TDs
    //        //sbHTML.AppendFormat("<b>{0}</b>", drInput["DaysOfWeek"].ToString());
    //        /*DSTP CSS Changes - END*/
    //        sbHTML.AppendFormat(drInput["DaysOfWeek"].ToString());
    //        sbHTML.Append("</td>");
    //        sbHTML.Append("</tr>");
    //        sbHTML.Append("<tr>");
    //        sbHTML.Append("<td class=\"wid_tdWeatherIcons\">");
    //        sbHTML.AppendFormat("<img alt=\"No Image\" src =\"{0}\" class=\"wid_imgWeatherIcons\">", drInput["ImageURL"].ToString());
    //        sbHTML.Append("</td>");
    //        sbHTML.Append("</tr>");
    //        sbHTML.Append("<tr>");
    //        sbHTML.Append("<td class=\"wid_tdTemp\"><nobr>");

    //        if (string.IsNullOrEmpty(drInput[2].ToString()))
    //            strTempMax = "0";
    //        else
    //            strTempMax = drInput[2].ToString();

    //        strTempMax = strTempMax + strUnit;

    //        if (string.IsNullOrEmpty(drInput[3].ToString()))
    //            strTempMin = "0";
    //        else
    //            strTempMin = drInput[3].ToString();

    //        strTempMin = strTempMin + strUnit;

    //        sbHTML.AppendFormat("{0}|{1}", strTempMin, strTempMax);//Using Coulmn names will not work in case if Culture is Changed
    //        sbHTML.Append("</td>");
    //        sbHTML.Append("</tr>");

    //        sbHTML.Append("</table>");
    //        sbHTML.Append("</td>");

    //    }


    //    sbHTML.AppendFormat("</tr>");
    //    //sbHTML.Append("<tr>");
    //    //sbHTML.Append("<td colspan="+iRowcount+" align=\"right\">");
    //    //sbHTML.Append("<a href=\"http://agricast-stage.syngenta.com/pub/glbsip02o/default.aspx\" target=\"_blank\">See 5-Day Forecast</a>");
    //    //sbHTML.Append("</td>");
    //    //sbHTML.Append("</tr>");
    //    sbHTML.Append("</table></div></div>");
    //    return sbHTML.ToString();
    //}
    #endregion



    #region Widget in Div Format
    public string GetHtmlForWidget(DataTable dtInput)
    {
        CommonUtil objCommonUtil = new CommonUtil();

        StringBuilder sbHTML = new StringBuilder();
        //Lat Long Format Change - implementation - begin
        string strPlace = place;
        //Lat Long Format Change - implementation - end

        int iRowcount = dtInput.Rows.Count;
        int width = iRowcount * 15;
        sbHTML.Append("<div id=\"ag_divModules\"><div>");
        sbHTML.Append("<div id=\"tblwidget\" class=\"widgetTblwidget\">");
        sbHTML.Append("<div class=\"divLocationContainer\">");
        /*Split the location and Forecast Text to two spans - Begin*/
        //Lat Long Format Change - implementation - begin
        if (place.IndexOf(',') != -1)
        {
            string[] strArray = place.Split(',');
            if (strArray.Length == 3)
            {
                strPlace = strArray[0];
            }
            else
            {
                strPlace = string.Format("{0} : {1}", strArray[0], strArray[1]);
            }
        }

        //Translate this text
        //sbHTML.AppendFormat("{0} : Weather Forecast", place.ToUpper());

        sbHTML.Append("<span class=\"widgetForecastLocation\">");
        sbHTML.Append(strPlace.ToUpper());
        sbHTML.Append("</span>");
        sbHTML.Append("<span class=\"widgetColon\">&nbsp;&nbsp;");
        sbHTML.Append(":&nbsp;&nbsp;</span>");
        sbHTML.Append("<span class=\"widgetForecastText\">");
        sbHTML.Append(objCommonUtil.getTransText(strWeatherForecastKey));
        sbHTML.Append("</span>");
        // sbHTML.AppendFormat("{0} : {1}", strPlace, objCommonUtil.getTransText(strWeatherForecastKey));
        /*Split the location and Forecast Text to two spans - End*/

        //Lat Long Format Change - implementation - end
        sbHTML.Append("</div>");

        //Create a <TD> tag for each Day data :- iStep
        string strTempMax = string.Empty;
        string strTempMin = string.Empty;
        string strUnit = string.Empty;

        //Unit Implementation for Embed JS -BEGIN
        //if (dtInput.Columns[2].ColumnName.IndexOf("(") != -1 && dtInput.Columns[2].ColumnName.Contains("C"))
        //    strUnit = "°C";
        //For units, Take the substring with parathesis in tempaerature series
        string strTempColName = dtInput.Columns[2].ColumnName;
        int idxLeftParan = -1;//Left ( index
        int idxRightParan = -1; //Right ) index
        if (!string.IsNullOrEmpty(strTempColName))
        {
            //GEt the indices of left and right paranthensis
            idxLeftParan = strTempColName.IndexOf("(");
            idxRightParan = strTempColName.IndexOf(")");

            if (idxLeftParan != -1 && idxRightParan != -1)
            {

                strUnit = strTempColName.Substring(idxLeftParan + 1, (idxRightParan - idxLeftParan - 1));
            }
        }
        //Unit Implementation for Embed JS - END

        sbHTML.Append("<div class=\"divwidgetContainer\">");
        foreach (DataRow drInput in dtInput.Rows)
        {
            int i= dtInput.Rows.IndexOf(drInput);
            sbHTML.Append("<div class=\"div_days_"+(i+1)+"\">");

            sbHTML.Append("<div class=\"wid_tdDaysofWeek\">");
            sbHTML.AppendFormat(drInput["DaysOfWeek"].ToString());
            sbHTML.Append("</div>");

            sbHTML.Append("<div class=\"wid_tdWeatherIcons\">");
            sbHTML.AppendFormat("<img alt=\"No Image\" src =\"{0}\" class=\"wid_imgWeatherIcons\"></img>", drInput["ImageURL"].ToString());
            sbHTML.Append("</div>");

            sbHTML.Append("<div class=\"wid_tdTemp\">");
            //sbHTML.Append("<nobr>");

            if (string.IsNullOrEmpty(drInput[2].ToString()))
                strTempMax = "0";
            else
                strTempMax = drInput[2].ToString();

            strTempMax = strTempMax + strUnit;

            if (string.IsNullOrEmpty(drInput[3].ToString()))
                strTempMin = "0";
            else
                strTempMin = drInput[3].ToString();

            strTempMin = strTempMin + strUnit;

            sbHTML.Append("<div class=\"wid_divTempMax\">");
            sbHTML.Append(strTempMax);
            sbHTML.Append("</div>");

            sbHTML.Append("<div class=\"wid_divTempMin\">");
            sbHTML.Append(strTempMin);
            sbHTML.Append("</div>");
            //sbHTML.AppendFormat("{0}|{1}", strTempMin, strTempMax);//Using Coulmn names will not work in case if Culture is Changed
            //sbHTML.Append("</nobr>");
            sbHTML.Append("</div>");

            sbHTML.Append("</div>");

        }


        sbHTML.AppendFormat("</div>");//closing tag of div id=container

        sbHTML.Append("</div>");//closing tag of div id=tblWidget
        sbHTML.Append("</div></div>");

        return sbHTML.ToString();
    }

    #endregion

    #endregion

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

    string checkCountryValidity(double lat, double lng, string strCountry)
    {
        //check if the Lat long specified belongs to the selected country code
        //if found, method returns country code , if not, some junk html
        /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  Begin*/
        string strCountryCode = objComUtil.CheckLatLongForCountry(double.Parse(lat.ToString()), double.Parse(lng.ToString()));
        //string strCountryCode = "CN";
        /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  ENd*/
        //takte the first two characters
        if (!string.IsNullOrEmpty(strCountryCode))
            strCountryCode = strCountryCode.Substring(0, 2);


        /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  Begin*/
        //This check for the lat , lng for the specified country
        //incase if the country is not specified in the query string , strCOuntry wil be empty. 
        //In this scenario,we need to search for the lat lng in all the countries specified in the service configuration
        //if (!string.IsNullOrEmpty(strCountry))
        //{
        //    if (strCountryCode.Equals(strCountry))
        //    {
        //        isLatLongValid = true;
        //    }
        //    else
        //    {
        //        isLatLongValid = false;
        //        // HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.GEN_LATLONGINVALID);
        //    }
        //}
        // check if the country to which the lat ln belongs to , is specified in the service config    
        //else
        //{
        DataTable dtCountries = sh.loadCountries();
        if (dtCountries != null && dtCountries.Rows.Count > 0)
        {
            // look for the country code obtained from CheckLatLongForCountry() in the datatable
            if (dtCountries.Select(string.Format("code= '{0}'", strCountryCode.ToUpper())).Distinct().Count() > 0)
            {
                DataTable dt = dtCountries.Select(string.Format("code= '{0}'", strCountryCode.ToUpper())).Distinct().CopyToDataTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    isLatLongValid = true;

                }
                else
                {
                    isLatLongValid = false;
                }
            }
            else
                isLatLongValid = false;

        }
        //}
        return strCountryCode;
        /*IM01173263 - New Agricast - EmbeddedJS- location search querystring -  END*/
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

    //Wind icons and Relative Humdity to Embed JS - Begin
    public Control LoadCtrl(Page page, PlaceHolder Container, string Filename)
    {
        Control ctr;
        string fullName = "UserControls\\" + Filename + ".ascx";
        ctr = page.LoadControl(fullName);
        Container.Controls.Add(ctr);
        Container.ID = Filename + "1";
        return ctr;
    }

    //Wind icons and Relative Humdity to Embed JS - End
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
        if (string.IsNullOrEmpty(objLocInfo.latitude == 0.0 ? "" : objLocInfo.latitude.ToString()) || string.IsNullOrEmpty(objLocInfo.longitude == 0.0 ? "" : objLocInfo.longitude.ToString()))
        {
            if(!string.IsNullOrEmpty(lat))
            objLocInfo.latitude = double.Parse(lat);
            if(!string.IsNullOrEmpty(lng))
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
                
                strText = strText.Replace("{Dir}", objComUtil.getTextDirection(Convert.ToInt32(dtNearByStations.Rows[i]["BearingDegrees"].ToString())));
                strText = strText.Replace("{CityName}", objLocInfo.placeName);
                if (objSvcInfo.Unit.ToLower().Equals("imperial", StringComparison.CurrentCultureIgnoreCase))
                {
                    strText = strText.Replace("{Dist}", Convert.ToString(Math.Round(Convert.ToDouble(objComUtil.ConvertValueMetrics(dtNearByStations.Rows[i]["DistanceKm"].ToString(), "km-mi")))));
                    strText = strText.Replace("{Elevation}", objComUtil.ConvertValueMetrics(dtNearByStations.Rows[i]["Altitude"].ToString(),"m-ft"));
                }
                else
                {
                    strText = strText.Replace("{Dist}", Convert.ToString(Math.Round(Convert.ToDouble(dtNearByStations.Rows[i]["DistanceKm"].ToString()))));
                    strText = strText.Replace("{Elevation}", dtNearByStations.Rows[i]["Altitude"].ToString());
                }
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
                            string TransString = objSvcPre.getTranslatedText(text.Substring(startDelimiter.Count(), text.Length - (endDelimiter.Count() + startDelimiter.Count())), objSvcInfo.Culture);
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
}
#endregion