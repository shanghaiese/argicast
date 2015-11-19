using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.Data.Access;
using System.Data;

namespace Syngenta.AgriCast.Charting.Service
{
    public class ChartIconsService
    {
        LocationInfo objLocInfo;
        DataPointInfo objDataPoint;
        DateTime current = System.DateTime.Today;
        DateTime startDate;
        DateTime endDate;
        TableData objTableData = new TableData();
        static double dStnLatitude = 0;
        static double dStnLongitude = 0;
        static int iAltitude;
        static int iMaxAllowedDist;
        static int iMaxAltitudeDiff;
        WeatherData1 wData = new WeatherData1();
        FeatureRequest fReq;
        WeatherDataRequest wReq;
        WeatherDataResponse wRes;


        /*Method to populate the objects. If session is availabel then objects will be populated using seesion objects 
        else objects will be populated using values passed from the webservice  * */
        protected void populateObject()
        {
            if (HttpContext.Current != null)
            {
                objLocInfo = LocationInfo.getLocationInfoObj;
                objDataPoint = objLocInfo.DataPointInfo;
            }
            else
            {
                objDataPoint = new DataPointInfo();
                objLocInfo = new LocationInfo();

                objDataPoint.stationLatitude = dStnLatitude;
                objDataPoint.stationLongitude = dStnLongitude;
                objDataPoint.altitude = iAltitude;
                objDataPoint.NearbyPointSettings.MaxAllowedDistance = iMaxAllowedDist;
                objDataPoint.altitude = iMaxAltitudeDiff;
            }
        }

        //This method will be called from the web service to populate the values required to populate the DataPoint object
        public void setTableWebServiceValues(double dStnLat, double dStnLong, int iAtl, int iMaxDist, int iMaxAltDiff)
        {
            dStnLatitude = dStnLat;
            dStnLongitude = dStnLong;
            iAltitude = iAtl;
            iMaxAllowedDist = iMaxDist;
            iMaxAltitudeDiff = iMaxAltDiff;
        }


        public DataTable GetSeriesData(DataTable series, string aggregation, string dataSource, int start, int end, int startHours, int endHours)
        {
            //objLocInfo = LocationInfo.getLocationInfoObj;
            //objDataPoint = objLocInfo.DataPointInfo;
            populateObject();
            Serie serie = new Serie("", "", false, null, "");
            List<Serie> lst = new List<Serie>();

            enumDataSource source;
            switch (dataSource.ToUpper())
            {
                case "GLBSTNFCST":
                    source = enumDataSource.glbStnFcst;
                    break;
                case "GLB25ECMWF":
                    source = enumDataSource.glb25ECMWF;
                    break;
                case "GLB25OBS":
                    source = enumDataSource.glb25Obs;
                    break;
                case "GLB25OBSEXT":
                    source = enumDataSource.glbStnObsExt;
                    break;
                case "GLB25OBSSYN":
                    source = enumDataSource.glbStnObsSyn;
                    break;
                default:
                    source = enumDataSource.glbStnFcst;
                    break;
            }

            enumTemporalAggregation tmpAggregation;
            switch (aggregation.ToLower())
            {
                case "hourly":
                    tmpAggregation = enumTemporalAggregation.Hourly;
                    break;
                case "daily":
                    tmpAggregation = enumTemporalAggregation.Daily;
                    break;
                case "decade":
                    tmpAggregation = enumTemporalAggregation.Decade;
                    break;
                case "8hourly":
                    tmpAggregation = enumTemporalAggregation.EightHourly;
                    break;
                case "12hourly":
                    tmpAggregation = enumTemporalAggregation.TwelveHourly;
                    break;
                case "weekly":
                    tmpAggregation = enumTemporalAggregation.Weekly;
                    break;
                case "monthly":
                    tmpAggregation = enumTemporalAggregation.Monthly;
                    break;
                default:
                    tmpAggregation = enumTemporalAggregation.Hourly;
                    break;
            }
            foreach (DataRow dr in series.Rows)
            {
                if (tmpAggregation == enumTemporalAggregation.Daily || tmpAggregation == enumTemporalAggregation.Hourly)
                    lst.Add(
                       new Serie(
                          dr["Name"].ToString()
                          , ""
                          , false
                          , null
                          , ""));
                else
                    lst.Add(
                     new Serie(
                        dr["Name"].ToString()
                        , ""
                        , false
                        , (enumAggregationFunction)Enum.Parse(typeof(enumAggregationFunction), dr["aggregationfunction"].ToString(), true)
                        , ""));

            }
            startDate = current.AddDays(start);
            endDate = current.AddDays(end);
            if (startDate > endDate)
            {
                DateTime tmp = startDate;
                startDate = endDate;
                endDate = tmp;
            }

            startDate = startDate.AddHours(startHours);
            endDate = endDate.AddHours(endHours);

            fReq = new FeatureRequest(objDataPoint.stationLatitude, objDataPoint.stationLongitude, objDataPoint.altitude, objDataPoint.NearbyPointSettings.MaxAllowedAltitude, objDataPoint.NearbyPointSettings.MaxAllowedDistance);

            wReq = new WeatherDataRequest(source, false, fReq, startDate, endDate, tmpAggregation, lst);
            wRes = wData.GetWeatherData(wReq);
            return wRes.WeatherData;
        }
        public string getTranslatedText(string strLabelName, string strCultureCode)
        {
            return objTableData.getTranslatedText(strLabelName, strCultureCode);
        }
    }
}