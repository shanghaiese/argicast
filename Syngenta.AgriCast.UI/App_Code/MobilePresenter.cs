using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls; 
using System.Data;
using Syngenta.AgriCast.Icon.View;
using Syngenta.AgriCast.Icon.Presenter;  
using System.Text;
using Syngenta.AgriCast.Tables; 
using Syngenta.AgriCast.Tables.Presenter; 
using Syngenta.AgriCast.Tables.View;
using System.Collections;
using System.Globalization;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common;
using Syngenta.AgriCast.Common.Service;
using System.IO;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.Icon.Service;
using Syngenta.AgriCast.Icon.DTO;
using Syngenta.Agricast.Modals;
using Syngenta.AgriCast.Tables.Service;

/// <summary>
/// Summary description for MobilePresenter
/// </summary>
namespace Syngenta.AgriCast.Mobile.Presenter
{
    public class MobilePresenter
    {
        # region Declarations
        IconService objIconServ = new IconService();
        DTOIcon objDTOSIcon = new DTOIcon();
        TablePresenter objTablePre;
        CommonUtil objComm;
        ServicePresenter objSvcPre;
        ServiceHandler objServiceHandler;
        TableService objTblSvc;
        ServiceInfo svcInfo = new ServiceInfo();
        ITable oItable;
        DataSet dsTableSeries;
        PaletteMap objPm;
        DataTable dtSeriesDetails;
        DataTable dtPageNodes;
        List<string[]> al;
        List<string[]> alSeriesLegend;
        DataTable dtTransposed;
        string CultureCode;
        string strAllignDetails;
        DataTable dtByDays;
        int step = 1;
        int NumOfDays;
        GridView gvDynamic;
        CultureInfo VariantCulture;
        DataTable dtModalInput = new DataTable();
        DataTable dtOutput;
        DataPointInfo objDataPointInfo;
        DataTable dtByDaysCopy = null;
        DataSet dsSeriesData;
        DataTable dtSeries;
        const string TBLDAYSROWS = "tblDaysRows";
        const string TBLSERIESROWS = "tblSeriesRows";
        const string THREEHOURLY = "3hourly";
        const string SIXHOURLY = "6hourly";
        const string EIGHTHOURLY = "8hourly";
        const string TWELVEHOURLY = "12hourly";
        const string HOURLY = "hourly";
        const string SERVICEPAGENAME = "servicePage";

         string WIND_DIRECTION = "winddirection";
         string WIND_DIRECTION_TEXT = "winddirectionText";
         string WIND_SPEED = "windspeed";
         string PRECIP = "precipamount";
         string TEMPMAX = "maxtempair";
         string IMAGEURL = "imageurl";
         string TEMPMIN = "mintempair";      
          
        #endregion

        public MobilePresenter()
        {
            objComm = new CommonUtil();
            objServiceHandler = new ServiceHandler();
            objTblSvc = new TableService();
            objSvcPre = new ServicePresenter();
            svcInfo = ServiceInfo.ServiceConfig;
        }

        public ArrayList GetIconData(string Name)
        {

            //Get The Icon data
            return objIconServ.getWeatherIcons(objDTOSIcon, Name);
        }

