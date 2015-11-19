using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syngenta.AgriCast.Charting.View;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common.DTO;
using System.Globalization;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common;
using System.Data;
using Syngenta.AgriCast.Charting.Service;
using System.Collections;

namespace Syngenta.AgriCast.Charting.Presenter
{
    public class ChartIconsPresenter
    {
        ServiceHandler objSvcHandler = new ServiceHandler();
        ChartIconsService objIconSvc = new ChartIconsService();
        //DTOIcon objDTOSIcon = new DTOIcon();
        ServiceInfo objSvcInfo;
        string controlName;
        IChartIcons objIChartIcon;
        ServicePresenter objSvcPre = new ServicePresenter();
        CommonUtil objComUtil = new CommonUtil();
        ServiceHandler objSvc = new ServiceHandler();

        int sHours = 0;
        int eHours = 0;
        string strCulCode;
        const string FIRSTCOLNAME = "hour";

        public ChartIconsPresenter(IChartIcons objIChartIconView, string name)
        {
            if (objIChartIconView != null)
                objIChartIcon = objIChartIconView;

            controlName = name;

            /* UAT issue - icon chart issue - Jerrey - Start */
            /*Wind Icon as a Sepearate component -- BEGIN*/
            //Code commented 
            // width already set from windiconssetting
            //DataTable dtChartSetting = objSvc.getChartSettings("Fiveinone");
            //if (dtChartSetting.Rows.Count > 0)
            //{
            //    int chartIconWidth = 0;
            //    int.TryParse(dtChartSetting.Rows[0]["Width"].ToString(), out chartIconWidth);
            //    objIChartIconView.IconChartWidth = chartIconWidth;
            //}
            /* UAT issue - icon chart issue - Jerrey - End */
            /*Wind Icon as a Sepearate component -- END*/
        }

        public void CreateTables(string name, string allign, string aggregation, string datasource, int start, int end)
        {

            objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            //GetcompleteSeriesList(allign);

            /*Wind Icon as a Sepearate component -- BEGIN*/
            //GetTableSeries(allign);
            GetWindIconSeries(allign);
            /*Wind Icon as a Sepearate component -- END*/

            objIChartIcon.dtTableSeries = objIChartIcon.dsSeriesData.Tables[0];
            //To change first column as based on the chosen unit
            objIChartIcon.dtTableSeries = objSvcPre.ChangeUnits(objIChartIcon.dtTableSeries, objSvcInfo.Unit, objSvcInfo.WUnit);

            /*Wind Icon as a Sepearate component -- BEGIN*/
            //No Legend Needed for wind icons
            // objIChartIcon.dtTableLegends = objIChartIcon.dsSeriesData.Tables[1];
            /*Wind Icon as a Sepearate component -- END*/
            GetSeriesData(objIChartIcon.dtTableSeries, aggregation, datasource, start, end, sHours, eHours);

            //Condition to check for the availability of the data from database
            if (objIChartIcon.dtSeries != null)
            {
                objIChartIcon.dtByDays = objIChartIcon.dtSeries;
                DataColumn dc = new DataColumn();
                dc.ColumnName = getTranslatedText(FIRSTCOLNAME, objSvcInfo.Culture);
                dc.DefaultValue = null;
                objIChartIcon.dtByDays.Columns.Add(dc);


                int LastColIndex = objIChartIcon.dtByDays.Columns.Count - 1;
                for (int i = 0; i < objIChartIcon.dtByDays.Rows.Count; i++)
                {
                    DateTime Date = Convert.ToDateTime(objIChartIcon.dtByDays.Rows[i][0].ToString());
                    objIChartIcon.dtByDays.Rows[i][LastColIndex] = Date.Hour;
                }


                objIChartIcon.alSeries = new ArrayList();

                for (int i = 1; i < LastColIndex; i++)
                {
                    //indices in columns in table series
                    //0 - Name , 1 - PallateName ,2 - AggregationFunction , 3 -Transtag
                    //in case of wind icons , no pallete defined in series
                    //0 - Name ,  1 - AggregationFunction , 2 -Transtag

                    objIChartIcon.alSeries.Add(objIChartIcon.dtByDays.Columns[i].ColumnName.ToString()
                        + "," + ' ' + ","
                        + objIChartIcon.dtTableSeries.Rows[i - 1][0].ToString() + ","
                        + objIChartIcon.dtTableSeries.Rows[i - 1][2].ToString());

                }
            }
        }

        public void GetSeriesData(DataTable series, string aggregation, string datasource, int start, int end, int startHrs, int endHrs)
        {
            objIChartIcon.dtSeries = objIconSvc.GetSeriesData(series, aggregation, datasource, start, end, startHrs, endHrs);
        }

