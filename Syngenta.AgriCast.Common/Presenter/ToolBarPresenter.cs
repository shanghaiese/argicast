using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syngenta.AgriCast.Common.View;
using Syngenta.AgriCast.Common.Service;
using System.Data;
using System.Collections;
using System.Web.SessionState;
using System.Web;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.ExceptionLogger;


namespace Syngenta.AgriCast.Common.Presenter
{
    public class ToolBarPresenter
    {
        ToolBarService objService = new ToolBarService();
        IToolbar objITool;
        ServiceInfo objSvcInfo;
        UserInfo ObjUserInfo;
        ServiceHandler objSvcHand = new ServiceHandler();
        ServicePresenter objSvcPre = new ServicePresenter();
        public ToolBarPresenter()
        {

        }

        public ToolBarPresenter(IToolbar ITool)
        {
            if (ITool != null)
            {
                objITool = ITool;
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["objServiceInfo"];
            }
        }

        public void getFavorites()
        {
            try
            {

                ObjUserInfo = (UserInfo)HttpContext.Current.Session["objUserInfo"];

                objITool.dtFavorites = ObjUserInfo.DtFavorites;
                //1objService.getFavorites();//objFavList);


            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = getTranslatedText(Constants.FAVORITE_GENERIC_FAILURE, objSvcInfo.Culture) + ex.Message.ToString();
            }
        }

        public void AddToFavorites()
        {

            objITool.IsFavAdded = objService.AddToFavorites(objITool.Favorite);

        }
        public void DeleteFavorite(string strFavKey)
        {
            objService.DeleteFavorite(strFavKey);
        }

        public void UpdateFavorite(string strNewFavName, string strFavKey)
        {
            try
            {
                objITool.IsFavUpdated = objService.updateFavorite(strNewFavName, strFavKey);

            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = getTranslatedText(Constants.FAVORITE_UPDATE_FAILURE, objSvcInfo.Culture) + ex.Message.ToString();
            }
        }

        public string getTranslatedText(string strLabelName, string strCultureCode)
        {

            return objITool.strText = objService.getTranslatedText(strLabelName, strCultureCode);
        }

        public void getCultureCode()
        {
            try
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                objITool.strCulCode = objSvcInfo.Culture;
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = getTranslatedText(Constants.CULTURE_LOADFAILURE, objSvcInfo.Culture) + ex.Message.ToString();
            }
        }

        //public ArrayList loadUnits()
        //{
        //    return objITool.setUnits = objService.loadUnits();
        //}


        public void GetFavPlaceDetails(string PlaceID)
        {
            objITool.dtPlaceDetails = objService.GetFavPlaceDetails(PlaceID);


        }

        public void GetRandomText(string CharacterSet, int CaptchaLength)
        {
            try
            {
                objITool.strRandomText = objService.GetRandomText(CharacterSet, CaptchaLength);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = getTranslatedText(Constants.FEEDBACK_CAPTCHA_LOADFAILURE, objSvcInfo.Culture) + ex.Message.ToString();
            }
        }

        public void getDefaultEmailSettings()
        {
            objITool.alEmailSettings = objSvcHand.getDefaultEmailSetttings();
        }

        public void SendMail(string FeedbackName, string ToMail, string FeedbackEmail, string FeedbackMessage)
        {
            try
            {
                objService.SendMail(FeedbackName, ToMail, FeedbackEmail, FeedbackMessage);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = getTranslatedText(Constants.FEEDBACK_GENERROR, objSvcInfo.Culture) + ex.Message.ToString();
            }
        }
        public void loadPageSettings()
        {
            try
            {
                string[] arPageSettings = objSvcHand.loadPageSettings();
                objITool.showFavorites = Convert.ToBoolean(arPageSettings[0]);
                objITool.showPrint = Convert.ToBoolean(arPageSettings[1]);
                objITool.showEmail = Convert.ToBoolean(arPageSettings[2]);
                objITool.showExportExcel = Convert.ToBoolean(arPageSettings[3]);
                objITool.showFeedback = Convert.ToBoolean(arPageSettings[4]);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            }

        }

        public void ConvertFavoriteToDataTable()
        {

        }

        public void getNodeList(string nodename)
        {
            try
            {
                objITool.alNodeList = objSvcHand.getNodeList(nodename);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = getTranslatedText(Constants.NODELIST_FETCH_FAILURE, objSvcInfo.Culture) + ex.Message.ToString();
            }
        }
    }
}
