using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Syngenta.AgriCast.Tables.Presenter;
using System.Collections;
using System.Drawing;
using Syngenta.AgriCast.Tables.View;
using System.IO;
using System.Text;
using Syngenta.AgriCast.Common.Presenter;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Syngenta.Agricast.Modals;
using Syngenta.AgriCast.Common.DTO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Configuration;
using Syngenta.AgriCast.ExceptionLogger;
using Syngenta.AgriCast.Common.Service;
using Syngenta.AgriCast.Common;

namespace Syngenta.AgriCast.Tables
{
    public partial class Tables : System.Web.UI.UserControl, ITable, IRuleSets
    {

        #region Declarations

        TablePresenter objTblPre;
        string AllignDetails;
        DataSet dsTableSeries;
        PaletteMap objPm;
        DataTable dtSeriesDetails;
        DataTable dtPageNodes;
        List<string[]> al;
        DataTable dtTransposed;
        string CultureCode;
        Controls common = new Controls();
        int step = 1;
        int NumOfDays;
        GridView gvDynamic;
        CultureInfo VariantCulture;
        DataTable dtModalInput = new DataTable();
        DataTable dtOutput;
        DataPointInfo objDataPointInfo;
        LocationInfo objLocInfo;
        ServicePresenter objSvcPre = new ServicePresenter();
        CommonUtil objCommonUtil = new CommonUtil();

        //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - start
        // A delegate type for hooking up change notifications.
        public delegate void SeriesGridViewDataBindEventHandler(object sender, EventArgs e);
        //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - end
        #endregion

        /* SOCB - Alignment Issue - Jerrey - SOCB*/
        private int imageWidth = 0;
        /* EOCB - Alignment Issue - Jerrey - EOCB*/

        #region Constants
        const string BOXMINUS = "~/Images/boxminus.gif";
        const string BOXPLUS = "~/Images/boxplus.gif";
        const string TBLDAYSROWS = "tblDaysRows";
        const string TBLSERIESROWS = "tblSeriesRows";
        const string TABLES = "tables";
        const string TABLESGAP = "tablesGap";
        const string LABEL250 = "label250";
        const string FIRSTCOLNAME = " ";
        const string THREEHOURLY = "3hourly";
        const string SIXHOURLY = "6hourly";
        const string EIGHTHOURLY = "8hourly";
        const string TWELVEHOURLY = "12hourly";
        const string HOURLY = "hourly";
        const string LOCATION_TABLE_NAME = "Location Details";
        const string RATE_IT = "Rate it";
        const string SMALL_FONT = "smallFont";
        #endregion

        # region Events

        //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - start
        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event SeriesGridViewDataBindEventHandler SeriesGridViewDataBind;
        //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - end

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private void loadTables()
        {
            if (HttpContext.Current != null)
            {
                objLocInfo = LocationInfo.getLocationInfoObj;

            }
            else
            {

                objLocInfo = new LocationInfo();
            }
            if (Name == "excel")
            {
                DataSet ds = new DataSet();



                foreach (string[] node in ExcelList)
                {
                    objTblPre = new TablePresenter(this, node[1].ToString());
                    //objTblPre.ReadAllignment(Node);
                    //numOfDays = Convert.ToInt32(AllignDetails.Split(',')[1]);
                    //string datasourceExcel = AllignDetails.Split(',')[2];
                    //string aggregationExcel = AllignDetails.Split(',')[5];
                    //int startExcel = Convert.ToInt32(AllignDetails.Split(',')[3]);
                    //int endExcel = Convert.ToInt32(AllignDetails.Split(',')[4]);
                    //string rulesetExcel = AllignDetails.Split(',')[6];
                    //string palletteExcel = AllignDetails.Split(',')[7];
                    //string NodenameExcel = AllignDetails.Split(',')[8];
                    //if (rulesetExcel != string.Empty || rulesetExcel != "")
                    //{


                    //}
                    DataTable dt = objTblPre.getTableDataForExcelExport(node[0].ToString(), node[1].ToString(), this);
                    ds.Tables.Add(dt);

                }

                objSvcPre.ExportToExcel(ds);

            }
            else
            {

                ///* SOCB - Alignment Issue - Jerrey - SOCB*/
                //objTblPre = new TablePresenter(this, "Fiveinone");
                //imageWidth = objTblPre.ReadChartWidth("chart");
                ///* EOCB - Alignment Issue - Jerrey - EOCB*/

                objTblPre = new TablePresenter(this, Name);
                objTblPre.getCultureCode();
                VariantCulture = new CultureInfo(strCulCode);
                objTblPre.ReadAllignment(Node);
                numOfDays = Convert.ToInt32(AllignDetails.Split(',')[1]);
                string datasource = AllignDetails.Split(',')[2];
                string aggregation = AllignDetails.Split(',')[5];
                int start = Convert.ToInt32(AllignDetails.Split(',')[3]);
                int end = Convert.ToInt32(AllignDetails.Split(',')[4]);
                //To handle current date
                //end = (end > 0 ? end - 1 : end + 1);
                string ruleset = AllignDetails.Split(',')[6];
                string pallette = AllignDetails.Split(',')[7];
                string Nodename = AllignDetails.Split(',')[8];
                string feedback = AllignDetails.Split(',')[9];
                string collapsible = AllignDetails.Split(',')[10];

                /*IM01277709 - change in spray window. begin*/
                //if displayAMPM is specified , then the length will be 12
                string showAMPM = string.Empty;
                if (AllignDetails.Split(',').Length == 12)
                    showAMPM = AllignDetails.Split(',')[11];


                if (ruleset == string.Empty || ruleset == "")
                {
                    DisplayTables(Name, Node, aggregation, datasource, start, end, feedback, collapsible, showAMPM);
                }
                else
                {
                    DisplayRulesetTables(Name, Node, aggregation, datasource, start, end, ruleset, pallette, Nodename, feedback, collapsible, showAMPM);
                }
                /*IM01277709 - change in spray window. end*/
            }
        }
        protected void Page_prerender(object sender, EventArgs args)
        {
            try
            {
                objLocInfo = (LocationInfo)HttpContext.Current.Session["objLocationInfo"];
                //if (objLocInfo.DataPointInfo != null && !IsEmpty<DataPointInfo>(objLocInfo.DataPointInfo))
                if (objLocInfo.DataPointInfo != null && objLocInfo.DataPointInfo.stationLatitude != 0 && objLocInfo.DataPointInfo.stationLongitude != 0 && objLocInfo.DataPointInfo.stationName != "")
                    loadTables();
            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.TAB_LOAD_FAILURE) + ":" + ex.Message.ToString();
            }
        }
        void gvDynamic_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridView objGridView = (GridView)sender;
                GridViewRow objgridviewrow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
                TableCell objtablecell = new TableCell();

                AddMergedCells(objgridviewrow, objtablecell, 0, "", Color.FromArgb(239, 237, 234).Name);

                /* SOCB - Alignment Issue - Jerrey - SOCB*/
                var innerChartWidth = 770;
                var cellWidth = innerChartWidth / numOfDays;
                /* EOCB - Alignment Issue - Jerrey - EOCB*/

                for (int i = 0; i < numOfDays; i++)
                {
                    /* SOCB - Alignment Issue - Jerrey - SOCB*/
                    var colWidth = 0;
                    if (i < numOfDays - 1)
                        colWidth = cellWidth;
                    else
                        colWidth = innerChartWidth - cellWidth * (numOfDays - 1);
                    /* EOCB - Alignment Issue - Jerrey - EOCB*/

                    AddMergedCells(objgridviewrow,
                        objtablecell,
                        24 / Step,
                        VariantCulture.DateTimeFormat.GetDayName(DateTime.Today.AddDays(i).DayOfWeek).ToString(),
                        Color.FromArgb(239, 237, 234).Name,
                        colWidth);
                }


