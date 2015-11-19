using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.LocationSearch.Service;
using Syngenta.AgriCast.Icon.Service;
using System.Data;
using System.Collections;
using Syngenta.AgriCast.Icon.DTO;
using Syngenta.AgriCast.WebService.View;
using Syngenta.AgriCast.Charting.Service;
using Syngenta.AgriCast.Charting.DTO;
using Syngenta.AgriCast.Tables.Service;
using Syngenta.AgriCast.Tables.Presenter;
using Syngenta.AgriCast.Charting.Presenter;
using System.IO;
using Syngenta.Agricast.Modals;
using Syngenta.AgriCast.ExceptionLogger;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Linq.Expressions;
using System.Drawing;

 
 
namespace Syngenta.AgriCast.WebService.Presenter
{
    public class WebServicePresenter
    {
        CommonUtil objCommonUtil = new CommonUtil();
        nearbyPointService objNearbyPointSvc = new nearbyPointService();
        locSearchService objLocSearchSvc = new locSearchService();
        IconService objIconSvc = new IconService();
        ChartService objChartSvc = new ChartService();
        ServiceHandler objSvcHandler = new ServiceHandler();
        TableService objTableSvc = new TableService();
        IAgricastService IAgWebService;
      
        ChartPresenter objChartPre = new ChartPresenter("Fiveinone");
        TablePresenter objTblPre = new TablePresenter();
        //IRuleSets objIRuleset;

        public WebServicePresenter(IAgricastService IAgService)
        {
            if (IAgService != null)
            {
                IAgWebService = IAgService;
            }
        }

        //Method to fetch sunrise time
        public DateTime getSunrise(DateTime dDay, double dLat, double dLong)
        {
            return objCommonUtil.getSunrise(dDay, dLat, dLong).AddHours(Convert.ToDouble(IAgWebService.iTimeZoneOffset + IAgWebService.iDstOn));
        }

        //Method to fetch sunset time
        public DateTime getSunset(DateTime dDay, double dLat, double dLong)
        {
            return objCommonUtil.getSunset(dDay, dLat, dLong).AddHours(Convert.ToDouble(IAgWebService.iTimeZoneOffset + IAgWebService.iDstOn));
        }

        //Method to fetch time zone offset value
        public int getTimeZoneOffset(double dLat, double dLong, int intMaxAllowedDist, int intMaxAllowedAlt, int intResultCount)
        {
            DataTable dtStations = new DataTable();
            dtStations = objNearbyPointSvc.getNearbyDataService(dLat, dLong, intMaxAllowedDist, intMaxAllowedAlt, intResultCount);
            IAgWebService.iTimeZoneOffset = Convert.ToInt32(dtStations.Rows[0]["TimezoneOffset"]);
            IAgWebService.iDstOn = Convert.ToInt32(dtStations.Rows[0]["DstOn"]);
            return Convert.ToInt32(dtStations.Rows[0]["TimezoneOffset"]);
        }

        public DataTable getNodeList(string strModules)
        {
            return objSvcHandler.getWebServiceNodeList(strModules);
        }

