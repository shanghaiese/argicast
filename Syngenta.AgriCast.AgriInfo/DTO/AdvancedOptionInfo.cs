using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syngenta.AgriCast.Common.Service;
using System.Web.SessionState;

namespace Syngenta.AgriCast.AgriInfo.DTO
{
    public class AdvancedOptionInfo
    {
        private string series;
        private string serie;
        private string aggregation;
        private string accumulate;
        private int year;
        private string trend;
        private string altitude;
        private string method;
        private string cap;
        private string strbase;
        private static AdvancedOptionInfo objAdvancedOptionInfo;
        
        public string Aggregation
        {
            get { return aggregation; }
            set { aggregation = value; }
        }
       
        public string Accumulate
        {
            get { return accumulate; }
            set { accumulate = value; }
        }
       
        public int Year
        {
            get { return year; }
            set { year = value; }
        }
       
        public string Trend
        {
            get { return trend; }
            set { trend = value; }
        }
     

        public string Altitude
        {
            get { return altitude; }
            set { altitude = value; }
        }
        public string Method
        {
            get { return method; }
            set { method = value; }
        }
        public string Cap
        {
            get { return cap; }
            set { cap = value; }
        }
        public string Base
        {
            get { return strbase; }
            set { strbase = value; }
        }
        public string Serie
        {
            get { return serie; }
            set { serie = value; }
        }
     
        public string Series
        {
            get { return series; }
            set { series = value; }
        }
        public static AdvancedOptionInfo getAdvancedOptionObject
        {
           
            get
            {              

                if (CommonUtil.IsSessionAvailable())
                {
                    HttpSessionState Session = HttpContext.Current.Session;
                    if (Session["objAdvancedOptionInfo"] == null)
                    {
                        Session["objAdvancedOptionInfo"] = new AdvancedOptionInfo();
                    }
                    return (AdvancedOptionInfo)Session["objAdvancedOptionInfo"];
                }
                else
                {
                    if (objAdvancedOptionInfo == null) objAdvancedOptionInfo = new AdvancedOptionInfo();
                    return objAdvancedOptionInfo;
                }
            }
            set
            {
                HttpSessionState Session = HttpContext.Current.Session;

                if (HttpContext.Current != null) Session = HttpContext.Current.Session;
                if (HttpContext.Current != null && Session != null)
                {
                    Session["objAdvancedOptionInfo"] = value;
                }
                else
                {
                    objAdvancedOptionInfo = value;
                }
            }
        }
    }
}