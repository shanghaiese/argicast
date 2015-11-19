using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.LocationSearch.DataAccess;
using Syngenta.AgriCast.Common.DataAccess;

namespace Syngenta.AgriCast.LocationSearch.Service
{
   public class nearbyPointService
    {
        nearbyPointDB nearByDA = new nearbyPointDB();
        Common.DTO.DataPointInfo objStnInfo= new DataPointInfo();
        /// <summary>
        /// Constructor
        /// </summary>
        public nearbyPointService()
        {
            
        }

       //Method to get the nearby stations data. This is called from
        public DataTable getNearbydata(double Lat, double Long)
        {
            return nearByDA.getNearbyData(Lat,Long);
        }

        public DataTable getNearbyDataService(double dlat, double dlong, int intMaxAllowedDist, int intMaxAllowedAlt, int intResultCount)
        {
            return nearByDA.getNearbyDataService(dlat, dlong, intMaxAllowedDist, intMaxAllowedAlt, intResultCount);
        }
        //public XmlDocument formatDatatoXML(DataTable dtStnResults)
        //{
        //    throw new System.NotImplementedException();
        //}
        
        //public void getWeatherData(DataPointInfo objStnInfo)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