        //Method to fetch the chart details
        /*Unit Implementation in Web Services - Begin*/
        public DataTable getChartData(double dStnLat, double dStnLong, int altitude, int maxAltDiff, int maxDistDiff, string strModule, string strSvcName, string strCul,string strUnit)
        /*Unit Implementation in Web Services - End*/
        {
            DataTable dtChartData = new DataTable();
            DataTable dtChart = new DataTable();
            dtChartData.Columns.Add("ModuleID");
            dtChartData.Columns.Add("ChartUrl");
            DataRow drChart;
            drChart = dtChartData.NewRow();
            drChart["ModuleID"] = "Chart";
            /*Unit Implementation in Web Services - Begin*/
            objChartPre.setChartInputValues(dStnLat, dStnLong, altitude, maxAltDiff, maxDistDiff, strModule, strSvcName, strCul, strUnit);
            /*Unit Implementation in Web Services - Begin*/

            //web service issue fix
            objChartSvc.setChartWebServiceValues(dStnLat, dStnLong, altitude, maxAltDiff, maxDistDiff, strSvcName,strModule);
            //drChart["ChartUrl"] = objChartPre.getChartData(dtChart); //objCommonUtil.toBase64(objChartSvc.getChartUrl());
            /* UAT Tracker - 522 - Chart image is not getting generated. - Begin */
            //string ChartUrl = objChartPre.getChartData(dtChart).Substring(1); 
            string ChartUrl = objChartPre.getChartData(dtChart).Substring(2);
            /* UAT Tracker - 522 - Chart image is not getting generated. - End  */
            ChartUrl = System.Configuration.ConfigurationManager.AppSettings["WSHost"].ToString() + ChartUrl;
            /**********IM01144192 - New Agricast webservice - chart URL  - Begin ***********************/
            //drChart["ChartUrl"] = objCommonUtil.toBase64(ChartUrl);
            drChart["ChartUrl"] = ChartUrl;
            /**********IM01144192 - New Agricast webservice - chart URL  - End ***********************/
            dtChartData.Rows.Add(drChart);
            return dtChartData;
        }


        //Method to set the input values
        public void setIconWebServiceValues(double dLat, double dLong, int alt, int maxAltDiff, int maxDistDiff, DateTime dSunrise, DateTime dSunset, string strServiceName, string strModuleName, string strCulCode)
        {
            objIconSvc.setIconWebServiceValues(dLat, dLong, alt, maxAltDiff, maxDistDiff, dSunrise, dSunset, strServiceName, strModuleName, strCulCode);

        }

     

        public void setServiceHandlerWebServiceValues(string strServiceName, string strModuleName)
        {
            objSvcHandler.setSvcHandlerWebSvcValues(strServiceName, strModuleName);
        }

        //Method to fetch the Icon details
        public List<IconData> getWeatherIcons(DTOIcon objParam, string strControlName)
        {
            DataTable dtIconData = new DataTable();           
            IconData icon; 
            List<IconData> iconlist = new List<IconData>();
            try
            {
                ArrayList al = new ArrayList();
                dtIconData = objIconSvc.getIconData(objParam, strControlName);
                //al = objIconSvc.getWeatherIcons(objParam, strControlName);
                DataTable dtIconList = objIconSvc.getWeatherIconsWithTooltip(objParam, strControlName);    
                DateTime startDate = DateTime.Parse(dtIconData.Rows[0][0].ToString()).Date;
                for (int j = 0; j < dtIconList.Rows.Count; j++)
                {
                    icon = new IconData();
                    string day = "";
                    string tooltip = System.Text.RegularExpressions.Regex.Replace(dtIconList.Rows[j][1].ToString(), "<.*?>", string.Empty);
                    if (objParam.iStep == 24)
                    {
                        day = startDate.AddHours(j * objParam.iStep).DayOfWeek.ToString();
                        icon.Date = day;
                        icon.IconToolTip = tooltip.Replace("{day}", day).Replace("{}", ""); 
                    }
                    else
                    {
                        day = startDate.AddHours(j * objParam.iStep).DayOfWeek.ToString();
                        icon.Date = startDate.AddHours(j * objParam.iStep).ToString();                       
                        icon.IconToolTip = tooltip.Replace("{day}", day).Replace("{}", ""); 

                    }
                    string IconURL = dtIconList.Rows[j]["ImageUrl"].ToString();
                    /********IM01144144 - New Agricast Webservices - icons URL - BEGIN ***************************/
                    IconURL=IconURL.Substring(IconURL.IndexOf("\\Images"));
                    //IconURL = System.Configuration.ConfigurationManager.AppSettings["WSHost"].ToString() + IconURL.Substring(IconURL.IndexOf("\\Images"));
                    IconURL = System.Configuration.ConfigurationManager.AppSettings["WSHost"].ToString() + IconURL.Substring(IconURL.IndexOf("\\") + 1);
                   
                    //icon.IconPath = objCommonUtil.toBase64(IconURL);
                    icon.IconPath = IconURL;
                    /********IM01144144 - New Agricast Webservices - icons URL - END ***************************/
                    iconlist.Add(icon);                    
                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error); 
            }
            return iconlist;
        }

