using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syngenta.AgriCast.AgriInfo.DataAccess;
using System.Data;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.Data.Access;
using Syngenta.AgriCast.Common.Service;
using Syngenta.Agricast.Modals;

namespace Syngenta.AgriCast.AgriInfo.Service
{
    public class AgriInfoService
    {
        AgriInfoDB agriDA = new AgriInfoDB();
        LocationInfo objLocInfo;
        DataPointInfo objDataPoint;

        ServiceInfo objServiceInfo; 
        
        ServiceHandler sh = new ServiceHandler();
        DateTime current = System.DateTime.Today;
        DateTime startDate;
        DateTime endDate;
        WeatherData1 wData = new WeatherData1();
        FeatureRequest fReq;
        WeatherDataRequest wReq;
        WeatherDataResponse wRes;
        string CHART = "ChartSettings";
        string ChartSerie = "ChartSerie";
        int valuestep = 1;
        public string getTranslatedText(string strLabelName, string strCultureCode)
        {
            return agriDA.getTranslatedText(strLabelName, strCultureCode);
        }

        public DataTable GetSeriesData(DataTable series, string aggregation, string dataSource, int start, int end, string cName)
        {
            objLocInfo = LocationInfo.getLocationInfoObj;
            objDataPoint = objLocInfo.DataPointInfo;
            Serie serie = new Serie("", "", false, null, "");
            List<Serie> lst = new List<Serie>();
            Dictionary<string, string> agriInfo = null;
            if (HttpContext.Current.Session["IAgriInfo"] != null)
            {
                  agriInfo = (Dictionary<string, string>)HttpContext.Current.Session["IAgriInfo"];
                startDate = DateTime.Parse(agriInfo["startDate"].ToString());
                endDate = DateTime.Parse(agriInfo["endDate"].ToString());
                if (cName.ToLower() != "gdd")
                    aggregation = agriInfo["aggregation"].ToString();
                else
                    aggregation = "daily";                 
            }
            else
            {
                startDate = current.AddDays(start);
                //endDate = current.AddDays(end > 0 ? end - 1 : end + 1);
                endDate = current.AddDays(end);
            }            

           enumDataSource source= (enumDataSource)Enum.Parse(typeof(enumDataSource),dataSource,true);

            enumTemporalAggregation tmpAggregation;
            switch (aggregation.ToLower())
            {
                case "hourly":
                    tmpAggregation = enumTemporalAggregation.Hourly;
                    valuestep = 1;
                    break;
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
                default:
                    tmpAggregation = enumTemporalAggregation.Daily;
                    valuestep = 24;
                    break;
            }


            if (bool.Parse(agriInfo["IsGDD"].ToString()) && cName.ToLower() == "gdd")
            {                 
                DataTable dtCombined = GDD.RulesetSeriesList;
                if (series.Rows.Count > 1)
                {
                    for (int i = 1; i < series.Rows.Count; i++)
                    {
                        DataRow dr = dtCombined.NewRow();
                        dr[0] = series.Rows[i][0].ToString();
                        dtCombined.Rows.Add(dr);
                    }
                }
                foreach (DataRow dr in dtCombined.Rows)
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
            }
            else
            {
                foreach (DataRow dr in series.Rows)
                {
                    if (tmpAggregation == enumTemporalAggregation.Daily || tmpAggregation == enumTemporalAggregation.Hourly)
                        lst.Add(
                           new Serie(
                              dr["Name"].ToString()
                              , ""
                              , ((dr["Name"].ToString().ToLower().Contains("tempair") && agriInfo["altitude"].ToString() != string.Empty) ? true : false)
                              , null
                              , ""));
                    else
                        lst.Add(
                         new Serie(
                            dr["Name"].ToString()
                            , ""
                            , ((dr["Name"].ToString().ToLower().Contains("tempair") && agriInfo["altitude"].ToString() != string.Empty) ? true : false)
                            , (enumAggregationFunction)Enum.Parse(typeof(enumAggregationFunction), dr["aggregationfunction"].ToString(), true)
                            , ""));
                }
            }

            fReq = new FeatureRequest(objDataPoint.stationLatitude, objDataPoint.stationLongitude, agriInfo["altitude"].ToString() != string.Empty ? Int32.Parse(agriInfo["altitude"].ToString()) : 0, objDataPoint.NearbyPointSettings.MaxAllowedAltitude, objDataPoint.NearbyPointSettings.MaxAllowedDistance);

            if (startDate > endDate)
            {
                DateTime tmp = startDate;
                startDate = endDate;
                endDate = tmp;
            }

            DateTime newStartDate = startDate;
            DateTime newEndDate = endDate;
            int maxyear = 0;
            int minyear = 0;
            bool NotWaterModel = true;
            if (!bool.Parse(agriInfo["IsGDD"]) && cName.ToLower() != "gdd")
            {
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
            if ((bool.Parse(agriInfo["IsGDD"]) && cName.ToLower() == "gdd") && agriInfo["plantingDate"] != "")
            {
                newStartDate = DateTime.Parse(agriInfo["plantingDate"]);
                NotWaterModel = false;
            }
                       
            wReq = new WeatherDataRequest(source, false, fReq, newStartDate, newEndDate, tmpAggregation, lst);
            if (NotWaterModel)
                wRes = wData.GetWeatherData(wReq, true, objDataPoint.DataPoint);
            else
                wRes = wData.GetWeatherData(wReq);


            DataTable rawdata = new DataTable();
            rawdata = wRes.WeatherData;
            if (bool.Parse(agriInfo["IsGDD"].ToString()) && cName.ToLower() == "gdd")
            {
                DataTable dtDataRuleset;
                GDD.dtInput = rawdata;
                GDD.Base = Int32.Parse(agriInfo["Base"]);
                GDD.Cap = Int32.Parse(agriInfo["Cap"]);
                GDD.Method = agriInfo["Method"];
                GDD.CalculateGDD();
                dtDataRuleset = GDD.dtOutput;

                DataTable targetTable = dtDataRuleset.Clone();
                var dt2Columns = rawdata.Columns.OfType<DataColumn>().Select(dc =>
                   new DataColumn(dc.ColumnName, dc.DataType, dc.Expression, dc.ColumnMapping));
                   var dt2FinalColumns = from dc in dt2Columns.AsEnumerable()
                                         where targetTable.Columns.Contains(dc.ColumnName) == false
                                         select dc;
                   targetTable.Columns.AddRange(dt2FinalColumns.ToArray());

                   var rowData = from row1 in dtDataRuleset.AsEnumerable()
                                 join row2 in rawdata.AsEnumerable()
                                 on row1.Field<DateTime>("date") equals row2.Field<DateTime>("date")
                                 select row1.ItemArray.Concat(row2.ItemArray.Where(r2 => row1.ItemArray.Contains(r2) == false)).ToArray();
                   foreach (object[] values in rowData)
                       targetTable.Rows.Add(values); 

                   rawdata = targetTable;
                   rawdata.Columns.Remove(GDD.RulesetSeriesList.Rows[0][0].ToString());
                   rawdata.Columns.Remove(GDD.RulesetSeriesList.Rows[1][0].ToString());
            }                         

            return getFinalData(series, startDate, endDate,rawdata ,bool.Parse(agriInfo["IsGDD"].ToString()));
        }

        DataTable getFinalData(DataTable series,DateTime sDate,DateTime eDate,DataTable rawData,bool isGdd)
        {
            DataTable finalData = new DataTable();
            finalData.Columns.Add("date");
            int cols = 1;
            //if (isGdd)
            //{
            //    DataRow dr = series.Rows[0];
            //    dr["name"] = "GDD";
            //    series.Rows[1].Delete();
            //    series.AcceptChanges();
            //}
            foreach(DataRow dr in series.Rows)
            {
                int rowCount = 0;
                string name = dr["name"].ToString();
                int noofvalues = ((TimeSpan)(endDate - startDate)).Days / (valuestep == 24 ? 1 : valuestep);
                noofvalues = noofvalues + 1;
                DataRow[] dt = null;
                DateTime from = startDate;
                DateTime to = endDate;
                if (dr["year"].ToString() != "")
                {
                    from = new DateTime(Int32.Parse(dr["year"].ToString()), from.Month, (from.Month == 2 && from.Day > 28) ? 28 : from.Day);
                    int range = (endDate.Year - startDate.Year);
                    to = new DateTime(Int32.Parse(dr["year"].ToString()) + range, to.Month, (to.Month == 2 && to.Day > 28) ? 28 : to.Day);
                    if (range != 0)
                       name = name + " " + dr["year"].ToString() + "-" + (Int32.Parse(dr["year"].ToString()) + range).ToString();
                    else
                        name = name + " " + dr["year"].ToString();
                    if (to > DateTime.Parse(rawData.Select("date=max(date)")[0]["date"].ToString()))
                    {
                        to = DateTime.Parse(rawData.Select("date=max(date)")[0]["date"].ToString());
                    }
                }

                dt = rawData.Select("date>='" + from + "' and date<='" + to + "'", "date asc");
                //dt = rawData.Select();
                finalData.Columns.Add(name);
                if (dt != null && dt.Count() != 0)
                {                    
                    int i = 0;
                    from = DateTime.Parse(dt[0]["date"].ToString());
                    while (from <= to)
                    {
                        DataRow frow=null;
                        if (cols == 1)
                        {
                            frow = finalData.NewRow();                   
                            frow["date"] = from.ToString();                            
                        }
                        else
                        {
                            if (finalData.Rows.Count > rowCount)
                                frow = finalData.Rows[rowCount];
                            else
                            {
                                from = from.AddDays(valuestep == 24 ? 1 : valuestep);
                                continue;
                            }
                        }
                        double j;
                        if (dt.Length > rowCount &&  dt[rowCount][cols].ToString() != "")
                        {
                            frow[name] = double.TryParse(dt[rowCount][cols].ToString(), out j) ? j : 0;                            
                        }
                        if(cols==1)
                           finalData.Rows.Add(frow);
                        rowCount++;
                        from = from.AddDays(valuestep == 24 ? 1 : valuestep);
                        i++;
                    }
                }
                cols++;
            }
            return finalData;
        }
        public DataTable getChartData(string chartName, DataTable agriInfoData)
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
            objDataPoint = DataPointInfo.getDataPointObject;
            DataSet dsData = new DataSet();
            dsData.Tables.Add(sh.getChartSettings(chartName));
            string dtSource = dsData.Tables[CHART].Rows[0]["dataSource"].ToString();

            dsData.Tables.Add(agriInfoData);
            DataTable dtData = GetSeriesData(agriInfoData, "", dtSource, 0, 0, chartName).Copy();
            return dtData;

        }
        public string GetChartName()
        {
            return sh.GetChartName();
        }

    }
}