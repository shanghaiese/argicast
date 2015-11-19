using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;





namespace Syngenta.AgriCast.ExceptionLogger
{
    public class AgriCastException:Exception
    {
      
        #region Constructors of the custom exception class
        /// <summary>
        /// Default costructor for RMS Exception 
        /// class
        /// </summary>
        //public KAMException() : base() { }
        public AgriCastException(ArrayList alError,Exception ex)
            : base(ex.Message, ex)
        {
            //string userId, string service, string module, string server, string domain,
         

          //  objUserInfo = UserInfo.getUserInfoObject;

            //Setting the values for all the properties           
            //this.ExceptionPath = exceptionPath;
            this.UserId = Environment.UserName;//alError[2].ToString();
            this.Service = alError[0].ToString();
            this.Module = alError[1].ToString();
            if (HttpContext.Current != null)
            {
                this.Server = HttpContext.Current.Server.MachineName;
                this.Domain = HttpContext.Current.Request.Url.Host;
            }
            else
            {
                this.Server = "";
                this.Domain = HttpRuntime.AppDomainId.ToString(); 

            }
            this.ExceptionType = ex.Message;
            this.ExceptionDetail = ex.StackTrace;




        }
        public AgriCastException(string Location, ArrayList alDetails)
            
        {
            //string userId, string service, string module, string server, string domain,


            //  objUserInfo = UserInfo.getUserInfoObject;

            //Setting the values for all the properties           
            //this.ExceptionPath = exceptionPath;
            this.UserId = Environment.UserName;//objUserInfo.UserName;
            this.Service = alDetails[0].ToString();
            this.Module = alDetails[1].ToString();
            this.Server = HttpContext.Current.Server.MachineName;
            this.Domain = HttpContext.Current.Request.Url.Host;
           // this.Location = location;

        }

        public AgriCastException(IDictionary dictAudit)
        {
            //string userId, string service, string module, string server, string domain,


            //  objUserInfo = UserInfo.getUserInfoObject;

            //Setting the values for all the properties           
            //this.ExceptionPath = exceptionPath;
            //this.UserIP =
            //this.UserId = Environment.UserName;//objUserInfo.UserName;
            //this.Service = alDetails[0].ToString();
            //this.Module = alDetails[1].ToString();
            //this.Server = HttpContext.Current.Server.MachineName;
            //this.Domain = HttpContext.Current.Request.Url.Host;
            // this.Location = location;
            this.UserIP = dictAudit["userIP"].ToString();
            this.UserId = dictAudit["userID"].ToString();
            this.Token = dictAudit["token"].ToString();
            this.Culture = dictAudit["culture"].ToString();
            this.SessionID = dictAudit["sessionID"].ToString();
            this.Service = dictAudit["service"].ToString();
            this.Module = dictAudit["module"].ToString();
            this.LocSearchType = dictAudit["locSearchType"].ToString();
            this.LocSearchString = dictAudit["locSearchString"].ToString();
            this.LocSearchDatasource = dictAudit["locSearchDatasource"].ToString();
            this.NumOfLocs = dictAudit["numOfLocs"].ToString();
            this.SearchLatLong = dictAudit["searchLatLong"].ToString();
            this.CountryName = dictAudit["countryName"].ToString();
            this.LocName = dictAudit["locName"].ToString();
            this.WeatherDatasource = dictAudit["weatherDatasource"].ToString();
            this.WeatherLatLong = dictAudit["weatherLatLong"].ToString();
            this.WeatherDateRange = dictAudit["weatherDateRange"].ToString();
            this.WeatherSeries = dictAudit["weatherSeries"].ToString();
            this.WeatherParams = dictAudit["weatherParams"].ToString();



        }
        #endregion

        #region Variable Declaration
       // private string exceptionPath;
        private string userId;
        private DateTime timeStamp;
        private string service;
        private string module;
        private string server;
        private string domain;
        private string exceptionType;
        private string exceptionDetail;
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
        private string sessionID;
        private string userIP;
        private string token;
        private string culture;


        public string Token
        {
            get { return token; }
            set { token = value; }
        }
        public string UserIP
        {
            get { return userIP; }
            set { userIP = value; }
        }
        public string Culture
        {
            get { return culture; }
            set { culture = value; }
        }

        public string SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }
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


        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }
       

        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
        }
      

        public string Service
        {
            get { return service; }
            set { service = value; }
        }
      

        public string Module
        {
            get { return module; }
            set { module = value; }
        }
    

        public string Server
        {
            get { return server; }
            set { server = value; }
        }
       

        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }
        

        public string ExceptionType
        {
            get { return exceptionType; }
            set { exceptionType = value; }
        }
       

        public string ExceptionDetail
        {
            get { return exceptionDetail; }
            set { exceptionDetail = value; }
        }


        #endregion

        #region Properties

        /// <summary>
        /// This property holds the value of page url
        /// </summary>
        //public string ExceptionPath
        //{
        //    get { return exceptionPath; }
        //    set { exceptionPath = value; }
        //}



        #endregion
    }
}