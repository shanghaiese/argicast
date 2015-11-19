using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Syngenta.AgriCast.LocationSearch.Service;
using Syngenta.AgriCast.LocationSearch.View;
using Syngenta.AgriCast.Common.DTO;
using System.Collections;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common.View;
using Syngenta.AgriCast.Common.Presenter;
using System.Web;
using System.Web.SessionState;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common;
using Syngenta.AgriWeb.LocationSearch.DataObjects;


namespace Syngenta.AgriCast.LocationSearch.Presenter
{
    public class locSearchPresenter
    {
        locSearchService locSvc = new locSearchService();
        ServiceHandler ServiceObj = new ServiceHandler(); 
        ServicePresenter objSvcPre = new ServicePresenter();
        DataPointInfo objDataPointInfo = new DataPointInfo();
        //ServiceInfo objServiceInfo;
        CommonUtil objComUtil = new CommonUtil();
        ILocSearch ILocation;
        IToolbar objITool;
        //LocationInfo locInfo;
        //private string strSearch;
        //private string strCntry;

        /// <summary>
        /// constructor
        /// </summary>
        public locSearchPresenter()
        {

        }

        public locSearchPresenter(ILocSearch ISearch)
        {
            if (ISearch != null)
            {
                ILocation = ISearch;
            }
        }

        /// <summary>
        /// gets the country name and AredId to populate the country dropdown
        /// </summary>
        //public DataSet getCountry()
        public void getCountry()
        {
            try
            {

                ILocation.dtCountry = ServiceObj.loadCountries();
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_FETCH_COUNTRIES_FAILURE) + ex.Message.ToString();
                return;
            }
        }

        //Method to fetch the name of the data provider
        public void getProviderName()
        {
            try
            {
                ILocation.eProviderName = ServiceObj.getProviderName();
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_GETPROVIDERNAME_FALIURE)+ ex.Message.ToString();
                return;
            }
        }

        public void getLocationDetails(string strName, string strCountry, LocationSearchSource eProvider, double lat, double lang, string culture)
        {
            try
            {
                ILocation.LocationList = locSvc.getLocationDetails(strName, strCountry, eProvider, lat, lang,culture);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_FETCH_LOCATION_FAILURE) + ex.Message.ToString();
                return;
            }
        }

        //public void updateFavorite(string strNewFavName, int intFavId)
        //{
        //    try
        //    {
        //        locSvc.updateFavorite(strNewFavName, intFavId);
        //    }
        //    catch (Exception ex)
        //    {
        //        AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
        //        AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
        //        HttpContext.Current.Session["ErrorMessage"] = "The following error occured while updating favorites : " + ex.Message.ToString();
        //        return;
        //    }
        //}

        public void getTranslatedText(string strLabelName, string strCultureCode)
        {
            try
            {
                ILocation.strTranslatedText = locSvc.getTranslatedText(strLabelName, strCultureCode);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.TRANSLATION_FAILURE) + ex.Message.ToString();
                return;
            }
        }

        public void getCultureCode()
        {
            try
            {
                ILocation.strCultureCode = objITool.selLang;
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.CULTURECODE_FAILURE) + ex.Message.ToString();
                return;
            }
        }

        //public void getTimeZoneOffset(int iStationID)
        //{
        //    try
        //    {
        //        DataTable dt = new DataTable();
        //        dt = locSvc.getTimeZoneOffset(iStationID);
        //        ILocation.iTimeZoneOffset = Convert.ToInt32(dt.Rows[0]["TimezoneOffset"]);
        //        if (Convert.ToBoolean(dt.Rows[0]["DstOn"]))
        //            ILocation.iDstOn = 1;
        //        else
        //            ILocation.iDstOn = 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
        //        AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
        //        HttpContext.Current.Session["ErrorMessage"] = "The following error occured while retrieving the timezone offset : " + ex.Message.ToString();
        //        return;
        //    }

        //}

        public List<string> getAutoSuggestLocation(string strCountryCode, string strSearchString)
        {
            return locSvc.getAutoSuggestLocation(strCountryCode, strSearchString, LocationInfo.getLocationInfoObj.Provider);
        }

        public void getSunriseOrSunsetTime(DateTime dCurrDate, double dLat, double dLong)
        {
            try
            {
                ILocation.sunRiseTime = objComUtil.getSunrise(dCurrDate, dLat, dLong);
                ILocation.sunSetTime = objComUtil.getSunset(dCurrDate, dLat, dLong);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.SUNRISEORSET_FAILURE) + ex.Message.ToString();
                return;
            }
        }

        public void CheckMapVisibility()
        {
            ILocation.showMap = ServiceObj.CheckMapVisibility();
        }

    }
}
