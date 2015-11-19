using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syngenta.AgriCast.Common.Service;
using System.Web.SessionState;

namespace Syngenta.AgriCast.AgriInfo.DTO
{
    public class SeriesInfo
    {
        private static SeriesInfo objSeriesInfo;
        private List<AdvancedOptionInfo> seriesList;

        public List<AdvancedOptionInfo> SeriesList
        {
            get { return seriesList; }
            set { seriesList = value; }
        }

        public static SeriesInfo getSeriesInfoObject
        {
            get
            {

                if (CommonUtil.IsSessionAvailable())
                {
                    HttpSessionState Session = HttpContext.Current.Session;
                    if (Session["objSeriesInfo"] == null)
                    {
                        Session["objSeriesInfo"] = new SeriesInfo();
                    }
                    return (SeriesInfo)Session["objSeriesInfo"];
                }
                else
                {
                    if (objSeriesInfo == null) objSeriesInfo = new SeriesInfo();
                    return objSeriesInfo;
                }
            }
            set
            {
                HttpSessionState Session = HttpContext.Current.Session;

                if (HttpContext.Current != null) Session = HttpContext.Current.Session;
                if (HttpContext.Current != null && Session != null)
                {
                    Session["objSeriesInfo"] = value;
                }
                else
                {
                    objSeriesInfo = value;
                }
            }
        }
    }
}