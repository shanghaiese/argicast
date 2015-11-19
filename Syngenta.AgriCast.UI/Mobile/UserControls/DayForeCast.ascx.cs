#region Namespace
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
using Syngenta.AgriCast.Mobile.Presenter;
using System.Reflection;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.ExceptionLogger;
#endregion

#region Class
public partial class Mobile_DayForeCast : System.Web.UI.UserControl
{
    # region Declarations
    MobilePresenter objMobPresenter = null;
    CommonUtil objComm;
    ServiceHandler objServiceHandler;
    ServiceInfo svcInfo;
    ServicePresenter objSvcPre;
    CommonUtil objCommonUtil = new CommonUtil();
    string AllignDetails = string.Empty;

    ArrayList alIocnList;
    int iIconsCount = 0;
    DataTable dtModalInput = new DataTable();
    DataTable dtByDays = null;
    DateTime dFirstDate;
    const string TBLDAYSROWS = "tblDaysRows";
    const string TBLSERIESROWS = "tblSeriesRows";

    const string WIND_DIRECTION = "winddir";
    const string WIND_DIRECTION_TEXT = "winddirtext";
    const string WIND_SPEED = "windspeed";
    const string PRECIP = "precip";
    const string TEMPMAX = "maxTemp";
    const string IMAGEURL = "imageurl";
    const string TEMPMIN = "minTemp";
    const string SERVICEPAGENAME = "servicePage";
    const string DATE = "date";

    const string TRNSTAG_TEMPMAX = "temperature_max";
    const string TRNSTAG_TEMPMIN = "temperature_min";
    const string TRNSTAG_WIND = "we_wind_kmh";
    const string TRNSTAG_RAIN = "precipitation";


    string Node = string.Empty;
    string Name = string.Empty;

    public string WUnit = "kmh";
    public string TUnit = "C";
    public string PUnit = "mm";

    public string Max = "Max: ";
    public string Min = "Min: ";
    public string Wind = "Wind: ";
    public string Rain = "Rain: ";

    string NodeSpray;
    string NameSpray;

    #endregion

    #region Events

    #region Page_Init
    protected void Page_Init(object sender, EventArgs e)
    {

        //Initialize the objects
        objComm = new CommonUtil();
        objServiceHandler = new ServiceHandler();
        objMobPresenter = new MobilePresenter();
        svcInfo = new ServiceInfo();
        svcInfo = ServiceInfo.ServiceConfig;
    }
    #endregion

    #region Page_Load
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //Get the NodeList
            List<string[]> objList = objServiceHandler.getNodeList(SERVICEPAGENAME);

            for (int i = 0; i < objList.Count; i++)
            {
                if (objList[i].Contains("icon") && objList[i][1].ToString().ToLower().Contains("detail"))
                {
                    //Name = objList[i][1].ToString();
                    //Node = objList[i][0].ToString();
                    // objIconPre = new IconPresenter(this, Name);

                    //Get The Icon data
                    //objIconPre.getIconData();
                    alIocnList = objMobPresenter.GetIconData(objList[i][1].ToString());
                    iIconsCount = alIocnList.Count;

                }

                if (objList[i].Contains("tblSeriesRows") && objList[i][1].ToString().ToLower().Contains("detail"))
                {
                    Name = objList[i][1].ToString();
                    Node = objList[i][0].ToString();
                    //objTablePre = new TablePresenter(this, Name);
                    DisplayTableData();
                }

                //if (objList[i].Contains("tblDaysRows"))
                //{
                //    //Load spray window user control
                //    NameSpray = objList[i][1].ToString();
                //    NodeSpray = objList[i][0].ToString();
                //}
            }
            if (svcInfo.Unit.ToLower() == "imperial")
            {
                TUnit = "F";
                PUnit = "in";
            }
            switch (svcInfo.WUnit.ToLower())
            {
                case "beaufort":
                    WUnit = "bft";
                    break;
                case "mph":
                    WUnit = "mph";
                    break;
                case "kmh":
                    WUnit = "kmh";
                    break;
            }

