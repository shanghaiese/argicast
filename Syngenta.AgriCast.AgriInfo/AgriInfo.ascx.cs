using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.Common;
using Syngenta.AgriCast.Common.DTO;
using System.Data;
using System.Web.UI.HtmlControls;
using Syngenta.AgriCast.AgriInfo.View;
using Syngenta.AgriCast.AgriInfo.Presenter;
using Syngenta.AgriCast.AgriInfo.DTO;
using System.Collections;
using Syngenta.AgriCast.AgriInfo;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Charting.Presenter;
using Syngenta.AgriCast.Charting;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Configuration;
using Syngenta.AgriCast.ExceptionLogger;
using System.Text.RegularExpressions;
using System.Web.SessionState;
using System.Globalization;
using Syngenta.AgriCast.Common.Service;
using System.Net;
using System.Xml;
using System.Text;


namespace Syngenta.AgriCast.AgriInfo
{
    public partial class AgriInfo : System.Web.UI.UserControl, IAgriInfo
    {
        AgriInfoPresenter objAgriPresenter;
        ServiceInfo objSvcInfo;
        Controls common = new Controls();
        LinkButton lbMore;
        string checkedSeries;
        LocationInfo objLocInfo;
        DataPointInfo objDataPointInfo;
        AdvancedOptionInfo objAdvInfo;
        SeriesInfo objSeriesInfo;
        public const string STARTDATE = "1991-01-01";
        ServicePresenter objSvcPre = new ServicePresenter();
        DataTable dtData;
        Dictionary<string, string> objAgriInfo = new Dictionary<string, string>();
        bool IsGDD = false;
        bool AggregationValid = false;
        CultureInfo VariantCulture;
        CommonUtil objComUtil = new CommonUtil();
        bool valid = true;
        CultureInfo NewCulture;
        CultureInfo OldCulture;
        DateTimeFormatInfo oldinfo;
        DateTimeFormatInfo newinfo;
        string url = string.Empty;
        string root = string.Empty;

        #region Constants
        const string BOXMINUS = "~/Images/boxminus.gif";
        const string BOXPLUS = "~/Images/boxplus.gif";
        const string LOCATION_TABLE_NAME = "Location Details";
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session == null || Session["serviceInfo"] == null)
                {

                    objSvcInfo = new ServiceInfo();
                }
                else
                {
                    objSvcInfo = (ServiceInfo)Session["serviceInfo"];
                }
                objAgriPresenter = new AgriInfoPresenter(this, Name);

                if (HttpContext.Current != null)
                {
                    objLocInfo = LocationInfo.getLocationInfoObj;

                }
                else
                {
                    objLocInfo = new LocationInfo();
                }

                if (Session["CurrentCulture"] != null)
                {

                    NewCulture = new CultureInfo(Session["CurrentCulture"].ToString());

                }
                else
                {
                    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - Start */
                    if (string.IsNullOrWhiteSpace(objSvcInfo.Culture))
                        NewCulture = new CultureInfo("en-GB");
                    else
                        NewCulture = new CultureInfo(objSvcInfo.Culture);
                    // NewCulture = new CultureInfo("en-GB");
                    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - End */

                }
                if (Session["PreviousCulture"] != null)
                {

                    OldCulture = new CultureInfo(Session["PreviousCulture"].ToString());

                }
                else
                {
                    OldCulture = new CultureInfo("en-GB");
                }
                //Set the datetime formats

                //Add for when change Pub, the data formate will not correct - 20140615 - Start
                url = HttpContext.Current.Request.Url.ToString().ToLower();
                url = url.Remove(url.LastIndexOf(".aspx"));
                root = url.Remove(url.LastIndexOf('/')).Substring(url.IndexOf("pub/") + 4);
            
                HttpCookie agriInfoCookiePub = Request.Cookies["AgriInfo_HistoryData_ECMWF"];

                if (agriInfoCookiePub != null && Server.UrlDecode(agriInfoCookiePub.Value).ToString() != "")
                {
                    string[] cookValues = Server.UrlDecode(agriInfoCookiePub.Value).Split('#');
                    if (cookValues[8] != root)
                    {
                        if (string.IsNullOrWhiteSpace(objSvcInfo.Culture))
                        {
                            NewCulture = new CultureInfo("en-GB");
                        }
                        else
                        {
                            NewCulture = new CultureInfo(objSvcInfo.Culture);
                        }
                        OldCulture = new CultureInfo("en-GB");
                        Session["PreviousCulture"] = null;
                        Session["CurrentCulture"] = null;
                    }
                }
                //Add for when change Pub, the data formate will not correct - 20140615 - END

                oldinfo = OldCulture.DateTimeFormat;
                newinfo = NewCulture.DateTimeFormat;

