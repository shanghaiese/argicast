using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for IRuleSets
/// </summary>
namespace Syngenta.Agricast.Modals
{
    public interface IRuleSets
    {

        DateTime SunRise
        {
            get;
            set;
        }

        DateTime SunSet
        {
            get;
            set;
        }

        DataTable DtInput
        {
            get;
            set;
        }

        DataTable DtOutput
        {
            get;
            set;
        }


        DateTime StartDate
        {
            get;
            set;
        }

        DateTime EndDate
        {
            get;
            set;
        }
    }
}