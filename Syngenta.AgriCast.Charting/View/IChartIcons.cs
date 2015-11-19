using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

namespace Syngenta.AgriCast.Charting.View
{
    public interface IChartIcons
    {
        string strAllignDetails
        {
            get;
            set;

        }

        int numOfDays
        {
            get;
            set;
        }

        string Node
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }
        List<string[]> alSeriesLegend
        {
            get;
            set;
        }
        DataTable dtSeries
        {
            get;
            set;
        }
        DataTable dtWindIcons
        {
            get;
            set;
        }
        DataSet dsSeriesData
        {
            get;
            set;
        }
        DataTable dtTableSeries
        {
            get;
            set;
        }
        DataTable dtTableLegends
        {
            get;
            set;
        }
        string strCulCode
        {
            get;
            set;
        }
        DataTable dt
        {
            get;
            set;
        }
        int Step
        {
            get;
            set;
        }
        ArrayList alSeries
        {
            get;
            set;
        }
        DataTable dtByDays
        {
            get;
            set;
        }

        /* UAT issue - icon chart issue - Jerrey - Start */
        int IconChartWidth
        {
            get;
            set;
        }
        /* UAT issue - icon chart issue - Jerrey - End */
        /*Wind Icon as a Sepearate component -- END*/
        DataTable dtWindIconSettings
        {
            get;
            set;

        }
        /*Wind Icon as a Sepearate component -- END*/
    }
}