        //Method to get the details of the tables module
        public Legend getLegendData(string strLegendName)
        {
            Legend leg = new Legend(); 
            StreamReader streamReader = new StreamReader(HttpRuntime.AppDomainAppPath + objSvcHandler.getLegendAttributes(strLegendName));
                string strLegendText = streamReader.ReadToEnd();
                streamReader.Close();
                leg.ModuleID = strLegendName;
                leg.HTMLString = strLegendText;
                return leg; 
        }

        //Method to get the Nearby stations details
        public DataTable getNeabyStations(double dLat, double dLong, int intMaxAllowedDist, int intMaxAllowedAlt, int intResultCount, string strLangID)
        {
            DataTable dt = new DataTable();
            dt = objNearbyPointSvc.getNearbyDataService(dLat, dLong, intMaxAllowedDist, intMaxAllowedAlt, intResultCount);
            //dt.Columns.Add("DistText");
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    string strText = objLocSearchSvc.getTranslatedText("Located", strLangID);
            //    dt.Rows[i]["DistText"] = strText + " " + getTextDirection(Convert.ToInt32(dt.Rows[i]["BearingDegrees"]), strLangID) + " at " + Convert.ToString(Math.Round(Convert.ToDouble(dt.Rows[i]["DistanceKm"].ToString()))) + "km";
            //}
            //dt.Columns.Remove("DataPointID");
            //dt.Columns.Remove("DistanceKm");
            //dt.Columns.Remove("TimezoneOffset");
            //dt.Columns.Remove("Altitude");
            //dt.Columns.Remove("BearingDegrees");
            //dt.Columns.Remove("DstOn");
            return dt;
        }

        //Method to get the Direction text by passing bearing degrees
        public string getTextDirection(int degrees, string strLangID)
        {
            // N,NE,E,SE,S,SW,W,NW
            /*       N (0)
             *  
             *   W (270) E (90)
             * 
             *       S (180)
             */
            string[] directArr = null;
            string strTranslatedText = objLocSearchSvc.getTranslatedText("ResDirections", strLangID);
            if (strTranslatedText != null && strTranslatedText.IndexOf(",") > 0)
            {
                directArr = strTranslatedText.Split(',');
                // check there are 8 directions
                if (directArr.Length != 8) directArr = new string[8];
            }
            else directArr = new string[8];
            while (degrees < 0) degrees += 360;
            while (degrees > 360) degrees -= 360;
            if (degrees >= 338 || degrees < 23) return "N";
            else if (degrees >= 23 && degrees < 68) return directArr[1]; // NE
            else if (degrees >= 68 && degrees < 113) return directArr[2]; // E
            else if (degrees >= 113 && degrees < 158) return directArr[3]; // SE
            else if (degrees >= 158 && degrees < 203) return directArr[4]; // S
            else if (degrees >= 203 && degrees < 248) return directArr[5]; // SW
            else if (degrees >= 248 && degrees < 293) return directArr[6]; // W
            else if (degrees >= 293 && degrees < 338) return directArr[7]; // NW
            else return "-";
        }


