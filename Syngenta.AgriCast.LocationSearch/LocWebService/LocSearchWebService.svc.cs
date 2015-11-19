using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using Syngenta.AgriCast.LocationSearch.Presenter;
using Syngenta.AgriCast.LocationSearch.Service;
using Syngenta.AgriCast.LocationSearch.View;
using System.IO;
using System.Xml;
using System.Web;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common.Presenter;
using System.ServiceModel.Activation;
using Syngenta.AgriWeb.LocationSearch.DataObjects;


namespace Syngenta.AgriCast.LocationSearch.LocWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "LocSearchWebService" in code, svc and config file together.
    /* SOCB - For rest - Jerrey - SOCB */
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    /* EOCB - For rest - Jerrey - EOCB */
    public class LocSearchWebService : ILocSearch
    {
        locSearchPresenter locPre;
        nearbyPointPresenter ObjNearByPre;
        DataSet dsCon = new DataSet();
        DataTable dtCon;
        DataSet dsLoc = new DataSet();
        ServicePresenter objSvcPre = new ServicePresenter();
        
        //DataTable dtNearBy = new DataTable();
        string strText;

        /// <summary>
        /// Method to fetch the country name and area Id
        /// </summary>        
        //public DataSet getCountry()
        public DataTable getCountry()
        {
            locPre = new locSearchPresenter(this);
            locPre.getCountry();
            return dtCountry;
        }

        /// <summary>
        /// Method to get the translated text by passing key value and language id
        /// </summary>
        public string getTranslatedText(string strLabelName, string strLangID)
        {
            locPre = new locSearchPresenter(this);
            return strTranslatedText;

        }

        //Method to fetch location details
        public List<Location> getLocationDetails(string strPlaceName, string strCountry, string strCultureCode, LocationSearchSource strProvider)
        {             
            double lat = 0.00;
            double lang = 0.00;
            locPre = new locSearchPresenter(this);
            try
            {
                //locPre.getLocationDetails(HttpUtility.HtmlDecode(strPlaceName), HttpUtility.HtmlDecode(strCountry), HttpUtility.HtmlDecode(strProvider), lat, lang, HttpUtility.HtmlDecode(strCultureCode));
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                // HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.WS_GETSVCDATA_FAILURE) + " : " + ex.Message.ToString();


            }
            finally
            {
                IDictionary dictAudit = new Hashtable();
                dictAudit["userIP"] = "";// HttpContext.Current.Request.UserHostAddress;
                dictAudit["userID"] =  "";
                dictAudit["token"] = "";
                dictAudit["referrer"] = "none";
                dictAudit["entrancePath"] = HttpContext.Current!=null?"EmbeddedJS":"WebService";
                dictAudit["culture"] = strCultureCode;
                dictAudit["sessionID"] = "";// HttpContext.Current.Session.SessionID;
                dictAudit["service"] = "LocSearchWebService";
                dictAudit["module"] = "";
                dictAudit["locSearchType"] = "";
                dictAudit["locSearchStringType"] = "";
                dictAudit["locSearchString"] = strPlaceName;
                dictAudit["locSearchDatasource"] = strProvider;
                dictAudit["numOfLocs"] = 0;
                if(lat==0)
                    dictAudit["searchLat"] = DBNull.Value;
                else
                dictAudit["searchLat"] = lat;
                if (lang == 0)
                    dictAudit["searchLong"] = DBNull.Value;
                else
                dictAudit["searchLong"] =lang;
                dictAudit["countryName"] = strCountry;
                dictAudit["locName"] = "";
                dictAudit["weatherDatasource"] = "";
                dictAudit["weatherLat"] = DBNull.Value;
                dictAudit["weatherLong"] = DBNull.Value;
                dictAudit["weatherDateFrom"] = "";
                dictAudit["weatherDateTo"] = "";
                dictAudit["weatherSeries"] = "";
                dictAudit["weatherParams"] = "";
                objSvcPre.SaveServiceAuditData(dictAudit);
            }
            return null;

        }

        //Method to fetch near by stations
        public DataSet getNearByStations(double dlat, double dlong, int intMaxAllowedDist, int intMaxAllowedAlt, int intResultCount)
        {
            DataTable dtStations = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                ObjNearByPre = new nearbyPointPresenter(this);
              
                ObjNearByPre.getNearbyDataService(dlat, dlong, intMaxAllowedDist, intMaxAllowedAlt, intResultCount);

                ds.Tables.Add(dtNearByPoints);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                // HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.WS_GETSVCDATA_FAILURE) + " : " + ex.Message.ToString();


            }
            finally
            {
                IDictionary dictAudit = new Hashtable();
                dictAudit["userIP"] = "";// HttpContext.Current.Request.UserHostAddress;
                dictAudit["userID"] =  "";
                dictAudit["token"] = "";
                dictAudit["referrer"] = "none";
                dictAudit["entrancePath"] = HttpContext.Current != null ? "EmbeddedJS" : "WebService";
                dictAudit["culture"] = "";
                dictAudit["sessionID"] = "";// HttpContext.Current.Session.SessionID;
                dictAudit["service"] = "LocSearchWebService";
                dictAudit["module"] = "";
                dictAudit["locSearchType"] = "";
                dictAudit["locSearchStringType"] = "";
                dictAudit["locSearchString"] = "";
                dictAudit["locSearchDatasource"] = "";
                dictAudit["numOfLocs"] = 0;
                if (dlat == 0)
                    dictAudit["searchLat"] = DBNull.Value;
                else
                dictAudit["searchLat"] = dlat;
                if (dlong == 0)
                    dictAudit["searchLong"] = DBNull.Value;
                else
                dictAudit["searchLong"] = dlong;
                dictAudit["countryName"] = "";
                dictAudit["locName"] = "";
                dictAudit["weatherDatasource"] = "";
                dictAudit["weatherLat"] = DBNull.Value;
                dictAudit["weatherLong"] = DBNull.Value;
                dictAudit["weatherDateFrom"] = "";
                dictAudit["weatherDateTo"] = "";
                dictAudit["weatherSeries"] = "";
                dictAudit["weatherParams"] = DBNull.Value;
                objSvcPre.SaveServiceAuditData(dictAudit);
            }
            return ds;
        }
        public ArrayList GetServiceDetails()
        {
            ArrayList alError = new ArrayList();
            alError.Add("LocSearchWebService");
            alError.Add("LocSearchWebService");
            alError.Add(string.Empty);
            return alError;
        }
        //method to return version
        public string Version()
        {
            return "$Id: LocSearchService 266 2012-02-09 20:34:19Z Infosys $\n";
        }

        public DataTable dtNearByPoints
        {
            get;
            set;
        }
       
        public DataTable dtCountry
        {
            get
            {
                return dtCon;
            }
            set
            {
                dtCon = value;
            }
        }

        public string strTranslatedText
        {
            get
            {
                return strText;
            }
            set
            {
                strText = value;
            }
        }

        public DataSet dsLocation
        {
            get
            {
                return dsLoc;
            }
            set
            {
                dsLoc = value;
            }
        }

        public DataTable dtFavorites
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public string strCultureCode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string strProviderName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime sunRiseTime
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime sunSetTime
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int iTimeZoneOffset
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int iDstOn
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public bool showMap
        {
            get;
            set;
        }
      
        public LocationSearchSource eProviderName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public List<Location> LocationList
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
