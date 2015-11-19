using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.ServiceModel.Web;
using System.ServiceModel;
using System.Collections;
using Syngenta.AgriWeb.LocationSearch.DataObjects; 

namespace Syngenta.AgriCast.LocationSearch.View
{
    [ServiceContract]
    public interface ILocSearch
    {
        DateTime sunRiseTime
        {
            get;
            set;
        }

        DateTime sunSetTime
        {
            get;
            set;
        }

        DataTable dtNearByPoints
        {
            get;
            set;
        }

        int iTimeZoneOffset
        {
            get;
            set;
        }

        int iDstOn
        {
            get;
            set;
        }
        
        DataTable dtCountry
        {
            get;
            set;
        }

        string strTranslatedText
        {
            get;
            set;
        }

        string strCultureCode
        {
            get;
            set;
        }

        List<Location> LocationList
        {
            get;
            set;
        }

        DataTable dtFavorites
        {
            get;
            set;
        }

        LocationSearchSource eProviderName
        {
            get;
            set;
        }
        bool showMap
        {
            get;
            set;
        }
        
        /// <summary>
        /// Method to get the country name and AreaID
        /// </summary>
        //[OperationContract]
        //[WebGet(UriTemplate = "/getCountry", ResponseFormat = WebMessageFormat.Xml)]
        //DataTable getCountry();

        /// <summary>
        /// Method to get the translated text by passing key value and language id
        /// </summary>        
        //[OperationContract]
        //[WebGet(UriTemplate = "/getTranslatedText?strLabelName={strLabelName}&strLangID={strLangID}", ResponseFormat = WebMessageFormat.Xml)]
        //string getTranslatedText(string strLabelName, string strLangID);

        [OperationContract]
        [WebGet(UriTemplate = "/getLocationDetails?strPlaceName={strPlaceName}&strCountry={strCountry}&strCultureCode={strCultureCode}&eProvider={eProvider}", ResponseFormat = WebMessageFormat.Xml)]
        List<Location> getLocationDetails(string strPlaceName, string strCountry, string strCultureCode, LocationSearchSource strProvider);

        [OperationContract]
        [WebGet(UriTemplate = "/getNearByStations?lat={dlat}&long={dlong}&MaxAllowedDist={intMaxAllowedDist}&MaxAllowedAlt={intMaxAllowedAlt}&ResultCount={intResultCount}", ResponseFormat = WebMessageFormat.Xml)]
        DataSet getNearByStations(double dlat, double dlong, int intMaxAllowedDist, int intMaxAllowedAlt, int intResultCount);

        [OperationContract]
        [WebGet(UriTemplate = "/Version", ResponseFormat = WebMessageFormat.Xml)]
        string Version();

    }
}
