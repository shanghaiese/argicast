using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using Syngenta.AgriCast.LocationSearch.DataAccess;
using System.Collections;

namespace Syngenta.AgriCast.LocationSearch.Service
{
    public class locSearchDTO
    {
        locSearchData locDA = new locSearchData();
        /// <summary>
        /// Constructor
        /// </summary> 
        public locSearchDTO()
        {
            
        }

        /// <summary>
        /// this is to get the results from an external source
        /// </summary>
        public XmlDocument getLocResultsWS(string strSearch, string strCntry)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// this is to convert2XML the response from DB to XML for web service
        /// </summary>
        public XmlDocument convert2XML(DataTable dtLocResults)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// to convert results from external source to a datatable
        /// </summary>
        public DataTable convert2DataTable(string strXML)
        {
            throw new System.NotImplementedException();
        }

        public DataTable getLocResults(string strSearch, string strCntry)
        {
            throw new System.NotImplementedException();
        }    
           
    }
}
