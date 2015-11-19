using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Syngenta.AgriCast.Common.DataAccess;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AIS.NearbyStation.Internal.Package;
using Syngenta.AgriWeb.NearByStation.DataObjects;

namespace Syngenta.AgriCast.LocationSearch.DataAccess
{
    class nearbyPointDB
    {
        DBConnections objDB = new DBConnections();
        ServiceHandler objSvcHandler = new ServiceHandler();
        DataPointInfo objDatapointInfo;

        public DataTable getNearbyData(double Lat, double Long)
        {
            objDatapointInfo = DataPointInfo.getDataPointObject;
            return NearByStationSearchConsumer.GetNearByData(Lat, Long, objDatapointInfo.NearbyPointSettings.MaxAllowedDistance, objDatapointInfo.altitude, objDatapointInfo.NearbyPointSettings.ResultCount, objDatapointInfo.NearbyPointSettings.DataSource, "Agricast V2");
            
        }

        public DataTable getNearbyDataService(double Lat, double Long, int intMaxAllowedDist, int intMaxAllowedAlt, int intResultCount)
        {
            
            return NearByStationSearchConsumer.GetNearByData(Lat, Long, intMaxAllowedDist, intMaxAllowedAlt, intResultCount, enumDataSources.glbStnFcst, "Agricast V2");
        }
    }
}
