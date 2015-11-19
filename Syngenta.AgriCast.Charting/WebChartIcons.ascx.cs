/* Agricast CR - R6 - Add wind icons and legend for Humidity - Begin */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.Charting.Presenter;
using Syngenta.AgriCast.Common.Presenter;
using System.Data;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common;
using Syngenta.AgriCast.Charting.View;
using ChartFX.WebForms;
using System.Globalization;
using Syngenta.AgriCast.Charting.Service;
using System.Text;
using System.Collections;
using System.IO;
using System.Configuration;

namespace Syngenta.AgriCast.Charting
{
    public partial class WebChartIcons : System.Web.UI.UserControl, IChartIcons
    {
        #region Declarations

        ChartIconsPresenter objChartIconsPre;
        ServicePresenter objSvcPre = new ServicePresenter();
        CommonUtil objCommonUtil = new CommonUtil();
        LocationInfo objLocInfo;
        CultureInfo VariantCulture;
        DataSet dsTableSeries;

        List<string[]> al;
        DataTable dtSeriesDetails;
        DataTable dtTransposed;
        int step = 1;
        int NumOfDays;
        string AllignDetails;
        string CultureCode;

        const string THREEHOURLY = "3hourly";
        const string SIXHOURLY = "6hourly";
        const string EIGHTHOURLY = "8hourly";
        const string TWELVEHOURLY = "12hourly";
        const string HOURLY = "hourly";

        #endregion

        public WebChartIcons()
        {
            /*Wind Icon as a Sepearate component -- BEGIN*/
            //Name = "vew_daily";
            //Node = "tblSeriesRows";
            /*Wind Icon as a Sepearate component -- END*/
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            var host = HttpContext.Current.Request.Url.Scheme + "://"
                         + HttpContext.Current.Request.Url.Authority
                         + HttpRuntime.AppDomainAppVirtualPath;
            // fix the issue of when AppDomainAppVirtualPath come "/"
            if (host.EndsWith("/"))
                host = host.Substring(0, host.Length - 1);
            /*Wind Icon as a Sepearate component -- BEGIN*/
            //var imgPath = host + "/temp/icons/";
            var imgPath = host + "/temp/icons/";
            /*Wind Icon as a Sepearate component -- END*/

            windIconsLabel.Text = objCommonUtil.getTransText("Wind Icons");
            weatherLabel.Text = objCommonUtil.getTransText("ResStrongWind");
            // goto GetWindLabel(dt)

            img15.ImageUrl = imgPath + Utils.CreateArrow(180f, 15f);
            img40.ImageUrl = imgPath + Utils.CreateArrow(180f, 40f);
            img75.ImageUrl = imgPath + Utils.CreateArrow(180f, 75f);
            img120.ImageUrl = imgPath + Utils.CreateArrow(180f, 120f);

            northDir.Text = objCommonUtil.getTransText("N");
            westDir.Text = objCommonUtil.getTransText("W");
            southDir.Text = objCommonUtil.getTransText("S");
            eastDir.Text = objCommonUtil.getTransText("E");
            imgDirection.ImageUrl = imgPath + Utils.CreateArrow(225f, 75f);

            humidityLabel.Text = objCommonUtil.getTransText("ResHumidity");
            //}
        }

        private void loadIcons()
        {
            objChartIconsPre = new ChartIconsPresenter(this, Name);
            objChartIconsPre.getCultureCode();
            VariantCulture = new CultureInfo(strCulCode);


            /*Wind Icon as a Sepearate component -- BEGIN*/
            //objChartIconsPre.ReadAllignment(Node);
            objChartIconsPre.getWindIconSettings(Node);

            if (dtWindIconSettings != null && dtWindIconSettings.Rows.Count == 1)
            {


                //numOfDays = Convert.ToInt32(AllignDetails.Split(',')[1]);
                //string datasource = AllignDetails.Split(',')[2];
                //string aggregation = AllignDetails.Split(',')[5];
                //int start = Convert.ToInt32(AllignDetails.Split(',')[3]);
                //int end = Convert.ToInt32(AllignDetails.Split(',')[4]);
                numOfDays = Convert.ToInt32(dtWindIconSettings.Rows[0]["NumOfDays"].ToString());
                string datasource = dtWindIconSettings.Rows[0]["dataSource"].ToString();
                string aggregation = dtWindIconSettings.Rows[0]["temporalAggregation"].ToString();
                int start = Convert.ToInt32(dtWindIconSettings.Rows[0]["startDate"].ToString());
                int end = Convert.ToInt32(dtWindIconSettings.Rows[0]["endDate"].ToString());

                //Set the Iconswidth from the settings. 
                //this is used in DisplayiconsTable method.
                IconChartWidth = Convert.ToInt32(dtWindIconSettings.Rows[0]["width"].ToString());

                DisplayIconsTables(Name, Node, aggregation, datasource, start, end);
            }

        }

        protected void Page_PreRender(object sender, EventArgs args)
        {
            try
            {
                objLocInfo = (LocationInfo)HttpContext.Current.Session["objLocationInfo"];
                if (objLocInfo.DataPointInfo != null && objLocInfo.DataPointInfo.stationLatitude != 0 && objLocInfo.DataPointInfo.stationLongitude != 0 && objLocInfo.DataPointInfo.stationName != "")
                {
                    loadIcons();
                }
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.WINDICONS_LOAD_FAILURE) + ":" + ex.Message.ToString();
            }
        }

