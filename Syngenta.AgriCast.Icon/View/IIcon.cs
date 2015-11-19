using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace Syngenta.AgriCast.Icon.View
{
    public interface IIcon
    {
        int iDesiredNoOfDays
        {
            get;
            set;
        }   

        ArrayList alIocnList
        {
            get;
            set;
        }

        DataTable dtIconList
        {
            get;
            set;
        }

        int iColumnsPerDay
        {
            get;
            set;
        }

        string strCulCode
        {
            get;
            set;
        }
        string iFeedbackEnabled
        {
            get;
            set;
        }
        string iTransText
        {
            get;
            set;
        }
        bool plotHeader
        {
            get;
            set;
        }
    }
}
