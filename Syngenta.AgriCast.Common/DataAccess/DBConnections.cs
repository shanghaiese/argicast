using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Syngenta.AgriCast.Common.DataAccess
{
    public class DBConnections
    {
       
        /// <summary>
        /// Method to read the connection string from web.config file
        /// </summary>
        public string getConnectionString(string strKeyVal)
        {
            string strLocationDBConn = ConfigurationManager.ConnectionStrings[strKeyVal].ConnectionString;
            return strLocationDBConn;
        }
        
       
       
    }
}