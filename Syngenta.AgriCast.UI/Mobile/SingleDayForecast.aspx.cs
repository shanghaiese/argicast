using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common.Service;

public partial class Mobile_SingleDayForecast : System.Web.UI.Page
{
    string strWindDir = string.Empty;
    string strWindSpeed = string.Empty;
    string strPrecip = string.Empty;
    string strTempMax = string.Empty;
    string strTempMin = string.Empty;
    string strImageURl = string.Empty;
    ServicePresenter objSerPre = new ServicePresenter();
    CommonUtil objCommonUtil = new CommonUtil();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //Check the Page Query string
            if (!string.IsNullOrEmpty(Page.Request.QueryString["WindDir"]))
            {
                strWindDir = Page.Request.QueryString["WindDir"];

            }
            if (!string.IsNullOrEmpty(Page.Request.QueryString["WindSpeed"]))
            {
                strWindSpeed = Page.Request.QueryString["WindSpeed"];

            }

            if (!string.IsNullOrEmpty(Page.Request.QueryString["Precip"]))
            {
                strPrecip = Page.Request.QueryString["Precip"];

            }


            if (!string.IsNullOrEmpty(Page.Request.QueryString["TempMax"]))
            {
                strTempMax = Page.Request.QueryString["TempMax"];

            }


            if (!string.IsNullOrEmpty(Page.Request.QueryString["TempMin"]))
            {
                strTempMin = Page.Request.QueryString["TempMin"];

            }


            if (!string.IsNullOrEmpty(Page.Request.QueryString["ImageURL"]))
            {
                strImageURl = Page.Request.QueryString["ImageURL"];

            }

            //Replace all the Place Holders
            //phWind.InnerText = strWindDir;

            // form the URL of WInd Direction Image
            phWindDirection.InnerHtml = "<img src=\"../../Images/" + strWindDir + ".gif\" alt=\"No Image of WindDirection\" />";

            //Wind Speed
            phWindSpeed.InnerText = strWindSpeed;

            //Precip
            phPrecip.InnerText = strPrecip;

            //Temp Max
            phTempMax.InnerText = strTempMax;

            //Temp Min
            phTempMin.InnerText = strTempMin;

            //Image URL
            phImageUrl.InnerHtml = "<img src=\"" + strImageURl + "\" alt=\"No Image \"/>";
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSerPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_LOAD_FAILURE) + " : " + ex.Message.ToString();
            return;
        }
    }
}