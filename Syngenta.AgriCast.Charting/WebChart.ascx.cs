using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.Charting.Presenter;
using Syngenta.AgriCast.Charting.View;
using ChartFX.WebForms;
using System.Data;
using System.IO;
using System.Web.UI.HtmlControls;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common;

namespace Syngenta.AgriCast.Charting
{
    public partial class WebChart : System.Web.UI.UserControl, IChart
    {
        ChartPresenter objPresenter;
        ServicePresenter objSvcPre = new ServicePresenter();
        DataTable dtData = new DataTable();
        Chart Chart1 = new Chart();
        LocationInfo objLocInfo;
        CommonUtil objCommon = new CommonUtil();
        //string cCode;        
        //string _imageUrl;
        //string _title;
        //string _footerText;
        const string LABEL_RATING_TEXT = "Rate it";

        protected void Page_Load(object sender, EventArgs e)
        {
            //try
            //{
            //    //objPresenter = new ChartPresenter(this,Name);  
            //    //objPresenter.getChartData(agriInfoData);
            //    //Chart1.ClientIDMode = ClientIDMode.Static;
            //    //imgChart.Src = imageUrl;      
            //    loadData();

            //}
            //catch (Exception ex)
            //{
            //    AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            //    AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            //    HttpContext.Current.Session["ErrorMessage"] = "The following error occured while loading charts : " + ex.Message.ToString();
            //    return;
            //}

            chartFeedback.Visible = false;
        }

        private void loadData()
        {
            objPresenter = new ChartPresenter(this, Name);
            objPresenter.getChartData(agriInfoData);
            Chart1.ClientIDMode = ClientIDMode.Static;
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                imgChart.Src = imageUrl;
                /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - Begin */
                /* 3.3.1	Charting component should have zooming enabled. */
                var host = HttpContext.Current.Request.Url.Scheme + "://"
                           + HttpContext.Current.Request.Url.Authority
                           + HttpRuntime.AppDomainAppVirtualPath;
                // fix the issue of when AppDomainAppVirtualPath come "/"
                if (host.EndsWith("/"))
                    host = host.Substring(0, host.Length - 1);

                /*IM01166162 - AgriInfo UI Issues - Begin*/
                // redirection issue.
                //Add additonal param to JS method to identify if its agriinfo chart or forecast.
                //provide the settings as per the flag
                bool isAgriInfo = true;
                if (agriInfoData == null || agriInfoData.Rows.Count == 0)
                    isAgriInfo = false;
                //imgChart.Attributes.Add("onclick", String.Format("showBigImage('{0}/pub/{1}/ChartZoomedImage.aspx?img={2}');"
                /*IM01166162 - AgriInfo UI Issues - Begin*/

                /*Security Issue -begin*/
                //imgChart.Attributes.Add("onclick", String.Format("showBigImage('{0}/pub/{1}/ChartZoomedImage.aspx?img={2}','{3}')"
                //           , host, objLocInfo.ServiceInfo.ServiceName
                //           , HttpContext.Current.Server.HtmlEncode(host + bigImageUrl.Substring(1)), isAgriInfo));

                imgChart.Attributes.Add("onclick", String.Format("showBigImage('{0}/pub/{1}/ChartZoomedImage.aspx?img={2}','{3}')"
                          , host, objLocInfo.ServiceInfo.ServiceName
                          , HttpContext.Current.Server.UrlEncode("~" + bigImageUrl.Substring(1))
                          , isAgriInfo));
                /*Security Issue -End*/
                /*IM01246233 :- New Agricast - missing translation tags - Begin*/
                //Add the title as transtext
                imgChart.Attributes.Add("title",objCommon.getTransText(Constants.CHART_ZOOM_TITLE));
                    
                    /*IM01246233 :- New Agricast - missing translation tags - End */
                /*IM01166162 - AgriInfo UI Issues - END*/

                /*IM01166162 - AgriInfo UI Issues - End*/
                /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - End */

                /* Chart DIV alignment issue - jerrey - Start */
                if (!string.IsNullOrEmpty(cssStyle))
                {
                    string originalCss = chrtDiv.Attributes["class"];
                    chrtDiv.Attributes["class"] = string.Format("{0} {1}", originalCss, cssStyle);
                }
                /* Chart DIV alignment issue - jerrey - End */
            }
        }

