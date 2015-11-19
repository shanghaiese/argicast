using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.SessionState;
using Syngenta.AgriCast.Common.Service;
using System.Web;
using System.Globalization;
using Syngenta.AgriWeb.LocationSearch.DataObjects;

namespace Syngenta.AgriCast.Common.DTO
{
   public  class LocationInfo
    {
       private string strSearchLocation = "";
        private string strPlaceName= "";
        private double fLat;
        private double fLong;
        private int intPlaceId;
        private int intCntryID;
        private LocationSearchSource eDataSource = LocationSearchSource.defaultService;
        private string strAdminName = "";
        private string strCntryCode = "";
        private DataPointInfo objDataPointInfo;
        private  static LocationInfo objLocationInfo;
        private ServiceInfo objSvcInfo;
        private LocationInfo objLocInfo;
       CommonUtil objComUtil = new CommonUtil();
       ServiceHandler objSvcHandler = new ServiceHandler();
       
    
        //public LocationInfo()
        //{
        //    throw new System.NotImplementedException();
        //}


        public double latitude
        {
            get
            {
                return fLat;
            }
            set
            {
                fLat = value;
            }
        }

        public double longitude
        {
            get
            {
                return fLong;
            }
            set
            {
                fLong = value;
            }
        }

        public string placeName
        {
            get
            {
                /*RunTime error in sprayweather - Begin*/
                //return strPlaceName; 
                return HttpUtility.UrlDecode(strPlaceName);
                /*RunTime error in sprayweather - End*/
            }
            set
            {
                /*RunTime error in sprayweather - Begin*/
                // strPlaceName = value; 
                strPlaceName = HttpUtility.UrlEncode(value);
                /*RunTime error in sprayweather - End*/
            }
        }

        public string searchLocation
        {
            get
            {
                return strSearchLocation;
            }
            set
            {
                strSearchLocation = value;
            }
        }

        public string AdminName
        {
            get
            {
                return strAdminName;
            }
            set
            {
                strAdminName = value;
            }
        }

        public int CountryID
        {
            get
            {
                return intCntryID;
            }
            set
            {
                intCntryID = value;
            }
        }

        public string CountryCode
        {
            get
            {
                return strCntryCode;
            }
            set
            {
                strCntryCode = value;
            }
        }

        public int placeID
        {
            get
            {
                return intPlaceId;
            }
            set
            {
                intPlaceId = value;
            }
        }

        

        public LocationSearchSource Provider
        {
            get
            {
                return eDataSource;
            }
            set
            {
                eDataSource = value;
            }
        }

        public DataPointInfo DataPointInfo
        {
            get
            {
                return objDataPointInfo;
            }
            set
            {
                objDataPointInfo = value;
            }
        }
        public ServiceInfo ServiceInfo
        {
            get
            {
                return objSvcInfo;
            }
            set
            {
                objSvcInfo = value;
            }
        }
       

        public bool CheckCountryValidity()
        {
            throw new System.NotImplementedException();
        }

        public static LocationInfo getLocationInfoObj
        {
            get
            {
                if (CommonUtil.IsSessionAvailable())
                {
                    HttpSessionState Session = HttpContext.Current.Session;
                    if (Session["objLocationInfo"] == null)
                    {
                        Session["objLocationInfo"] = new LocationInfo();
                    }
                    return (LocationInfo)Session["objLocationInfo"];
                }
                else
                {
                    if (objLocationInfo == null)
                    objLocationInfo = new LocationInfo();
                    return objLocationInfo;
                }
            }
            set
            {
                HttpSessionState Session = null;
                if (HttpContext.Current != null) Session = HttpContext.Current.Session;
                if (HttpContext.Current != null && Session != null)
                {
                    Session["objLocationInfo"] = value;
                }
                else
                {
                    objLocationInfo = value;
                }
            }
        }


        public  string SerializeCookieString()
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (HttpContext.Current != null) Session = HttpContext.Current.Session;
            objDataPointInfo = DataPointInfo.getDataPointObject;
            objLocInfo = LocationInfo.getLocationInfoObj;
            if (Session == null || Session["serviceInfo"] == null)
            {

                objSvcInfo = new ServiceInfo();
            }
            else
            {
                objSvcInfo = (ServiceInfo)Session["serviceInfo"];
            }

            return objLocInfo.searchLocation + "#"
                + objLocInfo.placeID + "#"
                + HttpUtility.UrlEncode(objLocInfo.placeName) + "#"
                + objLocInfo.longitude.ToString(NumberFormatInfo.InvariantInfo) + "#"
                + objLocInfo.latitude.ToString(NumberFormatInfo.InvariantInfo) + "#"
                + objLocInfo.CountryCode + "#"
                + objLocInfo.Provider + "#"
                +  objLocInfo.AdminName  + "#"
                + HttpUtility.UrlEncode(objDataPointInfo.stationName) + "#"
                + objDataPointInfo.stationLatitude + "#"
                + objDataPointInfo.stationLongitude + "#"
                + objSvcInfo.Country + "#"
                + objSvcInfo.Culture + "#" 
                + objSvcInfo.Module + "#"
                //+ objSvcInfo.ServiceName + "#"
                + objSvcInfo.Unit + "#";
             
        }
        public  LocationInfo DeserializeCookieString(string cookiestr)
        {
            string[] values = cookiestr.Split('#');
            //int i = 0;
            HttpSessionState Session = HttpContext.Current.Session;
            if (HttpContext.Current != null) Session = HttpContext.Current.Session;
            objDataPointInfo = DataPointInfo.getDataPointObject;
            objLocInfo = LocationInfo.getLocationInfoObj;
            if (Session == null || Session["serviceInfo"] == null)
            {

                objSvcInfo = new ServiceInfo();
            }
            else
            {
                objSvcInfo = (ServiceInfo)Session["serviceInfo"];
            }
            try
            {
                int pid=0;
                double coord=0d;
                searchLocation = (values[0]);
                placeID = Int32.TryParse(values[1],out pid)?pid:0;
                placeName =  HttpUtility.UrlDecode(values[2]);
                longitude = double.TryParse(values[3], out coord) ? coord : 0d;
                latitude = double.TryParse(values[4],out coord)?coord:0d;
                CountryCode = (values[5]);
                if (Enum.IsDefined(typeof(LocationSearchSource), values[6]))
                    Provider = (LocationSearchSource)Enum.Parse(typeof(LocationSearchSource), values[6], true);
                else
                    Provider = LocationSearchSource.defaultService;
                AdminName =  values[7] ;
                objDataPointInfo.stationName = HttpUtility.UrlDecode(values[8]);
                objDataPointInfo.stationLatitude = double.TryParse(values[9],out coord)?coord:0d;
                objDataPointInfo.stationLongitude = double.TryParse(values[10], out coord) ? coord : 0d;
                objSvcInfo.Country = (values[11]);
                objSvcInfo.Culture = (values[12]);
                objSvcInfo.Module = (values[13]);
                //objSvcInfo.ServiceName = (values[14]);
                objSvcInfo.Unit = (values[14]);
                //objDataPointInfo.CheckStationValidity();
  
            }
            catch
            {
                return null;
            }
            return this;
        }
        
    }
}