        public DataSet GetTableData(ArrayList alIocnList, string Node, string Name)
        {
            //Use the Service Layer Method
            // objTablePre.getCultureCode();
            DataSet dsByDays = new DataSet();
            //call readalign() to get the config data
            strAllignDetails = objServiceHandler.ReadAllignment(Node, Name);

            string datasource = strAllignDetails.Split(',')[2];
            string aggregation = strAllignDetails.Split(',')[5];
            int start = Convert.ToInt32(strAllignDetails.Split(',')[3]);
            int end = Convert.ToInt32(strAllignDetails.Split(',')[4]);
            end = end > 0 ? end - 1 : end + 1;
            string ruleset = strAllignDetails.Split(',')[6];
            string pallette = strAllignDetails.Split(',')[7];
            string Nodename = strAllignDetails.Split(',')[8];


            //For Non Rules Set data         
            if (string.IsNullOrEmpty(ruleset))
            {
                //Get The NodeList 
                alSeriesLegend = objServiceHandler.getNodeList(Node);

                ///Table alSeriesLegend Legend
                dsSeriesData = objServiceHandler.GetTableSeries(Node, Name);
                objSvcPre.ChangeUnits(dsSeriesData.Tables[0], svcInfo.Unit, svcInfo.WUnit);
                dsByDays.Tables.Add(dsSeriesData.Tables[0].Copy());

                //Actual Dtbydays data
                dtByDays = objTblSvc.GetSeriesData(dsSeriesData.Tables[0], aggregation, datasource, start, end,0,0);

                //Type type = dtByDays.Columns[WIND_DIRECTION].GetType();

                DataColumn dc = new DataColumn(WIND_DIRECTION_TEXT);
                dc.DataType = System.Type.GetType("System.String");
                dtByDays.Columns.Add(dc);
                 
                dc = new DataColumn(IMAGEURL);
                dc.DataType = System.Type.GetType("System.String");
                dtByDays.Columns.Add(dc);

                List<string> colList = new List<string>();
                var cols = from DataColumn col in dtByDays.Columns
                           select col.ColumnName;
                colList =  cols.ToList();
                WIND_DIRECTION = colList.Where(x => x.ToString().ToLower().Contains(WIND_DIRECTION)).FirstOrDefault();
                PRECIP = colList.Where(x => x.ToString().ToLower().Contains(PRECIP)).FirstOrDefault();
                //Convert WindSpeed to text
                if (dtByDays.Columns.Contains(WIND_DIRECTION))
                {
                    for (int i = 0; i < dtByDays.Rows.Count; i++)
                    {
                        int dir;
                        dtByDays.Rows[i][WIND_DIRECTION_TEXT] = objComm.getTextDirection(Int32.TryParse(dtByDays.Rows[i][WIND_DIRECTION].ToString(),out dir)?dir:0);
                        dtByDays.Rows[i][PRECIP] = Math.Round(Decimal.Parse(dtByDays.Rows[i][PRECIP].ToString() == "" ? "0.0" : dtByDays.Rows[i][PRECIP].ToString()));
                        if (alIocnList != null && i < alIocnList.Count)
                        {
                            dtByDays.Rows[i][IMAGEURL] = "../../Images/Icons/" + Path.GetFileName(alIocnList[i].ToString());
                        }
                    }
                }
            }
            else
            {
                //Call the Method with RuleSet Data

                //objTablePre.CreateTables(oItable.Name, oItable.Node, aggregation, datasource, start, end);
                // objTablePre.GetCompleteRuleset(oItable.Name, oItable.Node, aggregation, datasource, start, end,ruleset,pallette,Nodename, IRobj);
            }

            int hoursForData = 0;
            DateTime startDateTime;
            DateTime endDateTime;
            string newDateColumnName = "NewDate";

            dtByDays.Columns.Add(newDateColumnName);

            if (aggregation == "8Hourly")
            {
                hoursForData = 8;
            }
            else if (aggregation == "12Hourly")
            {
                hoursForData = 12;
            }
            else
            {
                hoursForData = 0;
            }

            if (hoursForData > 0 && dtByDays != null)
            {
                if (dtByDays.Rows.Count > 0)
                {
                    for (int i = 0; i < dtByDays.Rows.Count; i++)
                    {

                        endDateTime = Convert.ToDateTime(dtByDays.Rows[i][0]);
                        startDateTime = Convert.ToDateTime(dtByDays.Rows[i][0]);
                        if (hoursForData > 0)
                        {
                            startDateTime = startDateTime.Subtract(TimeSpan.FromHours(Convert.ToDouble(hoursForData)));
                            dtByDays.Rows[i][newDateColumnName] = startDateTime.ToString("ddddd dd/MM HH:mm") + " - " + endDateTime.ToString("HH:mm");
                        }
                        else
                        {
                            dtByDays.Rows[i][newDateColumnName] = startDateTime.ToString("ddddd dd/MM");
                        }
                    }
                }
            }
            try
            {
                DataTable dt = dtByDays.Copy();
                dt.TableName = "dtBydays";
                dsByDays.Tables.Add(dt);
            }
            catch
            {
            }
            return dsByDays;
        }

    }
}