using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syngenta.AgriCast.Charting.DataAccess;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using System.Drawing;
using System.Globalization;
using System.Collections;
using System.IO;
using System.Data;
using Syngenta.AgriCast.Charting.DTO;
using Syngenta.Data.Access;
using System.Web.SessionState;
using System.Web.UI;
using System.Web;
using Syngenta.AgriCast.Common.DataAccess;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.Agricast.Modals;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common;
using AIS.UsefulFunctions.DerivedCalculations;

namespace Syngenta.AgriCast.Charting.Service
{
    public class ChartService
    {
        ChartDb chartDA = new ChartDb();
        int _start = 0;
        int _end = 0;
        string temporalAgg = "";
        string dtSource = "";
        DateTime current = System.DateTime.Today;
        DateTime startDate;
        DateTime endDate;
        DTOchart objGparams;
        DateTime selectedEndDate;
        ServiceHandler sh = new ServiceHandler();
        DataSet dsData = new DataSet();
        DataTable dtData = new DataTable();
        Series series = new Series();
        CommonUtil cUtil = new CommonUtil();
        Dictionary<string, string> agriInfo;
        int noofvalues = 0;
        int valuesperday = 24;
        int dfactor = 8;
        int valuestep = 1;
        int noofdays = 0;
        LocationInfo loc;
        DataPointInfo dpi;
        string dswidth;
        string culture = "en-GB";
        string CHART = "ChartSettings";
        string PrimaryAxis = "PrimaryAxis";
        string SecondaryAxis = "SecondaryAxis";
        string ChartSerie = "ChartSerie";
        //Web Service Issue
        static double dStnLatitude = 0;
        static double dStnLongitude = 0;
        static int iAltitude;
        static int iMaxAllowedDist;
        static int iMaxAltitudeDiff;
        //double dStnLatitude = 0;
        //double dStnLongitude = 0;
        //int iAltitude;
        //int iMaxAllowedDist;
        //int iMaxAltitudeDiff;
        //End of Web Service Issue
        string strServiceName;
        string strModulename;
        double calcMaxY = 0d;
        double calcMinY = 0d;
        double calcMaxY2 = 0d;
        double calcMinY2 = 0d;
        string cName = "";
        WeatherData1 wData = new WeatherData1();
        FeatureRequest fReq;
        WeatherDataRequest wReq;
        WeatherDataResponse wRes;
        ServiceInfo objServiceInfo;
        ServicePresenter objSvcPre = new ServicePresenter();
        TranslateData objTranslate = new TranslateData();
        bool missingData = false;
        /// <summary>
        /// Constructor
        /// </summary>
        public ChartService()
        {
            loc = LocationInfo.getLocationInfoObj;
            dpi = (DataPointInfo)loc.DataPointInfo;
        }

        /// <summary>
        /// this is to get the results from AIS DB
        /// </summary>
        public Tuple<DataTable, FeatureResponse> getData(DataTable series, string aggregation, string dataSource, int start, int end, bool isAgriInfo)
        {
            Serie serie = new Serie("", "", false, enumAggregationFunction.avg, "");
            List<Serie> lst = new List<Serie>();
            bool NotWaterModel = true;

            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                if (agriInfo != null && isAgriInfo)
                {
                    startDate = DateTime.Parse(agriInfo["startDate"].ToString());
                    endDate = DateTime.Parse(agriInfo["endDate"].ToString());
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                    if (cName.ToLower() == "watermodel")
                    {
                        /* UAT Issue - Data from Mar 1st & from Mar 6th, the moisture value are not correct - Jerrey - Start */
                        //you have to show data till todays date -2 days
                        //if (endDate.Date.Subtract(DateTime.Now.Date).Days > 0)
                        //    endDate = DateTime.Now.Date.AddDays(-2);
                        //if (startDate.Date.Subtract(DateTime.Now.Date).Days > 0
                        //    && endDate.Date.Subtract(DateTime.Now.Date).Days > 0)
                        //{
                        //    HttpContext.Current.Session["ErrorMessage"] = cUtil.getTransText(Constants.INVALID_DATE_RANGE);
                        //    throw new Exception(cUtil.getTransText(Constants.INVALID_DATE_RANGE));
                        //}
                        startDate = new DateTime(startDate.Year, 1, 1);
                        NotWaterModel = false;
                        /* UAT Issue - Data from Mar 1st & from Mar 6th, the moisture value are not correct - Jerrey - End */
                    }
                    //if (cName.ToLower() != "gdd")
                    if (cName.ToLower() != "gdd" && cName.ToLower() != "watermodel")
                        /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                        aggregation = agriInfo["aggregation"].ToString();
                    else
                    {
                        aggregation = "daily";
                        NotWaterModel = false;
                    }
                }
                else
                {
                    startDate = current.AddDays(start);
                    //endDate = current.AddDays(end>0?end-1:end+1);                    
                    endDate = current.AddDays(end);
                }
            }
            else
            {
                startDate = current.AddDays(start);
                //endDate = current.AddDays(end >0 ? end-1:end+1);                
                endDate = current.AddDays(end);
            }
            DataRow[] dt = null;
            /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
            //if (isAgriInfo && (!bool.Parse(agriInfo["IsGDD"]) && cName.ToLower() != "gdd"))
            if (isAgriInfo && (!bool.Parse(agriInfo["IsGDD"])
                                && cName.ToLower() != "gdd" && cName.ToLower() != "watermodel"))
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                dt = series.Select("", "panel asc");
            else
                dt = series.Select("");
            enumDataSource source = (enumDataSource)Enum.Parse(typeof(enumDataSource), dataSource, true);
            enumTemporalAggregation tmpAggregation;
            switch (aggregation.ToLower())
            {
                case "daily":
                    tmpAggregation = enumTemporalAggregation.Daily;
                    valuestep = 24;
                    break;
                case "decade":
                    tmpAggregation = enumTemporalAggregation.Decade;
                    valuestep = 10;
                    break;
                case "8hourly":
                    tmpAggregation = enumTemporalAggregation.EightHourly;
                    valuestep = 8;
                    break;
                case "12hourly":
                    tmpAggregation = enumTemporalAggregation.TwelveHourly;
                    valuestep = 12;
                    break;
                case "weekly":
                    tmpAggregation = enumTemporalAggregation.Weekly;
                    valuestep = 7;
                    break;
                case "monthly":
                    tmpAggregation = enumTemporalAggregation.Monthly;
                    valuestep = 30;
                    break;
                case "hourly":
                    tmpAggregation = enumTemporalAggregation.Hourly;
                    valuestep = 1;
                    break;
                default:
                    tmpAggregation = enumTemporalAggregation.Daily;
                    valuestep = 24;
                    break;
            }

            foreach (DataRow dr in dt)
            {

                if (tmpAggregation == enumTemporalAggregation.Daily || tmpAggregation == enumTemporalAggregation.Hourly)
                    lst.Add(
                       new Serie(
                          dr["Name"].ToString()
                          , ""
                          , ((dr["Name"].ToString().ToLower().Contains("tempair") && NotWaterModel && isAgriInfo && agriInfo["altitude"].ToString() != string.Empty) ? true : false)
                          , null
                          , ""));
                else
                    lst.Add(
                     new Serie(
                        dr["Name"].ToString()
                        , ""
                        , ((dr["Name"].ToString().ToLower().Contains("tempair") && NotWaterModel && isAgriInfo && agriInfo["altitude"].ToString() != string.Empty) ? true : false)
                        , (enumAggregationFunction)Enum.Parse(typeof(enumAggregationFunction), dr["aggregationfunction"].ToString(), true)
                        , ""));
            }

            if (startDate > endDate)
            {
                DateTime tmp = startDate;
                startDate = endDate;
                endDate = tmp;
            }

            //Get the altitude from the agriinfo object
            if (isAgriInfo)
            {
                fReq = new FeatureRequest(dpi.stationLatitude, dpi.stationLongitude, agriInfo["altitude"].ToString() != string.Empty ? Int32.Parse(agriInfo["altitude"].ToString()) : 0, dpi.NearbyPointSettings.MaxAllowedAltitude, dpi.NearbyPointSettings.MaxAllowedDistance);
            }
            else
            {
                fReq = new FeatureRequest(dpi.stationLatitude, dpi.stationLongitude, dpi.altitude, dpi.NearbyPointSettings.MaxAllowedAltitude, dpi.NearbyPointSettings.MaxAllowedDistance);
            }


