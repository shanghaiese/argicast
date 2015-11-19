using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syngenta.Agricast.Modals;
using Syngenta.AgriCast.Tables.View;
using Syngenta.AgriCast.Tables.Presenter;
using System.Data;
using System.Globalization;
using Syngenta.AgriCast.Common.DTO;
using Syngenta.AgriCast.Common.Service;
using System.Text;
using System.Collections; 
using System.Drawing;
using System.IO;
using System.Configuration;
using System.Web.UI.HtmlControls;
using Syngenta.AgriCast.Common.Presenter;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common;


public partial class Mobile_UserControls_SprayWindowImg : UserControl, ITable, IRuleSets
{
    #region Declaration
    TablePresenter objTblPre;
    string DEFAULTMODULE = "MobileForecast";
    const string SERVICEPAGENAME = "servicePage";
    const string COLIMAGEURL = "imageurl";
    const string COLDATE = "date";

    string AllignDetails = string.Empty;

    DataSet dsTableSeries;
    PaletteMap objPm;
    DataTable dtSeriesDetails;
    DataTable dtPageNodes;
    List<string[]> al;
    DataTable dtTransposed;
    string CultureCode;
    ServiceInfo objSvcInfo;
    DataPointInfo objDataPointInfo;
    ServiceHandler objServiceHandler = new ServiceHandler();
    CommonUtil objCommonUtil = new CommonUtil();
    ServicePresenter objSvcPre = new ServicePresenter();

    int step = 1;
    int NumOfDays;
    GridView gvDynamic;
    CultureInfo VariantCulture;
    DataTable dtModalInput = new DataTable();
    DataTable dtOutput;
    DataTable dtByDaysLoc = null;
    DataTable dtSprayWindow = new DataTable();
    #endregion