        private string GetWindLabel(DataTable dt)
        {
            var labelText = string.Empty;

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString().Contains("windspeed"))
                {
                    labelText = objChartIconsPre.getTranslatedText(alSeries[i].ToString().Split(',')[3], CultureCode);
                }
            }

            return labelText.Replace("(", "<br />(");
        }

        public void DisplayIconsTables(string name, string allign, string aggregation, string datasource, int start, int end)
        {
            try
            {
                objChartIconsPre.CreateTables(name, allign, aggregation, datasource, start, end);

                if (aggregation.ToLower() != HOURLY)
                {

                    if (aggregation.ToLower() == THREEHOURLY)
                    {
                        step = 3;
                    }
                    else if (aggregation.ToLower() == SIXHOURLY)
                    {
                        step = 6;
                    }
                    else if (aggregation.ToLower() == EIGHTHOURLY)
                    {
                        step = 8;
                    }
                    else if (aggregation.ToLower() == TWELVEHOURLY)
                    {
                        step = 12;
                    }
                    else
                    {
                        step = 24;
                    }
                    objChartIconsPre.GenerateTransposedtblSeriesRows(dtByDays, numOfDays, aggregation, false , step);
                    

                    weatherLabel.Text = GetWindLabel(dt);

                    var builder = new StringBuilder();
                    var trWindIcon = new StringBuilder();
                    var trHumidity = new StringBuilder();

                    // 84 is total width of left gap & right gap in chart
                    // icon chart width 
                    int innerChartWidth = IconChartWidth - 85;
                    var zoomedInnerChartWidth = Convert.ToInt32(ConfigurationManager.AppSettings["bigChartWidth"]) - 84;
                    var isZoomedPage = this.Page.Request.Url.LocalPath.Contains("Zoomed");
                    var tableWidth = isZoomedPage ? zoomedInnerChartWidth : innerChartWidth;

                    var imgUrls = objChartIconsPre.getIcons(dt);
                    var humidities = objChartIconsPre.getHumidities(dt);
                    var totalStrip = numOfDays * (24 / step);

                    if (imgUrls.Count != totalStrip)
                    {
                        tableWidth = (isZoomedPage ? imgUrls.Count * zoomedInnerChartWidth : Math.Max(imgUrls.Count, humidities.Count) * innerChartWidth) / totalStrip;
                    }

                    builder.AppendFormat("<table id= \"tblwindicons\" class=\"{1}\" style=\"width:{0};\"><tbody>",
                        tableWidth + "px",
                        (imgUrls.Count == totalStrip) ? "" : "incomplete-data");
                    trWindIcon.Append("<tr>");
                    foreach (var img in imgUrls)
                    {
                        var host = HttpContext.Current.Request.Url.Scheme + "://"
                          + HttpContext.Current.Request.Url.Authority
                          + HttpRuntime.AppDomainAppVirtualPath;
                        if (string.IsNullOrEmpty(img))
                            trWindIcon.Append("<td>&nbsp;</td>");
                        else
                        {

                            /*Wind Icon as a Sepearate component -- BEGIN*/
                            var imgUrl = host + Path.Combine("/temp/icons/", img);
                            //var imgUrl = Path.Combine("../../temp/icons/", img);

                            /*Wind Icon as a Sepearate component -- END*/
                            trWindIcon.AppendFormat("<td><img src=\"{0}\" alt=\"\" /></td>", imgUrl);
                        }
                    }
                    trWindIcon.Append("</tr>");

                    trHumidity.Append("<tr>");
                    foreach (var humdt in humidities)
                    {
                        trHumidity.AppendFormat("<td><strong>{0}</strong></td>", (string.IsNullOrEmpty(humdt) ? "00" : humdt));
                    }

                    trHumidity.Append("</tr>");
                    builder.Append(trHumidity).Append(trWindIcon);
                    builder.Append("</tbody></table>");

                    iconsContainer.InnerHtml = builder.ToString();
                }
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.WINDICONS_DISPLAY_FAILURE) + ":" + ex.Message.ToString();
            }
        }

        #region //IChartIcons Members

        public string strAllignDetails
        {
            get
            {
                return AllignDetails;
            }
            set
            {
                AllignDetails = value;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public string Node
        {
            get;
            set;
        }

        public DataSet dsSeriesData
        {
            get
            {
                return dsTableSeries;
            }
            set
            {
                dsTableSeries = value;
            }
        }

        public DataTable dtWindIcons
        {
            get;
            set;
        }

        public string strCulCode
        {
            get
            {
                return CultureCode;
            }
            set
            {
                CultureCode = value;
            }
        }
        public DataTable dt
        {
            get
            {
                return dtTransposed;
            }
            set
            {
                dtTransposed = value;
            }
        }

        public int Step
        {
            get
            {
                return step;
            }
            set
            {
                step = value;
            }
        }

        public int numOfDays
        {
            get
            {
                return NumOfDays;
            }
            set
            {
                NumOfDays = value;
            }
        }
        public List<string[]> alSeriesLegend
        {
            get
            {
                return al;
            }
            set
            {
                al = value;
            }
        }

        public DataTable dtSeries
        {
            get
            {
                return dtSeriesDetails;
            }
            set
            {
                dtSeriesDetails = value;
            }

        }

        public DataTable dtTableSeries
        {
            get;
            set;
        }
        public DataTable dtTableLegends
        {
            get;
            set;
        }
        public DataTable dtByDays
        {
            get;
            set;
        }

        public ArrayList alSeries
        {
            get;
            set;
        }

        /* UAT issue - icon chart issue - Jerrey - Start */
        public int IconChartWidth
        {
            get;
            set;
        }
        /* UAT issue - icon chart issue - Jerrey - End */

        /*Wind Icon as a Sepearate component -- END*/
        public DataTable dtWindIconSettings
        {
            get;
            set;

        }
        /*Wind Icon as a Sepearate component -- END*/
        #endregion
    }
}
/* Agricast CR - R6 - Add wind icons and legend for Humidity - End */
