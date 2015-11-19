using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syngenta.AgriCast.Common.Service;
using System.Web.SessionState;

namespace Syngenta.AgriCast.Common.DTO
{
    public class AuditInfo
    {
        private static AuditInfo objAuditInfo;
        CommonUtil objComUtil = new CommonUtil();

        public static AuditInfo getAuditInfoObj
        {
            get
            {
                if (CommonUtil.IsSessionAvailable())
                {
                    HttpSessionState Session = HttpContext.Current.Session;
                    if (Session["objAuditInfo"] == null)
                    {
                        Session["objAuditInfo"] = new AuditInfo();
                    }
                    return (AuditInfo)Session["objAuditInfo"];
                }
                else
                {
                    if (objAuditInfo == null)
                        objAuditInfo = new AuditInfo();
                    return objAuditInfo;
                }
            }
            set
            {
                HttpSessionState Session = null;
                if (HttpContext.Current != null) Session = HttpContext.Current.Session;
                if (HttpContext.Current != null && Session != null)
                {
                    Session["objAuditInfo"] = value;
                }
                else
                {
                    objAuditInfo = value;
                }
            }
        }

        private string userIP;
        private string userID;
        private string token;
        private string culture;
        private string sessionID;
        private string service;
        private string module;
        private string locSearchType;
        private string locSearchString;
        private string locSearchDatasource;
        private string numOfLocs;
        private string searchLatLong;
        private string countryName;
        private string locName;
        private string weatherDatasource;
        private string weatherLatLong;
        private string weatherDateRange;
        private string weatherSeries;
        private string weatherParams;

        public string WeatherParams
        {
            get { return weatherParams; }
            set { weatherParams = value; }
        }

        public string WeatherSeries
        {
            get { return weatherSeries; }
            set { weatherSeries = value; }
        }

        public string WeatherDateRange
        {
            get { return weatherDateRange; }
            set { weatherDateRange = value; }
        }



        public string WeatherDatasource
        {
            get { return weatherDatasource; }
            set { weatherDatasource = value; }
        }
      


        public string WeatherLatLong
        {
            get { return weatherLatLong; }
            set { weatherLatLong = value; }
        }

        public string LocName
        {
            get { return locName; }
            set { locName = value; }
        }

        public string CountryName
        {
            get { return countryName; }
            set { countryName = value; }
        }

        public string SearchLatLong
        {
            get { return searchLatLong; }
            set { searchLatLong = value; }
        }

        public string NumOfLocs
        {
            get { return numOfLocs; }
            set { numOfLocs = value; }
        }

        public string LocSearchDatasource
        {
            get { return locSearchDatasource; }
            set { locSearchDatasource = value; }
        }

        public string LocSearchString
        {
            get { return locSearchString; }
            set { locSearchString = value; }
        }

        public string LocSearchType
        {
            get { return locSearchType; }
            set { locSearchType = value; }
        }

        public string Module
        {
            get { return module; }
            set { module = value; }
        }

        public string Service
        {
            get { return service; }
            set { service = value; }
        }


        public string SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }


        public string Culture
        {
            get { return culture; }
            set { culture = value; }
        }


        public string Token
        {
            get { return token; }
            set { token = value; }
        }


        public string UserID
        {
            get { return userID; }
            set { userID = value; }
        }

        public string UserIP
        {
            get { return userIP; }
            set { userIP = value; }
        }

    }
}