        protected void Page_prerender(object sender, EventArgs e)
        {
            try
            {
                //objPresenter = new ChartPresenter(this,Name);  
                //objPresenter.getChartData(agriInfoData);
                //Chart1.ClientIDMode = ClientIDMode.Static; 
                //imgChart.Src = imageUrl;
                objLocInfo = (LocationInfo)HttpContext.Current.Session["objLocationInfo"];
                //if (objLocInfo.DataPointInfo != null && !IsEmpty<DataPointInfo>(objLocInfo.DataPointInfo))
                if (objLocInfo.DataPointInfo != null && objLocInfo.DataPointInfo.stationLatitude != 0 && objLocInfo.DataPointInfo.stationLongitude != 0 && objLocInfo.DataPointInfo.stationName != "")
                {
                    loadData();
                    //Enabling or disabling feedback based on config setting
                    chartFeedback.Visible = (hasFeedback) ? true : false;
                    if (cultureCode != null)
                        objPresenter.getTranslatedText(LABEL_RATING_TEXT, cultureCode);
                    lblRating.InnerText = transText;
                    imgChart.Attributes.Add("class", "show imgArrow");
                }
                else
                {
                    imgChart.Attributes.Add("class", "hide");
                }

            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objCommon.getTransText(Constants.CHART_LOAD_FAILURE) + ex.Message.ToString();
                return;
            }
        }

        private bool IsEmpty<T>(dynamic obj)
        {
            object property_value = null;

            bool flag = false;

            System.Reflection.PropertyInfo[] properties_info = typeof(T).GetProperties();
            //System.Reflection.PropertyInfo property_info = default(System.Reflection.PropertyInfo); 

            foreach (System.Reflection.PropertyInfo prop in properties_info)
                if (prop != null)
                {
                    property_value = prop.GetValue(obj, null);
                    switch (prop.PropertyType.Name.ToString().ToLower())
                    {
                        case "int32": if (property_value != null)
                                flag = Int32.Parse(property_value.ToString()) == 0;
                            break;
                        case "double": if (property_value != null)
                                flag = double.Parse(property_value.ToString()) == 0.0;
                            break;
                        case "string": if (property_value != null)
                                flag = property_value.ToString() == "";
                            break;
                        case "datetime": if (property_value != null)
                                flag = DateTime.Parse(property_value.ToString()) == DateTime.MinValue || DateTime.Parse(property_value.ToString()) == DateTime.MaxValue;
                            break;
                    }
                }

            return flag;
        }

        #region IChart Members

        public string title
        {
            get;
            set;
        }

        public string footerText
        {
            get;
            set;
        }

        public string cultureCode
        {
            get;
            set;
        }

        public string imageUrl
        {
            get;
            set;
        }

        /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - Begin */
        /* 3.3.1	Charting component should have zooming enabled. */
        public string bigImageUrl
        {
            get;
            set;
        }
        /* Agricast CR - R3 - Web service changes (background Image for each cell and color) and chart zooming feature - End */

        public DataTable serieData
        {
            get;
            set;
        }

        public Chart fChart
        {
            get { return Chart1; }
            set { Chart1 = value; }
        }
        public bool hasFeedback
        {
            get;
            set;
        }
        public string transText
        {
            get;
            set;
        }

        /* alignment issue for agriInfo chart - jerrey - Start */
        public string cssStyle
        {
            get;
            set;
        }
        /* alignment issue for agriInfo chart - jerrey - End */

        #endregion

        public string Name
        {
            get;
            set;
        }

        public DataTable agriInfoData
        {
            get;
            set;
        }

    }
}