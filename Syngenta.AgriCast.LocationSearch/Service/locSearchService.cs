using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Syngenta.AgriCast.LocationSearch.DataAccess;
using Syngenta.AgriWeb.LocationSearch.DataObjects;

namespace Syngenta.AgriCast.LocationSearch.Service
{
   public class locSearchService
    {
        locSearchData locDA = new locSearchData();
        /// <summary>
        /// Constructor
        /// </summary>
        public locSearchService() 
        {

        } 

        /// <summary>
        /// this is to get the results from an external source
        /// </summary>
        public string getLocResultsWS(string strSearch, string strCntry)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// this is to convert2XML the response from DB to XML for web service
        /// </summary>
        public string convert2XML(DataTable dtLocResults)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// to convert results from external source to a datatable
        /// </summary>
        public DataTable convert2DataTable(string strXML)
        {
            throw new System.NotImplementedException();
        }

        public DataTable getLocResults(string strSearch, string strCntry)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Method to fetch the country name and AredId to populate the country dropdown
        /// </summary>        

        //public DataTable getCountry(string strCountryCodes)
        //{
        //    return locDA.getCountry(strCountryCodes);
        //}

        public List<Location> getLocationDetails(string strName, string strCountry, LocationSearchSource eProvider, double lat, double lang, string culture)
        {
            return locDA.getLocationDetails(strName, strCountry, eProvider, lat, lang, culture);
        }

        //public DataTable getFavorites()
        //{
        //    DataTable dtFavorites = new DataTable();
        //    dtFavorites = locDA.getFavorites();
        //    return dtFavorites;
        //}

        //public void updateFavorite(string strNewFavName, int intFavId)
        //{
        //    locDA.updateFavorite(strNewFavName, intFavId);
        //}

        public string getTranslatedText(string strLabelName, string strCultureCode)
        {
            return locDA.getTranslatedText(strLabelName, strCultureCode);
        }
       
        public List<string> getAutoSuggestLocation(string strCountryCode, string strSearchString, LocationSearchSource provider)
        {
            //DataTable dtLoc = new DataTable(); 
            //dtLoc = locDA.getAutoSuggestLocation(strCountryCode, strSearchString, provider);
            //return dtLoc;
            return locDA.getAutoSuggestLocation(strCountryCode, strSearchString, provider);
        }

        /// <summary>
        /// to fetch the Offset value
        /// </summary>
        public DataTable getTimeZoneOffset(int iStationID)
        {
            return locDA.getTimeZoneOffset(iStationID);
        }
    }
}
