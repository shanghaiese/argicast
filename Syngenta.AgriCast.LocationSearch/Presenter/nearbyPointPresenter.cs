using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data; 
using Syngenta.AgriCast.LocationSearch.View;
using Syngenta.AgriCast.LocationSearch.Service;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common.Presenter;
using System.Web;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common;

namespace Syngenta.AgriCast.LocationSearch.Presenter
{
    public class nearbyPointPresenter
    {
        /// <summary>
        /// service object
        /// </summary>
        nearbyPointService nearBySvc = new nearbyPointService();
        ServiceHandler objSvcHandler = new ServiceHandler();
        ServicePresenter objSvcPre = new ServicePresenter();
        CommonUtil objComUtil = new CommonUtil();
        /// <summary>
        /// interface object
        /// </summary>
        ILocSearch ILocation;
        /// <summary>
        /// Name of the near by point selected
        /// </summary>
        private string nearbyName;
        /// <summary>
        /// latlong of the selected station or grid cell
        /// </summary>
        private float fLat;
        /// <summary>
        /// distance and direction details text
        /// </summary>
        private string nearbyText;
        private float fLong;
        private string strStnProvider;
        Common.DTO.DataPointInfo objStnInfo;

        /// <summary>
        /// constructor
        /// </summary>
        public nearbyPointPresenter(ILocSearch ISearch)
        {
            if (ISearch != null)
            {
                ILocation = ISearch;
            }
        }

        public void getNearbyPoints(double Lat, double Long)
        {
            try
            {
                ILocation.dtNearByPoints = nearBySvc.getNearbydata(Lat, Long);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_NEARBYPONITS_FAILURE) + ex.Message.ToString();
                return;
            }
        }

        public void getNearbyDataService(double dlat, double dlong, int intMaxAllowedDist, int intMaxAllowedAlt, int intResultCount)
        {
            try
            {
                ILocation.dtNearByPoints = nearBySvc.getNearbyDataService(dlat, dlong, intMaxAllowedDist, intMaxAllowedAlt, intResultCount);
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objComUtil.getTransText(Constants.LS_NEARBYPONITS_FAILURE) + ex.Message.ToString();
                return;
            }
        }

        //public void getWeatherData(DataPointInfo objStnInfo)
        //{
        //    throw new System.NotImplementedException();
        //}

        public NearByPointSettings setNearbyStationAttributes()
        {
            return objSvcHandler.getNearbyDataPoint();
        }
    }
}
