using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.SessionState;
using System.Web;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriWeb.NearByStation.DataObjects;

namespace Syngenta.AgriCast.Common.DTO
{
    public class DataPointInfo
    {
        private int intAlt;
        private string strStnName = "";
        private double fStnLat;
        private double fStnLong;
        private int intDataPointId;
        private string strDirection = "";
        private enumDataSources eStnDataSource = enumDataSources.glbStnFcst;
        private DateTime sunrise;
        private DateTime sunset;
        private int offset;
        private int iDstOnVal;
        private static DataPointInfo objDataPointInfo;
        private double fdistance;

        public double distance
        {
            get { return fdistance; }
            set { fdistance = value; }
        }
        public int altitude
        {
            get
            {
                return intAlt;
            }
            set
            {
                intAlt = value;
            }
        }

        public NearByPointSettings NearbyPointSettings
        {
            get;
            set;
        }


        public int DayOffset
        {
            get { return offset; }
            set { offset = value; }
        }

        public int iDstOn
        {
            get;
            set;
        }

        public string directionLetter
        {
            get
            {
                return strDirection;
            }
            set
            {
                strDirection = value;
            }
        }

        public double stationLatitude
        {
            get
            {
                return fStnLat;
            }
            set
            {
                fStnLat = value;
            }
        }

        public double stationLongitude
        {
            get
            {
                return fStnLong;
            }
            set
            {
                fStnLong = value;
            }
        }

        /*RunTime error in sprayweather - Begin*/
        public string stationName
        {
            get
            {

                //return strStnName;
                return HttpUtility.UrlDecode(strStnName);

            }
            set
            {
                //strStnName = value;
                strStnName = HttpUtility.UrlEncode(value);
            }
        }
        /*RunTime error in sprayweather - End*/

        public int DataPoint
        {
            get
            {
                return intDataPointId;
            }
            set
            {
                intDataPointId = value;
            }
        }

        public DateTime SunRise
        {
            get { return sunrise; }
            set { sunrise = value; }
        }
        public DateTime SunSet
        {
            get { return sunset; }
            set { sunset = value; }
        }

        public bool CheckStationValidity()
        {
            return true;
        }

        public static DataPointInfo getDataPointObject
        {
            get
            {

                if (CommonUtil.IsSessionAvailable())
                {
                    HttpSessionState Session = HttpContext.Current.Session;
                    if (Session["objDataPointInfo"] == null)
                    {
                        Session["objDataPointInfo"] = new DataPointInfo();
                    }
                    return (DataPointInfo)Session["objDataPointInfo"];
                }
                else
                {
                    if (objDataPointInfo == null) objDataPointInfo = new DataPointInfo();
                    return objDataPointInfo;
                }
            }
            set
            {
                HttpSessionState Session = HttpContext.Current.Session;

                //HttpSessionState Session = null;
                if (HttpContext.Current != null) Session = HttpContext.Current.Session;
                if (HttpContext.Current != null && Session != null)
                {
                    Session["objDataPointInfo"] = value;
                }
                else
                {
                    objDataPointInfo = value;
                }
            }
        }

        public DataPointInfo()
        {
            this.NearbyPointSettings = new NearByPointSettings();
        }
    }
}