            if (isAgriInfo)
            {
                DateTime newStartDate = startDate;
                DateTime newEndDate = endDate;
                /*IM01166165 -New Agricast - Agriinfo GDD IssueAfter providing planting date and start date and selecting GDD an error is being thrown. -BEGIN*/
                int iDateDiff = (newEndDate - newStartDate).Days;
                /*IM01166165 -New Agricast - Agriinfo GDD IssueAfter providing planting date and start date and selecting GDD an error is being thrown. -BEGIN*/
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                //if (bool.Parse(agriInfo["IsGDD"].ToString()) && cName.ToLower() == "gdd")
                if (bool.Parse(agriInfo["IsGDD"].ToString())
                    && (cName.ToLower() == "gdd" || cName.ToLower() == "watermodel"))
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                {
                    if (agriInfo.ContainsKey("plantingDate") && agriInfo["plantingDate"] != "")
                        newStartDate = DateTime.Parse(agriInfo["plantingDate"]);
                    /*IM01166165 -New Agricast - Agriinfo GDD IssueAfter providing planting date and start date and selecting GDD an error is being thrown. -BEGIN*/
                    //newEndDate = newStartDate.AddDays(iDateDiff + 1 );
                    /*IM01166165 -New Agricast - Agriinfo GDD IssueAfter providing planting date and start date and selecting GDD an error is being thrown. -BEGIN*/
                }
                else
                {
                    int maxyear = 0;
                    int minyear = 0;
                    var year = from row in series.AsEnumerable()
                               where row.Field<string>("year") != ""
                               group row by 1 into grp
                               select new
                               {
                                   min = grp.Min(r => r.Field<string>("year")),
                                   max = grp.Max(r => r.Field<string>("year"))
                               };


                    if (year.ToList().Count > 0 && year.ToList()[0].min.ToString() != "")
                    {
                        minyear = Int32.Parse(year.ToList()[0].min.ToString());
                    }

                    if (year.ToList().Count > 0 && year.ToList()[0].max.ToString() != "")
                    {
                        maxyear = Int32.Parse(year.ToList()[0].max.ToString());
                    }

                    if (minyear != 0 && minyear < startDate.Year)
                        newStartDate = startDate.AddYears(minyear - startDate.Year);
                    if (maxyear != 0)
                    {
                        maxyear = maxyear + (endDate.Year - startDate.Year);
                        if (maxyear > endDate.Year)
                        {
                            newEndDate = endDate.AddYears(maxyear - endDate.Year);
                        }
                    }
                }                
                wReq = new WeatherDataRequest(source, false, fReq, newStartDate, (newEndDate == newStartDate ? newEndDate.AddDays(1) : newEndDate), tmpAggregation, lst);
                if (NotWaterModel)
                    wRes = wData.GetWeatherData(wReq, true, dpi.DataPoint);
                else
                wRes = wData.GetWeatherData(wReq);
            }
            else
            {
                wReq = new WeatherDataRequest(source, false, fReq, startDate, (endDate == startDate ? endDate.AddDays(1) : endDate), tmpAggregation, lst);
                wRes = wData.GetWeatherData(wReq);
            }           

            /*IM01166165 -New Agricast - Agriinfo GDD IssueAfter providing planting date and start date and selecting GDD an error is being thrown. -BEGIN*/

            if (wRes.WeatherData != null)
            {
                selectedEndDate = endDate;
            }
            //if (wRes.WeatherData.Rows.Count > 0)
            //{
            //    selectedEndDate = endDate;
            //    //if(endDate > DateTime.Parse(wRes.WeatherData.Select("date=max(date)")[0]["date"].ToString()))
            //    //  endDate = DateTime.Parse(wRes.WeatherData.Select("date=max(date)")[0]["date"].ToString());
            //}

            /*IM01166165 -New Agricast - Agriinfo GDD IssueAfter providing planting date and start date and selecting GDD an error is being thrown. -BEGIN*/