        public void GetcompleteSeriesList(string allign)
        {
            try
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                objIChartIcon.alSeriesLegend = objSvc.getNodeList(allign);

            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.GEN_LOADSERIES_FALURE) + " : " + ex.Message.ToString();
            }
        }

        public void GetWindIconSeries(string allign)
        {
            try
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                /*Wind Icon as a Sepearate component -- BEGIN*/
                //objIChartIcon.dsSeriesData = objSvc.GetTableSeries(allign, controlName);
                objIChartIcon.dsSeriesData = objSvc.getWindIconSeries(controlName);
                /*Wind Icon as a Sepearate component -- END*/
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.GEN_LOADSERIES_FALURE) + " : " + ex.Message.ToString();
            }
        }

        public DataTable ChangeUnits(DataTable dt, string Units, string WUnits)
        {
            Units objUnits = new Units();
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = objUnits.GetChangedUnits(dt.Rows[i], Units, WUnits);
                    if (dt.Columns.Contains("name"))
                        dt.Rows[i]["name"] = dr["name"].ToString();
                    if (dt.Columns.Contains("trnsTag"))
                        dt.Rows[i]["trnsTag"] = dr["trnsTag"].ToString();
                    if (dt.Columns.Contains("colorPaletteName"))
                        dt.Rows[i]["colorPaletteName"] = dr["colorPaletteName"].ToString();


                }
            }
            return dt;
        }

        public void getWindIconSettings(string allign)
        {
            try
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                /*Wind Icon as a Sepearate component -- BEGIN*/
                //objIChartIcon.strAllignDetails = objSvc.ReadAllignment(allign,controlName);
                objIChartIcon.dtWindIconSettings = objSvc.getWindIconSettings(controlName);
                /*Wind Icon as a Sepearate component -- END*/
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.TAB_ALIGNMENT_FAILURE) + ":" + ex.Message.ToString();
            }
        }

        public void getCultureCode()
        {
            try
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                objIChartIcon.strCulCode = objSvcInfo.Culture;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.CULTURE_LOADFAILURE) + " : " + ex.Message.ToString();
            }
        }

        public string getTranslatedText(string strLabelName, string strCultureCode)
        {
            return objIconSvc.getTranslatedText(strLabelName, strCultureCode);
        }

        public List<string> getIcons(DataTable dt)
        {
            var iconImgUrls = new List<string>();
            var windDirection = new List<string>();
            var windSpeed = new List<float>();
            float f = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString().Contains("winddirection"))
                {
                    for (var i = 1; i < dt.Columns.Count; i++)
                    {
                        windDirection.Add(row[i].ToString());
                    }
                }
                if (row[0].ToString().Contains("windspeed"))
                {
                    for (var i = 1; i < dt.Columns.Count; i++)
                    {
                        /* production issue - as the data value will be null or empty, so we need use TryParse */
                        float.TryParse(row[i].ToString(), out f);
                        windSpeed.Add(f);
                    }
                }
            }

            for (var i = 0; i < windDirection.Count; i++)
            {
                iconImgUrls.Add(Utils.CreateArrow(GetDirection(windDirection[i]), windSpeed[i]));
            }

            return iconImgUrls;
        }



        public List<string> getHumidities(DataTable dt)
        {
            var humidityList = new List<string>();
            int count = 1;
            foreach (DataRow row in dt.Rows)
            {
                if (count==1)
                {
                    for (var i = 1; i < dt.Columns.Count; i++)
                    {
                        if (row[0].ToString().Contains("relhumidity"))
                            humidityList.Add(row[i].ToString()+"%");
                        else
                            humidityList.Add(row[i].ToString());
                    }
                }
                count = 0;
            }
            return humidityList;
        }

        private float GetDirection(string dir)
        {
            /* Modified by Jerrey - IM01354471 - New Agricast - wind icon should not be language specific */
            switch (dir)
            {
                case "N": return 90f;
                case "S": return 270f;
                case "W": return 180;
                case "E": return 0f;
                case "NE": return 45f;
                case "SE": return 315f;
                case "SW": return 225f;
                case "NW": return 135f;
                default: return 0f;
            }
        }

       
       
        public void GenerateTransposedtblSeriesRows(DataTable dt, int numOfDays, string aggregation, bool hasRuleset, int iStep)
          
        {
            const string THREEHOURLY = "3hourly";
            const string SIXHOURLY = "6hourly";
            const string EIGHTHOURLY = "8hourly";
            const string TWELVEHOURLY = "12hourly";
            const string HOURLY = "hourly";
            
            try
            {

                int HourColIndex = dt.Columns.Count - 1;
                DataTable outTable = new DataTable();
                DataTable outputTable = new DataTable();
                if (hasRuleset == true)
                {
                    HourColIndex = 1;
                }
                else
                {
                    HourColIndex = dt.Columns.Count - 1;
                }
                if (aggregation.ToLower() == "hourly")
                {

                    foreach (DataRow dro in dt.Rows)
                    {
                        // dro[0] = dro[0].ToString().Remove(9);
                        dro[0] = DateTime.Parse(dro[0].ToString()).Date;

                    }


                    DateTime nextDate = DateTime.Today;
                    if (dt.Rows.Count > 0)
                        nextDate = Convert.ToDateTime(dt.Rows[0][0].ToString()).AddDays(numOfDays);
                    if (hasRuleset)
                    {
                        if (dt.Select("day = '" + nextDate + "'").Count() > 0)//Date on onsite DB
                        {
                            outTable = dt.Select("day = '" + nextDate + "'").CopyToDataTable();


                            // Header row's first column is same as in inputTable
                            outputTable.Columns.Add(dt.Columns[HourColIndex].ColumnName.ToString());

                            for (int i = 0; i <= 23; i++)
                            {
                                DataColumn col = new DataColumn();
                                col.ColumnName = i.ToString();
                                outputTable.Columns.Add(col);

                            }

                            foreach (DataRow dr in outTable.Rows)
                            {
                                if (dr[0].ToString().Length >= 9)
                                    dr[0] = dr[0].ToString().Remove(9);
                            }
                            for (int j = 0; j < outTable.Columns.Count; j++)
                            {
                                if (j >= 1)
                                {
                                    DataRow drow = outputTable.NewRow();
                                    drow[0] = outTable.Columns[j].ColumnName;
                                    outputTable.Rows.Add(drow);

                                }
                            }

                            for (int i = 0; i < outputTable.Rows.Count; i++)
                            {

                                for (int j = 0; j < outputTable.Columns.Count; j++)
                                {
                                    if (j < outTable.Rows.Count)
                                    {
                                        outputTable.Rows[i][j + 1] = outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString().Contains('.') ? (float.Parse(outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString()) > 200 ? String.Format("{0:0}", outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1]) : String.Format("{0:0.0}", outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1])) : outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString();

                                    }
                                    if (j > 0)
                                    {
                                        if (outputTable.Rows[i][0].ToString().ToLower().Contains("winddirection"))
                                        {
                                            //If session is not available i.e. if the call to method is from web service then set the language using web service value
                                            if (HttpContext.Current == null)
                                                objComUtil.setWebServiceCultureCode(strCulCode);
                                            int dirdeg;
                                            /* IM01354471 - New Agricast - wind icon should not be language specific - Jerrey - Begin */
                                            //outputTable.Rows[i][j] = objComUtil.getTextDirection(Int32.TryParse(outputTable.Rows[i][j].ToString(), out dirdeg) ? dirdeg : 0);
                                            outputTable.Rows[i][j] = objComUtil.getTextDirectionForWindIcon(Int32.TryParse(outputTable.Rows[i][j].ToString(), out dirdeg) ? dirdeg : 0);
                                            /* IM01354471 - New Agricast - wind icon should not be language specific - Jerrey - End */
                                        
                                        }

                                    }

                                }

                            }

                        }
                    }
                    else
                    {

                        if (dt.Select("Date = '" + nextDate + "'").Count() > 0)//Date on onsite DB
                        {
                            outTable = dt.Select("Date = '" + nextDate + "'").CopyToDataTable();


                            // Header row's first column is same as in inputTable
                            outputTable.Columns.Add(dt.Columns[HourColIndex].ColumnName.ToString());

                            for (int i = 0; i <= 23; i++)
                            {
                                DataColumn col = new DataColumn();
                                col.ColumnName = i.ToString();
                                outputTable.Columns.Add(col);

                            }

                            foreach (DataRow dr in outTable.Rows)
                            {
                                if (dr[0].ToString().Length >= 9)
                                    dr[0] = dr[0].ToString().Remove(9);
                            }
                            for (int j = 0; j < outTable.Columns.Count - 1; j++)
                            {
                                if (j >= 1)
                                {
                                    DataRow drow = outputTable.NewRow();
                                    drow[0] = outTable.Columns[j].ColumnName;
                                    outputTable.Rows.Add(drow);

                                }
                            }

                            for (int i = 0; i < outputTable.Rows.Count; i++)
                            {

                                for (int j = 0; j < outputTable.Columns.Count; j++)
                                {
                                    if (j < outTable.Rows.Count)
                                    {
                                        // outputTable.Rows[i][j + 1] = outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString().Contains('.') ? String.Format("{0:0.0}", outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1]) : outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString();
                                        outputTable.Rows[i][j + 1] = outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString().Contains('.') ? (float.Parse(outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString()) > 200 ? String.Format("{0:0}", outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1]) : String.Format("{0:0.0}", outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1])) : outTable.Rows[Convert.ToInt32(outTable.Rows[j][HourColIndex].ToString())][i + 1].ToString();
                                    }
                                    if (j > 0)
                                    {
                                        if (outputTable.Rows[i][0].ToString().ToLower().Contains("winddirection"))
                                        {
                                            //If session is not available i.e. if the call to method is from web service then set the language using web service value
                                            if (HttpContext.Current == null)
                                                objComUtil.setWebServiceCultureCode(strCulCode);
                                            int dirdeg;
                                            /* IM01354471 - New Agricast - wind icon should not be language specific - Jerrey - Begin */
                                            //outputTable.Rows[i][j] = objComUtil.getTextDirection(Int32.TryParse(outputTable.Rows[i][j].ToString(), out dirdeg) ? dirdeg : 0);
                                            outputTable.Rows[i][j] = objComUtil.getTextDirectionForWindIcon(Int32.TryParse(outputTable.Rows[i][j].ToString(), out dirdeg) ? dirdeg : 0);
                                            /* IM01354471 - New Agricast - wind icon should not be language specific - Jerrey - End */
                                        }

                                    }

                                }

                            }

                        }
                    }
                }
                else
                {


                    // Header row's first column is same as in inputTable
                    outputTable.Columns.Add(dt.Columns[HourColIndex].ColumnName.ToString());


                    // for (int i = 0; i < dt.Rows.Count; i++)
                    for (int i = 0, j = 0; i < (numOfDays * 24) / iStep; i++, j++)
                    {
                        if (i < dt.Rows.Count)
                        {
                            DataColumn col = new DataColumn();
                            col.ColumnName = dt.Rows[i][0].ToString();
                            if (j >= 24 / iStep)
                                j = 0;
                            string HourDisplay = "";
                            if (j == 0)
                                HourDisplay = "0-" + iStep;
                            else
                                HourDisplay = (j * iStep).ToString() + "-" + (((j + 1) * iStep) == 24 ? 0 : (j + 1) * iStep).ToString();

                            col.ColumnName = Convert.ToDateTime(dt.Rows[i][0]).ToShortDateString() + "," + HourDisplay;
                            col.Caption = HourDisplay;
                            outputTable.Columns.Add(col);
                        }

                    }
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        DataRow dr = outputTable.NewRow();
                        dr[0] = dt.Columns[i].ColumnName;
                        outputTable.Rows.Add(dr);
                    }

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        for (int j = 0; j < outputTable.Columns.Count; j++)
                        {
                            if (((j + 1) < outputTable.Columns.Count) && (i + 1) < dt.Columns.Count)

                                outputTable.Rows[i][j + 1] = ((dt.Rows[j][i + 1].ToString().Contains('.')) ? (float.Parse(dt.Rows[j][i + 1].ToString()) > 200 ? String.Format("{0:0}", dt.Rows[j][i + 1]).ToString() : String.Format("{0:0.0}", dt.Rows[j][i + 1]).ToString()) : dt.Rows[j][i + 1].ToString());
                        }
                    }

                    for (int i = 0; i < outputTable.Rows.Count; i++)
                    {
                        for (int j = 1; j < outputTable.Columns.Count; j++)
                        {
                            if (outputTable.Rows[i][0].ToString().ToLower().Contains("winddirection"))
                            {
                                //If session is not available i.e. if the call to method is from web service then set the language using web service value
                                if (HttpContext.Current == null)
                                    objComUtil.setWebServiceCultureCode(strCulCode);
                                int dirdeg;
                                /* IM01354471 - New Agricast - wind icon should not be language specific - Jerrey - Begin */
                                //outputTable.Rows[i][j] = objComUtil.getTextDirection(Int32.TryParse(outputTable.Rows[i][j].ToString(), out dirdeg) ? dirdeg : 0);
                                outputTable.Rows[i][j] = objComUtil.getTextDirectionForWindIcon(Int32.TryParse(outputTable.Rows[i][j].ToString(), out dirdeg) ? dirdeg : 0);
                                /* IM01354471 - New Agricast - wind icon should not be language specific - Jerrey - End */
                            }
                        }
                    }
                    if (!hasRuleset)
                        outputTable.Rows.RemoveAt(outputTable.Rows.Count - 1);

                }
                if (HttpContext.Current != null)
                    objIChartIcon.dt = outputTable;
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.TAB_DATAFETCH_FAILURE) + " : " + ex.Message.ToString();
                DataTable dtOut = new DataTable();
            }
        }
    }
}