                if (!ExcelFlag)
                {
                    //if (!IsPostBack) 
                    //{

                    //    PopulateAltitude();
                    //}
                    readDate();

                    setControlText();
                    PopulateDropDowns();
                    loadSeries();
                    HttpCookie agriInfoCookie = Request.Cookies["AgriInfo_" + objSvcInfo.Module];

                    if (agriInfoCookie != null && agriInfoCookie.Value.ToString() != "")
                    {
                        DeserializeAgriInfoCookieString(Server.UrlDecode(agriInfoCookie.Value));

                    }
                }

                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                if (objSvcInfo.Moss == "true" && objSvcInfo.Module.ToLower() == "watermodel")
                {
                    if (Session["IAgriInfo"] != null)
                    {
                        objAgriInfo = (Dictionary<string, string>)Session["IAgriInfo"];
                    }
                    hdnSeries.Value = "gdd_cb_WaterModel";
                    hdnSeries_ValueChanged("gdd_cb_WaterModel");
                    hApply_Click(sender, e);
                }

                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.GENERIC_ERRORONPAGE) + ":" + ex.Message.ToString();
            }
        }
        string getEndDate()
        {
            string endDate;
            string date = ShortMonthDayPattern(oldinfo, newinfo, txtstartDate.Value);
            try
            {
                DateTime x;
                switch (ddlDuration.SelectedValue.ToString())
                {
                    case "1D":
                        //EndDate = DateTime.Parse(txtstartDate.Value.ToString()).AddDays(1).ToString();
                        //Added by Rahul
                        // endDate = DateTime.Parse(GetDateInCorrectFormat(txtstartDate.Value.ToString(), true)).ToString();
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - start
                        //endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString() : date);
                        endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString() : (DateTime.TryParse(date.Substring(0, date.Length - 1), out x)) ? Convert.ToDateTime(x).ToShortDateString() : date);
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - end
                        break;
                    case "5D":
                        //endDate = DateTime.Parse(GetDateInCorrectFormat(txtstartDate.Value.ToString(), true)).AddDays(4).ToString();
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - start
                        //endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(4).ToShortDateString() : date);
                        endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(4).ToShortDateString() : (DateTime.TryParse(date.Substring(0, date.Length - 1), out x))?Convert.ToDateTime(x).AddDays(4).ToShortDateString():date);
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - end
                        break;
                    case "10D":
                        //endDate = DateTime.Parse(GetDateInCorrectFormat(txtstartDate.Value.ToString(), true)).AddDays(9).ToString();
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - start
                        //endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(9).ToShortDateString() : date);
                        endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(9).ToShortDateString() : (DateTime.TryParse(date.Substring(0, date.Length - 1), out x)) ? Convert.ToDateTime(x).AddDays(9).ToShortDateString() : date);
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - end
                        break;
                    case "1W":
                        //endDate = DateTime.Parse(GetDateInCorrectFormat(txtstartDate.Value.ToString(), true)).AddDays(6).ToString();
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - start
                        //endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(6).ToShortDateString() : date);
                        endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(6).ToShortDateString() : (DateTime.TryParse(date.Substring(0, date.Length - 1), out x)) ? Convert.ToDateTime(x).AddDays(6).ToShortDateString() : date);
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - end
                        break;
                    case "2W":
                        //endDate = DateTime.Parse(GetDateInCorrectFormat(txtstartDate.Value.ToString(), true)).AddDays(13).ToString();
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - start
                        //endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(13).ToShortDateString() : date);
                        endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(13).ToShortDateString() : (DateTime.TryParse(date.Substring(0, date.Length - 1), out x)) ? Convert.ToDateTime(x).AddDays(13).ToShortDateString() : date);
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - end
                        break;
                    case "1M":
                        //endDate = DateTime.Parse(GetDateInCorrectFormat(txtstartDate.Value.ToString(), true)).AddDays(29).ToString();
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - start
                        //endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(29).ToShortDateString() : date);
                        endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(29).ToShortDateString() : (DateTime.TryParse(date.Substring(0, date.Length - 1), out x)) ? Convert.ToDateTime(x).AddDays(29).ToShortDateString() : date);
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - end
                        break;
                    case "3M":
                        //endDate = DateTime.Parse(GetDateInCorrectFormat(txtstartDate.Value.ToString(), true)).AddMonths(3).ToString();
                        //endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(90).ToShortDateString() : date);
                        endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(90).ToShortDateString() : (DateTime.TryParse(date.Substring(0, date.Length - 1), out x)) ? Convert.ToDateTime(x).AddDays(90).ToShortDateString() : date);
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - end
                        break;
                    case "6M":
                        //endDate = DateTime.Parse(GetDateInCorrectFormat(txtstartDate.Value.ToString(), true)).AddMonths(6).ToString();
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - start
                        //endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(180).ToShortDateString() : date);
                        endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddDays(180).ToShortDateString() : (DateTime.TryParse(date.Substring(0, date.Length - 1), out x)) ? Convert.ToDateTime(x).AddDays(180).ToShortDateString() : date);
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - end
                        break;
                    case "1Y":
                        //endDate = DateTime.Parse(GetDateInCorrectFormat(txtstartDate.Value.ToString(), true)).AddYears(1).ToString();
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - start
                        //endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddYears(1).ToShortDateString() : date);
                        endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).AddYears(1).ToShortDateString() : (DateTime.TryParse(date.Substring(0, date.Length - 1), out x)) ? Convert.ToDateTime(x).AddYears(1).ToShortDateString() : date);
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - end
                        break;
                    case "to_date":
                        date = ShortMonthDayPattern(oldinfo, newinfo, txtEndDate.Value);
                        //endDate = txtEndDate.Value.ToString();
                        endDate = (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString() : date);
                        break;
                    case "Last_date":
                    default:
                        endDate = DateTime.Today.AddDays(-2).ToString();
                        break;
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.GENERIC_ERRORONPAGE) + ":" + ex.Message.ToString();
                throw ex;
            }
            return endDate;
        }
        void setValues()
        {
            try
            {
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                if (objSvcInfo.Moss == "true" && objSvcInfo.Module.ToLower() == "watermodel" && Session["IAgriInfo"] != null)
                {
                    objAgriInfo = (Dictionary<string, string>)Session["IAgriInfo"];
                }
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                else
                {
                    if (txtstartDate.Value != "")
                    {
                        Dictionary<string, string> objAgriInfo = new Dictionary<string, string>();
                        //  objAgriInfo.Add("startDate", txtstartDate.Value.ToString());
                        string date = ShortMonthDayPattern(oldinfo, newinfo, txtstartDate.Value);
                        objAgriInfo.Add("startDate", (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString() : date).ToString());
                        string EndDate = getEndDate();
                        objAgriInfo.Add("endDate", EndDate);
                        objAgriInfo.Add("aggregation", ddlAggregate.SelectedValue != null ? ddlAggregate.SelectedValue.ToString() : "");
                        if (txtPlantingDate.Value != string.Empty)
                        {
                            string plntDate = ShortMonthDayPattern(oldinfo, newinfo, txtPlantingDate.Value);
                            objAgriInfo.Add("plantingDate", (valid ? DateTime.ParseExact(plntDate, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString() : plntDate).ToString());
                        }
                        else
                        {
                            objAgriInfo.Add("plantingDate", "");
                        }
                        //objAgriInfo.Add("altitude", txtAltitude.Value.ToString() != "" ? txtAltitude.Value.ToString() : objDataPointInfo.altitude.ToString());

                        /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                        double topSoilWHC = 0.0, subSoilWHC = 0.0;
                        switch (ddlTopSoilType.SelectedValue)
                        {
                            case "Sd": topSoilWHC = 0.07; break;
                            case "Lm": topSoilWHC = 0.15; break;
                            case "Sl": topSoilWHC = 0.14; break;
                            case "Ls": topSoilWHC = 0.11; break;
                            case "Cy": topSoilWHC = 0.18; break;
                            case "So": topSoilWHC = 0.155; break;
                            default: topSoilWHC = 0.0; break;
                        }

                        switch (ddlSubSoilType.SelectedValue)
                        {
                            case "Sd": subSoilWHC = 0.07; break;
                            case "Lm": subSoilWHC = 0.15; break;
                            case "Sl": subSoilWHC = 0.14; break;
                            case "Ls": subSoilWHC = 0.11; break;
                            case "Cy": subSoilWHC = 0.18; break;
                            case "So": subSoilWHC = 0.155; break;
                            default: subSoilWHC = 0.0; break;
                        }
                        objAgriInfo.Add("TopSoilWHC", topSoilWHC.ToString());
                        objAgriInfo.Add("SubSoilWHC", subSoilWHC.ToString());

                        if (string.IsNullOrWhiteSpace(txtTopSoilDepth.Value))
                            objAgriInfo.Add("TopSoilDepth", "0");
                        else
                            objAgriInfo.Add("TopSoilDepth", txtTopSoilDepth.Value);

                        if (string.IsNullOrWhiteSpace(txtSubSoilDepth.Value))
                            objAgriInfo.Add("SubSoilDepth", "0");
                        else
                            objAgriInfo.Add("SubSoilDepth", txtSubSoilDepth.Value);
                        /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/

                        Session["IAgriInfo"] = objAgriInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.GENERIC_ERRORONPAGE) + ":" + ex.Message.ToString();
                throw ex;
            }
        }
        void setControlText()
        {
            try
            {
                lblDateScale.InnerText = TranslatedText(Constants.DATE_SCALE);
                lblStartDate.InnerText = TranslatedText(Constants.START_DATE);
                lblEndDate.InnerText = TranslatedText(Constants.END_DATE);
                lblDuration.InnerText = TranslatedText(Constants.DURATION);
                lblAggregate.InnerText = TranslatedText(Constants.AGGREGATION);
                lblCropInformation.InnerText = TranslatedText(Constants.CROP_INFORMATION);
                lblPlantingdate.InnerText = TranslatedText(Constants.PLANTING_DATE);
                //lblTopSoil.InnerText = TranslatedText("tip_wm_topsoil");
                //lblDepth.InnerText = TranslatedText("wm_depth");
                //lblsubSoil.InnerText = TranslatedText("tip_wm_subsoil");
                //lblDepth.InnerText = TranslatedText("wm_depth");
                //lblWHC.InnerText = TranslatedText("WHC");
                //lblInitialBal.InnerText = TranslatedText("wm_initialbalance");
                lblSeries.InnerText = TranslatedText(Constants.SERIES);
                hApply.Text = TranslatedText(Constants.APPLY_TO_CHART);
                hCancel.Text = TranslatedText(Constants.CLEAR_SELECTION);
                lblAltitude.InnerText = TranslatedText(Constants.ALTITUDE);

                //  lblMasl.InnerText = TranslatedText("meteraslabbr");
                /*IM01258137 - New Agricast - Translation - can't translate "{.More}" - BEGIN */
                spnClickCalenderToolTip.InnerText = TranslatedText(Constants.CALENDAR_TOOLTIP);
                /*IM01258137 - New Agricast - Translation - can't translate "{.More}" - END */
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                lblSoilInformation.InnerText = TranslatedText(Constants.SOIL_INFORMATION);
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/


                /*IM01258137 - New Agricast - Translation - can't translate "{.More}" - BEGIN */
                /*if (objSvcInfo.Unit.Equals("imperial", StringComparison.InvariantCultureIgnoreCase))
                {

                    //lblMasl.InnerText = Constants.FEET_ASL;
                    lblMasl.InnerText = TranslatedText(Constants.FEET_ASL);
                }
                else if (objSvcInfo.Unit.Equals("metric", StringComparison.InvariantCultureIgnoreCase))
                {
                    //lblMasl.InnerText = Constants.M_ASL;
                    lblMasl.InnerText = TranslatedText(Constants.M_ASL);
                }
                /*IM01258137 - New Agricast - Translation - can't translate "{.More}" - END */

            }

            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.TRANSLATION_FAILURE) + ex.Message.ToString();
            }

        }
        string TranslatedText(string label)
        {
            if (objSvcInfo == null)
                objSvcInfo = (ServiceInfo)Session["serviceInfo"];

            objAgriPresenter.getTranslatedText(label, objSvcInfo.Culture);
            return strTranslatedText;
        }
        void PopulateAltitude()
        {
            if (HttpContext.Current != null)
            {
                objLocInfo = LocationInfo.getLocationInfoObj;
            }
            else
            {
                objLocInfo = new LocationInfo();
            }

            //Fill the altitude textbox with value from datapoint object
            if (objLocInfo != null)
            {
                txtAltitude.Value = GetElevation(objLocInfo.latitude, objLocInfo.longitude);
            }
        }

        public string GetElevation(double latitude, double longitude)
        {

            DataSet dsLocation = new DataSet();
            string proxyName = ConfigurationManager.AppSettings["proxyName"];
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            XmlDocument xdoc = new XmlDocument();
            string username = ConfigurationManager.AppSettings["UserName"];
            string domain = ConfigurationManager.AppSettings["Domain"];
            string password = ConfigurationManager.AppSettings["Password"];
            webClient.Credentials = new System.Net.NetworkCredential(username, password, domain);
            webClient.Headers.Add(HttpRequestHeader.UserAgent, "test");
            WebProxy proxy = new WebProxy(proxyName, true);
            proxy.Credentials = new System.Net.NetworkCredential(username, password, domain);
            webClient.Proxy = proxy;
            string strReturn;

            strReturn = webClient.DownloadString("http://api.geonames.org/gtopo30?token=AISGeonames&username=lostinswiss&lat=" + latitude + "&lng=" + longitude);

            if (strReturn == "-9999\r\n")
            {
                strReturn = "NULL\r\n";
            }
            return strReturn;
        }


        void PopulateDropDowns()
        {
            ddlDuration.Items.Clear();
            ddlAggregate.Items.Clear();

            if (ddlDuration.Items.Count == 0)
            {
                ddlDuration.Items.Add(newItem("1D", "1 Day"));
                ddlDuration.Items.Add(newItem("5D", "5 Days"));
                ddlDuration.Items.Add(newItem("1W", "1 Week"));
                ddlDuration.Items.Add(newItem("10D", "10 Days"));
                ddlDuration.Items.Add(newItem("2W", "2 Weeks"));
                ddlDuration.Items.Add(newItem("1M", "1 Month"));
                ddlDuration.Items.Add(newItem("3M", "3 Months"));
                ddlDuration.Items.Add(newItem("6M", "6 Months"));
                ddlDuration.Items.Add(newItem("1Y", "1 Year"));
                ddlDuration.Items.Add(newItem("to_date", "To specific date"));
                ddlDuration.Items.Add(newItem("last_date", "Last available date"));
                ddlDuration.DataBind();
                ddlDuration.SelectedIndex = 7;

            }


            if (ddlAggregate.Items.Count == 0)
            {

                ddlAggregate.Items.Add(newItem("daily", "daily"));
                ddlAggregate.Items.Add(newItem("weekly", "weekly"));
                if (objSvcInfo.Module != "HistoryData_NexRad")
                ddlAggregate.Items.Add(newItem("decade", "decade"));
                ddlAggregate.Items.Add(newItem("monthly", "monthly"));
                ddlAggregate.DataBind();
                ddlAggregate.SelectedIndex = 1;

                /* IM01277495 - issue reported by alec - BEGIN*/
                string strHistoryFcstTransTag = objAgriPresenter.getServicePageDetails(objSvcInfo.Module);
                if (!string.IsNullOrEmpty(strHistoryFcstTransTag) && strHistoryFcstTransTag.Equals("All_historicdata_Forecast"))
                {
                    ddlAggregate.Items.FindByValue("weekly").Enabled = false;
                    ddlAggregate.Items.FindByValue("decade").Enabled = false;
                    ddlAggregate.Items.FindByValue("monthly").Enabled = false;
                    ddlAggregate.SelectedIndex = 0;
                }
                /* IM01277495 - issue reported by alec - END*/
            }



            /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
            ddlTopSoilType.Items.Clear();
            if (ddlTopSoilType.Items.Count == 0)
            {
                ddlTopSoilType.Items.Add(newItem("Sd", "Sand"));
                ddlTopSoilType.Items.Add(newItem("Lm", "Loam"));
                ddlTopSoilType.Items.Add(newItem("Sl", "Sandy loam"));
                ddlTopSoilType.Items.Add(newItem("Ls", "Loamy sand"));
                ddlTopSoilType.Items.Add(newItem("Cy", "Clay"));
                ddlTopSoilType.Items.Add(newItem("So", "Oraganic"));
                ddlTopSoilType.DataBind();
            }

            ddlSubSoilType.Items.Clear();
            if (ddlSubSoilType.Items.Count == 0)
            {
                ddlSubSoilType.Items.Add(newItem("Sd", "Sand"));
                ddlSubSoilType.Items.Add(newItem("Lm", "Loam"));
                ddlSubSoilType.Items.Add(newItem("Sl", "Sandy loam"));
                ddlSubSoilType.Items.Add(newItem("Ls", "Loamy sand"));
                ddlSubSoilType.Items.Add(newItem("Cy", "Clay"));
                ddlSubSoilType.Items.Add(newItem("So", "Oraganic"));
                ddlSubSoilType.DataBind();
            }

            /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
        }

        ListItem newItem(string Value, string Text)
        {
            ListItem item = new ListItem();
            item.Text = TranslatedText(Text);
            item.Value = Value;
            return item;
        }
        void readDate()
        {
            objAgriPresenter.readDate();
            if (txtstartDate.Value == "")
            {
                //start of changes - 2013/03/12 
                //txtstartDate.Value = StartDate.ToShortDateString(); 
                txtstartDate.Value = ShortMonthDayPattern(null, newinfo, StartDate.ToShortDateString());
                //End of changes - 2013/03/12

                //Add for IM01848073:New Agricast - Agriinfo - error in HU - start
                //Default value 1st July will be changed to 7th Jan, it is not correct 
                if (newinfo.LongDatePattern.ToString() == "yyyy. MMMM d.")
                {
                    string[] b = txtstartDate.Value.ToString().Split('.');
                    txtstartDate.Value = b[1] + "." + b[0] + "." + b[2];
                }
                //Add for IM01848073:New Agricast - Agriinfo - error in HU - start               
            }

        }
        void loadSeries()
        {
            Popup objPopUp = new Popup();
            try
            {
                objAgriPresenter.getSessionDetails();
                string strPubname = ar[0].ToString();
                string strModule = ar[1].ToString();
                string strCulture = ar[2].ToString();
                objAgriPresenter.loadSeries();

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();

                    //img.ImageUrl = BOXPLUS;
                    img.ID = "imgSeriesNode" + i;
                    img.ClientIDMode = ClientIDMode.Static;
                    img.CssClass = "buttonTree";
                    //img.Attributes.Add("src", BOXPLUS);

                    Label lbl = new Label();
                    lbl.Text = TranslatedText(ds.Tables[i].TableName);
                    lbl.CssClass = "labelTreeH1";
                    lbl.ID = "lblNode" + i;
                    Control cntrlPH = FindControl("ph" + i);
                    cntrlPH.Controls.Add(img);
                    img.ImageUrl = BOXPLUS;
                    img.Attributes.Add("alt", "+");
                    cntrlPH.Controls.Add(lbl);

                    for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                    {

                        HtmlGenericControl lt = new HtmlGenericControl();
                        string liid = "node" + i + "_li_" + ds.Tables[i].Rows[j][0].ToString();
                        lt.ID = liid;
                        lt.TagName = "li";
                        CheckBox cb = new CheckBox();
                        string cbid = "node" + i + "_cb_" + ds.Tables[i].Rows[j][0].ToString();
                        cb.ID = cbid;
                        cb.Text = TranslatedText(ds.Tables[i].Rows[j][1].ToString());
                        cb.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        if (ds.Tables[i].Rows[j]["isChecked"].ToString().ToLower() == "true")
                        {
                            cb.Checked = true;
                            hdnSeries.Value = hdnSeries.Value != "" ? hdnSeries.Value + ";" + cb.ID : cb.ID;
                        }
                        lt.Controls.Add(cb);

                        if (ds.Tables[i].Rows[j]["advancedOption1"].ToString().ToLower() == "true")
                        {
                            lbMore = new LinkButton();
                            lbMore.Text = TranslatedText(".More");
                            lbMore.ID = "node" + i + "_lb_" + ds.Tables[i].Rows[j][0].ToString();

                            //add a new attribute errorText
                            lbMore.Attributes.Add("data-TransErrorText", TranslatedText(Constants.GEN_ADVOPTIONSERROR));
                            //add the transtag of "Advanced option"
                            lbMore.Attributes.Add("data-TransAdvOptionText", TranslatedText(Constants.GEN_ADV_OPTIONTEXT));
                            //string strTransText=
                            //lbMore.Click += new EventHandler(lbMore_Click);
                            lt.Controls.Add(lbMore);
                        }

                        Control cntrl = FindControl("UL" + i);
                        cntrl.Controls.Add(lt);
                        Control cntrlDiv = FindControl("Div" + i);
                        // if (!IsPostBack)
                        {
                            HtmlGenericControl div = new HtmlGenericControl();
                            div = (HtmlGenericControl)cntrlDiv;
                            div.Attributes.Add("class", "hide");
                        }
                    }
                }


                objAgriPresenter.LoadGddSeries();
                if (dtGdd != null && dtGdd.Rows.Count > 0)
                {
                    //if (!IsPostBack)
                    //    divGddSeries.Attributes.Add("class", "hide");
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                    divSoilInfo.Attributes.Add("class", "show");
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                    divCropInfo.Attributes.Add("class", "show");
                    for (int i = 0; i < dtGdd.Rows.Count; i++)
                    {

                        HtmlGenericControl lt = new HtmlGenericControl();
                        string liid = "gdd_li_" + dtGdd.Rows[i][0].ToString();
                        lt.ID = liid;
                        lt.TagName = "li";
                        CheckBox cb = new CheckBox();
                        string cbid = "gdd_cb_" + dtGdd.Rows[i][0].ToString();
                        cb.ID = cbid;
                        cb.Text = TranslatedText(dtGdd.Rows[i]["trnsTag"].ToString());
                        cb.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        lt.Controls.Add(cb);
                        if (dtGdd.Rows[i]["advancedOption1"].ToString().ToLower() == "true")
                        {
                            lbMore = new LinkButton();
                            lbMore.Text = TranslatedText(".More");
                            lbMore.ID = "gdd_lb_" + dtGdd.Rows[i][0].ToString();
                            //add a new attribute errorText
                            lbMore.Attributes.Add("data-TransErrorText", TranslatedText(Constants.GEN_ADVOPTIONSERROR));

                            //add the transtag of "Advanced option"
                            lbMore.Attributes.Add("data-TransAdvOptionText", TranslatedText(Constants.GEN_ADV_OPTIONTEXT));

                            //lbMore.Click += new EventHandler(lbMore_Click);
                            lt.Controls.Add(lbMore);
                        }
                        GddUl.Controls.Add(lt);

                    }
                }
                else
                {
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                    divSoilInfo.Attributes.Add("class", "hide");
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                    divCropInfo.Attributes.Add("class", "hide");
                    divGDD.Attributes.Add("class", "hide");
                }

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.GEN_LOADSERIES_FALURE) + " : " + ex.Message.ToString();
            }
        }
        DataTable ReadSeriesDetails(string[] seriesList)
        {
            DataTable dtSeries = new DataTable();
            dtSeries.Columns.Add("Name");
            dtSeries.Columns.Add("trnsTag");
            dtSeries.Columns.Add("aggregationfunction");
            dtSeries.Columns.Add("markerType");
            dtSeries.Columns.Add("axisPosition");
            dtSeries.Columns.Add("stacked");
            dtSeries.Columns.Add("gallery");
            dtSeries.Columns.Add("color");
            dtSeries.Columns.Add("panel");
            dtSeries.Columns.Add("addlInfo");
            dtSeries.Columns.Add("year");
            dtSeries.Columns.Add("labeltext");
            dtSeries.Columns.Add("pallete");
            dtSeries.Columns.Add("textColor");
            foreach (string SerieName in seriesList)
            {
                DataRow dr = dtSeries.NewRow();
                if (SerieName.StartsWith("node"))
                {
                    //getting the node num to get the table
                    int i = int.Parse((SerieName.Remove(5)).Substring(4));
                    //getting the serie name
                    string serie = SerieName.Substring(9);

                    //Reading the different values form the corresponding datatable
                    var aggFunction = from row in ds.Tables[i].AsEnumerable()
                                      where row.Field<string>("name").Trim() == serie
                                      select new
                                      {
                                          trnsTag = row.Field<string>("trnsTag"),
                                          aggregationFunction = row.Field<string>("aggregationFunction"),
                                          markerType = row.Field<string>("markerType"),
                                          axisPosition = row.Field<string>("axisPosition"),
                                          stacked = row.Field<bool>("stacked"),
                                          gallery = row.Field<string>("gallery"),
                                          color = row.Field<string>("color"),
                                          labeltext = row.Field<string>("labelText")
                                      };
                    if (aggFunction.ToList().Count > 0)
                    {
                        dr["Name"] = serie;
                        dr["trnsTag"] = aggFunction.ToList()[0].trnsTag;
                        dr["aggregationfunction"] = aggFunction.ToList()[0].aggregationFunction;
                        dr["markerType"] = aggFunction.ToList()[0].markerType;
                        dr["axisPosition"] = aggFunction.ToList()[0].axisPosition;
                        dr["stacked"] = aggFunction.ToList()[0].stacked;
                        dr["gallery"] = aggFunction.ToList()[0].gallery;
                        dr["color"] = aggFunction.ToList()[0].color;
                        dr["panel"] = i;
                        dr["addlInfo"] = "";
                        dr["year"] = "";
                        dr["labeltext"] = aggFunction.ToList()[0].labeltext;
                        dtSeries.Rows.Add(dr);
                    }
                }

                else if (SerieName.StartsWith("gdd"))
                {
                    string serie = SerieName.Substring(7);
                    var aggFunction = from row in dtGdd.AsEnumerable()
                                      where row.Field<string>("name").Trim() == serie
                                      select new
                                      {
                                          trnsTag = row.Field<string>("trnsTag"),
                                          aggregationFunction = row.Field<string>("aggregationFunction"),
                                          markerType = row.Field<string>("markerType"),
                                          axisPosition = row.Field<string>("axisPosition"),
                                          stacked = row.Field<bool>("stacked"),
                                          gallery = row.Field<string>("gallery"),
                                          color = row.Field<string>("color"),
                                          labeltext = row.Field<string>("trnsTag"),
                                          pallete = row.Field<string>("colorPaletteName"),
                                          textColor = row.Field<string>("textColor")
                                      };

                    dr["Name"] = serie;//"TempAir_C_DayTimeMax";
                    dr["trnsTag"] = aggFunction.ToList()[0].trnsTag;
                    dr["aggregationfunction"] = aggFunction.ToList()[0].aggregationFunction;
                    dr["markerType"] = aggFunction.ToList()[0].markerType;
                    dr["axisPosition"] = aggFunction.ToList()[0].axisPosition;
                    dr["stacked"] = aggFunction.ToList()[0].stacked;
                    dr["gallery"] = aggFunction.ToList()[0].gallery;
                    dr["color"] = aggFunction.ToList()[0].color;
                    dr["panel"] = 4;
                    dr["addlInfo"] = "";
                    dr["year"] = "";
                    dr["labeltext"] = aggFunction.ToList()[0].labeltext;
                    dr["pallete"] = aggFunction.ToList()[0].pallete;
                    dr["textColor"] = aggFunction.ToList()[0].textColor;
                    dtSeries.Rows.Add(dr);

                    //dr = dtSeries.NewRow();
                    //dr["Name"] = "TempAir_C_NightTimeMin";
                    //dr["trnsTag"] = aggFunction.ToList()[0].trnsTag;
                    //dr["aggregationfunction"] = aggFunction.ToList()[0].aggregationFunction;
                    //dr["markerType"] = aggFunction.ToList()[0].markerType;
                    //dr["axisPosition"] = aggFunction.ToList()[0].axisPosition;
                    //dr["stacked"] = aggFunction.ToList()[0].stacked;
                    //dr["gallery"] = aggFunction.ToList()[0].gallery;
                    //dr["color"] = aggFunction.ToList()[0].color;
                    //dr["panel"] = 4;
                    //dr["title"] = "GDD";
                    //dr["year"] = "";
                    //dr["labeltext"] = aggFunction.ToList()[0].labeltext;
                    //dtSeries.Rows.Add(dr);
                    IsGDD = true;
                }
            }
            dtSeries.TableName = "ChartSerie";
            return dtSeries;
        }
        #region Properties
        public string Name
        {
            get;
            set;
        }
        #endregion

        #region IAgriInfo Members

        public string strTranslatedText
        {
            get;
            set;
        }


        public DataSet ds
        {
            get;
            set;
        }
        public DataTable dtGdd
        {
            get;
            set;
        }
        public Boolean ExcelFlag
        {
            get;
            set;
        }
        public string Selected
        {
            get;
            set;
        }
        public ArrayList ar
        {
            get;
            set;
        }
        public DateTime StartDate
        {
            get;
            set;
        }
        public List<string> GddValues
        {
            get;
            set;
        }
        #endregion

        private void hdnSeries_ValueChanged(string checkedSeries)//,List<string[]> lstCol)
        {
            try
            {
                string name = objAgriPresenter.GetChartName();
                string gddSeries = "";
                string nonGddSeries = checkedSeries;
                if (checkedSeries.ToLower().Contains("gdd"))
                {
                    gddSeries = checkedSeries.Substring(checkedSeries.ToLower().IndexOf("gdd"));
                    if (checkedSeries.ToLower().IndexOf("gdd") > 0)
                    {
                        nonGddSeries = checkedSeries.Substring(0, checkedSeries.ToLower().IndexOf("gdd") - 1);
                    }
                    else
                    {
                        nonGddSeries = "";
                    }
                }
                if (nonGddSeries != "")
                {
                    UpdateSeries(nonGddSeries);
                    common.LoadCtrl<WebChart>(this.Page, PlaceHolder1, "WebChart", name, "", dtData);
                }
                if (gddSeries != "")
                {
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                    //name = "GDD";
                    name = checkedSeries.ToLower().Contains("watermodel") ? "WaterModel" : "GDD";
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                    UpdateSeries(gddSeries);
                    common.LoadCtrl<WebChart>(this.Page, PlaceHolder1, "WebChart", name, "", dtData);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.AGRIINFO_READSERIES_FALURE) + ex.Message.ToString();
            }
            finally
            {

                if (Session["AuditData"] != null)
                {
                    IDictionary dict = (IDictionary)Session["AuditData"];
                    if (Session["IAgriInfo"] != null)
                    {
                        IDictionary dictAgriInfo = (IDictionary)Session["IAgriInfo"];
                        dict["weatherDateRange"] = dictAgriInfo["startDate"] + " to " + dictAgriInfo["endDate"];
                    }
                    string WeatherSeries = string.Empty;
                    string[] seriesList = checkedSeries.Split(';');
                    foreach (string SerieName in seriesList)
                    {

                        if (SerieName.StartsWith("node"))
                        {
                            WeatherSeries = WeatherSeries + SerieName.Substring(9) + " , ";
                        }
                        else if (SerieName.StartsWith("gdd"))
                        {
                            WeatherSeries = WeatherSeries + SerieName.Substring(7) + " , ";
                        }


                    }
                    dict["weatherParams"] = WeatherSeries;

                    Session["AuditData"] = dict;
                }
            }
        }
        protected void hApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (objLocInfo.DataPointInfo != null && objLocInfo.DataPointInfo.stationLatitude != 0 && objLocInfo.DataPointInfo.stationLongitude != 0 && objLocInfo.DataPointInfo.stationName != "")
                {
                    if (hdnSeries.Value == "")
                    {
                        HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.AGRIINFO_HAPPLY_NOSETTINGS);
                    }
                    else
                    {
                        checkFields();
                    }
                }
                else
                {
                    HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.AGRIINFO_HAPPLY_NOSTATION);
                }
            }

            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.AGRIINFO_HAPPLY_CHARTFAILURE) + ": " + ex.Message.ToString();
                return;
            }
        }
        void checkFields()
        {
            if (HttpContext.Current != null)
            {

                objDataPointInfo = DataPointInfo.getDataPointObject;
            }
            else
            {
                objDataPointInfo = new DataPointInfo();
            }
            // To check numerals in required fields
            //to be changed to be done from client side
            if (ValidateFields())
            {
                try
                {
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                    if (objSvcInfo.Moss == "true" && objSvcInfo.Module.ToLower() == "watermodel" && Session["IAgriInfo"] != null)
                    {
                        objAgriInfo = (Dictionary<string, string>)Session["IAgriInfo"];
                    }
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                    else
                    {
                        /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                        objAgriInfo.Clear();
                        //objAgriInfo.Add("startDate", txtstartDate.Value.ToString());
                        string date = ShortMonthDayPattern(oldinfo, newinfo, txtstartDate.Value);
                        DateTime x;
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - start
                        //objAgriInfo.Add("startDate", (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString() : date).ToString());
                        objAgriInfo.Add("startDate", (valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString() : DateTime.TryParse(date, out x) ? date : date.Substring(0, date.Length - 1)).ToString());
                        //Modify for IM01848073:New Agricast - Agriinfo - error in HU - end
                        string EndDate = getEndDate();
                        //Modify for Date Issue - 20140606 - Start
                        //objAgriInfo.Add("endDate", EndDate);
                        if (EndDate.EndsWith("/"))
                        {
                            EndDate = EndDate.Remove(EndDate.LastIndexOf("/"), 1);
                        }
                        if (!DateTime.TryParse(EndDate, out x) && oldinfo.LongDatePattern.ToString() == "yyyy. MMMM d.")
                        {
                            EndDate = EndDate.Replace(".", "/");
                        }
                        objAgriInfo.Add("endDate", (DateTime.TryParse(EndDate, out x) ? x.ToShortDateString() : DateTime.ParseExact(EndDate, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString()).ToString());
                        //Modify for Date Issue - 20140606 - End
                        objAgriInfo.Add("aggregation", ddlAggregate.SelectedValue.ToString());
                        if (txtPlantingDate.Value != string.Empty)
                        {
                            string plntDate = ShortMonthDayPattern(oldinfo, newinfo, txtPlantingDate.Value);
                            objAgriInfo.Add("plantingDate", (valid ? DateTime.ParseExact(plntDate, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString() : plntDate).ToString());
                        }
                        else
                        {
                            objAgriInfo.Add("plantingDate", "");
                        }
                        string altitude = "";
                        //if altitude is in feet, convert it to metres and add to object
                        if (objSvcInfo.Unit.ToLower().Equals("imperial", StringComparison.CurrentCultureIgnoreCase))
                        {
                            /*IM01288866 - New Agricast - altitude adjustement taking feet and not meters-Begin*/
                            //if (txtAltitude.Value != string.Empty && lblMasl.InnerText == Constants.FEET_ASL)
                            if (txtAltitude.Value != string.Empty && lblMasl.InnerText == objComUtil.getTransText(Constants.FEET_ASL))
                                altitude = objComUtil.ConvertValueMetrics(txtAltitude.Value, "ft-m");
                            /*IM01288866 - New Agricast - altitude adjustement taking feet and not meters-end*/

                        }
                        else
                        {
                            altitude = txtAltitude.Value.ToString();
                        }

                        objAgriInfo.Add("altitude", altitude);

                        if (EndDate != "")//Add for  IM01838379:Cookie Issue
                        {
                            //Modify for Date Issue - 20140606 - Start
                            string EndDate2 = EndDate;
                            string date2 = date;
                            bool valid2 = valid;
                            if (date2.EndsWith("."))
                            {
                                date2 = date2.Remove(date2.LastIndexOf("."), 1);
                            }
                            //newinfo.LongDatePattern="dd MMMM yyyy" and newinfo.LongDatePattern="dddd, MMMM dd, yyyy" can't be changed to Datetime
                            if (!DateTime.TryParse(EndDate2, out x))
                            {
                                EndDate2 = EndDate2.Replace(".", "/").Replace("-", "/");
                                if (!DateTime.TryParse(EndDate2, out x))
                                {
                                    string[] a = EndDate.Split('/');
                                    EndDate2 = a[1] + "/" + a[0] + "/" + a[2];
                                }
                            }
                            if (!DateTime.TryParse(date2, out x))
                            {
                                date2 = date.Replace(".", "/").Replace("-", "/");
                                if (date2.EndsWith("/"))
                                {
                                    date2=date2.Remove(date2.LastIndexOf("/"), 1);
                                }
                                if (!DateTime.TryParse(date2, out x))
                                {
                                    string[] b = date2.Split('/');
                                    date2 = b[1] + "/" + b[0] + "/" + b[2];
                                }
                            }
                            if (valid2 == true)
                            {
                                if (!DateTime.TryParse(DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString(), out x))
                                {
                                    valid2 = false;
                                }
                            }

                            //Commented by Rahul
                            //TimeSpan diffDate = DateTime.Parse(EndDate).Subtract(DateTime.Parse(txtstartDate.Value.ToString()));
                            //TimeSpan diffDate = DateTime.Parse(EndDate).Subtract(valid ?
                            //                        DateTime.Parse(DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString())
                            //                      : DateTime.Parse(date));
                            TimeSpan diffDate = DateTime.Parse(EndDate2).Subtract(valid2 ?
                                                    DateTime.Parse(DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString())
                                                  : DateTime.Parse(date2));
                            //Modify for Date Issue - 20140606 - End
                            //Added by Rahul
                            //TimeSpan diffDate = DateTime.Parse(EndDate).Subtract(DateTime.Parse(GetDateInCorrectFormat(txtstartDate.Value.ToString(), true)));
                            int duration = diffDate.Days + 1;

                            string agg = ddlAggregate.SelectedValue.ToString().ToLower();
                            if ((duration < 7 && agg == "weekly") || (duration < 10 && agg == "decade") || (duration < 30 && agg == "monthly"))
                            {
                                HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.ERROR_AGGREGATION_RANGE);
                                return;
                            }

                            else
                            {
                                AggregationValid = true;
                            }
                        }
                        //if (duration < 7 && ddlAggregate.SelectedValue.ToString().ToLower() != "dy")
                        //{
                        //    HttpContext.Current.Session["ErrorMessage"] = TranslatedText("Aggregation should be smaller than duration.");
                        //    return;
                        //}
                        //else if (duration > 10 && duration < 30 && (ddlAggregate.SelectedValue.ToString().ToLower() != "dc" || ddlAggregate.SelectedItem.ToString().ToLower() != "wy" || ddlAggregate.SelectedItem.ToString().ToLower() != "dy"))
                        //{
                        //    HttpContext.Current.Session["ErrorMessage"] = TranslatedText("Aggregation should be smaller than duration.");
                        //    return;
                        //}                  
                        /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                        double topSoilWHC = 0.0, subSoilWHC = 0.0;
                        switch (ddlTopSoilType.SelectedValue)
                        {
                            case "Sd": topSoilWHC = 0.07; break;
                            case "Lm": topSoilWHC = 0.15; break;
                            case "Sl": topSoilWHC = 0.14; break;
                            case "Ls": topSoilWHC = 0.11; break;
                            case "Cy": topSoilWHC = 0.18; break;
                            case "So": topSoilWHC = 0.155; break;
                            default: topSoilWHC = 0.0; break;
                        }

                        switch (ddlSubSoilType.SelectedValue)
                        {
                            case "Sd": subSoilWHC = 0.07; break;
                            case "Lm": subSoilWHC = 0.15; break;
                            case "Sl": subSoilWHC = 0.14; break;
                            case "Ls": subSoilWHC = 0.11; break;
                            case "Cy": subSoilWHC = 0.18; break;
                            case "So": subSoilWHC = 0.155; break;
                            default: subSoilWHC = 0.0; break;
                        }
                        objAgriInfo.Add("TopSoilWHC", topSoilWHC.ToString());
                        objAgriInfo.Add("SubSoilWHC", subSoilWHC.ToString());

                        if (string.IsNullOrWhiteSpace(txtTopSoilDepth.Value))
                            objAgriInfo.Add("TopSoilDepth", "0");
                        else
                            objAgriInfo.Add("TopSoilDepth", txtTopSoilDepth.Value);

                        if (string.IsNullOrWhiteSpace(txtSubSoilDepth.Value))
                            objAgriInfo.Add("SubSoilDepth", "0");
                        else
                            objAgriInfo.Add("SubSoilDepth", txtSubSoilDepth.Value);
                        /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                        Session["IAgriInfo"] = objAgriInfo;
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.GENERIC_ERRORONPAGE) + ":" + ex.Message.ToString();
                    throw ex;
                }
            }
            else
            {
                HttpContext.Current.Session["ErrorMessage"] = ((HttpContext.Current.Session["ErrorMessage"]).ToString() == string.Empty) ? "Please enter correct values." : (HttpContext.Current.Session["ErrorMessage"]).ToString();
            }

        }
        bool ValidateFields()
        {
            string reg = @"^([0-9]*|\d*\.\d{1}?\d*)$";

            if (txtAltitude.Value != "")
            {
                double alt = 0.0;
                bool isNum;
                isNum = double.TryParse(txtAltitude.Value, out alt);
                if (isNum == true)
                {
                    if (alt < 0)
                    {
                        HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.AGRIINFO_ALT_NOT_NEGATIVE);//Same message as in default excel export;
                        return false;
                    }
                }
                else
                {
                    HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.AGRIINFO_ALT_NUMBER);//Same message as in default excel export;
                    return false;
                }
            }

            if (txtstartDate.Value == "")
            {
                HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.DEF_SAVEEXCEL_STARTDATE);//Same message as in default excel export;
                return false;
            }
            DateTime date;
            if (DateTime.TryParse(txtstartDate.Value, out date))
            {
                if (date < DateTime.Parse(STARTDATE) || date > new DateTime(DateTime.Today.Year,12,31))
                {
                    HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.AGRIINFO_STARTDATE_OUTOFRANGE);
                    return false;
                }

            }
            if (txtPlantingDate.Value != "")
            {
                if (DateTime.TryParse(txtPlantingDate.Value, out date))
                {
                    if (date < DateTime.Parse(STARTDATE) || date > new DateTime(DateTime.Today.Year, 12, 31))
                    {
                        HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.AGRIINFO_PLANTINGDATE_OUTOFRANGE);
                        return false;
                    }

                }
            }
            /*IM01166165 -New Agricast - Agriinfo GDD IssueAfter providing planting date and start date and selecting GDD an error is being thrown. -BEGIN*/
            //if GDD is chosen and planting date is provided , then the planting date should be less than the end date
            if (!string.IsNullOrEmpty(hdnSeries.Value))
            {
                string[] seriesList = hdnSeries.Value.Split(';');
                if (seriesList != null && seriesList.Length > 0)
                {
                    foreach (string serie in seriesList)
                    {
                        if (serie.StartsWith("gdd"))
                        {
                            if (!string.IsNullOrEmpty(txtPlantingDate.Value))
                            {
                                string endDate = getEndDate();
                                if (!string.IsNullOrEmpty(endDate))
                                {
                                    CultureInfo cul = new CultureInfo(objSvcInfo.Culture);

                                    if (DateTime.Parse(txtPlantingDate.Value, cul) > DateTime.Parse(endDate))
                                    {
                                        HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.AGRIINFO_PLANTINGDATE_ENDDATE);
                                        return false;
                                    }
                                }//end of endate check
                            }//end of planting date check
                        }//end of GDD series check
                    }// end of for each
                }//end of series split

            }

            /*IM01166165 -New Agricast - Agriinfo GDD IssueAfter providing planting date and start date and selecting GDD an error is being thrown. -BEGIN*/
            return true;

        }
        protected void Page_prerender(object sender, EventArgs e)
        {

            if (!ExcelFlag)
            {

                setValues();

                if (objSvcInfo == null)
                    objSvcInfo = (ServiceInfo)Session["serviceInfo"];

                //change altitude value based on culture
                //if (txtAltitude.Value != string.Empty)
                //{
                //    //if ((objSvcInfo.Culture.ToLower().Equals("en-us", StringComparison.CurrentCultureIgnoreCase) || objSvcInfo.Culture.ToLower().Equals("sl-SI", StringComparison.CurrentCultureIgnoreCase)))
                //{
                //    if (Session["PreviousCulture"] != null)
                //    {
                //        if (Session["PreviousCulture"].ToString().ToLower() != "en-us" && Session["PreviousCulture"].ToString().ToLower() != "sl-si")
                //            txtAltitude.Value = objComUtil.ConvertValueMetrics(txtAltitude.Value, "m-ft");
                //    }
                //}
                //else
                //{
                //    if (Session["PreviousCulture"] != null)
                //    {
                //        if (Session["PreviousCulture"].ToString().ToLower() == "en-us" || Session["PreviousCulture"].ToString().ToLower() == "sl-si")
                //            txtAltitude.Value = objComUtil.ConvertValueMetrics(txtAltitude.Value, "ft-m");
                //    }
                //}


                //}

                //Change altitude value based on units
                if (objSvcInfo.Unit.ToLower().Equals("imperial", StringComparison.CurrentCultureIgnoreCase))
                {

                    /*IM01288866 - New Agricast - altitude adjustement taking feet and not meters-Begin*/
                    //if (txtAltitude.Value != string.Empty && lblMasl.InnerText != Constants.FEET_ASL)
                    if (txtAltitude.Value != string.Empty && lblMasl.InnerText == objComUtil.getTransText(Constants.M_ASL))
                        txtAltitude.Value = objComUtil.ConvertValueMetrics(txtAltitude.Value, "m-ft");
                    /*IM01288866 - New Agricast - altitude adjustement taking feet and not meters-End*/

                    /*IM01258137 - New Agricast - Translation - can't translate "{.More}" - BEGIN */
                    //lblMasl.InnerText = Constants.FEET_ASL;
                    lblMasl.InnerText = TranslatedText(Constants.FEET_ASL);
                    /*IM01258137 - New Agricast - Translation - can't translate "{.More}" - END */



                }
                else if (objSvcInfo.Unit.ToLower().Equals("metric", StringComparison.CurrentCultureIgnoreCase))
                {
                    /*IM01288866 - New Agricast - altitude adjustement taking feet and not meters-Begin*/
                    //if (txtAltitude.Value != string.Empty && lblMasl.InnerText != Constants.M_ASL)
                    if (txtAltitude.Value != string.Empty && lblMasl.InnerText == objComUtil.getTransText(Constants.FEET_ASL))
                        txtAltitude.Value = objComUtil.ConvertValueMetrics(txtAltitude.Value, "ft-m");
                    /*IM01288866 - New Agricast - altitude adjustement taking feet and not meters-End*/

                    /*IM01258137 - New Agricast - Translation - can't translate "{.More}" - BEGIN */
                    //lblMasl.InnerText = Constants.M_ASL;
                    lblMasl.InnerText = TranslatedText(Constants.M_ASL);
                    /*IM01258137 - New Agricast - Translation - can't translate "{.More}" - END */

                }

                //set the culture info
                //Commented by Rahul
                //VariantCulture = new CultureInfo(objSvcInfo.Culture);

                //Added by Rahul
                if (objSvcInfo.Culture != "en-GB")
                {
                    VariantCulture = new CultureInfo(objSvcInfo.Culture);
                }
                else
                {
                    VariantCulture = new CultureInfo("en-US");
                }

                HttpCookie agriInfoCookie = Request.Cookies["AgriInfo_" + objSvcInfo.Module];
                if (agriInfoCookie == null || agriInfoCookie.Value.ToString() == "")
                {
                    PopulateAltitude();
                }
                else
                {
                    string[] values = Server.UrlDecode(agriInfoCookie.Value).Split('#');
                    string[] lat = Server.UrlDecode(values[7]).Split('&');
                    if (objLocInfo.latitude.ToString() != lat[0] || objLocInfo.longitude.ToString() != lat[1])
                    {
                        PopulateAltitude();
                    }
                }


                checkFields();
                if (ddlDuration.SelectedValue.ToString() == "to_date")
                {
                    divEnd.Attributes.Add("class", "show");

                    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - Start */
                    if (txtEndDate.Value != "")
                        txtEndDate.Value = ShortMonthDayPattern(oldinfo, newinfo, txtEndDate.Value);
                    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - End */
                }
                else
                {
                    txtEndDate.Value = "";
                    divEnd.Attributes.Add("class", "hide");
                }
                //checkFields();//Move up for Date Issue - 20140612
                if (hdnSeries.Value != "" && ValidateFields() && AggregationValid)
                    hdnSeries_ValueChanged(hdnSeries.Value.ToString());

                if (hdnExpandCollapse.Value != "")
                {
                    string[] ExpandedNodes = hdnExpandCollapse.Value.Split(';');
                    foreach (string node in ExpandedNodes)
                    {
                        if (node.Contains("imgSeriesNode"))
                        {
                            string divID = "Div" + node.Substring(13);
                            HtmlGenericControl divCntrl = (HtmlGenericControl)common.FindControlRecursive(this, divID);
                            if (divCntrl != null)
                            {
                                divCntrl.Attributes.Add("class", "show");
                                Image imgCntrl = (Image)common.FindControlRecursive(this, node);
                                imgCntrl.ImageUrl = BOXMINUS;
                                imgCntrl.Attributes.Add("alt", "-");
                            }

                        }
                    }
                }
                if (hdnSeries.Value != null)
                {
                    string[] CheckedNodes = hdnSeries.Value.Split(';');
                    foreach (string node in CheckedNodes)
                    {
                        CheckBox cb = (CheckBox)common.FindControlRecursive(this, node);
                        if (cb != null)
                        {
                            cb.Checked = true;
                        }
                    }
                }
            }
            // if (ExcelFlag)
            else
            {
                ExportData();
            }
            CreateAgriInfoCookie();

            //convert the values of the date textboxes into the selected culture
            if (txtstartDate.Value != "")
            {
                //txtstartDate.Value = (DateTime.Parse(txtstartDate.Value)).ToString("MM/dd/yyyy", VariantCulture);
                //Commented by Rahul
                //txtstartDate.Value = (GetDateInCorrectFormatGetDate(txtstartDate.Value, false)).ToString(VariantCulture.DateTimeFormat.ShortDatePattern.ToString(), VariantCulture);
                txtstartDate.Value = ShortMonthDayPattern(oldinfo, newinfo, txtstartDate.Value);
                //Added by Rahul
                //txtstartDate.Value = GetDateInCorrectFormat(txtstartDate.Value.ToString(), false);
                //Add for IM01848073:New Agricast - Agriinfo - error in HU - start
                if (txtstartDate.Value.EndsWith("/") && newinfo.LongDatePattern.ToString() == "yyyy. MMMM d.")
                {
                    txtstartDate.Value = txtstartDate.Value.Replace("/", ".");
                }
                //Add for IM01848073:New Agricast - Agriinfo - error in HU - end
            }
            if (txtPlantingDate != null && txtPlantingDate.Value != "")
            {
                //txtPlantingDate.Value = (DateTime.Parse(txtPlantingDate.Value)).ToString("MM/dd/yyyy", VariantCulture);
                txtPlantingDate.Value = ShortMonthDayPattern(oldinfo, newinfo, txtPlantingDate.Value);
                //Add for IM01848073:New Agricast - Agriinfo - error in HU - start
                if (txtPlantingDate.Value.EndsWith("/") && newinfo.LongDatePattern.ToString() == "yyyy. MMMM d.")
                {
                    txtPlantingDate.Value = txtPlantingDate.Value.Replace("/", ".");
                }
                //Add for IM01848073:New Agricast - Agriinfo - error in HU - end
            }
            //Add for IM01848073:New Agricast - Agriinfo - error in HU - start
            if (txtEndDate.Value.EndsWith("/") && newinfo.LongDatePattern.ToString() == "yyyy. MMMM d.")
            {
                txtEndDate.Value = txtEndDate.Value.Replace("/", ".");
            }
            //Add for IM01848073:New Agricast - Agriinfo - error in HU - end
            Session["PreviousCulture"] = Session["CurrentCulture"];
        }
        void ExportData()
        {
            try
            {
                string checkedSeries = Selected;
                DataSet dsData = new DataSet();
                if (Selected != null && Selected != string.Empty)
                {

                    //string[] seriesList = checkedSeries.Split(';');
                    //DataTable dtSeriesDetails = objAgriPresenter.ReadSeriesDetailsExcel(seriesList);
                    objAgriPresenter.loadSeries();
                    objAgriPresenter.LoadGddSeries();

                    string gddSeries = "";
                    string nonGddSeries = checkedSeries;
                    if (checkedSeries.ToLower().Contains("gdd"))
                    {
                        gddSeries = checkedSeries.Substring(checkedSeries.ToLower().IndexOf("gdd"));
                        if (checkedSeries.ToLower().IndexOf("gdd") > 0)
                        {
                            nonGddSeries = checkedSeries.Substring(0, checkedSeries.ToLower().IndexOf("gdd") - 1);
                        }
                        else
                        {
                            nonGddSeries = "";
                        }
                    }
                    if (nonGddSeries != "")
                    {
                        UpdateSeries(nonGddSeries);
                        dsData.Tables.Add(getDataForExcel(dtData, objAgriPresenter.GetChartName()));
                    }
                    if (gddSeries != "")
                    {
                        UpdateSeries(gddSeries);
                        dsData.Tables.Add(getDataForExcel(dtData, "GDD"));
                    }
                }
                objSvcPre.ExportToExcel(dsData);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.AGRIINFO_EXPORTDATA_FAILURE);
            }
        }


        DataTable getDataForExcel(DataTable data, string name)
        {
            DataTable dt = null;
            DataTable dtInput = null;
            DataTable dtSeriesDetails = data;
            string Chartname = name;
            //To change first column as based on the chosen unit
            dtSeriesDetails = objSvcPre.ChangeUnits(dtSeriesDetails, objSvcInfo.Unit, objSvcInfo.WUnit);
            dt = objAgriPresenter.GetChartDataForExport(Chartname, dtSeriesDetails);
            dtInput = dt.Copy();
            dtInput.TableName = name;
            dtInput.Columns[0].ColumnName = TranslatedText("{" + dt.Columns[0].ColumnName + "}");
            for (int i = 1; i < dtInput.Columns.Count; i++)
            {
                int count = 1;
                string unit = "";
                if (dtSeriesDetails.Rows[i - 1]["trnsTag"].ToString().Contains("Ge_Temp"))
                    unit = "(°C)";
                else if (dtSeriesDetails.Rows[i - 1]["trnsTag"].ToString().Contains("Ge_Precipitation") || dtSeriesDetails.Rows[i - 1]["trnsTag"].ToString().Contains("Ge_precipitation"))
                    unit = "(mm)";
                else if (dtSeriesDetails.Rows[i - 1]["trnsTag"].ToString().Contains("Ge_Humidity"))
                    unit = "(%)";
                else if (dtSeriesDetails.Rows[i - 1]["trnsTag"].ToString().Contains("Ge_GlobalRadiation"))
                    unit = "(Wh/m2)";
                else if (dtSeriesDetails.Rows[i - 1]["trnsTag"].ToString().Contains("Ge_Windspeed_Avg"))
                    unit = "(m/s)";
                else if (dtSeriesDetails.Rows[i - 1]["trnsTag"].ToString().Contains("Ge_Windgust"))
                    unit = "(m/s)";
                else if (dtSeriesDetails.Rows[i - 1]["trnsTag"].ToString().Contains("Ge_Evapotranspiration"))
                    unit = "(mm)";
                else if (dtSeriesDetails.Rows[i - 1]["trnsTag"].ToString().Contains("Ge_SunshineDuration"))
                    unit = "(hrs)";

                string ColName = TranslatedText(dtSeriesDetails.Rows[i - 1]["trnsTag"].ToString()) + " " + unit;

                //string ColName = TranslatedText(dtSeriesDetails.Rows[i - 1]["trnsTag"].ToString()) + "_" + dtSeriesDetails.Rows[i - 1]["panel"].ToString();
                //string ColName = TranslatedText(dtSeriesDetails.Rows[i - 1]["labelText"].ToString());
                if (dtSeriesDetails.Rows[i - 1]["year"].ToString() != "")
                    ColName = ColName + dtSeriesDetails.Rows[i - 1]["year"].ToString();
                for (int j = 0; j < dtInput.Columns.Count; j++)
                {
                    if (dt.Columns[j].ColumnName == ColName)
                    {
                        ColName = ColName + "_" + count.ToString();
                    }
                }
                dtInput.Columns[i].ColumnName = ColName;
            }
            return dtInput;
        }
        string SerializeAgriInfoCookieString()
        {
            string cookieValue;
            try
            {
                string date = ShortMonthDayPattern(oldinfo, newinfo, txtstartDate.Value);

                string plntDate = string.Empty;
                if (txtPlantingDate != null && txtPlantingDate.Value != string.Empty)
                    plntDate = ShortMonthDayPattern(oldinfo, newinfo, txtPlantingDate.Value);

                /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - Start */
                string endDate = string.Empty;
                if (txtEndDate != null && txtEndDate.Value != string.Empty)
                    endDate = ShortMonthDayPattern(oldinfo, newinfo, txtEndDate.Value);
                /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - End */

                if (HttpContext.Current != null)
                {
                    objLocInfo = LocationInfo.getLocationInfoObj;
                }
                else
                {
                    objLocInfo = new LocationInfo();
                }


                cookieValue = Server.UrlEncode((valid ? DateTime.ParseExact(date, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString() : date).ToString() + "#"
                   + hdnSeries.Value + "#"
                   + ddlAggregate.SelectedIndex + "#"
                   + ddlDuration.SelectedIndex + "#"
                   + (txtPlantingDate != null && txtPlantingDate.Value != string.Empty ? (valid ? DateTime.ParseExact(plntDate, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString() : plntDate).ToString() : "") + "#"//);

                /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - Start */
                   + (txtEndDate != null && txtEndDate.Value != string.Empty ? (valid ? DateTime.ParseExact(endDate, NewCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString() : txtEndDate.Value).ToString() : "") + "#"//);
                    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - End */
                   + txtAltitude.Value + "#"
                   + objLocInfo.latitude + "&" + objLocInfo.longitude + "#"
                   + root + "#");// Add Pub name for the data format issue when change the pub 20140615
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.GENERIC_ERRORONPAGE) + ":" + ex.Message.ToString();
                throw ex;
            }

            return cookieValue;
        }

        void DeserializeAgriInfoCookieString(string cookiestr)
        {
            string[] values = cookiestr.Split('#');
            if (values[8] == root)// Add Pub name for the data format issue when change the pub 20140615
            {
                //Add for Date Issue - 20140612 - Start
                if (valid)
                {
                    string format = newinfo.ShortDatePattern.ToString();
                    DateTime Y;
                    if (!string.IsNullOrEmpty(values[0]) && DateTime.TryParse(values[0], out Y))
                    {
                        //DateTime.TryParseExact(values[0], "MM/DD/YYYY", null, DateTimeStyles.None, out startDateFromCookie);
                        values[0] = Convert.ToDateTime(values[0]).ToString(format).ToString();
                    }
                    if (!string.IsNullOrEmpty(values[5]) && DateTime.TryParse(values[5], out Y))
                    {
                        // DateTime.TryParseExact(values[0], "MM/DD/YYYY", null, DateTimeStyles.None, out endDateFromCookie);
                        values[5] = Convert.ToDateTime(values[5]).ToString(format).ToString();
                    }
                }
                //Add for Date Issue - 20140612 - End
                int i = 0;
                {
                    try
                    {
                        DateTime date = StartDate;
                        DateTime plntDate = DateTime.Today;
                        bool valid2 = valid;//valid for StatDate and EndDate should be same

                        //IM01838379:Cookie Issue - start

                        txtstartDate.Value = ShortMonthDayPattern(valid ? null : oldinfo, newinfo, values[0]);
                        //if (!string.IsNullOrEmpty(values[5]))
                        if (!string.IsNullOrEmpty(values[5]))
                        {
                            txtEndDate.Value = ShortMonthDayPattern(valid2 ? null : oldinfo, newinfo, values[5]);
                        }
                        else
                        {
                            txtEndDate.Value = "";
                        }
                        //txtstartDate.Value = ShortMonthDayPattern(valid ? null : oldinfo, newinfo, values[0]);
                        //txtEndDate.Value = ShortMonthDayPattern(valid ? null : oldinfo, newinfo, values[5]);
                        //txtstartDate.Value = ShortMonthDayPattern(oldinfo, newinfo, values[0]);
                        //txtEndDate.Value = ShortMonthDayPattern(oldinfo, newinfo, values[5]);
                        //IM01838379:Cookie Issue - end

                        //start of changes - 2013/03/12 
                        //txtstartDate.Value = DateTime.TryParse(ShortMonthDayPattern(null, newinfo, values[0]), out date) ? date.ToShortDateString() : StartDate.ToShortDateString();  

                        //txtstartDate.Value = ShortMonthDayPattern(null, newinfo, values[0]);
                        //end of changes - 2013/03/12

                        /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - Start */
                        //txtEndDate.Value = ShortMonthDayPattern(null, newinfo, values[5]);
                        /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - End */

                        hdnSeries.Value = values[1];
                        ddlAggregate.SelectedIndex = Int32.TryParse(values[2], out i) ? (i >= 0) ? i : 0 : 0;
                        ddlDuration.SelectedIndex = Int32.TryParse(values[3], out i) ? (i >= 0) ? i : 0 : 0;
                        if (HttpContext.Current != null)
                        {
                            objLocInfo = LocationInfo.getLocationInfoObj;
                        }
                        else
                        {
                            objLocInfo = new LocationInfo();
                        }

                        string[] lat = values[7].Split('&');

                        if (lat[0] == objLocInfo.latitude.ToString() && lat[1] == objLocInfo.longitude.ToString())
                            txtAltitude.Value = values[6];
                        //if (txtPlantingDate != null && values[4] != string.Empty)
                        //    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - Start */
                        //    //txtPlantingDate.Value = DateTime.TryParse(ShortMonthDayPattern(null, newinfo, values[4]), out plntDate) ? plntDate.ToShortDateString() : "";
                        //    txtPlantingDate.Value = DateTime.TryParse(ShortMonthDayPattern(null, newinfo, Server.UrlDecode(values[4])), out plntDate) ? plntDate.ToShortDateString() : "";
                        ///* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - Start */
                    }

                    catch (Exception ex)
                    {
                        HttpContext.Current.Session["ErrorMessage"] = TranslatedText(Constants.GENERIC_ERRORONPAGE) + ":" + ex.Message.ToString();
                        throw ex;
                    }

                }
            }

        }
        void CreateAgriInfoCookie()
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (HttpContext.Current != null) Session = HttpContext.Current.Session;

            if (Session == null || Session["serviceInfo"] == null)
            {

                objSvcInfo = new ServiceInfo();
            }
            else
            {
                objSvcInfo = (ServiceInfo)Session["serviceInfo"];
            }
            HttpCookie AgriInfoCookie = new HttpCookie(("AgriInfo_" + objSvcInfo.Module), SerializeAgriInfoCookieString());
            AgriInfoCookie.Expires = DateTime.Now.AddDays(365);
            Response.Cookies.Add(AgriInfoCookie);
        }
        void UpdateSeries(string checkedSeries)
        {
            string[] seriesList = checkedSeries.Split(';');
            dtData = ReadSeriesDetails(seriesList);
            List<string[]> alColumns = new List<string[]>();

            objSeriesInfo = SeriesInfo.getSeriesInfoObject;
            if (objSeriesInfo.SeriesList != null && !IsGDD)
            {
                foreach (AdvancedOptionInfo obj in objSeriesInfo.SeriesList)
                {
                    string addlInfo = "";
                    if (obj.Year.ToString() != string.Empty && obj.Year.ToString() != "0")
                    {
                        DataRow drNew;
                        for (int i = 0; i < dtData.Rows.Count; i++)
                        {
                            if (dtData.Rows[i]["name"].ToString().ToLower() == obj.Serie.ToLower() && dtData.Rows[i]["year"].ToString() == string.Empty
                                && "node" + dtData.Rows[i]["panel"].ToString() == obj.Series.ToString())
                            {
                                drNew = dtData.NewRow();
                                drNew["Name"] = dtData.Rows[i]["Name"];
                                drNew["trnsTag"] = dtData.Rows[i]["trnsTag"];
                                drNew["aggregationfunction"] = dtData.Rows[i]["aggregationfunction"];
                                drNew["markerType"] = dtData.Rows[i]["markerType"];
                                drNew["axisPosition"] = dtData.Rows[i]["axisPosition"];
                                drNew["gallery"] = dtData.Rows[i]["gallery"];
                                drNew["stacked"] = dtData.Rows[i]["stacked"];
                                drNew["color"] = dtData.Rows[i]["color"];
                                drNew["panel"] = dtData.Rows[i]["panel"];
                                drNew["addlInfo"] = dtData.Rows[i]["addlInfo"];
                                drNew["trnsTag"] = dtData.Rows[i]["trnsTag"];
                                drNew["year"] = obj.Year.ToString();
                                dtData.Rows.Add(drNew);
                                addlInfo = obj.Year.ToString();
                            }
                        }

                    }
                    addlInfo = addlInfo + "," + obj.Aggregation.ToString();
                    string column = obj.Aggregation.ToString() + obj.Serie;
                    if (obj.Trend != "")
                    {
                        column = column + obj.Trend.Trim();
                        addlInfo = addlInfo + "," + obj.Trend.Trim();
                    }
                    if (obj.Altitude != "")
                    {
                        //objLocInfo.DataPointInfo.altitude = Int32.Parse(obj.Altitude.ToString());
                        column = bool.Parse(obj.Altitude.ToString()) ? column + "_altAdj" : column;
                        addlInfo = addlInfo + "," + TranslatedText("AltAdjustment");
                    }
                    if (obj.Accumulate != "")
                    {
                        column = bool.Parse(obj.Accumulate) ? "acc" + column : column;
                        addlInfo = addlInfo + "," + TranslatedText("accumilate");
                    }

                    alColumns.Add(new string[] { column, obj.Serie, obj.Series, addlInfo });
                }
            }
            if (alColumns.Count > 0)
            {
                foreach (DataRow dr in dtData.Rows)
                {
                    foreach (string[] ser in alColumns)
                    {
                        if (dr["name"].ToString().ToLower() == ser[1].ToString().ToLower())
                        {
                            string agg = ser[0].ToString().Substring(0, 3).ToLower();
                            if ((agg == "sum" || agg == "max" || agg == "min" || agg == "avg"))
                            {
                                ser[0] = ser[0].ToString().Substring(3, ser[0].ToString().Length - 3).ToLower();
                                dr["aggregationfunction"] = agg;
                            }

                            agg = ser[0].ToString().Length > 6 ? ser[0].ToString().Substring(0, 6).ToLower() : "";
                            if ((agg == "accsum" || agg == "accmax" || agg == "accmin" || agg == "accavg"))
                            {
                                if (ddlAggregate.SelectedValue == "daily")
                                {
                                    ser[0] = agg.Substring(0, 3) + ser[0].ToString().Substring(6, ser[0].ToString().Length - 6).ToLower();
                                }
                                else
                                {
                                    ser[0] = ser[0].ToString().Substring(6, ser[0].ToString().Length - 6).ToLower();
                                }
                                dr["aggregationfunction"] = agg;
                            }

                            if (dr["year"].ToString() == string.Empty && "node" + dr["panel"].ToString() == ser[2].ToString())
                            {
                                dr["name"] = ser[0].ToString();
                                dr["addlInfo"] = ser[3].ToString();
                            }
                        }
                    }
                }
            }
            if (IsGDD && !ExcelFlag)
            {
                List<AdvancedOptionInfo> serie = new List<AdvancedOptionInfo>();
                string addlInfo = "";
                if (objSeriesInfo.SeriesList != null)
                {
                    var sers = from ser in objSeriesInfo.SeriesList
                               where ser.Serie.ToLower().Contains("gdd")
                               select ser;
                    serie.AddRange(sers.ToList());
                }

                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/

                if (serie.ToList() != null && serie.ToList().Count > 0)
                {
                    if (objAgriInfo.ContainsKey("Method"))
                        objAgriInfo["Method"] = serie.ToList()[0].Method;
                    else
                        objAgriInfo.Add("Method", serie.ToList()[0].Method);

                    if (objAgriInfo.ContainsKey("Cap"))
                        objAgriInfo["Cap"] = serie.ToList()[0].Cap;
                    else
                        objAgriInfo.Add("Cap", serie.ToList()[0].Cap);

                    if (objAgriInfo.ContainsKey("Base"))
                        objAgriInfo["Base"] = serie.ToList()[0].Base;
                    else
                        objAgriInfo.Add("Base", serie.ToList()[0].Base);
                    addlInfo = serie.ToList()[0].Method + "," + serie.ToList()[0].Cap + "," + serie.ToList()[0].Base;
                    dtData.Rows[0]["addlInfo"] = addlInfo;
                }
                else
                {
                    objAgriPresenter.getGDDvalues();
                    if (objAgriInfo.ContainsKey("Method"))
                        objAgriInfo["Method"] = GddValues[0];
                    else
                        objAgriInfo.Add("Method", GddValues[0]);

                    if (objAgriInfo.ContainsKey("Cap"))
                        objAgriInfo["Cap"] = GddValues[2];
                    else
                        objAgriInfo.Add("Cap", GddValues[2]);

                    if (objAgriInfo.ContainsKey("Base"))
                        objAgriInfo["Base"] = GddValues[1];
                    else
                        objAgriInfo.Add("Base", GddValues[1]);
                    addlInfo = GddValues[0] + "," + GddValues[2] + "," + GddValues[1];
                    dtData.Rows[0]["addlInfo"] = addlInfo;
                }

                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
            }

            if (objAgriInfo.Count == 0)
            {
                objAgriInfo = (Dictionary<string, string>)Session["IAgriInfo"];
                objAgriInfo["IsGDD"] = IsGDD.ToString();
            }
            else
            {
                if (objAgriInfo.ContainsKey("IsGDD"))
                    objAgriInfo["IsGDD"] = IsGDD.ToString();
                else
                    objAgriInfo.Add("IsGDD", IsGDD.ToString());
            }
            //if (IsGDD)
            //    objAgriInfo["aggregation"] = "Daily";
            Session["IAgriInfo"] = objAgriInfo;
        }
        //Added by Rahul
        public string GetDateInCorrectFormat(string datetimeString, bool setValue)
        {
            return GetDateInCorrectFormatGetDate(datetimeString, setValue).ToShortDateString();
        }

        //Added by Rahul
        public DateTime GetDateInCorrectFormatGetDate(string datetimeString, bool setValue)
        {
            //Added by Rahul
            bool isInCorrectDateFormat = false;
            DateTime dateTimeInCorrectFormat;
            CultureInfo culture;
            DateTimeStyles styles;

            if (setValue)
            {
                if (Session["CurrentCulture"] != null)
                {
                    //if (Session["CurrentCulture"].ToString() != "en-GB")
                    //{
                    culture = CultureInfo.CreateSpecificCulture(Session["CurrentCulture"].ToString());
                    //}
                    //else
                    //{
                    //    culture = CultureInfo.CreateSpecificCulture("en-US");
                    //}

                }
                else
                {
                    culture = CultureInfo.CreateSpecificCulture("en-GB");
                }
            }
            else
            {
                if (Session["PreviousCulture"] != null)
                {
                    //if (Session["PreviousCulture"].ToString() != "en-GB")
                    //{
                    culture = CultureInfo.CreateSpecificCulture(Session["PreviousCulture"].ToString());
                    //}
                    //else
                    //{
                    //    culture = CultureInfo.CreateSpecificCulture("en-US");
                    //}
                }
                else
                {
                    culture = CultureInfo.CreateSpecificCulture("en-GB");
                }
            }

            styles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal;

            //isInCorrectDateFormat = DateTime.TryParse(datetimeString, culture, styles, out dateTimeInCorrectFormat);
            // dateTimeInCorrectFormat = DateTime.ParseExact(datetimeString, culture.DateTimeFormat.ShortDatePattern.ToString(), culture);
            try
            {
                dateTimeInCorrectFormat = DateTime.Parse(datetimeString, culture);
            }
            catch (Exception ex)
            {
                dateTimeInCorrectFormat = DateTime.Today;
            }


            return dateTimeInCorrectFormat;
        }
        /* modified by jerrey @Nov 27th, 2013 - begin */
        string formatdate(string date, DateTimeFormatInfo dtFormatInfo)
        {
            char[] sep = { '/', '.', '-' };
            char strDateSeparator = ' ';
            char strDtFormatInfoSeparator = ' ';

            for (int i = 0; i < sep.Length; i++)
            {
                if (date.Trim().Contains(sep[i].ToString()))
                {
                    strDateSeparator = sep[i];
                    break;
                }
            }

            for (int i = 0; i < sep.Length; i++)
            {
                if (dtFormatInfo.ShortDatePattern.Trim().Contains(sep[i].ToString()))
                {
                    strDtFormatInfoSeparator = sep[i];
                    break;
                }
            }

            string newdate = string.Empty;
            string[] dateParts = date.Split(strDateSeparator);
            for (int i = 0; i < dateParts.Count(); i++)
            {
                if (dateParts[i].Length == 1)
                    dateParts[i] = "0" + dateParts[i];

                newdate = newdate + dateParts[i] + strDtFormatInfoSeparator;
            }
            if (newdate != string.Empty)
                date = newdate.Substring(0, newdate.Length - 1);
            return date;
        }
        /* modified by jerrey @Nov 27th, 2013 - end */

        string ShortMonthDayPattern(DateTimeFormatInfo olddt, DateTimeFormatInfo newdt, string date)
        {
            if (olddt == null)
            {
                /* modified by Jerrey @May 7th 2013 - Begin */
                //CultureInfo USCult = new CultureInfo("en-US");                
                CultureInfo USCult;
                if (objSvcInfo.Culture != "en-GB")
                {
                    USCult = new CultureInfo(objSvcInfo.Culture);
                }
                else
                {
                    USCult = new CultureInfo("en-US");
                }
                /* modified by Jerrey @May 7th 2013 - End */
                olddt = USCult.DateTimeFormat;
            }
            date = formatdate(date, olddt);
            DateTime indate;
            string outdate = date;
            // if (DateTime.TryParseExact(date, olddt.ShortDatePattern, null, DateTimeStyles.None, out indate))
            if (DateTime.TryParseExact(date, olddt.ShortDatePattern, null, DateTimeStyles.None, out indate))
            {
                string format = newdt.ShortDatePattern.ToString();
                outdate = indate.ToString(format);
                valid = true;
            }
            else
            {
                valid = false;
            }
            return outdate;
        }

    }
}

