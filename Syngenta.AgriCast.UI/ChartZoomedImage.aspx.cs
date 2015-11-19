using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.Icon;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.View;
using Syngenta.AgriCast.Common.Service;
using System.Collections;
using System.Web.UI.HtmlControls;
using Syngenta.AgriCast.Charting;

public partial class ChartZoomedImage : System.Web.UI.Page, IShowImage
{
    Controls common = new Controls();
    DataPointInfo objDataPointInfo;
    LocationInfo objLocInfo;
    ServiceInfo objSvcInfo;
    ServicePresenter objSvcPre;
    CommonUtil objCommonUtil = new CommonUtil();

    #region Properties
    public List<string[]> alNodeList
    {
        get;
        set;
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session == null || Session["serviceInfo"] == null)
            objSvcInfo = new ServiceInfo();
        else
            objSvcInfo = (ServiceInfo)Session["serviceInfo"];

        objDataPointInfo = DataPointInfo.getDataPointObject;
        objLocInfo = new LocationInfo();

        HttpCookie locCookie = Request.Cookies["ais_LocationInfo"];
        if (locCookie != null)
        {
            LocationInfo newInfo = objLocInfo.DeserializeCookieString(locCookie.Value);
            if (newInfo != null)
            {
                Session["objLocationInfo"] = newInfo;
                objLocInfo = newInfo;
            }
            if (Session["AuditData"] != null)
            {
                IDictionary dict = (IDictionary)Session["AuditData"];
                dict["locSearchType"] = "By Cookie";
                Session["AuditData"] = dict;
            }
        }

        objSvcPre = new ServicePresenter(this);

        //set the Cutlure code from ObjLocation object to Service info
        //this is used in Login page for tranlsation
        if (objLocInfo != null && objLocInfo.ServiceInfo != null)
            objSvcInfo.Culture = objLocInfo.ServiceInfo.Culture;

        objSvcPre.getNodeList1("servicePage");
        // get weather icon info
        var iconNode = alNodeList.FirstOrDefault(q => q[0].ToString() == "icon");
        if (iconNode != null)
        {
            common.LoadCtrl<Icon>(this.Page, weatherIcons, "Icon", iconNode[1].ToString(), "");
        }
        /* Agricast CR - R6 - Add wind icons and legend for Humidity - Begin */

        /*Wind icons a seperate entity - BEGIN*/
        var windIconNode = alNodeList.FirstOrDefault(q => q[0].ToString() == "windIcon");
        if (windIconNode != null)
        {
            //common.LoadCtrl(this.Page, windIcons, "WebChartIcons");
            common.LoadCtrl<WebChartIcons>(this.Page, windIcons, "WebChartIcons", windIconNode[1].ToString(), "");
        }
        /*Wind icons a seperate entity - END*/
        /* Agricast CR - R6 - Add wind icons and legend for Humidity - End */ 

        var imgPath = HttpUtility.UrlDecode(Request.QueryString["img"]).Replace("'","").Replace('>',' ').Replace('<', ' ').Replace('"',' ').Replace(':',' ').Replace(';',' ');
        if (!string.IsNullOrEmpty(imgPath))
        {
            imgPath = Server.HtmlDecode(imgPath);
            if (!IsPostBack)
            {
                imgBigger.ImageUrl = imgPath;
            }
        }
    }
}