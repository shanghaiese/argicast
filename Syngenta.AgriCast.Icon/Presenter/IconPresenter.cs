using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Icon.View;
using Syngenta.AgriCast.Icon.Service;
using Syngenta.AgriCast.Icon.DTO;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Common;
 
namespace Syngenta.AgriCast.Icon.Presenter
{
    public class IconPresenter
    {
        ServiceHandler objSvcHandler = new ServiceHandler();
        IconService objIconSvc = new IconService();
        DTOIcon objDTOSIcon = new DTOIcon();
        ServiceInfo objSvcInfo;
        string controlName;
        IIcon objIIcon;
        ServicePresenter objSvcPre = new ServicePresenter();
        CommonUtil objComUtil = new CommonUtil();
        /// <summary>
        /// constructor
        /// </summary>
        public IconPresenter()
        {

        }

        public IconPresenter(IIcon IIConObj,string Name)
        {
            if (IIConObj != null)
            {
                objIIcon = IIConObj;
            }
            controlName = Name;
        }

        public void getIconData()
        {
            try
            { 
                objIIcon.dtIconList = objIconSvc.getWeatherIconsWithTooltip(objDTOSIcon, controlName);            
                objIIcon.iDesiredNoOfDays = objDTOSIcon.iNoOfdays;
                objIIcon.iColumnsPerDay = 24 / objDTOSIcon.iStep;
                objIIcon.iFeedbackEnabled = objDTOSIcon.strFeedbackEnabled;
                objIIcon.plotHeader = objDTOSIcon.headerRow;
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.ICONS_DISPLAY_FAILURE) + ex.Message.ToString();
            }
        }

        //public void setWebServiceValues(double dLat, double dLong, DateTime dSunrise, DateTime dSunset)
        //{
        //    try
        //    {
        //        objIconSvc.setIconWebServiceValues(dLat, dLong, dSunrise, dSunset);
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.Current.Session["ErrorMessage"] = "The following error occured while setting web service values: " + ex.Message.ToString();
        //    }
        //}

        public void getCultureCode()
        {
            try
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                objIIcon.strCulCode = objSvcInfo.Culture;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.CULTURE_LOADFAILURE) +" : "+ ex.Message.ToString();
            }
        }
        public void getTranslatedText(string strLabelName, string strCultureCode)
        {
           objIIcon.iTransText =  objIconSvc.getTranslatedText(strLabelName, strCultureCode);
        }


    }
}