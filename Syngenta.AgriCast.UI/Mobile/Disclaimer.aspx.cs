using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common.DTO;

public partial class Mobile_Disclaimer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ServiceInfo objSvc = ServiceInfo.ServiceConfig;
        ServicePresenter objSvcPre = new ServicePresenter();
        lblTitle.Text = objSvcPre.getTranslatedText("ResDisclaimerHeader", (objSvc != null && objSvc.Culture != "") ? objSvc.Culture : "en-GB");
        lblDisclaimer.Text = objSvcPre.getTranslatedText("ResDisclaimer", (objSvc != null && objSvc.Culture != "") ? objSvc.Culture : "en-GB"); 
    } 
}