    #region Page_Load
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {//Initialize the location and service objects
            if (Session == null || Session["serviceInfo"] == null)
            {
                objSvcInfo = new ServiceInfo();
            }
            else
            {
                objSvcInfo = (ServiceInfo)Session["serviceInfo"];
            }
            //add columns to the datatable
            dtSprayWindow.Columns.Add(COLDATE);
            dtSprayWindow.Columns.Add(COLIMAGEURL);
            //Get the NodeList
            List<string[]> objList = objServiceHandler.getNodeList(SERVICEPAGENAME);

            for (int i = 0; i < objList.Count; i++)
            {
                if (objList[i].Contains("tblDaysRows"))
                {
                    //Load spray window user control
                    Name = objList[i][1].ToString();
                    Node = objList[i][0].ToString();
                    LoadTables();
                }
                else if (objList[i].Contains("legend"))
                {
                    //Load spray window user control
                    Name = objList[i][1].ToString();
                    Node = objList[i][0].ToString();
                    liSprayLegend.InnerHtml = getTransText("Spray Legend");
                    DisplayLegend(Node, Name);
                }
            }
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_LOAD_FAILURE) + " : " + ex.Message.ToString();
            return;
        }
    }
    #endregion

    #region LoadTables

    void LoadTables()
    {
        try
        {
            objTblPre = new TablePresenter(this, Name);
            objTblPre.getCultureCode();
            VariantCulture = new CultureInfo(strCulCode);
            objTblPre.ReadAllignment(Node);
            string datasource = "";
            string aggregation = "";
            int start;
            int end;
            string ruleset = "";
            string pallette = "";
            string Nodename = "";
            if (AllignDetails.ToString() != "")
            {
                numOfDays = Convert.ToInt32(AllignDetails.Split(',')[1]);
                datasource = AllignDetails.Split(',')[2];
                aggregation = AllignDetails.Split(',')[5];
                start = Convert.ToInt32(AllignDetails.Split(',')[3]);
                end = Convert.ToInt32(AllignDetails.Split(',')[4]);
                ruleset = AllignDetails.Split(',')[6];
                pallette = AllignDetails.Split(',')[7];
                Nodename = AllignDetails.Split(',')[8];
            }
            if (ruleset == string.Empty || ruleset == "")
            {
                //DisplayTables(Name, Node, aggregation, datasource, start, end);
            }
            else
            {
                //DisplayRulesetTables(Name, Node, aggregation, datasource, start, end, ruleset, pallette, Nodename);
                //objTblPre.GetCompleteRuleset(Name, Node, aggregation, datasource, start, end, ruleset, pallette, Nodename, this);
                //objTblPre.GenerateTransposedtblDaysRows(DtOutput, "value", aggregation, true, Step);
                /*Unit Implementation in Web Services - Begin*/
                DataSet ds = objTblPre.getTableDataForService(Node, Name, this, objSvcInfo.ServiceName, objSvcInfo.Module, objSvcInfo.Culture,objSvcInfo.Unit);
                /*Unit Implementation in Web Services - End*/
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow drDate = ds.Tables[0].Rows[0];
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        DataRow drow = dtSprayWindow.NewRow();
                        //if (Convert.ToDateTime(dr["Date"]).ToShortDateString() == Date.ToShortDateString())
                        //{
                        drDate = dr;
                        //        break;
                        //    }
                        //}
                        StringBuilder url = new StringBuilder();
                        url.Append("../../gradient.ashx?width=240&height=40");
                        url.Append("&text=");
                        url.Append("0h,,,,12h,,,,18h,,,,23h");
                        url.Append("&colours=");

                        foreach (DataColumn dc in drDate.Table.Columns)
                        {
                            if (dc.ColumnName.ToLower().Contains("color"))
                            {
                                url.Append(Server.UrlEncode(drDate[dc].ToString() != "" ? drDate[dc].ToString() : "#FFFFFF"));
                                url.Append(Server.UrlEncode(","));
                            }
                        }
                        drow[COLDATE] = dr[COLDATE];
                        drow[COLIMAGEURL] = url.ToString().Substring(0, url.Length - 3);
                        dtSprayWindow.Rows.Add(drow);
                        dtSprayWindow.AcceptChanges();
                    }
                }
            }

            if (dtSprayWindow != null && dtSprayWindow.Rows.Count > 0)
            {
                Repeater1.DataSource = dtSprayWindow;
                Repeater1.DataBind();
            }
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_LOAD_FAILURE) + " : " + ex.Message.ToString();
            return;
        }
    }
    #endregion


    public void DisplayLegend(string node, string name)
    {
        try
        {
            string TransString = string.Empty;
            //GetLegenddetails(node, name);
            //load only if station is selected
            //if (objDataPointInfo != null && (objDataPointInfo.stationLatitude != 0.0 || objDataPointInfo.stationLongitude != 0.0))
            {
                Label lblLegend = new Label();
                TransString = "";
                lblLegend.Text = TransString;
                lblLegend.CssClass = "label250";
                lblLegend.ID = "lbl_" + name;
                StringBuilder sbBody = new StringBuilder();
                StringBuilder sb1;
                StringBuilder sb;

                ServiceHandler serviceHandlerObj = new ServiceHandler();
                DataTable dtLegenddetails = serviceHandlerObj.GetLegendData(name);
                string legendPath = dtLegenddetails.Rows[0][2].ToString().ToString();
                using (StreamReader sr = new StreamReader(HttpRuntime.AppDomainAppPath + @"\Legends\" + legendPath))
                {
                    String line;
                    // Read and display lines from the file until the end of  the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {

                        sbBody.Append(line);

                    }
                    sb1 = new StringBuilder(sbBody.ToString().Substring(sbBody.ToString().IndexOf("<div"), (sbBody.ToString().IndexOf("</div>") - sbBody.ToString().IndexOf("<div") + 6)));

                    string strLegend = sb1.ToString();
                    //get delimiters from web.config
                    string startDelimiter = ConfigurationManager.AppSettings["startDelimiter"] != null && ConfigurationManager.AppSettings["startDelimiter"] != string.Empty ? ConfigurationManager.AppSettings["startDelimiter"] : "{";
                    string endDelimiter = ConfigurationManager.AppSettings["endDelimiter"] != null && ConfigurationManager.AppSettings["endDelimiter"] != string.Empty ? ConfigurationManager.AppSettings["endDelimiter"] : "}";
                    while (strLegend.IndexOf(endDelimiter) != -1)
                    {
                        int start = strLegend.IndexOf(startDelimiter);
                        int end = strLegend.IndexOf(endDelimiter);

                        string text = strLegend.Substring(start, end - start + startDelimiter.Count());
                        if (text != string.Empty)
                        {
                            TransString = getTransText(text.Substring(startDelimiter.Count(), text.Length - (endDelimiter.Count() + startDelimiter.Count())));
                            strLegend = strLegend.Remove(start) + TransString + strLegend.Substring(end + endDelimiter.Count());
                        }

                    }
                    sb = new StringBuilder(strLegend);
                }

                HtmlGenericControl div1 = new HtmlGenericControl("div");
                div1.ID = "divLegend_" + name;
                LiteralControl literal = new LiteralControl(sb.ToString());
                literal.ID = "legend_" + name;
                //System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                //img.ImageUrl = "~/Images/boxminus.gif";
                //img.ID = "img1_" + name;
                //img.ClientIDMode = ClientIDMode.Static;
                //CentrePlaceHolder.Controls.Add(img);
                CentrePlaceHolder.Controls.Add(lblLegend);
                div1.Controls.Add(literal);
                CentrePlaceHolder.Controls.Add(div1);
            }
        }
        catch (Exception ex)
        {
            objSvcPre = new ServicePresenter();
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_DISPLAYLEGEND_ERROR) + " : " + ex.Message.ToString();

        }
    }

    public string getTransText(string key)
    {
        string TransString = string.Empty;
        try
        {
           
            TransString = objCommonUtil.getTransText(key);
           
        }
        catch (Exception ex)
        {
            AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
            AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
            HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.MOB_TRANS_ERROR) + " : " + ex.Message.ToString();

        }
        return TransString;
    }


    # region IRuleset Members

    public DateTime SunRise
    {
        get
        {
            objDataPointInfo = DataPointInfo.getDataPointObject;
            return objDataPointInfo.SunRise;
        }
        set
        {
            //throw new NotImplementedException();
        }
    }

    public DateTime SunSet
    {
        get
        {
            objDataPointInfo = DataPointInfo.getDataPointObject;
            return objDataPointInfo.SunSet;
        }
        set
        {
            //throw new NotImplementedException();
        }
    }

    public DataTable DtInput
    {
        get
        {
            return dtModalInput;
        }
        set
        {
            dtModalInput = value; ;
        }
    }

    public DataTable DtOutput
    {

        get
        {
            if (dtOutput == null)
            {
                dtOutput = new DataTable();
                dtOutput.Columns.Add("day");
                dtOutput.Columns.Add("Hour");
                dtOutput.Columns.Add("ColorCode");
                dtOutput.Columns.Add("value");
                dtOutput.Columns.Add("restrictions");
            }
            return dtOutput;

        }
        set
        {
            dtOutput = value;
        }
    }

    public DateTime StartDate
    {
        get;
        set;
    }

    public DateTime EndDate
    {
        get;
        set;

    }
    #endregion

    #region ITable Members
    public string strAllignDetails
    {
        get
        {
            return AllignDetails;
        }
        set
        {
            AllignDetails = value;
        }
    }


    public DataSet dsSeriesData
    {
        get
        {
            return dsTableSeries;
        }
        set
        {
            dsTableSeries = value;
        }
    }


    public PaletteMap pmPallette
    {
        get
        {
            return objPm;
        }
        set
        {
            objPm = value;
        }
    }



    public List<string[]> alSeriesLegend
    {
        get
        {
            return al;
        }
        set
        {
            al = value;
        }
    }

    public DataTable dtPage
    {
        get
        {
            return dtPageNodes;
        }
        set
        {
            dtPageNodes = value;
        }
    }


    public string strCulCode
    {
        get
        {
            return CultureCode;
        }
        set
        {
            CultureCode = value;
        }
    }

    public ArrayList alSeries
    {
        get;
        set;
    }


    public DataTable dtSeries
    {
        get
        {
            return dtSeriesDetails;
        }
        set
        {
            dtSeriesDetails = value;
        }

    }


    public string Node
    {
        get;
        set;
    }

    public string Name
    {
        get;
        set;
    }


    public List<string[]> ExcelList
    {
        get;
        set;
    }

    public int Step
    {
        get
        {
            return step;
        }
        set
        {
            step = value;
        }
    }


    public int numOfDays
    {
        get
        {
            return NumOfDays;
        }
        set
        {
            NumOfDays = value;
        }
    }
    public DataTable dt
    {
        get
        {
            return dtTransposed;
        }
        set
        {
            dtTransposed = value;
            //gvDynamic.DataSource = dtTransposed;
        }
    }




    public string RuleSetColor
    {
        get;
        set;
    }

    public Color FontColor
    {
        get;
        set;
    }
    public DataTable dtTableSeries
    {
        get;
        set;
    }

    public DataTable dtTableLegends
    {
        get;
        set;
    }
    public DataTable dtByDays
    {
        get;
        set;
    }

    #endregion

    public DateTime Date
    {
        get;
        set;
    }
}