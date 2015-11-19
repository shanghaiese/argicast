using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common;

public partial class Mobile_MobileForecast : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ServicePresenter objSerPre = new ServicePresenter();
        CommonUtil objCommonUtil = new CommonUtil();
        try
        {
            PropertyInfo propertyInfo = ucForeCast.GetType().GetProperty("Filter");
            propertyInfo.SetValue(ucForeCast, Convert.ChangeType(false, propertyInfo.PropertyType), null);
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSerPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_LOAD_FAILURE) + " : " + ex.Message.ToString();

        }
    }
}  