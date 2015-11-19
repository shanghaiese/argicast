using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using Syngenta.AgriCast.LocationSearch.LocWebService;
using Syngenta.AgriCast.LocationSearch.View;
using Syngenta.AgriCast.LocationSearch.Presenter;
using System.Text.RegularExpressions;
using Syngenta.AgriCast.Common;
using Syngenta.AgriCast.Common.View;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common.Presenter;
using System.Web.SessionState;
using Syngenta.AgriCast.ExceptionLogger;
using System.Configuration;
using System.Net;
using System.Xml;
using System.Text;
using Syngenta.AgriWeb.LocationSearch.DataObjects;
using System.Linq.Expressions;

namespace Syngenta.AgriCast.LocationSearch
{
    public partial class WebLocationSearch : System.Web.UI.UserControl, ILocSearch
    {
        #region Declaration
        LocSearchWebService objWebService = new LocSearchWebService();
        locSearchPresenter objLocPresenter;
        nearbyPointPresenter objNearByPre;
        ToolBarService objToolSvc = new ToolBarService();
        ServiceInfo objSvcInfo;
        DataPointInfo objDataPointInfo;
        LocationInfo objLocInfo;
        CommonUtil objComUtil = new CommonUtil();
        List<Location> locationList = new List<Location>();
        DataTable dtFav = new DataTable();
        DataTable dtCon = new DataTable();
        DataTable dtNearbystatn = new DataTable();
        LocationSearchSource eProvider;
        ServicePresenter objSvcPre = new ServicePresenter();
        bool IsCountryPresent = true;

        #endregion


        protected WebLocationSearch()
        {
            objLocPresenter = new locSearchPresenter(this);
            objNearByPre = new nearbyPointPresenter(this);
            objSvcInfo = ServiceInfo.ServiceConfig;
            objDataPointInfo = DataPointInfo.getDataPointObject;
            objLocInfo = LocationInfo.getLocationInfoObj;

        }

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                getCountry();
                if (!IsPostBack)
                {

                    prefillpage();
                    objLocPresenter.CheckMapVisibility();
                    liMap.Visible = showMap;
                }

