using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Syngenta.AgriCast.Common.DataAccess;
using System.Collections;
using Syngenta.AgriCast.Common.Presenter;
using System.Web.SessionState;
using Syngenta.AgriCast.Common.DTO;

namespace Syngenta.AgriCast.Common.Service
{
    public class ToolBarService
    {
        CommonData objComData = new CommonData();
        TranslateData objTranslate = new TranslateData();
        ServicePresenter objSvcPre;
        UserInfo ObjUserInfo;
        DataTable dtFavorites = null;
        DataPointInfo objDataPointInfo = null;
        ServiceInfo objServiceInfo = null;
        LocationInfo objLocationinfo = null;
        DataTable dtTemp = null;
        DataRow dr = null;
        bool isFavoriteAdded = false;
        bool isFavoriteUpdated = false;
        public DataTable getFavorites()//Dictionary<string, Favorite> objFavList)
        {

            dtFavorites = objComData.getFavorites();

            return dtFavorites;
        }

        public bool updateFavorite(string strNewFavName, string strFavKey)
        {
            DataTable dtTemp = null;
            DataRow[] drRow = null;
            //Update the Dt Favorites Datatable in session
            ObjUserInfo = (UserInfo)HttpContext.Current.Session["objUserInfo"];

            dtTemp = ObjUserInfo.DtFavorites;

            //First Check if the Favorite with the same name exist in that datatable
            drRow = dtTemp.Select(Constants.FAV_FAVORITENAME + "=" + "'" + strNewFavName + "'");

            // if exists, do not update the dataTable
            if (drRow != null && drRow.Length != 1)
            {
                //Edit the Datatable
                drRow = dtTemp.Select(Constants.FAV_Key + "=" + "'" + strFavKey + "'");

                //Access the Row to be edited
                drRow[0].BeginEdit();

                //Modify the Name
                drRow[0][Constants.FAV_FAVORITENAME] = strNewFavName;

                drRow[0].AcceptChanges();

                ObjUserInfo.DtFavorites = dtTemp;

                HttpContext.Current.Session["objUserInfo"] = ObjUserInfo;

                isFavoriteUpdated = true;
                //objComData.updateFavorite(strNewFavName, intFavId);
            }

            return isFavoriteUpdated;

        }

        public DataTable GetFavPlaceDetails(string PlaceID)
        {
            return objComData.GetFavPlaceDetails(PlaceID);

        }


        public string getTranslatedText(string strLabelName, string strCultureCode)
        {
            string strText = objTranslate.getTranslatedText(strLabelName, strCultureCode);
            return strText;
        }

        public bool AddToFavorites(string strFavName)
        {
            //Update the Dt Favorites Datatable in session
            ObjUserInfo = (UserInfo)HttpContext.Current.Session["objUserInfo"];

            //Location Info object - moved code here  - trans message change
            objLocationinfo = (LocationInfo)HttpContext.Current.Session["objLocationInfo"];


            if (ObjUserInfo != null)
            {
                //Check if the Newly Added Key allreeady Exists in datatable
                //check whether the the Dictionary key exists in Datatable
                var res = from p in ObjUserInfo.DtFavorites.AsEnumerable()
                          where p[Constants.FAV_FAVORITENAME].Equals(strFavName)
                          select p[Constants.FAV_FAVORITENAME];

                //when no matach found
                if (res.ToList().Count == 1)
                {
                    isFavoriteAdded = false;
                    HttpContext.Current.Session["ErrorMessage"] = getTranslatedText(Constants.FAVORITE_UPDATE_DUPLICATE, objLocationinfo.ServiceInfo.Culture) + strFavName + getTranslatedText(Constants.GENERIC_ALREADYEXISTS, objLocationinfo.ServiceInfo.Culture);

                }
                else
                {
                    //Fetch all the Values from Session
                    isFavoriteAdded = true;

                    //Check if the Object is null
                    if (ObjUserInfo != null)
                    {
                        dtTemp = ObjUserInfo.DtFavorites;

                        dr = dtTemp.NewRow();


                        //Take the User Entered Favorite name 
                        dr[Constants.FAV_Key] = strFavName.Trim();
                        dr[Constants.FAV_FAVORITENAME] = strFavName.Trim();



                        //Fetch Id from LocationInfo Object
                        if (objLocationinfo != null)
                        {
                            //Fetch the Datapoint Info from Location Info itself
                            objDataPointInfo = objLocationinfo.DataPointInfo;

                            //Fetch the Service Info from Location Info itself
                            objServiceInfo = objLocationinfo.ServiceInfo;

                            dr[Constants.FAV_ALTITUDE] = objDataPointInfo.altitude;//Altitude
                            dr[Constants.FAV_LATITUDE] = objLocationinfo.latitude;//Place - Latitude
                            dr[Constants.FAV_LONGITUDE] = objLocationinfo.longitude;//Place - Longitiude
                            dr[Constants.FAV_PLACENAME] = objLocationinfo.placeName;//PlaceName

                            dr[Constants.FAV_PLACEID] = objLocationinfo.placeID;//Place ID


                            dr[Constants.FAV_SERVICENAME] = objServiceInfo.ServiceName;//ServieName
                            
                            dr[Constants.FAV_MODULENAME] = objServiceInfo.Module;//ModuleName 
                        }



                        //add the New Row to DtFavorites
                        dtTemp.Rows.Add(dr);

                        ObjUserInfo.DtFavorites = dtTemp;

                        //Add the Userinfo Back to Sessions
                        HttpContext.Current.Session["ObjUserInfo"] = ObjUserInfo;
                    }
                }
            }
            return isFavoriteAdded;
        }

        public void DeleteFavorite(string strFavKey)
        {
            DataTable dtTemp = null;

            //Update the Dt Favorites Datatable in session
            ObjUserInfo = (UserInfo)HttpContext.Current.Session["objUserInfo"];

            dtTemp = ObjUserInfo.DtFavorites;


            //Edit the Datatable
            DataRow[] drRow = dtTemp.Select(Constants.FAV_Key + "=" + "'" + strFavKey + "'");

            dtTemp.Rows.Remove(drRow[0]);

            ObjUserInfo.DtFavorites = dtTemp;

            HttpContext.Current.Session["objUserInfo"] = ObjUserInfo;

        }

        public ArrayList loadUnits()
        {

            //service svc = objSvcPre.readConfig("Demo");
            //serviceUnitsSettings unit = new serviceUnitsSettings();
            //unit = svc.unitsSettings;
            ArrayList al = new ArrayList();
            //al.Add(unit.defaultUnits.ToString());
            //al.Add(unit.defaultWindUnits.ToString());
            return al;
        }



        public string GetRandomText(string CharacterSet, int CaptchaLength)
        {

            char[] letters = CharacterSet.ToCharArray();
            string text = string.Empty;
            Random r = new Random();
            int num = -1;

            for (int i = 0; i < CaptchaLength; i++)
            {
                num = (int)(r.NextDouble() * (letters.Length - 1));
                text += letters[num].ToString();
            }
            return text;
        }


        public bool SendMail(string FeedbackName, string ToMail, string FeedbackEmail, string FeedbackMessage)
        {
            Email em = new Email();
            em.SendEmail("", "", "", FeedbackName, FeedbackMessage, FeedbackEmail, ToMail, "");
            return true;
        }

        public void SaveRatings(List<string[]> al)
        {

        }

    }
}