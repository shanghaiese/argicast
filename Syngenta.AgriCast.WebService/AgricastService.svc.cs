using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Syngenta.AgriCast.WebService.View;
using Syngenta.AgriCast.WebService.Presenter;
using System.Data;
using System.Xml;
using System.Collections;
using Syngenta.AgriCast.Icon.DTO;
using System.ServiceModel.Activation;
using System.Web;
using Syngenta.AgriCast.Tables.View;
using Syngenta.Agricast.Modals;
using System.Xml.Serialization;
using Syngenta.Data.Access;
using System.IO;
using Syngenta.AgriCast.ExceptionLogger;
using System.Configuration;
using System.Web.Security;



namespace Syngenta.AgriCast.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AgricastService : IAgricastService, IRuleSets
    {
        DataTable dtOutput;
        ServiceOutput so = new ServiceOutput();
        Chart chart;
        Icons icon;
        IconData idata;
        FeatureRequest objFR;
        StationDetails stnDet;
        tables tbls;
        Legend legs;
        //public ServiceOutput getAgricastServiceData(string strToken, string xmlFeatureRequest, string strServiceID, string strModuleIDs, DateTime dStartDate, DateTime dEndDate, string strCultureCode)
        public ServiceOutput getAgricastServiceData(string strToken, string xmlFeatureRequest, string strServiceID, string strModuleIDs, string dStartDate, string dEndDate, string strCultureCode, string strUnit)
        {
            //HttpContext.Current.Session["MySessionState"] = "Try";
            WebServicePresenter objWebSvcPre = new WebServicePresenter(this);
            xmlFeatureRequest = HttpUtility.UrlDecode(xmlFeatureRequest);
            xmlFeatureRequest = HttpUtility.HtmlDecode(xmlFeatureRequest);
            DTOIcon objParam = new DTOIcon();
            DataSet ds = new DataSet();
            InitialiseOutput();

            /*** Used for Testing Webservice*/
            #region TestData
            //strToken = "agricast";
            //xmlFeatureRequest = "<FeatureRequest><Latitude>47.5</Latitude><Longitude>7.5</Longitude><Altitude>500</Altitude><MaxAltitudeDiff>200</MaxAltitudeDiff><MaxDistanceDiff>450</MaxDistanceDiff></FeatureRequest>";
            //strServiceID = "glbsip02o";
            //strModuleIDs = "vew_daily";
            //dStartDate = System.DateTime.Today.AddDays(-15).ToShortDateString();
            //dEndDate = System.DateTime.Today.AddDays(-11).ToShortDateString();
            //strCultureCode = "en-GB";
            //strUnit = "metric";

            #endregion
            /****End of Test Data****/

            /* IM01770145:New Agricast - AgricastService.svc - remove default token - Modify by Infosys-20140402-start*/
            //if (HttpUtility.HtmlDecode(strToken).ToLower() != "agricast")
            //    return so;

            strToken = HttpUtility.HtmlDecode(strToken);
            // check if the token is valid 
            var validToken = Roles.FindUsersInRole(strServiceID, strToken);
            if (validToken.Length == 0)
                return so;
             
            /* IM01770145:New Agricast - AgricastService.svc - remove default token - Modify by Infosys-20140402-end*/

            //Deserialization of the XML file
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(xmlFeatureRequest);

                XmlSerializer xSerializer = new XmlSerializer(typeof(FeatureRequest));
                System.IO.StringReader rdr = new StringReader(xmlFeatureRequest);
                objFR = (FeatureRequest)xSerializer.Deserialize(rdr);

                int iResultCount = 10;
                string timezone = objWebSvcPre.getTimeZoneOffset(objFR.Latitude, objFR.Longitude, (int)objFR.MaxDistanceDiff, (int)objFR.Altitude, iResultCount).ToString();
                string sunrise = objWebSvcPre.getSunrise(DateTime.Now.Date, objFR.Latitude, objFR.Longitude).ToString();
                string sunset = objWebSvcPre.getSunset(DateTime.Now.Date, objFR.Latitude, objFR.Longitude).ToString();

                Response res = new Response();
                res.FeatureRequest = objFR;
                res.Sunrise = sunrise;
                res.Sunset = sunset;
                res.TimeZoneOffset = timezone;
                so.FeatureResponse = res;

                DataTable dt = objWebSvcPre.getNeabyStations(objFR.Latitude, objFR.Longitude, (int)objFR.MaxDistanceDiff, (int)objFR.Altitude, iResultCount, HttpUtility.HtmlDecode(strCultureCode));

                stnDet.Name = dt.Rows[0]["Name"].ToString();
                stnDet.Altitude = Int32.Parse(dt.Rows[0]["Altitude"].ToString());
                stnDet.BearingDegrees = double.Parse(dt.Rows[0]["BearingDegrees"].ToString());
                stnDet.Distance = double.Parse(dt.Rows[0]["DistanceKm"].ToString());
                stnDet.Latitude = double.Parse(dt.Rows[0]["Latitude"].ToString());
                stnDet.Longitude = double.Parse(dt.Rows[0]["Longitude"].ToString());
                res.StationDetails = stnDet;
                objWebSvcPre.setServiceHandlerWebServiceValues(HttpUtility.HtmlDecode(strServiceID), "");
                DataTable dtNodes = objWebSvcPre.getNodeList(HttpUtility.HtmlDecode(strModuleIDs));
                /*Unit Implementation in Web Services - Begin*/
                if (string.IsNullOrEmpty(strUnit) || !strUnit.ToLower().Contains("imperial"))
                    /*Unit Implementation in Web Services - End*/
                    strUnit = "metric";

                for (int i = 0; i < dtNodes.Rows.Count; i++)
                {
                    switch (dtNodes.Rows[i]["Node"].ToString())
                    {
                        case "chart":
                            chart = new Chart();
                            DataTable dtChartData = new DataTable();
                            /*Unit Implementation in Web Services - Begin*/
                            dtChartData = objWebSvcPre.getChartData(stnDet.Latitude, stnDet.Longitude, (int)stnDet.Altitude, (int)objFR.MaxAltitudeDiff, (int)objFR.MaxDistanceDiff, dtNodes.Rows[i]["ModuleName"].ToString(), strServiceID, strCultureCode, strUnit);
                            /*Unit Implementation in Web Services - End*/
                            chart.ChartUrl = dtChartData.Rows[0][1].ToString();
                            chart.ModuleID = dtChartData.Rows[0][0].ToString();
                            so.Charts.Add(chart);
                            break;

                        case "icon":
                            icon = new Icons();
                            objWebSvcPre.setIconWebServiceValues(stnDet.Latitude, stnDet.Longitude, (int)stnDet.Altitude, (int)objFR.MaxAltitudeDiff, (int)objFR.MaxDistanceDiff, Convert.ToDateTime(sunrise), Convert.ToDateTime(sunset), strServiceID, dtNodes.Rows[i]["ModuleName"].ToString(), strCultureCode);
                            icon.ModuleID = dtNodes.Rows[i][1].ToString();
                            icon.IconList = objWebSvcPre.getWeatherIcons(objParam, dtNodes.Rows[i]["NodeName"].ToString());
                            so.Icons.Add(icon);
                            break;

                        case "tblDaysRows":
                            tbls = new tables();
                            tbls.tableList = new List<table>();
                            tbls.ModuleID = dtNodes.Rows[i][1].ToString();
                            tbls.tableList = objWebSvcPre.getTableData(stnDet.Latitude, stnDet.Longitude, (int)stnDet.Altitude, (int)objFR.MaxDistanceDiff, (int)objFR.MaxAltitudeDiff, this, dtNodes.Rows[i]["Node"].ToString(), dtNodes.Rows[i]["NodeName"].ToString(), strCultureCode, strServiceID, dtNodes.Rows[i]["ModuleName"].ToString(), HttpUtility.HtmlDecode(strCultureCode), strUnit);
                            so.tables.Add(tbls);
                            break;

                        case "tblSeriesRows":
                            tbls = new tables();
                            tbls.tableList = new List<table>();
                            tbls.ModuleID = dtNodes.Rows[i][1].ToString();

                            tbls.tableList = objWebSvcPre.getTableData(stnDet.Latitude, stnDet.Longitude, (int)stnDet.Altitude, (int)objFR.MaxDistanceDiff, (int)objFR.MaxAltitudeDiff, this, dtNodes.Rows[i]["Node"].ToString(), dtNodes.Rows[i]["NodeName"].ToString(), strCultureCode, strServiceID, dtNodes.Rows[i]["ModuleName"].ToString(), HttpUtility.HtmlDecode(strCultureCode), strUnit);
                            so.tables.Add(tbls);
                            break;

                        //case "nearbyDataPoint":
                        //    ds.Tables.Add(objWebSvcPre.getNeabyStations(objFR.Latitude, objFR.Longitude, (int)objFR.MaxDistanceDiff, (int)objFR.Altitude, iResultCount, HttpUtility.HtmlDecode(strCultureCode)));
                        //    ds.AcceptChanges();
                        //    break;

                        case "legend":
                            legs = new Legend();
                            legs = objWebSvcPre.getLegendData(dtNodes.Rows[i]["NodeName"].ToString());
                            so.Legends.Add(legs);
                            break;

                    }
                }
            }
            catch (Exception ex)
            {

                AgriCastException currEx = new AgriCastException(objWebSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                // HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.WS_GETSVCDATA_FAILURE) + " : " + ex.Message.ToString();
            }
            finally
            {
                IDictionary dictAudit = new Hashtable();
                dictAudit["userIP"] = "";// HttpContext.Current.Request.UserHostAddress;
                dictAudit["userID"] = "";
                dictAudit["token"] = strToken;
                dictAudit["referrer"] = "none";
                /* IM01770145:New Agricast - AgricastService.svc - remove default token - Modify by Infosys-20140402-start*/
                //dictAudit["entrancePath"] = "WebService";
                dictAudit["entrancePath"] = "AgricastService - getAgricastServiceData";
                /* IM01770145:New Agricast - AgricastService.svc - remove default token - Modify by Infosys-20140402-end*/
                dictAudit["culture"] = HttpUtility.HtmlDecode(strCultureCode);
                dictAudit["sessionID"] = "";// HttpContext.Current.Session.SessionID;
                dictAudit["service"] = strServiceID;
                dictAudit["module"] = strModuleIDs;
                dictAudit["locSearchType"] = "";
                dictAudit["locSearchStringType"] = "";
                dictAudit["locSearchString"] = xmlFeatureRequest;
                dictAudit["locSearchDatasource"] = "";
                dictAudit["numOfLocs"] = 0;
                if (objFR.Latitude == 0)
                    dictAudit["searchLat"] = DBNull.Value;
                else
                    dictAudit["searchLat"] = objFR.Latitude;
                if (objFR.Longitude == 0)
                    dictAudit["searchLong"] = DBNull.Value;
                else
                    dictAudit["searchLong"] = objFR.Longitude;
                dictAudit["countryName"] = "";
                dictAudit["locName"] = "";
                dictAudit["weatherDatasource"] = "";
                dictAudit["weatherLat"] = DBNull.Value;
                dictAudit["weatherLong"] = DBNull.Value;
                dictAudit["weatherDateFrom"] = "";
                dictAudit["weatherDateTo"] = "";
                dictAudit["weatherSeries"] = "";
                dictAudit["weatherParams"] = DBNull.Value;
                objWebSvcPre.SaveServiceAuditData(dictAudit);
            }

            return so;
        }

        void InitialiseOutput()
        {
            so.FeatureResponse = new Response();
            objFR = new FeatureRequest();
            stnDet = new StationDetails();
            so.Charts = new List<Chart>();
            so.Icons = new List<Icons>();
            so.tables = new List<tables>();
            so.Legends = new List<Legend>();
        }

        public string SerializeToXML(ServiceOutput obj)
        {
            XmlDocument doc = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();

            try
            {
                serializer.Serialize(stream, obj);
                stream.Position = 0;
                doc.Load(stream);
                return doc.InnerXml;
            }
            catch
            {
                throw;
            }

            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
        //method to return version
        public string Version()
        {
            try
            {
                return "$Id: AgricastService 266 2012-02-09 20:34:19Z Infosys $\n";
            }
            catch (Exception ex)
            {
                WebServicePresenter objWebSvcPre = new WebServicePresenter(this);
                AgriCastException currEx = new AgriCastException(objWebSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                // HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.WS_GETSVCDATA_FAILURE) + " : " + ex.Message.ToString();
                return "";

            }
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

        #region Ruleset Properties
        public DateTime SunRise
        {
            get;
            set;
        }

        public DateTime SunSet
        {
            get;
            set;
        }

        public DataTable DtInput
        {
            get;
            set;
        }

        public DataTable DtOutput
        {
            get
            {
                if (dtOutput == null)
                {
                    dtOutput = new DataTable();
                    dtOutput.Columns.Add("day");
                    dtOutput.Columns.Add("Hour");
                    dtOutput.Columns.Add("ColorCode");
                    dtOutput.Columns.Add("value");
                    dtOutput.Columns.Add("restrictions");
                }
                return dtOutput;

            }
            set
            {
                dtOutput = value;
            }
        }

        public DateTime StartDate
        {
            get;
            set;
        }

        public DateTime EndDate
        {
            get;
            set;
        }
        #endregion
    }
}