                objLocPresenter.getProviderName();
                objLocInfo.Provider = eProviderName;
                if (objLocInfo.searchLocation != "" && objLocInfo.searchLocation != null)
                    getLocation();
                else
                {
                    LoadStations();
                    hdnGridStatus.Value = "hide";
                }
                //searchbox.Focus();
                this.Page.Form.DefaultButton = btnDefault.UniqueID;
                objLocPresenter.getTranslatedText(Constants.SEARCH, objSvcInfo.Culture);
                btnSearch.Text = strTranslatedText;
                objLocPresenter.getTranslatedText("Enter the search criteria", objSvcInfo.Culture);
                searchbox.Attributes.Add("placeholder", strTranslatedText);

            }


            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_LOAD_FAILURE) + ex.Message.ToString();
                return;

            }
        }

        protected void Page_prerender(object sender, EventArgs e)
        {


            if (this.gvStations.Rows.Count > 0)
            {
                gvStations.UseAccessibleHeader = true;
                gvStations.HeaderRow.TableSection = TableRowSection.TableHeader;
            }

            if (this.gvLocation.Rows.Count > 0)
            {
                gvLocation.UseAccessibleHeader = true;
                gvLocation.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            //To check the status of the Location and stations grid
            if (hdnGridStatus.Value == "Show-Location")
            {
                divLocation.Attributes.Add("class", "show");
                gvLocation.CssClass = "show";
                hNearByPoint.CssClass = "hide";
            }
            else if (hdnGridStatus.Value == "Show-Station")
            {
                divLocation.Attributes.Add("class", "Show");
                gvStations.CssClass = "tablesorter show";
                hNearByPoint.CssClass = "show";

            }
            else
            {
                divLocation.Attributes.Add("class", "hide");

            }



            if (gvStations.Rows.Count == 0)
            {
                hNearByPoint.Text = "";
            }
            if (IsCountryPresent && objLocInfo.CountryCode != string.Empty)
            {
                ddlCountry.SelectedValue = objLocInfo.CountryCode.ToUpper();
            }


            //set the searched location in the Saerch box
            /* IM01377132 - New Agricast - Default location : should keep the capital letters - Jerrey - Begin */
            //searchbox.Value = (searchbox.Value.ToString() != "" && searchbox.Value.ToString() != objLocInfo.placeName) ? searchbox.Value.ToString() : HttpUtility.HtmlDecode(objLocInfo.placeName);
            searchbox.Value = (searchbox.Value.ToString() != "" && searchbox.Value.ToString().ToLower() != objLocInfo.placeName.ToLower()) ? searchbox.Value.ToString() : HttpUtility.HtmlDecode(objLocInfo.placeName);
            /* IM01377132 - New Agricast - Default location : should keep the capital letters - Jerrey - Begin */
        }

        //This event is triggered on click of serach button after selecting the location
        protected void btnSearch_Click(object sender, EventArgs e)
        {

            try
            {
                var lat = 0.00;
                var lang = 0.00;
                /*IM01246266 - New Agricast - can't save a favourite - BEGIN*/
                int placeID = 0;
                /*IM01246266 - New Agricast - can't save a favourite - END*/
                objLocInfo.latitude = lat;
                objLocInfo.longitude = lang;

                /*IM01246266 - New Agricast - can't save a favourite - BEGIN*/
                //Remove the placeid appeneded at the end in Autocomplete.js 
                if (!string.IsNullOrEmpty(searchboxLatLng.Value))
                {
                    if (searchboxLatLng.Value.IndexOf('|') > 0)
                    {
                        //set the place Id 
                        placeID = int.Parse(searchboxLatLng.Value.Split('|')[1] ?? "0.0");

                        //set the lat long
                        searchboxLatLng.Value = searchboxLatLng.Value.Split('|')[0];
                    }
                }
                /*IM01246266 - New Agricast - can't save a favourite - END*/

                // if (searchbox.Attributes["latlng"] != null && searchbox.Attributes["latlng"].ToString() != "")
                if (searchboxLatLng.Value != null && searchboxLatLng.Value.ToString() != "")
                {
                    if (searchboxLatLng.Value.ToString().IndexOf(',') > 0 && searchboxLatLng.Value.ToString().Split(',')[0] != null)
                    {
                        double.TryParse(searchboxLatLng.Value.ToString().Split(',')[0], out lat);
                        double.TryParse(searchboxLatLng.Value.ToString().Split(',')[1], out lang);

                        //string MatchLatPattern = @"^-?([1-8]?[1-9]|[1-9]0)\.\d{0,6}";
                        //string MatchLongPattern = @"^-?([1]?[1-7][1-9]|[1]?[1-8][0]|[1-9]?[0-9])\.\d{0,6}";
                        string MatchLatPattern = @"^-?([1-8]?[1-9]|[1-9]0)?\.?[0-9]{0,4}";
                        string MatchLongPattern = @"^-?([1]?[1-7][1-9]|[1]?[1-8][0]|[1-9]?[0-9])?\.?[0-9]{0,4}";


                        if (Regex.IsMatch(lat.ToString(), MatchLatPattern) && Regex.IsMatch(lang.ToString(), MatchLongPattern))
                        {

                            objLocInfo.latitude = lat;
                            objLocInfo.longitude = lang;

                            objLocInfo.placeName = searchbox.Value;
                            /*IM01246266 - New Agricast - can't save a favourite - BEGIN*/
                            objLocInfo.placeID = placeID;
                            /*IM01246266 - New Agricast - can't save a favourite - END*/

                        }

                        searchboxLatLng.Value = "";
                        if (Session["AuditData"] != null)
                        {
                            IDictionary dict = (IDictionary)Session["AuditData"];
                            dict["locSearchStringType"] = "Placename";
                            dict["locSearchString"] = searchbox.Value;
                            if (objLocInfo.latitude == 0)
                                dict["searchLat"] = DBNull.Value;
                            else
                                dict["searchLat"] = objLocInfo.latitude;
                            if (objLocInfo.longitude == 0)
                                dict["searchLong"] = DBNull.Value;
                            else
                                dict["searchLong"] = objLocInfo.longitude;
                            Session["AuditData"] = dict;
                        }
                    }
                }
                else
                {
                    //If the user enter the coordinates
                    if (searchbox.Value.ToString().IndexOf(',') > 0 && searchbox.Value.ToString().Split(',')[0] != null)
                    {
                        double.TryParse(searchbox.Value.ToString().Split(',')[0], out lat);
                        double.TryParse(searchbox.Value.ToString().Split(',')[1], out lang);

                        //string MatchLatPattern = @"^-?([1-8]?[1-9]|[1-9]0)\.\d{0,6}";
                        //string MatchLongPattern = @"^-?([1]?[1-7][1-9]|[1]?[1-8][0]|[1-9]?[0-9])\.\d{0,6}";
                        string MatchLatPattern = @"^-?([1-8]?[1-9]|[1-9]0)?\.?[0-9]{0,4}";
                        string MatchLongPattern = @"^-?([1]?[1-7][1-9]|[1]?[1-8][0]|[1-9]?[0-9])?\.?[0-9]{0,4}";

                        if (Regex.IsMatch(lat.ToString(), MatchLatPattern) && Regex.IsMatch(lang.ToString(), MatchLongPattern))
                        {

                            objLocInfo.latitude = lat;
                            objLocInfo.longitude = lang;
                            objLocInfo.placeName = "Lat: " + lat + " Long: " + lang;
                            if (Session["AuditData"] != null)
                            {
                                IDictionary dict = (IDictionary)Session["AuditData"];
                                dict["locSearchStringType"] = "Lat/Long";
                                dict["locSearchString"] = searchbox.Value;
                                Session["AuditData"] = dict;
                            }

                        }


                    }
                    //Split the strings with "(" butnot with both ',' and "(" and take first part as Search string

                    else if (searchbox.Value.ToString().IndexOf(',') < 0 && searchbox.Value.ToString().IndexOf('(') > 0)
                    {
                        if (!string.IsNullOrEmpty(searchbox.Value.ToString()))
                            objLocInfo.searchLocation = searchbox.Value.ToString().Split('(')[0];
                        if (Session["AuditData"] != null)
                        {
                            IDictionary dict = (IDictionary)Session["AuditData"];
                            dict["locSearchStringType"] = "Placename";
                            dict["locSearchString"] = searchbox.Value;
                            Session["AuditData"] = dict;
                        }

                    }
                    else
                    {
                        objLocInfo.searchLocation = HttpUtility.HtmlDecode(searchbox.Value.ToString());
                        if (Session["AuditData"] != null)
                        {
                            IDictionary dict = (IDictionary)Session["AuditData"];
                            dict["locSearchStringType"] = "Placename";
                            dict["locSearchString"] = searchbox.Value;
                            Session["AuditData"] = dict;
                        }

                    }
                }

                objLocInfo.CountryCode = ddlCountry.SelectedValue.ToString();


                if (objLocInfo.longitude != 0.00 && objLocInfo.latitude != 0.00)
                {
                    //check if the Lat long specified belongs to the selected country code
                    //if found, method returns country code , if not, some junk html
                    string strCountryCode = objComUtil.CheckLatLongForCountry(double.Parse(lat.ToString()), double.Parse(lang.ToString()));

                    //takte the first two characters
                    if (!string.IsNullOrEmpty(strCountryCode))
                        strCountryCode = strCountryCode.Substring(0, 2);

                    if (strCountryCode.Equals(ddlCountry.SelectedValue.ToString()))
                    {
                        //load stations only if it lat long is valid for that country
                        LoadStations();

                        // //show the neasr by stations grid
                        hNearByPoint.CssClass = "show";
                        gvStations.CssClass = "tablesorter show";

                        lblStnName.Visible = true;
                        lblDistText.Visible = true;
                        lblPointText.Visible = true;
                    }
                    else
                    {
                        clearDataPoint();

                        // LoadStations();       
                        //gvStations.DataSource = null;
                        //gvStations.DataBind();

                        // //hide the neasr by stations grid
                        hNearByPoint.CssClass = "hide";

                        //hide the distance display label
                        lblStnName.Visible = false;
                        lblDistText.Visible = false;
                        lblPointText.Visible = false;
                        gvStations.CssClass = "tablesorter hide";

                        HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.GEN_LATLONGINVALID);

                        //clear the lat lon in location info object\
                        objLocInfo.latitude = 0.0;
                        objLocInfo.longitude = 0.0;
                    }

                }
                else
                {
                    getLocation();

                }
                hdnLocSelected.Value = objLocInfo.latitude + "|" + objLocInfo.longitude + "|" + objLocInfo.placeName;

            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_SEARCH_FAILURE) + ex.Message.ToString();
                return;
            }
            finally
            {
                if (Session["AuditData"] != null)
                {
                    IDictionary dict = (IDictionary)Session["AuditData"];
                    dict["locSearchType"] = "By Search";
                    //dict["locSearchString"] = objLocInfo.searchLocation;
                    dict["locSearchDatasource"] = objLocInfo.Provider;
                    dict["numOfLocs"] = gvLocation.Rows.Count;
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
                    dict["weatherDatasource"] = objDataPointInfo.NearbyPointSettings.DataSource;
                    if (objDataPointInfo.stationLatitude == 0)
                        dict["weatherLat"] = DBNull.Value;
                    else
                        dict["weatherLat"] = objDataPointInfo.stationLatitude;
                    if (objDataPointInfo.stationLongitude == 0)
                        dict["weatherLong"] = DBNull.Value;
                    else
                        dict["weatherLong"] = objDataPointInfo.stationLongitude;
                    Session["AuditData"] = dict;
                }

                //objSvcPre.SaveAuditData();
                objLocInfo.searchLocation = "";
                //searchbox.Value = "";
            }
        }


        protected void gvLocation_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvLocation.EditIndex = e.NewEditIndex;
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_ROW_EDIT) + ex.Message.ToString();
            }
        }



        protected void gvLocation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int iPlaceID = 0;

            try
            {
                GridView _gridView = (GridView)sender;

                if (e.CommandName != "Sort")
                {
                    int _selectedIndex = int.Parse(e.CommandArgument.ToString());
                    string _commandName = e.CommandName;

                    _gridView.SelectedIndex = _selectedIndex;
                    lblPointText.Visible = true;
                    lblStnName.Visible = true;
                    lblDistText.Visible = true;
                    objLocInfo.placeName = gvLocation.SelectedRow.Cells[0].Text;
                    objLocInfo.latitude = Convert.ToDouble(gvLocation.SelectedRow.Cells[1].Text);
                    objLocInfo.longitude = Convert.ToDouble(gvLocation.SelectedRow.Cells[2].Text);
                    objLocInfo.CountryCode = ddlCountry.SelectedValue.ToString();
                    objLocInfo.searchLocation = searchbox.Value.ToString();

                    objLocInfo.placeID = int.TryParse(gvLocation.Rows[0].Cells[6].Text.ToString(), out iPlaceID) ? iPlaceID : 0;

                    LoadStations();

                    gvStations.Visible = true;
                    gvLocation.CssClass = "hide";
                    divLocation.Attributes.Add("class", "hide");
                    hdnGridStatus.Value = "hide";
                    hNearByPoint.CssClass = "show";
                    hdnLatLng.Value = objLocInfo.latitude + "|" + objLocInfo.longitude + "|" + objLocInfo.placeName + "|" + "#LocList";
                    hdnLocSelected.Value = objLocInfo.latitude + "|" + objLocInfo.longitude + "|" + objLocInfo.placeName;
                }

            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.GENERIC_ERRORONPAGE) + ex.Message.ToString();
            }
            finally
            {
                if (Session["AuditData"] != null)
                {
                    IDictionary dict = (IDictionary)Session["AuditData"];
                    dict["locSearchDatasource"] = objLocInfo.Provider;
                    dict["numOfLocs"] = gvLocation.Rows.Count;
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
                    Session["AuditData"] = dict;
                }

            }
        }


        protected void gvLocation_RowDataBound(object obj, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Get reference to button field in the gridview.  
                    LinkButton _singleClickButton = (LinkButton)e.Row.Cells[5].Controls[0];
                    string _jsSingle = Page.ClientScript.GetPostBackClientHyperlink(_singleClickButton, "");
                    _jsSingle = _jsSingle.Insert(11, "setTimeout(\"");
                    _jsSingle += "\",300)";
                    e.Row.Attributes["onclick"] = _jsSingle;
                }
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.GENERIC_ERRORONPAGE) + ex.Message.ToString();
            }

        }

        protected void gvStations_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                GridView _gridView = (GridView)sender;

                //// Get the selected index and the command name
                if (e.CommandName != "Sort")
                {
                    int _selectedIndex = int.Parse(e.CommandArgument.ToString());
                    string _commandName = e.CommandName;

                    _gridView.SelectedIndex = _selectedIndex;
                    //IM01848085:New Agricast - Agriinfo - "location" name not translatable - Start
                    //lblStnName.InnerText = HttpUtility.HtmlDecode(gvStations.SelectedRow.Cells[0].Text);
                    string lattranslate = objSvcPre.getTranslatedText("Lat", objSvcInfo.Culture);
                    string longtranslate = objSvcPre.getTranslatedText("Long", objSvcInfo.Culture);
                    string lblStnNameInfo = HttpUtility.HtmlDecode(gvStations.SelectedRow.Cells[0].Text);
                    lblStnNameInfo = lblStnNameInfo.Replace("Lat", lattranslate);
                    lblStnName.InnerText = lblStnNameInfo.Replace("Long", longtranslate);
                    //IM01848085:New Agricast - Agriinfo - "location" name not translatable - End
                    lblDistText.InnerHtml = getDistLabelText(Convert.ToString(Math.Round(Convert.ToDouble(gvStations.SelectedRow.Cells[1].Text))), objComUtil.getTextDirection(Convert.ToInt32(gvStations.SelectedRow.Cells[8].Text)), gvStations.SelectedRow.Cells[2].Text);
                    setStationParameters(gvStations.SelectedRow.Cells[0].Text, Convert.ToDouble(gvStations.SelectedRow.Cells[4].Text), Convert.ToDouble(gvStations.SelectedRow.Cells[3].Text), Convert.ToInt32(gvStations.SelectedRow.Cells[7].Text), Convert.ToInt32(gvStations.SelectedRow.Cells[8].Text),
                    Convert.ToInt32(HttpUtility.HtmlDecode(gvStations.SelectedRow.Cells[9].Text).Trim() == "" ? "0" : HttpUtility.HtmlDecode(gvStations.SelectedRow.Cells[9].Text)), Convert.ToInt32(HttpUtility.HtmlDecode(gvStations.SelectedRow.Cells[10].Text).Trim() == "" ? "0" : HttpUtility.HtmlDecode(gvStations.SelectedRow.Cells[10].Text)), Convert.ToInt32(gvStations.SelectedRow.Cells[2].Text), Convert.ToDouble(gvStations.SelectedRow.Cells[1].Text));
                    hdnGridStatus.Value = "hide";
                    lblPointText.Visible = true;
                    lblStnName.Visible = true;
                    lblDistText.Visible = true;
                }
                else
                {
                    hdnGridStatus.Value = "Show-Stations";
                }
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.GENERIC_ERRORONPAGE) + ex.Message.ToString();
            }
            finally
            {
                if (Session["AuditData"] != null)
                {
                    IDictionary dict = (IDictionary)Session["AuditData"];
                    dict["weatherDatasource"] = objDataPointInfo.NearbyPointSettings.DataSource;
                    if (objDataPointInfo.stationLatitude == 0)
                        dict["weatherLat"] = DBNull.Value;
                    else
                        dict["weatherLat"] = objDataPointInfo.stationLatitude;
                    if (objDataPointInfo.stationLongitude == 0)
                        dict["weatherLong"] = DBNull.Value;
                    else
                        dict["weatherLong"] = objDataPointInfo.stationLongitude;

                    Session["AuditData"] = dict;
                }
            }
        }

        protected void gvStations_RowDataBound(object obj, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Get reference to button field in the gridview.  
                    LinkButton _singleClickButton = (LinkButton)e.Row.Cells[6].Controls[0];
                    string _jsSingle = Page.ClientScript.GetPostBackClientHyperlink(_singleClickButton, "");
                    _jsSingle = _jsSingle.Insert(11, "setTimeout(\"");
                    _jsSingle += "\",300)";
                    //e.Row.Style["cursor"] = "hand";
                    e.Row.Attributes["onclick"] = _jsSingle;
                }
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.GENERIC_ERRORONPAGE) + ex.Message.ToString();
            }
        }

        //Method triggers on click of palce or station on map
        protected void hdnLatLng_ValueChanged(object sender, EventArgs e)
        {
            try
            {

                string strLatLng = hdnLatLng.Value.ToString();
                if (strLatLng != "")
                {
                    lblStnName.Visible = false;
                    lblDistText.Visible = false;
                    lblPointText.Visible = false;
                    if ((strLatLng.Split('|')[3]) == "#LocList")
                    {
                        string strName = "";
                        double Lat;
                        double Long;
                        string lattranslate;
                        string longtranslate;
                        string lblStnNameInfo;
                        Lat = Convert.ToDouble(strLatLng.Split('|')[0]);
                        Long = Convert.ToDouble(strLatLng.Split('|')[1]);
                        strName = strLatLng.Split('|')[2];
                        objNearByPre.getNearbyPoints(Lat, Long);

                        if (gvStations.Rows.Count != 0)
                        {
                            //for (int i = 0; i < gvLocation.Rows.Count; i++)
                            //{
                            //    if (gvLocation.Rows[i].Cells[2].Text == Long.ToString())
                            //    {
                            //        strName = gvLocation.Rows[i].Cells[0].Text;
                            //        break;
                            //    }
                            //}
                            objLocInfo.placeName = strName;
                            objLocInfo.latitude = Lat;
                            objLocInfo.longitude = Long;
                            objLocInfo.CountryCode = ddlCountry.SelectedValue.ToString();
                            //objLocInfo.searchLocation = searchbox.Value.ToString();
                            setColumnText();

                            //IM01848085:New Agricast - Agriinfo - "location" name not translatable - Start
                            //lblStnName.InnerText = HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[0].Text);
                            lattranslate = objSvcPre.getTranslatedText("Lat", objSvcInfo.Culture);
                            longtranslate = objSvcPre.getTranslatedText("Long", objSvcInfo.Culture);
                            lblStnNameInfo = HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[0].Text);
                            lblStnNameInfo = lblStnNameInfo.Replace("Lat", lattranslate);
                            lblStnName.InnerText = lblStnNameInfo.Replace("Long", longtranslate);
                            //IM01848085:New Agricast - Agriinfo - "location" name not translatable - End
                            lblDistText.InnerHtml = getDistLabelText(Convert.ToString(Math.Round(Convert.ToDouble(gvStations.Rows[0].Cells[1].Text))), objComUtil.getTextDirection(Convert.ToInt32(gvStations.Rows[0].Cells[8].Text)), gvStations.Rows[0].Cells[2].Text);
                            lblStnName.Visible = true;
                            lblDistText.Visible = true;
                            lblPointText.Visible = true;
                            Loc_NoMatchFor.Visible = false;
                            DataRow dr = null;
                            double lat;
                            double lng;
                            double dist;
                            int bearingdeg;
                            int DpId;
                            int TZone;
                            int Dst;
                            int Alt;
                            string strStation = objDataPointInfo.stationName;
                            dr = dtNearByPoints.Select("name = '" + strStation.Replace("'", "''") + "'").Length == 0 ? null : dtNearByPoints.Select("name = '" + strStation.Replace("'", "''") + "'")[0];
                            if (dr != null)
                            {
                                //IM01848085:New Agricast - Agriinfo - "location" name not translatable - Start
                                //lblStnName.InnerText = HttpUtility.HtmlDecode(dr["Name"].ToString());
                                lblStnNameInfo = HttpUtility.HtmlDecode(dr["Name"].ToString());
                                lblStnNameInfo = lblStnNameInfo.Replace("Lat", lattranslate);
                                lblStnName.InnerText = lblStnNameInfo.Replace("Long", longtranslate);
                                //IM01848085:New Agricast - Agriinfo - "location" name not translatable - End

                                lat = double.TryParse(dr["Latitude"].ToString(), out lat) ? lat : 0.0;
                                lng = double.TryParse(dr["Longitude"].ToString(), out lng) ? lng : 0.0;
                                bearingdeg = Int32.TryParse(dr["BearingDegrees"].ToString(), out bearingdeg) ? bearingdeg : 0;
                                dist = double.TryParse(dr["DistanceKm"].ToString(), out dist) ? dist : 0;
                                DpId = Convert.ToInt32((dr["DataPointID"].ToString() == "" ? "0" : dr["DataPointID"].ToString()));
                                TZone = Convert.ToInt32(dr["TimezoneOffset"].ToString() == "" ? "0" : dr["TimezoneOffset"].ToString());
                                Dst = Convert.ToInt32(dr["DstOn"].ToString() == "" ? "0" : dr["DSTon"].ToString());
                                Alt = Convert.ToInt32(dr["Altitude"].ToString() == "" ? "0" : dr["Altitude"].ToString());
                                lblDistText.InnerHtml = getDistLabelText(Math.Round(dist).ToString(), objComUtil.getTextDirection(bearingdeg), dr["Altitude"].ToString());
                                lblPointText.Visible = true;
                                lblStnName.Visible = true;
                                lblDistText.Visible = true;
                                Loc_NoMatchFor.Visible = false;
                                //adding provider
                                Session["ForecastProvider"] = HttpUtility.HtmlDecode(dr["ProviderName"].ToString());
                                setStationParameters(dr["Name"].ToString(), lat, lng, DpId, bearingdeg, TZone, Dst, Alt, dist);
                            }
                            else
                            {
                                //adding provider
                                Session["ForecastProvider"] = HttpUtility.HtmlDecode(dtNearByPoints.Rows[0]["ProviderName"].ToString());
                                lat = double.TryParse(gvStations.Rows[0].Cells[4].Text.ToString(), out lat) ? lat : 0.0;
                                lng = double.TryParse(gvStations.Rows[0].Cells[3].Text.ToString(), out lng) ? lng : 0.0;
                                bearingdeg = Int32.TryParse(gvStations.Rows[0].Cells[8].Text.ToString(), out bearingdeg) ? bearingdeg : 0;
                                dist = double.TryParse(gvStations.Rows[0].Cells[1].Text.ToString(), out dist) ? dist : 0;

                                //IM01848085:New Agricast - Agriinfo - "location" name not translatable - Start
                                //lblStnName.InnerText = HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[0].Text);
                                lblStnNameInfo = HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[0].Text);
                                lblStnNameInfo = lblStnNameInfo.Replace("Lat", lattranslate);
                                lblStnName.InnerText = lblStnNameInfo.Replace("Long", longtranslate);
                                //IM01848085:New Agricast - Agriinfo - "location" name not translatable - End
                                lblDistText.InnerHtml = getDistLabelText(Math.Round(dist).ToString(), objComUtil.getTextDirection(bearingdeg), gvStations.Rows[0].Cells[2].Text);
                                lblPointText.Visible = true;
                                lblStnName.Visible = true;
                                lblDistText.Visible = true;
                                Loc_NoMatchFor.Visible = false;
                                setStationParameters(gvStations.Rows[0].Cells[0].Text, lat, lng, Convert.ToInt32(gvStations.Rows[0].Cells[7].Text ?? "0"), bearingdeg, Convert.ToInt32(HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[9].Text).Trim() == "" ? "0" : HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[9].Text)),
                                Convert.ToInt32(HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[10].Text).Trim() == "" ? "0" : HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[10].Text)), Convert.ToInt32(gvStations.Rows[0].Cells[2].Text ?? "0"), dist);
                            }
                            hNearByPoint.CssClass = "show";
                        }
                        else if (gvStations.Rows.Count == 0)
                        {
                            clearDataPoint();
                            HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_NOSTATIONSFOUND);

                        }
                    }

                    else
                    {
                        if (gvStations.Rows.Count != 0)
                        {
                            for (int i = 0; i < gvStations.Rows.Count; i++)
                            {
                                if (Convert.ToDouble(gvStations.Rows[i].Cells[3].Text).ToString("0.0000").TrimEnd('0') == String.Format("{0:0.0000}", Convert.ToDouble(strLatLng.Split('|')[1])).ToString().TrimEnd('0') &&
                                    Convert.ToDouble(gvStations.Rows[i].Cells[4].Text).ToString("0.0000").TrimEnd('0') == String.Format("{0:0.0000}", Convert.ToDouble(strLatLng.Split('|')[0])).ToString().TrimEnd('0'))
                                {
                                    lblDistText.InnerHtml = getDistLabelText(Convert.ToString(Math.Round(Convert.ToDouble(gvStations.Rows[i].Cells[1].Text))), objComUtil.getTextDirection(Convert.ToInt32(gvStations.Rows[i].Cells[8].Text)), gvStations.Rows[i].Cells[2].Text);
                                    lblStnName.InnerText = HttpUtility.HtmlDecode(gvStations.Rows[i].Cells[0].Text);
                                    lblStnName.Visible = true;
                                    lblDistText.Visible = true;
                                    lblPointText.Visible = true;
                                    setStationParameters(gvStations.Rows[i].Cells[0].Text, Convert.ToDouble(gvStations.Rows[i].Cells[4].Text), Convert.ToDouble(gvStations.Rows[i].Cells[3].Text), Convert.ToInt32(gvStations.Rows[i].Cells[7].Text), Convert.ToInt32(gvStations.Rows[i].Cells[8].Text),
                                    Convert.ToInt32(HttpUtility.HtmlDecode(gvStations.Rows[i].Cells[9].Text).Trim() == "" ? "0" : HttpUtility.HtmlDecode(gvStations.Rows[i].Cells[9].Text)), Convert.ToInt32(HttpUtility.HtmlDecode(gvStations.Rows[i].Cells[10].Text).Trim() == "" ? "0" : HttpUtility.HtmlDecode(gvStations.Rows[i].Cells[10].Text)), Convert.ToInt32(gvStations.Rows[i].Cells[2].Text), Convert.ToDouble(gvStations.Rows[i].Cells[1].Text));
                                    break;
                                }
                            }
                        }
                        else if (gvStations.Rows.Count == 0)
                        {
                            clearDataPoint();

                        }
                    }
                    gvStations.Visible = true;
                }
                hdnGridStatus.Value = "hide";
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.GENERIC_ERRORONPAGE) + ex.Message.ToString();
            }
            finally
            {
                objLocInfo.searchLocation = "";
            }
        }
        #endregion

        # region Methods
        //Method to get the culture code and to translate the the text
        public void getCultureCode(string strCultureCode)
        {
            string strTranForecastFor = string.Empty;
            try
            {
                objLocPresenter.getTranslatedText(Constants.SEARCH_IN, strCultureCode);
                searchIn.Text = strTranslatedText;
                objLocPresenter.getTranslatedText(@for.ID, strCultureCode);
                @for.Text = strTranslatedText;

                //set the translated text for List and map view 
                objLocPresenter.getTranslatedText(Constants.LIST_VIEW, strCultureCode);
                textlist.InnerText = strTranslatedText;

                objLocPresenter.getTranslatedText(Constants.MAP_VIEW, strCultureCode);
                textmap.InnerText = strTranslatedText;

                //SOCB - Forecast text change
                if (objLocInfo != null)
                {
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                    //if (objLocInfo.ServiceInfo != null && objLocInfo.ServiceInfo.Module.ToLower().Contains("history"))
                    if (objLocInfo.ServiceInfo != null
                        && (objLocInfo.ServiceInfo.Module.ToLower().Contains("history") || objLocInfo.ServiceInfo.Module.ToLower().Contains("watermodel")))
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                    {
                        strTranForecastFor = "Weather for";
                        objLocPresenter.getTranslatedText(Constants.NEARBY_GRID, strCultureCode);
                        hNearByPoint.Text = strTranslatedText;
                    }
                    else
                    {
                        strTranForecastFor = "Forecast for";
                        objLocPresenter.getTranslatedText(Constants.NEARBY_STATIONS, strCultureCode);
                        hNearByPoint.Text = strTranslatedText;
                    }
                }

                objLocPresenter.getTranslatedText(strTranForecastFor, strCultureCode);
                lblPointText.Text = strTranslatedText;

                //EOCB  - forecast text change
                if (gvLocation.Rows.Count != 0)
                {
                    objLocPresenter.getTranslatedText(Constants.NAME, objSvcInfo.Culture);
                    (gvLocation.HeaderRow.Cells[0]).Text = strTranslatedText;
                    objLocPresenter.getTranslatedText(Constants.LATITUDE, objSvcInfo.Culture);
                    (gvLocation.HeaderRow.Cells[1]).Text = strTranslatedText;
                    objLocPresenter.getTranslatedText(Constants.LONGITUDE, objSvcInfo.Culture);
                    (gvLocation.HeaderRow.Cells[2]).Text = strTranslatedText;
                    objLocPresenter.getTranslatedText(Constants.ADMIN1, objSvcInfo.Culture);
                    (gvLocation.HeaderRow.Cells[3]).Text = strTranslatedText;
                    objLocPresenter.getTranslatedText(Constants.ADMIN2, objSvcInfo.Culture);
                    (gvLocation.HeaderRow.Cells[4]).Text = strTranslatedText;
                }

                if (gvStations.Rows.Count != 0)
                {
                    objLocPresenter.getTranslatedText(Constants.NEAR_STATION_NAME, objSvcInfo.Culture);
                    (gvStations.HeaderRow.Cells[0]).Text = strTranslatedText;
                    objLocPresenter.getTranslatedText(Constants.NEAR_DISTANCE, objSvcInfo.Culture);
                    (gvStations.HeaderRow.Cells[5]).Text = strTranslatedText;
                    setColumnText();
                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.CULTURE_LOADFAILURE) + ex.Message.ToString();
                return;
            }
        }

        //Method to populate the country dropdown.
        protected void getCountry()
        {
            try
            {
                objLocPresenter.getCountry();


                if (objLocInfo.CountryCode != null && objLocInfo.CountryCode != "")
                {
                    string strCountryCode = "";
                    if (dtCon.Select("code = '" + objLocInfo.CountryCode + "'").Length != 0)
                        strCountryCode = dtCon.Select("code = '" + objLocInfo.CountryCode + "'")[0]["code"].ToString();
                    if (strCountryCode != null && strCountryCode != "")
                    {
                        IsCountryPresent = true;
                        ddlCountry.SelectedValue = strCountryCode;
                    }
                    else
                    {
                        IsCountryPresent = false;
                        Session["CountryNotPresent"] = true;
                        objLocInfo = new LocationInfo();
                        Session["objLocationInfo"] = objLocInfo;
                    }
                }

            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_FETCH_COUNTRIES_FAILURE) + ex.Message.ToString();
                return;
            }
        }

        //Method to fetch the place/location entered by the user
        protected void getLocation()
        {
            try
            {
                int iPlaceID = 0;

                objLocPresenter.getLocationDetails(objLocInfo.searchLocation.ToString(), objLocInfo.CountryCode.ToString(), eProviderName, 0.00, 0.00, objSvcInfo.Culture);
                if (gvLocation.Rows.Count == 1)
                {
                    objLocInfo.placeName = HttpUtility.HtmlDecode(gvLocation.Rows[0].Cells[0].Text.ToString()).ToString();
                    objLocInfo.latitude = Convert.ToDouble(gvLocation.Rows[0].Cells[1].Text.ToString());
                    objLocInfo.longitude = Convert.ToDouble(gvLocation.Rows[0].Cells[2].Text.ToString());
                    //objLocInfo.CountryCode = ddlCountry.SelectedValue.ToString();
                    //objLocInfo.searchLocation = searchbox.Value.ToString();

                    //Set the PlaceId
                    objLocInfo.placeID = int.TryParse(gvLocation.Rows[0].Cells[6].Text.ToString(), out iPlaceID) ? iPlaceID : 0;

                    hdnGridStatus.Value = "hide";
                    LoadStations();
                    if (gvStations.Rows.Count == 0)
                    {
                        HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_NOSTATIONSFOUND);
                        HttpContext.Current.Session["objLocationInfo"] = null;
                    }
                    hdnLocSelected.Value = objLocInfo.latitude + "|" + objLocInfo.longitude + "|" + objLocInfo.placeName;
                }
                else
                {
                    divLocation.Attributes.Add("class", "show");
                    gvStations.CssClass = "tablesorter hide";
                    gvLocation.CssClass = "show";
                    hdnGridStatus.Value = "Show-Location";
                    if (gvLocation.Rows.Count == 0)
                    {
                        Loc_NoMatchFor.Visible = true;
                        lblStnName.Visible = false;
                        lblDistText.Visible = false;
                        lblPointText.Visible = false;
                        HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.GENERIC_DETAILSNOTFOUND);
                        HttpContext.Current.Session["objLocationInfo"] = null;
                        objLocInfo.latitude = 0.0;
                        objLocInfo.longitude = 0.0;
                        objLocInfo.placeName = "";
                        hdnGridStatus.Value = "hide";
                        hNearByPoint.CssClass = "hide";
                        hdnLatLng.Value = objLocInfo.latitude + "|" + objLocInfo.longitude + "|" + objLocInfo.placeName + "|" + "#LocList";
                        clearDataPoint();
                    }
                    else
                    {
                        Loc_NoMatchFor.Visible = false;
                    }

                    //Clear the no stations found message
                    //check if the Erro message contains "no stations" message
                    //if so,then clear it
                    if (HttpContext.Current.Session["ErrorMessage"] != null)
                    {
                        if (HttpContext.Current.Session["ErrorMessage"].ToString().Equals(objComUtil.getTransText(Constants.LS_NOSTATIONSFOUND)))
                            HttpContext.Current.Session["ErrorMessage"] = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_FETCH_LOCATION_FAILURE) + ex.Message.ToString();
                return;
            }
            finally
            {
                objLocInfo.searchLocation = "";
            }
        }

        //Sets the station parameters to the DatapointInfo object
        protected void setStationParameters(string strStationName, double dLat, double dLong, int iStationID, int iDegrees, int iTimeZoneOffset, int iDstOn, int iAlt, double dist)
        {
            try
            {
                objDataPointInfo.stationName = strStationName;
                objDataPointInfo.stationLatitude = dLat;
                objDataPointInfo.stationLongitude = dLong;
                objDataPointInfo.DataPoint = iStationID;
                objDataPointInfo.DayOffset = iTimeZoneOffset;
                objDataPointInfo.iDstOn = iDstOn;
                objDataPointInfo.altitude = iAlt;
                objDataPointInfo.distance = Math.Round(dist);
                objLocPresenter.getSunriseOrSunsetTime(DateTime.Now.Date, dLat, dLong);
                objDataPointInfo.SunRise = sunRiseTime.AddHours(Convert.ToDouble(objDataPointInfo.DayOffset + objDataPointInfo.iDstOn));
                objDataPointInfo.SunSet = sunSetTime.AddHours(Convert.ToDouble(objDataPointInfo.DayOffset + objDataPointInfo.iDstOn));
                objDataPointInfo.directionLetter = objComUtil.getTextDirection(iDegrees);

            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_SETPARAMETERS_FALURE) + ex.Message.ToString();
                return;
            }
            finally
            {
                if (Session["AuditData"] != null)
                {
                    IDictionary dict = (IDictionary)Session["AuditData"];
                    dict["weatherDatasource"] = objDataPointInfo.NearbyPointSettings.DataSource;
                    if (objDataPointInfo.stationLatitude == 0)
                        dict["weatherLat"] = DBNull.Value;
                    else
                        dict["weatherLat"] = objDataPointInfo.stationLatitude;

                    if (objDataPointInfo.stationLongitude == 0)
                        dict["weatherLong"] = DBNull.Value;
                    else
                        dict["weatherLong"] = objDataPointInfo.stationLongitude;
                    Session["AuditData"] = dict;

                }
            }
        }

        //Sets Nearby station attributes
        protected void setNeabyStationAttributes()
        {
            try
            {
                NearByPointSettings arNearBydata = objNearByPre.setNearbyStationAttributes();
                if (arNearBydata != null)
                {
                    objDataPointInfo.NearbyPointSettings.MaxAllowedDistance = arNearBydata.MaxAllowedDistance;
                    objDataPointInfo.NearbyPointSettings.MaxAllowedAltitude = arNearBydata.MaxAllowedAltitude;
                    objDataPointInfo.NearbyPointSettings.ResultCount = arNearBydata.ResultCount;
                    objDataPointInfo.NearbyPointSettings.DataSource = arNearBydata.DataSource;
                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_SETSTAIONATTRB_FALURE) + ex.Message.ToString();
                return;
            }
        }

        //Methdo to load the nearby stations based on the location selected
        protected void LoadStations()
        {
            try
            {

                //Check if the country present in the session is available in the populated country list 
                if (IsCountryPresent)
                {

                    setNeabyStationAttributes();
                    hdnLatLng.Value = objLocInfo.latitude + "|" + objLocInfo.longitude + "|" + objLocInfo.placeName + "| #StnList";
                    string strStation = objDataPointInfo.stationName;
                    objNearByPre.getNearbyPoints(objLocInfo.latitude, objLocInfo.longitude);
                    if (gvStations.Rows.Count != 0)
                    {
                        setColumnText();
                        DataRow dr = null;
                        if (strStation != null && strStation != "")
                            dr = dtNearByPoints.Select("name = '" + strStation.Replace("'", "''") + "'").Length == 0 ? null : dtNearByPoints.Select("name = '" + strStation.Replace("'", "''") + "'")[0];
                        double lat;
                        double lng;
                        double dist;
                        int bearingdeg;
                        int DpId;
                        int TZone;
                        int Dst;
                        int Alt;
                        string lattranslate;
                        string longtranslate;
                        string lblStnNameInfo;

                        if (dr != null)
                        {
                            //IM01848085:New Agricast - Agriinfo - "location" name not translatable - Start
                            //lblStnName.InnerText = lblStnName.InnerText = HttpUtility.HtmlDecode(dr["Name"].ToString());
                            lattranslate = objSvcPre.getTranslatedText("Lat", objSvcInfo.Culture);
                            longtranslate = objSvcPre.getTranslatedText("Long", objSvcInfo.Culture);
                            lblStnNameInfo = HttpUtility.HtmlDecode(dr["Name"].ToString());
                            lblStnNameInfo = lblStnNameInfo.Replace("Lat", lattranslate);
                            lblStnName.InnerText = lblStnNameInfo.Replace("Long", longtranslate);
                            //IM01848085:New Agricast - Agriinfo - "location" name not translatable - End
                            
                            lat = double.TryParse(dr["Latitude"].ToString(), out lat) ? lat : 0.0;
                            lng = double.TryParse(dr["Longitude"].ToString(), out lng) ? lng : 0.0;
                            bearingdeg = Int32.TryParse(dr["BearingDegrees"].ToString(), out bearingdeg) ? bearingdeg : 0;
                            dist = double.TryParse(dr["DistanceKm"].ToString(), out dist) ? dist : 0;
                            DpId = Convert.ToInt32((dr["DataPointID"].ToString() == "" ? "0" : dr["DataPointID"].ToString()));
                            TZone = Convert.ToInt32(dr["TimezoneOffset"].ToString() == "" ? "0" : dr["TimezoneOffset"].ToString());
                            Dst = Convert.ToInt32(dr["DstOn"].ToString() == "" ? "0" : dr["DSTon"].ToString());
                            Alt = Convert.ToInt32(dr["Altitude"].ToString() == "" ? "0" : dr["Altitude"].ToString());
                            //adding provider
                            Session["ForecastProvider"] = HttpUtility.HtmlDecode(dr["ProviderName"].ToString());
                            lblDistText.InnerHtml = getDistLabelText(Math.Round(dist).ToString(), objComUtil.getTextDirection(bearingdeg), dr["Altitude"].ToString());
                            lblPointText.Visible = true;
                            lblStnName.Visible = true;
                            lblDistText.Visible = true;
                            Loc_NoMatchFor.Visible = false;


                            setStationParameters(dr["Name"].ToString(), lat, lng, DpId, bearingdeg, TZone, Dst, Alt, dist);
                        }
                        else
                        {
                            //adding provider
                            Session["ForecastProvider"] = HttpUtility.HtmlDecode(dtNearByPoints.Rows[0]["ProviderName"].ToString());
                            lat = double.TryParse(gvStations.Rows[0].Cells[4].Text.ToString(), out lat) ? lat : 0.0;
                            lng = double.TryParse(gvStations.Rows[0].Cells[3].Text.ToString(), out lng) ? lng : 0.0;
                            bearingdeg = Int32.TryParse(gvStations.Rows[0].Cells[8].Text.ToString(), out bearingdeg) ? bearingdeg : 0;
                            dist = double.TryParse(gvStations.Rows[0].Cells[1].Text.ToString(), out dist) ? dist : 0;

                            //IM01848085:New Agricast - Agriinfo - "location" name not translatable - Start
                            //lblStnName.InnerText = HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[0].Text);
                            lattranslate = objSvcPre.getTranslatedText("Lat", objSvcInfo.Culture);
                            longtranslate = objSvcPre.getTranslatedText("Long", objSvcInfo.Culture);
                            lblStnNameInfo = HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[0].Text);
                            lblStnNameInfo = lblStnNameInfo.Replace("Lat", lattranslate);
                            lblStnName.InnerText = lblStnNameInfo.Replace("Long", longtranslate);
                            //IM01848085:New Agricast - Agriinfo - "location" name not translatable - End

                            lblDistText.InnerHtml = getDistLabelText(Math.Round(dist).ToString(), objComUtil.getTextDirection(bearingdeg), gvStations.Rows[0].Cells[2].Text);
                            lblPointText.Visible = true;
                            lblStnName.Visible = true;
                            lblDistText.Visible = true;
                            Loc_NoMatchFor.Visible = false;
                            setStationParameters(gvStations.Rows[0].Cells[0].Text, lat, lng, Convert.ToInt32(gvStations.Rows[0].Cells[7].Text ?? "0"), bearingdeg, Convert.ToInt32(HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[9].Text).Trim() == "" ? "0" : HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[9].Text)),
                            Convert.ToInt32(HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[10].Text).Trim() == "" ? "0" : HttpUtility.HtmlDecode(gvStations.Rows[0].Cells[10].Text)), Convert.ToInt32(gvStations.Rows[0].Cells[2].Text ?? "0"), dist);
                        }

                        if (hdnGridStatus.Value != "Show-Location" && hdnGridStatus.Value != "hide")
                        {
                            hdnGridStatus.Value = "Show-Station";
                            gvStations.CssClass = "tablesorter show";
                        }
                        if (hdnGridStatus.Value != "Show-Location")
                        {
                            hNearByPoint.CssClass = "show";
                        }

                        //check if the Erro message contains "no stations" message
                        //if so,then clear it
                        if (HttpContext.Current.Session["ErrorMessage"] != null)
                        {
                            if (HttpContext.Current.Session["ErrorMessage"].ToString().Equals(objComUtil.getTransText(Constants.LS_NOSTATIONSFOUND)))
                                HttpContext.Current.Session["ErrorMessage"] = string.Empty;
                        }
                    }
                    else
                    {
                        if (searchbox.Value.Length != 0)
                            //if(objLocInfo.searchLocation.Length!=0)
                            HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_NOSTATIONSFOUND);

                        Loc_NoMatchFor.Visible = true;
                        lblStnName.Visible = false;
                        lblDistText.Visible = false;
                        lblPointText.Visible = false;
                        hNearByPoint.CssClass = "hide";
                        clearDataPoint();
                    }
                }

            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_STATIONLOAD_FAILURE) + ex.Message.ToString();
                return;
            }
            finally
            {
                if (Session["QueryStringSearch"] == null || bool.Parse(Session["QueryStringSearch"].ToString()) != true)
                    objLocInfo.searchLocation = "";
                else
                    Session["QueryStringSearch"] = false;
            }
        }

        //Method to clear datapoint object when no stations are returned.
        protected void clearDataPoint()
        {
            objDataPointInfo.stationLatitude = 0.0;
            objDataPointInfo.stationLongitude = 0.0;
            objDataPointInfo.stationName = "";
            Session["objDataPointInfo"] = objDataPointInfo;


        }
        //To set the text of the Distance label on selecting a station from the nearby stations list
        protected string getDistLabelText(string strDist, string strDir, string strElevation)
        {
            objLocPresenter.getTranslatedText(Constants.DISTANCE_FROM, objSvcInfo.Culture);
            string strText = strTranslatedText;
            int distance = Int32.Parse(strDist);


            strDist = distance < 10 ? "0" + distance.ToString() : distance.ToString();
            //strDist = distance.ToString();

            strText = strText.Replace("{Dist}", strDist);
            strText = strText.Replace("{Dir}", strDir);
            strText = strText.Replace("{CityName}", HttpUtility.HtmlDecode(objLocInfo.placeName));
            strText = strText.Replace("{Elevation}", strElevation);
            return strText;
        }

        //This method will be called on page load to populate the page with the data availabel in the seesion or cookies
        protected void prefillpage()
        {
            try
            {

                LoadStations();
                hdnLocSelected.Value = objLocInfo.latitude + "|" + objLocInfo.longitude + "|" + objLocInfo.placeName;
                gvStations.Visible = true;
                gvStations.CssClass = "tablesorter show";
                gvLocation.CssClass = "hide";
                divLocation.Attributes.Add("class", "hide");
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_LOAD_FAILURE) + ex.Message.ToString();
                return;
            }
        }

        //Method the sort the gridview data
        private void SortGridView(Expression sortExpression, string direction, string strGridName)
        {
            try
            {
                if (strGridName == "Stations")
                {
                    objNearByPre.getNearbyPoints(objLocInfo.latitude, objLocInfo.longitude);
                    DataTable dt = dtNearByPoints;
                    DataView dv = new DataView(dt);
                    dv.Sort = sortExpression + " " + direction;
                    gvStations.DataSource = dv;
                    gvStations.DataBind();
                    setColumnText();
                    divLocation.Attributes["class"] = "show";
                    hdnGridStatus.Value = "Show-Station";
                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_SORT_GRIDS) + ex.Message.ToString();
                return;
            }
        }

        //This method displays the nearby stations data in required format
        protected void setColumnText()
        {
            try
            {
                for (int i = 0; i < gvStations.Rows.Count; i++)
                {
                    objLocPresenter.getTranslatedText(Constants.DISTANCE_FROM, objSvcInfo.Culture);
                    string strText = strTranslatedText;

                    //to change text based on units
                    // to be changed once transtag gets added
                    if (objSvcInfo.Unit.ToLower().Equals("imperial", StringComparison.CurrentCultureIgnoreCase))
                    {
                        strText = Constants.ResDistFromImp;
                    }
                    string strDist = Math.Round(Convert.ToDouble(gvStations.Rows[i].Cells[1].Text)).ToString();
                    int distance = Int32.Parse(strDist);
                    string strElevation = gvStations.Rows[i].Cells[2].Text;

                    //convert Distance and Elevation to Miles and Feet respectively for 
                    //culture en-Us and 
                    //if (objSvcInfo.Culture.ToLower().Equals("en-us", StringComparison.CurrentCultureIgnoreCase))
                    //{
                    //    strDist = objComUtil.ConvertValueMetrics(strDist, "km-mi");
                    //    strElevation = objComUtil.ConvertValueMetrics(strElevation, "m-ft");
                    //}
                    if (objSvcInfo.Unit.ToLower().Equals("imperial", StringComparison.CurrentCultureIgnoreCase))
                    {
                        strDist = objComUtil.ConvertValueMetrics(strDist, "km-mi");
                        strElevation = objComUtil.ConvertValueMetrics(strElevation, "m-ft");
                    }
                    else
                    {
                        strDist = distance < 10 ? "0" + distance.ToString() : distance.ToString();
                        // strDist = distance.ToString();
                    }
                    //distance < 10 ? "0" + distance.ToString() : distance.ToString());
                    strText = strText.Replace("{Dist}", strDist);
                    //strText = strText.Replace("{Dist}", distance.ToString());
                    strText = strText.Replace("{Dir}", objComUtil.getTextDirection(Convert.ToInt32(gvStations.Rows[i].Cells[8].Text)));
                    strText = strText.Replace("{CityName}", HttpUtility.HtmlDecode(objLocInfo.placeName));
                    strText = strText.Replace("{Elevation}", strElevation);// gvStations.Rows[i].Cells[2].Text);
                    gvStations.Rows[i].Cells[5].Text = strText;
                    if (gvStations.Rows[i].Cells[0].Text == objDataPointInfo.stationName)
                    {
                        lblDistText.InnerHtml = gvStations.Rows[i].Cells[5].Text;


                    }
                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_SETCOLUMNTEXT_FAILURE) + ex.Message.ToString();
                return;
            }
        }

        #endregion

        public void Grid(DataSet ds, string dsName)
        {
            if (dsName == "Locations")
            {
                gvLocation.DataSource = ds;
                gvLocation.DataBind();
                gvStations.CssClass = "tablesorter hide";
                gvLocation.CssClass = "show";
            }
            else if (dsName == "Stations")
            {
                gvStations.DataSource = ds;
                gvStations.DataBind();
                gvStations.CssClass = "tablesorter show";
                gvLocation.CssClass = "hide";
            }
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
            }
        }

        public DataTable dtNearByPoints
        {
            get
            {
                return dtNearbystatn;
            }
            set
            {
                dtNearbystatn = value;
                gvStations.DataSource = dtNearbystatn;
                gvStations.DataBind();
                gvStations.CssClass = "tablesorter show";
                gvLocation.CssClass = "hide";

            }
        }

        public DataTable dtCountry
        {
            get
            {
                return dtCon;
            }
            set
            {
                dtCon = value;
                /*IM01277740 - New Agricast - add "Select" to country dropdown in both application and embed js - begin */
                // add the "select" value only if more than 1 country is specifed
                if (dtCon.Rows.Count != 1)
                {
                    DataRow drCon = dtCon.NewRow();
                    drCon["Value"] = "--Select--";
                    drCon["code"] = "XX";
                    dtCon.Rows.InsertAt(drCon, 0);

                }
                /*IM01277740 - New Agricast - add "Select" to country dropdown in both application and embed js - end */
                ddlCountry.Items.Clear();
                ddlCountry.DataSource = dtCon;
                ddlCountry.DataTextField = "Value";
                ddlCountry.DataValueField = "code";
                ddlCountry.DataBind();
                if (dtCon.Rows.Count == 1)
                    ddlCountry.Attributes.Add("disabled", "true");
                Session["CountryCode"] = ddlCountry.SelectedValue;
            }
        }

        public List<Location> LocationList
        {
            get
            {
                return locationList;
            }
            set
            {
                locationList = value;
                if (locationList != null && locationList.Count != 0)
                {
                    LocationSearchSource eProvider = (LocationSearchSource)Session["Provider"];
                    objLocInfo.Provider = eProvider;
                    Session["objLocationInfo"] = objLocInfo;
                    
                    gvLocation.DataSource = LocationList;
                    gvLocation.DataBind();
                }
                else
                {
                    gvLocation.DataSource = null;
                    gvLocation.DataBind();
                }

                //If Grid Location has no data, display error message
                if (gvLocation.Rows.Count == 0)
                {
                    Loc_NoMatchFor.Visible = true;
                }
            }
        }

        public string strTranslatedText
        {
            get;
            set;
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

        public string getTranslatedText(string strLabelName, string strLangID)
        {
            throw new NotImplementedException();
        }

        public List<Location> getLocationDetails(string strPlaceName, string strCountry, string strCultureCode, LocationSearchSource eProvider)
        {
            throw new NotImplementedException();
        }
        public string Version()
        {
            throw new NotImplementedException();
        }
        public string strCultureCode
        {
            get;
            set;
        }
        public bool showMap
        {
            get;
            set;
        }
        #region ILocSearch Members


        //DataTable ILocSearch.getCountry()
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        public DataSet getNearByStations(double dlat, double dlong, int intMaxAllowedDist, int intMaxAllowedAlt, int intResultCount)
        {
            throw new NotImplementedException();
        }

        public DateTime sunRiseTime
        {
            get;
            set;
        }

        public DateTime sunSetTime
        {
            get;
            set;
        }

        public int iTimeZoneOffset
        {
            get;
            set;
        }

        public int iDstOn
        {
            get;
            set;
        }
    }
}