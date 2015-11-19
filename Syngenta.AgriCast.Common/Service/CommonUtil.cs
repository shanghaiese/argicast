using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Collections;
using System.ComponentModel;
using Syngenta.AgriCast.Common.DataAccess;
using Syngenta.AgriCast.Common.DTO;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Configuration;
using System.Net;
using System.Xml;



namespace Syngenta.AgriCast.Common.Service
{
    public class CommonUtil
    {
        CommonData objCommonData = new CommonData();
        TranslateData tran = new TranslateData();
        ServiceInfo objSvcInfo;
        DataPointInfo objDataPointInfo;
        UserInfo objUserInfo;
        string strCulCode;

        public static bool IsSessionAvailable()
        {
            return (HttpContext.Current != null && HttpContext.Current.Session != null);
        }

        private bool CanUseType(Type propertyType)
        {
            //only strings and value types can be added reference types cannot be added
            if (propertyType.IsArray) return false;
            //Though string is a reference type we shall take it.
            if (!propertyType.IsValueType && propertyType != typeof(string)) return false;

            return true;
        }

        public string getlowercase(object input)
        {
            return input.ToString().ToLower();
        }

        public Color ContrastColor(Color color)
        {
            int d = 0;
            float luminance = (float)(0.299 * color.R + 0.587 * color.G + 0.114 * color.B);
            //double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            if (luminance < 131)
                d = 255; // dark colors - white font
            else
                d = 0; // bright colors - black font
            return Color.FromArgb(d, d, d);
        }

        //To set the culture from the web service
        public void setWebServiceCultureCode(string strCultCode)
        {
            strCulCode = strCultCode;

        }

        public DataTable CreateDataTable(Type typ)
        {
            DataTable table = new DataTable(typ.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typ);
            foreach (PropertyDescriptor property in properties)
            {
                Type propertyType = property.PropertyType;
                if (!CanUseType(propertyType)) continue;

                //nullables must use underlying types
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                //enums also need special treatment
                if (propertyType.IsEnum)
                    propertyType = Enum.GetUnderlyingType(propertyType);
                table.Columns.Add(property.Name, propertyType);
            }
            return table;
        }

        //Method to convert the list into a data table
        public DataTable ConvertToDataTable<T>(IList<T> list)
        {
            Type objType = typeof(T);
            DataTable table = CreateDataTable(objType);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(objType);
            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                table.Rows.Add(FillData(properties, row, item));
            }
            return table;
        }

        public DataRow FillData(PropertyDescriptorCollection pColl, DataRow row, Object item)
        {
            foreach (PropertyDescriptor property in pColl)
            {
                if (!CanUseType(property.PropertyType)) continue;
                row[property.Name] = property.GetValue(item) ?? DBNull.Value;
            }
            return row;
        }

        public DataTable ConvertToDataTable(IList list, DataTable dt)
        {
            Type objType = typeof(Type);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(objType);
            foreach (Type item in list)
            {
                DataRow row = dt.NewRow();
                dt.Rows.Add(FillData(properties, row, item));
            }
            return dt;
        }

        //Method to convert a list into a data set
        /*public DataSet ConvertToDataSet<T>(IList<T> list)
          {
              DataSet ds = new DataSet();
              DataTable dt = new DataTable();

              dt = ConvertToDataTable(list);
              Type objType = typeof(T);
              PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(objType);
              foreach (T item in list)
              {
                  foreach (PropertyDescriptor property in properties)
                  {
                      if (dt != null && dt.Rows.Count > 0)
                          ds.Tables.Add(dt);

                      if (!CanUseType(property.PropertyType))
                      {
                          dt = new DataTable();
                          dt = CreateDataTable(property.PropertyType);
                          dt = ConvertToDataTable();
                      }
                  }
                 
              }

              return ds;
          }*/

        public DateTime getSunrise(DateTime day, double lat, double lng)
        {
            //// 1. first calculate the day of the year
            //int N = day.DayOfYear;
            //// 2. convert the longitude to hour value and calculate an approximate time
            //double lngHour = 7.6/15;
            //double riseT = N + ((6 - lngHour) / 24);
            //double setT = N + ((18-lngHour)/24);
            //DateTime Sunrise = computeTime(DateTime.Now.AddDays(2), 0, N, 47.556, 7.6, riseT, true, 90.83333333333);

            return objCommonData.getSunriseOrSunset(day, lat, lng, "SUNRISE");
        }
        public DateTime getSunset(DateTime day, double lat, double lng)
        {
            //// 1. first calculate the day of the year
            //int N = day.DayOfYear;

            //// 2. convert the longitude to hour value and calculate an approximate time
            //double lngHour = 7.6/15;
            //double riseT = N + ((6 - lngHour) / 24);
            //double setT = N + ((18-lngHour)/24);
            //DateTime Sunset = computeTime(DateTime.Now.AddDays(2), 0, N, 47.556, 7.6, setT, false, 90.83333333333);

            return objCommonData.getSunriseOrSunset(day, lat, lng, "SUNSET");
        }
        public string getTextDirection(int degrees)
        {
            // N,NE,E,SE,S,SW,W,NW
            /*       N (0)
             *  
             *   W (270) E (90)
             * 
             *       S (180)
             */
            string strTranslatedText;
            if (HttpContext.Current != null)
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];