        //Method to fetch the table module details
        /*Unit Implementation in Web Services - Begin*/
        //Added a new parameter - strUnit
        public List<table> getTableData(double dStnLat, double dStnLong, int iAtl, int iMaxDist, int iMaxAltDiff, IRuleSets objIRuleset, string strNode, string strNodeName, string strCultureCode, string strServiceName, string strModuleName, string strLangID,string strUnit)
        /*Unit Implementation in Web Services - End*/
        {
            DataSet dsTbl = new DataSet();            
            objTableSvc.setTableWebServiceValues(dStnLat, dStnLong, iAtl, iMaxDist, iMaxAltDiff);
            /*Unit Implementation in Web Services - Begin*/
            dsTbl = objTblPre.getTableDataForService(strNode, strNodeName, objIRuleset, strServiceName, strModuleName, strCultureCode, strUnit);
            /*Unit Implementation in Web Services - End*/
            List<table> tablelist = new List<table>();
            table tbl;            
            tableRow tr = new tableRow();
            tableCell tc;
            HeaderRow hr1 = new HeaderRow();
            HeaderRow hr2 = new HeaderRow();
            HeaderCell hc;                   
           
            if (dsTbl.Tables[0].TableName.ToLower().Contains("header") && dsTbl.Tables.Count == 2)
            {
                tbl = new table();
                tbl.HeaderRows = new List<HeaderRow>();
                tbl.TableName = dsTbl.Tables[1].TableName;

                hr1.HeaderCells = new List<HeaderCell>();
                hr2.HeaderCells = new List<HeaderCell>();
                int count = 0;
                string day = "";
                string prevVal = "";
                foreach (DataRow dr in dsTbl.Tables[0].Rows)
                {
                    if (count == 0)
                    {
                        hc = new HeaderCell();
                        hc.Value = "";
                        hc.Colspan = "1";
                        hr1.HeaderCells.Add(hc);
                        hc = new HeaderCell();
                        hc.Value = "";
                        hc.Colspan = "1";
                        hr2.HeaderCells.Add(hc);
                    }
                    if (prevVal != dr[0].ToString().Split(',')[0] && !(dr[0].ToString().Split(',')[1].EndsWith("0")))
                    {
                        hc = new HeaderCell();
                        day = DateTime.Parse(dr[0].ToString().Split(',')[0].ToString()).DayOfWeek.ToString();
                        hc.Value = day;
                        hc.Colspan = dr[1].ToString();
                        hr1.HeaderCells.Add(hc);
                        prevVal = dr[0].ToString().Split(',')[0];
                    }
                    HeaderCell hc1 = new HeaderCell();
                    hc1.Value = dr[0].ToString().Split(',')[1].ToString();
                    hc1.Colspan = "1";
                    hr2.HeaderCells.Add(hc1);                    
                    count++;
                }
                tbl.HeaderRows.Add(hr1);
                tbl.HeaderRows.Add(hr2);

                tbl.tableRows = new List<tableRow>();
                foreach (DataRow dr in dsTbl.Tables[1].Rows)
                {
                    tr = new tableRow();
                    tr.tableCells = new List<tableCell>();
                    for (int i = 0; i < (dsTbl.Tables[1].Columns.Count/3); i++)
                    {
                        tc = new tableCell();
                        tc.Value = dr[0 + (i * 3)].ToString();
                        tc.ToolTip = dr[1 + (i * 3)].ToString();
                        tc.bgColor = dr[2 + (i * 3)].ToString();
                        if (i == 0)
                        {
                            tc.Color = "";
                            tc.CellImage = "";
                        }
                        else
                        {
                            Color bgColor = new Color();
                            bgColor = ColorTranslator.FromHtml(tc.bgColor);
                            tc.Color = ColorTranslator.FromHtml("#" + objCommonUtil.ContrastColor(bgColor).Name).Name;
                            /********IM01144144 - New Agricast Webservices - icons URL - BEGIN ***************************/
                            //tc.CellImage = objCommonUtil.toBase64(GetImage(tc.Value, tc.Color, tc.bgColor));
                            tc.CellImage = GetImage(tc.Value, tc.Color, tc.bgColor);
                            /********IM01144144 - New Agricast Webservices - icons URL - END ***************************/
                        }
                        if (i == 0)
                            tc.Header = "true";
                        else
                            tc.Header = "";                        
                        tr.tableCells.Add(tc);
                    }
                    tbl.tableRows.Add(tr);
                }

                tablelist.Add(tbl);
            }
            else
            {
                foreach (DataTable dt in dsTbl.Tables)
                {
                    tbl = new table();
                    tbl.HeaderRows = new List<HeaderRow>();
                    tbl.TableName = dt.TableName.ToString();

                    hr1.HeaderCells = new List<HeaderCell>();                    
                    for (int i = 0; i < (dt.Columns.Count / 3);i++ )
                    {
                        hc = new HeaderCell();
                        if (i == 0)
                            hc.Value = objLocSearchSvc.getTranslatedText("hour", strLangID);
                        else
                            hc.Value = (i-1).ToString();
                        hc.Colspan = "1";
                        hr1.HeaderCells.Add(hc);                         
                    }
                    tbl.HeaderRows.Add(hr1);


                    tbl.tableRows = new List<tableRow>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        tr = new tableRow();
                        tr.tableCells = new List<tableCell>();
                        for (int i = 0; i < (dr.Table.Columns.Count/3); i++)
                        {
                            tc = new tableCell();
                            tc.Value = dr[0 + (i * 3)].ToString();
                            tc.ToolTip = dr[1 + (i * 3)].ToString();
                            tc.bgColor = dr[2 + (i * 3)].ToString();
                            if (i == 0)
                            {
                                tc.Color = "";
                                tc.CellImage = "";
                            }
                            else
                            {
                                Color bgColor = new Color();
                                bgColor = ColorTranslator.FromHtml(tc.bgColor);
                                tc.Color = ColorTranslator.FromHtml("#" + objCommonUtil.ContrastColor(bgColor).Name).Name;
                                /********IM01144144 - New Agricast Webservices - icons URL - BEGIN ***************************/
                                //tc.CellImage =  objCommonUtil.toBase64(GetImage(tc.Value, tc.Color, tc.bgColor));
                                tc.CellImage = GetImage(tc.Value, tc.Color, tc.bgColor);
                                /********IM01144144 - New Agricast Webservices - icons URL - END ***************************/
                            }
                            if (i == 0)
                                tc.Header = "true";
                            else
                                tc.Header = "";
                            tr.tableCells.Add(tc);
                        }
                        tbl.tableRows.Add(tr);
                    }

                    tablelist.Add(tbl);
                }
            }
             return tablelist;
        }
         

