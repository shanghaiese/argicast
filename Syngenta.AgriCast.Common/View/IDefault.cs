using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

namespace Syngenta.AgriCast.Common.View
{
    public interface IDefault
    {
        DataTable dtMenuData
        {
            get;
            set;
        }
        List<string[]> alNodeList
        {
            get;
            set;
        }
        string DefaultTab
        {
            get;
            set;
        }
        DataSet dsMossMenu
        {
            get;
            set;
        }
        string ExtNavigation
        {
            get;
            set;
        }
        string TransString
        {
            get;
            set;
        }
        bool IsSecure
        {
            get;
            set;
        }
        string AllowedRoles
        {
            get;
            set;
        }
        DataTable dtLegenddetails
        {
            get;
            set;
        }
        string encryptKey
        {
            get;
            set;
        }

        /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - BEGIN*/
        string MossReturnUrl
        {
            get;
            set;
        }
        /*IM01184669 - New Agricast - redirection to a login page if the publicationis protected - END*/

      
    }
}