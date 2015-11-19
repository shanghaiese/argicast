using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Syngenta.AgriCast.Tables.Presenter;
using Syngenta.AgriCast.Common.Service;
using System.Collections;
using System.Drawing;

namespace Syngenta.AgriCast.Tables.View
{
    public interface ITable
    {
        string strAllignDetails
        {
            get;
            set;

        }
        DataSet dsSeriesData
        {
            get;
            set;
        }
        PaletteMap pmPallette
        {
            get;
            set;
        }
        DataTable dtSeries
        {
            get;
            set;
        }
        List<string[]> alSeriesLegend
        {
            get;
            set;
        }
        DataTable dtPage
        {
            get;
            set;
        }
        string strCulCode
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
        int numOfDays
        {
            get;
            set;
        }
        int Step
        {
            get;
            set;
        }
        DataTable dt
        {
            get;
            set;
        }
      
        string RuleSetColor
        {
            get;
            set;
        }
        Color FontColor
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
        List<string[]> ExcelList
        {
            get;
            set;
        }
    }
}