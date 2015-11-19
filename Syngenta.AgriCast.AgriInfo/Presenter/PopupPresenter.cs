 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Web.SessionState;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.AgriInfo.View;
using Syngenta.AgriCast.AgriInfo.Service;
using Syngenta.AgriCast.Common;

namespace Syngenta.AgriCast.AgriInfo.Presenter
{
    public class PopupPresenter
    {
        IPopup IPopupObj;
        AgriInfoService agriSvc = new AgriInfoService();
        CommonUtil objCommonUtil = new CommonUtil();
        public PopupPresenter(IPopup IPop)
        {
            if (IPop != null)
            {
                IPopupObj = IPop;
            }
        }
        public PopupPresenter()
        {
           
        }
       
        ServiceHandler ServiceObj = new ServiceHandler();

        public void GetAdvancedOptions(string series, string serie)
        {
            try
            {

                IPopupObj.al = ServiceObj.GetAdvancedOptions(series, serie);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.AGRIINFO_ADVOPTIONS) + ":" + ex.Message.ToString();
            }
        }
        public void getTranslatedText(string strLabelName, string strCultureCode)
        {
            IPopupObj.strTransText = agriSvc.getTranslatedText(strLabelName, strCultureCode);
        }
           
    }
}
    