using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

namespace Syngenta.AgriCast.AgriInfo.View
{
    public interface IAgriInfo
    {
        string strTranslatedText
        {
            get;
            set;
        }
        string Name
        {
            get;
            set;
        }
      
        ArrayList ar
        {
            get;
            set;
        }
        DataSet ds
        {
            get;
            set;
        }
        DataTable dtGdd
        {
            get;
            set;
        }
        Boolean ExcelFlag
        {
            get;
            set;
        }
        string Selected
        {
            get;
            set;
        }
        DateTime StartDate
        {
            get;
            set;
        }
        List<string> GddValues
        {
            get;
            set;
        }
    }
}