                /* Wind Direction not avaliable in web service issue - begin*/
                if (objSvcInfo != null && string.IsNullOrEmpty(objSvcInfo.Culture))
                {
                    if (!string.IsNullOrEmpty(strCulCode))
                    {
                        objSvcInfo.Culture = strCulCode;
                    }
                }
                /* Wind Direction not avaliable in web service issue - End*/

                strTranslatedText = tran.getTranslatedText("ResDirections", objSvcInfo.Culture);
            }
            else
            {
                strTranslatedText = tran.getTranslatedText("ResDirections", strCulCode);
            }
            string[] directArr = null;
            if (strTranslatedText != null && strTranslatedText.IndexOf(",") > 0)
            {
                directArr = strTranslatedText.Split(',');
                // check there are 8 directions
                if (directArr.Length != 8) directArr = new string[8];
            }
            else directArr = new string[8];
            while (degrees < 0) degrees += 360;
            while (degrees > 360) degrees -= 360;
            if (degrees >= 338 || degrees < 23) return "N";
            else if (degrees >= 23 && degrees < 68) return directArr[1]; // NE
            else if (degrees >= 68 && degrees < 113) return directArr[2]; // E
            else if (degrees >= 113 && degrees < 158) return directArr[3]; // SE
            else if (degrees >= 158 && degrees < 203) return directArr[4]; // S
            else if (degrees >= 203 && degrees < 248) return directArr[5]; // SW
            else if (degrees >= 248 && degrees < 293) return directArr[6]; // W
            else if (degrees >= 293 && degrees < 338) return directArr[7]; // NW
            else return "-";
        }

        /* IM01354471 - New Agricast - wind icon should not be language specific - Jerrey - Begin */
        public string getTextDirectionForWindIcon(int degrees)
        {
            // N,NE,E,SE,S,SW,W,NW
            /*       N (0)
             *  
             *   W (270) E (90)
             * 
             *       S (180)
             */
            string strTranslatedText = tran.getTranslatedText("ResDirections", "en-GB");
            string[] directArr = null;
            if (strTranslatedText != null && strTranslatedText.IndexOf(",") > 0)
            {
                directArr = strTranslatedText.Split(',');
                // check there are 8 directions
                if (directArr.Length != 8) directArr = new string[8];
            }
            else directArr = new string[8];
            while (degrees < 0) degrees += 360;
            while (degrees > 360) degrees -= 360;
            if (degrees >= 338 || degrees < 23) return "N";
            else if (degrees >= 23 && degrees < 68) return directArr[1]; // NE
            else if (degrees >= 68 && degrees < 113) return directArr[2]; // E
            else if (degrees >= 113 && degrees < 158) return directArr[3]; // SE
            else if (degrees >= 158 && degrees < 203) return directArr[4]; // S
            else if (degrees >= 203 && degrees < 248) return directArr[5]; // SW
            else if (degrees >= 248 && degrees < 293) return directArr[6]; // W
            else if (degrees >= 293 && degrees < 338) return directArr[7]; // NW
            else return "-";
        }
        /* IM01354471 - New Agricast - wind icon should not be language specific - Jerrey - End */

        /// <summary>
        /// checks if the given date is in the night or during the day. Returns true if at night.
        /// </summary>
        /// <param name="fdate"></param>
        /// <returns></returns>
        public bool isNight(DateTime fdate, double lat, double lng)
        {
            if (HttpContext.Current != null)
            {
                HttpSessionState Session = HttpContext.Current.Session;
                objDataPointInfo = (DataPointInfo)Session["objDataPointInfo"];
            }
            else
            {
                objDataPointInfo = new DataPointInfo();
            }
            DateTime Sunrise = objDataPointInfo.SunRise == DateTime.MinValue ? DateTime.Today : objDataPointInfo.SunRise;// objCommonData.getSunriseOrSunset(DateTime.Now, lat, lng, "SUNRISE");
            DateTime Sunset = objDataPointInfo.SunSet == DateTime.MinValue ? DateTime.Today : objDataPointInfo.SunSet; //objCommonData.getSunriseOrSunset(DateTime.Now, lat, lng, "SUNSET");
            DateTime sunRise = Sunrise.AddHours(-1);
            int minuteRise = sunRise.Hour * 60 + sunRise.Minute;
            int minuteSet = Sunset.Hour * 60 + Sunset.Minute;
            bool ssflip = false;
            if (minuteRise > minuteSet)
            {
                ssflip = true;
                int t = minuteRise;
                minuteRise = minuteSet;
                minuteSet = t;
            }
            int cMinute = fdate.Hour * 60 + fdate.Minute;
            return (cMinute <= minuteRise || cMinute >= minuteSet) ^ ssflip;
        }

        private static DateTime computeTime(DateTime day, double timezone, int N, double latit, double longit, double t, bool isRise, double zenith)
        {
            // 3. calculate the Sun's mean anomaly
            double M = (0.9856 * t) - 3.289;
            // 4. calculate the Sun's true longitude
            double L = M + (1.916 * sind(M)) + (0.020 * sind(2 * M)) + 282.634;
            // NOTE: L potentially needs to be adjusted into the range [0,360) by adding/subtracting 360
            if (L < 0) L += 360;
            if (L >= 360) L -= 360;
            // 5a. calculate the Sun's right ascension
            //double RA = Math.Atan(0.91764 * Math.Tan(Lrad))*180.0/Math.PI;
            double RA = atand(0.91764 * tand(L));
            // NOTE: RA potentially needs to be adjusted into the range [0,360) by adding/subtracting 360
            if (RA < 0) RA += 360;
            if (RA >= 360) RA -= 360;
            // 5b. right ascension value needs to be in the same quadrant as L
            int Lquadrant = ((int)(L / 90)) * 90;
            int RAquadrant = ((int)(RA / 90)) * 90;
            RA = RA + (Lquadrant - RAquadrant);
            // 5c. right ascension value needs to be converted into hours
            RA = RA / 15;
            // 6. calculate the Sun's declination
            double sinDec = 0.39782 * sind(L);
            double cosDec = cosd(asind(sinDec));
            // 7a. calculate the Sun's local hour angle
            double cosH = (cosd(zenith) - (sinDec * sind(latit))) / (cosDec * cosd(latit));
            if (cosH > 1 && isRise)
                return DateTime.MinValue;
            if (cosH < -1 && !isRise)
                return DateTime.MaxValue;
            // 7b. finish calculating H and convert into hours
            double H;
            if (isRise)
                H = 360 - (acosd(cosH));
            else
                H = acosd(cosH);
            H = H / 15;
            // 8. calculate local mean time of rising/setting
            double T = H + RA - (0.06571 * t) - 6.622;
            // 9. adjust back to UTC
            double UT = T - (longit / 15);
            // NOTE: UT potentially needs to be adjusted into the range [0,24) byadding/subtracting 24
            while (UT < 0) UT += 24;
            while (UT >= 24) UT -= 24;
            //UT = UT;
            DateTime retv = new DateTime(day.Year, day.Month, day.Day,
                (int)UT, (int)(60 * (UT % 1)), 0);
            //10. convert UTC to local time zone of latitude/longitude
            retv = retv.AddHours(timezone);

            return retv;
        }

        // Maths functions
        // These functions will take degrees as decimals and convert into radians.
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI) / 180.0;
        }
        private static double rad2deg(double rad)
        {
            return (rad * 180.0) / Math.PI;
        }

        private static double sind(double deg)
        {
            return Math.Sin(deg2rad(deg));
        }
        private static double cosd(double deg)
        {
            return Math.Cos(deg2rad(deg));
        }
        private static double tand(double deg)
        {
            return Math.Tan(deg2rad(deg));
        }
        private static double atand(double x)
        {
            return rad2deg(Math.Atan(x));
        }
        private static double asind(double x)
        {
            return rad2deg(Math.Asin(x));
        }
        private static double acosd(double x)
        {
            return rad2deg(Math.Acos(x));
        }

        /*First Time Page load Takes a long time issue - BEGIN*/
        public void getTransTextForFirstTimeLoad(string key)
        {

            tran.GetTransTextOnFirstLoad(key);
            return;
        }
        /*First Time Page load Takes a long time issue - END*/
        public string getTransText(string key)
        {
            if (HttpContext.Current != null)
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
            }

            return tran.getTranslatedText(key, (objSvcInfo != null && objSvcInfo.Culture != "") ? objSvcInfo.Culture : "en-GB");
        }

        public string getApplPath(string directory)
        {
            string path; // = System.IO.Directory.GetCurrentDirectory();

            if (IsSessionAvailable())
            {
                path = HttpRuntime.AppDomainAppPath + directory;
                //if (directory==null ||directory=="") //path = path + "\\";
            }
            else
            {
                path = System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                path = path.Replace("\\bin", "");
                path = path.Replace("file:\\", "");

                if (directory != null)
                    path = path + "/" + directory;
            }
            path = path.Replace("/", "\\");
            return path;
        }

        public string toBase64(string strVal)
        {
            byte[] vals = Encoding.UTF8.GetBytes(strVal);
            strVal = Convert.ToBase64String(vals);
            return strVal;

        }

        public string strReplace(string input)
        {
            string pattern = "[\\\\//':(\\s+)&.?#\"*;@_$-]";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            string result = rgx.Replace(input, replacement);
            return result;
        }

        public void SaveRatings(List<string[]> ratings)
        {
            string service;
            string username;
            if (HttpContext.Current != null)
            {
                HttpSessionState Session = HttpContext.Current.Session;
                objUserInfo = (UserInfo)Session["objUserInfo"];
                if (objUserInfo == null)
                    username = "";
                else
                    username = objUserInfo.UserName;
            }
            else
            {
                username = "";
            }
            if (HttpContext.Current != null)
            {
                objSvcInfo = (ServiceInfo)HttpContext.Current.Session["serviceInfo"];
                service = objSvcInfo.ServiceName;
            }
            else
            {
                service = "";
            }
            objCommonData.SaveRatings(ratings, service, username);
        }

        public void SaveAuditData()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session["AuditData"] != null)
                {
                    IDictionary dict = (IDictionary)HttpContext.Current.Session["AuditData"];
                    objCommonData.SaveAuditData(dict);
                }
            }
        }
        public void SaveServiceAuditData(IDictionary dict)
        {

            objCommonData.SaveAuditData(dict);

        }

        /// <summary>
        /// 
        /// converts Distance in kms to miles .this is used for culture en-US
        /// </summary>
        /// <param name="p_dist"></param>
        /// <returns></returns>
        public string ConvertValueMetrics(string p_inputVal, string p_metric)
        {
            double retValue = 0.0;
            double dInputVal = 0.0;
            if (!string.IsNullOrEmpty(p_metric))
            {
                //Parse the Input Value to double
                dInputVal = double.TryParse(p_inputVal, out dInputVal) ? dInputVal : 0.0;

                switch (p_metric)
                {
                    //convert KiloMetres to Metres
                    case "km-mi": //1km = 0.62 miles
                        retValue = 0.62 * dInputVal;
                        break;

                    //Metres to Feet    
                    case "m-ft":  //1 mtr = 3.28 feet
                        retValue = 3.28 * dInputVal;
                        break;

                    //Feet to Metres 
                    case "ft-m":  //1 mtr = 3.28 feet
                        retValue = dInputVal / 3.28;
                        break;
                }

            }
            return Math.Round(retValue) < 10 ? "0" + Math.Round(retValue).ToString() : Math.Round(retValue).ToString();
            //return Math.Round(retValue).ToString();
        }

        /// <summary>
        /// 
        /// converts Elevation Value from Metres to feet .this is used for culture en-US
        /// </summary>
        /// <param name="p_dist"></param>
        /// <returns></returns>
        public double MetresToFeet(string p_elevation)
        {
            return 0.0;
        }

        public string ShortMonthDayPattern(DateTimeFormatInfo dt)
        {
            string nl = dt.ShortDatePattern;
            nl = nl.Replace("y", "");
            if (nl[0] != 'M' && nl[0] != 'd') nl = nl.Substring(1);
            int l = nl.Length - 1;
            while (nl[l] != 'M' && nl[l] != 'd')
            {
                nl = nl.Substring(0, l);
                l = nl.Length - 1;
            }
            if (nl[l] != 'M' && nl[l] != 'd') nl = nl.Substring(0, l);
            return nl;
        }


        public string CheckLatLongForCountry(double dLat, double dLong)
        {
            DataSet dsLocation = new DataSet();
            string proxyName = ConfigurationManager.AppSettings["proxyName"];
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            XmlDocument xdoc = new XmlDocument();
            string username = ConfigurationManager.AppSettings["UserName"];
            string domain = ConfigurationManager.AppSettings["Domain"];
            string password = ConfigurationManager.AppSettings["Password"];
            webClient.Credentials = new System.Net.NetworkCredential(username, password, domain);
            webClient.Headers.Add(HttpRequestHeader.UserAgent, "test");
            WebProxy proxy = new WebProxy(proxyName, true);
            proxy.Credentials = new System.Net.NetworkCredential(username, password, domain);
            //webClient.Proxy = proxy;
            string strReturn;

            strReturn = webClient.DownloadString("http://ws.geonames.net/countrycode?token=AISGeonames&style=full&featureClass=P&username=lostinswiss&lat=" + dLat + "&lng=" + dLong);

            //strReturn = "";
            return strReturn;
        }

    }

}