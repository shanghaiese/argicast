using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;

public partial class Mobile_MobileMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {

        ServiceInfo objSvcInfo;
        CommonUtil objComUtil = new CommonUtil();
        if (Session == null || Session["serviceInfo"] == null)
        {

            objSvcInfo = new ServiceInfo();
        }
        else
        {
            objSvcInfo = (ServiceInfo)Session["serviceInfo"];
        }


        //set the master page controls translated text

        lnkTnC.InnerHtml = objComUtil.getTransText(lnkTnC.Attributes["name"]);
        lnkDisclaimer.InnerText = objComUtil.getTransText(lnkDisclaimer.Attributes["name"]);
        lnkFullSite.InnerText = objComUtil.getTransText(lnkFullSite.Attributes["name"]);
        btnBack.InnerHtml = objComUtil.getTransText("Back");

    }
}