        public ArrayList GetServiceDetails()
        {
            ArrayList alError = new ArrayList();
            alError.Add("AgriCastService");
            alError.Add("AgriCastService");
            alError.Add(string.Empty);
            return alError;
        }

        public void SaveServiceAuditData(IDictionary dict)
        {
            objCommonUtil.SaveServiceAuditData(dict);
        }


        /// <summary>
        /// This method creates and image based on its backcolor , text and forecolor  and returns the path
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textColor"></param>
        /// <param name="backColor"></param>
        /// <returns></returns>
        private string GetImage(String text, string textColor, string backColor)
        {
            int height= 25;
            int width = 35;
            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(backColor))
                return "";
            string file_name = width + "x" + height + "_" + backColor.Replace("#","") + "_" + text.Replace(".","-") + ".gif";
            string folderPath = HttpRuntime.AppDomainAppPath + @"temp\ws\";
            /* UAT Tracker - 522 - Chart image is not getting generated. - Begin  */
            //string subfolderpath = @"temp\ws\";
            string subfolderpath = @"temp/ws/";
            /* UAT Tracker - 522 - Chart image is not getting generated. - End  */
            string ImagePath = System.Configuration.ConfigurationManager.AppSettings["WSHost"].ToString() + subfolderpath + file_name;

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            if (!File.Exists(folderPath + file_name))
            {
                //first, create a dummy bitmap just to get a graphics object 
                Bitmap img = new Bitmap(width, height);
                Graphics drawing = Graphics.FromImage(img);

                // Create the Font object for the image text drawing.    
                Font font = new Font("Arial", 11, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);               
                
                //paint the background 
                drawing.Clear(ColorTranslator.FromHtml(backColor));

                //create a brush for the text 
                Brush textBrush = new SolidBrush(ColorTranslator.FromHtml(textColor));

                drawing.DrawString(text, font, textBrush, 0, 0);
                drawing.Save();

                textBrush.Dispose();
                drawing.Dispose();

                img.Save(folderPath + file_name);
            }
            return ImagePath;

        } 
    }
}