                objGridView.Controls[0].Controls.AddAt(0, objgridviewrow);

            }
        }
        protected void gvDynamic_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            VariantCulture = new CultureInfo(strCulCode);
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // set formatting for the category cell 
                TableCell cell = e.Row.Cells[0];

            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                if (step == 24)
                {
                    e.Row.CssClass = "hide";
                }
                else
                {
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        TableCell cell = e.Row.Cells[i];

                        if (i > 0)
                        {
                            //In case of header with date + day, just show the day in the chosen language
                            if (cell.Text.Contains(',') && objTblPre != null)
                            {
                                cell.Text = (cell.Text.Substring(cell.Text.LastIndexOf(',') + 1)).ToString(VariantCulture);
                                cell.CssClass = "tableRow";
                            }
                            else
                            {
                                cell.CssClass = "tableRowHourly";
                            }

                        }
                    }
                }
            }
        }
        #endregion


        #region Methods
        /*IM01277709 - change in spray window. begin*/
        //added new parameter showAMPM
        /*IM01277709 - change in spray window. end*/
        public void DisplayRulesetTables(string name, string allign, string aggregation, string datasource, int start, int end, string ruleset, string pallette, string Nodename, string feedback, string collapsible, string showAMPM)
        {
            try
            {
                string strDateFormat = "";//Default format is  month/day

                objTblPre.GetCompleteRuleset(name, allign, aggregation, datasource, start, end, ruleset, pallette, Nodename, this);



                if (allign.ToLower() == TBLDAYSROWS.ToLower())
                {
                    //Ruleset - tbldaysrows 
                    if (aggregation.ToLower() == THREEHOURLY)
                    {
                        step = 3;
                    }
                    else if (aggregation.ToLower() == SIXHOURLY)
                    {
                        step = 6;
                    }
                    else if (aggregation.ToLower() == EIGHTHOURLY)
                    {
                        step = 8;
                    }
                    else if (aggregation.ToLower() == TWELVEHOURLY)
                    {
                        step = 12;
                    }
                    else if (aggregation.ToLower() == HOURLY)
                    {
                        step = 1;
                    }
                    else
                    {
                        step = 24;
                    }

                    gvDynamic = new GridView();
                    gvDynamic.RowDataBound += new GridViewRowEventHandler(gvDynamic_RowDataBound);
                    /*IM01277709 - change in spray window. begin*/
                    //objTblPre.GenerateTransposedtblDaysRows(DtOutput, "value", aggregation, true, Step);
                    objTblPre.GenerateTransposedtblDaysRows(DtOutput, "value", aggregation, true, Step, showAMPM);
                    /*IM01277709 - change in spray window. end*/
                    gvDynamic.CssClass = TABLES;
                    gvDynamic.ID = "gv_" + "value" + name;
                    gvDynamic.DataBind();
                    objTblPre.createPallette(pallette);

                    strDateFormat = VariantCulture.DateTimeFormat.ShortDatePattern;

                    strDateFormat = "dddd " + strDateFormat.Substring(0, strDateFormat.Length - 5);


                    #region CommentedCode
                    ////check the culture code for "US" and change the date format
                    //if (VariantCulture.ToString().Equals("en-US", StringComparison.InvariantCultureIgnoreCase))
                    //    strDateFormat = "dddd dd/M";//day/month format for US
                    //else
                    //    strDateFormat = "dddd M/dd";//month/day format for Non US

                    //for (int k = 0; k < gvDynamic.Rows.Count; k++)
                    //{


                    //    for (int j = 1; j < dt.Columns.Count; j++)
                    //    {
                    //        //this is used to calculate the offset to pick the required row from 
                    //        //output datatable dt
                    //        iCount++;


                    //        //if (j > 0)
                    //        //{
                    //           //Get the Color from Output DataTable
                    //            var queryColor = from row in dtOutput.AsEnumerable()
                    //                             where row.Field<string>("Day").Trim() == DateTime.ParseExact(HttpUtility.HtmlDecode(gvDynamic.Rows[k].Cells[0].Text), strDateFormat, VariantCulture).ToShortDateString()
                    //                                         && row.Field<string>("Hour").ToString(VariantCulture) == dt.Columns[j].ColumnName
                    //                             select row.Field<string>("ColorCode");
                    //            if (queryColor.ToList().Count > 0)
                    //            {
                    //                double val;
                    //                bool flag = double.TryParse(queryColor.ToList()[0], out val) ? true : false;
                    //                if (flag)
                    //                {

                    //                    string color = objPm.getColor(val, name);
                    //                    System.Drawing.ColorConverter ccv = new System.Drawing.ColorConverter();
                    //                    gvDynamic.Rows[k].Cells[j].BackColor = (Color)ccv.ConvertFromString(color);
                    //                    objTblPre.GetContrastColor((Color)gvDynamic.Rows[k].Cells[j].BackColor);
                    //                    gvDynamic.Rows[k].Cells[j].ForeColor = FontColor;
                    //                }


                    //                var queryRestriction = from row in dtOutput.AsEnumerable()
                    //                                       where row.Field<string>("Day").Trim() == DateTime.ParseExact(HttpUtility.HtmlDecode(gvDynamic.Rows[k].Cells[0].Text), strDateFormat, VariantCulture).ToShortDateString()
                    //                                && row.Field<string>("Hour").ToString(VariantCulture) == dt.Columns[j].ColumnName
                    //                                       select row.Field<string>("restrictions");


                    //                if (queryRestriction.ToList().Count > 0 && queryRestriction.ToList()[0] != null)
                    //                {
                    //                    string[] ToolTips = queryRestriction.ToList()[0].Split(',');
                    //                    string ToolTip = "";
                    //                    foreach (string restriction in ToolTips)
                    //                    {
                    //                        ToolTip = ToolTip + GetRestriction(restriction) + ",";
                    //                    }
                    //                    gvDynamic.Rows[k].Cells[j].ToolTip = ToolTip.Remove(ToolTip.LastIndexOf(','));
                    //                }
                    //            }
                    //        //}

                    //    }//end of inner for loop - dt



                    //}
                    #endregion CommentedCode

                    int iRowIndex = 0;
                    int iColumnIndex = 0;
                    int ihour = 0;
                    bool isNextRow = false;
                    //loop thru the output datatable
                    for (int i = 0; i < DtOutput.Rows.Count; i++)
                    {
                        //get the hour of each row
                        ihour = int.Parse(DtOutput.Rows[i]["hour"].ToString());

                        //calcuate Rowindex
                        if (ihour == 23)
                            isNextRow = true;

                        //Calculate Column Index
                        if (i < 24)
                        {
                            iColumnIndex = i;
                        }
                        else
                        {
                            iColumnIndex = ((i - 24) % 24);
                        }


                        //Get the Color from Output DataTable
                        double val;
                        bool flag = double.TryParse(DtOutput.Rows[i]["ColorCode"].ToString(), out val) ? true : false;
                        if (flag)
                        {

                            string color = objPm.getColor(val, name);
                            System.Drawing.ColorConverter ccv = new System.Drawing.ColorConverter();
                            gvDynamic.Rows[iRowIndex].Cells[iColumnIndex + 1].BackColor = (Color)ccv.ConvertFromString(color);
                            objTblPre.GetContrastColor((Color)gvDynamic.Rows[iRowIndex].Cells[iColumnIndex + 1].BackColor);
                            gvDynamic.Rows[iRowIndex].Cells[iColumnIndex + 1].ForeColor = FontColor;
                        }

                        //Get the Restrictions
                        string[] ToolTips = null;
                        if (!string.IsNullOrEmpty(DtOutput.Rows[i]["restrictions"].ToString()))
                        {
                            ToolTips = DtOutput.Rows[i]["restrictions"].ToString().Split(',');

                            string ToolTip = "";
                            foreach (string restriction in ToolTips)
                            {
                                ToolTip = ToolTip + GetRestriction(restriction) + ",";
                            }
                            gvDynamic.Rows[iRowIndex].Cells[iColumnIndex + 1].ToolTip = ToolTip.Remove(ToolTip.LastIndexOf(','));
                        }
                        //change the row index incase isNextRow is true
                        if (isNextRow)
                        {
                            iRowIndex++;
                            isNextRow = false;
                        }
                    }

                    Label lbltext = new Label();
                    lbltext.Text = objTblPre.getTranslatedText(Nodename, strCulCode);
                    lbltext.CssClass = LABEL250;

                    HtmlGenericControl divOuter = new HtmlGenericControl("div");
                    HtmlGenericControl divOuterLeft = new HtmlGenericControl("div");
                    if (bool.Parse(feedback))
                    {
                        Label lblRating = new Label();
                        lblRating.Text = "Rate this section";
                        lblRating.CssClass = "label110";
                        HtmlGenericControl divRating = new HtmlGenericControl("div");
                        divRating.ID = ("rating_" + lbltext.Text).Replace(" ", "_");
                        divRating.Attributes.Add("class", "RateIt");
                        divRating.Attributes.Add("data-accesskey", "rate");
                        divOuter.Attributes.Add("class", "tabRight");
                        divOuterLeft.Attributes.Add("class", "Left");
                        divOuter.ID = "ratingContainer_" + gvDynamic.ID;
                        divOuterLeft.ID = "leftLabel_" + gvDynamic.ID;
                        divOuter.Controls.Add(lblRating);
                        divOuter.Controls.Add(divRating);
                        divRating.ClientIDMode = ClientIDMode.Static;
                        divRating.Attributes.Add("data-rateit-backingfld", "select#range");
                    }

                    HtmlGenericControl div1 = new HtmlGenericControl("div");
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.Attributes.Add("Class", "divOuter");
                    div1.ID = "div_" + gvDynamic.ID;
                    div1.Attributes.Add("class", "DetailsRating");
                    if (bool.Parse(collapsible))
                    {
                        System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                        img.ImageUrl = BOXMINUS;
                        img.ID = "img1_" + gvDynamic.ID;
                        img.ClientIDMode = ClientIDMode.Static;
                        //div1.Controls.Add(img);
                        divOuterLeft.Controls.Add(img);
                    }
                    divOuterLeft.Controls.Add(lbltext);
                    //div1.Controls.Add(lbltext);
                    div1.Controls.Add(divOuterLeft);
                    if (bool.Parse(feedback))
                        div1.Controls.Add(divOuter);
                    div1.Controls.Add(gvDynamic);
                    div.Controls.Add(div1);
                    ph1.Controls.Add(div);

                }
                else if (allign.ToLower() == TBLSERIESROWS.ToLower())
                {

                    gvDynamic = new GridView();
                    
                    //Ruleset - tbldaysrows - aggregated data
                    if (aggregation.ToLower() != HOURLY)
                    {

                        if (aggregation.ToLower() == THREEHOURLY)
                        {
                            step = 3;
                        }
                        else if (aggregation.ToLower() == SIXHOURLY)
                        {
                            step = 6;
                        }
                        else if (aggregation.ToLower() == EIGHTHOURLY)
                        {
                            step = 8;
                        }
                        else if (aggregation.ToLower() == TWELVEHOURLY)
                        {
                            step = 12;
                        }
                        else
                        {
                            step = 24;
                        }
                        objTblPre.GenerateTransposedtblSeriesRows(DtOutput, numOfDays, aggregation, true, step);

                        DataTable rulesetDt = dt.Copy();
                        dt.Clear();
                        DataRow dr = dt.NewRow();
                        for (int i = 1; i < dt.Columns.Count; i++)
                        {
                            dr[0] = objTblPre.getTranslatedText(Nodename, strCulCode);
                            dr[i] = rulesetDt.Rows[2][i];
                        }
                        dt.Rows.Add(dr);

                        gvDynamic.RowCreated += new GridViewRowEventHandler(gvDynamic_RowCreated);
                        gvDynamic.RowDataBound += new GridViewRowEventHandler(gvDynamic_RowDataBound);
                        gvDynamic.ID = "gv_" + DateTime.Today.ToShortDateString();
                        gvDynamic.DataBind();
                        HtmlGenericControl divOuter = new HtmlGenericControl("div");
                        if (bool.Parse(feedback))
                        {
                            Label lblRating = new Label();
                            lblRating.Text = objTblPre.getTranslatedText(RATE_IT, CultureCode);
                            lblRating.CssClass = "label110";
                            HtmlGenericControl divRating = new HtmlGenericControl("div");
                            divRating.ID = "rating_" + name;
                            divRating.Attributes.Add("class", "RateIt");
                            divRating.Attributes.Add("data-accesskey", "rate");
                            divRating.ClientIDMode = ClientIDMode.Static;
                            divRating.Attributes.Add("data-rateit-backingfld", "select#range");

                            divOuter.Attributes.Add("class", "Right");
                            divOuter.ID = "ratingContainer_" + gvDynamic.ID;
                            divOuter.Controls.Add(lblRating);
                            divOuter.Controls.Add(divRating);
                        }
                        objTblPre.createPallette(pallette);
                        for (int i = 0; i < gvDynamic.Rows.Count; i++)
                        {
                            string serieName = gvDynamic.Rows[i].Cells[0].Text.ToString();
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                if (j > 0)
                                {

                                    string queryColor = rulesetDt.Rows[1][j].ToString();
                                    if (queryColor != "")
                                    {
                                        double val;
                                        bool flag = double.TryParse(queryColor, out val) ? true : false;
                                        if (flag)
                                        {

                                            string color = objPm.getColor(val, name);
                                            System.Drawing.ColorConverter ccv = new System.Drawing.ColorConverter();
                                            gvDynamic.Rows[i].Cells[j].BackColor = (Color)ccv.ConvertFromString(color);
                                            objTblPre.GetContrastColor((Color)gvDynamic.Rows[i].Cells[j].BackColor);
                                            gvDynamic.Rows[i].Cells[j].ForeColor = FontColor;
                                        }


                                        string queryRestriction = rulesetDt.Rows[3][j].ToString();
                                        if (queryRestriction != "")
                                        {
                                            string[] ToolTips = queryRestriction.Split(',');
                                            string ToolTip = "";
                                            foreach (string restriction in ToolTips)
                                            {
                                                ToolTip = ToolTip + GetRestriction(restriction) + ",";
                                            }
                                            gvDynamic.Rows[i].Cells[j].ToolTip = ToolTip.Remove(ToolTip.LastIndexOf(','));
                                        }
                                    }
                                }
                            }
                        }
                        HtmlGenericControl div = new HtmlGenericControl("div");
                        div.Attributes.Add("Class", "divOuter");
                        HtmlGenericControl div1 = new HtmlGenericControl("div");
                        div1.ID = "div_" + gvDynamic.ID;
                        /* IM01417008 - INF-GLOB-SPTL2 request - Jerrey - Begin */
                        div1.Attributes.Add("Class", " alignTableToChart ");
                        /* IM01417008 - INF-GLOB-SPTL2 request - Jerrey - Begin */
                        if (bool.Parse(feedback))
                            div.Controls.Add(divOuter);
                        gvDynamic.CssClass = TABLESGAP;

                        /* SOCB - Alignment Issue - Jerrey - SOCB*/
                        /*IM01170843- New Agricast - embeded JS - vew daily table columns width - BEGIN*/
                        //Set the below calculated styles in css class

                        gvDynamic.CssClass += " alignToChart ";
                        gvDynamic.CssClass += string.Format("tblWidth_{0}", numOfDays);

                        /*IM01170843- New Agricast - embeded JS - vew daily table columns width - END*/
                        /* EOCB - Alignment Issue - Jerrey - EOCB*/

                        div.Controls.Add(div1);
                        div1.Controls.Add(gvDynamic);
                        ph1.Controls.Add(div);

                    }

                    else
                    {
                        //Ruleset - tbldaysrows - non aggregated data
                        int k = 0;
                        gvDynamic = new GridView();
                        gvDynamic.RowDataBound += new GridViewRowEventHandler(gvDynamic_RowDataBound);
                        Label lbldate = new Label();
                        if (start > end)
                        {
                            int tmp = start;
                            start = end;
                            end = tmp;
                        }

                        lbldate.Text = objTblPre.getTranslatedText(Nodename, strCulCode); //(DateTime.Today.AddDays(start).AddDays((double)k)).ToString("D", VariantCulture);
                        lbldate.CssClass = LABEL250;
                        HtmlGenericControl divOuter = new HtmlGenericControl("div");
                        if (bool.Parse(feedback))
                        {
                            Label lblRating = new Label();
                            lblRating.Text = objTblPre.getTranslatedText(RATE_IT, CultureCode);
                            lblRating.CssClass = "label110";
                            HtmlGenericControl divRating = new HtmlGenericControl("div");
                            divRating.ID = "rating_" + (DateTime.Today.AddDays(start).AddDays((double)k)).ToShortDateString();
                            divRating.Attributes.Add("class", "RateIt");
                            divRating.Attributes.Add("data-accesskey", "rate");
                            divRating.ClientIDMode = ClientIDMode.Static;
                            divRating.Attributes.Add("data-rateit-backingfld", "select#range");

                            divOuter.Attributes.Add("class", "Right");
                            divOuter.Controls.Add(lblRating);
                            divOuter.Controls.Add(divRating);
                        }
                        gvDynamic.ID = "gv_" + DateTime.Today.AddDays(start).AddDays((double)k).DayOfWeek + DateTime.Today.AddDays(start).AddDays((double)k).Day;
                        gvDynamic.ClientIDMode = ClientIDMode.Static;
                        objTblPre.GenerateTransposedtblSeriesRows(DtOutput, k, aggregation, true, step);
                        DataTable rulesetDt = dt.Copy();
                        dt.Clear();
                        DataRow dr = dt.NewRow();
                        for (int i = 1; i <= 23; i++)
                        {
                            dr[0] = objTblPre.getTranslatedText(Nodename, strCulCode);
                            dr[i] = rulesetDt.Rows[2][i];
                        }
                        dt.Rows.Add(dr);
                        gvDynamic.DataBind();

                        objTblPre.createPallette(pallette);

                        for (int i = 0; i < gvDynamic.Rows.Count; i++)
                        {
                            string serieName = gvDynamic.Rows[i].Cells[0].Text.ToString();
                            for (int j = 0; j <= 24; j++)
                            {
                                if (j > 0)
                                {

                                    string queryColor = rulesetDt.Rows[1][j].ToString();
                                    if (queryColor != "")
                                    {
                                        double val;
                                        bool flag = double.TryParse(queryColor, out val) ? true : false;
                                        if (flag)
                                        {

                                            string color = objPm.getColor(val, name);
                                            System.Drawing.ColorConverter ccv = new System.Drawing.ColorConverter();
                                            gvDynamic.Rows[i].Cells[j].BackColor = (Color)ccv.ConvertFromString(color);
                                            objTblPre.GetContrastColor((Color)gvDynamic.Rows[i].Cells[j].BackColor);
                                            gvDynamic.Rows[i].Cells[j].ForeColor = FontColor;
                                        }


                                        string queryRestriction = rulesetDt.Rows[3][j].ToString();
                                        if (queryRestriction != "")
                                        {
                                            string[] ToolTips = queryRestriction.Split(',');
                                            string ToolTip = "";
                                            foreach (string restriction in ToolTips)
                                            {
                                                ToolTip = ToolTip + GetRestriction(restriction) + ",";
                                            }
                                            gvDynamic.Rows[i].Cells[j].ToolTip = ToolTip.Remove(ToolTip.LastIndexOf(','));
                                        }
                                    }
                                }
                            }
                        }


                        HtmlGenericControl div1 = new HtmlGenericControl("div");
                        HtmlGenericControl div = new HtmlGenericControl("div");
                        div.Attributes.Add("Class", "divOuter");
                        div1.ID = "div_" + gvDynamic.ID;

                        gvDynamic.CssClass = TABLES;

                        if (gvDynamic.Rows.Count > 0)
                        {
                            if (bool.Parse(collapsible))
                            {
                                System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                                img.ImageUrl = BOXMINUS;
                                img.ID = "img1_" + gvDynamic.ID;
                                img.ClientIDMode = ClientIDMode.Static;
                                div1.Controls.Add(img);
                            }
                            div1.Controls.Add(lbldate);
                            if (bool.Parse(feedback))
                                div.Controls.Add(divOuter);
                        }

                        div1.Controls.Add(gvDynamic);
                        div.Controls.Add(div1);
                        ph1.Controls.Add(div);


                    }
                }


                foreach (DataRow dr in dtTableLegends.Rows)
                {

                    Label lblLegend = new Label();
                    lblLegend.Text = objTblPre.getTranslatedText(dr[2].ToString(), CultureCode);
                    lblLegend.CssClass = LABEL250;
                    lblLegend.ID = "lblRulesetL_" + dr[0].ToString();
                    StringBuilder sbBody = new StringBuilder();
                    StringBuilder sb;
                    StringBuilder sb1;
                    string legendPath = dr[1].ToString();

                    using (StreamReader sr = new StreamReader(HttpRuntime.AppDomainAppPath + legendPath))
                    {
                        String line;
                        // Read and display lines from the file until the end of  the file is reached.
                        while ((line = sr.ReadLine()) != null)
                        {

                            sbBody.Append(line);

                        }
                        sb1 = new StringBuilder(sbBody.ToString().Substring(sbBody.ToString().IndexOf("<div"), (sbBody.ToString().IndexOf("</div>") - sbBody.ToString().IndexOf("<div") + 6)));
                        string strLegend = sb1.ToString();
                        string startDelimiter = ConfigurationManager.AppSettings["startDelimiter"] != null && ConfigurationManager.AppSettings["startDelimiter"] != string.Empty ? ConfigurationManager.AppSettings["startDelimiter"] : "{";
                        string endDelimiter = ConfigurationManager.AppSettings["endDelimiter"] != null && ConfigurationManager.AppSettings["endDelimiter"] != string.Empty ? ConfigurationManager.AppSettings["endDelimiter"] : "}";
                        while (strLegend.IndexOf(endDelimiter) != -1)
                        {
                            int startDelit = strLegend.IndexOf(startDelimiter);
                            int endDelit = strLegend.IndexOf(endDelimiter);

                            string text = strLegend.Substring(startDelit, endDelit - startDelit + startDelimiter.Count());
                            if (text != string.Empty)
                            {
                                string translatedText = objTblPre.getTranslatedText(text.Substring(startDelimiter.Count(), text.Length - (endDelimiter.Count() + startDelimiter.Count())), strCulCode);
                                translatedText=translatedText.TrimStart('{').TrimEnd('}');
                                strLegend = strLegend.Remove(startDelit) + translatedText + strLegend.Substring(endDelit + endDelimiter.Count());
                            }

                        }
                        sb = new StringBuilder(strLegend);
                    }


                    LiteralControl literal = new LiteralControl(sb.ToString());
                    literal.ID = "legend_" + dr[0].ToString();

                    HtmlGenericControl div1 = new HtmlGenericControl("div");
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.Attributes.Add("Class", "divOuter");
                    HtmlGenericControl divbind = new HtmlGenericControl("div");
                    HtmlGenericControl divAdditional = new HtmlGenericControl("div");
                    divAdditional.Controls.Add(div1);
                    div1.ID = "divRulesetL_" + dr[0].ToString();
                    divbind.Attributes.Add("Class", "LegendPadding");
                    //div.Attributes.Add("Class", "LegendPadding");
                    if (bool.Parse(collapsible))
                    {
                        System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                        img.ImageUrl = BOXMINUS;
                        img.ID = "img1_" + dr[0].ToString();
                        img.ClientIDMode = ClientIDMode.Static;
                        divbind.Controls.Add(img);
                    }

                    divbind.Controls.Add(lblLegend);
                    div1.Controls.Add(literal);
                    //divbind.Controls.Add(div1);
                    divbind.Controls.Add(divAdditional);
                    div.Controls.Add(divbind);
                    ph1.Controls.Add(div);
                    // phLegend.Controls.Add(div);
                }


            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.TAB_DISPLAY_FAILURE) + ":" + ex.Message.ToString();
            }
        }
        /*IM01277709 - change in spray window. begin*/
        //added new parameter showAMPM
        /*IM01277709 - change in spray window. end*/
        public void DisplayTables(string name, string allign, string aggregation, string datasource, int start, int end, string feedback, string collapsible, string showAMPM)
        {
            try
            {
                objTblPre.CreateTables(name, allign, aggregation, datasource, start, end);

                if (allign.ToLower() == TBLDAYSROWS.ToLower())
                {
                    //history data - tbldaysrows - aggregated + non aggregated data
                    if (aggregation.ToLower() == THREEHOURLY)
                    {
                        step = 3;
                    }
                    else if (aggregation.ToLower() == SIXHOURLY)
                    {
                        step = 6;
                    }
                    else if (aggregation.ToLower() == EIGHTHOURLY)
                    {
                        step = 8;
                    }
                    else if (aggregation.ToLower() == TWELVEHOURLY)
                    {
                        step = 12;
                    }
                    else if (aggregation.ToLower() == HOURLY)
                    {
                        step = 1;
                    }
                    else
                    {
                        step = 24;
                    }

                    foreach (string ser in alSeries)
                    {
                        string seriesPallette = ser.Split(',')[1].ToString();
                        string seriesName = ser.Split(',')[0].ToString();
                        string serie = ser.Split(',')[2].ToString();
                        gvDynamic = new GridView();
                        gvDynamic.RowDataBound += new GridViewRowEventHandler(gvDynamic_RowDataBound);
                        objTblPre.GenerateTransposedtblDaysRows(dtByDays, seriesName, aggregation, false, step,showAMPM);
                        //if aggregation hourly, css applies on grid is ".tables". For any other aggregation ".tablesGap"
                        gvDynamic.CssClass = (aggregation.ToLower() == HOURLY) ? TABLES : TABLESGAP;

                        /* SOCB - Alignment Issue - Jerrey - SOCB*/
                        /*IM01170843- New Agricast - embeded JS - vew daily table columns width - BEGIN*/
                        //Set the below calculated styles in css class
                        if (aggregation.ToLower() != HOURLY)
                        {
                            gvDynamic.CssClass += " alignToChart ";
                            gvDynamic.CssClass += string.Format("tblWidth_{0}", numOfDays);
                        }
                        /*IM01170843- New Agricast - embeded JS - vew daily table columns width - END*/
                        /* EOCB - Alignment Issue - Jerrey - EOCB*/

                        gvDynamic.ID = "gv_" + seriesName;
                        gvDynamic.DataBind();


                        Label lbl = new Label();
                        lbl.Text = objTblPre.getTranslatedText(ser.Split(',')[3].ToString(), CultureCode);//seriesName;
                        lbl.CssClass = LABEL250;
                        Label lblRating = new Label();
                        HtmlGenericControl divOuter = new HtmlGenericControl("div");
                        if (bool.Parse(feedback))
                        {
                            lblRating.Text = objTblPre.getTranslatedText(RATE_IT, CultureCode);
                            lblRating.CssClass = "label110";
                            HtmlGenericControl divRating = new HtmlGenericControl("div");
                            divRating.ID = "rating_" + ser.Split(',')[3].ToString();
                            divRating.Attributes.Add("class", "RateIt");
                            divRating.Attributes.Add("data-accesskey", "rate");
                            divRating.ClientIDMode = ClientIDMode.Static;
                            divRating.Attributes.Add("data-rateit-backingfld", "select#range");
                            divOuter.Attributes.Add("class", "tabRight");
                            divOuter.ID = "ratingContainer_" + gvDynamic.ID;
                            divOuter.Controls.Add(lblRating);
                            divOuter.Controls.Add(divRating);
                        }
                        if (seriesPallette.ToLower() != "none")
                        {
                            seriesPallette = objTblPre.checkUnitPalette(seriesPallette);
                            objTblPre.createPallette(seriesPallette);
                            for (int k = 0; k < gvDynamic.Rows.Count; k++)
                            {
                                for (int j = 0; j < dt.Columns.Count; j++)
                                {
                                    if (j > 0)
                                    {
                                        if (gvDynamic.Rows[k].Cells[j].Text.ToString() != "" && gvDynamic.Rows[k].Cells[j].Text.ToString() != "&nbsp;")
                                        {
                                            double val;
                                            bool flag = double.TryParse(gvDynamic.Rows[k].Cells[j].Text, out val) ? true : false;
                                            if (flag)
                                            {
                                                string color = objPm.getColor(val, seriesName);
                                                System.Drawing.ColorConverter ccv = new System.Drawing.ColorConverter();
                                                gvDynamic.Rows[k].Cells[j].BackColor = (Color)ccv.ConvertFromString(color);
                                                objTblPre.GetContrastColor((Color)gvDynamic.Rows[k].Cells[j].BackColor);
                                                gvDynamic.Rows[k].Cells[j].ForeColor = FontColor;
                                                if (double.Parse(gvDynamic.Rows[k].Cells[j].Text) > 200)
                                                    gvDynamic.Rows[k].Cells[j].CssClass = SMALL_FONT;

                                            }
                                        }
                                    }

                                }

                            }
                        }

                        HtmlGenericControl div1 = new HtmlGenericControl("div");
                        HtmlGenericControl div = new HtmlGenericControl("div");
                        HtmlGenericControl divOuterLeft = new HtmlGenericControl("div");

                        div1.ID = "div_" + gvDynamic.ID;
                        div1.Attributes.Add("class", "DetailsRating");
                        divOuterLeft.ID = "divOuterLeftDetails_" + gvDynamic.ID;
                        divOuterLeft.Attributes.Add("class", "Left");
                        //add boxplus/boxminus images only if collapsible is true in the config
                        if (bool.Parse(collapsible))
                        {
                            System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                            img.ImageUrl = BOXMINUS;
                            img.ID = "img1_" + gvDynamic.ID;
                            img.ClientIDMode = ClientIDMode.Static;

                            //div1.Controls.Add(img);
                            divOuterLeft.Controls.Add(img);
                        }
                        divOuterLeft.Controls.Add(lbl);
                        div1.Controls.Add(divOuterLeft);
                        //div1.Controls.Add(lbl);

                        if (bool.Parse(feedback))
                            div1.Controls.Add(divOuter);
                        div1.Controls.Add(gvDynamic);
                        div.Controls.Add(div1);
                        ph1.Controls.Add(div);

                        int i = 0;
                        while (i < alSeriesLegend.Count)
                        {

                            if ((alSeriesLegend[i][1].ToString() == serie) && (i + 1) < alSeriesLegend.Count)
                            {
                                if (alSeriesLegend[i + 1][0].ToString().ToLower().Contains("legend"))
                                {
                                    DisplayLegend(alSeriesLegend[i + 1][1].ToString(), gvDynamic.ID);
                                    break;

                                }

                            }
                            i++;

                        }
                    }



                }
                else if (allign.ToLower() == TBLSERIESROWS.ToLower())
                {
                    //history data - tblseriesrows - aggregated data
                    gvDynamic = new GridView();
                    if (aggregation.ToLower() != HOURLY)
                    {

                        if (aggregation.ToLower() == THREEHOURLY)
                        {
                            step = 3;
                        }
                        else if (aggregation.ToLower() == SIXHOURLY)
                        {
                            step = 6;
                        }
                        else if (aggregation.ToLower() == EIGHTHOURLY)
                        {
                            step = 8;
                        }
                        else if (aggregation.ToLower() == TWELVEHOURLY)
                        {
                            step = 12;
                        }
                        else
                        {
                            step = 24;
                        }
                        objTblPre.GenerateTransposedtblSeriesRows(dtByDays, numOfDays, aggregation, false, step);
                        gvDynamic.RowCreated += new GridViewRowEventHandler(gvDynamic_RowCreated);
                        gvDynamic.RowDataBound += new GridViewRowEventHandler(gvDynamic_RowDataBound);
                        gvDynamic.ID = "gv_" + DateTime.Today.ToShortDateString();
                        gvDynamic.DataBind();
                        //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - start
                        if (SeriesGridViewDataBind != null)
                        {
                            SeriesGridViewDataBind(gvDynamic, new EventArgs());
                        }
                        //Add for IM01977477:AIS - Modify kecp01fao publication - 20140802 - end
                        HtmlGenericControl divOuter = new HtmlGenericControl("div");
                        if (bool.Parse(feedback))
                        {
                            Label lblRating = new Label();
                            lblRating.Text = objTblPre.getTranslatedText(RATE_IT, CultureCode);
                            lblRating.CssClass = "label110";
                            HtmlGenericControl divRating = new HtmlGenericControl("div");
                            divRating.ID = "rating_" + name;
                            divRating.Attributes.Add("class", "RateIt");
                            divRating.Attributes.Add("data-accesskey", "rate");
                            divRating.ClientIDMode = ClientIDMode.Static;
                            divRating.Attributes.Add("data-rateit-backingfld", "select#range");
                            divOuter.ID = "ratingContainer_" + gvDynamic.ID;
                            divOuter.Attributes.Add("class", "Right");
                            divOuter.Controls.Add(lblRating);
                            divOuter.Controls.Add(divRating);
                        }
                        string seriesPallette = "";
                        for (int i = 0; i < gvDynamic.Rows.Count; i++)
                        {
                            string serieName = gvDynamic.Rows[i].Cells[0].Text.ToString();
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                if (j == 0)
                                {
                                    gvDynamic.Rows[i].Cells[0].Text = objTblPre.getTranslatedText(alSeries[i].ToString().Split(',')[3], CultureCode); //gvDynamic.Rows[i].Cells[j].Text, CultureCode);

                                }

                                if (j > 0)
                                {
                                    foreach (string ser in alSeries)
                                    {
                                        if (ser.Split(',')[0].ToString() == serieName)
                                        {
                                            seriesPallette = ser.Split(',')[1].ToString();
                                            break;
                                        }

                                    }
                                    if (seriesPallette.ToLower() != "none")
                                    {
                                        seriesPallette = objTblPre.checkUnitPalette(seriesPallette);
                                        objTblPre.createPallette(seriesPallette);
                                        if (gvDynamic.Rows[i].Cells[j].Text.ToString() != "" && gvDynamic.Rows[i].Cells[j].Text.ToString() != "&nbsp;")
                                        {
                                            double val;
                                            bool flag = double.TryParse(gvDynamic.Rows[i].Cells[j].Text, out val) ? true : false;
                                            if (flag)
                                            {
                                                string color = objPm.getColor(val, gvDynamic.Rows[i].Cells[0].Text.ToString());
                                                System.Drawing.ColorConverter ccv = new System.Drawing.ColorConverter();
                                                gvDynamic.Rows[i].Cells[j].BackColor = (Color)ccv.ConvertFromString(color);
                                                objTblPre.GetContrastColor((Color)gvDynamic.Rows[i].Cells[j].BackColor);
                                                gvDynamic.Rows[i].Cells[j].ForeColor = FontColor;
                                                if (double.Parse(gvDynamic.Rows[i].Cells[j].Text) > 200)
                                                    gvDynamic.Rows[i].Cells[j].CssClass = SMALL_FONT;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        HtmlGenericControl div = new HtmlGenericControl("div");
                        div.Attributes.Add("Class", "divOuter");
                        HtmlGenericControl div1 = new HtmlGenericControl("div");
                        div1.ID = "div_" + gvDynamic.ID;
                        /* IM01417008 - INF-GLOB-SPTL2 request - Jerrey - Begin */
                        div1.Attributes.Add("Class", " alignTableToChart ");
                        /* IM01417008 - INF-GLOB-SPTL2 request - Jerrey - Begin */
                        if (bool.Parse(feedback))
                            div.Controls.Add(divOuter);
                        gvDynamic.CssClass = TABLESGAP;
                        
                        /* SOCB - Alignment Issue - Jerrey - SOCB*/
                        /*IM01170843- New Agricast - embeded JS - vew daily table columns width - BEGIN*/
                        //Set the below calculated styles in css class

                        gvDynamic.CssClass += " alignToChart ";
                        gvDynamic.CssClass += string.Format("tblWidth_{0}", numOfDays);

                        /*IM01170843- New Agricast - embeded JS - vew daily table columns width - END*/
                        /* EOCB - Alignment Issue - Jerrey - EOCB*/
                        div.Controls.Add(div1);
                        div1.Controls.Add(gvDynamic);
                        ph1.Controls.Add(div);

                    }

                    else
                    {
                        //history data - tbldaysrows -  non aggregated data
                        int k = 0;
                        //reapeat for each day
                        while (k < numOfDays)
                        {
                            gvDynamic = new GridView();
                            gvDynamic.RowDataBound += new GridViewRowEventHandler(gvDynamic_RowDataBound);
                            Label lbldate = new Label();
                            if (start > end)
                            {
                                int tmp = start;
                                start = end;
                                end = tmp;
                            }

                            lbldate.Text = (DateTime.Today.AddDays(start).AddDays((double)k)).ToString("D", VariantCulture);
                            lbldate.CssClass = LABEL250;
                            HtmlGenericControl divOuter = new HtmlGenericControl("div");
                            if (bool.Parse(feedback))
                            {
                                Label lblRating = new Label();
                                lblRating.Text = objTblPre.getTranslatedText(RATE_IT, CultureCode);
                                lblRating.CssClass = "label110";
                                HtmlGenericControl divRating = new HtmlGenericControl("div");
                                divRating.ID = "rating_" + (DateTime.Today.AddDays(start).AddDays((double)k)).ToShortDateString();
                                divRating.Attributes.Add("class", "RateIt");
                                divRating.Attributes.Add("data-accesskey", "rate");
                                divRating.ClientIDMode = ClientIDMode.Static;
                                divRating.Attributes.Add("data-rateit-backingfld", "select#range");

                                divOuter.Attributes.Add("class", "Right");
                                divOuter.Controls.Add(lblRating);
                                divOuter.Controls.Add(divRating);
                            }
                            gvDynamic.ID = "gv_" + DateTime.Today.AddDays(start).AddDays((double)k).DayOfWeek + DateTime.Today.AddDays(start).AddDays((double)k).Day;
                            gvDynamic.ClientIDMode = ClientIDMode.Static;
                            divOuter.ID = "ratingContainer_" + gvDynamic.ID;
                            objTblPre.GenerateTransposedtblSeriesRows(dtByDays, k, aggregation, false, step);
                            gvDynamic.DataBind();



                            string seriesPallette = "";

                            for (int i = 0; i < gvDynamic.Rows.Count; i++)
                            {
                                string serieName = gvDynamic.Rows[i].Cells[0].Text.ToString();
                                for (int j = 0; j <= 24; j++)
                                {
                                    if (j == 0)
                                    {
                                        gvDynamic.Rows[i].Cells[0].Text = objTblPre.getTranslatedText(alSeries[i].ToString().Split(',')[3], CultureCode); //gvDynamic.Rows[i].Cells[j].Text, CultureCode);

                                    }

                                    if (j > 0)
                                    {
                                        foreach (string ser in alSeries)
                                        {
                                            if (ser.Split(',')[0].ToString() == serieName)
                                            {
                                                seriesPallette = ser.Split(',')[1].ToString();
                                                break;
                                            }

                                        }
                                        if (seriesPallette.ToLower() != "none")
                                        {
                                            seriesPallette = objTblPre.checkUnitPalette(seriesPallette);
                                            objTblPre.createPallette(seriesPallette);
                                            if (gvDynamic.Rows[i].Cells[j].Text.ToString() != "" && gvDynamic.Rows[i].Cells[j].Text.ToString() != "&nbsp;")
                                            {
                                                double val;
                                                bool flag = double.TryParse(gvDynamic.Rows[i].Cells[j].Text, out val) ? true : false;
                                                if (flag)
                                                {
                                                    string color = objPm.getColor(val, gvDynamic.Rows[i].Cells[0].Text.ToString());
                                                    System.Drawing.ColorConverter ccv = new System.Drawing.ColorConverter();
                                                    gvDynamic.Rows[i].Cells[j].BackColor = (Color)ccv.ConvertFromString(color);
                                                    objTblPre.GetContrastColor((Color)gvDynamic.Rows[i].Cells[j].BackColor);
                                                    gvDynamic.Rows[i].Cells[j].ForeColor = FontColor;
                                                    if (double.Parse(gvDynamic.Rows[i].Cells[j].Text) > 200)
                                                        gvDynamic.Rows[i].Cells[j].CssClass = SMALL_FONT;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            k++;
                            HtmlGenericControl div1 = new HtmlGenericControl("div");
                            HtmlGenericControl div = new HtmlGenericControl("div");
                            div.Attributes.Add("Class", "divOuter");
                            div1.ID = "div_" + gvDynamic.ID;

                            gvDynamic.CssClass = TABLES;

                            if (gvDynamic.Rows.Count > 0)
                            {
                                if (bool.Parse(collapsible))
                                {
                                    System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                                    img.ImageUrl = BOXMINUS;
                                    img.ID = "img1_" + gvDynamic.ID;
                                    img.ClientIDMode = ClientIDMode.Static;
                                    div1.Controls.Add(img);
                                }
                                div1.Controls.Add(lbldate);
                                if (bool.Parse(feedback))
                                    div.Controls.Add(divOuter);
                            }

                            div1.Controls.Add(gvDynamic);
                            div.Controls.Add(div1);
                            ph1.Controls.Add(div);
                        }

                    }
                }
                foreach (DataRow dr in dtTableLegends.Rows)
                {
                    StringBuilder sb1;
                    StringBuilder sb;
                    Label lblLegend = new Label();
                    lblLegend.Text = objTblPre.getTranslatedText(dr[2].ToString(), CultureCode);
                    lblLegend.CssClass = LABEL250;
                    lblLegend.ID = "lblLegend_" + dr[0].ToString();
                    StringBuilder sbBody = new StringBuilder();
                    string legendPath = dr[1].ToString();
                    using (StreamReader sr = new StreamReader(HttpRuntime.AppDomainAppPath + legendPath))
                    {
                        String line;
                        // Read and display lines from the file until the end of  the file is reached.
                        while ((line = sr.ReadLine()) != null)
                        {

                            sbBody.Append(line);

                        }
                        sb1 = new StringBuilder(sbBody.ToString().Substring(sbBody.ToString().IndexOf("<div"), (sbBody.ToString().IndexOf("</div>") - sbBody.ToString().IndexOf("<div") + 6)));
                        string strLegend = sb1.ToString();
                        string startDelimiter = ConfigurationManager.AppSettings["startDelimiter"] != null && ConfigurationManager.AppSettings["startDelimiter"] != string.Empty ? ConfigurationManager.AppSettings["startDelimiter"] : "{";
                        string endDelimiter = ConfigurationManager.AppSettings["endDelimiter"] != null && ConfigurationManager.AppSettings["endDelimiter"] != string.Empty ? ConfigurationManager.AppSettings["endDelimiter"] : "}";
                        while (strLegend.IndexOf(endDelimiter) != -1)
                        {
                            int startDelit = strLegend.IndexOf(startDelimiter);
                            int endDelit = strLegend.IndexOf(endDelimiter);

                            string text = strLegend.Substring(startDelit, endDelit - startDelit + startDelimiter.Count());
                            if (text != null)
                            {
                                string translatedText = objTblPre.getTranslatedText(text.Substring(startDelimiter.Count(), text.Length - (endDelimiter.Count() + startDelimiter.Count())), strCulCode);
                                strLegend = strLegend.Remove(startDelit) + translatedText + strLegend.Substring(endDelit + endDelimiter.Count());
                            }

                        }
                        sb = new StringBuilder(strLegend);
                    }

                    LiteralControl literal = new LiteralControl(sb.ToString());
                    literal.ID = "legend_" + dr[0].ToString() + name;

                    HtmlGenericControl div1 = new HtmlGenericControl("div");
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.Attributes.Add("Class", "divOuter");
                    HtmlGenericControl divbind = new HtmlGenericControl("div");
                    div1.ID = "divLegend_" + dr[0].ToString() + name;
                    div1.ClientIDMode = ClientIDMode.Static;
                    divbind.Attributes.Add("Class", "LegendPadding");
                    if (bool.Parse(collapsible))
                    {

                        System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                        img.ImageUrl = BOXMINUS;
                        img.ID = "img1_" + dr[0].ToString() + name;
                        img.ClientIDMode = ClientIDMode.Static;
                        divbind.Controls.Add(img);
                    }

                    divbind.Controls.Add(lblLegend);
                    div1.Controls.Add(literal);
                    divbind.Controls.Add(div1);
                    div.Controls.Add(divbind);
                    phLegend.Controls.Add(div);
                    //phLegend.Controls.Add(div1);


                }



            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.TAB_DISPLAY_FAILURE) + ":" + ex.Message.ToString();
            }
        }
        public void DisplayLegend(string legend, string gvid)
        {
            try
            {

                foreach (DataRow dr in dtTableLegends.Rows)
                {
                    if (dr[0].ToString() == legend)
                    {
                        Label lblLegend = new Label();
                        lblLegend.Text = objTblPre.getTranslatedText(dr[2].ToString(), CultureCode);
                        lblLegend.CssClass = LABEL250;
                        lblLegend.ID = "lbl_" + gvid;
                        StringBuilder sbBody = new StringBuilder();
                        StringBuilder sb1;
                        StringBuilder sb;
                        string legendPath = dr[1].ToString();
                        using (StreamReader sr = new StreamReader(HttpRuntime.AppDomainAppPath + legendPath))
                        {
                            String line;
                            // Read and display lines from the file until the end of  the file is reached.
                            while ((line = sr.ReadLine()) != null)
                            {

                                sbBody.Append(line);

                            }
                            sb1 = new StringBuilder(sbBody.ToString().Substring(sbBody.ToString().IndexOf("<div"), (sbBody.ToString().IndexOf("</div>") - sbBody.ToString().IndexOf("<div") + 6)));

                            string strLegend = sb1.ToString();
                            string startDelimiter = ConfigurationManager.AppSettings["startDelimiter"] != null && ConfigurationManager.AppSettings["startDelimiter"] != string.Empty ? ConfigurationManager.AppSettings["startDelimiter"] : "{";
                            string endDelimiter = ConfigurationManager.AppSettings["endDelimiter"] != null && ConfigurationManager.AppSettings["endDelimiter"] != string.Empty ? ConfigurationManager.AppSettings["endDelimiter"] : "}";
                            while (strLegend.IndexOf(endDelimiter) != -1)
                            {
                                int startDelit = strLegend.IndexOf(startDelimiter);
                                int endDelit = strLegend.IndexOf(endDelimiter);

                                string text = strLegend.Substring(startDelit, endDelit - startDelit + startDelimiter.Count());
                                if (text != null)
                                {
                                    string translatedText = objTblPre.getTranslatedText(text.Substring(startDelimiter.Count(), text.Length - (endDelimiter.Count() + startDelimiter.Count())), strCulCode);
                                    strLegend = strLegend.Remove(startDelit) + translatedText + strLegend.Substring(endDelit + endDelimiter.Count());
                                }

                            }
                            sb = new StringBuilder(strLegend);
                        }

                        HtmlGenericControl div1 = new HtmlGenericControl("div");
                        div1.Attributes.Add("Class", "divOuter");
                        div1.ID = "divL_" + gvid;
                        LiteralControl literal = new LiteralControl(sb.ToString());
                        literal.ID = "legend_" + gvid;
                        ph1.Controls.Add(lblLegend);
                        div1.Controls.Add(literal);
                        ph1.Controls.Add(div1);
                    }

                }

            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                //same as Default page display legend error
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.DEF_DISPLAYLEGENDS_FAILURE) + " : " + ex.Message.ToString();
            }
        }
        public string GetRestriction(string restriction)
        {
            try
            {
                return objTblPre.getTranslatedText(restriction, strCulCode);
            }

            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.TRANSLATION_FAILURE) + " : " + ex.Message.ToString();
                return "";
            }
        }
        /*  add column width - Agricast CRs - Jerrey Begin */
        protected void AddMergedCells(GridViewRow objgridviewrow, TableCell objtablecell, int colspan, string celltext, string backcolor)
        {
            AddMergedCells(objgridviewrow, objtablecell, colspan, celltext, backcolor, 0);
        }

        protected void AddMergedCells(GridViewRow objgridviewrow, TableCell objtablecell, int colspan, string celltext, string backcolor, int colWidth)
        {
            try
            {
                objtablecell = new TableCell();

                /*IM01170843- New Agricast - embeded JS - vew daily table columns width - BEGIN*/
                //Set the below calculated styles in css class
                //Manually calculate the values and add it in css files . 


                objtablecell.Text = celltext;
                objtablecell.Style.Add("background-color", backcolor);
                objtablecell.ColumnSpan = colspan;
                //objtablecell.HorizontalAlign = HorizontalAlign.Center;
                /*  add column width - Agricast CRs - Jerrey Begin */
                ////if (colWidth > 0)
                ////    objtablecell.Style.Add("width", colWidth + "px");

                /*  add column width - Agricast CRs - Jerrey End*/
                //cell text will be null for first column of the header
                if (!string.IsNullOrEmpty(celltext))
                {
                    objtablecell.CssClass = string.Format("tblSeriesHeaderStyles_{0}", numOfDays);
                }
                objgridviewrow.Cells.Add(objtablecell);

            }
            catch (Exception ex)
            {
                AgriCastException currEx = new AgriCastException(objSvcPre.GetServiceDetails(), ex);
                AgriCastLogger.Publish(currEx, AgriCastLogger.LogType.Error);
                HttpContext.Current.Session["ErrorMessage"] = objCommonUtil.getTransText(Constants.TAB_CELLSMERGE_FAILURE) + " : " + ex.Message.ToString();
            }
        }
        /*  add column width - Agricast CRs - Jerrey End*/

        public string Text_to_Image(string strtxt, string fontshape, int fontsize, string Forecolor, Color bgcolor)
        {
            //creating bitmap image
            // Bitmap bitmap = new Bitmap(1, 1);

            // //FromImage method creates a new Graphics from the specified Image.
            // Graphics gImage = Graphics.FromImage(bitmap);
            // // Create the Font object for the image text drawing.
            // Font font = new Font(fontshape, fontsize);
            // // Instantiating object of Bitmap image again with the correct size for the text and font.
            // SizeF mystringSize = gImage.MeasureString(strtxt, font);
            // bitmap = new Bitmap(bitmap, (int)mystringSize.Width, (int)mystringSize.Height);
            // gImage = Graphics.FromImage(bitmap);

            // /* It can also be a way
            //bitmap = new Bitmap(bitmap, new Size((int)gImage.MeasureString(strtxt, font).Width, (int)gImage.MeasureString(strtxt, font).Height));*/

            // //Draw Specified text with specified format 
            // gImage.DrawString(strtxt, font, Brushes.Red, 0, 0);
            // font.Dispose();
            // gImage.Flush();
            // gImage.Dispose();
            // string imgpath = HttpRuntime.AppDomainAppPath + @"temp\tempbmp" + System.Diagnostics.Stopwatch.GetTimestamp() + ".jpeg";
            // bitmap.Save(imgpath, System.Drawing.Imaging.ImageFormat.Jpeg);
            // //return bitmap;     //return Bitmap Image 
            // return imgpath;

            Bitmap bmap;
            Graphics objGraphics;
            bool bold = true;
            bmap = new Bitmap(1, 1);
            objGraphics = Graphics.FromImage(bmap);
            float luminance = (float)(0.299 * bgcolor.R + 0.587 * bgcolor.G + 0.114 * bgcolor.B);
            // luminance = (float)Math.Round(luminance, 0);
            // content = luminance.ToString(); // <-- enable this to see luminance value in chart
            // bgcolor = Color.FromArgb(luminance, luminance, luminance); <-- enable this for gray scale
            Brush brush = new SolidBrush(bgcolor);
            objGraphics.FillRectangle(brush, 0, 0, bmap.Width, bmap.Height);
            Font font = new Font("Arial", 8, bold ? FontStyle.Bold : FontStyle.Regular);
            brush = new SolidBrush(Color.Black);
            if (luminance <= 128) brush = new SolidBrush(Color.White);
            int fh = (int)font.GetHeight();
            //int fw = getPixelLength(content) + 1;
            objGraphics.DrawString(strtxt, font, brush, (bmap.Width) / 2, (bmap.Height) / 2);
            // Innobit.Fwk.Image.ImageUtil.SaveGIFWithNewColorTable(bmap, fullfilepath, false);
            // File.SetLastWriteTime(fullfilepath, DateTime.Now.AddMonths(1));
            string imgpath = HttpRuntime.AppDomainAppPath + @"temp\tempbmp" + System.Diagnostics.Stopwatch.GetTimestamp() + ".jpeg";
            bmap.Save(imgpath, System.Drawing.Imaging.ImageFormat.Jpeg);
            // //return bitmap;     //return Bitmap Image 
            // return imgpath;
            bmap.Dispose();
            objGraphics.Dispose();
            return imgpath;
        }
        private bool IsEmpty<T>(dynamic obj)
        {
            object property_value = null;

            bool flag = false;

            System.Reflection.PropertyInfo[] properties_info = typeof(T).GetProperties();
            //System.Reflection.PropertyInfo property_info = default(System.Reflection.PropertyInfo); 

            foreach (System.Reflection.PropertyInfo prop in properties_info)
                if (prop != null)
                {
                    property_value = prop.GetValue(obj, null);
                    switch (prop.PropertyType.Name.ToString().ToLower())
                    {
                        case "int32": if (property_value != null)
                                flag = Int32.Parse(property_value.ToString()) == 0;
                            break;
                        case "double": if (property_value != null)
                                flag = double.Parse(property_value.ToString()) == 0.0;
                            break;
                        case "string": if (property_value != null)
                                flag = property_value.ToString() == "";
                            break;
                        case "datetime": if (property_value != null)
                                flag = DateTime.Parse(property_value.ToString()) == DateTime.MinValue || DateTime.Parse(property_value.ToString()) == DateTime.MaxValue;
                            break;
                    }
                }

            return flag;
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
                gvDynamic.DataSource = dtTransposed;
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

    }
}