            // ChangeLabelText();
        }
        catch (Exception ex)
        {
            objSvcPre = new ServicePresenter();
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_LOAD_FAILURE) + " : " + ex.Message.ToString();
            return;
        }
    }
    #endregion

    /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
    /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */
    /* label: 
     * value: 
     * unit: "&deg;"
     * */
    private void GenerateHtmlForSeries(StringBuilder strBuilder, string label, string value, string unit = "")
    {
        strBuilder.Append("<div>");
        strBuilder.AppendFormat(@"<label style=""color: Gray;"">{0}:&nbsp;</label>", label);
        strBuilder.AppendFormat(@"<span style=""color: Gray;"">{0}{1}<label style=""font-size: smaller;""></label></span>",
                                value, unit);
        strBuilder.Append("</div>");
    }
    /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */

    #region Repeater1_ItemDataBound
    protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
        /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */

        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            var drView = e.Item.DataItem as DataRowView;
            DataRow dr = drView.Row;

            var strBuilder = new StringBuilder();

            foreach (DataRow row in dtSeries.Rows)
            {
                var colName = string.Format("{0}{1}", row["aggregationFunction"].ToString(), row["name"].ToString());
                var tagName = row["trnsTag"].ToString();
                GenerateHtmlForSeries(strBuilder, getTranslatedText(tagName), dr[colName].ToString());
            }

            var ltlSeries = e.Item.FindControl("ltlSeries") as Literal;
            ltlSeries.Text = strBuilder.ToString();
        }

        /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */

        //if (e.Item.ItemType == ListItemType.Item)
        //{
        //    Mobile_UserControls_SprayWindowImg spray = (Mobile_UserControls_SprayWindowImg)e.Item.FindControl("ucSprayWindowImg1");
        //    PropertyInfo propertyInfoName = spray.GetType().GetProperty("Name");
        //    propertyInfoName.SetValue(spray, Convert.ChangeType(NameSpray, propertyInfoName.PropertyType), null);

        //    PropertyInfo propertyInfoNode = spray.GetType().GetProperty("Node");
        //    propertyInfoNode.SetValue(spray, Convert.ChangeType(NodeSpray, propertyInfoNode.PropertyType), null);

        //    PropertyInfo propertyInfoDate = spray.GetType().GetProperty("Date");
        //    propertyInfoDate.SetValue(spray, Convert.ChangeType(DataBinder.Eval(e.Item.DataItem, "date"), propertyInfoDate.PropertyType), null);
        //}

    }
    #endregion

    #endregion

    #region Methods

    void ChangeLabelText()
    {

        Max = getTranslatedText("Max: ");
        Min = getTranslatedText("Min: ");
        Wind = getTranslatedText("Wind: ");
        Rain = getTranslatedText("Rain: ");
    }

    string getTranslatedText(string text)
    {
        if (Session["serviceInfo"] != null)
        {
            svcInfo = (ServiceInfo)Session["serviceInfo"];
        }
        else
        {
            svcInfo = ServiceInfo.ServiceConfig;
        }
        try
        {
            string strCul = svcInfo.Culture;
            objSvcPre = new ServicePresenter();
            text = objSvcPre.getTranslatedText(text, strCul);
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_TRANS_ERROR) + " : " + ex.Message.ToString();

        }
        return text;
    }
    #region GetSprayWindow
    /// <summary>
    /// To get the spray window table.
    /// </summary>
    /// <param name="dtSprayWindow">DataTable with spray window values</param>
    /// <returns>string</returns>
    /* public string GetSprayWindow(DataTable dtSprayWindow)
     {
         StringBuilder sb = new StringBuilder("");
         sb.Append(" <table align=\"center\" class=\"sprayWindow\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
         sb.Append(" <tbody> ");


         foreach (DataRow dr in dtSprayWindow.Rows)
         {

             sb.Append(" <tr> <td> ");
             sb.Append(Convert.ToDouble(dr[""].ToString()));
             sb.Append(" </td> </tr> ");
         }

         sb.Append(" </tbody> ");
         sb.Append(" <tfoot> ");

         int i = 0;
         foreach (DataRow dr in dtSprayWindow.Rows)
         {
             double start = Convert.ToDouble(dr[""].ToString());
             double end = Convert.ToDouble(dr[""].ToString());
             double step = Convert.ToDouble(dr[""].ToString());

             if (i == 0)
             {
                 sb.Append(" <tr> <td  class=\"firstCell\"> ");
                 sb.Append("<div>");
                 sb.Append(start);
                 sb.Append(" </div>");
                 sb.Append(" <div>");
                 //sb.Append(curr);
                 sb.Append(" </div>");
                 sb.Append(" </td> </tr> ");
                 i++;
             }
             else
             {
                 sb.Append(" <tr> <td> ");
                 sb.Append(Convert.ToDouble(dr[""].ToString()));
                 sb.Append(" </td> </tr> ");
             }

         }

         //string start = "0";
         //string end = "24";
         //int step = 1;
         //int hrSt, hrEn;
         //int.TryParse(start, out hrSt);
         //int.TryParse(end, out hrEn);

         //TimeSpan startTime = new TimeSpan(hrSt, 0, 0);
         //TimeSpan endTime = new TimeSpan(hrEn, 0, 0);

         //StringBuilder sb = new StringBuilder();
         //sb.Append(string.Format("{0:hh\\:mm}", new TimeSpan(hrSt, 0, 0)));

         //TimeSpan curr = startTime + new TimeSpan(step, 0, 0);
         //int i = 0;
         //while (curr <= endTime)
         //{
         //    sb.Append(", ");
         //    sb.Append(string.Format("{0:hh\\:mm}", curr));

         //    curr = curr + new TimeSpan(step, 0, 0);
         //    if (i > 24)
         //        break;
         //    else
         //        i++;
         //}

         //        <tr>
         //            <td class="firstCell">
         //                <div>
         //                    00:00
         //                </div>
         //                <div>
         //                    03:00
         //                </div>
         //            </td>
         //            <td class="textRight">
         //                <div>
         //                    06:00
         //                </div>
         //            </td>
         //            <td class="textRight">
         //                <div>
         //                    09:00
         //                </div>
         //            </td>
         //            <td class="textRight">
         //                <div>
         //                    12:00
         //                </div>
         //            </td>
         //            <td class="textRight">
         //                <div>
         //                    15:00
         //                </div>
         //            </td>
         //        </tr>
         //    </tfoot>
         //</table>

         return sb.ToString();
     }*/

    #endregion

    void changeColNames(DataTable dtByDays)
    {
        foreach (DataColumn col in dtByDays.Columns)
        {
            string colname = col.ColumnName.ToLower();
            if (colname.Contains("temp"))
            {
                if (colname.Contains("daytime") || colname.Contains("maxt"))
                {
                    dtByDays.Columns[colname].ColumnName = TEMPMAX;
                }
                else
                {
                    dtByDays.Columns[colname].ColumnName = TEMPMIN;
                }
            }
            if (colname.Contains("windspeed"))
                dtByDays.Columns[colname].ColumnName = WIND_SPEED;
            if (colname.Contains("direction"))
            {
                if (colname.Contains("directiontext"))
                    dtByDays.Columns[colname].ColumnName = WIND_DIRECTION_TEXT;
                else
                    dtByDays.Columns[colname].ColumnName = WIND_DIRECTION;
            }
            if (colname.Contains("precip"))
                dtByDays.Columns[colname].ColumnName = PRECIP;
        }
    }
    #region DisplayTableData

    void modifyData(DataTable dt)
    {
        try
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    Type typ = dt.Rows[i][j].GetType();
                    switch (typ.Name.ToLower())
                    {
                        case "int32":
                            dt.Rows[i][j] = Math.Round(Convert.ToDouble(dt.Rows[i][j]));
                            break;
                        case "decimal":
                            dt.Rows[i][j] = Math.Round(Convert.ToDouble(dt.Rows[i][j]));
                            break;
                    }

                }
            }
        }
        catch (Exception ex)
        {
            objSvcPre = new ServicePresenter();
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_MODIFYDATA_ERROR) + " : " + ex.Message.ToString();

        }
    }

    /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
    /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */
    DataTable dtSeries;
    /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */
    /// <summary> 
    /// To display Tablular list data
    /// </summary>
    ///       
    public void DisplayTableData()
    {
        try
        {
            //Create the Mobile Presenter
            objMobPresenter = new MobilePresenter();
            DataSet ds = new DataSet();
            //Call the Mobile Presenter  
            ds = objMobPresenter.GetTableData(alIocnList, Node, Name);
            /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
            /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */
            dtSeries = ds.Tables[0];
            /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */
            dtByDays = ds.Tables[1];
            /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
            /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */
            // commented for CR
            //changeColNames(dtByDays);
            /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */

            modifyData(dtByDays);

            if (dtByDays.Rows.Count > 0)
            {
                dFirstDate = DateTime.Parse(dtByDays.Rows[0][DATE].ToString());
            }

            DataTable dtFiltered;

            if (Filter)
            {
                dtFiltered = dtByDays.Select("" + DATE + "='" + dFirstDate.AddDays(1) + "' or " + DATE + "='" + dFirstDate.AddDays(2) + "'").CopyToDataTable();
            }
            else
            {
                dtFiltered = dtByDays;
            }


            if (dtFiltered.Rows.Count > 0)
            {
                Repeater1.DataSource = dtFiltered;
                Repeater1.DataBind();
            }
            /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - Begin */
            /* 2.2	If we add a new series in service configuration under mobile section e.g. relative humidity, it should be displayed on mobile. */
            // commented for CR
            //DataTable dtSeries = ds.Tables[0];
            //foreach (DataRow dr in dtSeries.Rows)
            //{
            //    if (dr["trnsTag"].ToString().ToLower().Contains("temp"))
            //    {
            //        if (dr["trnsTag"].ToString().ToLower().Contains("max"))
            //            Max = getTranslatedText(dr["trnsTag"].ToString());
            //        else
            //            Min = getTranslatedText(dr["trnsTag"].ToString());
            //    }
            //    if (dr["trnsTag"].ToString().Contains("_wind_"))
            //        Wind = getTranslatedText(dr["trnsTag"].ToString());
            //    if (dr["trnsTag"].ToString().Contains(TRNSTAG_RAIN))
            //        Rain = getTranslatedText(dr["trnsTag"].ToString());
            //}
            /* Agricast CR - R2 - Mobile site-Login Page and service configuration changes - End */
        }
        catch (Exception ex)
        {
            objSvcPre = new ServicePresenter();
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_DISPLAYTABLE_ERROR) + " : " + ex.Message.ToString();

        }

    }


    #endregion

    #endregion

    #region Properties
    public bool Filter
    {
        get;
        set;
    }
    #endregion


}
#endregion