            return new Tuple<DataTable,FeatureResponse>(wRes.WeatherData,wRes.FeatureResponse);
        }
        public void setChartWebServiceValues(double dStnLat, double dStnLong, int iAtl, int iMaxDist, int iMaxAltDiff, string strSvcName, string strModule)
        {
            dStnLatitude = dStnLat;
            dStnLongitude = dStnLong;
            iAltitude = iAtl;
            iMaxAllowedDist = iMaxDist;
            iMaxAltitudeDiff = iMaxAltDiff;
            strServiceName = strSvcName;
            strModulename = strModule;
        }
        public void getPubSettings(string chartName, DataTable agriInfoData)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session == null || HttpContext.Current.Session["serviceInfo"] == null)
                {

                    objServiceInfo = new ServiceInfo();
                }
                else
                {
                    objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                }
            }

            else
            {
                objServiceInfo = new ServiceInfo();
                dpi = new DataPointInfo();

                dpi.stationLatitude = dStnLatitude;
                dpi.stationLongitude = dStnLongitude;
                dpi.altitude = iAltitude;
                dpi.NearbyPointSettings.MaxAllowedDistance = iMaxAllowedDist;
                dpi.NearbyPointSettings.MaxAllowedAltitude = iMaxAltitudeDiff;

                sh.setSvcHandlerWebSvcValues(strServiceName, strModulename);
            }

            //Web service Issue
            if (dpi == null)
            {
                dpi = new DataPointInfo();
                dpi.stationLatitude = dStnLatitude;
                dpi.stationLongitude = dStnLongitude;
                dpi.altitude = iAltitude;
                dpi.NearbyPointSettings.MaxAllowedDistance = iMaxAllowedDist;
                dpi.NearbyPointSettings.MaxAllowedAltitude = iMaxAltitudeDiff;

                sh.setSvcHandlerWebSvcValues(strServiceName, strModulename);
            }
            //end of web service change

            /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - Begin */
            /* 3.3.1	Charting component should have zooming enabled. */
            dsData.Tables.Clear();
            /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - End */

            dsData.Tables.Add(sh.getChartSettings(chartName));
            dtSource = dsData.Tables[CHART].Rows[0]["dataSource"].ToString();
            if (dsData.Tables[CHART].Columns.Contains("temporalAggregation"))
                temporalAgg = dsData.Tables[CHART].Rows[0]["temporalAggregation"].ToString();
            if (dsData.Tables[CHART].Columns.Contains("startdate"))
                _start = Int32.Parse(dsData.Tables[CHART].Rows[0]["startdate"].ToString());
            if (dsData.Tables[CHART].Columns.Contains("enddate"))
                _end = Int32.Parse(dsData.Tables[CHART].Rows[0]["enddate"].ToString());
            noofdays = Math.Abs(_end) - Math.Abs(_start);
            if (dsData.Tables[CHART].Columns.Contains("labelstep"))
                switch (dsData.Tables[CHART].Rows[0]["labelstep"].ToString())
                {
                    case "1":
                        dfactor = 1;
                        break;
                    case "24":
                        dfactor = 24;
                        break;
                    case "10":
                        dfactor = 10;
                        break;
                    case "8":
                        dfactor = 8;
                        break;
                    case "12":
                        dfactor = 12;
                        break;
                    case "7":
                        dfactor = 7;
                        break;
                    case "monthly":
                        dfactor = 30;
                        break;
                    default:
                        break;
                }
            /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
            //if (objServiceInfo.Module.ToLower().Contains("history"))
            if (objServiceInfo.Module.ToLower().Contains("history") || objServiceInfo.Module.ToLower().Contains("watermodel"))
            /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
            {
                dsData.Tables.Add(agriInfoData);
            }
            else
            {
                dsData.Tables.Add(sh.GetChartSeries(chartName));
            }
            dsData.Tables.Add(sh.GetAxes(chartName, "P"));
            dsData.Tables.Add(sh.GetAxes(chartName, "S"));


        }

        public void getChartData(DTOchart objparams, string chartName, string cultureCode, DataTable agriInfoData)
        {
            culture = cultureCode;
            objGparams = objparams;
            cName = chartName;
            getPubSettings(chartName, agriInfoData);
            var output = new Tuple<DataTable, FeatureResponse>(null, null);
            if (agriInfoData != null && agriInfoData.Rows.Count > 0)
            {
                objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                objSvcPre.ChangeUnits(agriInfoData, objServiceInfo.Unit, objServiceInfo.WUnit);
                if (HttpContext.Current.Session["IAgriInfo"] != null)
                {
                    agriInfo = (Dictionary<string, string>)HttpContext.Current.Session["IAgriInfo"];

                }
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                //if (bool.Parse(agriInfo["IsGDD"].ToString()) && cName.ToLower() == "gdd")
                if (bool.Parse(agriInfo["IsGDD"].ToString())
                    && (cName.ToLower() == "gdd" || cName.ToLower() == "watermodel"))
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                {
                    DataTable dtCombined = null;
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                    switch (cName.ToLower())
                    {
                        case "gdd":
                            //DataTable dtCombined = GDD.RulesetSeriesList;
                            dtCombined = GDD.RulesetSeriesList;
                            /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/

                            //If GDD includes non-gdd series for markers/shaders
                            //combine the datatable from agriinfo and the datatable 
                            //returned by the GDD ruleset and fetch data using the combined datatable

                            /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                            break;
                        case "watermodel":
                            dtCombined = BorderWaterModel.RulesetSeriesList;
                            break;

                    }
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/

                    if (dsData.Tables[ChartSerie].Rows.Count > 1)
                    {
                        for (int i = 1; i < dsData.Tables[ChartSerie].Rows.Count; i++)
                        {
                            DataRow dr = dtCombined.NewRow();
                            dr[0] = dsData.Tables[ChartSerie].Rows[i][0].ToString();
                            dtCombined.Rows.Add(dr);
                        }
                    }
                    output = getData(dtCombined, temporalAgg, dtSource, _start, _end, true);
                    dtData = output.Item1.Copy();

                }
                else
                {
                    output = getData(dsData.Tables[ChartSerie], temporalAgg, dtSource, _start, _end, true);
                    dtData = output.Item1.Copy();
                }

                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                // check the data table, if there is DBNull in cell value, change it to 0

                if (objServiceInfo.ServiceName != "geotroll1")
                {
                    foreach (DataRow row in dtData.Rows)
                    {
                        for (var i = 1; i < dtData.Columns.Count; i++)
                        {
                            if (row[i] == DBNull.Value)
                                row[i] = 0;
                        }
                    }
                }
                dtData.AcceptChanges();
                //if (bool.Parse(agriInfo["IsGDD"].ToString()) && cName.ToLower() == "gdd")
                if (bool.Parse(agriInfo["IsGDD"].ToString())
                         && (cName.ToLower() == "gdd" || cName.ToLower() == "watermodel"))
                /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                {
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                    switch (cName.ToLower())
                    {
                        case "gdd":
                            /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/

                            DataTable dtDataRuleset;
                            GDD.dtInput = dtData;
                            GDD.Base = Int32.Parse(agriInfo["Base"]);
                            GDD.Cap = Int32.Parse(agriInfo["Cap"]);
                            GDD.Method = agriInfo["Method"];
                            GDD.CalculateGDD();

                            dtDataRuleset = GDD.dtOutput;

                            //Merge the data returned by the ruleset columns(dtDataRuleset) and the non-gdd series(dtData)
                            DataTable targetTable = dtDataRuleset.Clone();
                            var dt2Columns = dtData.Columns.OfType<DataColumn>().Select(dc =>
                            new DataColumn(dc.ColumnName, dc.DataType, dc.Expression, dc.ColumnMapping));
                            var dt2FinalColumns = from dc in dt2Columns.AsEnumerable()
                                                  where targetTable.Columns.Contains(dc.ColumnName) == false
                                                  select dc;
                            targetTable.Columns.AddRange(dt2FinalColumns.ToArray());

                            var rowData = from row1 in dtDataRuleset.AsEnumerable()
                                          join row2 in dtData.AsEnumerable()
                                          on row1.Field<DateTime>("date") equals row2.Field<DateTime>("date")
                                          select row1.ItemArray.Concat(row2.ItemArray.Where(r2 => row1.ItemArray.Contains(r2) == false)).ToArray();
                            foreach (object[] values in rowData)
                                targetTable.Rows.Add(values);

                            dtData = targetTable;
                            dtData.Columns.Remove(GDD.RulesetSeriesList.Rows[0][0].ToString());
                            dtData.Columns.Remove(GDD.RulesetSeriesList.Rows[1][0].ToString());

                            dtData.TableName = "ChartData";
                            dsData.Tables.Add(dtData);
                            setParamGDD(dsData);

                            /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
                            break;
                        case "watermodel":

                            BorderWaterModel.dtInput = dtData;
                            BorderWaterModel.Base = Int32.Parse(agriInfo["Base"]);
                            BorderWaterModel.Cap = Int32.Parse(agriInfo["Cap"]);
                            BorderWaterModel.Method = agriInfo["Method"];

                            double TopSoilWHC = 0.0, TopSoilDepth = 0.0, SubSoilWHC = 0.0, SubSoilDepth = 0.0;
                            if (agriInfo.ContainsKey("TopSoilWHC"))
                                TopSoilWHC = double.Parse(agriInfo["TopSoilWHC"]);
                            if (agriInfo.ContainsKey("TopSoilDepth"))
                                TopSoilDepth = double.Parse(agriInfo["TopSoilDepth"]);
                            if (agriInfo.ContainsKey("SubSoilWHC"))
                                SubSoilWHC = double.Parse(agriInfo["SubSoilWHC"]);
                            if (agriInfo.ContainsKey("SubSoilDepth"))
                                SubSoilDepth = double.Parse(agriInfo["SubSoilDepth"]);

                            if (TopSoilWHC > 0 && TopSoilDepth > 0 && SubSoilWHC > 0)
                            {
                                BorderWaterModel.GridID = wRes.FeatureResponse.FeatureResponseId;
                                BorderWaterModel.Latitude = wRes.FeatureResponse.Latitude;
                                BorderWaterModel.TopSoilDepth = TopSoilDepth;
                                BorderWaterModel.TopSoilWHC = TopSoilWHC;
                                BorderWaterModel.SubSoilDepth = SubSoilDepth;
                                BorderWaterModel.SubSoilWHC = SubSoilWHC;

                                /* UAT Issue - Data from Mar 1st & from Mar 6th, the moisture value are not correct - Jerrey - Start */
                                //BorderWaterModel.StartDate = startDate;
                                BorderWaterModel.StartDate = DateTime.Parse(agriInfo["startDate"].ToString());
                                /* UAT Issue - Data from Mar 1st & from Mar 6th, the moisture value are not correct - Jerrey - End */
                                BorderWaterModel.EndDate = DateTime.Parse(agriInfo["endDate"].ToString());

                                BorderWaterModel.CalculateBWM();

                                dtData = BorderWaterModel.dtOutput;
                                dtData.TableName = "ChartData";

                                dsData.Tables.Add(dtData);
                                setParamBWM(dsData);
                            }
                            else
                            {
                                throw new System.InvalidOperationException(Constants.AGRIINFO_HAPPLY_NOSETTINGS);
                            }
                            break;
                    }
                    /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/
                }
                else
                {
                    dtData.TableName = "ChartData";
                    dsData.Tables.Add(dtData);
                    setParamsHistory(dsData, output.Item2);
                }
            }
            else
            {
                if (HttpContext.Current != null)
                {
                    // Change in units handled here for all the tables
                    foreach (DataTable dt in dsData.Tables)
                    {
                        objServiceInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                        objSvcPre.ChangeUnits(dt, objServiceInfo.Unit, objServiceInfo.WUnit);
                    }
                }
                dtData = getData(dsData.Tables[ChartSerie], temporalAgg, dtSource, _start, _end, false).Item1.Copy();

                dtData.TableName = "ChartData";
                dsData.Tables.Add(dtData);
                setParams(dsData);
            }
        }

        public DataTable getChartDataForExport(DTOchart objparams, string chartName, string cultureCode, DataTable agriInfoData)
        {
            culture = cultureCode;
            objGparams = objparams;
            getPubSettings(chartName, agriInfoData);
            if (agriInfoData != null)
                dtData = getData(dsData.Tables[ChartSerie], temporalAgg, dtSource, _start, _end, true).Item1.Copy();
            else
                dtData = getData(dsData.Tables[ChartSerie], temporalAgg, dtSource, _start, _end, false).Item1.Copy();

            dtData.TableName = "ChartData";

            return dtData;
        }
        private void setParams(DataSet config)
        {
            objGparams.Height = Int32.Parse(config.Tables[CHART].Rows[0]["height"].ToString());
            objGparams.Width = Int32.Parse(config.Tables[CHART].Rows[0]["width"].ToString());
            objGparams.WaterMark = cUtil.getApplPath(@"Images\") + config.Tables[CHART].Rows[0]["watermark"].ToString();
            objGparams.TodayMarker = (bool)(config.Tables[CHART].Rows[0]["todaymarker"]);
            //to enable or diable feedback
            objGparams.HasFeedback = bool.Parse(config.Tables[CHART].Rows[0]["feedback"].ToString());

            int cols = 1;
            //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - start
            for (int i = config.Tables[ChartSerie].Rows.Count-1; i > -1; i--)
            {
                if (config.Tables[ChartSerie].Columns.Contains("isInvisible") ? bool.Parse(config.Tables[ChartSerie].Rows[i]["isInvisible"].ToString()) : false)
                {
                    config.Tables[ChartSerie].Rows[i].Delete();
                }
            }
            //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - end
            foreach (DataRow dr in config.Tables[ChartSerie].Rows)
            {
                series = new Series();
                if (dr.Table.Columns.Contains("trnsTag"))
                    series.SerieName = cUtil.getTransText(dr["trnsTag"].ToString());
                else
                    series.SerieName = dr["name"].ToString();
                series.MarkerType = dr["markerType"].ToString();
                series.Position = dr["axisPosition"].ToString();
                series.Stacked = Convert.ToBoolean(dr["stacked"]);
                series.Gallery = dr["gallery"].ToString();
                series.Color = dr["color"].ToString();
                series.pane = dr.Table.Columns.Contains("panel") ? Int32.Parse(dr["panel"].ToString()) : 1;
                series.inverted = dr.Table.Columns.Contains("inverted") ? bool.Parse(dr["inverted"].ToString()) : false;
                valuesperday = 24 / valuestep;
                noofvalues = (Math.Abs(_end) - Math.Abs(_start)) * valuesperday;
                double?[] vals = new double?[noofvalues];
                for (int i = 0; i < noofvalues; i++)
                {
                    double j;
                    if (i < config.Tables["ChartData"].Rows.Count && config.Tables["ChartData"].Rows[i][cols].ToString() != "")
                    {
                        vals[i] = double.TryParse(config.Tables["ChartData"].Rows[i][cols].ToString(), out j) ? j : 0;
                    }
                    else
                    {
                        missingData = true;
                        series.hasGaps = true;
                    }

                }
                if (series.Position.ToLower() == "primary")
                {
                    series.MinorY = double.Parse(config.Tables[PrimaryAxis].Rows[0]["minValue"].ToString());
                    series.MajorY = double.Parse(config.Tables[PrimaryAxis].Rows[0]["maxvalue"].ToString());
                    series.Scale = config.Tables[PrimaryAxis].Rows[0]["ScaleType"].ToString();
                    calcMaxY = (vals.Max() ?? 0d) > calcMaxY ? (vals.Max() ?? 0d) : calcMaxY;
                    calcMinY = (vals.Min() ?? 0d) < calcMinY ? (vals.Min() ?? 0d) : calcMinY;
                }
                else
                {
                    series.MinorY = double.Parse(config.Tables[SecondaryAxis].Rows[0]["minValue"].ToString());
                    series.MajorY = double.Parse(config.Tables[SecondaryAxis].Rows[0]["maxvalue"].ToString());
                    series.Scale = config.Tables[SecondaryAxis].Rows[0]["ScaleType"].ToString();
                    calcMaxY2 = (vals.Max() ?? 0d) > calcMaxY2 ? (vals.Max() ?? 0d) : calcMaxY2;
                    calcMinY2 = (vals.Min() ?? 0d) < calcMinY2 ? (vals.Min() ?? 0d) : calcMinY2;
                }
                series.values = vals;
                objGparams.addSeries(series);
                cols++;
            }

            objGparams.LeftScaleType = config.Tables[PrimaryAxis].Rows[0]["scaleType"].ToString();
            objGparams.LeftAxisLabel = cUtil.getTransText(config.Tables[PrimaryAxis].Rows[0]["trnsTag"].ToString());
            objGparams.Ymax = double.Parse(config.Tables[PrimaryAxis].Rows[0]["maxValue"].ToString());
            objGparams.Ymin = double.Parse(config.Tables[PrimaryAxis].Rows[0]["minValue"].ToString());


            objGparams.RightAxisLabel = cUtil.getTransText(config.Tables[SecondaryAxis].Rows[0]["trnsTag"].ToString());
            objGparams.RightScaleType = config.Tables[SecondaryAxis].Rows[0]["scaletype"].ToString();
            objGparams.Y2max = double.Parse(config.Tables[SecondaryAxis].Rows[0]["maxValue"].ToString());
            objGparams.Y2min = double.Parse(config.Tables[SecondaryAxis].Rows[0]["minValue"].ToString());
            //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - start
            objGparams.isInvisible = config.Tables[SecondaryAxis].Columns.Contains("isInvisible") ? (bool)(config.Tables[SecondaryAxis].Rows[0]["isInvisible"]) : false;
            //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - end
            if (objServiceInfo != null && objServiceInfo.Unit.ToString().ToLower() == "imperial")
            {
                if (config.Tables[PrimaryAxis].Rows[0]["maxValueImperialSpecified"].ToString().ToLower() == "true" && config.Tables[PrimaryAxis].Rows[0]["minValueImperialSpecified"].ToString().ToLower() == "true")
                {
                    objGparams.Ymax = double.Parse(config.Tables[PrimaryAxis].Rows[0]["maxValueImperial"].ToString());
                    objGparams.Ymin = double.Parse(config.Tables[PrimaryAxis].Rows[0]["minValueImperial"].ToString());
                }
                else
                {
                    objGparams.Ymax = (objGparams.Ymax > calcMaxY) ? objGparams.Ymax : calcMaxY;
                    objGparams.Ymin = (objGparams.Ymin < calcMinY) ? objGparams.Ymin : calcMinY;
                }
                if (config.Tables[SecondaryAxis].Rows[0]["maxValueImperialSpecified"].ToString().ToLower() == "true" && config.Tables[SecondaryAxis].Rows[0]["maxValueImperialSpecified"].ToString().ToLower() == "true")
                {
                    objGparams.Y2max = double.Parse(config.Tables[SecondaryAxis].Rows[0]["maxValueImperial"].ToString());
                    objGparams.Y2min = double.Parse(config.Tables[SecondaryAxis].Rows[0]["minValueImperial"].ToString());
                }
                else
                {
                    objGparams.Y2max = (objGparams.Y2max > calcMaxY2) ? objGparams.Y2max : calcMaxY2;
                    objGparams.Y2min = (objGparams.Y2min < calcMinY2) ? objGparams.Y2min : calcMinY2;
                }
            }

            AddMarkers();
            objGparams.BottomLabels = getLabels(startDate);
            getTopAxisLabels();
            if (config.Tables[CHART].Columns.Contains("PlotShades") && bool.Parse(config.Tables[CHART].Rows[0]["PlotShades"].ToString()))
                getSunriseSunsetAndShaders();
            ylabels();
            objGparams.Title = cUtil.getTransText("wc_titlefiveinone");

            if (missingData)
            {
                //objGparams.FooterText = HttpUtility.HtmlDecode("&#9888;") +cUtil.getTransText("wm_warning") + ":" + cUtil.getTransText(" This chart has been plotted with missing data.") + "\n \n";
                objGparams.warning = cUtil.getTransText("wm_warning") + ":" + cUtil.getTransText("This chart has been plotted with missing data.");
            }

            //objGparams.FooterText += cUtil.getTransText("chartcopyright");
            objGparams.minorStep = 1.0 / 24.0;
            int step = dfactor;
            if (step > 0) objGparams.majorStep = (double)step / 24.0;
        }
        void setParamsHistory(DataSet config, FeatureResponse featureResponse = null)
        {
            objGparams.Height = Int32.Parse(config.Tables[CHART].Rows[0]["height"].ToString());
            objGparams.Width = Int32.Parse(config.Tables[CHART].Rows[0]["width"].ToString());
            objGparams.WaterMark = cUtil.getApplPath(@"Images\") + config.Tables[CHART].Rows[0]["watermark"].ToString();
            int cols = 1;
            foreach (DataRow dr in config.Tables[ChartSerie].Select("", "panel"))
            {
                int rowCount = 0;
                series = new Series();
                string extras = dr["addlInfo"].ToString() != "" ? " (" + dr["addlInfo"].ToString().TrimStart(',') + ")" : "";
                if (dr.Table.Columns.Contains("trnsTag"))
                    series.SerieName = cUtil.getTransText(dr["trnsTag"].ToString()) + extras;
                else
                    series.SerieName = dr["name"].ToString() + extras;
                series.MarkerType = dr["markerType"].ToString();
                series.Position = dr["axisPosition"].ToString();
                series.Stacked = Convert.ToBoolean(dr["stacked"]);
                series.Gallery = dr["gallery"].ToString();
                series.Color = dr["color"].ToString();
                series.pane = dr.Table.Columns.Contains("panel") ? Int32.Parse(dr["panel"].ToString()) : 1;
                series.axisLabel = (dr.Table.Columns.Contains("labeltext") && dr["labeltext"].ToString() != "") ? cUtil.getTransText(dr["labeltext"].ToString()) : "";
                /* IM01288870 - New Agricast - Missing data - Jerrey - Begin */
                //noofvalues = ((TimeSpan)(endDate - startDate)).Days / (valuestep == 24 ? 1 : valuestep); 
                //noofvalues = (valuestep == 30 && noofvalues > 0) ? noofvalues : noofvalues + 1;
                noofvalues = ((TimeSpan)(endDate.AddDays(1) - startDate)).Days / (valuestep == 24 ? 1 : valuestep);
                /* IM01288870 - New Agricast - Missing data - Jerrey - End */
                double?[] vals = new double?[noofvalues];
                DataRow[] dt = null;
                DateTime from = startDate;
                DateTime to = endDate;
                if (dr["year"].ToString() != "")
                {
                    from = new DateTime(Int32.Parse(dr["year"].ToString()), from.Month, (from.Month == 2 && from.Day > 28) ? 28 : from.Day);
                    int range = (endDate.Year - startDate.Year);
                    to = new DateTime(Int32.Parse(dr["year"].ToString()) + range, to.Month, (to.Month == 2 && to.Day > 28) ? 28 : to.Day);
                    if (range != 0)
                        series.SerieName = series.SerieName + " " + dr["year"].ToString() + "-" + (Int32.Parse(dr["year"].ToString()) + range).ToString();
                    else
                        series.SerieName = series.SerieName + " " + dr["year"].ToString();
                    if (to > DateTime.Parse(config.Tables["ChartData"].Select("date=max(date)")[0]["date"].ToString()))
                    {
                        to = DateTime.Parse(config.Tables["ChartData"].Select("date=max(date)")[0]["date"].ToString());
                    }
                }
                dt = config.Tables["ChartData"].Select("date>='" + from + "' and date<='" + to + "'", "date asc");
                if (dt != null && dt.Count() != 0)
                {
                    if (series.MarkerType.ToString().ToLower() == "marker")
                    {
                        ArrayList mks = new ArrayList();
                        ResultMarker marker = null;
                        dswidth = "1";
                        int barwidth = int.Parse(dswidth);
                        string color = dr.Table.Columns.Contains("textColor") ? dr["textColor"].ToString() : Color.Black.Name;
                        for (int x = 0; x < dt.Length; x++)
                        {
                            if (dt[x][cols].ToString() != "")
                            {
                                marker = new ResultMarker(DateTime.Parse(dt[x][0].ToString()).ToOADate(), 0, color, dt[x][cols].ToString());
                                marker.width = barwidth;
                                //marker.label = dt[x][cols].ToString();                                     
                                mks.Add(marker);
                            }
                        }
                        objGparams.markers = (ResultMarker[])mks.ToArray(marker.GetType());
                    }
                    else if (series.MarkerType.ToString().ToLower() == "shader")
                    {

                        ResultShade rsh = null;
                        ArrayList shades = new ArrayList();
                        PaletteMap objPm = new PaletteMap();
                        objPm = getPalleteColor(dr["pallete"].ToString());
                        DateTime cdate = startDate;
                        double shadestart = cdate.ToOADate();
                        double shadestop = cdate.ToOADate();
                        double preval = 0d;
                        double curval = 0d;
                        for (int i = 0; i <= dt.Length; i++)
                        {
                            string color = Color.Red.Name;
                            if (i == 0)
                            {
                                if (!string.IsNullOrEmpty(dt[i][cols].ToString()))
                                    curval = double.Parse(dt[i][cols].ToString());
                                shadestart = DateTime.Parse(dt[i][0].ToString()).ToOADate();
                                preval = curval;
                            }
                            if (i < dt.Length && !string.IsNullOrEmpty(dt[i][cols].ToString()))
                                curval = double.Parse(dt[i][cols].ToString());
                            else
                                curval = 0d;

                            if (preval != curval)
                            {
                                shadestop = DateTime.Parse(dt[i - 1][0].ToString()).ToOADate();
                                if (dr.Table.Columns.Contains("pallete"))
                                {
                                    color = !string.IsNullOrEmpty(preval.ToString()) ? objPm.getColor(preval, "") : Color.Red.Name;
                                }

                                rsh = new ResultShade(shadestart, shadestop, color);
                                shades.Add(rsh);
                                shadestart = 0d;
                                shadestop = 0d;
                                shadestart = DateTime.Parse(dt[i - 1][0].ToString()).ToOADate();
                                preval = curval;
                            }
                        }
                        if (shades.Count > 0) objGparams.shades = (ResultShade[])shades.ToArray(rsh.GetType());
                    }
                    else
                    {
                        int i = 0;
                        from = DateTime.Parse(dt[0]["date"].ToString());
                        /* IM01288870 - New Agricast - Missing data - Jerrey - Begin */
                        //while (from <= to)
                        while (from <= to.AddDays(1).AddDays(-(valuestep == 24 ? 1 : valuestep)))
                        /* IM01288870 - New Agricast - Missing data - Jerrey - End */
                        {
                            double j;
                            if (dt.Length > rowCount && dt[rowCount][cols].ToString() != "" && vals.Length > i)
                            {
                                vals[i] = double.TryParse(dt[rowCount][cols].ToString(), out j) ? j : 0;
                                series.hasvalues = true;
                            }
                            else
                            {
                                missingData = true;
                                series.hasGaps = true;
                            }
                            rowCount++;
                            //}
                            from = from.AddDays(valuestep == 24 ? 1 : valuestep);
                            i++;
                        }
                    }
                }
                if (series.Position.ToLower() == "primary")
                {
                    series.MinorY = double.Parse(config.Tables[PrimaryAxis].Rows[0]["minValue"].ToString());
                    series.MajorY = double.Parse(config.Tables[PrimaryAxis].Rows[0]["maxvalue"].ToString());
                    series.Scale = config.Tables[PrimaryAxis].Rows[0]["ScaleType"].ToString();
                }
                else
                {
                    series.MinorY = double.Parse(config.Tables[SecondaryAxis].Rows[0]["minValue"].ToString());
                    series.MajorY = double.Parse(config.Tables[SecondaryAxis].Rows[0]["maxvalue"].ToString());
                    series.Scale = config.Tables[SecondaryAxis].Rows[0]["ScaleType"].ToString();
                }
                series.values = vals;
                objGparams.addSeries(series);
                cols++;
            }

            objGparams.LeftScaleType = config.Tables[PrimaryAxis].Rows[0]["scaleType"].ToString();
            objGparams.LeftAxisLabel = cUtil.getTransText(config.Tables[PrimaryAxis].Rows[0]["trnsTag"].ToString());
            objGparams.Ymax = double.Parse(config.Tables[PrimaryAxis].Rows[0]["maxValue"].ToString());
            objGparams.Ymin = double.Parse(config.Tables[PrimaryAxis].Rows[0]["minValue"].ToString());


            objGparams.RightAxisLabel = cUtil.getTransText(config.Tables[SecondaryAxis].Rows[0]["trnsTag"].ToString());
            objGparams.RightScaleType = config.Tables[SecondaryAxis].Rows[0]["scaletype"].ToString();
            objGparams.Y2max = double.Parse(config.Tables[SecondaryAxis].Rows[0]["maxValue"].ToString());
            objGparams.Y2min = double.Parse(config.Tables[SecondaryAxis].Rows[0]["minValue"].ToString());

            objGparams.BottomLabels = getLabelsHistory(startDate, selectedEndDate, valuestep);
            string distText = cUtil.getTransText("ResDistanceFrom");
            string stationInfo = dpi.stationName.ToString();
            string lattranslate = objSvcPre.getTranslatedText("Lat", culture);
            string longtranslate = objSvcPre.getTranslatedText("Long", culture);
            stationInfo = stationInfo.Replace("Lat", lattranslate);
            stationInfo = stationInfo.Replace("Long", longtranslate);
            if (featureResponse != null && dpi.stationLatitude != featureResponse.Latitude && dpi.stationLongitude != featureResponse.Longitude)
            {
                distText = distText.Replace("{Dist}", featureResponse.Distance.ToString());
                distText = distText.Replace("{Dir}", cUtil.getTextDirection(Convert.ToInt32(featureResponse.BearingDegrees)));
                distText = distText.Replace("{Elevation}", featureResponse.Altitude.ToString());
                stationInfo = lattranslate + ":" + featureResponse.Latitude + ";" + longtranslate + ":" + featureResponse.Longitude;
            }
            else
            {
                distText = distText.Replace("{Dist}", dpi.distance.ToString());
                distText = distText.Replace("{Dir}", dpi.directionLetter);
                distText = distText.Replace("{Elevation}", dpi.altitude.ToString());
            }
           
            distText = distText.Replace("{CityName}", HttpUtility.HtmlDecode((LocationInfo.getLocationInfoObj).placeName));           
            distText = distText.Replace("<strong>", "").Replace("</strong>", "");

            /*IM01166162 - AgriInfo UI Issues - BEGIN*/
            //objGparams.Title = cUtil.getTransText("weather") + " " + cUtil.getTransText("for") + " " + dpi.stationName.ToString() +" "+ distText + "\n" +
            //                    cUtil.getTransText("from")+ " " + startDate.ToString("dd-MMM-yyyy", new CultureInfo(culture)) + " " +
            //                    cUtil.getTransText("to") + " " + selectedEndDate.ToString("dd-MMM-yyyy", new CultureInfo(culture)) + " " + cUtil.getTransText("aggregation")+" :"+ cUtil.getTransText(agriInfo["aggregation"]);
            /*IM01848085 - New Agricast - Agriinfo - "location" name not translatable - BEGIN*/
            //objGparams.Title = cUtil.getTransText("weather") + " " + cUtil.getTransText("for") + " " + dpi.stationName.ToString() + "#" + distText + "#"
            
            /* IM01848087 - New Agricast - AgriInfo chart title - changing the structure of the lines - Start */
            if (culture == "hu-HU")
            {
                //objGparams.Title = cUtil.getTransText("weather") + " " + cUtil.getTransText("for") + " " + stationInfo + "#" + distText + "#"
                //    /*IM01848085 - New Agricast - Agriinfo - "location" name not translatable - END*/
                //    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - Start */
                //                    + FormatDateTime(startDate, culture) + " " + cUtil.getTransText("from") //startDate.ToString("dd-MMM-yyyy", new CultureInfo(culture)) 
                //                    + " " + FormatDateTime(selectedEndDate, culture) + " " + cUtil.getTransText("to") //selectedEndDate.ToString("dd-MMM-yyyy", new CultureInfo(culture))             
                //    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - End */
                //                    + " " + cUtil.getTransText("Aggregation") + ": " + cUtil.getTransText(agriInfo["aggregation"]);
                objGparams.Title = cUtil.getTransText("weather") + " " + cUtil.getTransText("for") + " " + stationInfo + "#" + distText;
            }
            else
            {
                //objGparams.Title = cUtil.getTransText("weather") + " " + cUtil.getTransText("for") + " " + stationInfo + "#" + distText + "#"
                //    /*IM01848085 - New Agricast - Agriinfo - "location" name not translatable - END*/
                //    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - Start */
                //    + cUtil.getTransText("from") + " " + FormatDateTime(startDate, culture) //startDate.ToString("dd-MMM-yyyy", new CultureInfo(culture)) 
                //    + " " + cUtil.getTransText("to") + " " + FormatDateTime(selectedEndDate, culture) //selectedEndDate.ToString("dd-MMM-yyyy", new CultureInfo(culture))
                //    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - End */
                //    + " " + cUtil.getTransText("Aggregation") + ": " + cUtil.getTransText(agriInfo["aggregation"]);
                objGparams.Title = cUtil.getTransText("weather") + " " + cUtil.getTransText("for") + " " + stationInfo + "#" + distText;
            }
            /* IM01848087 - New Agricast - AgriInfo chart title - changing the structure of the lines - End */
            /*IM01166162 - AgriInfo UI Issues - END*/
            if (missingData)
                objGparams.warning = cUtil.getTransText("wm_warning") + ":" + cUtil.getTransText("This chart has been plotted with missing data.");
           // objGparams.FooterText += cUtil.getTransText("chartcopyright");

        }
        void setParamGDD(DataSet config)
        {
            objGparams.Height = Int32.Parse(config.Tables[CHART].Rows[0]["height"].ToString());
            objGparams.Width = Int32.Parse(config.Tables[CHART].Rows[0]["width"].ToString());
            objGparams.WaterMark = cUtil.getApplPath(@"Images\") + config.Tables[CHART].Rows[0]["watermark"].ToString();
            // objGparams.TodayMarker = (bool)(config.Tables[CHART].Rows[0]["todaymarker"]);
            int cols = 1;
            // DataRow drow = config.Tables[ChartSerie].Rows[0];
            // drow["name"] = "GDD";

            foreach (DataRow dr in config.Tables[ChartSerie].Select("", "panel"))
            {
                int rowCount = 0;
                series = new Series();
                string extras = dr["addlInfo"].ToString() != "" ? "   (" + dr["addlInfo"].ToString().TrimStart(',') + ")" : "";
                if (dr.Table.Columns.Contains("trnsTag"))
                    series.SerieName = cUtil.getTransText(dr["trnsTag"].ToString()) + extras;
                else
                    series.SerieName = dr["name"].ToString() + extras;
                series.MarkerType = dr["markerType"].ToString();
                series.Position = dr["axisPosition"].ToString();
                series.Stacked = Convert.ToBoolean(dr["stacked"]);
                series.Gallery = dr["gallery"].ToString();
                series.Color = dr["color"].ToString();
                series.pane = dr.Table.Columns.Contains("panel") ? Int32.Parse(dr["panel"].ToString()) : 1;
                series.axisLabel = (dr.Table.Columns.Contains("labeltext") && dr["labeltext"].ToString() != "") ? cUtil.getTransText(dr["labeltext"].ToString()) : "";
                noofvalues = ((TimeSpan)(endDate - startDate)).Days / (valuestep == 24 ? 1 : valuestep);
                /* IM01288870 - New Agricast - Missing data - Jerrey - Begin */
                //noofvalues = ((TimeSpan)(endDate - startDate)).Days / (valuestep == 24 ? 1 : valuestep); 
                //noofvalues = (valuestep == 30 && noofvalues > 0) ? noofvalues : noofvalues + 1;
                noofvalues = ((TimeSpan)(endDate.AddDays(1) - startDate)).Days / (valuestep == 24 ? 1 : valuestep);
                /* IM01288870 - New Agricast - Missing data - Jerrey - End */
                double?[] vals = new double?[noofvalues];
                DataRow[] dt = null;
                DateTime from = startDate;
                DateTime to = endDate;

                dt = config.Tables["ChartData"].Select("date>='" + from + "' and date<='" + to + "'", "date asc");
                if (dt != null && dt.Count() != 0)
                {
                    if (series.MarkerType.ToString().ToLower() == "marker")
                    {
                        ArrayList mks = new ArrayList();
                        ResultMarker marker = null;
                        dswidth = "1";
                        int barwidth = int.Parse(dswidth);
                        string color = dr.Table.Columns.Contains("textColor") ? dr["textColor"].ToString() : Color.Black.Name;
                        for (int x = 0; x < dt.Length; x++)
                        {
                            if (dt[x][cols].ToString() != "")
                            {
                                marker = new ResultMarker(DateTime.Parse(dt[x][0].ToString()).ToOADate(), 0, color, dt[x][cols].ToString());
                                marker.width = barwidth;
                                //marker.label = dt[x][cols].ToString();                                     
                                mks.Add(marker);
                            }
                        }
                        objGparams.markers = (ResultMarker[])mks.ToArray(marker.GetType());
                    }
                    else if (series.MarkerType.ToString().ToLower() == "shader")
                    {

                        ResultShade rsh = null;
                        ArrayList shades = new ArrayList();
                        PaletteMap objPm = new PaletteMap();
                        objPm = getPalleteColor(dr["pallete"].ToString());
                        DateTime cdate = startDate;
                        double shadestart = cdate.ToOADate();
                        double shadestop = cdate.ToOADate();
                        double preval = 0d;
                        double curval = 0d;
                        for (int i = 0; i <= dt.Length; i++)
                        {
                            string color = Color.Red.Name;
                            if (i == 0)
                            {
                                if (!string.IsNullOrEmpty(dt[i][cols].ToString()))
                                    curval = double.Parse(dt[i][cols].ToString());
                                shadestart = DateTime.Parse(dt[i][0].ToString()).ToOADate();
                                preval = curval;
                            }
                            if (i < dt.Length && !string.IsNullOrEmpty(dt[i][cols].ToString()))
                                curval = double.Parse(dt[i][cols].ToString());
                            else
                                curval = 0d;

                            if (preval != curval)
                            {
                                shadestop = DateTime.Parse(dt[i - 1][0].ToString()).ToOADate();
                                if (dr.Table.Columns.Contains("pallete"))
                                {
                                    color = !string.IsNullOrEmpty(preval.ToString()) ? objPm.getColor(preval, "") : Color.Red.Name;
                                }

                                rsh = new ResultShade(shadestart, shadestop, color);
                                shades.Add(rsh);
                                shadestart = 0d;
                                shadestop = 0d;
                                shadestart = DateTime.Parse(dt[i - 1][0].ToString()).ToOADate();
                                preval = curval;
                            }
                        }
                        if (shades.Count > 0) objGparams.shades = (ResultShade[])shades.ToArray(rsh.GetType());
                    }
                    else
                    {
                        int i = 0;
                        /* IM01288870 - New Agricast - Missing data - Jerrey - Begin */
                        //while (from <= to)
                        while (from <= to.AddDays(1).AddDays(-(valuestep == 24 ? 1 : valuestep)))
                        /* IM01288870 - New Agricast - Missing data - Jerrey - End */
                        {
                            double j;
                            if (bool.Parse(agriInfo["IsGDD"]))
                                if (from != DateTime.Parse(dt[rowCount][0].ToString()))
                                {
                                    from = from.AddDays(valuestep == 24 ? 1 : valuestep);
                                    i++;
                                    continue;
                                }
                            if (dt.Length > rowCount && dt[rowCount][cols].ToString() != "" && vals.Length > i)
                            {
                                vals[i] = double.TryParse(dt[rowCount][cols].ToString(), out j) ? j : 0;
                                series.hasvalues = true;
                            }
                            else
                            {
                                missingData = true;
                                series.hasGaps = true;
                            }
                            rowCount++;
                            //}
                            from = from.AddDays(valuestep == 24 ? 1 : valuestep);
                            i++;
                        }
                    }
                }
                if (series.Position.ToLower() == "primary")
                {
                    series.MinorY = double.Parse(config.Tables[PrimaryAxis].Rows[0]["minValue"].ToString());
                    series.MajorY = double.Parse(config.Tables[PrimaryAxis].Rows[0]["maxvalue"].ToString());
                    series.Scale = config.Tables[PrimaryAxis].Rows[0]["ScaleType"].ToString();
                }
                else
                {
                    series.MinorY = double.Parse(config.Tables[SecondaryAxis].Rows[0]["minValue"].ToString());
                    series.MajorY = double.Parse(config.Tables[SecondaryAxis].Rows[0]["maxvalue"].ToString());
                    series.Scale = config.Tables[SecondaryAxis].Rows[0]["ScaleType"].ToString();
                }
                series.values = vals;
                objGparams.addSeries(series);
                cols++;
            }

            objGparams.LeftScaleType = config.Tables[PrimaryAxis].Rows[0]["scaleType"].ToString();
            objGparams.LeftAxisLabel = cUtil.getTransText(config.Tables[PrimaryAxis].Rows[0]["trnsTag"].ToString());
            objGparams.Ymax = double.Parse(config.Tables[PrimaryAxis].Rows[0]["maxValue"].ToString());
            objGparams.Ymin = double.Parse(config.Tables[PrimaryAxis].Rows[0]["minValue"].ToString());


            objGparams.RightAxisLabel = cUtil.getTransText(config.Tables[SecondaryAxis].Rows[0]["trnsTag"].ToString());
            objGparams.RightScaleType = config.Tables[SecondaryAxis].Rows[0]["scaletype"].ToString();
            objGparams.Y2max = double.Parse(config.Tables[SecondaryAxis].Rows[0]["maxValue"].ToString());
            objGparams.Y2min = double.Parse(config.Tables[SecondaryAxis].Rows[0]["minValue"].ToString());

            objGparams.BottomLabels = getLabelsHistory(startDate, selectedEndDate, valuestep);
            string distText = cUtil.getTransText("ResDistanceFrom");
            distText = distText.Replace("{Dist}", dpi.distance.ToString());
            distText = distText.Replace("{Dir}", dpi.directionLetter);
            distText = distText.Replace("{CityName}", HttpUtility.HtmlDecode((LocationInfo.getLocationInfoObj).placeName));
            distText = distText.Replace("{Elevation}", dpi.altitude.ToString());
            distText = distText.Replace("<strong>", "").Replace("</strong>", "");

            /*IM01166162 - AgriInfo UI Issues - BEGIN*/
            //objGparams.Title = cUtil.getTransText("weather") + " " + cUtil.getTransText("for") + " " + dpi.stationName.ToString() + " " + distText + "\n" +
            //                    cUtil.getTransText("from") + " " + startDate.ToString("dd-MMM-yyyy", new CultureInfo(culture)) + " " +
            //                    cUtil.getTransText("to") + " " + selectedEndDate.ToString("dd-MMM-yyyy", new CultureInfo(culture)) + " " + cUtil.getTransText("aggregation") + " :" + cUtil.getTransText(agriInfo["aggregation"]);
            /*IM01848085 - New Agricast - Agriinfo - "location" name not translatable - BEGIN*/
            //objGparams.Title = cUtil.getTransText("weather") + " "
            //                    + cUtil.getTransText("for") + " "
            //                    + dpi.stationName.ToString() + " " + distText + "#"
            string stationInfo = dpi.stationName.ToString();
            string lattranslate = objSvcPre.getTranslatedText("Lat", culture);
            string longtranslate = objSvcPre.getTranslatedText("Long", culture);
            stationInfo = stationInfo.Replace("Lat", lattranslate);
            stationInfo = stationInfo.Replace("Long", longtranslate);
            /* IM01848087 - New Agricast - AgriInfo chart title - changing the structure of the lines - Start */
            if (culture == "hu-HU")
            {
                objGparams.Title = cUtil.getTransText("weather") + " "
                                    + cUtil.getTransText("for") + " "
                                    + stationInfo + " " + distText + "#"
                    /*IM01848085 - New Agricast - Agriinfo - "location" name not translatable - END*/

                    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - Start */
                                    + FormatDateTime(startDate, culture) + " " + cUtil.getTransText("from") //startDate.ToString("dd-MMM-yyyy", new CultureInfo(culture))
                                    + " " + FormatDateTime(selectedEndDate, culture) + " " + cUtil.getTransText("to") //selectedEndDate.ToString("dd-MMM-yyyy", new CultureInfo(culture))
                    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - End */
                                    + " " + cUtil.getTransText("aggregation") + " :" + cUtil.getTransText(agriInfo["aggregation"]);
            }
            else
            {
                objGparams.Title = cUtil.getTransText("weather") + " "
                                    + cUtil.getTransText("for") + " "
                                    + stationInfo + " " + distText + "#"
                    /*IM01848085 - New Agricast - Agriinfo - "location" name not translatable - END*/

                    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - Start */
                    + cUtil.getTransText("from") + " " + FormatDateTime(startDate, culture) //startDate.ToString("dd-MMM-yyyy", new CultureInfo(culture))
                    + " " + cUtil.getTransText("to") + " " + FormatDateTime(selectedEndDate, culture) //selectedEndDate.ToString("dd-MMM-yyyy", new CultureInfo(culture))
                    /* IM01289657 - New Agricast - Agriinfo - date formatting - Jerrey - End */
                                    + " " + cUtil.getTransText("aggregation") + " :" + cUtil.getTransText(agriInfo["aggregation"]);
            }
            /* IM01848087 - New Agricast - AgriInfo chart title - changing the structure of the lines - End */
            /*IM01166162 - AgriInfo UI Issues - END*/
            if (missingData)
                objGparams.warning = cUtil.getTransText("wm_warning") + ":" + cUtil.getTransText("This chart has been plotted with missing data.");
            //objGparams.FooterText += cUtil.getTransText("chartcopyright");

        }

        /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - Start*/
        private void AddSeriesToChartParams(DataSet config, DataRow dr, string serieName, string axisLabel, string gallery, string position, string markerType, string serieColor, string columnName, int pane)
        {
            int rowCount = 0;
            series = new Series();
            series.SerieName = serieName;
            series.Gallery = gallery;
            series.pane = pane;
            series.Color = serieColor;
            series.MarkerType = markerType;
            series.Position = position;
            series.axisLabel = axisLabel;

            series.Stacked = Convert.ToBoolean(dr["stacked"]);
            /* UAT Issue - Data from Mar 1st & from Mar 6th, the moisture value are not correct - Jerrey - Start */
            startDate = BorderWaterModel.StartDate;
            endDate = BorderWaterModel.EndDate;
            //endDate = DateTime.Parse(agriInfo["endDate"].ToString());
            /* UAT Issue - Data from Mar 1st & from Mar 6th, the moisture value are not correct - Jerrey - End */

            /* IM01288870 - New Agricast - Missing data - Jerrey - Begin */
            //noofvalues = ((TimeSpan)(endDate - startDate)).Days / (valuestep == 24 ? 1 : valuestep); 
            //noofvalues = (valuestep == 30 && noofvalues > 0) ? noofvalues : noofvalues + 1;
            noofvalues = ((TimeSpan)(endDate.AddDays(1) - startDate)).Days / (valuestep == 24 ? 1 : valuestep);
            /* IM01288870 - New Agricast - Missing data - Jerrey - End */
            double?[] vals = new double?[noofvalues];

            DataRow[] dt = null;
            DateTime from = startDate;
            DateTime to = endDate;

            dt = config.Tables["ChartData"].Select("date>='" + from + "' and date<='" + to + "'", "date asc");
            if (dt != null && dt.Count() != 0)
            {
                if (series.MarkerType.ToString().ToLower() == "marker")
                {
                    ArrayList mks = new ArrayList();
                    ResultMarker marker = null;
                    dswidth = "1";
                    int barwidth = int.Parse(dswidth);
                    string color = dr.Table.Columns.Contains("textColor") ? dr["textColor"].ToString() : Color.Black.Name;
                    for (int x = 0; x < dt.Length; x++)
                    {
                        if (dt[x][columnName].ToString() != "")
                        {
                            marker = new ResultMarker(DateTime.Parse(dt[x][0].ToString()).ToOADate(), 0, color, dt[x][columnName].ToString());
                            marker.width = barwidth;
                            mks.Add(marker);
                        }
                    }
                    objGparams.markers = (ResultMarker[])mks.ToArray(marker.GetType());
                }
                else if (series.MarkerType.ToString().ToLower() == "shader")
                {

                    ResultShade rsh = null;
                    ArrayList shades = new ArrayList();
                    PaletteMap objPm = new PaletteMap();
                    objPm = getPalleteColor(dr["pallete"].ToString());
                    DateTime cdate = startDate;
                    double shadestart = cdate.ToOADate();
                    double shadestop = cdate.ToOADate();
                    double preval = 0d;
                    double curval = 0d;
                    for (int i = 0; i <= dt.Length; i++)
                    {
                        string color = Color.Red.Name;
                        if (i == 0)
                        {
                            if (!string.IsNullOrEmpty(dt[i][columnName].ToString()))
                                curval = double.Parse(dt[i][columnName].ToString());
                            shadestart = DateTime.Parse(dt[i][0].ToString()).ToOADate();
                            preval = curval;
                        }
                        if (i < dt.Length && !string.IsNullOrEmpty(dt[i][columnName].ToString()))
                            curval = double.Parse(dt[i][columnName].ToString());
                        else
                            curval = 0d;

                        if (preval != curval)
                        {
                            shadestop = DateTime.Parse(dt[i - 1][0].ToString()).ToOADate();
                            if (dr.Table.Columns.Contains("pallete"))
                            {
                                color = !string.IsNullOrEmpty(preval.ToString()) ? objPm.getColor(preval, "") : Color.Red.Name;
                            }

                            rsh = new ResultShade(shadestart, shadestop, color);
                            shades.Add(rsh);
                            shadestart = 0d;
                            shadestop = 0d;
                            shadestart = DateTime.Parse(dt[i - 1][0].ToString()).ToOADate();
                            preval = curval;
                        }
                    }
                    if (shades.Count > 0) objGparams.shades = (ResultShade[])shades.ToArray(rsh.GetType());
                }
                else
                {
                    int i = 0;
                    /* IM01288870 - New Agricast - Missing data - Jerrey - Begin */
                    //while (from <= to)
                    while (from <= to.AddDays(1).AddDays(-(valuestep == 24 ? 1 : valuestep)))
                    /* IM01288870 - New Agricast - Missing data - Jerrey - End */
                    {
                        double j;
                        if (bool.Parse(agriInfo["IsGDD"]))
                            if (from != DateTime.Parse(dt[rowCount][0].ToString()))
                            {
                                from = from.AddDays(valuestep == 24 ? 1 : valuestep);
                                i++;
                                continue;
                            }
                        if (dt.Length > rowCount && dt[rowCount][columnName].ToString() != "" && vals.Length > i)
                        {
                            vals[i] = double.TryParse(dt[rowCount][columnName].ToString(), out j) ? j : 0;
                            series.hasvalues = true;
                        }
                        else
                        {
                            missingData = true;
                            series.hasGaps = true;
                        }
                        rowCount++;

                        from = from.AddDays(valuestep == 24 ? 1 : valuestep);
                        i++;
                    }
                }
            }
            if (series.Position.ToLower() == "primary")
            {
                series.MinorY = double.Parse(config.Tables[PrimaryAxis].Rows[0]["minValue"].ToString());
                series.MajorY = double.Parse(config.Tables[PrimaryAxis].Rows[0]["maxvalue"].ToString());
                series.Scale = config.Tables[PrimaryAxis].Rows[0]["ScaleType"].ToString();
            }
            else
            {
                series.MinorY = double.Parse(config.Tables[SecondaryAxis].Rows[0]["minValue"].ToString());
                series.MajorY = double.Parse(config.Tables[SecondaryAxis].Rows[0]["maxvalue"].ToString());
                series.Scale = config.Tables[SecondaryAxis].Rows[0]["ScaleType"].ToString();
            }
            series.values = vals;

            objGparams.addSeries(series);
        }

        private void setParamBWM(DataSet config)
        {
            objGparams.Height = Int32.Parse(config.Tables[CHART].Rows[0]["height"].ToString());
            objGparams.Width = Int32.Parse(config.Tables[CHART].Rows[0]["width"].ToString());
            objGparams.WaterMark = cUtil.getApplPath(@"Images\") + config.Tables[CHART].Rows[0]["watermark"].ToString();

            foreach (DataRow dr in config.Tables[ChartSerie].Select("", "panel"))
            {
                AddSeriesToChartParams(config, dr, cUtil.getTransText("we_temperature_max"), cUtil.getTransText("we_temperature_cdegree"), "line", "secondary", "Circle", "Red", "TempAir_C_DaytimeMax", 0);
                AddSeriesToChartParams(config, dr, cUtil.getTransText("we_temperature_min"), cUtil.getTransText("we_temperature_cdegree"), "line", "secondary", "Triangle", "Orange", "TempAir_C_NighttimeMin", 0);
                AddSeriesToChartParams(config, dr, cUtil.getTransText("we_rainfall"), cUtil.getTransText("we_rainfall_mm"), "bar", "primary", "Rect", "#" + Color.FromArgb(187, 187, 255).Name, "PrecipAmount_mm_DailySum", 0);
                AddSeriesToChartParams(config, dr, cUtil.getTransText("wm_watersurfacesss"), cUtil.getTransText("wm_mm"), "line", "primary", "Diamond", "Blue", BorderWaterModel.S_TOPSOIL_MM, 1);
            }

            objGparams.LeftScaleType = config.Tables[PrimaryAxis].Rows[0]["scaleType"].ToString();
            objGparams.LeftAxisLabel = cUtil.getTransText(config.Tables[PrimaryAxis].Rows[0]["trnsTag"].ToString());
            objGparams.Ymax = double.Parse(config.Tables[PrimaryAxis].Rows[0]["maxValue"].ToString());
            objGparams.Ymin = double.Parse(config.Tables[PrimaryAxis].Rows[0]["minValue"].ToString());

            objGparams.RightAxisLabel = cUtil.getTransText(config.Tables[SecondaryAxis].Rows[0]["trnsTag"].ToString());
            objGparams.RightScaleType = config.Tables[SecondaryAxis].Rows[0]["scaletype"].ToString();
            objGparams.Y2max = double.Parse(config.Tables[SecondaryAxis].Rows[0]["maxValue"].ToString());
            objGparams.Y2min = double.Parse(config.Tables[SecondaryAxis].Rows[0]["minValue"].ToString());

            objGparams.BottomLabels = getLabelsHistory(startDate, selectedEndDate, valuestep);

            if (missingData)
                objGparams.warning = cUtil.getTransText("wm_warning") + ":" + cUtil.getTransText("This chart has been plotted with missing data.");

            string distText = cUtil.getTransText("ResDistanceFrom");
            distText = distText.Replace("{Dist}", dpi.distance.ToString());
            distText = distText.Replace("{Dir}", dpi.directionLetter);
            distText = distText.Replace("{CityName}", HttpUtility.HtmlDecode((LocationInfo.getLocationInfoObj).placeName));
            distText = distText.Replace("{Elevation}", dpi.altitude.ToString());
            distText = distText.Replace("<strong>", "").Replace("</strong>", "");

            var location = string.Empty; ;
            if (HttpContext.Current.Session["service"] != null)
            {
                var srv = (service)HttpContext.Current.Session["service"];
                if (string.IsNullOrEmpty(srv.locationSearch.defaultLocation))
                    location = loc.placeName;
                else
                    location = HttpUtility.HtmlDecode(srv.locationSearch.defaultLocation);
            }

            objGparams.Title = string.Format("{0} {1} # '{2}' #"
                                            + "{3}: {4}m {15} {5} {6} {7} {8} ({9} {10})#"
                                            + "{11}: {12} mm {13}: {14} mm",
                                cUtil.getTransText("wm_cropwaterbalance"),
                                cUtil.getTransText("for"),
                                location,

                                cUtil.getTransText("altabbr"),
                                dpi.altitude.ToString(),
                                cUtil.getTransText("from"),
                                FormatDateTime(startDate, culture),
                                cUtil.getTransText("to"),
                                FormatDateTime(DateTime.Parse(agriInfo["endDate"].ToString()), culture),
                                DateTime.Parse(agriInfo["endDate"].ToString()).Subtract(startDate).Days,
                                cUtil.getTransText("days"),

                                cUtil.getTransText("wm_totalholdingcapacity"),
                                BorderWaterModel.TotalWHC.ToString("###.##"),
                                cUtil.getTransText("wm_initialbalance"),
                                BorderWaterModel.TotalWHC.ToString("###.##"),
                                "ü.d.M."
                                );

            //objGparams.FooterText += cUtil.getTransText("chartcopyright");
        }

        private string FormatDateTime(DateTime date, string culture)
        {
            string dtStr = date.Date.ToString(new CultureInfo(culture));
            return dtStr.Substring(0, dtStr.IndexOf(' '));
        }

        /*3.1 UC – BodenWasser Modell - display webpage as IFrame - Jerrey - End*/

        void AddMarkers()
        {
            ArrayList mks = new ArrayList();
            ResultMarker marker = null;
            dswidth = "1";
            if (dswidth != null && dswidth.Length > 0)
            {
                int barwidth = int.Parse(dswidth);
                string color = Color.Black.Name;
                for (int x = 1; x < noofdays; x++)
                {
                    marker = new ResultMarker(startDate.AddDays(x).ToOADate() - valuestep / 48.0, 0, color, "");
                    marker.width = barwidth;
                    mks.Add(marker);
                }
            }
            if (objGparams.TodayMarker)
            {
                dswidth = "1";
                if (dswidth != null && dswidth.Length > 0)
                {
                    DateTime gmt = DateTime.Now.ToUniversalTime();
                    //Need to get the timezone offset for the given lat long.
                    gmt = gmt.AddHours(dpi.DayOffset + dpi.iDstOn);
                    marker = new ResultMarker(gmt.ToOADate() - valuestep / 48.0, 0, Color.Red.Name, "");
                    marker.label = "";//cUtil.getTransText("cal_today");                   
                    marker.width = int.Parse(dswidth);
                    mks.Add(marker);
                }
            }
            objGparams.markers = (ResultMarker[])mks.ToArray(marker.GetType());
        }
        private void getSunriseSunsetAndShaders()
        {
            // add sunrise / sunset markers
            string color = Color.LightGray.Name;
            ResultShade rsh = null;
            ArrayList shades = new ArrayList();
            DateTime rdate = cUtil.getSunrise(startDate, 47.6, 7.6);
            DateTime sdate = cUtil.getSunset(rdate, 47.6, 7.6);
            int setMin = sdate.Hour * 60 + sdate.Minute;
            int riseMin = rdate.Hour * 60 + rdate.Minute;
            if (setMin > riseMin) setMin -= 24 * 60;

            DateTime cdate = startDate;
            DateTime shadestart = cdate;
            DateTime shadestop = cdate;
            int risemin = riseMin;
            int setmin = setMin;
            int cmin = cdate.Hour * 60 + cdate.Minute;
            int maxvalue = (Math.Abs(_end) - Math.Abs(_start)) * 24 * 60;

            while (cmin < maxvalue + (24 * 60))
            {
                int startmin = setmin;
                int endmin = risemin;
                if (startmin < 0) startmin = 0;
                if (endmin > maxvalue) endmin = maxvalue;
                rsh = new ResultShade((float)cdate.AddMinutes(startmin).ToOADate(),
                    (float)cdate.AddMinutes(endmin).ToOADate(), color);
                shades.Add(rsh);
                risemin += 24 * 60;
                setmin += 24 * 60;
                cmin += 24 * 60;
            }
            if (shades.Count > 0) objGparams.shades = (ResultShade[])shades.ToArray(rsh.GetType());
        }
        private double[] getLabels(DateTime desiredDateFrom)
        {
            int max = noofvalues + 1;
            double[] Labels = new double[max];
            int y = 0, x = 0;
            while (y < max)
            {
                Labels[y++] = desiredDateFrom.ToOADate();
                desiredDateFrom = desiredDateFrom.AddHours(valuestep);
                x++;
            }
            return Labels;
        }
        private void ylabels()
        {
            double Max = 0.0;
            double Min = 0.0;
            bool isPrimary = false;
            if (!objGparams.RightAxisLabel.Contains("precip"))
            {
                Max = objGparams.Ymax;
                Min = Max - (objGparams.Ymax / objGparams.Y2max * (objGparams.Y2max - objGparams.Y2min));
                isPrimary = true;
                objGparams.Ymin = Min;
            }
            else
            {
                Max = objGparams.Y2max;
                Min = Max - (objGparams.Y2max / objGparams.Ymax * (objGparams.Ymax - objGparams.Ymin));
                objGparams.Y2min = Min;
            }
            CultureInfo myCulture = new CultureInfo(culture);
            float step = 1f;
            double rest1;
            double rest2;
            rest1 = Math.IEEERemainder(Max, 1);
            rest2 = Math.IEEERemainder(Min, 1);
            // check if we need more detailed step
            step = 10000.0f;
            if (Max < step)
            {
                while (Max / step < 5) step /= 10;
            }

            int max = (int)Math.Round((Max - Min + 1) / step);
            string[] legends = new string[max];
            double fx = Math.Round(Min / step, 0) * step;
            for (int x = 0; x < max; fx += step, x++)
            {
                double fxval = Math.Round(fx, (step >= 1) ? 0 : 1);
                if (fxval >= 0) legends[x] = fxval.ToString((step >= 1) ? "F0" : "F1", myCulture.NumberFormat);
                else legends[x] = null;
            }
            if (isPrimary)
            {
                objGparams.PrimaryLabelValue = step;
                objGparams.PrimaryLabels = legends;
            }
            else
            {
                objGparams.PrimaryLabelValue = step;
                objGparams.SecondaryLabels = legends;
            }

        }
        private void getTopAxisLabels()
        {
            DateTime cdate = startDate;
            objGparams.TopLabels = new string[noofdays];
            string nl = "ddddd ";
            if (noofdays > 5) nl = "ddddd\n";
            if (noofdays > 6 || objServiceInfo.IsMobile) nl = "ddd\n";
            CultureInfo ci = new CultureInfo(culture);
            nl += cUtil.ShortMonthDayPattern(ci.DateTimeFormat);
            for (int x = 0; x < noofdays; x++)
            {
                objGparams.TopLabels[x] = cdate.ToString(nl, ci.DateTimeFormat);
                cdate = cdate.AddDays(1);
            }
        }
        PaletteMap getPalleteColor(string pallete)
        {
            PaletteMap objPm = new PaletteMap();
            try
            {
                PalletteConstants objPalCon = new PalletteConstants();
                DataTable dt = objPalCon.getPallette(pallete);
                foreach (DataRow dr in dt.Rows)
                {
                    string text = dt.Columns.Count > 2 ? dr[2].ToString() : "";
                    objPm.addLimit(Convert.ToDouble(dr[0].ToString()), dr[1].ToString(), text);
                }
                return objPm;
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = getTranslatedText(Constants.TAB_PALLETE_FAILURE, objServiceInfo.Culture) + " : " + ex.Message.ToString();
                return objPm;
            }
        }
        double[] getLabelsHistory(DateTime fromdate, DateTime todate, int decimation)
        {
            if (decimation == 24) decimation = 1;
            int maxdesired = ((TimeSpan)(todate - fromdate)).Days + 1;
            int max = 0;
            int y = 0, x = 0;
            double[] labels;
            DateTime cdate = fromdate;
            if (decimation == 30)
            {
                while (cdate.Month + cdate.Year * 12 < todate.Month + todate.Year * 12)
                {
                    max++;
                    cdate = cdate.AddMonths(1);
                }
                if (max < noofvalues + 1) max = noofvalues + 1;
                labels = new double[max];
                cdate = new DateTime(fromdate.Year, fromdate.Month, 15);
                while (y < max)
                {
                    labels[y++] = cdate.ToOADate();
                    cdate = cdate.AddMonths(1);
                }
            }
            else if (decimation == 7)
            {
                labels = getWeeklylegend(fromdate, todate);
            }
            else
            {
                max = (maxdesired / decimation) + 1;
                if (decimation > 1 && max <= 0) max = 1;
                labels = new double[max];
                while (y < max)
                {
                    if (x % decimation == 0) labels[y++] = fromdate.ToOADate();
                    fromdate = fromdate.AddDays(1);
                    x++;
                }
            }
            return labels;

        }
        double[] getWeeklylegend(DateTime datefrom, DateTime dateto)
        {
            int weekend = (dateto.DayOfYear / 7);
            if (weekend >= 52) weekend = 51;
            weekend += dateto.Year * 52;

            int weekstart = (datefrom.DayOfYear / 7);
            if (weekstart >= 52) weekstart = 51;
            weekstart += datefrom.Year * 52;
            int maxdesired = weekend - weekstart;

            int cweek = datefrom.DayOfYear / 7 + 1;
            if (cweek > 52) cweek = 52;
            int cyear = datefrom.Year;
            int max = maxdesired + 1;
            double[] labels = new double[max];
            //datefrom = new DateTime(datefrom.Year, 1, 1);
            //datefrom = datefrom.AddDays((cweek - 1) * 7);
            int x = 0, y = 0;
            while (y < max)
            {
                if (x % 1 == 0) labels[y++] = datefrom.ToOADate();
                cweek++;
                datefrom = datefrom.AddDays(7);
                if (cweek > 52)
                {
                    cweek = 0;
                    cyear++;
                    datefrom = new DateTime(cyear, 1, 1);
                }
                x++;
            }

            return labels;
        }
        public string getTranslatedText(string strLabelName, string strCultureCode)
        {
            return objTranslate.getTranslatedText(strLabelName, strCultureCode);
        }
    }
}
